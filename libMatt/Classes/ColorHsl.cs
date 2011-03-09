using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace libMatt.Classes {
	public class ColorHsl {

		public ColorHsl(float h, float s, float l) {
			this.H = h;
			this.S = s;
			this.L = l;
		}

		public float H { get; set; }
		public float S { get; set; }
		public float L { get; set; }

	}
}
