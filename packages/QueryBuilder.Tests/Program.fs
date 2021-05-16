module Program

open System.Diagnostics
open System.Text

let private combine1 (strs : string list) : string =
    let folder (sb : StringBuilder) (s : string) =
        if sb.Length = 0
        then
            sb.Append s
        else
            if s = ""
            then sb
            else (sb.Append " ").Append s
    strs
    |> List.fold folder (StringBuilder())
    |> string

let private combine2 (strs : string list) : string =
    strs
    |> List.filter ((<>) "")
    |> String.concat " "

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

let _user = {
    Get = fun (Message (user, _)) -> user
    Set = fun (Message (_, msg)) newUser -> Message (newUser, msg)
}

[<EntryPoint>]
let main _ =
    let user1 = User (12, "Alice")
    let msg1 = Message (user1, "Hello, world!")

    printfn "%A" <| _id.Get user1 // => 12
    printfn "%A" <| _name.Get user1 // => Alice
    printfn "%A" <| _name.Set user1 "Tom" // => User (12, "Tom")
    printfn "%A" <| _name.Modify user1 (fun str -> str.ToUpper()) // => User (12, "ALICE")
    printfn "%A" <| (_user ^|-> _name).Get msg1 // => "Alice"
    printfn "%A" <| (_user ^|-> _name).Set msg1 "Bob" // => Message (User (12, "Bob"), "Hello, world!")

    let list = [""; "A"; ""; "B"; "C"; ""]
    let numOfTimes = 100000

    measureExecutionTime numOfTimes (fun () -> combine1 list)
    |> printfn "combine1: %-3.0fns"

    measureExecutionTime numOfTimes (fun () -> combine2 list)
    |> printfn "combine2: %-3.0fns"

    0
