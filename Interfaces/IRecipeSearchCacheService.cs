using MVCSite.Models;

namespace MVCSite.Interfaces;
public interface IRecipeSearchCacheService
{
    public RecipeSearchResult[] GetRecipeSearchCache(string query);
    public bool SetRecipeSearchCache(string query, RecipeSearchResult[] recipeSearchResult);
}