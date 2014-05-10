#load "Text.fs"
#load "ADR.fs"
#load "TrainingData.fs"

open ADRDemo.Text
open ADRDemo.Classifier
open ADRDemo.TrainingData

let sampleText = "See Dick run. See Dick jump. See Dick learn a simple text classification strategy."

let cleanChars chars = chars |> Seq.filter isCharAllowed |> Seq.map System.Char.ToUpper |> Seq.toArray

let sanitizedText = System.String (cleanChars sampleText)

let words = sanitizedText.Split(' ')

let bigrams = getNGrams 1 words

let gramGroups = bigrams |> Seq.groupBy (fun g -> g)

let termFreqs = gramGroups |> Seq.map (fun g -> fst g, Seq.length (snd g))
termFreqs |> Seq.iter (printfn "%A")