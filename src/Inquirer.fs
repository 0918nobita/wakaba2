module Inquirer

open Fable.Core
open Fable.Core.JsInterop

[<ImportMember("inquirer")>]
let private prompt: obj -> JS.Promise<obj> = jsNative

let inputQuestion (name : string) (message : string option) : JS.Promise<string> =
    promise {
        let! result =
            seq {
                yield ("type", "input" :> obj)
                yield ("name", name :> obj)
                match message with
                | Some(text) -> yield ("message", text :> obj)
                | None -> ()
            }
            |> createObj
            |> prompt
        return result?(name)
    }

let passwordQuestion (name : string) (message : string option) : JS.Promise<string> =
    promise {
        let! result =
            seq {
                yield ("type", "password" :> obj)
                yield ("name", name :> obj)
                match message with
                | Some(text) -> yield ("message", text :> obj)
                | None -> ()
            }
            |> createObj
            |> prompt
        return result?(name)
    }
