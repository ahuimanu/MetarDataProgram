using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MetarLib
{
    public class WeatherData
    {
        private static Dictionary<WeatherCodeIndex, string> weatherCodes;
        public static Dictionary<WeatherCodeIndex, string> WeatherCodes
        {
            get { return weatherCodes; }
        }

        private static void InitializeWeatherCodes()
        {
            WeatherCodes.Add(WeatherCodeIndex.Plus, "+");
            WeatherCodes.Add(WeatherCodeIndex.Minus, "-");
            WeatherCodes.Add(WeatherCodeIndex.MI, "Shallow");
            WeatherCodes.Add(WeatherCodeIndex.BC, "Patches");
            WeatherCodes.Add(WeatherCodeIndex.BL, "Blowing");
            WeatherCodes.Add(WeatherCodeIndex.TS, "Thunderstorm");
            WeatherCodes.Add(WeatherCodeIndex.VC, "In the Vicinity");
            WeatherCodes.Add(WeatherCodeIndex.PR, "Partial");
            WeatherCodes.Add(WeatherCodeIndex.DR, "Low Drifting");
            WeatherCodes.Add(WeatherCodeIndex.SH, "Showers");
            WeatherCodes.Add(WeatherCodeIndex.FZ, "Freezing");
            WeatherCodes.Add(WeatherCodeIndex.RA, "Rain");
            WeatherCodes.Add(WeatherCodeIndex.SN, "Snow");
            WeatherCodes.Add(WeatherCodeIndex.IC, "Ice Crystals");
            WeatherCodes.Add(WeatherCodeIndex.GR, "Hail");
            WeatherCodes.Add(WeatherCodeIndex.UP, "Unknown Precipitation");
            WeatherCodes.Add(WeatherCodeIndex.DZ, "Drizzle");
            WeatherCodes.Add(WeatherCodeIndex.SG, "Snow grains");
            WeatherCodes.Add(WeatherCodeIndex.PL, "Ice Pellets");
            WeatherCodes.Add(WeatherCodeIndex.GS, "Small hail");
            WeatherCodes.Add(WeatherCodeIndex.FG, "Fog");
            WeatherCodes.Add(WeatherCodeIndex.BR, "Mist");
            WeatherCodes.Add(WeatherCodeIndex.DU, "Widespread Dust");
            WeatherCodes.Add(WeatherCodeIndex.SA, "Sand");
            WeatherCodes.Add(WeatherCodeIndex.VA, "Volcanic Ash");
            WeatherCodes.Add(WeatherCodeIndex.HZ, "Haze");
            WeatherCodes.Add(WeatherCodeIndex.FU, "Smoke");
            WeatherCodes.Add(WeatherCodeIndex.PY, "Spray");
            WeatherCodes.Add(WeatherCodeIndex.SQ, "Squall");
            WeatherCodes.Add(WeatherCodeIndex.DS, "Dust storm");
            WeatherCodes.Add(WeatherCodeIndex.FC, "Funnel Cloud");
            WeatherCodes.Add(WeatherCodeIndex.DO, "Dust or Sand Whirls");
            WeatherCodes.Add(WeatherCodeIndex.SS, "Sand storm");
            WeatherCodes.Add(WeatherCodeIndex.B, "Begin at time");
            WeatherCodes.Add(WeatherCodeIndex.E, "End at time");
        }

        static WeatherData()
        {
            weatherCodes = new Dictionary<WeatherCodeIndex, string>();
            InitializeWeatherCodes();
        }

        private string observation;
        public string Observation
        {
            get { return observation; }
            set { observation = value; }
        }

        private WeatherCodeIndex observationCode;
        public WeatherCodeIndex ObservationCode
        {
            get { return observationCode; }
            set { observationCode = value; }
        }
    }

    public enum WeatherCodeIndex
    {
        Plus,       //+
        Minus,      //-
        MI,         //Shallow
        BC,         //Patches
        BL,         //Blowing
        TS,         //Thunderstorm
        VC,         //In the Vicinity
        PR,         //Partial
        DR,         //Low Drifting
        SH,         //Showers
        FZ,         //Freezing
        RA,         //Rain
        SN,         //Snow
        IC,         //Ice Crystals
        GR,         //Hail
        UP,         //Unknown Precipitation
        DZ,         //Drizzle
        SG,         //Snow grains
        PL,         //Ice Pellets
        GS,         //Small hail
        FG,         //Fog
        BR,         //Mist
        DU,         //Widespread Dust
        SA,         //Sand
        VA,         //Volcanic Ash
        HZ,         //Haze
        FU,         //Smoke
        PY,         //Spray
        SQ,         //Squall
        DS,         //Dust storm
        FC,         //Funnel Cloud
        DO,         //Dust or Sand whirls
        SS,         //Sand storm
        B,          //Begin at time
        E,          //End at time
    }
}
