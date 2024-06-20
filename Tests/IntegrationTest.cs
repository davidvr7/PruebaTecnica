using Microsoft.AspNetCore.Mvc;
using PruebaTecnica20_06.Controller;
using PruebaTecnica20_06.Data;
using PruebaTecnica20_06.Models;

namespace Tests
{

    [TestClass]
    public class IntegrationTest
    {
        private AccountController _controller;

        [TestInitialize]
        public void Setup()
        {
            // Configuración inicial de datos
            DataService.Accounts = new System.Collections.Generic.List<Account>
            {
                new Account
                {
                    Id = 1,
                    IBAN = "ES9820385778983000760236",
                    Balance = 1000,
                    Movements = new System.Collections.Generic.List<Movement>
                    {
                        new Movement { Id = 1, Date = System.DateTime.Now.AddDays(-5), Amount = 500, Type = "Ingreso", Description = "Ingreso de nómina" },
                        new Movement { Id = 2, Date = System.DateTime.Now.AddDays(-3), Amount = 50, Type = "Comisión", Description = "Comisión mantenimiento" },
                        new Movement { Id = 3, Date = System.DateTime.Now.AddDays(-2), Amount = 200, Type = "Retirada", Description = "Retirada de efectivo" }
                    }
                }
            };

            _controller = new AccountController();
        }

        [TestMethod]
        public void ConsultarMovimientos_CuentaNoExistente()
        { 
            var accountId = 999; // Cuenta no existente
             
            var result = _controller.ConsultarMovimientos(accountId) as NotFoundObjectResult;
             
            Assert.IsNotNull(result);
            Xunit.Assert.Equal("{ error = Cuenta no encontrada }", result.Value.ToString());
        }

        [TestMethod]
        public void ConsultarMovimientos_CuentaExistente()
        { 
            var accountId = 1;
             
            var result = _controller.ConsultarMovimientos(accountId) as OkObjectResult;
            var movements = result.Value as System.Collections.Generic.List<Movement>;
             
            Assert.IsNotNull(result);
            Assert.IsNotNull(movements);
            Assert.AreEqual(3, movements.Count);
        }
    }
}