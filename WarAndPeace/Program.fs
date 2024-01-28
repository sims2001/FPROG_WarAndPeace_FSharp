module WarAndPeace

open System
open System.Collections.Generic

let readFileAsVector ( filePath : string ) : string list =
    IO.File.ReadLines(filePath) |> List.ofSeq

let tokenizeText (texts: string list) : string list =
    let tokenizeSingleLine (text : string) : string list =
        Text.RegularExpressions.Regex.Split(text, "[\s\p{P}]+")
        |> List.ofArray
        |> List.filter (fun word -> not (String.IsNullOrWhiteSpace word) )
        |> List.map (fun word -> word.ToLower())
    texts
    |> List.collect tokenizeSingleLine
    
let filterWords (words: string list) (filterList: string list) : string list =
    let isPrefixOfAny (word: string) (candidates: string list) =
        candidates
        |> List.exists (fun candidate -> word.StartsWith(candidate, StringComparison.OrdinalIgnoreCase))
    words
    |> List.filter (fun word -> isPrefixOfAny word filterList)
    
let countOccurrences (words : string list) : (string * int) list =
    words
    |> Seq.groupBy (fun word -> word)
    |> Seq.map (fun (key, values) -> key, Seq.length values)
    |> Seq.sortByDescending snd
    |> List.ofSeq

let calculateTermDensity (text: string list) (terms: string list) : float =
    let occurrences = countOccurrences terms
    let density = if (text |> List.length) > 0 then float (occurrences |> List.length ) / float ( text |> List.length ) else 0.0
    density

let splitChapters ( content : string list ) : string list list =
    let rec splitHelper lines acc currentChapter =
        match lines with
        | [] -> acc @ [currentChapter]
        | ( line : string ) :: rest ->
            if line.Contains("CHAPTER") && not (line.Contains("BOOK")) then
                let updatedAcc = if currentChapter <> [] then acc @ [currentChapter] else acc
                splitHelper rest updatedAcc [line]
            elif line.Contains("END OF THE PROJECT") then
                acc @ [currentChapter]
            else
                splitHelper rest acc (currentChapter @ [line])

    splitHelper content [] []

let removeChapterHeader (chapter: string list) : string list =
    match chapter with
    | header :: lines -> lines
    | [] -> []
    
let removeEmptyChapters (chapters: string list list) : string list list =
    chapters |> List.filter (fun chapter -> chapter <> [])

let processChapters (chapters: string list list) (warTerms: string list) (peaceTerms: string list) : float list * float list =
    let densitiesWar = new List<float>()
    let densitiesPeace = new List<float>()

    for chapter in chapters do
        let filteredWordsWar = filterWords chapter warTerms
        let filteredWordsPeace = filterWords chapter peaceTerms
        
        let densityCountWar = int ( filteredWordsWar |> List.length )
        let densityCountPeace = int ( filteredWordsPeace |> List.length )
        
        let densityWar = if densityCountWar = densityCountPeace then calculateTermDensity chapter warTerms else float densityCountWar
        let densityPeace = if densityCountPeace = densityCountWar then calculateTermDensity chapter peaceTerms else float densityCountPeace
        
        densitiesWar.Add(densityWar)
        densitiesPeace.Add(densityPeace)
        
    (densitiesWar |> List.ofSeq), (densitiesPeace |> List.ofSeq)

let categorizeChapters (densitiesWar: float list) (densitiesPeace: float list) : string list =
    densitiesPeace
    |> List.zip densitiesWar
    |> List.map (fun (densityWar, densityPeace) -> if densityWar > densityPeace then "war-related" else "peace-related" )
    
let saveToFile (filePath: string) (content: string) : unit =
    System.IO.File.WriteAllText(filePath, content)

let printAndSaveResults (results: string list) (filePath: string) : unit =
    let mutable resultContent = ""
    results
    |> List.iteri (fun index category ->
        let line = sprintf "Chapter %d: %s\n" (index + 1) category
        resultContent <- resultContent + line
    )
    printfn "Chapter Categories:\n%s" resultContent
    saveToFile filePath resultContent
    
    

//let warAndPeace  = "/home/simon/Desktop/FH/SEMESTER 5/FPROG/Semesterprojekt_F#/WarAndPeace/war_and_peace.txt"
let warAndPeace  = "war_and_peace.txt"
//let warTermsPath = "/home/simon/Desktop/FH/SEMESTER 5/FPROG/Semesterprojekt_F#/WarAndPeace/war_terms.txt"
let warTermsPath = "war_terms.txt"
//let peaceTermsPath = "/home/simon/Desktop/FH/SEMESTER 5/FPROG/Semesterprojekt_F#/WarAndPeace/peace_terms.txt"
let peaceTermsPath = "peace_terms.txt"

let tokenizedChapters = readFileAsVector warAndPeace |> splitChapters |> List.map removeChapterHeader |> removeEmptyChapters |> List.tail |> List.map (fun chapter -> tokenizeText chapter)
let warTerms = readFileAsVector warTermsPath |> tokenizeText
let peaceTerms = readFileAsVector peaceTermsPath |> tokenizeText



let warDensities, peaceDensities = processChapters tokenizedChapters warTerms peaceTerms
let categorizedChapters = categorizeChapters warDensities peaceDensities



//let outputPath = "/home/simon/Desktop/FH/SEMESTER 5/FPROG/Semesterprojekt_F#/WarAndPeace/result.txt"
let outputPath = "result.txt"
printAndSaveResults categorizedChapters outputPath
