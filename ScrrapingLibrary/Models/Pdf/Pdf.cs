namespace ScrapingLibrary.Models.Pdf
{
    public abstract class Pdf<T>
    {
        protected readonly string Path;

        public Pdf(string path)
        {
            Path = path;
        }

        public virtual IEnumerable<string> GetContent()
        {
            throw new NotImplementedException();
        }

        public abstract Task<T> Parse();
    }
}