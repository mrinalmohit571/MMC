using NUnit.Framework;
using System;

namespace MMC.Enums
{
    public class FrameworkConstants
    {
        public static TimeSpan MidTimeSpan => TimeSpan.FromMilliseconds(
        Convert.ToInt32("5000"));

        public static TimeSpan ShortTimeSpan => TimeSpan.FromMilliseconds(
          Convert.ToInt32(TestContext.Parameters.Get("shortTime", "1000")));

        public static TimeSpan LongTimeSpan => TimeSpan.FromMilliseconds(
            Convert.ToInt32(TestContext.Parameters.Get("longTime", "30000")));

    }
}



