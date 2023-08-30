using Microsoft.AspNetCore.Mvc;
using MVCSite.Interfaces;
using MVCSite.Models;

namespace MVCSite.Controllers;

public class SearchController : Controller
{
    private readonly ISearchService _searchService;
    public SearchController(ISearchService searchService)
    {
        _searchService = searchService;
    }
    public IActionResult Recipes(string query)
    {
        var result = _searchService.Search(query);
        var model = new RecipesSearchModel(){SearchResults = result};
        return View(model);
    }
}
