namespace ADRDemo

type Category = string
type Term = int
type TermFrequency = Term * int
type TrainingSample = {Path: string; Frequencies: Set<TermFrequency>}
type TFIDF = Term * float
type WeightedSample = Set<TFIDF>
type TrainingData = Map<Category, TrainingSample list>
type TrainedData = Map<Category, WeightedSample list>
type TFIDFData = Map<Term, float>