using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ElendilEvents2JSON
{
    public class Event
    {
        public String Name { get; set; }
        public List<EventInVenue> venues { get; set; }
        
    }
}