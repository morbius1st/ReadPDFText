#region + Using Directives
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SettingsManager;
using ShSheetData;
using ShTempCode.DebugCode;

#endregion

// user name: jeffs
// created:   7/2/2024 8:29:04 PM

namespace ScanPDFBoxes.Process2
{
	// public class SelectScanSettings
	// {
	// 	private static SheetFileManager2 sfm2 = new SheetFileManager2();
	//
	//
	// 	public bool SelectConfigFiles(int def)
	// 	{
	// 		ShSamples samples = new ShSamples();
	//
	// 		if (!samples.SelectScanSample(def)) return false;
	//
	// 		assignConfigFiles(samples.Selected);
	//
	// 		return true;
	// 	}
	//
	// 	public bool AssignConfigFiles(int idx)
	// 	{
	// 		ShSamples samples = new ShSamples();
	//
	// 		if (!samples.SampleScanData.ContainsKey(idx)) return false;
	//
	// 		assignConfigFiles(samples.SampleScanData[idx]);
	//
	// 		return true;
	// 	}
	//
	// 	private void assignConfigFiles(Sample s)
	// 	{
	// 		UserSettings.Data.DataFilePathString = s.DataFilePath.FullFilePath;
	// 		UserSettings.Data.ScanPdfFolder = s.ScanPDfFolder.FullFilePath;
	// 		UserSettings.Data.CreatePdfFilePathString = s.CreatePdfFilePath.FullFilePath;
	// 	}
	//
	//
	//
	// }
}
