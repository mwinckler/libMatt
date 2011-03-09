using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media;
using libMatt.Classes;

namespace libMatt.Converters {
	public static class ColorConversion {
		// These methods are implemented using algorithms found at 
		// http://130.113.54.154/~monger/hsl-rgb.html, which states that
		// the formulae originally came from _Fundamentals of Interactive Computer Graphics_
		// by Foley and van Dam (c 1982, Addison-Wesley), Chapter 17.

		public static void ToHsl(this Color color, out float H, out float S, out float L) {
			int max = Math.Max(color.R, Math.Max(color.G, color.B));
			int min = Math.Min(color.R, Math.Min(color.G, color.B));

			float 
				maxPct = max / 255f, 
				minPct = min / 255f;

			L = (maxPct + minPct) / 2.0f;
			S = 0;
			H = 0;

			if (max == min) {
				// Some kind of grey.
				S = 0;
				H = 0;
			} else {
				if (L < 0.5f) {
					S = (maxPct - minPct) / (maxPct + minPct);
				} else {
					S = (maxPct - minPct) / (2.0f - maxPct - minPct);
				}
			}

			if (color.R == max) {
				H = (color.ScG - color.ScB) / (maxPct - minPct);
			} else if (color.G == max) {
				H = 2.0f + (color.ScB - color.ScR) / (maxPct - minPct);
			} else {
				H = 4.0f + (color.ScR - color.ScG) / (maxPct - minPct);
			}

			H *= 60;
			if (H < 0)
				H += 360;
		}
		
		public static Color HslToColor(float H, float S, float L) {
			byte sByte = (byte)(S * 255);
			if (S == 0) {
				return Color.FromScRgb(1.0f, L, L, L);
			}

			float temp2;
			if (L < 0.5) {
				temp2 = L * (1.0f + S);
			} else {
				temp2 = L + S - (L * S);
			}

			float temp1 = (2.0f * L) - temp2;

			var hPct = H / 360f;

			Func<float, float> adjustColor = color => {
				if (color < 0)
					color += 1.0f;

				if (color > 1)
					color -= 1.0f;

				if (6.0f * color < 1)
					color = temp1 + (temp2 - temp1) * 6.0f * color;
				else if (2.0 * color < 1)
					color = temp2;
				else if (3.0 * color < 2)
					color = temp1 + (temp2 - temp1) * ((2.0f / 3.0f) - color) * 6.0f;
				else
					color = temp1;

				return color;
			};

			float r, g, b;
			r = adjustColor(hPct + (1.0f / 3.0f));
			g = adjustColor(hPct);
			b = adjustColor(hPct - (1.0f / 3.0f));

			return Color.FromRgb((byte)(r * 255), (byte)(g * 255), (byte)(b * 255));
		}


		public static string ToHex(this Color color) {
			if (color == null)
				return "";
			return string.Format("#{0:X2}{1:X2}{2:X2}{3:X2}", color.A, color.R, color.G, color.B);
		}
	}

}
