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

[<EntryPoint>]
let main _ =
    let list = [""; "A"; ""; "B"; "C"; ""]
    let numOfTimes = 100000

    measureExecutionTime numOfTimes (fun () -> combine1 list)
    |> printfn "combine1: %-3.0fns"

    measureExecutionTime numOfTimes (fun () -> combine2 list)
    |> printfn "combine2: %-3.0fns"

    0
