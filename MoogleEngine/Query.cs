using System.Text;
using System.IO;
namespace MoogleEngine;

public class Query
{   
    public string Text;
    public List<string>Words;
    public Dictionary<string,int> TF;
public Query(string query)
{
    this.Text=query.ToLower();
    this.Words=GetWords();
    this.TF=TermsFrecuency();
}
public string GetText()
{
    return Text;
}
public List<string> GetWords()
{
    List<string>Words=new List<string>();
    string[] separators = { " ", ".", ",", ";", ":", "(", ")", "[", "]", "{", "}"};
    string[] words = Text.Split(separators, StringSplitOptions.RemoveEmptyEntries);
    foreach (string word in words)
        {
            Words.Add(word);
        }
    return Words;
}
public Dictionary<string,int> TermsFrecuency()
{
    Dictionary<string,int>TF=new Dictionary<string,int>();
    foreach (var word in Words)
    {
        if (TF.ContainsKey(word))
        {
            TF[word]++;
        }
        else
        {
            TF[word]=1;
        }
    }
    return TF;
}


}
    