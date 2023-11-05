using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Patterson.model
{
    [Table("element")]
    public class Element
    {
        [Column("id")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }

        [MaxLength(255)]
        [Column("name")]
        public string Name { get; set; }

        [Column("deltar")]
        public double deltaR { get; set; }

        public Element(string name, double deltaR)
        {
            this.Name = name;
            this.deltaR = deltaR;
        }

        public Element()
        {

        }

    }
}
