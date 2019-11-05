using System;
using System.Collections.Generic;
using System.Text;

namespace erikitofunctions
{
    public class CreateGreetingRequest
    {
        public string Number { get; set; }
        public string FirstName { get; set; }
        public override string ToString() => $"{FirstName} {Number}";
    }
}
