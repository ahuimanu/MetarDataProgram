using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Device.Location;

namespace MetarLib
{
    public class Station
    {

        public static Uri URL_UCAR_RAP = new Uri("http://www.rap.ucar.edu/weather/surface/stations.txt");
        public static Uri URL_NOAA = new Uri("http://aviationweather.gov/static/adds/metars/stations.txt");

        private const string COMMENT_CHAR = "!";

        public static List<Station> stations;

        public static Station FindStation(string icao, List<Station> stations)
        {

            //this is a lambda expression which declares a station variable
            //and then examines the ICAO property for a match
            Station found = stations.Find(station => station.Icao == icao);

            //this anonymous delegate also works and may be easier for a beginner to understand
            //Station found2 = stations.Find(delegate(Station s) { return s.Icao == icao; });

            return found;
        }

        /*
         * STATIC CONSTRUCTOR
         */
        public static Station ParseStation(string line)
        {

            Station station = new Station();

            if (line.Length == 83)
            {
                //parsing
                station.State                   = Station.ParseState(line);
                station.StationName             = Station.ParseStationName(line);
                station.Icao                    = Station.ParseIcao(line);
                station.Iata                    = Station.ParseIata(line);
                station.Synop                   = Station.ParseSynop(line);
                station.Coordinates             = Station.ParseCoordinates(line);
                station.Elevation               = Station.ParseElevation(line);
                station.MetarReportingStation   = Station.ParseMetarReportingStation(line);
                station.Nexrad                  = Station.ParseNexrad(line);
                station.Avflag                  = Station.ParseAviationFlag(line);
                station.UpAir                   = Station.ParseUpperAir(line);
                station.Automation              = Station.ParseAutomation(line);
                station.OfcType                 = Station.ParseOfficeType(line);
                station.Priority                = Station.ParsePriority(line);
                station.CountryCode             = Station.ParseCountryCode(line);
            }
            else
            {
                station = null;
            }

            return station;
        }

        public override string ToString()
        {
            return this.Icao + " " + this.StationName + " " + this.Coordinates.Latitude + " " + this.Coordinates.Longitude;
        }

        private string state;
        public string State
        {
            get { return state; }
            set { state = value; }
        }

        private string stationName;
        public string StationName
        {
            get { return stationName; }
            set { stationName = value; }
        }

        private string icao;
        public string Icao
        {
            get { return icao; }
            set { icao = value; }
        }

        private string iata;
        public string Iata
        {
            get { return iata; }
            set { iata = value; }
        }

        private int synop;
        public int Synop
        {
            get { return synop; }
            set { synop = value; }
        }

        private GeoCoordinate coordinates;
        public GeoCoordinate Coordinates
        {
            get { return coordinates; }
            set { coordinates = value; }
        }

        private int elevation;
        public int Elevation
        {
            get { return elevation; }
            set { elevation = value; }
        }

        private bool metarReportingStation;
        public bool MetarReportingStation
        {
            get { return metarReportingStation; }
            set { metarReportingStation = value; }
        }

        private bool nexrad;
        public bool Nexrad
        {
            get { return nexrad; }
            set { nexrad = value; }
        }

        private AviationFlag avflag;
        public AviationFlag Avflag
        {
            get { return avflag; }
            set { avflag = value; }
        }

        private UpperAir upAir;
        public UpperAir UpAir
        {
            get { return upAir; }
            set { upAir = value; }
        }

        private Auto automation;
        public Auto Automation
        {
            get { return automation; }
            set { automation = value; }
        }

        private OfficeType ofcType;
        public OfficeType OfcType
        {
            get { return ofcType; }
            set { ofcType = value; }
        }

        private int priority;
        public int Priority
        {
            get { return priority; }
            set { priority = value; }
        }

        private string countryCode;
        public string CountryCode
        {
            get { return countryCode; }
            set { countryCode = value; }
        }

        /**
         * PARSE STATE ////////////////////////////////////////////////////////
         */
        private static string ParseState(string line)
        {
            string value = "";
            if (line != null)
            {
                value = line.Substring(0, 2);
            }
            return value;
        }

        /**
         * PARSE STATION NAME /////////////////////////////////////////////////
         */
        private static string ParseStationName(string line)
        {
            string value = "";
            if (line != null)
            {
                value = line.Substring(3, 16);
                value = value.Trim();
            }
            return value;
        }

        /**
         * PARSE ICAO CODE ////////////////////////////////////////////////////
         */
        private static string ParseIcao(string line)
        {
            string value = "";
            if (line != null)
            {
                value = line.Substring(20, 4);
            }
            return value;
        }

        /**
         * PARSE IATA CODE ////////////////////////////////////////////////////
         */
        private static string ParseIata(string line)
        {
            string value = "";
            if (line != null)
            {
                value = line.Substring(26, 4);
            }
            return value;
        }

        /**
         * PARSE SYNOP ////////////////////////////////////////////////////////
         */
        private static int ParseSynop(string line)
        {
            int value = 0;
           
            if (line != null && line.Substring(32, 5) != "     ")
            {
                try{
                    value = Convert.ToInt32(line.Substring(32,5));
                }
                catch(Exception exp)
                {
                    Console.Error.WriteLine("Problem: {0}", exp.Message);
                    Console.Error.WriteLine(exp.StackTrace);
                    Console.Error.WriteLine("Line: {0}", line);
                }
            }

            return value;
        }

        /**
         * PARSE GEOGRAPHIC COORDINATES ///////////////////////////////////////
         */
        private static GeoCoordinate ParseCoordinates(string line)
        {
            GeoCoordinate coord = null;

            string latDegrees    = line.Substring(39, 2);
            string latMinutes    = line.Substring(42, 2);
            string latDirection  = line.Substring(44, 1);
            double lat = 0;

            try
            {
                //Degrees, Minutes and seconds to decimal = D + M/60 + S/3600 
                lat = Convert.ToDouble(latDegrees) + (Convert.ToDouble(latMinutes) / 60);
            }
            catch (Exception exp)
            {
                Console.Error.WriteLine("Problem: " + exp.Message);
                Console.Error.WriteLine(exp.StackTrace);
                Console.Error.WriteLine("Line: {0}", line);
            }

            //Southern latitudes are negative
            if (latDirection == "S")
            {
                lat *= -1;
            }

            string longDegrees   = line.Substring(47, 3);
            string longMinutes   = line.Substring(51, 2);
            string longDirection = line.Substring(53, 1);
            double lon = 0;

            try
            {
                lon = Convert.ToDouble(longDegrees) + (Convert.ToDouble(latMinutes) / 60);
            }
            catch (Exception exp)
            {
                Console.Error.WriteLine("Problem: {0}", exp.Message);
                Console.Error.WriteLine(exp.StackTrace);
                Console.Error.WriteLine("Line: {0}", line);
            }

            //West longitudes are negative
            if (longDirection == "W")
            {
                lon *= -1;
            }

            coord = new GeoCoordinate(lat, lon);

            return coord;
        }

        /**
         * PARSE ELEVATION ////////////////////////////////////////////////////
         */
        private static int ParseElevation(string line)
        {
            int value = 0;
            if (line != null)
            {
                try
                {
                    value = Convert.ToInt32(line.Substring(55, 4));
                }
                catch (Exception exp)
                {
                    Console.Error.WriteLine("Problem: " + exp.Message);
                    Console.Error.WriteLine(exp.StackTrace);
                    Console.Error.WriteLine("Line: {0}", line);
                }
            }
            return value;
        }

        /**
         * PARSE METAR REPORTING STATION //////////////////////////////////////
         */
        private static bool ParseMetarReportingStation(string line)
        {
            bool value = false;
            if (line != null)
            {
                if (line.Substring(62, 1) == "X")
                {
                    value = true;
                }
            }
            return value;
        }

        //PARSE NEXRAD ////////////////////////////////////////////////////////
        private static bool ParseNexrad(string line)
        {
            bool value = false;
            if (line != null)
            {
                if (line.Substring(65, 1) == "X")
                {
                    value = true;
                }
            }
            return value;
        }

        /**
         * PARSE AVIATION FLAG ////////////////////////////////////////////////
         */
        private static AviationFlag ParseAviationFlag(string line)
        {
            AviationFlag flag = 0;

            string value = line.Substring(68, 1);
            switch (value)
            {
                case "V":
                    flag = AviationFlag.AirmetSigmet;
                    break;
                case "A":
                    flag = AviationFlag.ARTCC;
                    break;
                case "T":
                    flag = AviationFlag.TAF;
                    break;
                case "U":
                    flag = AviationFlag.TAFAndAirmetSigmet;
                    break;
                default:
                    flag = AviationFlag.None;
                    break;
            }

            return flag;
        }

        /**
         * PARSE UPPER AIR ////////////////////////////////////////////////////
         */
        private static UpperAir ParseUpperAir(string line)
        {
            UpperAir air = 0;

            string value = line.Substring(71, 1);
            switch (value)
            {
                case "X":
                    air = UpperAir.Rawinsonde;
                    break;
                case "W":
                    air = UpperAir.WindProfiler;
                    break;
                default:
                    air = UpperAir.None;
                    break;
            }

            return air;
        }

        /**
         * PARSE AUTOMATION
         */
        private static Auto ParseAutomation(string line)
        {
            Auto automation = 0;

            string value = line.Substring(74, 1);
            switch (value)
            {
                case "A":
                    automation = Auto.ASOS;
                    break;
                case "W":
                    automation = Auto.AWOS;
                    break;
                case "M":
                    automation = Auto.Meso;
                    break;
                case "H":
                    automation = Auto.Human;
                    break;
                case "G":
                    automation = Auto.Augmented;
                    break;
                default:
                    automation = Auto.None;
                    break;
            }

            return automation;
        }

        /**
         * PARSE OFFICE TYPE //////////////////////////////////////////////////
         */
        private static OfficeType ParseOfficeType(string line)
        {
            OfficeType type = 0;
            string value = line.Substring(77, 1);
            switch (value)
            {
                case "F":
                    type = OfficeType.WFO;
                    break;
                case "R":
                    type = OfficeType.RFC;
                    break;
                case "C":
                    type = OfficeType.NCEP;
                    break;
                default:
                    type = OfficeType.None;
                    break;
            }

            return type;
        }

        /**
         * PARSE PRIORITY /////////////////////////////////////////////////////
         */
        private static int ParsePriority(string line)
        {
            int value = 0;
            if (line != null)
            {
                value = Convert.ToInt32(line.Substring(79, 1));
            }
            return value;
        }

        /**
         * PARSE COUNTRY CODE /////////////////////////////////////////////////
         */
        private static string ParseCountryCode(string line)
        {
            string value = "";
            if (line != null)
            {
                value = line.Substring(81, 2);
            }
            return value;
        }
    }

    public enum AviationFlag
    {
        AirmetSigmet,
        ARTCC,
        TAF,
        TAFAndAirmetSigmet,
        None
    }

    public enum UpperAir
    {
        Rawinsonde,
        WindProfiler,
        None
    }

    public enum Auto
    {
        ASOS,
        AWOS,
        Meso,
        Human,
        Augmented,
        None
    }

    public enum OfficeType
    {
        WFO,
        RFC,
        NCEP,
        None
    }
}
