#region + Using Directives
using ScanPDFBoxes.SheetData;
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

namespace ShTempCode.DebugCode
{
	public enum ShowWhere
	{
		NONE = -1,
		DEBUG = 0,
		CONSOLE = 1,
		DBG_CONS = 2
	}

	public class DM
	{
		private static int prefaceWidth = -16;

		public static int[,] dmx;

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

		[DebuggerStepThrough]
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

			string fmt = $"{{0,{prefaceWidth}}}| ";

			if (chgIdxPre > 0) dmx[idx, 0] += chgIdxPre;
			if (chgIdxPost < 0 && dmx[idx, 0] != 0) dmx[idx, 0] += chgIdxPost;

			// string m = msg3 == null ? null : $"{msg3,prefaceWidth}|";
			string m = msg3 == null ? null : string.Format(fmt, msg3);

			if (dmx[idx,0] > 0) m += " ".Repeat(dmx[idx,0] * 2);

			m += msg1;

			if (msg2 != null) m += msg2;

			if (msg4 != null) m += msg4;

			ShowWhere w = where == ShowWhere.NONE ? (ShowWhere) dmx[idx, 1] : where;

			if (chgIdxPre < 0 && dmx[idx, 0] != 0) dmx[idx, 0] += chgIdxPre;
			if (chgIdxPost > 0) dmx[idx, 0] += chgIdxPost;

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
				
				Debug.Write($"{SheetDataManager2.SheetsCount,-4:F0}");

				Debug.Write(msg);

				// ShowSheetRectInfo.showStatus(ShowWhere.DEBUG);
			}
		}

		[DebuggerStepThrough]
		public static void configDebugMsgList()
		{
			// setup the whold list
			dmx = new int[50, 2];

			// 0 through 49
			for (var i = 0; i < 50; i++)
			{
				dmx[i,0] = -1;
				dmx[i,1] = (int) ShowWhere.DEBUG; // default location for all
			}

		}

	}
}
