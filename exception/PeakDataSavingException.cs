using System;

namespace Patterson.exception
{
    public class PeakDataSavingException : Exception
    {
        public PeakDataSavingException()
        {
        }

        public PeakDataSavingException(string message)
            : base(message)
        {
        }

        public PeakDataSavingException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}
