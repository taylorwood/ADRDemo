namespace ADRDemo

open System.IO
open ADRDemo.Text
open ADRDemo.Classifier

type TFIDFCalculator(tokenizer:NGramTokenizer) =

  let getCategoryName path = Path.GetFileName path

  member this.WeightTrainingData (trainingData: TrainingData) =
    let allSamples = trainingData |> Seq.collect (fun d -> d.Value) //flatten samples
    let allTermFreqs = allSamples |> Seq.collect (fun s -> s.Frequencies) //flatten all TFs
    let termSet = allTermFreqs |> Seq.map fst |> Set.ofSeq //unique term set
        
    //build map of term : inverse doc frequency
    let idfs = getTermIDFMap allSamples termSet

    //calculate tfidf weights for all samples
    let weightSample samp = samp.Frequencies |> Seq.map (tfidf idfs) |> Set.ofSeq
    let weightSamples samples = samples |> Seq.map weightSample
    let weightedSamples =
        trainingData
        |> Seq.map (fun d -> d.Key, (weightSamples d.Value) |> List.ofSeq)
        |> Map.ofSeq

    weightedSamples, idfs

  member this.LoadCategorySamples path = 
      let loadSample path = tokenizer.TokenizeFile path |> tokenizer.Tokenize |> getTermFreqs
      //tokenize all sample files
      let samplePaths = Directory.GetFiles path
      printfn "Loading samples for category %A" (getCategoryName path)
      samplePaths
      |> Seq.map (fun f -> {Path = f; Frequencies = loadSample f |> Set.ofSeq;})
      |> Seq.truncate 10 //take it easy!
      |> List.ofSeq

  member this.LoadTrainingData adrPath = 
      //load training samples from category directories
      let catPaths = Directory.GetDirectories adrPath
      let trainingData =
          catPaths
          |> Seq.map (fun path -> path, this.LoadCategorySamples path)
          |> Map.ofSeq

      //calc TFIDF weights
      let weightedCategories, data = time (fun() -> this.WeightTrainingData trainingData)

      trainingData, weightedCategories, data    

