using System;
using System.Collections.Generic;
using System.Text;

namespace SensorDataGenerator.Model
{
    /// <summary>
    /// Represents a sensor that is monitoring traffic
    /// </summary>
    class Sensor
    {
        /// <summary>
        /// Id of the sensor
        /// </summary>
        public int SendorId { get; set; }
        /// <summary>
        /// Number of people in for the timeslot
        /// </summary>
        public int PeopleIn { get; set; }
        /// <summary>
        /// Number of people out for the timeslot
        /// </summary>
        public int PeopleOut { get; set; }

        /// <summary>
        /// The timestamp of the moment that the data is sent and the counters are reset.
        /// Format YYYYMMDDHHmm
        /// </summary>
        public string ResetTimeStamp{ get; set; }

        public Sensor(int _id, string _resetTimeStamp)
        {
            PeopleIn = 0;
            PeopleOut = 0;
            SendorId = _id;
            ResetTimeStamp = _resetTimeStamp;
        }

        /// <summary>
        /// Reset the sensor (after sending message)
        /// </summary>
        /// <param name="_resetPer"></param>
        public void Reset(DateTime _resetPer)
        {
            ResetTimeStamp = Helper.DateToStamp(_resetPer);
            PeopleIn = 0;
            PeopleOut = 0;
        }

        /// <summary>
        /// Generete fake data for the sensor
        /// </summary>
        /// <param name="_factorIn">Factor for incoming people</param>
        /// <param name="_factorOut">Factor for outgoing people</param>
        /// <param name="_default">Max random per generation</param>
        /// <param name="_maxIn">Maximum incoming people</param>
        /// <param name="_maxOut">Maximum outgoing people</param>
        public void GenerateFakeData(double _factorIn, double _factorOut, int _default, int _maxIn, int _maxOut)
        {
            Random rnd = new Random();
            var c_in = Math.Round(rnd.Next(0, _default) * _factorIn, 0);
            var c_out = Math.Round(rnd.Next(0, _default) * _factorOut, 0);
            PeopleIn = (int)Math.Min(c_in, _maxIn);
            PeopleOut = (int)Math.Min(c_out, _maxOut);
        }

    }
}
