namespace ADRDemo

module Classifier =

    let time f =
        let stopwatch = System.Diagnostics.Stopwatch.StartNew()
        let result = f()
        stopwatch.Stop()
        printfn "%A elapsed" stopwatch.Elapsed
        result

    let docFreq allSamples term =
        let sampleHasTerm sample = sample.Frequencies |> Seq.exists (fun w -> fst w = term)
        allSamples |> Seq.filter sampleHasTerm |> Seq.length

    let getTermFreqs words = words |> Seq.countBy (fun w -> w)

    let calcInverseDocFreq sampleCount docFrequency = 
        System.Math.Log (float sampleCount / (float docFrequency + 1.0))

    let getTermIDFMap allSamples terms =
        let docFreq = docFreq allSamples
        let sampleCount = Seq.length allSamples
        let idf = docFreq >> calcInverseDocFreq sampleCount
        terms
        |> Array.ofSeq
        |> Array.Parallel.map (fun t -> t, idf t)
        |> Map.ofSeq

    let tfidf (idfs: Map<Term, float>) tf =
        let term, frequency = tf
        let hasWeight = idfs.ContainsKey term
        let idf = if hasWeight then idfs.[term] else 1.0
        term, float frequency * idf

    let weightTermFreqs idfs = Seq.map (tfidf idfs)

    let similarity unknown known =
        let getTerms = Seq.map fst >> Set.ofSeq
        let getWeights = Seq.map snd

        let unknownTerms = getTerms unknown
        let unknownWeights = getWeights unknown

        let knownTerms = getTerms known
        let knownWeights = getWeights known

        let commonTerms = Set.intersect unknownTerms knownTerms
        let isCommonTerm term = commonTerms |> Set.exists (fun w -> w = fst term)
        let commonWeights tfidfs = tfidfs |> Seq.filter isCommonTerm |> getWeights
        
        let commonUnknownWeights = commonWeights unknown
        let commonKnownWeights = commonWeights known

        (Seq.sum commonKnownWeights + Seq.sum commonUnknownWeights)
        / (Seq.sum unknownWeights + Seq.sum knownWeights)

    let calcCategoryProbabilities (trainedData: TrainedData) query =
        let allSamples = trainedData |> Seq.map (fun d -> d.Value)
        
        //calculate similarity score per category, using highest score from each
        let calcSim ws = similarity query ws
        let scoreSamples samples = samples |> Seq.map calcSim
        let scores =
            trainedData
            |> Seq.map (fun kvp -> kvp.Key, (scoreSamples kvp.Value) |> Seq.max)

        //sort scores descending
        scores |> Seq.sortBy snd |> List.ofSeq |> List.rev