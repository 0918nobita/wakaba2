module Fable.SQLite3.QueryBuilder

open System.Text

type Marker = interface end

type Table = Table of name : string

type ColsPattern = ColsPattern of string

type SelectOrder =
    | Asc
    | Desc
    override this.ToString() =
        match this with
        | Asc -> "ASC"
        | Desc -> "DESC"

type OrderByClauseItem =
    | OrderByClauseItem of col : string * SelectOrder
    override this.ToString() =
        match this with
        | OrderByClauseItem(col, order) -> col + " " + string order

type OrderByClause =
    | OrderByClause of OrderByClauseItem list
    override this.ToString() =
        match this with
        | OrderByClause [] -> ""
        | OrderByClause items ->
            let itemsStr =
                items
                |> List.map (fun item -> string item)
                |> String.concat ", "
            "ORDER BY " + itemsStr

type WhereClause =
    | WhereClause of string
    override this.ToString() =
        match this with
        | WhereClause cond -> "WHERE " + cond

type SelectStmt = Select of table : string * cols : string * OrderByClause * WhereClause option

let select (ColsPattern cols) (Table table) : SelectStmt =
    Select(table, cols, OrderByClause [], None)

let orderBy
        (col : string)
        (order : SelectOrder)
        (Select(table, cols, (OrderByClause items), where)) : SelectStmt =
    Select(
        table,
        cols,
        OrderByClause(items @ [OrderByClauseItem(col, order)]),
        where)

let where (cond : string) (Select(table, cols, order, _)) : SelectStmt =
    Select(table, cols, order, Some(WhereClause cond))

let private combine (strs : string list) : string =
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

let build (stmt : SelectStmt) : string =
    match stmt with
    | Select(table, cols, order, whereOpt) ->
        [
            "SELECT " + cols + " FROM " + table
            string order
            whereOpt
            |> Option.map string
            |> Option.defaultValue ""
        ]
        |> combine
