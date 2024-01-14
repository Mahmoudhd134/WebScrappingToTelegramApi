namespace ScrapingLibrary.Implementation;

public class CanNotMakePdfWithWkHtmlToPdfException : Exception
{
    public CanNotMakePdfWithWkHtmlToPdfException()
    {
    }

    public CanNotMakePdfWithWkHtmlToPdfException(string message) : base(message)
    {
    }

    public CanNotMakePdfWithWkHtmlToPdfException(string message, Exception innerException) : base(message, innerException)
    {
    }
}