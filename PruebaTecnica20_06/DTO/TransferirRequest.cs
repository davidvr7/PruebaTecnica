﻿namespace PruebaTecnica20_06.DTO
{
    public class TransferirRequest
    {
        public int AccountId { get; set; }
        public int CardId { get; set; }
        public decimal Amount { get; set; }
        public string IbanDestino { get; set; }
        public bool IsDifferentBank { get; set; }
    }
}
