using Microsoft.AspNetCore.Mvc;
using MVCSite.Interfaces;
using MVCSite.Models;
using MVCSite.Features.Extensions.Constants;
using MVCSite.Features.Configurations;

namespace MVCSite.Controllers;

public class SearchController : Controller
{
    private readonly ISearchService _searchService;
    private readonly IDBManager _dbManager;
    private readonly IImageService _imageService;
    private readonly int _elementsOnPage;
    public SearchController(ISearchService searchService, IDBManager dbManager, IImageService imageService, IConfiguration configuration)
    {
        _searchService = searchService;
        _dbManager = dbManager;
        _imageService = imageService;
        var pageConf = configuration.GetSection(SettingsName.SearchPageConfiguration).Get<SearchPageConfiguration>();
        if(pageConf != null)
            _elementsOnPage = pageConf.NumOfElements;
        else
            throw new Exception("SearchPAge settings not found");
    }
    public IActionResult Recipes(string query, int page = 1)
    {
        int numElements = _elementsOnPage;
        var searchResult = _searchService.SearchRecipes(query);
        int pages = 1;
        if(searchResult.Length % numElements == 0)
            pages = searchResult.Length / numElements;
        else
            pages = (searchResult.Length / numElements) + 1;
        if(page > pages)
            page = pages;
        var result = searchResult
            .Skip((page - 1) * numElements)
            .Take(numElements)
            .ToArray();
        var recipeIds = result.Select(x => new Guid(x.RecipeId));
        var imagesLinkDict = _dbManager.GetRecipeLabelImageIds(recipeIds)
            .ToDictionary(x => x.RecipeId, x => _imageService.GetLink(x.imageId, AppDataFolders.LabelImages));
        var model = new RecipesSearchModel()
        {
            SearchResults = result,
            RecipeIdsToLabelImageIds = imagesLinkDict,
            TotalPages = pages,
            CurrentPage = page,
            Query = query
        };
        return View(model);
    }
}
