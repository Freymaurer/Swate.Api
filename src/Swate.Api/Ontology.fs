namespace Swate.Api

[<AutoOpen>]
module Ontology =

    open FsHttp

    type Ontology with

        /// <summary>This function connects to the Swate API and returns all Ontologies found in the database.</summary>
        /// <param name="errorHandler">_Optional:_ Can be used to change the output message in case the api request returns an error.</param>
        /// <returns>Array of Ontology</returns>
        static member GetAll(?errorHandler: Response -> string) =
            let api = Static.OntologyApi + "getAllOntologies" 
            http {
                GET api
                CacheControl "no-cache"
            }
            |> Request.send
            |> fun response -> 
                if response.statusCode = System.Net.HttpStatusCode.OK then
                    response |> Response.deserializeJson<Ontology []>
                else
                    if errorHandler.IsSome then errorHandler.Value response else returnDefaultError response
                    |> failwith