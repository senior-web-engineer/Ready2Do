using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Web.Models
{
    public class SchedulerEventViewModel
    {
        /// <summary>
        /// String/Integer. Optional Uniquely identifies the given event. Different instances of repeating events should all have the same id.
        /// </summary>
        public int? Id { get; set; }

        /// <summary>
        /// String. Required. The text on an event's element
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// The date/time an event begins. Required. A Moment-ish input, like an ISO8601 string. 
        /// </summary>
        public string Start { get; set; }

        /// <summary>
        /// The exclusive date/time an event ends. Optional. 
        /// A Moment-ish input, like an ISO8601 string. Throughout the API this will become a real Moment object. 
        /// It is the moment immediately after the event has ended. 
        /// For example, if the last full day of an event is Thursday, the exclusive end of the event will be 00:00:00 on Friday!
        /// </summary>
        public string End { get; set; }

        /// <summary>
        /// Optional. A URL that will be visited when this event is clicked by the user. For more information on controlling this behavior, see the eventClick callback
        /// </summary>
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string Url { get; set; }

        /// <summary>
        /// String/Array. Optional. A CSS class (or array of classes) that will be attached to this event's element.
        /// </summary>
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string ClassName { get; set; }

        /// <summary>
        /// true or false. Optional. Overrides the master editable option for this single event.
        /// </summary>
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public bool? Editable { get; set; }

        /// <summary>
        /// Sets an event's background and border color just like the calendar-wide eventColor option.
        /// </summary>
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string Color { get; set; }

        /// <summary>
        /// Sets an event's background color just like the calendar-wide eventBackgroundColor option.
        /// </summary>
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string BackgroundColor { get; set; }

        /// <summary>
        /// Sets an event's border color just like the the calendar-wide eventBorderColor option.
        /// </summary>        
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string BorderColor { get; set; }

        /// <summary>
        /// Sets an event's text color just like the calendar-wide eventTextColor option.
        /// </summary>        
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string TextColor { get; set; }
    }
}
