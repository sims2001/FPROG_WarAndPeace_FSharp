module WarAndPeaceTests

open WarAndPeace
open NUnit.Framework

let generateWords () : string list =
    [ "sadly"; "happy"; "peppi"; "sadness"; "sad"; "sad"]
    
let generateSampleChapters () : string list =
    [
        "CHAPTER 1: Introduction";
        "This is the introduction.";
        "CHAPTER 2: Main Content";
        "This is the main content.";
        "CHAPTER 3: Conclusion";
        "This is the conclusion.";
        "CHAPTER 4: Empty";
        "END OF THE PROJECT GUTENBERG EBOOK, MY BOOK";
        "This is some additional useless Information!"
    ]



[<TestFixture>]
module TokenizeTests =
    [<Test>]
    let ``Tokenize empty string returns empty list`` () =
        let result = tokenizeText [""]
        Assert.IsEmpty(result)

    [<Test>]
    let ``Tokenize string with spaces returns list of words`` () =
        let result = tokenizeText ["This is a test"; "text with some content"; "hallo WELT meine, KleIne."; "\"Informatikerin\""] 
        Assert.AreEqual(["this"; "is"; "a"; "test"; "text"; "with"; "some"; "content"; "hallo"; "welt"; "meine"; "kleine"; "informatikerin"], result)
        

[<TestFixture>]
module FilterTests =
    [<Test>]
    let ``FilterWords with empty input lists returns empty list`` () =
        let result = filterWords [] []
        Assert.IsEmpty(result)

    [<Test>]
    let ``FilterWords with empty words list returns empty list`` () =
        let result = filterWords [] ["prefix"]
        Assert.IsEmpty(result)

    [<Test>]
    let ``FilterWords with empty filter list returns empty list`` () =
        let result = filterWords ["firstword"; "secondword"] []
        Assert.IsEmpty(result)

    [<Test>]
    let ``FilterWords filters words based on prefix`` () =
        let words = generateWords() 
        let filterList = ["sad"]
        let result = filterWords words filterList
        Assert.AreEqual(["sadly"; "sadness"; "sad"; "sad"], result)
        
        
[<TestFixture>]
module CountOccurrencesTests =

    [<Test>]
    let ``CountOccurrences with empty input list returns empty list`` () =
        let result = countOccurrences []
        Assert.IsEmpty(result)

    [<Test>]
    let ``CountOccurrences with distinct words counts occurrences correctly`` () =
        let words = generateWords()
        let result = countOccurrences words
        Assert.AreEqual([("sad", 2); ("sadly", 1); ("happy", 1); ("peppi", 1); ("sadness", 1)], result)

[<TestFixture>]
module CalculateTermDensityTests =

    [<Test>]
    let ``CalculateTermDensity with empty input lists returns 0`` () =
        let result = calculateTermDensity [] []
        Assert.AreEqual(0.0, result)

    [<Test>]
    let ``CalculateTermDensity with non-empty input lists calculates density correctly`` () =
        let text = generateWords()
        let terms = ["sad"; "sad"; "sadly"; "sadness"]
        let result = calculateTermDensity text terms
        Assert.AreEqual(3.0 / 6.0, result)
        

[<TestFixture>]
module ChapterTesting =

    [<Test>]
    let ``SplitChapters with empty input returns empty list`` () =
        let content = [] // Empty input
        let result = List.concat ( splitChapters content )
        Assert.IsEmpty(result)
        
    [<Test>]
    let ``SplitChapters splits chapters correctly`` () =
        let content = generateSampleChapters()
        let result = splitChapters content
        Assert.AreEqual([
            ["CHAPTER 1: Introduction"; "This is the introduction."];
            ["CHAPTER 2: Main Content"; "This is the main content."];
            ["CHAPTER 3: Conclusion"; "This is the conclusion."];
            ["CHAPTER 4: Empty"]
        ], result)
        
    [<Test>]
    let ``SplitChapters and RemoveChapterHeads working`` () =
        let content = generateSampleChapters()
        let result = splitChapters content |> List.map removeChapterHeader
        Assert.AreEqual([
            ["This is the introduction."];
            ["This is the main content."];
            ["This is the conclusion."];
            []
        ], result)
        
    [<Test>]
    let ``SplitChapters and RemoveChapterHeads and RemoveEmptyChapters`` () =
        let content = generateSampleChapters()
        let result = splitChapters content |> List.map removeChapterHeader |> removeEmptyChapters
        Assert.AreEqual([
            ["This is the introduction."];
            ["This is the main content."];
            ["This is the conclusion."]
        ], result)