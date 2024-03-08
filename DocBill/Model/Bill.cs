using System;
using System.ComponentModel.DataAnnotations.Schema;

#nullable disable

namespace DocBill.Model
{

    #region Data structures

    public enum PaymentStatus : short
    {

        Open = 0,

        Done = 1

    }

    #endregion

    [Table("bill")]
    public class Bill
    {

        [Column("id")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }

        [Column("number")]
        public string Number { get; set; }

        [Column("due_date")]
        public DateTime DueDate { get; set; }

        [Column("issuer")]
        public int IssuerId { get; set; }

        [Column("status")]
        public PaymentStatus Status { get; set; }

        [Column("created")]
        public DateTime Created { get; set; }

        [Column("modified")]
        public DateTime Modified { get; set; }

        public virtual Issuer Issuer { get; set; }

    }

}

#nullable enable