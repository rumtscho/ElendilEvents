using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ElendilEvents2JSON
{
    public class Event
    {
        public String Name { get; set; }
        public Dictionary<string, EventInVenue> venues { get; set; }
        
    }
}