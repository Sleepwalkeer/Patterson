using Patterson.model;
using Patterson.persistence;
using System.Collections.Generic;
using System;
using System.Windows.Forms;
using Patterson.exception;

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

