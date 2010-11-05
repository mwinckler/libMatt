using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;
using libMatt.MathExtensions;

namespace libMattTests {
	[TestFixture]
	public class MathTest {

		#region Date rounding
		[Test]
		public void RoundTimeSpanToMinuteDown() {
			Assert.AreEqual(
				TimeSpan.FromMinutes(1).Ticks,
				TimeSpan.FromSeconds(89).Round(TimeSpan.FromMinutes(1)).Ticks
			);
		}

		[Test]
		public void RoundTimeSpanToMinuteUp() {
			Assert.AreEqual(
				TimeSpan.FromMinutes(2).Ticks,
				TimeSpan.FromSeconds(91).Round(TimeSpan.FromMinutes(1)).Ticks
			);
		}

		[Test]
		public void RoundTimeSpanToMinuteEven() {
			Assert.AreEqual(
				TimeSpan.FromMinutes(4).Ticks,
				TimeSpan.FromSeconds(220).Round(TimeSpan.FromMinutes(1)).Ticks
			);
		}

		[Test]
		public void RoundTimeSpanToFourMinutes() {
			Assert.AreEqual(
				TimeSpan.FromMinutes(4).Ticks,
				TimeSpan.FromSeconds(260).Round(TimeSpan.FromMinutes(4)).Ticks
			);
		}


		[Test]
		public void RoundDateTimeToMinute() {
			Assert.AreEqual(
				new DateTime(2010, 11, 4, 10, 28, 0).Ticks,
				new DateTime(2010, 11, 4, 10, 28, 27).Round(TimeSpan.FromMinutes(1)).Ticks
			);
		}

		[Test]
		public void RoundDateTimeToDayUp() {
			Assert.AreEqual(
				new DateTime(2010, 11, 5).Ticks,
				new DateTime(2010, 11, 4, 13, 28, 27).Round(TimeSpan.FromDays(1)).Ticks
			);
		}

		[Test]
		public void RoundDateTimeToDayDown() {
			Assert.AreEqual(
				new DateTime(2010, 11, 4).Ticks,
				new DateTime(2010, 11, 4, 11, 28, 27).Round(TimeSpan.FromDays(1)).Ticks
			);
		}

		[Test]
		public void RoundTimeToNearestEven() {
			Assert.AreEqual(
				TimeSpan.FromMinutes(2).Ticks,
				TimeSpan.FromSeconds(150).Round(TimeSpan.FromMinutes(1), MidpointRounding.ToEven).Ticks
			);
		}

		[Test]
		public void RoundTimeAwayFromZero() {
			Assert.AreEqual(
				TimeSpan.FromMinutes(3).Ticks,
				TimeSpan.FromSeconds(150).Round(TimeSpan.FromMinutes(1), MidpointRounding.AwayFromZero).Ticks
			);
		}

		[Test]
		public void RoundToNearestFiveSeconds() {
			Assert.AreEqual(
				TimeSpan.FromSeconds(40).Ticks,
				TimeSpan.FromSeconds(38).Round(TimeSpan.FromSeconds(5)).Ticks
			);

			Assert.AreEqual(
				TimeSpan.FromSeconds(40).Ticks,
				TimeSpan.FromSeconds(42).Round(TimeSpan.FromSeconds(5)).Ticks
			);

			
			Assert.AreNotEqual(
				TimeSpan.FromSeconds(40).Ticks,
				TimeSpan.FromSeconds(43).Round(TimeSpan.FromSeconds(5)).Ticks
			);

			Assert.AreEqual(
				new TimeSpan(0, 2, 0).Ticks,
				new TimeSpan(0, 1, 58).Round(TimeSpan.FromSeconds(5)).Ticks
			);

		}

		[Test]
		public void RoundToNearestThirtySevenMinutes() {
			Assert.AreEqual(
				new TimeSpan(3, 42, 0).Ticks,
				new TimeSpan(3, 34, 0).Round(TimeSpan.FromMinutes(37)).Ticks
			);

			Assert.AreNotEqual(
				new TimeSpan(3, 42, 0).Ticks,
				new TimeSpan(3, 21, 0).Round(TimeSpan.FromMinutes(37)).Ticks
			);
		}


		[Test]
		public void TruncateDateTimeToDay() {
			Assert.AreEqual(
				new DateTime(2010, 11, 4).Ticks,
				new DateTime(2010, 11, 4, 15, 24, 13).Truncate(TimeSpan.FromDays(1)).Ticks
			);
		}

		[Test]
		public void TruncateTimespanToDay() {
			Assert.AreEqual(
				TimeSpan.FromDays(2).Ticks,
				new TimeSpan(70, 0, 0).Truncate(TimeSpan.FromDays(1)).Ticks
			);
		}

		[Test]
		public void TruncateDateTimeToHour() {
			Assert.AreEqual(
				new DateTime(2010, 11, 4, 13, 0, 0).Ticks,
				new DateTime(2010, 11, 4, 13, 52, 47).Truncate(TimeSpan.FromHours(1)).Ticks
			);
		}


		#endregion

	}
}
