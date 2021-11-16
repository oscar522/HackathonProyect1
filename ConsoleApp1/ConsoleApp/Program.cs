using System;
using WebApi.Controllers;
using WebApi.Services;

namespace ConsoleApp
{
    class Program
    {
        static void Main(string[] args)
        {
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


        }
    }
}
