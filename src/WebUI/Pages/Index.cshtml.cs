using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using WebUI.Models;
using WebUI.Services;

namespace WebUI.Pages;

public class IndexModel : PageModel
{
    private readonly ElasticsearchService _elasticsearchService;

    public IndexModel(ElasticsearchService elasticsearchService)
    {
        _elasticsearchService = elasticsearchService;
    }

    public List<Article> Articles { get; set; } = new();

    [BindProperty(SupportsGet = true)]
    public string? SearchQuery { get; set; }

    public async Task OnGetAsync()
    {
        if (!string.IsNullOrEmpty(SearchQuery))
        {
            Articles = await _elasticsearchService.SearchArticlesAsync(SearchQuery, 50);
        }
        else
        {
            Articles = await _elasticsearchService.GetAllArticlesAsync();
        }
    }
}
