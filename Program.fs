open ADRDemo.Classifier
open ADRDemo.Text
open ADRDemo.TrainingData
open System.IO

module Console =

    [<EntryPoint>]
    let main argv =
        let adrPath = @"C:\Temp\ADR\Demo\FirstPageTrainingData"

        let trainingData, trainedData, idfMap = loadTrainingData adrPath

        let queryPath = adrPath + @"\URLA\20173_URLA.txt"
        let tfidfTextFile = tokenizeFile >> getTermFreqs >> weightTermFreqs idfMap >> Set.ofSeq
        let weightedQuery = tfidfTextFile queryPath

        let probabilities = calcCategoryProbabilities trainedData weightedQuery |> Seq.truncate 10
        printfn "Top 10 matches: %A" probabilities
        0