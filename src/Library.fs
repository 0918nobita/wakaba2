module Wakaba2

open Fable.Core
open Fable.Core.JsInterop
open Browser

let [<Global>] navigator: obj = jsNative

let () =
    console.log("Hello from F#!")
    console.log(navigator?userAgent)
