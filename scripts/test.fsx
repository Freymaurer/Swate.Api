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

// getTestNumber()

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

    type TermMinimal = {
        /// This is the Ontology Term Name
        Name            : string
        /// This is the Ontology Term Accession 'XX:aaaaaa'
        TermAccession   : string
    } with
        static member create name accession = {
            Name            = name
            TermAccession   = accession
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


// Ontology.GetAll()

type Term with

    /// <summary>This function connects to the Swate API, searches Term.Name by `searchString` and returns 'n' terms.</summary>
    /// <param name="searchString">The string to search for in Term.Name.</param>
    /// <param name="n">The number of terms to return.</param>
    /// <param name="errorHandler">_Optional:_ Can be used to change the output message in case the api request returns an error.</param>
    /// <returns>Array of Terms. Maximal 'n', can be less.</returns>
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

    /// <summary>This function connects to the Swate API, searches Term.Name by `searchString` and returns 'n' terms. Only Terms with a directed relationship towards `parentTerm` are searched.</summary>
    /// <param name="searchString">The string to search for in Term.Name.</param>
    /// <param name="n">The number of terms to return.</param>
    /// <param name="parentTerm">The parent term to search for, as `TermMinimal`.</param>
    /// <param name="errorHandler">_Optional:_ Can be used to change the output message in case the api request returns an error.</param>
    /// <returns>Array of child terms. Maximal 'n', can be less.</returns>
    static member SearchByParent(searchString: string, n: int, parentTerm: TermMinimal, ?errorHandler:Response -> string) =
        let api = OntologyApi + "getTermSuggestionsByParentTerm" 
        http {
            POST api
            body
            json $"""
            [[ {n}, "{searchString}", {{ "Name": "{parentTerm.Name}", "TermAccession": "{parentTerm.TermAccession}" }} ]]
            """
        }
        |> Request.send
        |> fun response -> 
            if response.statusCode = System.Net.HttpStatusCode.OK then
                response |> Response.deserializeJson<Term []>
            else
                if errorHandler.IsSome then errorHandler.Value response else returnError response
                |> failwith

    /// <summary>This function connects to the Swate API, searches Term.Name by `searchString` and returns 'n' terms. Only terms with directed relationships from `childterm` towards them are searched.</summary>
    static member SearchByChild(searchString: string, n: int, childTerm: TermMinimal, ?errorHandler:Response -> string) =
        let api = OntologyApi + "getTermSuggestionsByChildTerm" 
        http {
            POST api
            body
            json $"""
            [[ {n}, "{searchString}", {{ "Name": "{childTerm.Name}", "TermAccession": "{childTerm.TermAccession}" }} ]]
            """
        }
        |> Request.send
        |> fun response -> 
            if response.statusCode = System.Net.HttpStatusCode.OK then
                response |> Response.deserializeJson<Term []>
            else
                if errorHandler.IsSome then errorHandler.Value response else returnError response
                |> failwith

    /// <summary>This function connects to the Swate API and returns all Terms found in the database, with a directed relationship towards `parentTerm`</summary>
    /// <param name="parentTerm">The parent term to search for, as `TermMinimal`.</param>
    /// <param name="errorHandler">_Optional:_ Can be used to change the output message in case the api request returns an error.</param>
    /// <returns>All child terms. Can be 0.</returns>
    static member GetAllByParent(parentTerm: TermMinimal, ?errorHandler:Response -> string) =
        let api = OntologyApi + "getAllTermsByParentTerm" 
        http {
            POST api
            body
            json $"""
            [ {{ "Name": "{parentTerm.Name}", "TermAccession": "{parentTerm.TermAccession}" }} ]
            """
        }
        |> Request.send
        |> fun response -> 
            if response.statusCode = System.Net.HttpStatusCode.OK then
                response |> Response.deserializeJson<Term []>
            else
                if errorHandler.IsSome then errorHandler.Value response else returnError response
                |> failwith

    /// <summary>This function connects to the Swate API and returns all Terms found in the database, with a directed relationship from `childTerm`.</summary>
    /// <param name="childTerm">Only terms with directed relationships from `childterm` towards them are searched`.</param>
    /// <param name="errorHandler">_Optional:_ Can be used to change the output message in case the api request returns an error.</param>
    /// <returns>All parent terms. Can be 0.</returns>
    static member GetAllByChild(childTerm: TermMinimal, ?errorHandler:Response -> string) =
        let api = OntologyApi + "getAllTermsByChildTerm" 
        http {
            POST api
            body
            json $"""
            [ {{ "Name": "{childTerm.Name}", "TermAccession": "{childTerm.TermAccession}" }} ]
            """
        }
        |> Request.send
        |> fun response -> 
            if response.statusCode = System.Net.HttpStatusCode.OK then
                response |> Response.deserializeJson<Term []>
            else
                if errorHandler.IsSome then errorHandler.Value response else returnError response
                |> failwith

// Term.Search("instrument", 5)
// Term.SearchByParent("SCIEX", 5, TermMinimal.create "instrument model" "MS:1000031")
// Term.SearchByChild("Proteomics Standards", 5, TermMinimal.create "MALDI Synapt MS" "MS:1001776")
// Term.GetAllByParent(TermMinimal.create "instrument model" "MS:1000031")
// Term.GetAllByChild(TermMinimal.create "MALDI Synapt MS" "MS:1001776")
