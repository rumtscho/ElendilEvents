

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
            List<int> unreadableLines;
            string unreadable; 
            string path = @"~/App_Data/SourceData.csv"; 
            Dictionary<string, Event> eventInstances = readFile(path, out unreadableLines);
            if (unreadableLines.Count > 0)
                unreadable = String.Join(", ", unreadableLines);
            else unreadable = ""; 
            ViewBag.Unreadable = unreadable; 
            return View("Index", eventInstances.Values.ToList());
        }

        /// <summary>
        /// This will read a csv file where each line is formatted as: event name; event venue; date; cost
        /// It is possible to add comments after the cost
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public Dictionary<string, Event> readFile(string path, out List<int> unreadableLines)
        {
            //get the contents out of the file
            var lines = System.IO.File.ReadLines(Server.MapPath(path));
            // split each line into an array of strings
            var csv = lines
                .Select(line => line.Split(';'))
                .ToArray();

            //will hold all events with the event name as key and an Event object as value
            Dictionary<string, Event> events = new Dictionary<string, Event>();
            //will hold the numbers of all lines which were OK
            List<int> unreadable = new List<int>(); 

            //read each line, if you want to skip header lines, change the zero
            for (int lineCounter = 0; lineCounter < csv.Length; lineCounter++)
            {
                string[] line= csv[lineCounter]; 

                if (line.Length >= 4)
                {
                    string eventName = line[0];

                    Event currentEvent;
                    //if we haven't yet created the event, create it now and add it to the dictionary 
                    if (!events.ContainsKey(eventName))
                    {
                        currentEvent = new Event { Name = eventName };
                        //the venues of the new event are still empty
                        currentEvent.venues = new Dictionary<string, EventInVenue>();
                        events.Add(currentEvent.Name, currentEvent);
                    }
                    else currentEvent = events[eventName];

                    // the same as above: we have the event now, if the current venue isn't yet on its list, enter it, else use the old one
                    string venueName = line[1];
                    EventInVenue currentVenue;
                    if (!currentEvent.venues.ContainsKey(venueName))
                    {
                        currentVenue = new EventInVenue { VenueName = venueName };
                        currentVenue.Dates = new List<EventInstance>();
                        currentEvent.venues.Add(venueName, currentVenue);
                    }

                    string date = line[2];
                    string cost = line[3];

                    //the event instances within the venue are a simple list, not a dictionary. We just create one and add it to the list. 
                    EventInstance currentEventInstance = new EventInstance { When = date, Cost = cost };
                    currentEvent.venues[venueName].Dates.Add(currentEventInstance);
                }
                else
                    //if the line was too short
                    unreadable.Add(lineCounter + 1); 

            }
            unreadableLines = unreadable; 
            return events; 
        }
        
    }
}

