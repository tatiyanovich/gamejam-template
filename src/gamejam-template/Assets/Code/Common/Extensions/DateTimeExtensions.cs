using System;

namespace Code.Common.Extensions
{
	public static class DateTimeExtensions
	{
		public static DateTime ToDateTime(this long unixTime)
		{
			return DateTimeOffset.FromUnixTimeSeconds(unixTime).DateTime;
		}

		public static long ToUnixTimeSeconds(this DateTime dateTime)
		{
			return dateTime.ToDateTimeOffset(TimeSpan.Zero).ToUnixTimeSeconds();
		}
        
		public static DateTimeOffset ToDateTimeOffset(this DateTime dateTime)
		{
			return dateTime.ToDateTimeOffset(TimeSpan.Zero);
		}

		public static DateTimeOffset ToDateTimeOffset(this DateTime dateTime, TimeSpan offset)
		{
			return new DateTimeOffset(DateTime.SpecifyKind(dateTime, DateTimeKind.Unspecified), offset);
		}
	}
}