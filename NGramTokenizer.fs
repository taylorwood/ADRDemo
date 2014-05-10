namespace ADRDemo

open ADRDemo.Text
open System.IO
open System.Collections.Generic
open System.Text.RegularExpressions

/// Stores a list of all n-grams and their corresponding Term value.
type NGramTokenizer() =

  let mutable tokenCount = 0
  let gramTokenMap = new Dictionary<string,Term>()
  
  member private this.AddToken ngram =
    tokenCount <- tokenCount + 1
    gramTokenMap.Add (ngram, tokenCount)
    tokenCount

  /// Returns the Term for an n-gram
  member this.Tokenize ngram =
    let tryGet = gramTokenMap.TryGetValue ngram
    match tryGet with
      | false, _ -> this.AddToken ngram
      | true, t -> t

  /// Returns the Terms for a sequence of n-grams
  member this.Tokenize (ngrams:string list) =
    ngrams |> Seq.map this.Tokenize
  
  member private this.GetWords gramSize (text: string) =
    let words = Regex.Split(text, @"\s+")
    words |> Seq.filter inWhitelist |> getNGrams gramSize |> Seq.toList

  member this.TokenizeFile path = File.ReadAllText path |> sanitizeText |> this.GetWords 1