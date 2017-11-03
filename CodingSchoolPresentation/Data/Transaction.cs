using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using MainSite.Models;

namespace MainSite.Data
{
    public class Transaction
    {
        [Column("TransactionID")]
        public int TransactionId { get; set; }

        [Column("Amount")]
        public decimal Amount { get; set; }

        [Column("Created")]
        public DateTime Created { get; set; }

        [Column("UserId")]
        public string UserId { get; set; }

        [Column("IBAN")]
        [Required]
        public string Iban { get; set; }

        [ForeignKey(nameof(UserId))]
        public ApplicationUser User { get; set; }
    }
}
