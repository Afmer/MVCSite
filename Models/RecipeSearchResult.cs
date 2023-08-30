namespace MVCSite.Models;

public class RecipeSearchResult
{
    public string RecipeId { get; set; }
    public string Label { get; set; }
    public RecipeSearchResult(string recipeId, string label)
    {
        RecipeId = recipeId;
        Label = label;
    }
}
