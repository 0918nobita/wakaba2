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

let convertMsToNs (ms: float<ms>) : float<ns> =
    LanguagePrimitives.FloatWithMeasure (float ms * 1000000.0)

[<EntryPoint>]
let main _ =
    let list = [""; "A"; ""; "B"; "C"; ""]
    let numOfTimes = 100000
    let stopWatch = Stopwatch.StartNew()
    for _ in 1..numOfTimes do
        ignore <| combine1 list
    stopWatch.Stop()
    let elapsed: float<ms> =
        LanguagePrimitives.FloatWithMeasure (float stopWatch.ElapsedMilliseconds)
    elapsed / float numOfTimes
    |> convertMsToNs
    |> printfn "combine1: %-3.0fns"
    stopWatch.Restart()
    for _ in 1..numOfTimes do
        ignore <| combine2 list
    stopWatch.Stop()
    printfn
        "combine2: %-3.0fns"
        (float stopWatch.ElapsedMilliseconds / float numOfTimes * 1000000.0)
    0
