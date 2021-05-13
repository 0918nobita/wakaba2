module Inquirer

open Fable.Core
open Fable.Core.JsInterop

[<ImportMember("inquirer")>]
let private prompt: obj -> JS.Promise<obj> = jsNative

[<Sealed>]
type Wrapper private () =
    static member InputQuestion(name: string, ?message: string, ?defaultValue: string) : JS.Promise<string> =
        promise {
            let! result =
                seq {
                    yield ("type", "input" :> obj)
                    yield ("name", name :> obj)
                    match message with
                    | Some text -> yield ("message", text :> obj)
                    | None -> ()
                    match defaultValue with
                    | Some value -> yield ("default", value :> obj)
                    | None -> ()
                }
                |> createObj
                |> prompt
            return result?(name)
        }

    static member PasswordQuestion(name: string, ?message: string) : JS.Promise<string> =
        promise {
            let! result =
                seq {
                    yield ("type", "password" :> obj)
                    yield ("name", name :> obj)
                    match message with
                    | Some text -> yield ("message", text :> obj)
                    | None -> ()
                }
                |> createObj
                |> prompt
            return result?(name)
        }
