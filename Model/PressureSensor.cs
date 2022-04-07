using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SensorDataGenerator.Model
{
    /// <summary>
    /// Represents a sensors that is monitoring used machines as pressure sensors
    /// </summary>
    public class PressureSensor
    {
        /// <summary>
        /// Id of the sensor
        /// </summary>
        public int SensorId { get; set; }
        /// <summary>
        /// Is the sensor pressured/machine in use
        /// </summary>
        public bool InUse { get; set; }
        /// <summary>
        /// the timestamp of the moment that the data is sent and the counters are reset
        /// FORMAT: YYYYMMDDHHmm
        /// </summary>
        public int PeopleUsing  { get; set; }
        public int PeopleNotUsing { get; set; }
        public string ResetTimeStamp { get; set; }

        public PressureSensor(int _id, string _resetTimeStamp)
        {
            SensorId = _id;
            InUse = false;
            ResetTimeStamp = _resetTimeStamp;
        }
        /// <summary>
        /// Reset the sensor (after sending message)
        /// </summary>
        /// <param name="_resetPer"></param>
        public void Reset(DateTime _resetPer)
        {
            ResetTimeStamp = Helper.DateToStamp(_resetPer);
            InUse = false;
        }
        /// <summary>
        /// Generate fake data for sensor
        /// </summary>
        /// <param name="_factorIn"></param>
        /// <param name="_factorOut"></param>
        /// <param name="_default"></param>
        /// <param name="_maxIn"></param>
        /// <param name="_maxOut"></param>
        public void GenerateFakeData()
        {
            Random rnd = new Random();
            var INUSE = rnd.Next(0, 1);
            if(INUSE == 0)
            {
                InUse = false;
            }else if(INUSE == 1)
            {
                InUse = true;
            }
        }

    }
}
