module QueryBuilder.Tests

open NUnit.Framework
open Fable.SQLite3
open FsCheck

module QB = QueryBuilder

[<Test>]
let Test1 () =
    let combineMethodInfo =
        typeof<QueryBuilder.Marker>.DeclaringType
        |> ReflectionUtil.getPrivateStaticMethod "combine"
        |> Option.defaultWith
            (fun () -> failwith "Failed to get `combine` method info")

    let alphabeticString: Gen<string> =
        Gen.oneof [Gen.choose (65, 90); Gen.choose (97, 122)]
        |> Gen.map System.Convert.ToChar
        |> Gen.arrayOf
        |> Gen.map System.String

    let ``List of alphabetic or empty string``: Arbitrary<string list> =
        Gen.oneof
            [
                Gen.fresh (fun () -> Gen.eval 10 (Random.newSeed()) alphabeticString)
                Gen.constant ""
            ]
        |> Gen.listOf
        |> Arb.fromGen

    Prop.forAll
        ``List of alphabetic or empty string``
        (fun strs ->
            strs |> List.filter ((<>) "") |> String.concat " "
                = (combineMethodInfo.Invoke(null, [| strs |]) :?> string))
    |> Check.VerboseThrowOnFailure

[<Test>]
let Test2 () =
    Assert.AreEqual(
        "SELECT * FROM users",
        QB.Table "users"
        |> QB.select (QB.ColsPattern "*")
        |> QB.build)
