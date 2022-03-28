using System;
using System.Collections.Generic;
using System.Text;

namespace LoRaDataGenerator.Model
{
    /// <summary>
    /// Represents a city
    /// </summary>
    class City
    {
        /// <summary>
        /// Max number of cars in the city
        /// </summary>
        public int MaxCars { get; set; }
        /// <summary>
        /// Current number of cars in the city
        /// </summary>
        public int CurrentCars { get; set; }
        /// <summary>
        /// Sensors in the city to monitor traffic
        /// </summary>
        public List<Sensor> Sensors { get; set; }
        /// <summary>
        /// The current datatime to calculate for
        /// </summary>
        public DateTime CalculatingDateTime { get; set; }

        /// <summary>
        /// Constructor for the city
        /// </summary>
        /// <param name="_calculatingDateTime">The date we are calculating the traffic for</param>
        public City(DateTime _calculatingDateTime, int _sensors)
        {
            MaxCars = 20000;
            CurrentCars = 5000;
            CalculatingDateTime = _calculatingDateTime;
            Sensors = new List<Sensor>();
            for (int i = 0; i < _sensors; i++)
            {
                Sensors.Add(new Sensor(i, Helper.DateToStamp(_calculatingDateTime)));
            }
        }
    }
}
