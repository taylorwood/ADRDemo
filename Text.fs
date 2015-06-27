namespace ADRDemo

module Text =

    open System
    open System.IO
    open System.Text.RegularExpressions

    let inline isCharAllowed c =
        Char.IsLetterOrDigit c || Char.IsWhiteSpace c || c = '-'

    let sanitizeText (text: string) =
        let cleanChars = text |> Seq.filter isCharAllowed |> Seq.toArray
        new String(cleanChars)

    let toUpper (str: string) = str.ToUpper()
    
    let getWhitelistWords =
        let csvRows = File.ReadAllLines @"C:\Temp\ADR\Demo\WhitelistDict.csv"
        csvRows |> Seq.map sanitizeText |> Set.ofSeq

    let inWhitelist word =
      let whitelist = getWhitelistWords
      if Set.count whitelist > 0 then
        whitelist |> Set.exists (fun wl -> wl = word)
      else
        true

    let getNGrams n words =
        let stringJoin (items: string[]) = System.String.Join(" ", items)
        words |> Seq.windowed n |> Seq.map stringJoin