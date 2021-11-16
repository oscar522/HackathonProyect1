using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace WebApplication1.Models
{
    public class Orders
    {
        public int id { get; set; }
        public string order_id { get; set; }
        public DateTime created_at { get; set; }
        public DateTime delivery_start_date {get; set;}
        public DateTime delivery_end_date {get; set;}
        public double latitude { get; set; }
        public double longitude { get; set; }
        public string method { get; set; }
        public int quantity { get; set; }
        public double weight { get; set; }
        public double volume { get; set; }
        public DateTime date {get; set;}
        public DateTime? delivery_date { get; set;}
        public int? track_number { get; set; }
        public int? track_order { get; set; }
	}
}
