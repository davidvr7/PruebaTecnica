using PruebaTecnica20_06.Models;

namespace PruebaTecnica20_06.Data
{
    public static class DataService
    {
        public static List<Account> Accounts = new List<Account>
        {
            new Account
            {
                Id = 1,
                IBAN = "ES9820385778983000760236",
                Balance = 1000,
                Movements = new List<Movement>
                {
                    new Movement { Id = 1, Date = DateTime.Now.AddDays(-5), Amount = 500, Type = "Ingreso", Description = "Ingreso de nómina" },
                    new Movement { Id = 2, Date = DateTime.Now.AddDays(-3), Amount = 50, Type = "Comisión", Description = "Comisión mantenimiento" },
                    new Movement { Id = 3, Date = DateTime.Now.AddDays(-2), Amount = 200, Type = "Retirada", Description = "Retirada de efectivo" }
                }
            },
            new Account
            {
                Id = 2,
                IBAN = "ES4500112233445566778899",
                Balance = 2500,
                Movements = new List<Movement>
                {
                    new Movement { Id = 4, Date = DateTime.Now.AddDays(-10), Amount = 1000, Type = "Ingreso", Description = "Ingreso de transferencia" },
                    new Movement { Id = 5, Date = DateTime.Now.AddDays(-7), Amount = 300, Type = "Retirada", Description = "Retirada de cajero automático" }
                }
            }
        };

        public static List<Card> Cards = new List<Card>
        {
            new Card
            {
                Id = 1,
                Number = "1111-2222-3333-4444",
                PIN = "1234",
                IsActive = true,
                IsCredit = false,
                CreditLimit = 0,
                WithdrawalLimit = 500,
                AccountId = 1,
                Account = Accounts[0]
            },
            new Card
            {
                Id = 2,
                Number = "5555-6666-7777-8888",
                PIN = "5678",
                IsActive = true,
                IsCredit = true,
                CreditLimit = 2000,
                WithdrawalLimit = 3000,
                AccountId = 1,
                Account = Accounts[0]
            },
            new Card
            {
                Id = 3,
                Number = "9999-8888-7777-6666",
                PIN = "4321",
                IsActive = false,
                IsCredit = false,
                CreditLimit = 0,
                WithdrawalLimit = 300,
                AccountId = 2,
                Account = Accounts[1]
            }
        };
    }
}
