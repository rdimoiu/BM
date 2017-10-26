namespace BuildingManagement.Models
{
    public class Provider
    {
        public int ID { get; set; }

        public string Name { get; set; }

        public string FiscalCode { get; set; }

        public string TradeRegister { get; set; }

        public string Address { get; set; }

        public string Phone { get; set; }

        public string Email { get; set; }

        public string BankAccount { get; set; }

        public string Bank { get; set; }

        public bool TVAPayer { get; set; }
    }
}