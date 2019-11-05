using System;
using System.Collections.Generic;
using System.Text;

namespace erikitofunctions
{
    public class GreetingRequest
    {
        public string Number { get; set; }
        public string Message { get; set; }
        public override string ToString() => $"{Number} {Message}";
    }
}
