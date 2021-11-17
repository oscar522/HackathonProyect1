using System;
using WebApi.Controllers;
using WebApi.Services;

namespace ConsoleApp
{
    class Program
    {
        static void Main(string[] args)
        {
            /*
            Console.WriteLine("Inicio de proceso");
            string DateRoutes = "2021-09-07";
            try {
                RoutesService RoutesService_ = new RoutesService();
                RoutesService_.RoutesCreate(Convert.ToDateTime(DateRoutes));
            }
            catch (Exception e) { 
                Console.WriteLine("ups ! algo Fallo : "+ e.Message);

            }
            Console.WriteLine("Fin de proceso");
            */
            Console.WriteLine("Inicio de proceso");
            DateTime date = new DateTime(2021,09,08);
            RoutesService RoutesService_ = new RoutesService();
            try
            {
                while (date <= new DateTime(2021, 09, 08))
                {
                    RoutesService_.RoutesCreate(date);
                    Console.WriteLine("Día " + date.ToShortDateString() + " procesado");
                    date = date.AddDays(1);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("ups ! algo Fallo : " + e.Message);

            }
            Console.WriteLine("Fin de proceso");

        }
    }
}
