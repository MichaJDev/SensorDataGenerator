using System;
using System.Collections.Generic;
using System.Text;

namespace SensorDataGenerator.Model
{
    /// <summary>
    /// Represents a location/project placeholder
    /// </summary>
    class Location
    {
        /// <summary>
        /// Max number of people of the location
        /// </summary>
        public int MaxPersons { get; set; }
        /// <summary>
        /// Current number of people
        /// </summary>
        public int CurrentPersons { get; set; }
        /// <summary>
        /// Sensors in the location to monitor 
        /// </summary>
        public List<Sensor> Sensors { get; set; }
        /// <summary>
        /// The current datatime to calculate for
        /// </summary>
        public List<PressureSensor> PressureSensors { get; set; }
        public DateTime CalculatingDateTime { get; set; }

        /// <summary>
        /// Constructor for the location
        /// </summary>
        /// <param name="_calculatingDateTime">The date we are calculating for</param>
        public Location(DateTime _calculatingDateTime, int _sensors, int _pSensors)
        {
            MaxPersons = 100;
            CurrentPersons = 0;
            CalculatingDateTime = _calculatingDateTime;
            Sensors = new List<Sensor>();
            for (int i = 0; i < _sensors; i++)
            {
                Sensors.Add(new Sensor(i, Helper.DateToStamp(_calculatingDateTime)));
            }
            PressureSensors = new List<PressureSensor>();
            for(int i = 0; i < _pSensors; i++)
            {
                PressureSensors.Add(new PressureSensor(i, Helper.DateToStamp(_calculatingDateTime)));
            }
        }
    }
}
