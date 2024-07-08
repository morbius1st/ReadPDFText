#region + Using Directives
using Microsoft.VisualStudio.OLE.Interop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Org.BouncyCastle.Bcpg.OpenPgp;
using static ShCode.StatCodeGroups;
using static ShCode.StatCodes;

#endregion

// user name: jeffs
// created:   7/6/2024 10:35:05 AM

namespace ShCode
{
	public enum StatCodeGroups
	{
		SCG_G       = 0,
		SCG_INIT    = 1000,
		SCG_CFG     = SCG_INIT + 20,  // 1020
		SCG_DM      = SCG_CFG  + 20,  // 1040
		SCG_SFM     = SCG_DM   + 20,  // 1060
	}



	public enum StatCodes
	{
		SC_SFM_SHEET_DATA_FOLDER_MISSING               = -SCG_SFM - 1,

		SC_DM_INIT_DATA_MANAGER_FAILED                 = -SCG_DM - 3,
		SC_DM_DATA_FILE_PATH_ALREADY_SET               = -SCG_DM - 2,
		SC_DM_DATA_FILE_HAS_SHEETS                     = -SCG_DM - 1,

		SC_CFG_DATA_FILE_SHEET_LIST_INVALID            = -SCG_CFG - 9,
		SC_CFG_DATA_FILE_HAS_SHEETS_INVALID            = -SCG_CFG - 3,
		SC_CFG_DATA_FILE_MISSING                       = -SCG_CFG - 2,
		SC_CFG_DATA_FILE_PATH_MISSING                  = -SCG_CFG - 1,

		SC_INIT_GET_PATHS_FAIL                         = -SCG_INIT - 1,
		SC_INIT_CFG_DATA_PATH_FAIL                     = -SCG_INIT - 2,
		SC_INIT_CFG_SHT_DATA_PATH_FAIL                 = -SCG_INIT - 3,
		SC_INIT_START_DATA_MGR_FAIL                    = -SCG_INIT - 4,
		SC_INIT_LOAD_SHT_DATA_FILES_FAIL               = -SCG_INIT - 5,


		SC_G_FAIL   = SCG_G - 1, // general fail code

		SC_G_NONE   = SCG_G + 0, // 0 neutral

		SC_G_GOOD   = SCG_G + 1, // >0 good
		SC_G_WORKED = SCG_G + 2, // >0 good

	}



	public class StatMgr
	{
		public static Tuple<StatCodes, string, string, string> StatusCode { get; private set; }

		public static void SetStatCode(StatCodes code, string note = null,  
			[CallerMemberName] string mx = null, [CallerFilePath] string sx = null)
		{
			sx = Path.GetFileNameWithoutExtension(sx);

			StatusCode = new Tuple<StatCodes, string, string, string>(code, note, mx, sx);
		}

		public static StatCodes Current => StatusCode.Item1;

		public static string StatusMessage(StatCodes code)
		{
			string msg = $"Status Code Undefined ({code.ToString()})";

			Tuple<StatCodeGroups, string> desc;

			if (StatusDesc.TryGetValue(code, out desc))
			{
				msg = $"{desc.Item2} ({(int) code})";
			}

			return msg;
		}

		public static void ShowStatus(bool showFrom = false, bool showOk = false)
		{
			string s2 = null;

			if (StatusCode.Item4 != null && StatusCode.Item3 != null)
			{
				s2 = $"({StatusCode.Item4}/{StatusCode.Item3})";
			}
			else
			if (StatusCode.Item4 != null || StatusCode.Item2 != null)
			{
				s2 = $"({StatusCode.Item4}{StatusCode.Item3})";
			}


			string s1;
			s1 = $" | {StatMgr.StatusMessage(StatusCode.Item1)}";
			s1 += StatusCode.Item2 != null ? $"({StatusCode.Item2})" : null;
			s1 += showFrom && s2 !=null ? $" | {s2}" : null;



			if (showOk && StatusCode.Item1 > SC_G_NONE)
			{
				s1 = "\n*** Good " + s1;
				Console.WriteLine(s1);
			} 
			else 
			if (StatusCode.Item1 < SC_G_NONE)
			{
				s1 = "\n*** Error " + s1;
				Console.WriteLine(s1);
			}
		}

		// static data

		public static Dictionary<StatCodeGroups, string> StatusGroupDesc = new ()
		{
			{ SCG_G,   "General" },
			{ SCG_CFG, "Configuration" },
			{ SCG_DM,  "Data Manager" },
			{ SCG_SFM, "Sheet File Manager" },
		};

		public static Dictionary<StatCodes, Tuple<StatCodeGroups, string>> StatusDesc = new ()
		{
			{ SC_G_NONE, new Tuple<StatCodeGroups, string>(SCG_G, "None / Unassigned") },
			{ SC_G_GOOD, new Tuple<StatCodeGroups, string>(SCG_G, "Good") },
			{ SC_G_WORKED, new Tuple<StatCodeGroups, string>(SCG_G, "Process Worked") },

			{ SC_DM_DATA_FILE_HAS_SHEETS, 
				new Tuple<StatCodeGroups, string>(SCG_DM, "The DataManager already has sheets loaded") },

			{ SC_DM_DATA_FILE_PATH_ALREADY_SET, 
				new Tuple<StatCodeGroups, string>(SCG_DM, "The file path for the DataManager is already assigned") },

			{ SC_DM_INIT_DATA_MANAGER_FAILED, 
				new Tuple<StatCodeGroups, string>(SCG_DM, "Initializing the DataManager failed") },



			{ SC_SFM_SHEET_DATA_FOLDER_MISSING, 
				new Tuple<StatCodeGroups, string>(SCG_SFM, "The SheetData Folder is missing") },



			{ SC_CFG_DATA_FILE_PATH_MISSING, 
				new Tuple<StatCodeGroups, string>(SCG_SFM, "The DataManager file path is missing") },

			{ SC_CFG_DATA_FILE_MISSING, 
				new Tuple<StatCodeGroups, string>(SCG_SFM, "The DataManager file is missing") },

			{ SC_CFG_DATA_FILE_HAS_SHEETS_INVALID, 
				new Tuple<StatCodeGroups, string>(SCG_SFM, "The DataManager sheet List is not valid") },

			{ SC_CFG_DATA_FILE_SHEET_LIST_INVALID, 
				new Tuple<StatCodeGroups, string>(SCG_SFM, "The SheetData file list is not valid") },
			


			{ SC_INIT_GET_PATHS_FAIL, 
				new Tuple<StatCodeGroups, string>(SCG_SFM, "Getting the initial data paths failed") },

			{ SC_INIT_CFG_DATA_PATH_FAIL, 
				new Tuple<StatCodeGroups, string>(SCG_SFM, "Configuring the DataManager path failed") },

			{ SC_INIT_CFG_SHT_DATA_PATH_FAIL, 
				new Tuple<StatCodeGroups, string>(SCG_SFM, "Configuring the Sheet Data path failed") },

			{ SC_INIT_START_DATA_MGR_FAIL, 
				new Tuple<StatCodeGroups, string>(SCG_SFM, "Starting the DataManager failed") },

			{ SC_INIT_LOAD_SHT_DATA_FILES_FAIL, 
				new Tuple<StatCodeGroups, string>(SCG_SFM, "Loading the SheetData failed") },


		};



	}
}
