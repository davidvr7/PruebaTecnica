using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using PruebaTecnica20_06.Data;
using PruebaTecnica20_06.Models;
using PruebaTecnica20_06.DTO;
using System.Security.Cryptography;

namespace PruebaTecnica20_06.Controller
{
    [ApiController]
    [Route("[controller]")]
    public class AccountController : ControllerBase
    {
        // Consultar movimientos
        [HttpGet("movimientos")]
        public IActionResult ConsultarMovimientos(int accountId)
        {
            var cuenta = DataService.Accounts.FirstOrDefault(a => a.Id == accountId);
            if (cuenta != null)
            {
                return Ok(cuenta.Movements);
            }
            return NotFound(new { error = "Cuenta no encontrada" });
        }

        // Sacar dinero
        [HttpPost("sacar")]
        public IActionResult SacarDinero([FromBody] SacarDineroRequest request)
        {
            var cuenta = DataService.Accounts.FirstOrDefault(a => a.Id == request.AccountId);
            var tarjeta = DataService.Cards.FirstOrDefault(c => c.Id == request.CardId && c.AccountId == request.AccountId);

            if (cuenta != null && tarjeta != null)
            {
                if (!tarjeta.IsActive)
                    return BadRequest(new { error = "Tarjeta no activada" });

                var comision = request.IsOtherBankATM ? 5 : 0;
                var totalARetirar = request.Amount + comision;

                if (tarjeta.IsCredit)
                {
                    if (tarjeta.WithdrawalLimit >= totalARetirar && tarjeta.CreditLimit >= totalARetirar)
                    {
                        // Realizar el retiro
                        tarjeta.CreditLimit -= totalARetirar;

                        // Registrar movimiento de retiro
                        cuenta.Movements.Add(new Movement
                        {
                            Date = DateTime.Now,
                            Type = "Retirada",
                            Amount = request.Amount,
                            Description = "Retiro de efectivo"
                        });

                        // Registrar comisión si aplica
                        if (comision > 0)
                        {
                            cuenta.Movements.Add(new Movement
                            {
                                Date = DateTime.Now,
                                Type = "Comisión",
                                Amount = comision,
                                Description = "Comisión por uso de cajero de otro banco"
                            });
                        }

                        // Devolver saldo actualizado
                        return Ok(new { saldo = tarjeta.CreditLimit });
                    }
                    return BadRequest(new { error = "Límite de crédito o límite de retiro excedido" });
                }
                else
                {
                    if (cuenta.Balance >= totalARetirar)
                    {
                        cuenta.Balance -= totalARetirar;
                        cuenta.Movements.Add(new Movement
                        {
                            Date = DateTime.Now,
                            Type = "Retirada",
                            Amount = request.Amount,
                            Description = "Retiro de efectivo"
                        });

                        if (comision > 0)
                        {
                            cuenta.Movements.Add(new Movement
                            {
                                Date = DateTime.Now,
                                Type = "Comisión",
                                Amount = comision,
                                Description = "Comisión por uso de cajero de otro banco"
                            });
                        }
                        return Ok(new { saldo = cuenta.Balance });
                    }
                    return BadRequest(new { error = "Saldo insuficiente" });
                }
            }
            return NotFound(new { error = "Cuenta o tarjeta no encontrada" });
        }

        // Ingresar dinero
        [HttpPost("ingresar")]
        public IActionResult IngresarDinero([FromBody] IngresarDineroRequest request)
        {
            var cuenta = DataService.Accounts.FirstOrDefault(a => a.Id == request.AccountId);
            var tarjeta = DataService.Cards.FirstOrDefault(c => c.Id == request.CardId && c.AccountId == request.AccountId);

            if (cuenta != null && tarjeta != null)
            {
                if (!tarjeta.IsActive)
                    return BadRequest(new { error = "Tarjeta no activada" });

                cuenta.Balance += request.Amount;
                cuenta.Movements.Add(new Movement
                {
                    Date = DateTime.Now,
                    Type = "Ingreso",
                    Amount = request.Amount,
                    Description = "Ingreso de efectivo"
                });

                return Ok(new { saldo = cuenta.Balance });
            }
            return NotFound(new { error = "Cuenta o tarjeta no encontrada" });
        }

        // Hacer transferencias
        [HttpPost("transferir")]
        public IActionResult HacerTransferencia([FromBody] TransferirRequest request)
        {
            var cuenta = DataService.Accounts.FirstOrDefault(a => a.Id == request.AccountId);
            var tarjeta = DataService.Cards.FirstOrDefault(c => c.Id == request.CardId && c.AccountId == request.AccountId);

            if (!IsValidIban(request.IbanDestino))
                return BadRequest(new { error = "IBAN inválido" });

            if (cuenta != null && tarjeta != null)
            {
                if (!tarjeta.IsActive)
                    return BadRequest(new { error = "Tarjeta no activada" });

                var comision = request.IsDifferentBank ? 2 : 0;
                var totalATransferir = request.Amount + comision;

                if (cuenta.Balance >= totalATransferir)
                {
                    cuenta.Balance -= totalATransferir;
                    cuenta.Movements.Add(new Movement
                    {
                        Date = DateTime.Now,
                        Type = "Transferencia",
                        Amount = request.Amount,
                        Description = $"Transferencia a {request.IbanDestino}"
                    });

                    if (comision > 0)
                    {
                        cuenta.Movements.Add(new Movement
                        {
                            Date = DateTime.Now,
                            Type = "Comisión",
                            Amount = comision,
                            Description = "Comisión por transferencia a otro banco"
                        });
                    }
                    return Ok(new { saldo = cuenta.Balance });
                }
                return BadRequest(new { error = "Saldo insuficiente" });
            }
            return NotFound(new { error = "Cuenta o tarjeta no encontrada" });
        }

        // Activar tarjeta
        [HttpPost("activar")]
        public IActionResult ActivarTarjeta([FromBody] ActivarTarjetaRequest request)
        {
            var tarjeta = DataService.Cards.FirstOrDefault(c => c.Id == request.CardId);
            if (tarjeta != null)
            {
                tarjeta.IsActive = true;
                return Ok(new { message = "Tarjeta activada correctamente" });
            }
            return NotFound(new { error = "Tarjeta no encontrada" });
        }

        // Cambiar código PIN
        [HttpPost("cambiar-pin")]
        public IActionResult CambiarPin([FromBody] CambiarPinRequest request)
        {
            var tarjeta = DataService.Cards.FirstOrDefault(c => c.Id == request.CardId);
            if (tarjeta != null)
            {
                // Encriptar el nuevo PIN antes de guardarlo
                string pinEncriptado = EncriptarPin(request.NewPin);
                tarjeta.PIN = pinEncriptado;

                return Ok(new { message = "PIN cambiado correctamente" });
            }
            return NotFound(new { error = "Tarjeta no encontrada" });
        }

        // Método validar IBAN
        private bool IsValidIban(string iban)
        {
            return !string.IsNullOrEmpty(iban) && iban.Length >= 20;
        }

        private string EncriptarPin(string pin)
        {
            // Generar una sal aleatoria
            byte[] salt;
            new RNGCryptoServiceProvider().GetBytes(salt = new byte[16]);

            // Configurar el algoritmo de derivación de clave PBKDF2
            var pbkdf2 = new Rfc2898DeriveBytes(pin, salt, 10000, HashAlgorithmName.SHA256);
            byte[] hash = pbkdf2.GetBytes(20); // Tamaño del hash SHA-256

            // Combinar la sal y el hash
            byte[] hashBytes = new byte[36];
            Array.Copy(salt, 0, hashBytes, 0, 16);
            Array.Copy(hash, 0, hashBytes, 16, 20);

            // Convertir a cadena base64 para almacenamiento
            string passwordHash = Convert.ToBase64String(hashBytes);

            return passwordHash;
        }

    }
}
