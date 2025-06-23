using Nest;
using WebUI.Models;

namespace WebUI.Services;

public class ElasticsearchService
{
  private readonly ElasticClient _client;
  private readonly string _indexName = "sozcu-articles";

  public ElasticsearchService()
  {
    var settings = new ConnectionSettings(new Uri("http://localhost:9200"))
      .DefaultIndex(_indexName);

    _client = new ElasticClient(settings);
  }

  public async Task<List<Article>> SearchArticlesAsync(string query, int size = 20)
  {
    try
    {
      if (string.IsNullOrEmpty(query))
      {
        return await GetAllArticlesAsync(size);
      }

      var searchResponse = await _client.SearchAsync<Article>(s => s
          .Index(_indexName)
          .Query(q => q
              .MultiMatch(m => m
                  .Query(query)
                  .Fields(f => f
                      .Field(a => a.Title, 2.0)
                      .Field(a => a.Content)
                      .Field(a => a.Author)
                      .Field(a => a.Url)
                  )
              )
          )
          .Size(size)
      );

      return searchResponse.IsValid ? searchResponse.Documents.ToList() : new List<Article>();
    }
    catch (Exception ex)
    {
      Console.WriteLine($"Arama hatası: {ex.Message}");
      return new List<Article>();
    }
  }

  public async Task<List<Article>> GetAllArticlesAsync(int size = 50)
  {
    try
    {
      var searchResponse = await _client.SearchAsync<Article>(s => s
          .Index(_indexName)
          .Query(q => q.MatchAll())
          .Size(size)
      );

      return searchResponse.IsValid ? searchResponse.Documents.ToList() : new List<Article>();
    }
    catch (Exception ex)
    {
      Console.WriteLine($"Haber getirme hatası: {ex.Message}");
      return new List<Article>();
    }
  }
}
