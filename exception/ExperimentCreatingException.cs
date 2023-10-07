using System;

namespace Patterson.exception
{
    public class ExperimentCreatingException : Exception
    {
        public ExperimentCreatingException()
        {
        }

        public ExperimentCreatingException(string message)
            : base(message)
        {
        }

        public ExperimentCreatingException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}
