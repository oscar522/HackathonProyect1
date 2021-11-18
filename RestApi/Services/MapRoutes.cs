using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebApplication1.Models;

namespace WebApplication1.Services
{
    public class MapRoutes
    {
        public List<Models.Orders> Orders;
        /// <summary>
        /// Orientación del mapa 0:Vertical, 1:Horizontal
        /// </summary>
        public int Orientation { get; set; }



        private double MinLatitude { get; }
        private double MinLongitude { get; }
        private double MaxLatitude { get; }
        private double MaxLongitude { get; }

        private int OrderQty;

        public int Group { get; }

        public MapRoutes Group1;
        public MapRoutes Group2;
        public List<Models.Orders> discarded;
        public MapRoutes(List<Orders> orders, int orientation, int group)
        {
            Orders = orders;
            Orientation = orientation;
            Group = group;
            MinLatitude = orders.Min(x => x.latitude);
            MinLongitude = orders.Min(x => x.longitude);
            MaxLatitude = orders.Max(x => x.latitude);
            MaxLongitude = orders.Max(x => x.longitude);
            OrderQty = orders.Count();
        }

        /// <summary>
        /// Crea las rutas
        /// </summary>
        public void Reorganize(ref GroupOrders g)
        {
            this.discarded = new List<Orders>();
            if (this.OrderQty > 100)
            {
                int groups2 = OrderQty / 2;  //Cantidad de ordenes de grupo1
                List<Orders> list = (this.Orientation == 0) ? list = this.Orders.OrderBy(x => x.longitude).ToList() : this.Orders.OrderBy(x => x.latitude).ToList();
                if (groups2 > 98)
                {
                    Group1 = new MapRoutes(list.Take(OrderQty - groups2).ToList(), (this.Orientation == 0) ? 1 : 0, 1);
                    Group2 = new MapRoutes(list.TakeLast(groups2).ToList(), (this.Orientation == 0) ? 1 : 0, 2);

                    this.Group2.Reorganize(ref g);
                    this.Group1.Orders.AddRange(this.Group2.discarded);
                    this.Group1.OrderQty = this.Group1.Orders.Count();
                    this.Group1.Reorganize(ref g);
                    this.discarded = this.Group1.discarded;
                    this.Orders = this.Group1.Orders;
                    this.Orders.AddRange(Group2.Orders);
                    this.OrderQty = this.Orders.Count();
                }
                else
                {
                    if (g.group98 > 0)
                    {
                        groups2 = 98;
                        g.group98--;
                    }
                    else if (g.group99 > 0)
                    {
                        groups2 = 99;
                        g.group99--;
                    }
                    else
                    {
                        groups2 = 100;
                        g.group100--;
                    }
                    this.discarded = list.Take(OrderQty - groups2).ToList();
                    this.Orders = list.TakeLast(groups2).ToList();
                    this.OrderQty = this.Orders.Count();
                }
            }
        }

        internal List<Orders> CreateTracks(ref int v)
        {
            if (Group1 == null)
            {
                foreach (Orders item in Orders)
                    item.track_number = v;
                v++;
            }
            else
            {
                this.Orders = Group1.CreateTracks(ref v);
                this.Orders.AddRange(Group2.CreateTracks(ref v));
            }
            return this.Orders;
        }
    }
}
