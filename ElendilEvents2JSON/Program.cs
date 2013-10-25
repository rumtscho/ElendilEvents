using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Web.Script.Serialization; 

namespace ElendilEvents2JSON
{
    class Program
    {
        private static string currentIndent; 
        static void Main(String[] args)
        {
            //read the file 
            List<int> unreadable;
            List<Event> events = readFile(@".\SourceData.csv", out unreadable);

            //write the file using our own JSON writer. Outputs a nicely formatted file and a warning, but if you have to change the data structure, it will have to be changed as well. 
            string outputText;
            outputText = JSONCreator(events, unreadable);
            File.WriteAllText(@".\output.txt", outputText);

            //write the file using the normal JSON serializer. Will output just everything as a single line. If the data structure is changed, it will output in the new structure. 
            string autoOutput;
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            autoOutput = serializer.Serialize(events); 
            File.WriteAllText(@".\autoOutput.json", autoOutput); 
        }

        public static List<Event> readFile(string path, out List<int> unreadableLines)
        {
            //get the contents out of the file
            var lines = System.IO.File.ReadLines(path);
            // split each line into an array of strings
            var csv = lines
                .Select(line => line.Split(';'))
                .ToArray();

            //will hold all events with the event name as key and an Event object as value
            List<Event> events = new List<Event>();
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
                    if (!events.Select(ev => ev.Name).Contains(eventName))
                    {
                        currentEvent = new Event { Name = eventName };
                        //the venues of the new event are still empty
                        currentEvent.venues = new List<EventInVenue>();
                        events.Add(currentEvent);
                    }
                    else currentEvent = events.Where(ev => ev.Name == eventName).Single();

                    // the same as above: we have the event now, if the current venue isn't yet on its list, enter it, else use the old one
                    string venueName = line[1];
                    EventInVenue currentVenue;
                    if (!currentEvent.venues.Select(ven => ven.VenueName).Contains(venueName))
                    {
                        currentVenue = new EventInVenue { VenueName = venueName };
                        currentVenue.Dates = new List<EventInstance>();
                        currentEvent.venues.Add(currentVenue);
                    }
                    else currentVenue = currentEvent.venues.Where(ven => ven.VenueName == venueName).Single(); 

                    string date = line[2];
                    string cost = line[3];

                    //the event instances within the venue are a simple list, not a dictionary. We just create one and add it to the list. 
                    EventInstance currentEventInstance = new EventInstance { When = date, Cost = cost };
                    currentVenue.Dates.Add(currentEventInstance);
                }
                else
                    //if the line was too short
                    unreadable.Add(lineCounter + 1);

            }
            unreadableLines = unreadable;
            return events;
        }

        public static string JSONCreator(List<Event> events, List<int> unreadable)
        {
            currentIndent = "";
            // this representation of a quote a bit ugly to have everywhere, so we will give it a short name
            string q = '"'.ToString(); 
            // A StringBuilder is practically the same thing as a string, only it can have other strings attached to its end very efficiently
            StringBuilder JSON = new StringBuilder();

            //The buildLine method below creates a nicely formatted line out of a list of strings
            JSON.Append(buildLine(new List<string> { "[" }));

            foreach(Event currentEvent in events)
            {
                JSON.Append(buildLine(new List<string> { "{" }));
                JSON.Append(buildLine(new List<string> { q, "name", q, ": ", q, currentEvent.Name, q }));
                JSON.Append(buildLine(new List<string> {q, "venues", q, ": [" })); 

                foreach(EventInVenue currentVenue in currentEvent.venues)
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
                    // we append a comma after the last element, so when the collection is over, we want to remove the last comma
                    JSON.Remove(JSON.Length - 4, 4);
                    JSON.Append(System.Environment.NewLine); 
                    JSON.Append(buildLine(new List<string> { "]" }));
                    JSON.Append(buildLine(new List<string> { "}, " }));
                }
                JSON.Remove(JSON.Length - 4, 4);
                JSON.Append(System.Environment.NewLine); 
                JSON.Append(buildLine(new List<string> { "]" }));
                JSON.Append(buildLine(new List<string> { "}, " }));

            }
            JSON.Remove(JSON.Length - 4, 4);
            JSON.Append(System.Environment.NewLine); 
            JSON.Append(buildLine(new List<string> { "]" }));

            //warn that we couldn't read some lines
            if (unreadable.Count > 0)
                JSON.AppendLine("Warning! Could not read lines numbers:" + string.Join(",", unreadable)); 

            return JSON.ToString();  

        }
        
        //works on the static variable holding the indent
        public static void increaseIndent()
        {
            currentIndent = currentIndent + "  "; 
        }

        //works on the static variable holding the indent
        public static void decreaseIndent()
        {
            if(currentIndent.Length >=2)
            currentIndent = currentIndent.Substring(0, currentIndent.Length - 2); 
        }

        /// <summary>
        /// create a line with the correct indent and the pieces passed in. It doesn't add spaces between pieces, all pieces must contain their own spaces as needed
        /// </summary>
        /// <param name="contentsList"></param>
        /// <returns></returns>
        public static string buildLine(List<string> contentsList)
        {
            //so we can refer to them by index
            string[] contents = contentsList.ToArray(); 

            //will hold our line
            StringBuilder line = new StringBuilder();

            //we want to have less indentation after closing brackets
            if (contents[0].StartsWith("}") || contents[0].StartsWith("]")) decreaseIndent(); 

            //start with the indentation
            line.Append(currentIndent);

            // append each piece 
            foreach (string piece in contents) line.Append(piece);

            //add a newline 
            line.Append(System.Environment.NewLine);
            
            //if we just opened a bracket, we want more indent in the next line
            if (contents[contents.Length-1].EndsWith("{") || contents[contents.Length-1].EndsWith("[")) increaseIndent();

            return line.ToString(); 
        }
    }
}
