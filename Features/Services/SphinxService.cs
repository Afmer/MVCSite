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
    public RecipeSearchResult[] Search(string query)
    {
        var data = _sphinxConnector.GetData($"SELECT RecipeId, Label FROM RecipesIndex WHERE MATCH('{query}')");
        return data.Select(x => new RecipeSearchResult((string)x[0], (string)x[1])).ToArray();
    }
}