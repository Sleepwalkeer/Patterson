using System;

namespace Patterson.exception
{
    public class PattersonPeakSavingException : Exception
    {
        public PattersonPeakSavingException()
        {
        }

        public PattersonPeakSavingException(string message)
            : base(message)
        {
        }

        public PattersonPeakSavingException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }

}
