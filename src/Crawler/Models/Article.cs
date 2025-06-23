namespace Crawler.Models;

public class Article
{
  public string Title { get; set; } = string.Empty;
  public string Url { get; set; } = string.Empty;
  public string Content { get; set; } = string.Empty;
  public DateTime PublishDate { get; set; }
  public string Author { get; set; } = string.Empty;
  public string ImageUrl { get; set; } = string.Empty;
}