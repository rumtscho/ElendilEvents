using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO; 

namespace ElendilEvents2JSON
{
    class Program
    {
        private static string currentIndent; 
        static void Main(String[] args)
        {
            List<int> unreadable;
            Dictionary<string, Event> events = readFile(@".\SourceData.csv", out unreadable);
            string outputText;

            outputText = JSONCreator(events); 

            File.WriteAllText(@".\output.txt", outputText);
        }

        public static Dictionary<string, Event> readFile(string path, out List<int> unreadableLines)
        {
            //get the contents out of the file
            var lines = System.IO.File.ReadLines(path);
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
                string[] line = csv[lineCounter];

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

        public static string JSONCreator(Dictionary<string, Event> events)
        {
            currentIndent = "";
            // this representation of a quote a bit ugly to have everywhere, so we will give it a short name
            string q = '"'.ToString(); 
            // A StringBuilder is practically the same thing as a string, only it can have other strings attached to its end very efficiently
            StringBuilder JSON = new StringBuilder();

            //The buildLine method below creates a nicely formatted line out of a list of strings
            JSON.Append(buildLine(new List<string> { "[" }));

            foreach(Event currentEvent in events.Values)
            {
                JSON.Append(buildLine(new List<string> { "{" }));
                JSON.Append(buildLine(new List<string> { q, "name", q, ": ", q, currentEvent.Name, q }));
                JSON.Append(buildLine(new List<string> {q, "venues", q, ": [" })); 

                foreach(EventInVenue currentVenue in currentEvent.venues.Values)
                {
                JSON.Append(buildLine(new List<string> { "{" }));
                JSON.Append(buildLine(new List<string> {q, "venue", q, ": ", q, currentVenue.VenueName, q }));
                JSON.Append(buildLine(new List<string> { q, "dates", q, ": [" }));
                    foreach(EventInstance currentDate in currentVenue.Dates)
                    {
                        JSON.Append(buildLine(new List<string> { "{" }));
                        JSON.Append(buildLine(new List<string> { q, "date", q, ": ", q, currentDate.When, q }));
                        JSON.Append(buildLine(new List<string> { q, "price", q, ": ", q, currentDate.Cost, q }));
                        JSON.Append(buildLine(new List<string> { "}, " }));
                    }
                    JSON.Remove(JSON.Length - 4, 4);
                    JSON.Append(buildLine(new List<string> { "]" }));
                    JSON.Append(buildLine(new List<string> { "}, " }));
                }
                JSON.Remove(JSON.Length - 4, 4);
                JSON.Append(buildLine(new List<string> { "]" }));
                JSON.Append(buildLine(new List<string> { "}, " }));

            }
            JSON.Remove(JSON.Length - 4, 4);

            return JSON.ToString();  

        }

        public static void increaseIndent()
        {
            currentIndent = currentIndent + "  "; 
        }

        public static void decreaseIndent()
        {
            currentIndent = currentIndent.Substring(0, currentIndent.Length - 2); 
        }

        public static string buildLine(List<string> contentsList)
        {
            string[] contents = contentsList.ToArray(); 
            StringBuilder line = new StringBuilder();
            if (contents[0] == "}" || contents[0] == "]") decreaseIndent(); 
            line.Append(currentIndent);
            foreach (string piece in contents) line.Append(piece);
            line.Append(System.Environment.NewLine);
            if (contents[contents.Length-1] == "{" || contents[contents.Length-1] == "[") increaseIndent();

            return line.ToString(); 
        }
    }
}
