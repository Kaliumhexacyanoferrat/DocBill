using System;
using System.ComponentModel.DataAnnotations.Schema;

#nullable disable

namespace DocBill.Model
{

    #region Data structures

    public enum PaymentStatus : short
    {

        /// <summary>
        /// Yet to be payed.
        /// </summary>
        Open = 0,

        /// <summary>
        /// Payed.
        /// </summary>
        Done = 1,

        /// <summary>
        /// Open AND due.
        /// </summary>
        Due = 99

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

        [Column("billing_date")]
        public DateTime BillingDate { get; set; }

        [Column("due_date")]
        public DateTime DueDate { get; set; }

        [Column("amount")]
        public double Amount { get; set; }

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