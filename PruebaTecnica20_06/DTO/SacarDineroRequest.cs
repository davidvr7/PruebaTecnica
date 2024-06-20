namespace PruebaTecnica20_06.DTO
{
    public class SacarDineroRequest
    {
        public int AccountId { get; set; }
        public int CardId { get; set; }
        public decimal Amount { get; set; }
        public bool IsOtherBankATM { get; set; }
    }
}
