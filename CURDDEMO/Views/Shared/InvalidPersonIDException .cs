namespace Exceptions
{
    public class InvalidPersonIDException : ArgumentException
    {
        public InvalidPersonIDException()   :base()
        {
            
        }
        public InvalidPersonIDException(string? Message) : base( Message)
        {

        }

        public InvalidPersonIDException(string? Message,Exception? innerException) : base(Message, innerException)
        {

        }
    }
}
