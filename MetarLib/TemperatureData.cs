using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MetarLib
{
    public class TemperatureData
    {

        /**
         * F to C /////////////////////////////////////////////////////////////
         */
        public static float FahrenheitToCelcius(float fahrenheit)
        {
            return (5.0f / 9.0f) * (fahrenheit - 32);
        }

        /**
         * C to F /////////////////////////////////////////////////////////////
         */
        public static float CelciusToFahrenheit(float celcius)
        {
            return celcius * (9.0f / 5.0f) + 32;
        }

        private float temperature;          //in North America/USA assume F
        public float Temperature
        {
            get { return temperature; }
            set { temperature = value; }
        }

        private float dewpoint;
        public float Dewpoint
        {
            get { return dewpoint; }
            set { dewpoint = value; }
        }

        private float relativeHumidity;
        public float RelativeHumidity
        {
            get { return relativeHumidity; }
            set { relativeHumidity = value; }
        }

        /**
         * RELATIVE HUMIDITY CALCULATION //////////////////////////////////////
         */
        //source for calculation is here: http://www.hpc.ncep.noaa.gov/html/dewrh.shtml
        //view page source for JavaScript calculation
        public void DoRelativeHumidityCalculation()
        {
            double saturationVaporPressure, actualVaporPressure;
            
            saturationVaporPressure = 6.11 * Math.Pow(10, (7.5 * this.Temperature / (237.7 + this.Temperature)));
            actualVaporPressure = 6.11 * Math.Pow(10, (7.5 * this.Dewpoint / (237.7 + this.Dewpoint)));
            double rh = (actualVaporPressure / saturationVaporPressure) * 100;
            RelativeHumidity = (float)(Math.Round(rh * 100, 0) / 100);
        }
    }
}
