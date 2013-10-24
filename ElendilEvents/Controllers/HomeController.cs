using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.IO;
using ElendilEvents.Models; 

namespace ElendilEvents.Controllers
{
    public class HomeController : Controller
    {
        // displays all events
        public ActionResult Index()
        {
            List<EventInstance> eventInstances = readFile();
            return View("Index", eventInstances);
        }

        /// <summary>
        /// Reads the data from the csv file and returns it as prepared objects. 
        /// It expects the data to be available in the format 
        /// Event name; Event location; Date 
        /// Always one per line
        /// </summary>
        /// <param name="path">The path where the file with the events is</param>
        /// <returns></returns>
        public List<EventInstance> readFile(string path = @"~/App_Data/SourceData.csv")
        {
            //get the contents out of the file
            var lines = System.IO.File.ReadLines(Server.MapPath(path));
            // split each line into an array of strings
            var csv = lines
                .Select(line => line.Split(';'))
                .ToArray();

            //an empty list to hold our objects
            List<EventInstance> eventInstances = new List<EventInstance>(); 

            //This loop reads each line once
            for (int lineCounter = 0; lineCounter < csv.Length; lineCounter++ )
            {
                //an empty object which we will fill with info from the line
                EventInstance evInstance = new EventInstance();

                //check that we have at least 3 segments, else write an error instead of name
                if (csv[lineCounter].Length >= 3)
                {
                    // csv is an array of arrays of strings. To get a single string, we need to use to indices. 
                    //The first index gives us the current line, the second gives us the position of the string within the line. Arrays in C# are zero-based, so csv[lineCounter][2] gives us the third piece of the current line. 
                    string timeAsString = csv[lineCounter][2];
                    // an empty date object into which we will read the time from the csv
                    DateTime timeAsDate;
                    //TryParse is a method which returns true if it was successful. It reads the string written as a first argument, and writes the result into the variable named as second argument (after the keyword out). 
                    // if there is something wrong with the date in the file, we will write this info into the name column. 
                    if (DateTime.TryParse(timeAsString, out timeAsDate))
                    {
                        //The object evInstance has four fields. We fill the three we could read, and leave the errorMessage empty. 
                        evInstance.When = timeAsDate;
                        evInstance.Where = csv[lineCounter][1];
                        evInstance.Name = csv[lineCounter][0];
                        evInstance.errorMessage = ""; 
                    }
                    else evInstance.errorMessage = String.Format("Could not read date in row {0}", lineCounter + 1);
                }
                else evInstance.errorMessage = String.Format("Row {0} is not available in the expected format", lineCounter + 1); 

                // This line adds the event instance we just created to the list of all event instances 
                eventInstances.Add(evInstance); 
            }
            return eventInstances; 
        }
    }
}