using Patterson.model;
using Patterson.persistence;
using System.Collections.Generic;

namespace Patterson.repository
{
    public class PattersonPeakRepository : IPattersonPeakRepository
    {
        private readonly PattersonDBContext context;

        public PattersonPeakRepository(PattersonDBContext context)
        {
            this.context = context;
        }

        public void SaveData(List<PattersonPeak> pattersonPeaks)
        {
            context.PattersonPeaks.AddRange(pattersonPeaks);
            context.SaveChanges();
        }
    }
}
