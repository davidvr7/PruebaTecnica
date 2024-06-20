using Xunit;
using Microsoft.AspNetCore.Mvc;
using PruebaTecnica20_06.Controller;

namespace Tests
{
    [TestClass]
    public class UnitTest1
    { 

        [Fact]
        public void ConsultarMovimientos_CuentaNoExistente()
        {
        
            var controller = new AccountController();

            var accountId = 999; // Cuenta no existente
             
            var result = controller.ConsultarMovimientos(accountId) as NotFoundObjectResult;
             
            Xunit.Assert.NotNull(result);
            Xunit.Assert.Equal("{ error = Cuenta no encontrada }", result.Value.ToString());
        }
    }
}