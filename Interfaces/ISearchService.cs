using MVCSite.Models;

namespace MVCSite.Interfaces;

public interface ISearchService
{
    public RecipeSearchResult[] Search(string query);
}