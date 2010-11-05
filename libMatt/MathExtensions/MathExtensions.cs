using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using libMatt.Converters;
using System.ComponentModel;

namespace libMatt.MathExtensions {
	public static class MathExtensions {

		/// <summary>
		/// Finds and returns the smallest number in a series of numbers.
		/// </summary>
		/// <param name="Num1"></param>
		/// <param name="nums"></param>
		/// <example>double m = Min(4,2,3,1,7); // returns 1</example>
		/// <returns>The smallest of any number passed to the function.</returns>
		public static double Min(params double[] nums) {
			if (nums == null || nums.Length == 0) {
				return 0;
			}

			double ret = nums[0];
			for (int i = 1; i < nums.Length; i++) {
				ret = Math.Min(ret, nums[i]);
			}
			return ret;
		}


		/// <summary>
		/// Finds and returns the smallest number in a series of numbers.
		/// </summary>
		/// <param name="Num1"></param>
		/// <param name="nums"></param>
		/// <example>double m = Max(4,2,3,1,7); // returns 7</example>
		/// <returns>The largest of any number passed to the function.</returns>
		public static double Max(params double[] nums) {
			if (nums == null || nums.Length == 0) {
				return 0;
			}

			double ret = nums[0];
			for (int i = 1; i < nums.Length; i++) {
				ret = Math.Max(ret, nums[i]);
			}
			return ret;
		}


		/// <summary>
		/// Returns a value representing &lt;time&gt; rounded to the nearest multiple of &lt;roundingInterval&gt;.
		/// This function has a maximum granularity of seconds.
		/// </summary>
		/// <param name="time">The timespan to be rounded.</param>
		/// <param name="roundingInterval">The interval to which &lt;time&gt; should be rounded. (E.g. to round to the nearest 5 minutes, supply TimeSpan.FromMinutes(5).)</param>
		/// <returns></returns>

		public static TimeSpan Round(this TimeSpan time, TimeSpan roundingInterval, MidpointRounding roundingType) {
			return new TimeSpan(
				Convert.ToInt64(Math.Round(
					time.Ticks / (decimal)roundingInterval.Ticks,
					roundingType
				)) * roundingInterval.Ticks
			);
		}

		public static TimeSpan Round(this TimeSpan time, TimeSpan roundingInterval) {
			return Round(time, roundingInterval, MidpointRounding.ToEven);
		}

		public static DateTime Round(this DateTime datetime, TimeSpan roundingInterval) {
			return new DateTime((datetime - DateTime.MinValue).Round(roundingInterval).Ticks);
		}


		public static TimeSpan Truncate(this TimeSpan span, TimeSpan interval) {
			return new TimeSpan(
				Convert.ToInt64(
					Math.Floor(span.Ticks / (decimal)interval.Ticks) * interval.Ticks
				)
			);
		}

		public static DateTime Truncate(this DateTime datetime, TimeSpan interval) {
			return new DateTime((datetime - DateTime.MinValue).Truncate(interval).Ticks);
		}

	}

}
