module Sqlite

open Fable.Core

type IDatabase =
    abstract run: string -> unit

type ISqliteInstance =
    [<Emit("new $0.Database($1)")>]
    abstract Database: string -> IDatabase

type ISqliteModule =
    abstract verbose: unit -> ISqliteInstance

[<ImportDefault("sqlite3")>]
let private sqlite3: ISqliteModule = jsNative

let verbose = sqlite3.verbose
