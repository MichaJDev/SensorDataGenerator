using SensorDataGenerator.Model;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SensorDataGenerator.DA
{
    /// <summary>
    /// Data access layer
    /// </summary>
    class DAL
    {
        private string source = ".";
        private string catalog = "SensorData";

        const int MAXRANDOM = 10; // maximum number of people to add or delete per datageneration
        const int INTERVALMINUTES = 5; // interval in minutes to generate data for

        private string GetConnectionString()
        {
            return $"Data Source=" + source + ";Initial Catalog=" + catalog + ";Integrated Security=True";            
        }

        /// <summary>
        /// The location/project the sensors belong to
        /// </summary>
        public Location Location { get; set; }

        /// <summary>
        /// The constructor for the DAL
        /// </summary>
        /// <param name="_startDateCalculation"></param>
        /// <param name="_datasource">servername of the database server</param>
        /// <param name="_catalog">database or catalogname of the server</param>
        public DAL(DateTime _startDateCalculation, string _datasource, string _catalog)
        {            
            Location = new Location(_startDateCalculation, 15);
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
                            return $"Number of records {Int32.Parse(dataReader[0].ToString())}";
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
                        case <= 10:
                            factorIn = 0.1;
                            break;
                        case <= 16:
                            factorIn = 0.8;
                            break;
                        default:
                            factorIn = 1;
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

                // next timeframe
                Location.CalculatingDateTime = Location.CalculatingDateTime.AddMinutes(INTERVALMINUTES);
            }
        }
    }
}
