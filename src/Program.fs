module Program

open Fable.Core
open Fable.Core.JsInterop
open Node

type IOujAccountInfo =
    abstract username : string
    abstract password : string

type IConfig =
    abstract executablePath : string
    abstract ouj : IOujAccountInfo

type Prompt = Inquirer.Wrapper

let init configFilePath =
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

type IResponse =
    abstract json: unit -> JS.Promise<obj>

let [<Global>] fetch: string -> JS.Promise<IResponse> = jsNative

let vod configFilePath =
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
        do! page.waitForNavigation !!{| waitUntil = Puppeteer.NetworkIdle0 |}
        let! data = page.evaluate(fun () ->
            (fetch "https://v.ouj.ac.jp/v1/tenants/1/categories")
                .``then``(fun (res: IResponse) -> res.json()))
        JS.console.log(data)
    }
    |> ignore

let reset configFilePath =
    fs.unlinkSync(U2.Case1 configFilePath)
    printfn "Done"

let dbTest () =
    let sqlite3 = Sqlite.verbose()

    use db = sqlite3.Database("test.sqlite3")

    db.Serialize(fun () ->
        db.Run("CREATE TABLE IF NOT EXISTS test(id INTEGER PRIMARY KEY AUTOINCREMENT, name TEXT)")

        using (db.Prepare("INSERT INTO test(name) VALUES(?)")) (fun stmt ->
            for i in 1..10 do
                stmt.Run([| sprintf "User %d" i |]))

        using (db.Prepare("UPDATE test SET name = ? WHERE id = ?")) (fun stmt ->
            stmt.Run([| "Alice"; "7" |]))
    )

module QB = QueryBuilder

let queryBuilderTest () =
    QB.select (QB.Table "users") (QB.ColsPattern "*")
    |> QB.orderBy "age" QB.Asc
    |> QB.orderBy "address" QB.Desc
    |> QB.build
    |> printfn "%s"

let () =
    let argv = ``process``.argv.ToArray()
    let configFilePath = path.join(__dirname, "../config.json")
    if Array.length argv >= 3
    then
        match argv.[2] with
        | "init" -> init configFilePath
        | "vod" -> vod configFilePath
        | "reset" -> reset configFilePath
        | "db" -> dbTest ()
        | "query" -> queryBuilderTest ()
        | cmd ->
            eprintfn "Unknown command: %s" cmd
            ``process``.exit 1
    else
        printfn "OUJ CLI v%s" Package.version
