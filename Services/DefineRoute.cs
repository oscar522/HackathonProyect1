using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebApplication1.Models;

namespace WebApplication1.Services
{
    public abstract class DefineRoute
    {
        public static List<Orders> Define(List<Orders> orders)
        {
            double minX, maxX, minY, maxY;
            int orderQty;

            minX = orders.Min(x => x.latitude);
            minY = orders.Min(x => x.longitude);
            maxX = orders.Max(x => x.latitude);
            maxY = orders.Max(x => x.longitude);
            orderQty = orders.Count();

            //Define la cantidad de ordenes que se puede enviar, la diferencia hay que descartar
            GroupOrders groups = DefineRoute.Verify(orderQty);

            while (groups.Total() < orders.Count(x=>x.delivery_date.HasValue ))
            {
                orders.Where(x => (x.delivery_end_date > x.delivery_date)).OrderBy(x => x.longitude).FirstOrDefault().delivery_date = null;
            }

            //Crea las rutas de 98 ordenes
            int track_number = 1;
            CreateTrack(ref orders, ref track_number, 98, groups.group98);
            CreateTrack(ref orders, ref track_number, 99, groups.group99);
            CreateTrack(ref orders, ref track_number, 100, groups.group100);

            return orders;
        }

        private static void CreateTrack(ref List<Orders> orders, ref int track_number, int group, int qty)
        {
            int i;
            for (i = 0; i < qty; i++)
            {
                List<Orders> lst = orders.Where(x => (!x.track_number.HasValue) && (x.delivery_date.HasValue)).OrderBy(x => x.longitude).Take(group).ToList();
                foreach (Orders item in lst)
                {
                    item.track_number = track_number;
                }
                track_number++;
            }

        }

        /// <summary>
        /// Verifica la cantidad de ordenes que puede enviar optimizando el uso de los camiones
        /// </summary>
        /// <param name="orderQty">Cantidad de ordenes iniciales</param>
        /// <returns> Retorna una clase en donde informa cuántas rutas hay que crear de 98, de 99 y de 100</returns>
        private static GroupOrders Verify(int orderQty)
        {
            
            GroupOrders groups = new GroupOrders();

            if (orderQty < 98) return groups;
            /*if (orderQty % 100 == 0)
            {
                groups.group100 = (orderQty / 100);
                return groups;
            }
            if (orderQty % 99 == 0)
            {
                groups.group99 = (orderQty / 99);
                return groups;
            }
            if (orderQty % 98 == 0)
            {
                groups.group98 = (orderQty / 98);
                return groups;
            }


            GroupOrders o1 = Verify(orderQty % 100);
            o1.group100 = orderQty / 100;
            if (o1.Total() == orderQty) return o1;

            GroupOrders o2 = Verify(orderQty % 99);
            o2.group99 = orderQty / 99;
            if (o2.Total() == orderQty) return o2;

            GroupOrders o3 = Verify(orderQty % 98);
            o3.group98 = orderQty / 98;
            if (o3.Total() == orderQty) return o3;

            if (o1.Total() < o2.Total()) o1 = o2;
            if (o1.Total() < o3.Total()) o1 = o3;
            */

            GroupOrders o1 = Verify(orderQty % 98);
            o1.group98 = orderQty / 98;

            while ((o1.Total() < orderQty) && (o1.group98 > 0))
            {
                o1.group99++;
                o1.group98--;
            }

            while ((o1.Total() < orderQty) && (o1.group99 > 0))
            {
                o1.group100++;
                o1.group99--;
            }

            return o1;

        }

    }
}
