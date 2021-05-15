module ReflectionUtil

open System
open System.Reflection

let getPrivateStaticMethod
        (name : string)
        (moduleType : Type) : MethodInfo option =
    let method =
        moduleType
            .GetMethod(
                name,
                BindingFlags.Static ||| BindingFlags.NonPublic)
    if method <> null
    then Some method
    else None
