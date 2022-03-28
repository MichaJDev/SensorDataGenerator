using LoRaDataGenerator.Model;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LoRaDataGenerator.DA
{
    /// <summary>
    /// Data access layer
    /// </summary>
    class DAL
    {
        private string source = ".";
        private string catalog = "Casus2021B3";

        private string GetConnectionString()
        {
            return $"Data Source=" + source + ";Initial Catalog=" + catalog + ";Integrated Security=True";            
        }

        /// <summary>
        /// The city
        /// </summary>
        public City ValkenBurg { get; set; }

        /// <summary>
        /// The constructor for the DAL
        /// </summary>
        /// <param name="_startDateCalculation"></param>
        /// <param name="_datasource">servername of the database server</param>
        /// <param name="_catalog">database or catalogname of the server</param>
        public DAL(DateTime _startDateCalculation, string _datasource, string _catalog)
        {            
            ValkenBurg = new City(_startDateCalculation, 15);
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
                    cmd.CommandText = "SELECT count(*) FROM LoRaSensorReading";
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
                        cmd.CommandText = "delete from LoRaSensorReading";
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
        /// <param name="_sensor"></param>
        private void StoreData(Sensor _sensor)
        {
            // show feedback
            Console.WriteLine($"Sensor {_sensor.SendorId}, Carsin {_sensor.CarsIn.ToString().PadLeft(5, '0')}, Carsout {_sensor.CarsOut.ToString().PadLeft(5, '0')}, Timestamp {_sensor.ResetTimeStamp}, CurrentCars {ValkenBurg.CurrentCars.ToString().PadLeft(5, '0')}");

            // actually save the record
            try
            {
                SqlConnection cnn = new SqlConnection
                {
                    ConnectionString = GetConnectionString()
                };
                cnn.Open();
                string sql = "INSERT INTO LoRaSensorReading (SensorId, Car_in, Car_out, TimeStamp) VALUES (@SensorId, @Car_in, @Car_out, @TimeStamp)";
                using (SqlCommand cmd = new SqlCommand(sql, cnn))
                {
                    cmd.Parameters.AddWithValue("@SensorId", _sensor.SendorId);
                    cmd.Parameters.AddWithValue("@Car_in", _sensor.CarsIn);
                    cmd.Parameters.AddWithValue("@Car_out", _sensor.CarsOut);
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
            while (ValkenBurg.CalculatingDateTime < DateTime.Now)
            {
                // create sensordata
                foreach (var sensor in ValkenBurg.Sensors)
                {
                    sensor.Reset(ValkenBurg.CalculatingDateTime);

                    double factorIn;
                    switch (ValkenBurg.CalculatingDateTime.Hour)
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
                    switch (ValkenBurg.CalculatingDateTime.Hour)
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
                    sensor.GenerateFakeData(factorIn, factorOut, 1000, ValkenBurg.MaxCars - ValkenBurg.CurrentCars, ValkenBurg.CurrentCars);
                    
                    // adjust valkenburg values
                    ValkenBurg.CurrentCars += sensor.CarsIn;
                    ValkenBurg.CurrentCars -= sensor.CarsOut;

                    // store sensordata and reset
                    StoreData(sensor);
                }

                // next timeframe
                ValkenBurg.CalculatingDateTime = ValkenBurg.CalculatingDateTime.AddMinutes(15);
            }
        }
    }
}
