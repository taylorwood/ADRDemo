namespace ADRDemo

module Text =

    open System.IO
    open System.Text.RegularExpressions

    let isCharAllowed c =
        System.Char.IsLetterOrDigit c || System.Char.IsWhiteSpace c || c = '-'

    let sanitizeText (text: string) =
        let cleanChars = text |> Seq.filter isCharAllowed |> Seq.toArray
        new System.String(cleanChars)

    let toUpper (str: string) = str.ToUpper()
    
    let getWhitelistWords =
        let csvRows = File.ReadAllLines @"C:\Temp\ADR\Demo\WhitelistDict.csv"
        csvRows |> Seq.map sanitizeText |> Set.ofSeq

    let inWhitelist word = getWhitelistWords |> Set.exists (fun wl -> wl = word)

    let getNGrams n words =
        let stringJoin (items: string[]) = System.String.Join(" ", items)
        words |> Seq.windowed n |> Seq.map stringJoin

    let getWords (text: string) =
        let words = Regex.Split(text, @"\s+")
        words |> Seq.filter inWhitelist |> getNGrams 1 |> Seq.toList

    let tokenizeFile = File.ReadAllText >> sanitizeText >> getWords