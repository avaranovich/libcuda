using System;
using System.Diagnostics;
using System.Text;

namespace Libcuda.Api.DataTypes
{
    // note. cannot use TimeSpan instead because it ain't work with fractions of milliseconds

    [DebuggerNonUserCode]
    public class ElapsedTime
    {
        public float TotalDays { get; set; }
        public float TotalHours { get; set; }
        public float TotalMinutes { get; set; }
        public float TotalSeconds { get; set; }

        public int Days { get; set; }
        public int Hours { get; set; }
        public int Minutes { get; set; }
        public int Seconds { get; set; }
        public float Flash { get; set; }

        public ElapsedTime()
            : this(0f)
        {
        }

        public ElapsedTime(float msecs)
        {
            TotalDays = msecs / (1000 * 60 * 60 * 24);
            TotalHours = msecs / (1000 * 60 * 60);
            TotalMinutes = msecs / (1000 * 60);
            TotalSeconds = msecs / 1000;

            Days = (int)Math.Floor(TotalDays);
            Hours = (int)Math.Floor(TotalHours) - Days * 24;
            Minutes = (int)Math.Floor(TotalMinutes) - Hours * 60;
            Seconds = (int)Math.Floor(TotalSeconds) - Minutes * 60;
            Flash = TotalSeconds - (float)Math.Floor(TotalSeconds);
        }

        public override String ToString()
        {
            var buf = new StringBuilder();
            if (Days != 0) buf.AppendFormat(Days.ToString("00") + ":");
            buf.AppendFormat("{0:00}:{1:00}:{2:00}.{3:000000}",
                Hours, Minutes, Seconds, (int)(Flash * 1e6));
            return buf.ToString();
        }
    }
}