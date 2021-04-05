module Json

open Fable.Core

type private IExports =
    abstract stringify : obj -> string
    abstract parse : string -> obj

let [<Global>] private JSON: IExports = jsNative

let parse = JSON.parse

let stringify = JSON.stringify
