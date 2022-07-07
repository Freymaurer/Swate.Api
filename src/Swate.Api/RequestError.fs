namespace Swate.Api

[<AutoOpen>]
module internal RequestError =

    open FsHttp

    let returnDefaultError (response: Response) = 
        sprintf "Statuscode: %A. %A" response.statusCode <| response.originalHttpResponseMessage.Content.ReadAsStringAsync().Result 

