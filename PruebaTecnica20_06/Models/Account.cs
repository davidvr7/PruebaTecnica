namespace PruebaTecnica20_06.Models
{
    public class Account
    {
        public int Id { get; set; }
        public string IBAN { get; set; }
        public decimal Balance { get; set; }
        public List<Movement> Movements { get; set; }
    }
}
