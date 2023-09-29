namespace MVCSite.Models;

public class RecipesSearchModel
{
    public RecipeSearchResult[] SearchResults { get; set; } = null!;
    public Dictionary<Guid, string> RecipeIdsToLabelImageIds {get; set;} = null!;
    public int TotalPages {get; set;}
    public int CurrentPage {get; set;}
    public string Query {get; set;} = null!;
}
