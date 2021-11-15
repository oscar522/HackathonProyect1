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
            int orderQty, orderMax;

            minX = orders.Min(x => x.latitude);
            minY = orders.Min(x => x.longitude);
            maxX = orders.Max(x => x.latitude);
            maxY = orders.Max(x => x.longitude);
            orderQty = orders.Count();

            //Define la cantidad de ordenes que puede enviar
            orderMax = DefineRoute.Verify(orderQty);

            return orders;
        }

        public static int Verify(int orderQty)
        {
            if (orderQty < 98) return 0;
            if ((orderQty % 100 == 0)||(orderQty % 99 == 0)|| (orderQty % 98 == 0)) return orderQty;
            
            int o1 = (orderQty / 100)*100 + Verify(orderQty % 100);
            if (o1 == orderQty) return orderQty;

            int o2 = (orderQty / 99)*99 + Verify(orderQty % 99);
            if (o2 == orderQty) return orderQty; 
            
            int o3 = (orderQty / 98)*98 + Verify(orderQty % 98);
            if (o3 == orderQty) return orderQty;

            if (o1 < o2) o1 = o2;
            if (o1 < o3) o1 = o3;
            return o1;

        }
    }
}
