using System;
using System.ComponentModel.DataAnnotations.Schema;

#nullable disable

namespace DocBill.Model
{

    [Table("issuer")]
    public class Issuer
    {

        [Column("id")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }

        [Column("name")]
        public string Name { get; set; }

        [Column("created")]
        public DateTime Created { get; set; }

        [Column("modified")]
        public DateTime Modified { get; set; }

    }

}

#nullable enable