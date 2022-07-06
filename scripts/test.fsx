#r "nuget: FsHttp"

open System.IO
open FsHttp

let returnError (response: Response) = 
    sprintf "Statuscode: %A. %A" response.statusCode <| response.originalHttpResponseMessage.Content.ReadAsStringAsync().Result 

[<Literal>]
let Basepath = "https://swate.nfdi4plants.de/"

[<Literal>]
let OntologyApi = Basepath + "api/IOntologyAPIv1/"

// Could be used as connection test.
let getTestNumber () =
    let api = OntologyApi + "getTestNumber" 
    http {
        GET api
        CacheControl "no-cache"
    }
    |> Request.send
    |> Response.deserializeJson<int>

getTestNumber()

[<AutoOpen>]
module Types =
    type Ontology = {
        Name            : string
        CurrentVersion  : string
        DateCreated     : System.DateTime
        UserID          : string
    }

    type Term = {
        OntologyName    : string
        Accession       : string
        Name            : string
        Definition      : string
        XRefValueType   : string option
        IsObsolete      : bool
    }

type Ontology with

    /// <summary>This function connects to the Swate API and returns all Ontologies found in the database.</summary>
    /// <param name="errorHandler">_Optional:_ Can be used to change the output message in case the api request returns an error.</param>
    /// <returns>Array of Ontology</returns>
    static member GetAll(?errorHandler: Response -> string) =
        let api = OntologyApi + "getAllOntologies" 
        http {
            GET api
            CacheControl "no-cache"
        }
        |> Request.send
        |> fun response -> 
            if response.statusCode = System.Net.HttpStatusCode.OK then
                response |> Response.deserializeJson<Ontology []>
            else
                if errorHandler.IsSome then errorHandler.Value response else returnError response
                |> failwith


Ontology.GetAll()

type Term with

    /// <summary>This function connects to the Swate API and returns 'n' terms fitting best to the searchstring.</summary>
    /// <param name="searchString">The string to search for in Term.Name.</param>
    /// <param name="n">The number of terms to return.</param>
    /// <param name="errorHandler">_Optional:_ Can be used to change the output message in case the api request returns an error.</param>
    /// <returns>Array of Terms fitting the searchString. Maximal 'n', can be less if no more Terms scored high enough.</returns>
    static member Search(searchString: string, n: int, ?errorHandler:Response -> string) =
        let api = OntologyApi + "getTermSuggestions" 
        http {
            POST api
            body
            json $"""
            [[ {n}, "{searchString}" ]]
            """
        }
        |> Request.send
        |> fun response -> 
            if response.statusCode = System.Net.HttpStatusCode.OK then
                response |> Response.deserializeJson<Term []>
            else
                if errorHandler.IsSome then errorHandler.Value response else returnError response
                |> failwith


Term.Search("instrument", 5)