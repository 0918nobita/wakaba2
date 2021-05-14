module QueryBuilder.Tests

open NUnit.Framework
open Fable.SQLite3

module QB = QueryBuilder

[<Test>]
let Test1 () =
    let combineMethodInfo =
        typeof<QueryBuilder.Marker>.DeclaringType
        |> ReflectionUtil.getPrivateStaticMethod "combine"
        |> Option.defaultWith (fun () -> failwith "Failed to get `combine` method info")
    Assert.AreEqual(
        "A B C",
        combineMethodInfo.Invoke(null, [| ["A"; "B"; ""; "C"] |]))

[<Test>]
let Test2 () =
    Assert.AreEqual(
        "SELECT * FROM users",
        QB.select (QB.Table "users") (QB.ColsPattern "*")
        |> QB.build)
