open System
open System.IO
open System.Text.RegularExpressions

// Function to load text from a file
let loadTextFromFile (filePath: string) =
    try
        if File.Exists(filePath) then
            File.ReadAllText(filePath)
        else
            printfn "File not found: %s" filePath
            ""
    with
    | :? IOException as ex -> 
        printfn "Error reading file: %s" ex.Message
        ""
    | ex -> 
        printfn "Unexpected error: %s" ex.Message
        ""

// Function to clean and split text into words
let splitWords (text: string) =
    text
    |> fun t -> t.ToLower() // Case insensitive analysis
    |> fun t -> Regex.Replace(t, @"[^\w\s]", "") // Remove punctuation
    |> fun t -> t.Split([|' '; '\n'; '\r'; '\t'|], StringSplitOptions.RemoveEmptyEntries)

// Function to count words
let countWords (text: string) =
    splitWords text |> Array.length

// Function to count sentences
let countSentences (text: string) =
    let sentencePattern = @"[.!?]"
    Regex.Matches(text, sentencePattern).Count

let countParagraphs (text: string) =
    text.Split([|'\n'|], StringSplitOptions.RemoveEmptyEntries) |> Array.length

// Function to calculate word frequencies
let wordFrequency (text: string) =
    let words = splitWords text
    words
    |> Array.groupBy id
    |> Array.map (fun (word, occurrences) -> (word, occurrences.Length))
    |> Array.sortByDescending snd

// Function to calculate the average sentence length (words per sentence)
let averageSentenceLength (text: string) =
    let sentenceCount = countSentences text
    if sentenceCount > 0 then
        let totalWords = countWords text
        float totalWords / float sentenceCount
    else
        0.0

// Function to calculate the Flesch-Kincaid readability score
let fleschKincaidReadability (text: string) =
    let words = splitWords text |> Array.length
    let sentences = countSentences text
    let syllables = text.Split([|' '; '.'; '\n'|], StringSplitOptions.RemoveEmptyEntries)
                     |> Array.sumBy (fun word -> word.Length / 2) // Approximation
    let averageSentenceLength = float words / float sentences
    let averageSyllablesPerWord = float syllables / float words
    206.835 - (1.015 * averageSentenceLength) - (84.6 * averageSyllablesPerWord)

// Function to display results
let displayResults (text: string) =
    printfn "\nText Analysis Results:"
    printfn "------------------------"
    printfn "Total Words: %d" (countWords text)
    printfn "Total Sentences: %d" (countSentences text)
    printfn "Total Paragraphs: %d" (countParagraphs text)
    
    let wordFreq = wordFrequency text |> Array.take 10
    printfn "Top 10 Most Frequent Words:"
    wordFreq |> Array.iter (fun (word, count) -> printfn "%s: %d" word count)
    
    printfn "Average Sentence Length (words per sentence): %.2f" (averageSentenceLength text)
    printfn "Flesch-Kincaid Readability Score: %.2f" (fleschKincaidReadability text)

// Main function to prompt for user input and load text
let main () =
    printfn "Enter a file path or type 'exit' to quit:"
    let input = Console.ReadLine()
    
    if input.ToLower() = "exit" then
        printfn "Exiting the program."
    else
        let text = loadTextFromFile input
        if text <> "" then
            displayResults text
        else
            printfn "Error: Could not load the file."


main ()
