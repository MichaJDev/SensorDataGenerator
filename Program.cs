using SensorDataGenerator.DA;
using SensorDataGenerator.Model;
using System;

namespace SensorDataGenerator
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("DATABASE CONNECTION:");
            Console.WriteLine("Enter database servername (default: .)");
            var server = Console.ReadLine();
            Console.WriteLine("Enter catalog name (default: SensorData)");
            var db = Console.ReadLine();

            Console.Clear();

            Console.WriteLine("Creating location placeholder!");
            Console.WriteLine("Enter number of days for data generation (default 60)");
            
            var daysStr = Console.ReadLine();
            int daysInt;
            try
            {
                daysInt = Int32.Parse(daysStr);
            }
            catch (Exception)
            {
                daysInt = 60;
            }
            daysInt = -1 * daysInt;
            
            DAL LocationDAL;
            // Create the dal for the given connection. Provide the number of days to calculate from, the number of sensors (number of entries of the location) and the max number of persons in the location
            LocationDAL = new DAL(DateTime.Now.Add(new TimeSpan(daysInt, 0, 0, 0)), server, db, 1, 200);

            // check connection
            Console.WriteLine(LocationDAL.CheckDatabaseConnection());
            // empty table
            Console.WriteLine($"Empty table: {LocationDAL.TruncateDataTable()} rows deleted");
            Console.WriteLine("Press any key to start generating data");
            Console.ReadKey();
            // generate data
            LocationDAL.GenerateAndStoreData();

            // check connection (show number of records)
            Console.WriteLine(LocationDAL.CheckDatabaseConnection());

        }
    }
}
