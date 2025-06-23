using System.Net;
using Crawler.Models;
using HtmlAgilityPack;


namespace Crawler.Services;

public static class Scraper
{
  private static readonly HttpClient _client = new HttpClient
  {
    BaseAddress = new Uri("https://www.sozcu.com.tr")
  };

  public static async Task<List<string>> GetLinksAsync()
  {
    var allLinks = new List<string>();
    var categories = new[]
    {
      "/",
      "/yazarlar",
      "/gundem",
      "/ekonomi",
      "/dunya",
      "/gunun-icinden",
      "/spor",
      "/futbol",
      "/basketbol",
      "/voleybol",
      "/dunyadan-spor",
      "/diger-sporlar",
      "/hayat",
      "/saglik",
      "/yasam",
      "/kultur-sanat",
      "/magazin",
      "/finans"
    };

    Console.WriteLine($"{categories.Length} kategori taranacak...");

    foreach (var category in categories)
    {
      try
      {
        Console.WriteLine($"Taranan kategori: {category}");
        var links = await GetLinksFromCategoryAsync(category);
        allLinks.AddRange(links);
        Console.WriteLine($"  → {links.Count} link bulundu");
      }
      catch (Exception ex)
      {
        Console.WriteLine($"  → Hata: {ex.Message}");
      }
    }
    var uniqueLinks = allLinks.Distinct().ToList();
    Console.WriteLine($"\nToplam: {allLinks.Count} link, {uniqueLinks.Count} benzersiz link");

    return uniqueLinks;
  }

  public static async Task<List<string>> GetLinksFromCategoryAsync(string category)
  {
    var validLinks = new List<string>();
    var document = new HtmlDocument();
    var response = await _client.GetAsync(category);

    response.EnsureSuccessStatusCode();
    document.LoadHtml(await response.Content.ReadAsStringAsync());
    var nodes = document.DocumentNode.SelectNodes("//a");
    if (nodes == null) return validLinks;

    // /gulben-ergen-...-p186865 || // https://www.sozcu.com.tr/baris-...-tabak-kirdi-p186870
    foreach (var node in nodes)
    {
      var href = node.GetAttributeValue("href", string.Empty);

      if (!string.IsNullOrEmpty(href) && href.Contains("-p"))
      {
        if (!href.StartsWith("http"))
        {
          href = $"https://www.sozcu.com.tr{href}";
        }

        validLinks.Add(href);
      }
    }

    return validLinks;
  }

  public static async Task<List<Article>> GetArticlesAsync(List<string> links)
  {
    var articles = new List<Article>();

    Console.WriteLine($"{links.Count} adet haber linki işleniyor lütfen bekleyiniz...");
    foreach (var link in links)
    {
      var article = await GetArticleAsync(link);
      if (article != null)
      {
        articles.Add(article);
      }
    }

    return articles;
  }

  public static async Task<Article?> GetArticleAsync(string url)
  {
    try
    {
      var response = await _client.GetAsync(url);
      response.EnsureSuccessStatusCode();
      var html = await response.Content.ReadAsStringAsync();

      var document = new HtmlDocument();
      document.LoadHtml(html);

      var article = new Article
      {
        Url = "https://www.sozcu.com.tr" + url,
        Title = ExtractTitle(document),
        Content = ExtractContent(document),
        Author = ExtractAuthor(document),
        PublishDate = ExtractPublishDate(document),
        ImageUrl = ExtractImageUrl(document),
      };

      if (string.IsNullOrEmpty(article.Title) || string.IsNullOrEmpty(article.Content))
      {
        return null;
      }

      return article;
    }
    catch (Exception)
    {
      return null;
    }
  }

  private static string ExtractTitle(HtmlDocument document)
  {
    var titleNode = document.DocumentNode.SelectSingleNode("//meta[@name='title']");
    var title = titleNode?.GetAttributeValue("content", "") ?? "";
    title = WebUtility.HtmlDecode(title);
    return title;
  }

  private static string ExtractContent(HtmlDocument document)
  {
    var contentNodes = document.DocumentNode.SelectNodes("//div[@class='article-body']//p");

    return contentNodes != null ? string.Join("\n", contentNodes.Select(n => n.InnerText)) : "";
  }

  private static string ExtractAuthor(HtmlDocument document)
  {
    var authorNode = document.DocumentNode.SelectSingleNode("//meta[@name='articleAuthor']");
    return authorNode?.GetAttributeValue("content", "") ?? "Sözcü";
  }

  private static DateTime ExtractPublishDate(HtmlDocument document)
  {
    var dateNode = document.DocumentNode.SelectSingleNode("//meta[@name='datePublished']");
    var time = WebUtility.HtmlDecode(dateNode?.GetAttributeValue("content", ""));
    //time: 2025-06-22T05:00:00&#x2B;0300

    if (!string.IsNullOrEmpty(time) && DateTime.TryParse(time, out var publishDate))
    {
      return publishDate;
    }

    return DateTime.Now;
  }

  private static string ExtractImageUrl(HtmlDocument document)
  {
    var imgNode = document.DocumentNode.SelectSingleNode("//div[contains(@class, 'main-image')]//img[@src]");
    return imgNode != null ? imgNode.GetAttributeValue("src", "") : "";
  }
}