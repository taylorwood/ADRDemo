open ADRDemo
open ADRDemo.Classifier
open ADRDemo.Text
open System.IO
open System.Collections.Generic

module Console =

    [<EntryPoint>]
    let main argv =
        //the training data is expected to be .txt files organized into folders by category/class
        //the folder name is the category name
        let adrPath = @"C:\Temp\ADR\Demo\FirstPageTrainingData\"
        
        let tokenizer = new NGramTokenizer()
        let tfidf = new TFIDFCalculator(tokenizer)
        //load training samples from category directories
        let catPaths = Directory.GetDirectories adrPath
        let trainingData =
            catPaths
            |> Seq.map (fun path -> path, tfidf.LoadCategorySamples path)
            |> Map.ofSeq

        //calc TFIDF weights
        let trainedData, idfMap = time (fun() -> tfidf.WeightTrainingData trainingData)

        let termToToken (term:string) = tokenizer.Tokenize term

        let queryTextPath = adrPath + "unknown.txt" //path to the "query" document to be classified/categorized
        let tfidfTextFile path =
          tokenizer.TokenizeFile path
          |> tokenizer.Tokenize
          |> getTermFreqs
          |> weightTermFreqs idfMap
          |> Set.ofSeq

        let weightedQuery = tfidfTextFile queryTextPath
        let probabilities = calcCategoryProbabilities trainedData weightedQuery |> Seq.truncate 10 |> Seq.toList
        printfn "Top 10 matches:\r\n%A" probabilities
        0