using MVCSite.Interfaces;
using MVCSite.Models;

namespace MVCSite.Features.Services;

public class SphinxService : ISearchService
{
    private readonly ISphinxConnector _sphinxConnector;
    public SphinxService(ISphinxConnector sphinxConnector)
    {
        _sphinxConnector = sphinxConnector;
    }
    private List<object[]> Search(string query, string index, string[] attributes)
    {
        string attributesStr = "";
        for(int i = 0; i < attributes.Length - 1; i++)
            attributesStr += attributes[i] + ", ";
        attributesStr += attributes[^1];
        var result = _sphinxConnector.GetData($"SELECT {attributesStr} FROM {index} WHERE MATCH('{query}')");
        return result;
    }
    public RecipeSearchResult[] SearchRecipes(string query)
    {
        var data = Search(query, "RecipesIndex", new string[]{"RecipeId", "Label"});
        return data.Select(x => new RecipeSearchResult((string)x[0], (string)x[1])).ToArray();
    }
}