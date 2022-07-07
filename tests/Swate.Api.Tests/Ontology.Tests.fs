module Ontology.Tests

open Expecto
open Swate.Api

[<Tests>]
let ontologyTests =
    testList "Ontology.Tests" [
        testCase "check Ontology.GetAll" <| fun _ ->
            let ontologies = Ontology.GetAll()
            let msOntology = ontologies |> Array.find (fun x -> x.Name = "ms")
            Expect.equal msOntology.Name "ms" "`MS` ontology is the most used ontology for testing. Rather small and should always be included in the database"  
]