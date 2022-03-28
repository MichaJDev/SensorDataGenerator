using System;
using System.Collections.Generic;
using System.Text;

namespace LoRaDataGenerator.Model
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
        /// Number of cars in for the timeslot
        /// </summary>
        public int CarsIn { get; set; }
        /// <summary>
        /// Number of cars out for the timeslot
        /// </summary>
        public int CarsOut { get; set; }

        /// <summary>
        /// The timestamp of the moment that the data is sent and the counters are reset.
        /// Format YYYYMMDDHHmm
        /// </summary>
        public string ResetTimeStamp{ get; set; }

        public Sensor(int _id, string _resetTimeStamp)
        {
            CarsIn = 0;
            CarsOut = 0;
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
            CarsIn = 0;
            CarsOut = 0;
        }

        /// <summary>
        /// Generete fake data for the sensor
        /// </summary>
        /// <param name="_factorIn">Factor for incoming cars</param>
        /// <param name="_factorOut">Factor for outgoing cars</param>
        /// <param name="_default">Max random per genration</param>
        /// <param name="_maxIn">Maximum incoming cars</param>
        /// <param name="_maxOut">Maximum outgoing cars</param>
        public void GenerateFakeData(double _factorIn, double _factorOut, int _default, int _maxIn, int _maxOut)
        {
            Random rnd = new Random();
            var c_in = Math.Round(rnd.Next(0, _default) * _factorIn, 0);
            var c_out = Math.Round(rnd.Next(0, _default) * _factorOut, 0);
            CarsIn = (int)Math.Min(c_in, _maxIn);
            CarsOut = (int)Math.Min(c_out, _maxOut);
        }

    }
}
