module Connection.Tests

open Expecto
open Swate.Api
open FsHttp

[<Tests>]
let connectionTests =
    testList "Connection.Tests" [
        testCase "Api.getTestNumber" <| fun _ ->
            // Could be used as connection test.
            let getTestNumber =
                let api = Static.OntologyApi + "getTestNumber" 
                http {
                    GET api
                    CacheControl "no-cache"
                }
                |> Request.send
                |> Response.deserializeJson<int>

            Expect.equal getTestNumber 42 "MUST return 42 if server is up and running and api is accessible."  
]