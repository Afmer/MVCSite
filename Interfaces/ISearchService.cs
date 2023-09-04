using MVCSite.Models;

namespace MVCSite.Interfaces;

public interface ISearchService
{
    public RecipeSearchResult[] SearchRecipes(string query);
}