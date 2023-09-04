using MVCSite.Interfaces;
using MVCSite.Models;

namespace MVCSite.Features.Services;

public class SphinxService : ISearchService
{
    private readonly ISphinxConnector _sphinxConnector;
    private readonly ISearchCacheService _searchCacheService;
    public SphinxService(ISphinxConnector sphinxConnector, ISearchCacheService searchCacheService)
    {
        _sphinxConnector = sphinxConnector;
        _searchCacheService = searchCacheService;
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
        var cache = _searchCacheService.GetRecipeSearchCache(query);
        if(cache == null)
        {
            var data = Search(query, "RecipesIndex", new string[]{"RecipeId", "Label"});
            var result = data.Select(x => new RecipeSearchResult((string)x[0], (string)x[1])).ToArray();
            _searchCacheService.SetRecipeSearchCache(query, result);
            return result;
        }
        else
            return cache;
    }
}