﻿using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Patterson.model
{
    [Table("patterson_peak", Schema = "public")]
    public class PattersonPeak
    {
        [Key, Column("exp_id", Order = 1)]
        public Guid ExperimentId { get; set; }

        [ForeignKey("ExperimentId")]
        public Experiment Experiment { get; set; }

        [Key, Column("is_uv_exposed", Order = 2)]
        public bool IsUvExposed { get; set; }

        [Key, Column("peak_id", Order = 3)]
        public int PeakId { get; set; }

        [Key, Column("pu", Order = 4)]
        public double Pu { get; set; }
    }
}