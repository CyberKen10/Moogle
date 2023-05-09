using System.Text;
using System.IO;

namespace MoogleEngine;

public class Documents
{  
    public List<string> FileName;
    public List<List<string>>Content;
    public List<List<string>> Words;
    public Dictionary<int,Dictionary<string,int>> TF;
    public Dictionary<string, double> IDF;



    public Documents( )
    {
        this.FileName=GetName();
        this.Content=GetContent();
        this.Words=GetWords();
        this.TF=GetTF();
        this.IDF=GetIDF();
    }
    public List<List<string>> GetContent()
{
    List<List<string>> content = new List<List<string>>();
    int contador=0;
    foreach(var document in Directory.GetFiles(Path.Join("..", "Content"))
    {
        
        try
        {
            string text = File.ReadAllText(document);
            text = new string(text.Where(c => !char.IsControl(c)).ToArray());
            string fileName = FileName[contador];
            content.Add(new List<string>{fileName, text});
            
        }
        catch(Exception ex)
        {
            Console.WriteLine("Error al leer el archivo " + document + ": " + ex.Message); 
        }
        contador++;
    }
    return content;
}
    public List<string> GetName()//devuelve el nombre de un documento .txt
    {
        List<string> Names = new List<string>();
       
        foreach(var document in Directory.GetFiles(Path.Join("..", "Content"))
        {
            string name=Path.GetFileNameWithoutExtension(document);
            Names.Add(name);
        }
        return Names;
    }
    public string GetFileName(int a)
    {
        return FileName[a];
    }
    
    public List<List<string>> GetWords()
{
    List<List<string>> Words = new List<List<string>>();
    foreach (var document in Content)
    {
        List<string> documentWords = new List<string>();
        foreach (var doc in document)
        {
            string[] separators = { " ", ".", ",", ";", ":", "(", ")", "[", "]", "{" , "}"};
            string[] words = doc.Split(separators, StringSplitOptions.RemoveEmptyEntries);

            foreach (string word in words)
            {
                if (!string.IsNullOrWhiteSpace(word))
                {
                    documentWords.Add(word.ToLower());
                }
            }
        }
        Words.Add(documentWords);
    }
    return Words;
}
    public List<string> GetWords(int a)
    {
        return Words[a];
    }
    public Dictionary<int,Dictionary<string,int>> GetTF()
    {
        Dictionary<int,Dictionary<string,int>> termFrequencies = new Dictionary<int,Dictionary<string,int>>();
        int numero=0;
        foreach (var documentWords in Words)
        {
            Dictionary<string,int> docTermFrequencies = new Dictionary<string,int>();
            foreach (var word in documentWords)
            {
                if (docTermFrequencies.ContainsKey(word))
            {
                docTermFrequencies[word] +=1;
            }
            else
            {
                docTermFrequencies[word] =1;
            }
            }
            termFrequencies[numero] = docTermFrequencies;
            numero++;
        }
        
        return termFrequencies;
    } 
    public int GetTF(int a,string b)
    {
        return  TF[a][b];
    }
    public Dictionary<string, double> GetIDF()
    {
        Dictionary<string, double> IDF = new Dictionary<string, double>();
        int numDocuments = Words.Count;
        foreach (var documentWords in Words)
        {
            foreach (var word in documentWords.Distinct())
            {
                if (IDF.ContainsKey(word))
                {
                    IDF[word] += 1;
                }
                else
                {
                    IDF[word] = 1;
                }
            }
        }
        foreach (var word in IDF.Keys.ToList())
        {
            IDF[word] = Math.Log(numDocuments / IDF[word]);
        }
        return IDF;
    }
    public string GetSnippet(string fileName, string query)
    {
    List<string>? document = Content.FirstOrDefault(doc => doc[0] == fileName);
        if (document == null)
    {
        throw new ArgumentException($"No se encontró el documento {fileName}");
    }

   List<string> documentWords = Words[FileName.IndexOf(fileName)].Select(w => w.ToLowerInvariant()).ToList();


    Dictionary<string, int> queryTermFrequencies = new Dictionary<string,int>();
    foreach (string term in query.Split(' '))
    {
        string cleanedTerm = term.TrimStart('!', '*', '^'); // eliminar los caracteres no deseados al inicio del término
        if (queryTermFrequencies.ContainsKey(cleanedTerm))
        {
            queryTermFrequencies[cleanedTerm] += 1;
        }
        else
        {
            queryTermFrequencies[cleanedTerm] = 1;
        }
    }

    double k1 = 1.2;
    double b = 0.75;
    double documentLength = documentWords.Count;
    double averageDocumentLength = Words.Average(docWords => docWords.Count);

    Dictionary<string, double> bm25Scores = new Dictionary<string, double>();
    foreach (string term in queryTermFrequencies.Keys)
    {
        if (IDF[term] <0.9) continue;
        int termFrequency = documentWords.Count(word => string.Equals(word, term, StringComparison.OrdinalIgnoreCase));
        double idf = IDF[term];
        double numerator = termFrequency * (k1 + 1);
        double denominator = termFrequency + k1 * (1 - b + b * (documentLength / averageDocumentLength));
        double bm25Score = idf * numerator / denominator;
        bm25Scores[term] = bm25Score;
    }

    List<(int sentenceIndex, double score)> sentenceScores = new List<(int, double)>();
    string[] sentences = document[1].Split(new[] { '.', '!', '?' }, StringSplitOptions.RemoveEmptyEntries);
    for (int i = 0; i < sentences.Length; i++)
    {
        double sentenceScore = 0;
        foreach (string term in queryTermFrequencies.Keys)
        {
            if (IDF[term]<0.8)
            {
                continue;
            }
            int termFrequency = sentences[i].ToLower().Split(' ').Count(word => word == term.ToLower());
            sentenceScore += bm25Scores.ContainsKey(term) ? bm25Scores[term] * termFrequency : 0;
        }
        sentenceScores.Add((i, sentenceScore));
    }

    (int sentenceIndex, double score) topSentence = sentenceScores.OrderByDescending(s => s.score).FirstOrDefault();
    if (topSentence == default)
    {
        return "";
    }

    string topSentenceText = sentences[topSentence.sentenceIndex].Trim();
    int startIndex = Math.Max(topSentenceText.IndexOf(query, StringComparison.OrdinalIgnoreCase) - 20, 0);
    int length = Math.Min(topSentenceText.Length - startIndex, 100);

    return "..." + topSentenceText.Substring(startIndex, length) + "...";
}


    
}

