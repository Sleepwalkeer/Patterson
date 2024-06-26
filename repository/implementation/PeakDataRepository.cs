﻿using Patterson.model;
using Patterson.persistence;
using System.Collections.Generic;

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
            context.PeaksData.AddRange(peaksData);
            context.SaveChanges();
        }
    }
}

