using System;

namespace Woof.TextEx {

    public class DateTimeFormatAttribute : Attribute {

        public string Format { get; } = "yyyy-MM-dd";

        public DateTimeFormatAttribute(string format) {
            if (format != null) Format = format;
        }

    }

    public class TimeSpanFormatAttribute : Attribute {

        public string Format { get; } = "HH:mm";


        public TimeSpanFormatAttribute(string format) {
            if (format != null) Format = format;
        }

    }

}