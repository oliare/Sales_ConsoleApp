using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace hw1_connected_mode
{
    internal class Program
    {
        class ManagerDB
        {
            private string connString;
            private SqlConnection connection;
            public ManagerDB()
            {
                connString = ConfigurationManager.ConnectionStrings["Sales_db"].ConnectionString;
                connection = new SqlConnection(connString);
                connection.Open();
                //Console.WriteLine("connected");
            }
            ~ManagerDB()
            {
                connection.Close();
            }


            public void ShowInfoCustomers()
            {
                string query = @"select * from Customers";
                SqlCommand cmd = new SqlCommand(query, connection);
                var reader = cmd.ExecuteReader();
                ShowReader(reader);
            }
            public void ShowInfoSellers()
            {
                string query = @"select * from Sellers";
                SqlCommand cmd = new SqlCommand(query, connection);
                var reader = cmd.ExecuteReader();
                ShowReader(reader);
            }
            public void SalesBySellerName(string name, string surname)
            {
                string query = @"select s.Id, s.SaleAmount as Amount, s.SaleDate as Date, se.Name, se.Surname 
                                    from Sales s join Sellers se on s.SellerId = se.Id
                                    where se.Name = @name and se.Surname = @surname";
                SqlCommand cmd = new SqlCommand(query, connection);
                cmd.Parameters.Add("@name", System.Data.SqlDbType.NVarChar).Value = name;
                cmd.Parameters.Add("@surname", System.Data.SqlDbType.NVarChar).Value = surname;
                var reader = cmd.ExecuteReader();
                ShowReader(reader);
            }
            public void SalesBigerThan(float price)
            {
                string query = @"select Id, SaleAmount as Amount, SaleDate as Date from Sales
                                     where SaleAmount > @price";
                SqlCommand cmd = new SqlCommand(query, connection);
                cmd.Parameters.Add("@price", System.Data.SqlDbType.Float).Value = price;
                var reader = cmd.ExecuteReader();
                ShowReader(reader);
            }
            public void MaxAndMinSalesByCustomer(string name, string surname)
            {
                string query = @"select c.Name, c.Surname, MAX(s.SaleAmount) as 'Max Purchase', MIN(s.SaleAmount) as 'Min Purchase'
                                    from Customers c join Sales s on c.Id = s.CustomerId
                                    where c.Name = @name and c.Surname = @surname
                                    group by c.Name, c.Surname";
                SqlCommand cmd = new SqlCommand(query, connection);
                cmd.Parameters.Add("@name", System.Data.SqlDbType.NVarChar).Value = name;
                cmd.Parameters.Add("@surname", System.Data.SqlDbType.NVarChar).Value = surname;
                var reader = cmd.ExecuteReader();
                ShowReader(reader);
            }
            public void FirstSaleBySeller(string name, string surname)
            {
                string query = @"select top 1 s.SaleAmount as Price, s.SaleDate as Date 
                                from Sales s join Sellers se on s.SellerId = se.Id
                                    where se.Name = @name and se.Surname = @surname
                                    order by s.SaleDate asc";
                SqlCommand cmd = new SqlCommand(query, connection);
                cmd.Parameters.Add("@name", System.Data.SqlDbType.NVarChar).Value = name;
                cmd.Parameters.Add("@surname", System.Data.SqlDbType.NVarChar).Value = surname;
                var reader = cmd.ExecuteReader();
                ShowReader(reader);
            }
            public void ShowReader(SqlDataReader reader)
            {
                Console.WriteLine();
                for (int i = 0; i < reader.FieldCount; i++)
                {
                    Console.Write($"{reader.GetName(i),15}");
                }
                Console.WriteLine();
                Console.WriteLine("\t" + new string('-', 70));

                while (reader.Read())
                {
                    for (int i = 0; i < reader.FieldCount; i++)
                    {
                        object tmp = reader[i];
                        if (reader[i] is DateTime)
                        {
                            tmp = ((DateTime)reader[i]).ToString("yyyy-MM-dd");
                        }
                        Console.Write($"{tmp,15}");
                    }
                    Console.WriteLine();
                }
                Console.WriteLine();
                reader.Close();
            }
        }
        static void Main(string[] args)
        {
            var mdb = new ManagerDB();
            Console.OutputEncoding = Encoding.Unicode;
            Console.InputEncoding = Encoding.Unicode;

            while (true)
            {
                Console.Write("\nSELECT AN OPTION\n\t1. show all customers\n\t2. show all sellers\n\t3. sales made by seller (name && surname)\n\t4. show sales for a price of more than" +
                    "\n\t5. show most expensive and cheapest purchase by customer (name && surname) \n\t6. show the first sale by seller (name && surname)\n\t0. exit\n> ");
                string option = Console.ReadLine();
                if (option == "0")
                {
                    Console.WriteLine("exit >>> ");
                    break;
                }
                switch (option)
                {
                    case "1":
                        mdb.ShowInfoCustomers();
                        break;
                    case "2":
                        mdb.ShowInfoSellers();
                        break;
                    case "3":
                        Console.Write("\nEnter name: "); var n = Console.ReadLine();
                        Console.Write("Enter surname: "); var s = Console.ReadLine();
                        mdb.SalesBySellerName(n, s);
                        break;
                    case "4":
                        Console.Write("\nEnter price: "); var p = float.Parse(Console.ReadLine());
                        mdb.SalesBigerThan(p);
                        break;
                    case "5":
                        Console.Write("\nEnter name: "); n = Console.ReadLine();
                        Console.Write("Enter surname: "); s = Console.ReadLine();
                        mdb.MaxAndMinSalesByCustomer(n, s); 
                        break;
                    case "6":
                        Console.Write("\nEnter name: "); n = Console.ReadLine();
                        Console.Write("Enter surname: "); s = Console.ReadLine();
                        mdb.FirstSaleBySeller(n, s);
                        break;

                    default:
                        Console.WriteLine("- wrong option entered -");
                        break;
                }
            }

        }
    }
}
