namespace Exceptions
{
    public class InvalidPersonIdException : ArgumentException
    {
        public InvalidPersonIdException() : base() { }
        public InvalidPersonIdException(string? message) : base() { }
        public InvalidPersonIdException(string? message, Exception? innerException) { }
    }
}