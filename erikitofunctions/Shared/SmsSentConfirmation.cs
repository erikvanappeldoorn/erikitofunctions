using System;
using System.Collections.Generic;
using System.Text;

namespace erikitofunctions
{
    public class SmsSentConfirmation
    {
        public string Number { get; set; }
        public string Message { get; set; }
        public string ReceiptId { get; set; }
        public override string ToString() => $"{ReceiptId} {Number} {Message}";
    }
}
