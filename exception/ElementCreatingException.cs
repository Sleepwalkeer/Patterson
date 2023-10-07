using System;

namespace Patterson.exception
{
    public class ElementCreatingException : Exception
    {
        public ElementCreatingException()
        {
        }

        public ElementCreatingException(string message)
            : base(message)
        {
        }

        public ElementCreatingException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }

}
