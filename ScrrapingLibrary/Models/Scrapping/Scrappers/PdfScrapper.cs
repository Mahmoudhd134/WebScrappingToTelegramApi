using ScrapingLibrary.Models.Pdf;

namespace ScrapingLibrary.Models.Scrapping.Scrappers
{
    public class PdfScrapper<T> : IScrapper<T>
    {
        private readonly Pdf<T> _pdf;

        public PdfScrapper(Pdf<T> pdf)
        {
            _pdf = pdf;
        }
    
        public async Task<T> GetData()
        {
            return await _pdf.Parse();
        }
    }
}