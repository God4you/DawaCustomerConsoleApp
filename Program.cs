using DawaCustomerConsoleApp.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace DawaCustomerConsoleApp
{
    class Program
    {
        private static string APIUrl = "https://localhost:44353/api/Estate";

        static void Main(string[] args)
        {
            GetCustomer();
       
            Console.Read();
        }
        
        public static async void GetCustomer()
        {
            List<string> customerList = new List<string>();

            HttpClient httpClient = new HttpClient();

            Estate estate = null;

            try
            {
                // connecting to the Rest Api
                string response = await httpClient.GetStringAsync(APIUrl);
                // converting from json to c# object liste.
                estate = Newtonsoft.Json.JsonConvert.DeserializeObject<Estate>(response);
            }
            catch (Exception e)
            {
                
                
                throw new Exception("Kunne ikke forbinde til serveren", e);

            }

            
            SqlDataReader reader = null;
            try
            {
                //connection string to db
                SqlConnection conn = new SqlConnection("Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog=master;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False");
                conn.Open();

                //Sql query Sorting to find the specific estate Customer with the conditions" 
                SqlCommand cmd = new SqlCommand("SELECT * FROM CustomerEstateDb.dbo.Estate INNER JOIN CustomerEstateDb.dbo.Customer ON CustomerEstateDb.dbo.Estate.Owner_id = CustomerEstateDb.dbo.Customer.Id WHERE Streetname= '" + estate.vejnavn + "' AND Housenumber= '" + estate.husnr + "' AND Zipcode= '" + estate.postnr + "'", conn);
                reader = cmd.ExecuteReader();
            }
            catch (Exception e)
            {
                
                throw new Exception("Sql fejlede, fejlbesked:", e);
            }
            if (reader != null) {
                while (reader.Read())
                {
                    
                    int columNumber = 6;
                    string Fname = (string)reader[columNumber];

                    columNumber = 7;
                    string Lname = (string)reader[columNumber];

                    customerList.Add("The owner of this house is: " + Fname + " " + Lname);
                }
                if (customerList.Count > 0)
                {
                    //Console.Write("The match is");
                    Console.WriteLine(customerList[0]);
                }
                else
                {
                    Console.WriteLine("Not found");
                }
            }
            else
            {
                throw new Exception("Sql reader findes ikke.");
            }
            
        }
}
}
