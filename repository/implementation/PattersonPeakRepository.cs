using Patterson.exception;
using Patterson.model;
using Patterson.persistence;
using System;
using System.Collections.Generic;
using System.Windows.Forms;

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
            try
            {
                var elementDbSet = context.PattersonPeaks;
                elementDbSet.AddRange(pattersonPeaks);
                context.SaveChanges();
            }
            catch (Exception ex)
            {
                MessageBox.Show("An error occurred while saving patterson peaks data" + ex.Message);
                throw new PattersonPeakSavingException(ex.Message);
            }
        }
    }
}
