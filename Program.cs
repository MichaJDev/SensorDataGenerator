using SensorDataGenerator.DA;
using SensorDataGenerator.Model;
using System;

namespace SensorDataGenerator
{
    class Program
    {
        static void Main(string[] args)
        {
            // get connection info
            Console.WriteLine("DATABASE CONNECTION:");
            Console.WriteLine("Enter database servername (default: .)");
            var server = Console.ReadLine();
            if(server == null)
            {
                server = ".";
            }
            Console.WriteLine("Enter catalog name (default: SensorData)");
            var db = Console.ReadLine();
            if(db == null)
            {
                db = "SensorData";
            }
            
            // get location and sensor info
            Console.Clear();
            Console.WriteLine("Creating location placeholder!");

            int daysInt = GetNumerFromConsole("Enter number of days for data generation", 60);
            daysInt = -1 * daysInt;

            int numberOfSensors = GetNumerFromConsole("Enter the number of sensors, aka the numbers om entrances of the location", 1);
            int maxPeopleLocation = GetNumerFromConsole("Enter the maximum number of people in the location", 200);
            
            DAL LocationDAL;
            // Create the dal for the given connection. Provide the number of days to calculate from, the number of sensors (number of entries of the location) and the max number of persons in the location
            LocationDAL = new DAL(DateTime.Now.Add(new TimeSpan(daysInt, 0, 0, 0)), server, db, numberOfSensors, maxPeopleLocation);
            // Creating database on server automatically since it wasnt included before, programmers do not like manual stuff
            Console.Clear();
            Console.WriteLine("Checking if Database exists...!");
            LocationDAL.SetupDB();
            // summary
            Console.Clear();
            Console.WriteLine($"Database server {server}");
            Console.WriteLine($"Database catalog {db}");
            Console.WriteLine($"Number of days to start generating data {daysInt}");
            Console.WriteLine($"Number of sensors {numberOfSensors}");
            Console.WriteLine($"Max number of persons in the location {maxPeopleLocation}");
            Console.WriteLine();

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

        /// <summary>
        /// Get an integer value from console input
        /// </summary>
        /// <param name="_text">The text to show</param>
        /// <param name="_default">The default value for the int in case an invalid value is entered</param>
        /// <returns></returns>
        public static int GetNumerFromConsole(string _text, int _default)
        {
            Console.WriteLine($"{_text} (Default {_default})");
            var readlineValue = Console.ReadLine();
            int theValueInt;
            try
            {
                theValueInt = Int32.Parse(readlineValue);
            }
            catch (Exception)
            {
                theValueInt = _default;
            }
            return theValueInt;
        }
    }
}
