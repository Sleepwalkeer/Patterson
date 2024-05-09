using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

namespace Patterson.model
{
    [Table("peak_data")]
    public class PeakData
    {
        [Key, Column("experiment_id", Order = 1)]
        public Guid ExperimentId { get; set; }

        [ForeignKey("ExperimentId")]
        public Experiment Experiment { get; set; }

        [Key, Column("is_uv_exposed", Order = 2)]
        public bool IsUvExposed { get; set; }

        [Key, Column("peak_id", Order = 3)]
        public int PeakId { get; set; }

        [Column("intensity")]
        public double Intensity { get; set; }

        [Column("double_theta")]
        public double DoubleTheta { get; set; }

        [Column("plg")]
        public double Plg { get; set; }

        [Column("f_squared")]
        public double FSquared { get; set; }

        [Column("d_over_n")]
        public double DOverN { get; set; }

        [Column("one_over_d")]
        public double OneOverD { get; set; }

        [NotMapped]
        public double sinThetta { get; set; }


        public PeakData(double intensity, double thetta)
        {
            this.Intensity = intensity;
            this.DoubleTheta = thetta;
        }

        public PeakData()
        {
        }

        public static List<PeakData> SortByTheta(List<PeakData> peakList)
        {
            List<PeakData> sortedList = peakList.OrderBy(peak => peak.DoubleTheta).ToList();

            return sortedList;
        }
    }
}
