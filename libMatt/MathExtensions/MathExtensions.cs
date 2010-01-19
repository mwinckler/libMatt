using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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

	}
}
