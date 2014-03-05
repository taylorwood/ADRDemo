open ADRDemo.Classifier
open ADRDemo.Text
open ADRDemo.TrainingData
open System.IO

module Console =

    [<EntryPoint>]
    let main argv =
        //the training data is expected to be .txt files organized into folders by category/class
        //the folder name is the category name
        let adrPath = @"C:\Temp\ADR\Demo\FirstPageTrainingData\"

        //load, tokenize, weight all training data samples
        let trainingData, trainedData, idfMap = loadTrainingData adrPath

        let queryTextPath = adrPath + "unknown.txt" //path to the "query" document to be classified/categorized
        let tfidfTextFile = tokenizeFile >> getTermFreqs >> weightTermFreqs idfMap >> Set.ofSeq
        let weightedQuery = tfidfTextFile queryTextPath

        let probabilities = calcCategoryProbabilities trainedData weightedQuery |> Seq.truncate 10
        printfn "Top 10 matches: %A" probabilities
        0