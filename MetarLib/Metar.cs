using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace MetarLib
{
    /// <summary>
    /// Represents a comprehensive class for dealing with METAR objects.
    /// Documentation: http://en.wikipedia.org/wiki/METAR
    /// </summary>
    public class Metar
    {

        /* the structure of a metar file in North America is documented here: 
         * http://en.wikipedia.org/wiki/METAR#North_American_METAR_codes
         */
        public static Uri URL_LATEST_OBSERVATION = new Uri("http://weather.noaa.gov/pub/data/observations/metar/stations/");
        public static Uri URL_LATEST_CYCLES = new Uri("http://weather.noaa.gov/pub/data/observations/metar/cycles/");

        private string icao;
        public string Icao
        {
            get { return icao; }
            set { icao = value; }
        }
        
        private string raw;
        public string Raw
        {
            get { return raw; }
            set { raw = value; }
        }
        
        private DateTime reportTime;
        public DateTime ReportTime
        {
            get { return reportTime; }
            set { reportTime = value; }
        }

        private WindData wind;
        public WindData Wind
        {
            get { return wind; }
            set { wind = value; }
        }

        private List<CloudData> clouds;
        public List<CloudData> Clouds
        {
            get { return clouds; }
            set { clouds = value; }
        }

        private string weather;
        public string Weather
        {
            get { return weather; }
            set { weather = value; }
        }

        private TemperatureData tempData;
        public TemperatureData TempData
        {
            get { return tempData; }
            set { tempData = value; }
        }

        //in North America/USA assume in. of Hg (hecto pascals in rest of the world)
        private AtmosphericPressure pressure;             
        public AtmosphericPressure Pressure
        {
            get { return pressure; }
            set { pressure = value; }
        }

        private string visibility;
        public string Visibility
        {
          get { return visibility; }
          set { visibility = value; }
        }

        /*
         * PARSE RAW METAR ////////////////////////////////////////////////////
         */
        public static Metar Parse(string report)
        {

            Metar met = new Metar();
            met.Raw = report.Substring(report.IndexOf("\n") + 1, 
                                      (report.Length - report.IndexOf("\n")) - 1);
            met.Raw = met.Raw.Trim();
            
            //uses the report time at the top of the observation
            met.ReportTime = Metar.ParseReportTime(report);

            //parse station
            met.Icao = Metar.ParseStation(met.Raw);

            //by now we've stripped the date out, so let's use the RAW data

            met.Visibility = Metar.ParseVisibility(met.Raw);
            met.Wind = Metar.ParseWindData(met.Raw);

            //////////////////////////////////////////////////////////////////
            ///////////////!!! TO DO!!! SET WEATHER PARSING !!!///////////////
            //////////////////////////////////////////////////////////////////
            met.Weather = Metar.ParseWeatherData(met.Raw);

            met.TempData = Metar.ParseTemperatureData(met.Raw);
            met.Clouds = Metar.ParseCloudData(met.Raw);
            met.Pressure = Metar.ParsePressure(met.Raw);

            return met;
        }

        /**
         * STATION ICAO ///////////////////////////////////////////////////////
         */
        public static string ParseStation(string report)
        {
            return report.Substring(0, 4);
        }

        /**
         * TIME DATA //////////////////////////////////////////////////////////
         */
        public static DateTime ParseReportTime(string report)
        {
            DateTime reportTime = DateTime.Now;
            try
            {
                reportTime = DateTime.Parse(report.Substring(0, report.IndexOf("\n")));
            }
            catch (Exception exp)
            {
                Console.Error.WriteLine("Problem: " + exp.Message);
                Console.Error.WriteLine(exp.StackTrace);
            }
            return reportTime;
        }

        /**
         * WIND DATA //////////////////////////////////////////////////////////
         */
        public static WindData ParseWindData(string report)
        {
            WindData wind = new WindData();

            try
            {

                //REGEX - A Regular Expression
                // a good regex builder tool - http://gskinner.com/RegExr/
                // a good regex validation tool for C#/.NET - http://regexhero.net/tester/

                //a regular expression to find wind or gusts
                Regex reg = new Regex(@"\d\d\d\d\dG?\d?\d?KT");
                Match match = reg.Match(report);
                if (match.Success)
                {
                    string windstr = match.Value;

                    if (windstr[5].Equals('G'))
                    {
                        string[] parts = windstr.Split(new char[] { 'G' });
                        wind.Direction = Convert.ToInt32(parts[0].Substring(0, 3));
                        wind.Speed = Convert.ToInt32(parts[0].Substring(3, 2));
                        wind.Gust = Convert.ToInt32(parts[1].Substring(0, 2));
                    }
                    else
                    {
                        wind.Direction = Convert.ToInt32(windstr.Substring(0, 3));
                        wind.Speed = Convert.ToInt32(windstr.Substring(3, 2));
                    }
                    
                    wind.SetCardinalDirection();
                }
            }
            catch (Exception exp)
            {
                Console.Error.WriteLine("Problem: " + exp.Message);
                Console.Error.WriteLine(exp.StackTrace);
            }

            return wind;
        }

        /**
         * PARSE VISIBILITY ///////////////////////////////////////////////////
         */
        public static string ParseVisibility(string report)
        {

            string vis = "";

            try
            {

                //Regular expressions help to find patterns in strings
                //\d?\s?\d?/?\d?SM - visibility
                Regex reg = new Regex(@"\d?\s?\d?/?\d?SM");
                Match match = reg.Match(report);
                if (match.Success)
                {
                    string tempstr = match.Value;
                    vis = tempstr.Substring(0, tempstr.Length - 2);
                }
            }
            catch (Exception exp)
            {
                Console.Error.WriteLine("Problem: " + exp.Message);
                Console.Error.WriteLine(exp.StackTrace);
            }

            return vis;
        }

        /**
         * TEMPERATURE DATA ///////////////////////////////////////////////////
         */
        public static TemperatureData ParseTemperatureData(string report)
        {

            TemperatureData temp = new TemperatureData();

            try
            {

                //Regular expressions help to find patterns in strings
                Regex reg = new Regex(@"[A-Z]?\d\d/[A-Z]?\d\d");
                Match match = reg.Match(report);
                if (match.Success)
                {
                    string tempstr = match.Value;
                    //split up temperature and dew point
                    string[] parts = tempstr.Split(new char[] { '/' });

                    //convert temperature - see if it starts with a negative value
                    if (parts[0].StartsWith("M"))
                    {
                        temp.Temperature = Convert.ToSingle(parts[0].Substring(1, 2));
                        temp.Temperature *= -1; //make the temperature negative
                    }
                    else
                    {
                        temp.Temperature = Convert.ToSingle(parts[0]);
                    }

                    //convert dew point
                    //convert temperature - see if it starts with a negative value
                    if (parts[1].StartsWith("M"))
                    {
                        temp.Dewpoint = Convert.ToSingle(parts[1].Substring(1, 2));
                        temp.Dewpoint *= -1; //make the temperature negative

                    }
                    else
                    {
                        temp.Dewpoint = Convert.ToSingle(parts[1]);
                    }

                    //do relative humidity calculation
                    temp.DoRelativeHumidityCalculation();
                }
            }
            catch (Exception exp)
            {
                Console.Error.WriteLine("Problem: " + exp.Message);
                Console.Error.WriteLine(exp.StackTrace);
            }

            return temp;
        }

        /**
         * WEATHER DATA ///////////////////////////////////////////////////////
         * 
         * This method uses the following tutorial to derive the principle weather categories which would be encountered;
         * http://www.wunderground.com/metarFAQ.asp
         * 
         * The weather detected here is not exhaustive and uses combinations thought to commonly exist in reports.
         */
        public static string ParseWeatherData(string report)
        {

            string weather = "";
            Regex reg = null;
            Match match = null;

            try
            {
                //DRIZZLE
                //light drizzle
                reg = new Regex(@" -DZ ");
                match = reg.Match(report);
                if (match.Success)
                {
                    weather += "Light Dizzle; ";
                }

                //drizzle
                reg = new Regex(@" DZ ");
                match = reg.Match(report);
                if (match.Success)
                {
                    weather += "Dizzle; ";
                }

                //heavy drizzle
                reg = new Regex(@" \+DZ ");
                match = reg.Match(report);
                if (match.Success)
                {
                    weather += "Heavy Dizzle; ";
                }

                //RAIN
                //light rain
                reg = new Regex(@" -RA");
                match = reg.Match(report);
                if (match.Success)
                {
                    weather += "Light Rain; ";
                }

                //Rain
                reg = new Regex(@" RA ");
                match = reg.Match(report);
                if (match.Success)
                {
                    weather += "Rain; ";
                }

                //heavy rain
                reg = new Regex(@" \+RA ");
                match = reg.Match(report);
                if (match.Success)
                {
                    weather += "Heavy Dizzle; ";
                }

                //light rain showers
                reg = new Regex(@" -SHRA ");
                match = reg.Match(report);
                if (match.Success)
                {
                    weather += "Light Rain Showers; ";
                }

                //rain showers
                reg = new Regex(@" SHRA ");
                match = reg.Match(report);
                if (match.Success)
                {
                    weather += "Rain Showers; ";
                }

                //heavy rain showers
                reg = new Regex(@" \+SHRA ");
                match = reg.Match(report);
                if (match.Success)
                {
                    weather += "Heavy Rain Showers; ";
                }

                //thunderstrom in the vicinity
                reg = new Regex(@" VCTS ");
                match = reg.Match(report);
                if (match.Success)
                {
                    weather += "Thunderstorm in the Vicinity; ";
                }

                //light thundershowers
                reg = new Regex(@" -TSRA ");
                match = reg.Match(report);
                if (match.Success)
                {
                    weather += "Light Thundershowers; ";
                }

                //light thundershowers
                reg = new Regex(@" TSRA ");
                match = reg.Match(report);
                if (match.Success)
                {
                    weather += "Thundershowers; ";
                }

                //heavy thundershowers
                reg = new Regex(@" \+TSRA ");
                match = reg.Match(report);
                if (match.Success)
                {
                    weather += "Heavy Thundershowers; ";
                }

                //light freezing rain
                reg = new Regex(@" -FZRA ");
                match = reg.Match(report);
                if (match.Success)
                {
                    weather += "Light Freezing Rain; ";
                }

                //Freezing rain
                reg = new Regex(@" FZRA ");
                match = reg.Match(report);
                if (match.Success)
                {
                    weather += "Freezing Rain; ";
                }

                reg = new Regex(@" \+FZRA ");
                match = reg.Match(report);
                if (match.Success)
                {
                    weather += "Heavy Freezing Rain; ";
                }

                //SNOW
                //light snow
                reg = new Regex(@" -SN ");
                match = reg.Match(report);
                if (match.Success)
                {
                    weather += "Light Snow; ";
                }

                //snow
                reg = new Regex(@" SN ");
                match = reg.Match(report);
                if (match.Success)
                {
                    weather += "Snow;";
                }

                //heavy snow
                reg = new Regex(@" \+SN ");
                match = reg.Match(report);
                if (match.Success)
                {
                    weather += "Heavy Snow; ";
                }

                //light blowing snow
                reg = new Regex(@" -BLSN ");
                match = reg.Match(report);
                if (match.Success)
                {
                    weather += "Light Blowing Snow; ";
                }

                //blowing snow
                reg = new Regex(@" BLSN ");
                match = reg.Match(report);
                if (match.Success)
                {
                    weather += "Blowing Snow; ";
                }

                //heavy blowing snow
                reg = new Regex(@" \+BLSN ");
                match = reg.Match(report);
                if (match.Success)
                {
                    weather += "Heavy Blowing Snow; ";
                }

                //light snow showers
                reg = new Regex(@" -SHSN ");
                match = reg.Match(report);
                if (match.Success)
                {
                    weather += "Light Snow Showers; ";
                }

                //snow showers
                reg = new Regex(@" SHSN ");
                match = reg.Match(report);
                if (match.Success)
                {
                    weather += "Snow Showers; ";
                }

                //heavy snow showers
                reg = new Regex(@" \+SHSN ");
                match = reg.Match(report);
                if (match.Success)
                {
                    weather += "Heavy Snow Showers; ";
                }

                //Freezing Fog
                reg = new Regex(@" FZFG ");
                match = reg.Match(report);
                if (match.Success)
                {
                    weather += "Freezing Fog; ";
                }

                //light mist
                reg = new Regex(@" -BR ");
                match = reg.Match(report);
                if (match.Success)
                {
                    weather += "Light Mist; ";
                }

                //Mist
                reg = new Regex(@" BR ");
                match = reg.Match(report);
                if (match.Success)
                {
                    weather += "Mist; ";
                }

                //heavy mist
                reg = new Regex(@" \+BR ");
                match = reg.Match(report);
                if (match.Success)
                {
                    weather += "Heavy Mist; ";
                }

                //Haze
                reg = new Regex(@" HZ ");
                match = reg.Match(report);
                if (match.Success)
                {
                    weather += "Haze; ";
                }

                //Spray
                reg = new Regex(@" -BR");
                match = reg.Match(report);
                if (match.Success)
                {
                    weather += "Light Mist; ";
                }

            }
            catch (Exception exp)
            {
                Console.Error.WriteLine("Problem: " + exp.Message);
                Console.Error.WriteLine(exp.StackTrace);
            }

            return weather;
        }

        /**
         * CLOUD DATA /////////////////////////////////////////////////////////
         */
        public static List<CloudData> ParseCloudData(string report)
        {
            List<CloudData> clouds = new List<CloudData>();

            try
            {
                //SKY CLEAR
                Regex regSKC = new Regex(@"SKC\d\d\d*");
                MatchCollection matchSKC = regSKC.Matches(report);
                if (matchSKC.Count > 0)
                {
                    foreach(Match m in matchSKC)
                    {
                        try
                        {
                            CloudData cd = new CloudData();
                            cd.Code = CloudCoverageCode.SKC;
                            cd.Height = 999;
                            clouds.Add(cd);
                        }
                        catch (Exception exp)
                        {
                            Console.Error.WriteLine("Problem: " + exp.Message);
                            Console.Error.WriteLine(exp.StackTrace);
                        }
                    }
                }

                //SKY CLEAR SKC\d\d\d
                Regex regCLR = new Regex(@"CLR");
                MatchCollection matchCLR = regCLR.Matches(report);
                if (matchCLR.Count > 0)
                {
                    foreach (Match m in matchCLR)
                    {
                        try
                        {
                            CloudData cd = new CloudData();
                            cd.Code = CloudCoverageCode.CLR;
                            cd.Height = 999;
                            clouds.Add(cd);
                        }
                        catch (Exception exp)
                        {
                            Console.Error.WriteLine("Problem: " + exp.Message);
                            Console.Error.WriteLine(exp.StackTrace);
                        }
                    }
                }

                //FEW - FEW\d\d\d*
                Regex regFEW = new Regex(@"FEW\d\d\d*");
                MatchCollection matchFEW = regFEW.Matches(report);
                if (matchFEW.Count > 0)
                {
                    foreach (Match m in matchFEW)
                    {
                        try
                        {
                            CloudData cd = new CloudData();
                            cd.Code = CloudCoverageCode.FEW;
                            cd.Height = Convert.ToInt32(m.Value.Substring(3, 3));
                            clouds.Add(cd);
                        }
                        catch (Exception exp)
                        {
                            Console.Error.WriteLine("Problem: " + exp.Message);
                            Console.Error.WriteLine(exp.StackTrace);
                        }
                    }
                }

                //SCATTERED - SCT\d\d\d*
                Regex regSCT = new Regex(@"SCT\d\d\d*");
                MatchCollection matchSCT = regSCT.Matches(report);
                if (matchSCT.Count > 0)
                {
                    foreach (Match m in matchSCT)
                    {
                        try
                        {
                            CloudData cd = new CloudData();
                            cd.Code = CloudCoverageCode.SCT;
                            cd.Height = Convert.ToInt32(m.Value.Substring(3, 3));
                            clouds.Add(cd);
                        }
                        catch (Exception exp)
                        {
                            Console.Error.WriteLine("Problem: " + exp.Message);
                            Console.Error.WriteLine(exp.StackTrace);
                        }
                    }
                }

                //BROKEN - BKN\d\d\d*
                Regex regBKN = new Regex(@"BKN\d\d\d*");
                MatchCollection matchBKN = regBKN.Matches(report);
                if (matchBKN.Count > 0)
                {
                    foreach (Match m in matchBKN)
                    {
                        try
                        {
                            CloudData cd = new CloudData();
                            cd.Code = CloudCoverageCode.BKN;
                            cd.Height = Convert.ToInt32(m.Value.Substring(3, 3));
                            clouds.Add(cd);
                        }
                        catch (Exception exp)
                        {
                            Console.Error.WriteLine("Problem: " + exp.Message);
                            Console.Error.WriteLine(exp.StackTrace);
                        }
                    }
                }

                //OVERCAST - OVC\d\d\d*
                Regex regOVC = new Regex(@"OVC\d\d\d*");
                MatchCollection matchOVC = regOVC.Matches(report);
                if (matchOVC.Count > 0)
                {
                    foreach (Match m in matchOVC)
                    {
                        try
                        {
                            CloudData cd = new CloudData();
                            cd.Code = CloudCoverageCode.OVC;
                            cd.Height = Convert.ToInt32(m.Value.Substring(3, 3));
                            clouds.Add(cd);
                        }
                        catch (Exception exp)
                        {
                            Console.Error.WriteLine("Problem: " + exp.Message);
                            Console.Error.WriteLine(exp.StackTrace);
                        }
                    }
                }

                //OVERCAST - OVC\d\d\d*
                Regex regVV = new Regex(@"VV\d\d\d*");
                MatchCollection matchVV = regVV.Matches(report);
                if (matchOVC.Count > 0)
                {
                    foreach (Match m in matchVV)
                    {
                        try
                        {
                            CloudData cd = new CloudData();
                            cd.Code = CloudCoverageCode.VV;
                            cd.Height = Convert.ToInt32(m.Value.Substring(3, 3));
                            clouds.Add(cd);
                        }
                        catch (Exception exp)
                        {
                            Console.Error.WriteLine("Problem: " + exp.Message);
                            Console.Error.WriteLine(exp.StackTrace);
                        }
                    }
                }

            }
            catch (Exception exp)
            {
                Console.Error.WriteLine("Problem: " + exp.Message);
                Console.Error.WriteLine(exp.StackTrace);
            }

            //sort the clouds list
            //uses the CompareTo method from IComparable
            clouds.Sort();

            return clouds;
        }

        /**
         * PRESSURE ///////////////////////////////////////////////////////////
         */
        public static AtmosphericPressure ParsePressure(string report)
        {
            AtmosphericPressure pressure = new AtmosphericPressure();
            try
            {
                //Regular expressions help to find patterns in strings
                Regex reg = new Regex(@"A\d\d\d\d");
                Match match = reg.Match(report);
                if (match.Success)
                {
                    pressure.Pressure = Convert.ToSingle(match.Value.Substring(1, 4));
                    pressure.Pressure /= 100;
                }
            }
            catch (Exception exp)
            {
                Console.Error.WriteLine("Problem: " + exp.Message);
                Console.Error.WriteLine(exp.StackTrace);
            }

            return pressure;
        }

        /**
         *  DECODE - TOSTRING /////////////////////////////////////////////////
         */
        public static string Decode(Metar report)
        {
            /* here is what a decoded message looks like at the NOAA site
             * RICHMOND INTERNATIONAL  AIRPORT, VA, United States (KRIC) 37-31N 077-19W 50M
             * Nov 18, 2013 - 03:54 PM EST / 2013.11.18 2054 UTC
             * Wind: from the W (260 degrees) at 13 MPH (11 KT):0
             * Visibility: 10 mile(s):0
             * Sky conditions: clear
             * Weather: 
             * Temperature: 70.0 F (21.1 C)
             * Dew Point: 30.9 F (-0.6 C)
             * Relative Humidity: 23%
             * Pressure (altimeter): 29.78 in. Hg (1008 hPa)
             * Pressure tendency: 0.01 inches (0.2 hPa) lower than three hours ago
             * ob: KRIC 182054Z 26011KT 10SM CLR 21/M01 A2978 RMK AO2 SLP089 T02111006 55002
             * cycle: 21
             */
            string output = "";
            Station station = Station.FindStation(report.Icao, Station.stations);

            if (station != null)
            {

                output += station.StationName + " " + station.State + " " + station.CountryCode + " " +
                          " (" + station.Icao + ") " + Math.Round(station.Coordinates.Latitude, 2) + " " + 
                          Math.Round(station.Coordinates.Longitude, 2) + Environment.NewLine;

                output += report.ReportTime.ToLongDateString() + " " + report.reportTime.ToLongTimeString();
                output += Environment.NewLine;

                output += "Wind: from the " + report.Wind.CardinalDirection + " (" + report.Wind.Direction + " degrees) " +
                          "at " + WindData.KnotsInMPH(report.Wind.Speed) + " MPH (" + report.Wind.Speed + " Knots)" + 
                          Environment.NewLine;

                output += "Visibility: " + report.Visibility + " mile(s)" + Environment.NewLine;

                output += "Sky conditions: ";

                if (report.Clouds.Count > 0)
                {
                    foreach (CloudData cloud in report.Clouds)
                    {
                        switch(cloud.Code){
                            case CloudCoverageCode.SKC:
                            case CloudCoverageCode.CLR:
                                output += CloudData.CloudCoverageAbbreviations[cloud.Code] + "; ";
                                break;
                            case CloudCoverageCode.SCT:
                            case CloudCoverageCode.FEW:
                            case CloudCoverageCode.BKN:
                            case CloudCoverageCode.OVC:
                                output += CloudData.CloudCoverageAbbreviations[cloud.Code] + cloud.Height + "00 feet; ";
                                break;
                            case CloudCoverageCode.VV:
                                output += CloudData.CloudCoverageAbbreviations[cloud.Code] + cloud.Height + "00 feet; ";
                                break;
                        }
                    }

                    output += Environment.NewLine;

                }

                //////////////////////////////////////////////////////////////////
                ///////////////!!! TO DO!!! SET WEATHER REPORTING !!!/////////////
                //////////////////////////////////////////////////////////////////
                output += "Weather: " + report.Weather + Environment.NewLine;

                output += "Temperature: " +
                          TemperatureData.CelciusToFahrenheit(report.TempData.Temperature) + " F (" +
                          report.TempData.Temperature + " C)" + Environment.NewLine;

                output += "Dew Point: " +
                          TemperatureData.CelciusToFahrenheit(report.TempData.Dewpoint) + " F (" +
                          report.TempData.Dewpoint + " C)" + Environment.NewLine;

                output += "Relative Humidity: " + Math.Round(report.TempData.RelativeHumidity, 0) + "%" +
                          Environment.NewLine;

                output += "Pressure (altimeter): " + report.Pressure.Pressure + " in. Hg " +
                          "(" + Math.Round(AtmosphericPressure.InchesHgTOhPa(report.Pressure.Pressure),0) + " hPa)" +
                          Environment.NewLine;

                output += "raw: " + report.Raw + Environment.NewLine;
            }
            else
            {
                output = "Station NOT FOUND!";
            }

            return output;
        }
    }
}
