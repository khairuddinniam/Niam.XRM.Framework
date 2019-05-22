using System;

namespace LateBound.Example.Plugins.Business
{
    public class DateRange
    {
        public DateTime From { get; }
        
        public DateTime To { get; }

        public DateRange(DateTime from, DateTime to)
        {
            From = from;
            To = to;
        }
    }
}