using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ElendilEvents2JSON
{
    public class EventInVenue 
    {
        public String VenueName { get; set; }
        public List<EventInstance> Dates { get; set; }


        //public override bool Equals(object obj)
        //{
        //    if(object.ReferenceEquals(obj, null)) return false; 
        //    if(object.ReferenceEquals(this, obj)) return true;
        //    if (this.GetType() != obj.GetType()) return false;
        //    return this.Equals(obj as Venue); 
        //}

        //public bool Equals(Venue other)
        //{
        //    if (other.Name == this.Name) return true;
        //    else return false; 
        //}
    }
}