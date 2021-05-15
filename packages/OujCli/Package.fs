module Package

open Fable.Core

type private IExports =
    abstract version : string

[<ImportDefault("../../package.json")>]
let private package: IExports = jsNative

let version = package.version
