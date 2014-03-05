ADRDemo
=======

A simple F# demonstration of Automated Document Recognition using techniques like text tokenization, n-grams, TF-IDF weighting, CSV parsing, and text classification.

The code assumes the existence of some training data, or plain text files organized into folders by category:

\ADRDemo\TrainingData
  \CategoryA
    \Sample1.txt
    \Sample2.txt
  \CategoryB
    \SampleA.txt
    
...and a plain text file to be classified: "unknown.txt".

It also assumes the existence of a word whitelist CSV file, but this can be easily changed to a blacklist ("stopwords") or removed altogether.
