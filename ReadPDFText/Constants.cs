#region + Using Directives
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

#endregion

// user name: jeffs
// created:   3/9/2024 11:32:02 PM

namespace SharedCode.ShDataSupport
{
	public static class Constants
	{
		public static float PI90 { get; private set; }
		public static float PI180 { get; private set; } = (float) Math.PI;
		public static float PI270 { get; private set; }
		public static float PI360 { get; private set; }

		static Constants()
		{
			PI90 = PI180 / 2;
			PI270 = PI90 * 3;
			PI360 = PI180 * 2;
		}

		public static double RadToDegrees(double deg)
		{
			return deg / Math.PI * 180;
		}
	}
}
