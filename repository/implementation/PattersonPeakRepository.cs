using Patterson.persistence;

namespace Patterson.repository
{
    public class PattersonPeakRepository : IPattersonPeakRepository
    {
        private readonly PattersonDBContext context;

        public PattersonPeakRepository(PattersonDBContext context)
        {
            this.context = context;
        }
    }
}
