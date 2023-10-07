using System;

namespace Patterson.exception
{
    public class PropertyParsingException : Exception
    {
        public PropertyParsingException()
        {
        }

        public PropertyParsingException(string message)
            : base(message)
        {
        }

        public PropertyParsingException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }

}
