using System.ComponentModel.DataAnnotations;

namespace BuildingManagement.Models
{
    public class SubClient
    {
        public int ID { get; set; }

        [Required]
        [StringLength(50, MinimumLength = 1)]
        public string Name { get; set; }

        [StringLength(15)]
        public string Phone { get; set; }

        [StringLength(50)]
        public string Country { get; set; }

        [StringLength(50)]
        public string State { get; set; }

        [StringLength(50)]
        public string City { get; set; }

        [StringLength(50)]
        public string Street { get; set; }

        [StringLength(50)]
        public string Contact { get; set; }

        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }

        [StringLength(30)]
        public string IBAN { get; set; }

        [StringLength(50)]
        public string Bank { get; set; }

        [StringLength(20)]
        public string CNP { get; set; }

        [StringLength(20)]
        public string FiscalCode { get; set; }

        [Required]
        public int ClientID { get; set; }
        public virtual Client Client { get; set; }
    }
}