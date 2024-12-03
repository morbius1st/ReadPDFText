#region + Using Directives
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using ShSheetData.ShSheetData2;
using UtilityLibrary;
#endregion

// user name: jeffs
// created:   6/23/2024 5:59:36 PM

namespace DebugCode
{
	// public enum ShowWhere
	// {
	// 	NONE = -1,
	// 	DEBUG = 0,		// debug window
	// 	CONSOLE = 1,	// console window
	// 	DBG_CONS = 2,	// debug and console windows
	// 	WPF_TBX = 3,	// wpf text box
	// 	DBG_TBX = 4,	// debug and text box
	// 	
	// }

	public class DM2
	{

		// number of debug channels
		public static int Quantity { get; set; }

		private static int prefaceWidth = -16;

		// dmx[x,0] = tab depth
		// dmx[x,1] = output location (per ShowWhere)
		public static int[,] dmx;

		public static void init(int qty)
		{
			Quantity = qty;

			configDebugMsgList();
		}


		[DebuggerStepThrough]
		public static void DbxSetIdx(int idx, int value)
		{
			dmx[idx, 0] = value;
		}

		[DebuggerStepThrough]
		public static void DbxChgIdx(int idx, int value)
		{
			dmx[idx, 0] += value;
		}

		[DebuggerStepThrough]
		public static void DbxLineEx(int idx, string msg1,
			int chgIdxPre = 0,
			int chgIdxPost = 0,
			ShowWhere where = ShowWhere.NONE,
			string msg2 = null,
			[CallerMemberName] string mx = null,
			[CallerFilePath] string sx = null
			)
		{
			if (dmx[idx,0] < 0) return;

			string zx = null;
			
			if (sx != null)
			{
				sx = Path.GetFileNameWithoutExtension(sx) + " . ";
			}

			if (mx != null)
			{
				sx += mx;

				if (msg1.StartsWith("start") || msg1.StartsWith("end"))
				{
					zx = $" ({mx})";
				}
			}

			prefaceWidth = -44;

			Dbx(idx, msg1, "\n",chgIdxPre, chgIdxPost, where, msg2, sx, zx);
		}

		[DebuggerStepThrough]
		public static void DbxEx(int idx, string msg1,
			int chgIdxPre = 0,
			int chgIdxPost = 0,
			ShowWhere where = ShowWhere.NONE,
			string msg2 = null,
			[CallerMemberName] string mx = null,
			[CallerFilePath] string sx = null)
		{
			if (dmx[idx,0] < 0) return;

			string zx = null;

			if (sx != null)
			{
				sx = Path.GetFileNameWithoutExtension(sx) + " . ";
			}			

			if (mx != null)
			{
				sx += mx;

				if (msg1.StartsWith("start") || msg1.StartsWith("end"))
				{
					zx = $" ({mx})";
				}
			}

			prefaceWidth = -44;

			Dbx(idx,msg1, null, chgIdxPre, chgIdxPost, where, msg2, sx, zx);
		}


		[DebuggerStepThrough]
		public static void DbxLine(int idx, string msg1,
			int chgIdxPre = 0,
			int chgIdxPost = 0,
			ShowWhere where = ShowWhere.NONE,
			string msg2 = null)
		{
			if (dmx[idx,0] < 0) return;

			string s = MethodBase.GetCurrentMethod().DeclaringType.Name;

			Dbx(idx, msg1, "\n",chgIdxPre, chgIdxPost,where, msg2);
		}

		// [DebuggerStepThrough]
		public static void Dbx(int idx, string msg1,
			string t1,
			int chgIdxPre = 0,
			int chgIdxPost = 0,
			ShowWhere where = ShowWhere.NONE,
			string msg2 = null,
			string msg3 = null,
			string msg4 = null)
		{
			if (dmx[idx,0] < 0) return;


			dmx[idx, 0] = dmx[idx, 0] + chgIdxPre < 0 ? 0 : dmx[idx, 0] + chgIdxPre;

			string s2 = $"{dmx[idx, 0],3:F0}";

			string fmt = $"{{0,{prefaceWidth}}}{s2}| ";

			string m = msg3 == null ? null : string.Format(fmt, msg3);

			if (dmx[idx,0] > 0) m += " ".Repeat(dmx[idx,0] * 2);

			// string s1 = $"  ({dmx[idx, 0].ToString("F0")})";
			// t1 = s1 + t1;

			m += msg1;

			if (msg2 != null) m += msg2;

			if (msg4 != null) m += msg4;

			ShowWhere w = where == ShowWhere.NONE ? (ShowWhere) dmx[idx, 1] : where;


			dmx[idx, 0] = dmx[idx, 0] + chgIdxPost < 0 ? 0 : dmx[idx, 0] + chgIdxPost;

			showDmx(m+t1, w);
		}

		[DebuggerStepThrough]
		private static void showDmx(string msg, ShowWhere where)
		{
			if (where == ShowWhere.CONSOLE || where == ShowWhere.DBG_CONS)
			{
				Console.Write(msg);
			} 

			if (where == ShowWhere.DEBUG  || where == ShowWhere.DBG_CONS)
			{
				Debug.Write(msg);
			}

		}

		[DebuggerStepThrough]
		public static void configDebugMsgList()
		{
			// setup the whole list
			dmx = new int[Quantity, 2];

			// 0 through 49
			for (var i = 0; i < Quantity; i++)
			{
				dmx[i,0] = -1;
				dmx[i,1] = (int) ShowWhere.DEBUG; // default location for all
			}

		}

	}
}
