using MVCSite.Models;
namespace Tests;

public static class ModelFabric
{
    public static RecipeDataModel GetRecipeDataModel()
    {
        var rnd = new Random();
        var result = new RecipeDataModel();
        result.AuthorLogin = "TestUser" + rnd.Next().ToString();
        result.Content = "<p>hello</p>";
        result.DateOfCreation = DateTime.UtcNow;
        result.Id = Guid.NewGuid();
        result.Label = "Label" + rnd.Next().ToString();
        return result;
    }
    public static TempRecipeImageInfoDataModel GetTempRecipeImageInfoDataModel()
    {
        var result = new TempRecipeImageInfoDataModel();
        result.DateOfCreation = DateTime.UtcNow;
        result.Id = Guid.NewGuid();
        result.RecipeId = Guid.NewGuid();
        return result;
    }
}
