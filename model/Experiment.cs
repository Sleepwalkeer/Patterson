using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Patterson.model
{
    [Table("experiment", Schema = "public")]
    public class Experiment
    {
        public Experiment(Guid id, DateTime date, Element element)
        {
            Id = id;
            Date = date;
            ElementId = element.Id;
            Element = element;
        }

        [Column("id")]
        public Guid Id { get; set; }

        [Column("datetime")]
        public DateTime Date { get; set; }

        [Column("description")]
        public string Description { get; set; }

        [Required]
        [Column("elem_id")]
        public Guid ElementId { get; set; }

        [ForeignKey("ElementId")]
        public Element Element { get; set; }
    }
}
