using System.ComponentModel.DataAnnotations;

namespace PruebaTecnica20_06.Models
{
    public class Card
    {
        public int Id { get; set; }
        public string Number { get; set; }

        [MaxLength(200)] 
        public string PIN { get; set; }

        public bool IsActive { get; set; }
        public bool IsCredit { get; set; }
        public decimal CreditLimit { get; set; }
        public decimal WithdrawalLimit { get; set; }
        public int AccountId { get; set; }
        public Account Account { get; set; }
    }
}
