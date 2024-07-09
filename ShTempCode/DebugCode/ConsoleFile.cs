#region + Using Directives
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualBasic;
using UtilityLibrary;

#endregion

// user name: jeffs
// created:   6/30/2024 9:44:06 PM

namespace ShTempCode.DebugCode
{
	// public class ConsoleFile
	// {
	// 	public static string OutputFile {get; set; }
	// 	public static bool Enabled { get; set; }
	//
	//
	// 	public static void Configure(string outputFile)
	// 	{
	// 		OutputFile = outputFile;
	//
	// 		string folder = Path.GetDirectoryName(outputFile);
	//
	// 		Enabled = Directory.Exists(folder);
	// 	}
	//
	// 	public static void WriteLine(string text)
	// 	{
	// 		Write(text+"\n");
	// 	}
	//
	// 	public static void Write(string text)
	// 	{
	// 		Console.Write(text);
	// 		if (Enabled) File.AppendAllText(OutputFile, text);
	// 	}
	// }

	public static class ConsoleFile
	{
		public static string OutputFile {get; set; }
		public static bool Enabled { get; set; }

		private static TextWriter _current;

		private class OutputWriter : TextWriter
		{
			public override Encoding Encoding
			{
				get
				{
					return _current.Encoding;
				}
			}

			public override void WriteLine(string value)
			{
				Write(value+"\n");
			}

			public override void Write(string value)
			{
				_current.Write(value);
				File.AppendAllText(OutputFile, value);
			}
		}

		public static void Init(string outputFile)
		{
			OutputFile = outputFile;

			string folder = Path.GetDirectoryName(outputFile);

			bool result = File.Exists(outputFile);

			if (result)
			{
				File.Delete(outputFile);
			}

			Enabled = Directory.Exists(folder);

			_current = Console.Out;
			Console.SetOut(new OutputWriter());

			Console.WriteLine("*".Repeat(30));
			Console.WriteLine(DateAndTime.Now.ToString());
			Console.WriteLine("*".Repeat(30)+"\n");
		}
	}
}
