namespace ScrapingLibrary.Models.Scrapping;

public interface IScrapper<T>
{
    Task<T> GetData();
}