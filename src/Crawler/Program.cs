using Crawler.Models;
using Crawler.Services;
using Nest;

Console.WriteLine("Sözcü makale crawler başlatılıyor...");

var links = await Scraper.GetLinksAsync();
Console.WriteLine($"{links.Count} link bulundu.");
var articles = await Scraper.GetArticlesAsync(links);
Console.WriteLine($"{articles.Count} haber işlendi.");

var settings = new ConnectionSettings(new Uri("http://localhost:9200"))
    .DefaultIndex("sozcu-articles")
    .DefaultMappingFor<Article>(m => m
        .IdProperty(a => a.Title)
    );
var client = new ElasticClient(connectionSettings: settings);

if (articles.Count > 0)
{
  var result = await client.IndexManyAsync(articles);

  if (!result.Errors)
  {
    Console.WriteLine("Haberler başarıyla Elasticsearch'e kaydedildi.");
  }
  else
  {
    Console.WriteLine("Bazı kayıtlar hatalı:");
    foreach (var item in result.ItemsWithErrors)
    {
      Console.WriteLine($"Hata: {item.Error}, ID: {item.Id}");
    }
  }
}

Console.WriteLine("\nCrawler tamamlandı!");
