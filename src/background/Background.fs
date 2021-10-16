module Background

open Fable.Core
open Browser

let [<Global>] browser: obj = jsNative

let () =
    console.log(browser)
