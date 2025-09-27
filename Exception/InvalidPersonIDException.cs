namespace Exceptions
{
    public class InvalidPersonIDExceptions : ArgumentException
    {
        public InvalidPersonIDExceptions() : base()
        {

        }
        public InvalidPersonIDExceptions(string? Message) : base(Message)
        {

        }

        public InvalidPersonIDExceptions(string? Message, Exception? innerException) : base(Message, innerException)
        {

        }
    }
}

