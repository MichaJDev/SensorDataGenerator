using SensorDataGenerator.Model;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace SensorDataGenerator.DA
{
    /// <summary>
    /// Data access layer
    /// </summary>
    class DAL
    {
        private readonly string source = ".";
        private readonly string catalog = "SensorData";

        const int MAXRANDOM = 8; // maximum number of people to add or delete per datageneration
        const int INTERVALMINUTES = 5; // interval in minutes to generate data for

        private string GetConnectionString()
        {
            return $"Data Source=" + source + ";Initial Catalog=" + catalog + ";Integrated Security=True";
        }

        public void SetupDB()
        {
            using (SqlConnection cnn = new SqlConnection(GetConnectionString()))
            {
                string script = File.ReadAllText(MapPath("~/Resources/Create_db_and_table_JhonnysVersie.sql"));
                IEnumerable<string> commandStrings = Regex.Split(script, @"^\s*GO\s*$", RegexOptions.Multiline | RegexOptions.IgnoreCase);
                cnn.Open();

                if (DataBaseExists() == 0)
                {
                    Console.WriteLine("Databse doesnt exist! Creating....");
                    foreach (string commandString in commandStrings)
                    {
                        try
                        {
                            if (commandString.Trim() != " ")
                            {
                                new SqlCommand(commandString, cnn).ExecuteNonQuery();
                                Console.WriteLine($"Database: {catalog}, created on Server: {source}");
                            }
                        }
                        catch (SqlException ex)
                        {
                            Console.WriteLine($"Unable to create Database: {catalog} on Server: {source}");
                            Console.WriteLine(ex.Message + "\n" + ex.StackTrace);
                        }
                    }
                }
                else
                {
                    Console.WriteLine("Database Already exists, halting creation");
                }
                cnn.Close();
            }
        }
        private int DataBaseExists()
        {
            using (SqlConnection cnn = new SqlConnection(GetConnectionString()))
            {
                using (SqlCommand cmd = new SqlCommand())
                {
                    cnn.Open();
                    cmd.Connection = cnn;
                    cmd.CommandText = $"SELECT * FROM mastetr.dbo.sysdatabases where name = '{catalog}'";
                    return (int)cmd.ExecuteScalar();
                }
            }
        }

        /// <summary>
        /// The location/project the sensors belong to
        /// </summary>
        public Location Location { get; set; }
        /// <summary>
        /// Empty Dal to start setup
        /// </summary>

        /// <summary>
        /// The constructor for the DAL
        /// </summary>
        /// <param name="_startDateCalculation"></param>
        /// <param name="_datasource">servername of the database server</param>
        /// <param name="_catalog">database or catalogname of the server</param>
        /// <param name="_numberOfSensors">number of sensors to generate</param>
        /// <param name="_locationMaxNumberOfPeople">Maximum number of people in the location</param>
        public DAL(DateTime _startDateCalculation, string _datasource, string _catalog, int _numberOfSensors, int _locationMaxNumberOfPeople)
        {
            Location = new Location(_startDateCalculation, _numberOfSensors, _numberOfSensors);
            Location.MaxPersons = _locationMaxNumberOfPeople;
            Location.CurrentPersons = 0;

            source = !string.IsNullOrEmpty(_datasource) ? _datasource : source;
            catalog = !string.IsNullOrEmpty(_catalog) ? _catalog : catalog;
        }

        /// <summary>
        /// Check the database connection
        /// </summary>
        /// <returns>A string with the number of records found</returns>
        internal string CheckDatabaseConnection()
        {
            using (SqlConnection cnn = new SqlConnection(GetConnectionString()))
            {
                using (SqlCommand cmd = new SqlCommand())
                {
                    cnn.Open();
                    cmd.Connection = cnn;
                    cmd.CommandText = "SELECT count(*) FROM SensorReading";
                    using (SqlDataReader dataReader = cmd.ExecuteReader())
                    {
                        while (dataReader.Read())
                        {
                            return $"Connection successfull. Number of records in table {Int32.Parse(dataReader[0].ToString())}";
                        }
                    }
                }
            }
            return "";
        }

        /// <summary>
        /// Empty the sensor reading table
        /// </summary>
        /// <returns>Number of deleted rows</returns>
        internal int TruncateDataTable()
        {
            try
            {
                using (SqlConnection cnn = new SqlConnection(GetConnectionString()))
                {
                    using (SqlCommand cmd = new SqlCommand())
                    {
                        cnn.Open();
                        cmd.Connection = cnn;
                        cmd.CommandText = "delete from SensorReading";
                        return cmd.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// Store the data for one sensor
        /// </summary>
        /// <param name="_sensor">The sensor to store for</param>
        private void StoreData(Sensor _sensor)
        {
            // show feedback
            Console.WriteLine($"Sensor {_sensor.SendorId}, PeopleIn {_sensor.PeopleIn.ToString().PadLeft(5, '0')}, PeopleOut {_sensor.PeopleOut.ToString().PadLeft(5, '0')}, Timestamp {_sensor.ResetTimeStamp}, CurrentPeople {Location.CurrentPersons.ToString().PadLeft(5, '0')}");

            // actually save the record
            try
            {
                SqlConnection cnn = new SqlConnection
                {
                    ConnectionString = GetConnectionString()
                };
                cnn.Open();
                string sql = "INSERT INTO SensorReading (SensorId, People_in, People_out, TimeStamp) VALUES (@SensorId, @People_in, @People_out, @TimeStamp)";
                using (SqlCommand cmd = new SqlCommand(sql, cnn))
                {
                    cmd.Parameters.AddWithValue("@SensorId", _sensor.SendorId);
                    cmd.Parameters.AddWithValue("@People_in", _sensor.PeopleIn);
                    cmd.Parameters.AddWithValue("@People_out", _sensor.PeopleOut);
                    cmd.Parameters.AddWithValue("@TimeStamp", _sensor.ResetTimeStamp);
                    cmd.ExecuteNonQuery();
                }
                cnn.Close();
            }
            catch (Exception)
            {
                throw;
            }
        }
        /// <summary>
        /// StorePressure SensorData
        /// </summary>
        /// <param name="_sensor"></param>
        private void StoreData(PressureSensor _sensor)
        {
            Console.WriteLine($"Sensor {_sensor.SensorId}, InUse {_sensor.InUse.ToString().PadLeft(5, '0')}, TimeStamp {_sensor.ResetTimeStamp}, CurrentPeople {Location.CurrentPersons.ToString().PadLeft(5, '0')}");
            try
            {
                SqlConnection cnn = new SqlConnection
                {
                    ConnectionString = GetConnectionString()
                };
                cnn.Open();
                string sql = "INSERT INTO PressureSensors (SensorId, InUse, TImeStamp) VALUES (@SensorId, @InUse, @TimeStamp)";
                using (SqlCommand cmd = new SqlCommand(sql, cnn))
                {
                    cmd.Parameters.AddWithValue("@SensorId", _sensor.SensorId);
                    cmd.Parameters.AddWithValue("@InUse", _sensor.InUse);
                    cmd.Parameters.AddWithValue("@TimeStamp", _sensor.ResetTimeStamp);
                }
                cnn.Close();
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// Generate pressure sensors data and store
        /// </summary>

        /// <summary>
        /// Generate the data and store
        /// </summary>
        internal void GenerateAndStoreData()
        {
            while (Location.CalculatingDateTime < DateTime.Now)
            {
                // create sensordata
                foreach (var sensor in Location.Sensors)
                {
                    sensor.Reset(Location.CalculatingDateTime);

                    double factorIn;
                    switch (Location.CalculatingDateTime.Hour)
                    {
                        //determine factor based on opening hours (example)
                        case <= 8:
                            factorIn = 0;
                            break;
                        case <= 10:
                            factorIn = 0.1;
                            break;
                        case <= 16:
                            factorIn = 0.8;
                            break;
                        case <= 18:
                            factorIn = 1;
                            break;
                        default:
                            factorIn = 0;
                            break;
                    }

                    double factorOut;
                    switch (Location.CalculatingDateTime.Hour)
                    {
                        case > 16:
                            factorOut = 1;
                            break;
                        case > 13:
                            factorOut = 0.3;
                            break;
                        default:
                            factorOut = 0.3;
                            break;
                    }
                    sensor.GenerateFakeData(factorIn, factorOut, MAXRANDOM, Location.MaxPersons - Location.CurrentPersons, Location.CurrentPersons);

                    // adjust location values
                    Location.CurrentPersons += sensor.PeopleIn;
                    Location.CurrentPersons -= sensor.PeopleOut;

                    // store sensordata and reset
                    StoreData(sensor);
                }
                // create sensordata
                foreach (var sensor in Location.PressureSensors)
                {
                    sensor.Reset(Location.CalculatingDateTime);

                    sensor.GenerateFakeData();

                    // adjust location values
                    Location.CurrentPersons += sensor.PeopleUsing;
                    Location.CurrentPersons -= sensor.PeopleNotUsing;

                    // store sensordata and reset
                    StoreData(sensor);
                }

                // next timeframe
                Location.CalculatingDateTime = Location.CalculatingDateTime.AddMinutes(INTERVALMINUTES);
            }
        }
        private string MapPath(string path)
        {
            return Path.Combine((string)AppDomain.CurrentDomain.GetData("ContentRootPath"),
            path);
        }
    }
}
