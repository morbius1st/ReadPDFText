#region + Using Directives

using System;
using SettingsManager;
using UtilityLibrary;

#endregion

// user name: jeffs
// created:   9/2/2020 6:01:46 AM

namespace SettingsManager
{
	public static class FileLocationSupport
	{
		// shortcut: "." + FileLocationSupport.DATA_FILE_EXT
		public const string DATA_FILE_EXT = "xml";

		public static readonly string FILE_PATTERN = "*."  +  DATA_FILE_EXT;

		public const string SORT_NAME_PROP = "SortName";

		public const string DEFAULT_FOLDER_NAME = "Default";

		// root locations
		public static string FileRootLocationUser => $"{UserSettings.Path.SuiteFolderPath}";

		public static string FileRootLocationDefault => $"{SuiteSettings.Path.FilePath.FolderPath}";


	#region sheet metrics

		// sheet metrics = data about sheets and box settings

		public const string SHEET_STORAGE_FOLDER_NAME = ".Sheet Metrics Files";

		// shortcut: FileLocationSupport.ClassifFileLocation + "\\"
		/// <summary>
		/// the general location for all of the user sheet metrics files
		/// </summary>
		public static string ShtMetricsFileLocation => $"{FileRootLocationUser}\\{SHEET_STORAGE_FOLDER_NAME}";

		// shortcut: FileLocationSupport.ClassifFileLocationUser + "\\" 
		/// <summary>
		/// the location for the user's sheet metrics files
		/// </summary>
		public static string  ShtMetricsFileLocationUser => $"{ShtMetricsFileLocation}\\{Environment.UserName}";

		/// <summary>
		/// the file path to a user's sheet metrics file
		/// </summary>
		public static string ShtMetricsFilePathUser(string fileId)
		{
			return ShtMetricsFileLocationUser + $"\\({Environment.UserName}) {fileId}.{DATA_FILE_EXT}";
		}


		/// <summary>
		/// the location for all of the default classification files, if any (management provided)
		/// will also contain some examples
		/// </summary>
		public static string ShtMetricsFileLocationDefault => $"{FileRootLocationDefault}\\{SHEET_STORAGE_FOLDER_NAME}";


		/// <summary>
		/// the file path to a default sheet metrics file
		/// </summary>
		public static string ShtMetricsFilePathDefault(string fileId)
		{
			return ShtMetricsFileLocationDefault + $"\\({DEFAULT_FOLDER_NAME}) {fileId}.{DATA_FILE_EXT}";
		}

	#endregion

	#region classification file

		// shortcut: FileLocationSupport.SAMPLE_FOLDER
		public const string SAMPLE_FOLDER = "Sample Files";

		// shortcut: "." + FileLocationSupport.SAMPLE_FILE_EXT
		public const string SAMPLE_FILE_EXT = "Sample";

		public const string CLASSIF_STORAGE_FOLDER_NAME = ".User Classification Files";

		// shortcut: FileLocationSupport.ClassifFileLocation + "\\"
		/// <summary>
		/// the location for all of the user classification files
		/// </summary>
		public static string ClassifFileLocation => $"{FileRootLocationUser}\\{CLASSIF_STORAGE_FOLDER_NAME}";

		/// <summary>
		/// the path for the root folder of all of the user classification files<br/>
		/// equals: "C:\ProgramData\{app name}\Andy\User Classification Files  (SM7.5)<br/>
		/// or equals: "C:\ProgramData\{app name}\Andy\{app name}\User Classification Files  (pre SM7.5)<br/>
		/// </summary>
		public static FilePath<FileNameSimple> ClassifFileLocationPath =>
			new FilePath<FileNameSimple>(ClassifFileLocation);


		// shortcut: FileLocationSupport.ClassifFileLocationUser + "\\" 
		/// <summary>
		/// the location for the user's classification files
		/// </summary>
		public static string ClassifFileLocationUser => $"{ClassifFileLocation}\\{Environment.UserName}";

		/// <summary>
		/// the path for the user's user classification files<br/>
		/// equals: "C:\ProgramData\{app name}\Andy\User Classification Files\{user name}  (SM7.5)<br/>
		/// or equals: "C:\ProgramData\{app name}\Andy\{app name}\User Classification Files\{user name}  (pre SM7.5)<br/>
		/// </summary>
		public static FilePath<FileNameSimple> ClassifFilePathUser =>
			new FilePath<FileNameSimple>(ClassifFileLocationUser);

		/// <summary>
		/// the file path to a user's classification file
		/// </summary>
		public static string ClassifFileUserPath(string fileId)
		{
			return ClassifFileLocationUser + $"\\({Environment.UserName}) {fileId}.{DATA_FILE_EXT}";
		}

		// shortcut: FileLocationSupport.ClassifSampleFileLocationUser
		/// <summary>
		/// the location for the user's classification file's sample folder
		/// </summary>
		public static string ClassifSampleFileLocationUser => $"{ClassifFileLocationUser}\\{SAMPLE_FOLDER}";

		/// <summary>
		/// the file path to a user's sample file
		/// </summary>
		public static string ClassifSampleFilePathUser(string fileId)
		{
			return ClassifSampleFileLocationUser + $"\\({Environment.UserName}) {fileId}.{SAMPLE_FILE_EXT}";
		}


		/// <summary>
		/// the location for all of the default classification files, if any (management provided)
		/// will also contain some examples
		/// </summary>
		public static string ClassifFileLocationDefault => $"{FileRootLocationDefault}\\{CLASSIF_STORAGE_FOLDER_NAME}";


		/// <summary>
		/// the file path to a default sheet metrics file
		/// </summary>
		public static string ClassifFileDefaultPath(string fileId)
		{
			return ClassifFileLocationDefault + $"\\({DEFAULT_FOLDER_NAME}) {fileId}.{DATA_FILE_EXT}";
		}

		/// <summary>
		/// the location for all of the default classification files, if any (management provided)
		/// will also contain some examples
		/// </summary>
		public static string ClassifSampleFileLocationDefault => $"{ClassifFileLocationDefault}\\{SAMPLE_FOLDER}";


		/// <summary>
		/// the file path to a default sheet metrics file
		/// </summary>
		public static string ClassifSampleFileDefaultPath(string fileId)
		{
			return ClassifSampleFileLocationDefault + $"\\({DEFAULT_FOLDER_NAME}) {fileId}.{SAMPLE_FILE_EXT}";
		}

	#endregion

		// @formatter:off

		private const int SUBJECT_WIDTH = -50;

		public static void ShowLocations(IWin w)
		{
			w.DebugMsgLine("Showing Locations");

			w.DebugMsgLine("\n*** consts (1) ***\n");

			w.DebugMsgLine($"\t{"data file extension (1)"                               , SUBJECT_WIDTH} | {DATA_FILE_EXT}");
			w.DebugMsgLine($"\t{"data file pattern (2)"                                 , SUBJECT_WIDTH} | {FILE_PATTERN}");
			w.DebugMsgLine($"\t{"sort name prop (3)"                                    , SUBJECT_WIDTH} | {SORT_NAME_PROP}");
			w.DebugMsgLine($"\t{"default folder name (4)"                               , SUBJECT_WIDTH} | {DEFAULT_FOLDER_NAME}");
			
			w.DebugMsgLine("\n*** system (101) ***\n");
			
			w.DebugMsgLine($"\t{"user name (101)"                                       , SUBJECT_WIDTH} | {Environment.UserName}");


			w.DebugMsgLine("\n*** locations - general (A) ***");

			w.DebugMsgLine($"\t{"FileRootLocationUser (A1)"                             , SUBJECT_WIDTH} | {FileRootLocationUser}");
			w.DebugMsgLine($"\t{"FileRootLocationDefault (A2)"                          , SUBJECT_WIDTH} | {FileRootLocationDefault}");



			w.DebugMsgLine("\n\n*** locations - sheet metrics (B1) ***");

			w.DebugMsgLine($"\t{"sheet metrics folder name (B1)"                        , SUBJECT_WIDTH} | {SHEET_STORAGE_FOLDER_NAME}");

			w.DebugMsgLine("\n*** locations - sheet metrics - user (B11) ***");

			w.DebugMsgLine($"\t{"general user's location         (B11=A1+B1)"           , SUBJECT_WIDTH} | {ShtMetricsFileLocation}");
			w.DebugMsgLine($"\t{"user's location                 (B12=B11+101)"         , SUBJECT_WIDTH} | {ShtMetricsFileLocationUser}");
			w.DebugMsgLine($"\t{"user's file name                (B101=B12++)"          , SUBJECT_WIDTH} | {ShtMetricsFilePathUser("USER FILE ID")}");


			w.DebugMsgLine("\n*** locations - sheet metrics - default (machine) (B51) ***");

			w.DebugMsgLine($"\t{"general default location       (A21)"                  , SUBJECT_WIDTH} | {FileRootLocationDefault}");
			w.DebugMsgLine($"\t{"default location               (B51=B11+4)"            , SUBJECT_WIDTH} | {ShtMetricsFileLocationDefault}");
			w.DebugMsgLine($"\t{"default's file name            (B102=B51++)"           , SUBJECT_WIDTH} | {ShtMetricsFilePathDefault("DEFAULT FILE ID")}");



			w.DebugMsgLine("\n\n*** locations - classification file (C1) ***");

			w.DebugMsgLine($"\t{"SAMPLE_FOLDER (C1)"                                    , SUBJECT_WIDTH} | {SAMPLE_FOLDER}");
			w.DebugMsgLine($"\t{"SAMPLE_FILE_EXT (C2)"                                  , SUBJECT_WIDTH} | {SAMPLE_FILE_EXT}");
			w.DebugMsgLine($"\t{"CLASSIF_STORAGE_FOLDER_NAME (C3)"                      , SUBJECT_WIDTH} | {CLASSIF_STORAGE_FOLDER_NAME}");
			

			w.DebugMsgLine("\n*** locations - classification file - user (C11) ***");
			
			// user location
			w.DebugMsgLine($"\t{"general user's location        (C11=A1+C3)"            , SUBJECT_WIDTH} | {ClassifFileLocation}");
			w.DebugMsgLine($"\t{"general user's location (path) (C12->C11)"             , SUBJECT_WIDTH} | {ClassifFileLocationPath.FolderPath}");

			w.DebugMsgLine($"\t{"user's location                (C13=C11+101)"          , SUBJECT_WIDTH} | {ClassifFileLocationUser}");
			w.DebugMsgLine($"\t{"user's location (path)         (C14->C13)"             , SUBJECT_WIDTH} | {ClassifFilePathUser.FolderPath}");
			w.DebugMsgLine($"\t{"user's file name               (C101=C13++)"           , SUBJECT_WIDTH} | {ClassifFileUserPath("USER FILE ID")}");
			
			w.DebugMsgLine($"\t{"user's sample location         (C15=C11+4))"           , SUBJECT_WIDTH} | {ClassifSampleFileLocationUser}");
			w.DebugMsgLine($"\t{"user's sample file name        (C101=C13++)"           , SUBJECT_WIDTH} | {ClassifSampleFilePathUser("USER FILE ID")}");

			w.DebugMsgLine("\n*** locations - classification file - default (C51) ***");
			
			w.DebugMsgLine($"\t{"general default location       (A2)"                   , SUBJECT_WIDTH} | {FileRootLocationDefault}");

			w.DebugMsgLine($"\t{"default location               (C51=A2+4))"            , SUBJECT_WIDTH} | {ClassifFileLocationDefault}");
			w.DebugMsgLine($"\t{"default file name              (C102=C51++)"           , SUBJECT_WIDTH} | {ClassifFileDefaultPath("DEFAULT FILE ID")}");

			w.DebugMsgLine($"\t{"default sample location        (C53=A21+C1))"          , SUBJECT_WIDTH} | {ClassifSampleFileLocationDefault}");
			w.DebugMsgLine($"\t{"default sample file name       (C103=C53++)"           , SUBJECT_WIDTH} | {ClassifSampleFileDefaultPath("DEFAULT FILE ID")}");


			// @formatter:on
		}
	}
}