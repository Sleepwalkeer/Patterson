using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Patterson.model
{
    [Table("experiment")]
    public class Experiment
    {
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

        public Experiment(Guid id, DateTime date, Element element, string description)
        {
            Id = id;
            Date = date;
            ElementId = element.Id;
            Element = element;
            Description = description;
        }
    }
}
