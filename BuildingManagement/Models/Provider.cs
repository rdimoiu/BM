using System.ComponentModel.DataAnnotations;

namespace BuildingManagement.Models
{
    public class Provider
    {
        public int ID { get; set; }

        [Required]
        [StringLength(30)]
        public string Name { get; set; }

        [Required]
        [StringLength(15)]
        public string FiscalCode { get; set; }

        [Required]
        [StringLength(15)]
        public string TradeRegister { get; set; }

        [Required]
        [StringLength(50)]
        public string Address { get; set; }

        [Required]
        [StringLength(15)]
        public string Phone { get; set; }

        [Required]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }

        [Required]
        [StringLength(25)]
        public string BankAccount { get; set; }

        [Required]
        [StringLength(30)]
        public string Bank { get; set; }

        public bool TVAPayer { get; set; }
    }
}