module QueryBuilder.Tests

open NUnit.Framework
open Fable.SQLite3

module QB = QueryBuilder

[<Test>]
let Test1 () =
    Assert.AreEqual(
        QB.select (QB.Table "users") (QB.ColsPattern "*")
        |> QB.build,
        "SELECT * FROM users")
