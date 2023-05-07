namespace MoogleEngine;


public static class Moogle
{

    public static SearchResult Query(string query) {
        // Modifique este método para responder a la búsqueda
        
        Documents documents =new Documents();
        Query Query = new Query(query);
        ModeloVectorial matriz= new ModeloVectorial(documents,Query);
        List<(string,double)>Documentos=matriz.GetScoreOrganizado();
        var PrimerDocumento=Documentos[0];
        var SegundoDocumento=Documentos[1];
        var TercerDocumento=Documentos[2];
        
        
        SearchItem[] items = new SearchItem[] 
        {
            new SearchItem(PrimerDocumento.Item1,documents.GetSnippet(PrimerDocumento.Item1,Query.GetText()),(float)PrimerDocumento.Item2),
            new SearchItem(SegundoDocumento.Item1,documents.GetSnippet(SegundoDocumento.Item1,Query.GetText()),(float)SegundoDocumento.Item2),
            new SearchItem(TercerDocumento.Item1,documents.GetSnippet(TercerDocumento.Item1,Query.GetText()),(float)TercerDocumento.Item2), 
        };
        return new SearchResult(items, query);
    }
}
