module QueryBuilder.Tests

open System.Reflection
open NUnit.Framework
open Fable.SQLite3

module QB = QueryBuilder

[<Test>]
let Test1 () =
    let queryBuilderType = typeof<QueryBuilder.Marker>.DeclaringType
    let combineMethodInfo =
        queryBuilderType
            .GetMethod(
                "combine",
                BindingFlags.Static ||| BindingFlags.NonPublic)
    Assert.NotNull(combineMethodInfo)
    Assert.AreEqual(
        "A B C",
        combineMethodInfo.Invoke(null, [| ["A"; "B"; ""; "C"] |]))

[<Test>]
let Test2 () =
    Assert.AreEqual(
        "SELECT * FROM users",
        QB.select (QB.Table "users") (QB.ColsPattern "*")
        |> QB.build)
