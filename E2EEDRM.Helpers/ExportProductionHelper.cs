using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;

namespace E2EEDRM.Helpers
{
	public class ExportProductionHelper
	{
		public static async Task DownloadProductionAsync(int workspaceArtifactId, int productionArtifactId)
		{
			try
			{
				Console2.WriteDisplayStartLine("Downloading the Production");

				if (!File.Exists(Constants.ProductionExport.RdcExecutablePath))
				{
					throw new Exception("RDC Executable does not exist");
				}

				string exportSettingsLocation = System.Reflection.Assembly.GetExecutingAssembly().Location.Replace(System.Reflection.Assembly.GetExecutingAssembly().GetName().Name + ".dll", Constants.ProductionExport.ExportSettingsFileName);
				if (!File.Exists(exportSettingsLocation))
				{
					FileStream stream = File.Create(exportSettingsLocation);
					stream.Close();
				}

				string command = $@".\kCura.EDDS.WinForm.exe -u:{Constants.Instance.RELATIVITY_ADMIN_USER_NAME} -p:{Constants.Instance.RELATIVITY_ADMIN_PASSWORD} -k:{exportSettingsLocation} -m:export -c:{workspaceArtifactId}";
				string exportToFolderPath = DetermineFolderPathToExport();
				Directory.CreateDirectory(exportToFolderPath);

				// Create Export Settings File (.kwx)
				CreateExportSettingsFile(productionArtifactId, exportSettingsLocation, exportToFolderPath);

				// Run Production Export
				await RunProductionExport(command);

				Console2.WriteDisplayEndLine($"Downloaded the Production to {exportToFolderPath}!");
			}
			catch (Exception ex)
			{
				throw new Exception("Error Downloading the Production", ex);
			}
		}

		private static string DetermineFolderPathToExport()
		{
			string exportToFolderPath;
			if (System.Reflection.Assembly.GetExecutingAssembly().Location.Contains("E2EEDRM.REST"))
			{
				exportToFolderPath = System.Reflection.Assembly.GetExecutingAssembly().Location.Replace(
					@"E2EEDRM.REST\bin\Debug\" + System.Reflection.Assembly.GetExecutingAssembly().GetName().Name + ".dll",
					Constants.ProductionExport.ExportedProductionFolderName);
			}
			else
			{
				exportToFolderPath = System.Reflection.Assembly.GetExecutingAssembly().Location.Replace(
					@"E2EEDRM\bin\Debug\" + System.Reflection.Assembly.GetExecutingAssembly().GetName().Name + ".dll",
					Constants.ProductionExport.ExportedProductionFolderName);
			}

			return exportToFolderPath;
		}

		private static async Task RunProductionExport(string command)
		{
			Process process = new Process();
			ProcessStartInfo startInfo = new ProcessStartInfo("cmd", "/c" + command);
			startInfo.WindowStyle = ProcessWindowStyle.Hidden;
			startInfo.WorkingDirectory = Path.GetDirectoryName(Constants.ProductionExport.RdcExecutablePath);
			startInfo.RedirectStandardOutput = true;
			startInfo.UseShellExecute = false;
			process.StartInfo = startInfo;

			await Task.Run(() => process.Start());
			string output = process.StandardOutput.ReadToEnd();
			Console2.WriteDebugLine(output);
			process.WaitForExit();
		}

		private static void CreateExportSettingsFile(int productionArtifactId, string exportSettingsLocation,
			string exportToFolderPath)
		{
			string[] arr = File.ReadAllLines(exportSettingsLocation);
			string exportSettingsContent =
				Constants.ProductionExport.ExportSettingsTemplate.Replace("*ProductionArtifactId*",
					productionArtifactId.ToString());
			exportSettingsContent = exportSettingsContent.Replace("*ExportFolderPath*", exportToFolderPath);
			File.WriteAllText(exportSettingsLocation, exportSettingsContent);
		}
	}
}
