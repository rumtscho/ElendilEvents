using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ElendilEvents.Models
{
    public class EventInstance
    {
        public String Name { get; set; }
        public String Where { get; set; }
        public DateTime When { get; set; }
        public String errorMessage { get; set; }
    }
}