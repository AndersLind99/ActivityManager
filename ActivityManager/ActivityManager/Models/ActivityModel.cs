using System;
using System.Collections.Generic;
using System.Text;

namespace ActivityManager.Models
{
    public enum ActivityState
    {
        None = 0,
        Active = 1,
        Schedule = 2,
    }


    public class ActivityModel
    {
        public Guid Id
        {
            get; set;
        }
        public string Name
        {
            get; set;
        }
        public string Type
        {
            get; set;
        }
        public ActivityState State
        {
            get; set;
        }
        public DateTime StartTime
        {
            get; set;
        }
        public DateTime? EndTime
        {
            get; set;
        }

        public string EndTimeString
        {
            get
            {
                if (this.IsFinished)
                    return EndTime.ToString();
                return string.Empty;
            }
        }

        public bool IsFinished
        {
            get; set;
        }



        public string GetSimpleStartTime()
        {
            string minute = StartTime.Minute.ToString();
            if (StartTime.Minute < 10)
                minute = "0" + minute;
            return $"{StartTime.Hour}:{minute}";
        }

        private DateTime ConvertUtcToLocal(DateTime UtcDateTime)
        {
            DateTime.SpecifyKind(UtcDateTime, DateTimeKind.Utc);
            return UtcDateTime.ToLocalTime();
        }
    }
}
