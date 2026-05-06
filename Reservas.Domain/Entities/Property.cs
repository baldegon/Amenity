using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reservas.Domain.Entities
{
    public class Property
    {
        public int Id { get; set; }
        public string? Title { get; set; }   
        public decimal PricePerNight { get; set; }
        public int UserId { get; set; }
    }
}
