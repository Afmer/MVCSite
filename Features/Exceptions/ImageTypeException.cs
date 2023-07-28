namespace MVCSite.Features.Exceptions;
public class ImageTypeException : Exception
{
    public ImageTypeException()
    {
    }
    public ImageTypeException(string message) : base(message)
    {
    }
    public ImageTypeException(string message, Exception innerException) : base(message, innerException)
    {
    }
}