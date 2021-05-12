module Sqlite

open Fable.Core

[<AbstractClass>]
type IStatement =
    abstract finalize: unit -> unit

    [<Emit("$0.run($1...)")>]
    abstract run: string[] -> unit

type Statement(stmt: IStatement) =
    member _.Finalize = stmt.finalize
    member _.Run = stmt.run

    interface System.IDisposable with
        member this.Dispose() = this.Finalize()

[<AbstractClass>]
type IDatabase =
    abstract close: unit -> unit
    abstract prepare: string -> IStatement
    abstract run: string -> unit
    abstract serialize: (unit -> unit) -> unit

type Database(db: IDatabase) =
    member _.Close = db.close
    member _.Prepare(sql: string) = new Statement(db.prepare(sql))
    member _.Run = db.run
    member _.Serialize = db.serialize

    interface System.IDisposable with
        member this.Dispose() = this.Close()

type ISqliteInstance =
    [<Emit("new $0.Database($1)")>]
    abstract Database: string -> IDatabase

type SqliteInstance(sqlite: ISqliteInstance) =
    member _.Database(name: string) = new Database(sqlite.Database(name))

type ISqliteModule =
    abstract verbose: unit -> ISqliteInstance

[<ImportDefault("sqlite3")>]
let private sqlite3: ISqliteModule = jsNative

let verbose () = new SqliteInstance(sqlite3.verbose())
