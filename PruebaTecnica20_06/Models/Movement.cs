namespace PruebaTecnica20_06.Models
{
    public class Movement
    {
        public int Id { get; set; }
        public DateTime Date { get; set; }
        public decimal Amount { get; set; }
        public string Type { get; set; } // Ingreso, Retirada, Comisión, Transferencia
        public string Description { get; set; }
        public int AccountId { get; set; }
        public Account Account { get; set; }
    }
}
