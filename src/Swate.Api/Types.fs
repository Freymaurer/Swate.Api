namespace Swate.Api

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