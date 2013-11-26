using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MetarLib
{
    public class AtmosphericPressure
    {

        /*
         * CONVERT FROM INCHES OF MERCURY TO HECTOPASCALS /////////////////////
         */
        //conversion taken from: http://www.conversion-website.com/pressure/inch_of_mercury_to_hectopascal.html
        public static float InchesHgTOhPa(float inches)
        {
            return inches * 33.86389f;
        }

        private float pressure;
        public float Pressure
        {
            get { return pressure; }
            set { pressure = value; }
        }
    }
}
