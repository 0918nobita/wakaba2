module Program

open Fable.Core
open Fable.Core.JsInterop
open Node

let argv = ``process``.argv.ToArray()

let configFilePath = path.join(__dirname, "../config.json")

type IOujAccountInfo =
    abstract username : string
    abstract password : string

type IConfig =
    abstract executablePath : string
    abstract ouj : IOujAccountInfo

type Prompt = Inquirer.Wrapper

let init () =
    let askExecutablePath () : JS.Promise<string> =
        let name = "executablePath"
        let message = "Path to the Chrome executable"
        let platform = ``process``.platform
        match platform with
        | Base.Platform.Darwin ->
            Prompt.InputQuestion(name, message, defaultValue = "/Applications/Google Chrome.app/Contents/MacOS/Google Chrome")
        | Base.Platform.Linux ->
            Prompt.InputQuestion(name, message, defaultValue = "/usr/bin/google-chrome")
        | _ ->
            Prompt.InputQuestion(name, message)

    promise {
        let! executablePath = askExecutablePath ()
        let! userName = Prompt.InputQuestion(name = "username")
        let! password = Prompt.PasswordQuestion(name = "password")
        fs.writeFileSync(configFilePath, Json.stringify (createObj [
            "executablePath" ==> executablePath
            "ouj" ==> createObj [
                "username" ==> userName
                "password" ==> password
            ]
        ]))
    }
    |> ignore

let vod () =
    if not (fs.existsSync(U2.Case1 configFilePath))
    then
        eprintf "Configuration file not found"
        eprintf "In order to generate one, please execute `ouj init`."
        ``process``.exit 1

    let config = Json.parse (fs.readFileSync(configFilePath)?toString()) :?> IConfig

    let setupPage (browser : Puppeteer.IBrowser) =
        promise {
            let! pages = browser.pages()
            let page = pages.[0]
            do! page.setUserAgent "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/69.0.3497.100"
            return page
        }

    promise {
        let! browser = Puppeteer.launch (createObj [
            "executablePath" ==> config.executablePath
            "headless" ==> false
        ])
        let! page = setupPage browser
        do! page.goto "https://v.ouj.ac.jp/view/ouj"
        let! button = page.waitForSelector ".login-button"
        do! button?click()
        do! page.waitForNavigation !!{| waitUntil = Puppeteer.NetworkIdle0 |}
        do! page.``type`` "input[name=\"username\"]" config.ouj.username
        do! page.``type`` "input[name=\"password\"]" config.ouj.password
        let! submitButton = page.waitForSelector ".btn-submit"
        do! submitButton?click()
        return! page.waitForNavigation !!{| waitUntil = Puppeteer.NetworkIdle0 |}
    }
    |> ignore

let () =
    if Array.length argv >= 3
    then
        match argv.[2] with
        | "init" -> init ()
        | "vod" -> vod ()
        | cmd ->
            eprintfn "Unknown command: %s" cmd
            ``process``.exit 1
    else
        printfn "OUJ CLI v%s" Package.version
