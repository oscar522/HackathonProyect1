using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebApplication1.Data;
using WebApplication1.Models;
namespace WebApplication1.Services
{

    public class ShortRoutes
    {

        private static Orders[] ordenarGen;
        private static double calAntiguo = 0;
        private static double calNuevo = 0;
        private static int campo = 0;
        private static int cont = 1;
        private static List<Orders> listFinal;
        public List<Orders> datos(List<Orders> orders)
        {
            List<List<Orders>> ordenes = new List<List<Orders>>();
            int camiones = orders.Max(x => x.track_number.Value);
            listFinal = new List<Orders>();
            for (int i = 0; i < camiones; i++)
            {
                cont = 1;
                campo = 0;
                calAntiguo = 0;
                calNuevo = 0;
                List<Orders> lst = orders.Where(x => (x.track_number.Value == i + 1)).ToList();
                ordenarGen = new Orders[lst.Count()];
                ordenarGen = lst.ToArray();
                ordenarGen[0].track_order = 1;
                //Console.WriteLine(ordenarGen[0].id + " : " + ordenarGen[0].track_order);
                Ordenamiento(ordenarGen, 0);
                listFinal.AddRange(ordenarGen.ToList());
            }
            return listFinal;


        }


        private static void Ordenamiento(Orders[] ordenar, int i)
        {
            bool validacion = false;
            calAntiguo = 1000;
            for (int j = 0; j < ordenar.Length; j++)
            {
                if (ordenar[j].track_order == null)
                {
                    if (i != j)
                    {
                        validacion = true;
                        calNuevo = CalcularDistancia(ordenar[i].latitude, ordenar[j].latitude, ordenar[i].longitude, ordenar[j].longitude) / 10000;
                        if (calNuevo < calAntiguo)
                        {
                            calAntiguo = calNuevo;
                            campo = j;
                        }
                    }
                }
            }
            if (validacion)
            {
                cont = cont + 1;
                ordenarGen[campo].track_order = cont;
                //Console.WriteLine(ordenarGen[campo].id + " : " + ordenarGen[campo].track_order + " : " + calAntiguo);
                Ordenamiento(ordenarGen, campo);
            }
        }


        //Funcion que calcula la distancia entre dos puntos
        private static double CalcularDistancia(double lat1, double lat2, double lon1, double lon2)
        {
            const double r = 6371; // metros

            var calcLat = Math.Sin((lat2 - lat1) / 2);
            var calLon = Math.Sin((lon2 - lon1) / 2);
            var q = calcLat * calcLat + Math.Cos(lat1) * Math.Cos(lat2) * calLon * calLon;
            return 2 * r * Math.Asin(Math.Sqrt(q));
        }
    }


}