using System.Text;
using System.IO;
using System.Linq;

namespace MoogleEngine;

public class ModeloVectorial
{
    private Documents documentos;
    private Query query;
    private double[,] matrizDocumentos;
    private List<(string, double)> Score;

    public ModeloVectorial(Documents documentos, Query query)
    {
        this.documentos = documentos;
        this.query = query;
        this.matrizDocumentos = GetMatrix();
        this.Score = ObtenerSimilitudDocumentos();
    }

     public double[,] GetMatrix()
    {
        int numDocumentos = documentos.FileName.Count;
        int numWords = documentos.IDF.Count;
        double[,] matriz = new double[numDocumentos, numWords];

        // Calcular la frecuencia de términos de cada documento en relación a la query
        for (int i = 0; i < numDocumentos; i++)
        {
            Dictionary<string, int> frecuencias = documentos.TF[i];
            int numWordsQuery= query.Words.Count;
            for (int j = 0; j < numWordsQuery; j++)
            {
                string palabra = query.Words[j];
                if (palabra.StartsWith("!"))
                {
                    // Si la palabra comienza con "!", simplemente continuar con la siguiente palabra
                    continue;
                }
                bool obligatorio = false;
                if (palabra.StartsWith("^"))
                {
                palabra = palabra.Substring(1); // quitar el símbolo '^'
                obligatorio = true;
                }
                int numAsteriscos = 0;
                while (palabra.StartsWith("*"))
                {
                numAsteriscos++;
                palabra = palabra.Substring(1);
                }

                if (frecuencias.ContainsKey(palabra))
                {
                    double tf = (double)frecuencias[palabra];
                    double idf = documentos.IDF[palabra];
                    tf *= (1 + numAsteriscos); // Multiplicar TF por la cantidad de asteriscos + 1
                    if (documentos.IDF[palabra]<0.9&&obligatorio==false)
                    {
                        tf=0;
                        matriz[i, j] = tf * idf;
                    }
                    
                    else{matriz[i, j] = tf * idf;}
                }
                if (obligatorio) // si la palabra es obligatoria, se multiplica por 1000
                {
                    matriz[i, j] *= 1000;
                }
            }
        }
        return matriz;
    }

    public List<(string, double)> ObtenerSimilitudDocumentos()
    {
        List<(string, double)> resultados = new List<(string, double)>();

        // Calcular la similitud de cosenos entre la query y cada documento
        for (int i = 0; i < documentos.FileName.Count; i++)
        {
            double similitud = CalcularSimilitudCosenos(matrizDocumentos, i);
            resultados.Add((documentos.FileName[i], similitud));
        }
        
        return resultados;
    }

    private double CalcularSimilitudCosenos(double[,] matriz, int indiceDocumento)
{
    double similitud = 0.0;
    double normaDocumento = 0.0;
    double normaQuery = 0.0;
    int numWordsQuery = query.Words.Count;

    if (numWordsQuery == 1) // Si la consulta es una sola palabra, calcular la similitud de cosenos para esa palabra
    {
        double valorQuery = 1;
        double valorDocumento = matriz[indiceDocumento, 0];
        similitud = valorQuery * valorDocumento;
        normaDocumento = valorDocumento;
        normaQuery = valorQuery;
    }
    else // Si la consulta tiene varias palabras, calcular la similitud de cosenos normalmente
    {
        double[] vectorQuery = new double[numWordsQuery];
        for (int i = 0; i < numWordsQuery; i++)
        {
            vectorQuery[i] = 1;
        }

        double[] vectorDocumento = new double[numWordsQuery];
        for (int i = 0; i < numWordsQuery; i++)
        {
            vectorDocumento[i] = matriz[indiceDocumento, i];
        }

        // Calcular la norma del documento
        normaDocumento = Math.Sqrt(vectorDocumento.Select(x => x * x).Sum());

        // Calcular la similitud de cosenos
        similitud = vectorQuery.Select((x, i) => x * vectorDocumento[i]).Sum();

        // Calcular la norma de la consulta
        normaQuery = 1;

    }

    return similitud / (normaDocumento * normaQuery);
}

    public List<(string, double)> GetScore()
    {
        return Score;
    }
    public List<(string, double)> GetScoreOrganizado()
    {
        Score= Score.OrderByDescending(x => x.Item2).ToList();
        return Score;
    }
}
