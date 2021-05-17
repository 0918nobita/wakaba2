module Program

open System.Diagnostics
open FSharp.Reflection

type Getter<'s, 'a> = 's -> 'a
type Setter<'s, 'a> = 's -> 'a -> 's

type Lens<'s, 'a> =
    {
        Get : Getter<'s, 'a>
        Set : Setter<'s, 'a>
    }
    member this.Modify (s : 's) (f : 'a -> 'a) : 's =
        s |> this.Get |> f |> this.Set s
    member this.Compose (other : Lens<'a, 'b>) : Lens<'s, 'b> =
        {
            Get = fun s -> s |> this.Get |> other.Get
            Set = fun s b -> other.Set (this.Get s) b |> this.Set s
        }
    static member (^|->) (a : Lens<'s, 'a>, b : Lens<'a, 'b>) : Lens<'s, 'b> = a.Compose b

type User = User of id : int * name : string

let _id = {
    Get = fun (User (id, _)) -> id
    Set = fun (User (_, name)) newId -> User (newId, name)
}

let _name = {
    Get = fun (User (_, name)) -> name
    Set = fun (User (userId, _)) name -> User (userId, name)
}

type Message = Message of User * string

type DerivedLenses(typeInfo : System.Type) =
    let unionCaseInfo = Array.head <| FSharpType.GetUnionCases typeInfo
    let fields = unionCaseInfo.GetFields ()
    let fieldNames = fields |> Array.map (fun prop -> prop.Name)

    member _.TryGetLens name =
        if Array.contains name fieldNames
        then
            let index = fields |> Array.findIndex (fun field -> field.Name = name)
            let fieldType = (fields |> Array.find (fun field -> field.Name = name)).PropertyType
            let getterType = FSharpType.MakeFunctionType (typeInfo, fieldType)
            let getterValue = FSharpValue.MakeFunction (getterType, fun obj ->
                let unionFields = snd <| FSharpValue.GetUnionFields (obj, typeInfo)
                unionFields.[index])
            let setterReturnType = FSharpType.MakeFunctionType (fieldType, typeInfo);
            let setterType = FSharpType.MakeFunctionType (typeInfo, setterReturnType)
            let setterValue = FSharpValue.MakeFunction (setterType, fun obj1 ->
                FSharpValue.MakeFunction (
                    setterReturnType,
                    (fun obj2 ->
                        let unionFields = snd <| FSharpValue.GetUnionFields (obj1, typeInfo)
                        unionFields.[index] <- obj2
                        FSharpValue.MakeUnion (unionCaseInfo, unionFields))))
            let lensType = (typedefof<Lens<_,_>>.MakeGenericType [| typeInfo; fieldType |])
            let lensValue = FSharpValue.MakeRecord (lensType, [| getterValue; setterValue |])
            Some lensValue
        else None

let inline (?) (lenses : DerivedLenses) (prop : string) = lenses.TryGetLens prop

[<Measure>] type ms
[<Measure>] type ns

let private convertMsToNs (ms : float<ms>) : float<ns> =
    ms * 1000000.0<ns/ms>

let private measureExecutionTime (numOfTimes : int) (op : unit -> 'a) : float<ns> =
    let stopWatch = Stopwatch.StartNew()
    for _ in 1..numOfTimes do
        ignore <| op ()
    stopWatch.Stop()
    let elapsed = 1.0<ms> * float stopWatch.ElapsedMilliseconds
    elapsed / float numOfTimes
    |> convertMsToNs

[<EntryPoint>]
let main _ =
    let userLenses = DerivedLenses(typeof<User>)
    let _idDerived : Lens<User, int> = downcast Option.get userLenses?id
    let _nameDerived : Lens<User, string> = downcast Option.get userLenses?name

    let msgLenses = DerivedLenses(typeof<Message>)
    let _user : Lens<Message, User> = downcast Option.get msgLenses?Item1

    let user1 = User (12, "Alice")
    let msg1 = Message (user1, "Hello, world!")

    printfn "%A" <| _id.Get user1 // => 12
    printfn "%A" <| _name.Get user1 // => Alice
    printfn "%A" <| _name.Set user1 "Tom" // => User (12, "Tom")
    printfn "%A" <| _name.Modify user1 (fun str -> str.ToUpper()) // => User (12, "ALICE")
    printfn "%A" <| (_user ^|-> _name).Get msg1 // => "Alice"
    printfn "%A" <| (_user ^|-> _name).Set msg1 "Bob" // => Message (User (12, "Bob"), "Hello, world!")

    printfn "\n[Performance]"
    measureExecutionTime 100 (fun () -> _id.Get user1) |> printfn "_id.Get: %.0fns"
    measureExecutionTime 100 (fun () -> _idDerived.Get user1) |> printfn "_idDerived.Get: %.0fns\n"

    measureExecutionTime 100 (fun () -> _name.Set user1 "Tom") |> printfn "_name.Get: %.0fns"
    measureExecutionTime 100 (fun () -> _nameDerived.Set user1 "Tom") |> printfn "_nameDerived.Get: %.0fns"
    0
