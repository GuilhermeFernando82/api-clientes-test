using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Apivscode2.Models
{
    public class CustomersRequestDTO
    {
        public string Name {get; set;}
        public string Cpf { get; set;}
        public string PublicPlace { get; set;}
        public string Neighborhood { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string Cep { get; set; }
    }
}