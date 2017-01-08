using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.IO;
using HtmlAgilityPack;

namespace MtaTrainTime
{
    class TrainTimeFunctions
    {
        static public DataTable UpdateTime(DataTable oldDT)
        {
            DataTable newDT = new DataTable();  // Create our new data table that will have updated times
            //  Add columns to new Data table
            newDT.Columns.Add("Station ID");
            newDT.Columns.Add("Uptown");
            newDT.Columns.Add("Downtown");

            foreach (DataRow row in oldDT.Rows) // For each row
            {
                object[] vals = row.ItemArray;  // Get row as object
                string stationID = vals[0].ToString(); // Get station ID
                int trainAtStationResponse = isTrainAtStation(stationID);

                if (trainAtStationResponse != 0)
                {
                    if (trainAtStationResponse < 0) // Downtown
                    {
                        newDT.Rows.Add(stationID, "", "X");
                    }

                    if (trainAtStationResponse > 0) // Uptown
                    {
                        newDT.Rows.Add(stationID, "X");
                    }
                }
                else
                {
                    newDT.Rows.Add(stationID);
                }
            }
            oldDT.Dispose();
            return newDT;
        }

        //Given the station ID, we will determine if there is a train at that station
        // If so, return 1 for an uptown train, -1 for downtown train, 0 for no train at that station
        static int isTrainAtStation(string stationID)  
        {
            var client = new WebClient();
            client.DownloadFile("http://apps.mta.info/trainTime/getTimesByStation.aspx?stationID=" + stationID, "times.txt");
            string[] times = File.ReadAllLines("times.txt");    // Read all lines of the file
            foreach (string line in times)
            {
                if (line.StartsWith("direction1[")) // Uptown Line
                {
                    string[] lineArray = line.Split(',');   // Splitting the array using , as a delimiter



                    TimeSpan trainTimeSpan = TimeStampToRegular(lineArray[1]);    // Turn time from result into timespan object
                    TimeSpan currentTime = new TimeSpan(DateTime.Now.Hour,DateTime.Now.Minute,DateTime.Now.Second);    // Turn current time into timespan object
                    
                    var diff = trainTimeSpan.TotalMinutes - currentTime.TotalMinutes; // subtract trantime from current time
                    if (diff < 2)   // If the train arrives in 1 minute or less then the train is at the station
                    {
                        File.Delete("times.txt");   // Delete the file downloaded since we don't need it anymore
                        return 1;
                    }

                }
                else if (line.StartsWith("direction2[")) // Downtown Line
                {
                    string[] lineArray = line.Split(',');   // Splitting the array using , as a delimiter

                    TimeSpan trainTimeSpan = TimeStampToRegular(lineArray[1]);    // Turn time from result into timespan object
                    TimeSpan currentTime = new TimeSpan(DateTime.Now.Hour, DateTime.Now.Minute, DateTime.Now.Second);    // Turn current time into timespan object

                    var diff = trainTimeSpan.TotalMinutes - currentTime.TotalMinutes; // subtract trantime from current time
                    if (diff < 2)   // If the train arrives in 1 minute or less then the train is at the station
                    {
                        File.Delete("times.txt");   // Delete the file downloaded since we don't need it anymore
                        return -1;

                    }
                }
            }
            File.Delete("times.txt");   // Delete the file downloaded since we don't need it anymore
            return 0;
        }

        public static DateTime? ConvertUnixTimeStamp(long unixTimeStamp)
        {
            DateTime sTime = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            sTime.AddSeconds(unixTimeStamp);
     
            return sTime.Subtract(new TimeSpan(0,0, 18000));

        }

        static TimeSpan TimeStampToRegular(string unixStamp)
        {
            var client = new WebClient();
            client.DownloadFile("http://www.epochconverter.com/timezones?q="+ unixStamp +"&tz=America%2FNew_York", "stamp.txt");
            string[] times = File.ReadAllLines("stamp.txt");    // Read all lines of the file
            HtmlDocument doc = new HtmlDocument();
            doc.Load("stamp.txt");
            foreach (HtmlNode node in doc.DocumentNode.SelectNodes("//*[@id=\"contentcolumn\"]/div[2]/b[1]"))
            {
                //Example String we get from server Sunday January 08, 2017 05:57:42 (am)
                var lol = node.InnerHtml;
                var formatted = lol.Remove(lol.Length - 5, 5);  //Sunday January 08, 2017 05:57:42 (am) --> Sunday January 08, 2017 05:57:42
                formatted = formatted.Remove(0, formatted.Length - 8); //Sunday January 08, 2017 05:57:42 --> 05:57:42 
                File.Delete("stamp.txt");
                return TimeSpan.Parse(formatted);
            }
            return new TimeSpan();
            
        }
    }
    
}
