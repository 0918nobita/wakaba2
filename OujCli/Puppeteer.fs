module Puppeteer

open Fable.Core

[<StringEnum>]
type LifeCycleEvent =
    | [<CompiledName("load")>] Load
    | [<CompiledName("domcontentloaded")>] DomContentLoaded
    | [<CompiledName("networkidle0")>] NetworkIdle0
    | [<CompiledName("networkidle2")>] NetworkIdle2

type IWaitForOptions =
    abstract timeout : int option with get, set
    abstract waitUntil : LifeCycleEvent option with get, set

type IPage =
    abstract evaluate : (unit -> 'a) -> JS.Promise<'b>
    abstract goto : url:string -> JS.Promise<unit>
    abstract setUserAgent : string -> JS.Promise<unit>
    abstract ``type`` : string -> string -> JS.Promise<unit>
    abstract waitForNavigation : ?options:IWaitForOptions -> JS.Promise<unit>
    abstract waitForSelector : string -> JS.Promise<obj>

type IBrowser =
    abstract close : unit -> JS.Promise<unit>
    abstract newPage : unit -> JS.Promise<IPage>
    abstract pages : unit -> JS.Promise<IPage[]>

type private IExports =
    abstract launch : obj -> JS.Promise<IBrowser>

[<ImportDefault("puppeteer-core")>]
let private puppeteer: IExports = jsNative

let launch = puppeteer.launch
