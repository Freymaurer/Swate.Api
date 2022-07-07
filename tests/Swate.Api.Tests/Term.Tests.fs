module Term.Tests

open Expecto
open Swate.Api

[<Tests>]
let termTests =
    testList "Term.Tests" [
        // instrument model ist my most used term for verification from `MS` ontology.
        testCase "Term.Search" <| fun _ ->
            let search = Term.Search("instrument mod", 10)
            let instrumentModel = search |> Array.find (fun x -> x.Accession = "MS:1000031")
            Expect.equal instrumentModel.Name "instrument model" "instrumentModel.Name check"  
            Expect.equal instrumentModel.OntologyName "ms" "instrumentModel.OntologyName check" 

        testCase "Term.SearchByParent" <| fun _ ->
            let search = Term.SearchByParent("SCIEX", 5, TermMinimal.create "instrument model" "MS:1000031")
            let sciexInstrumentModel = search |> Array.find (fun x -> x.Accession = "MS:1000121")
            Expect.equal sciexInstrumentModel.Name "SCIEX instrument model" "sciexInstrumentModel.Name check"  
            Expect.equal sciexInstrumentModel.OntologyName "ms" "sciexInstrumentModel.OntologyName check" 

        testCase "Term.SearchByChild" <| fun _ ->
            let search = Term.SearchByChild("Proteomics Standards", 5, TermMinimal.create "MALDI Synapt MS" "MS:1001776")
            let proteomicsStandard = search |> Array.find (fun x -> x.Accession = "MS:0000000")
            Expect.equal proteomicsStandard.Name "Proteomics Standards Initiative Mass Spectrometry Vocabularies" "proteomicsStandard.Name check"  
            Expect.equal proteomicsStandard.OntologyName "ms" "proteomicsStandard.OntologyName check" 

        testCase "Term.GetAllByParent" <| fun _ ->
            let search = Term.GetAllByParent(TermMinimal.create "instrument model" "MS:1000031")
            let resultLength = search.Length
            let sciexInstrumentModel = search |> Array.find (fun x -> x.Accession = "MS:1000121")
            Expect.equal sciexInstrumentModel.Name "SCIEX instrument model" "sciexInstrumentModel.Name check"  
            Expect.equal sciexInstrumentModel.OntologyName "ms" "sciexInstrumentModel.OntologyName check" 
            Expect.isTrue (resultLength > 300) "number of results check"

        testCase "Term.GetAllByChild" <| fun _ ->
            let search = Term.GetAllByChild(TermMinimal.create "MALDI Synapt MS" "MS:1001776")
            let resultLength = search.Length
            let proteomicsStandard = search |> Array.find (fun x -> x.Accession = "MS:0000000")
            Expect.equal proteomicsStandard.Name "Proteomics Standards Initiative Mass Spectrometry Vocabularies" "proteomicsStandard.Name check"  
            Expect.equal proteomicsStandard.OntologyName "ms" "proteomicsStandard.OntologyName check" 
            Expect.isTrue (resultLength > 1) "number of results check"
]
