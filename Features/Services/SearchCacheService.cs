using MVCSite.Models;
using MVCSite.Interfaces;
namespace MVCSite.Features.Services;
public class SearchCacheService : ISearchCacheUpdater, ISearchCacheService
{
    private static readonly Dictionary<string, (RecipeSearchResult[] result, DateTime dateOfCreation)> _recipeSearchCache = new();
    public void UpdateCache(TimeSpan lifeTime)
    {
        UpdateRecipeCache(lifeTime);
    }
    private void UpdateRecipeCache(TimeSpan lifeTime)
    {
        lock(_recipeSearchCache)
        {
            var removalResult = _recipeSearchCache.Where(x => DateTime.UtcNow - x.Value.dateOfCreation > lifeTime)
                .Select(x => x.Key)
                .ToArray();
            foreach(var item in removalResult)
            {
                _recipeSearchCache.Remove(item);
            }
        }
    }
    public RecipeSearchResult[] GetRecipeSearchCache(string query)
    {
        lock (_recipeSearchCache)
        {
            if(_recipeSearchCache.ContainsKey(query))
                return _recipeSearchCache[query].result;
            else
                return null!;
        }
    }
    public bool SetRecipeSearchCache(string query, RecipeSearchResult[] recipeSearchResult)
    {
        lock(_recipeSearchCache)
        {
            if(!_recipeSearchCache.ContainsKey(query))
            {
                _recipeSearchCache.Add(query, (recipeSearchResult, DateTime.UtcNow));
                return true;
            }
            else
                return false;
        }
    }
}