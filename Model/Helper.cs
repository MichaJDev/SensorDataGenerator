using System;
using System.Collections.Generic;
using System.Text;

namespace LoRaDataGenerator.Model
{
    class Helper
    {
        /// <summary>
        /// Return the timestamp for a given date time
        /// </summary>
        /// <param name="_dateTime">The date time to convert</param>
        /// <returns>the timestamp</returns>
        public static string DateToStamp(DateTime _dateTime)
        {
            return _dateTime.ToString("yyyyMMddHHmm");
        }
    }
}
