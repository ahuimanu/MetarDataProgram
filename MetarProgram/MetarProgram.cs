using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using MetarLib;

namespace MetarData
{
    public class MetarProgram
    {

        private string stationsFile = @"C:\temp\stations.txt";
        private string favoritesFile = @"C:\temp\favorites.txt";
        private string prompt = "Welcome to the METAR Reader, please make a selection:\n" +
                                "   1. Show Current Weather for favorite stations\n" +
                                "   2. Show Past Weather for favorite stations (1 to 23 hours ago)\n" +
                                "   3. Show/Edit favorite stations\n" +
                                "   4. Quit";

        private WebClient client;
        
        private string[] favorites;
        public string[] Favorites
        {
            get { return favorites; }
            set { favorites = value; }
        }

        private DateTime currentTime;
        public DateTime CurrentTime
        {
            get { return currentTime; }
            set { currentTime = value; }
        }

        private List<Metar> currentReports;
        public List<Metar> CurrentReports
        {
            get { return currentReports; }
            set { currentReports = value; }
        }

        /**
         *  ENTRY POINT ///////////////////////////////////////////////////////
         */
        public static void Main()
        {
            new MetarProgram();
        }

        /**
         *  CONSTRUCTOR ///////////////////////////////////////////////////////
         */
        public MetarProgram()
        {
            Init();
            ShowMenu();
        }

        /**
         * INIT ///////////////////////////////////////////////////////////////
         */
        private void Init()
        {

            //set current time
            CurrentTime = DateTime.Now;
            CurrentReports = new List<Metar>();
            client = new WebClient();
            client.DownloadFileCompleted += client_DownloadFileCompleted;
            client.DownloadProgressChanged += client_DownloadProgressChanged;

            LoadStations();
            WriteStationsToFile();
            LoadFavorites();
            LoadCurrentReports();
        }

        /*
         * LOAD FAVORITES /////////////////////////////////////////////////////
         */
        private void LoadFavorites()
        {
            try
            {
                favorites = File.ReadAllLines(favoritesFile);
            }
            catch (Exception exp)
            {
                Console.Error.WriteLine("Problem: " + exp.Message);
                Console.Error.WriteLine(exp.StackTrace);
            }
        }

        /**
         * LOAD STATIONS //////////////////////////////////////////////////////
         */
        private void LoadStations()
        {
            try
            {
                //download file using the Web Client
                client.DownloadFileAsync(Station.URL_UCAR_RAP, stationsFile);

                //wait until the client is done
                while (client.IsBusy)
                {
                    Thread.Sleep(10);
                }
                string[] lines = File.ReadAllLines(stationsFile, Encoding.UTF8);
                Station.stations = new List<Station>();

                for (int i = 0; i < lines.Length; i++)
                {
                    //check to see that the line is 83 chars in length
                    if (lines[i].Length == 83)
                    {
                        Station station = Station.ParseStation(lines[i]);
                        if (station != null)
                        {
                            Station.stations.Add(station);
                        }
                    }
                }
            }
            catch (Exception exp)
            {
                Console.Error.WriteLine("Problem: " + exp.Message);
                Console.Error.WriteLine(exp.StackTrace);
            }
        }

        /**
         * WRITE STATIONS /////////////////////////////////////////////////////
         */
        private void WriteStationsToFile()
        {
            //batch all out to a file
            string[] content = new string[Station.stations.Count];
            for (int i = 0; i < content.Length; i++)
            {
                content[i] = Station.stations[i].ToString();
            }

            File.WriteAllLines(stationsFile, content);
        }

        /**
         * LOAD CURRENT REPORTS ///////////////////////////////////////////////
         */
        private void LoadCurrentReports()
        {
            try
            {
                if (favorites.Length != 0)
                {
                    for (int i = 0; i < favorites.Length; i++)
                    {
                        string report = client.DownloadString(Metar.URL_LATEST_OBSERVATION + favorites[i].ToUpper() + ".TXT");
                        Metar met = Metar.Parse(report);
                        CurrentReports.Add(met);
                    }
                }
                else
                {
                    Console.Error.WriteLine("Favorites Not Loaded");
                }
            }
            catch (Exception exp)
            {
                Console.Error.WriteLine("Problem: " + exp.Message);
                Console.Error.WriteLine(exp.StackTrace);
            }
        }

        /**
         * SHOW MENU //////////////////////////////////////////////////////////
         */
        private void ShowMenu()
        {
            bool quit = false;
            try
            {
                while (!quit)
                {
                    Console.WriteLine(prompt);
                    int selection = Convert.ToInt32(Console.ReadLine());
                    switch (selection)
                    {
                        case 1:
                            Console.Clear();
                            DisplayCurrentFavoriteStationsReports();
                            break;
                        case 2:
                            //TO DO!
                            Console.Clear();
                            DisplayPreviousReportsForFavoriteStation();
                            //Console.WriteLine("TO DO!");
                            break;
                        case 3:
                            Console.Clear();
                            EditFavoriteStationsList();
                            break;
                        case 4:
                            //all is well
                            quit = true;
                            break;
                        default:
                            Console.Clear();
                            Console.WriteLine("That is not a valid selection, try again!");
                            break;
                    }
                }
            }
            catch (Exception exp)
            {
                Console.Error.WriteLine("Problem: " + exp.Message);
                Console.Error.WriteLine(exp.StackTrace);
            }
        }

        /*
         * DISPLAY CURRENT FAV STATION REPORTS ////////////////////////////////
         */
        private void DisplayCurrentFavoriteStationsReports()
        {
            string prompt = "Select favorite station to display..." + Environment.NewLine;
            int choice = 0;
            for (int i = 0; i < Favorites.Length; i++)
            {
                prompt += "   " + (i + 1) + ". " + Favorites[i] + Environment.NewLine;
            }

            Console.WriteLine(prompt);

            try
            {
                choice = Convert.ToInt32(Console.ReadLine());
                
                Console.Clear();
                //indexes the desired current report
                //Console.WriteLine("Length: " + CurrentReports.Count);
                Console.WriteLine(Metar.Decode(CurrentReports[choice - 1]));
            }
            catch (Exception exp)
            {
                Console.Error.WriteLine("Invalid choice!");
                Console.WriteLine(exp.StackTrace);
                Console.WriteLine(exp.Message);
            }

            Console.WriteLine("Press ENTER to continue...");
            Console.ReadLine();
            Console.Clear();
        }

        /*
         * DISPLAY PREVIOUS FAV STATION REPORTS ////////////////////////////////
         */
        private void DisplayPreviousReportsForFavoriteStation()
        {
            string date = "";
            string report = "";

            //walks through the favorites list
            string prompt = "Select favorite station to display..." + Environment.NewLine;
            int choice = 0;
            for (int i = 0; i < Favorites.Length; i++)
            {
                prompt += "   " + (i + 1) + ". " + Favorites[i] + Environment.NewLine;
            }

            Console.WriteLine(prompt);

            try
            {
                choice = Convert.ToInt32(Console.ReadLine());

                Console.Clear();
                //indexes the desired current report
                //Console.WriteLine("Length: " + CurrentReports.Count);
                //Console.WriteLine(Metar.Decode(CurrentReports[choice - 1]));
                int cycle = ShowHours();
                string data = "";
                
                //if the cycle selected is less than 9, then place a leading zero
                if (cycle < 10)
                {
                    Console.WriteLine(Metar.URL_LATEST_CYCLES + "0" + cycle + "Z.TXT");
                    data = client.DownloadString(Metar.URL_LATEST_CYCLES + "0" + cycle + "Z.TXT");
                    
                }
                //or not
                else
                {
                    Console.WriteLine(Metar.URL_LATEST_CYCLES + "" + cycle + "Z.TXT");
                    data = client.DownloadString(Metar.URL_LATEST_CYCLES + "" + cycle + "Z.TXT");
                }

                string[] reports = data.Split(new char[] { '\n' });
                if (reports.Length != 0)
                {
                    for (int i = 0; i < reports.Length; i++)
                    {
                        if (reports[i].StartsWith(Favorites[choice - 1]))
                        {
                            //reconstruct string as found in the file from NOAA
                            date = reports[i - 1];
                            report = reports[i];
                            report = date + "\n" + report;

                            Metar met = Metar.Parse(report);
                            Console.WriteLine(Metar.Decode(met));
                            break; //leave the loop

                        }
                    }
                }
                else
                {
                    Console.Error.WriteLine("Favorites Not Loaded");
                }
            }
            catch (Exception exp)
            {
                Console.WriteLine(exp.StackTrace);
                Console.WriteLine(exp.Message);
            }

            Console.WriteLine("Press ENTER to continue...");
            Console.ReadLine();
            Console.Clear();
        }

        /**
         * SHOW HOURS /////////////////////////////////////////////////////////
         */
        private int ShowHours()
        {
            Console.WriteLine("Please select the number of the cycle file you wish to review");
            Console.WriteLine("The current Cycle is: " + DateTime.Now.ToUniversalTime().Hour + "Z");

            for(int i = 1; i <= 12; i++)
            {
                Console.WriteLine("{0}Z\t\t{1}Z", i, i + 12);
            }

            int selection = Convert.ToInt32(Console.ReadLine());

            return selection;
            
        }

        /**
         * EDIT FAVORITES /////////////////////////////////////////////////////
         */
        private void EditFavoriteStationsList()
        {
            string prompt = "Select favorite station to replace..." + Environment.NewLine;
            prompt = "  You must use a United States station (begins with K)" + Environment.NewLine;
            for (int i = 0; i < Favorites.Length; i++)
            {
                prompt += "   " + (i + 1) + ". " + Favorites[i] + Environment.NewLine;
            }
            prompt += "   9. Cancel" + Environment.NewLine;
            Console.WriteLine(prompt);

            int choice = 0;

            try
            {
                choice = Convert.ToInt32(Console.ReadLine());

                //quit
                if (choice == 9)
                {
                    Console.Clear();
                    return;
                }

                Console.Clear();
                //indexes the desired current report
                Console.WriteLine("What is the ICAO code for your new favorite at position " + (choice + 1) + "?");
                string icao = Console.ReadLine();

                while (icao.Length != 4 || !icao.StartsWith("K"))
                {
                    Console.WriteLine("The station entered, " + icao + " is incorrect");
                    Console.WriteLine("What is the ICAO code for your new favorite at position " + (choice + 1) + "?");
                    icao = Console.ReadLine();
                }

                Favorites[choice - 1] = icao;
                File.WriteAllLines(favoritesFile, Favorites);
                Init();

            }
            catch (Exception exp)
            {
                Console.Error.WriteLine("Invalid choice!");
                Console.WriteLine(exp.Message);
            }

        }

        private void client_DownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e)
        {
            Console.Clear();
            Console.WriteLine("Loading file: " + e.ProgressPercentage + "%");

        }

        private void client_DownloadFileCompleted(object sender, System.ComponentModel.AsyncCompletedEventArgs e)
        {
            Console.Clear();
            Console.WriteLine("Complete");
        }
    }
}
