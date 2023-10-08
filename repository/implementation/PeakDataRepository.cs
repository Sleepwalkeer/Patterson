using Patterson.exception;
using Patterson.model;
using Patterson.persistence;
using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace Patterson.repository.implementation
{
    public class PeakDataRepository : IPeakDataRepository
    {
        private readonly PattersonDBContext context;

        public PeakDataRepository(PattersonDBContext context)
        {
            this.context = context;
        }
        public void SaveData(List<PeakData> peaksData)
        {
            try
            {
                var elementDbSet = context.PeakDataList;
                elementDbSet.AddRange(peaksData);
                context.SaveChanges();
            }
            catch (Exception ex)
            {
                MessageBox.Show("An error occurred while saving peaks data" + ex.Message);
                throw new PeakDataSavingException(ex.Message);
            }
        }
    }
}

