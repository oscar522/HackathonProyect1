using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using WebApi.Entities;
using WebApi.Helpers;
using WebApi.Models;
using WebApplication1.Data;
using WebApplication1.Models;

namespace WebApi.Services
{

    public class RoutesService
    {
        

        public RoutesService()
        {
        }
        public List<Orders> RoutesCreate(Connection context, DateTime RouteDate)
        {
            List<Orders> list = new List<Orders>();

            string Result = "";

            try {
                ////////////// PUNTO 1
                string sql = "EXEC [dbo].[SelectOrders] @dateDelivery, @deliveryMin, @deliveryMax  ";

                List<SqlParameter> parms = new List<SqlParameter>
                {
                    // Create parameter(s)    
                    new SqlParameter { ParameterName = "@dateDelivery", Value = RouteDate },
                    new SqlParameter { ParameterName = "@deliveryMin", Value = 25000 },
                    new SqlParameter { ParameterName = "@deliveryMax", Value = 28000 }

                };

                list = context.Orders.FromSqlRaw<Orders>(sql, parms.ToArray()).ToList();
                
                ////////////// PUNTO 2
                ///
                
               
            }
            catch (Exception e ) {
                Result = e.Message;
            }

            
            return list;
        }
    }
}