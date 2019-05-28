using E2EEDRM.Helpers;
using E2EEDRM.TransferApiConsole;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using Constants = E2EEDRM.Helpers.Constants;

namespace E2EEDRM
{
	public class Program
	{
		private static int _dtSearchArtifactId;
		private static int _workspaceArtifactId;
		private static int _productionArtifactId;
		private static ConnectionManager _connectionManager;

		public static void Main(string[] args)
		{
			Stopwatch stopwatch = new Stopwatch();
			stopwatch.Start();

			try
			{
				MainAsync(args).Wait();
			}
			finally
			{
				stopwatch.Stop();
				DisplayTimeElapsed(stopwatch.Elapsed, "Overall ");

				Console.ReadKey();
			}
		}

		public static async Task MainAsync(string[] args)
		{
			try
			{
				_connectionManager = new ConnectionManager();

				await CleanupWorkspacesAsync();

				await CreateWorkspaceAsync();

				await TransferDocumentsAsync();

				await CreateAndRunProcessingSetAsync();

				await CreateAndBuildDtSearch();

				await TagDocumentsAsResponsiveAsync();

				await CreateAndRunImagingSetAsync();

				await CreateAndRunProductionAsync();

				await DownloadProductionAsync();
			}
			catch (Exception ex)
			{
				string errorMessage = $"An error occured in the {nameof(MainAsync)} method.";
				Console2.WriteErrorLine(errorMessage);
				throw new Exception(errorMessage, ex);
			}
			finally
			{
				await DeleteWorkspaceAsync();
			}
		}

		private static async Task CleanupWorkspacesAsync()
		{
			Console2.WriteStartHeader("Clean Up Workspaces");
			Stopwatch stopwatch = new Stopwatch();
			stopwatch.Start();

			WorkspaceHelper workspaceHelper = new WorkspaceHelper(_connectionManager.RsapiClient);
			await workspaceHelper.CleanupWorkspacesAsync();

			stopwatch.Stop();
			DisplayTimeElapsed(stopwatch.Elapsed);
		}

		private static async Task TransferDocumentsAsync()
		{
			Console2.WriteStartHeader("Transfer Documents");
			Stopwatch stopwatch = new Stopwatch();
			stopwatch.Start();

			TransferApiHelper transferApiHelper = new TransferApiHelper(
				relativityUrl: Constants.Instance.BASE_INSTANCE_URL,
				relativityUserName: Constants.Instance.RELATIVITY_ADMIN_USER_NAME,
				relativityPassword: Constants.Instance.RELATIVITY_ADMIN_PASSWORD,
				workspaceArtifactId: _workspaceArtifactId,
				localDocumentsFolderPath: Constants.Transfer.LOCAL_DOCUMENTS_FOLDER_PATH,
				remoteDocumentsFolderPath: Constants.Transfer.REMOTE_DOCUMENTS_FOLDER_PATH);

			await transferApiHelper.TransferDocumentsAsync();

			stopwatch.Stop();
			DisplayTimeElapsed(stopwatch.Elapsed);
		}

		private static async Task CreateWorkspaceAsync()
		{
			Console2.WriteStartHeader("Create Workspace");
			Stopwatch stopwatch = new Stopwatch();
			stopwatch.Start();

			WorkspaceHelper workspaceHelper = new WorkspaceHelper(_connectionManager.RsapiClient);

			_workspaceArtifactId = await workspaceHelper.QueryTemplateAndCreateWorkspaceAsync();

			// Create the Responsive - E2E Field
			await workspaceHelper.CreateResponsiveFieldAsync(_workspaceArtifactId);

			stopwatch.Stop();
			DisplayTimeElapsed(stopwatch.Elapsed);
		}

		private static async Task CreateAndRunProcessingSetAsync()
		{
			Console2.WriteStartHeader("Processing");
			Stopwatch stopwatch = new Stopwatch();
			stopwatch.Start();

			ProcessingHelper processingHelper = new ProcessingHelper(_connectionManager.ProcessingProfileManager, _connectionManager.ProcessingCustodianManager, _connectionManager.ProcessingSetManager, _connectionManager.ProcessingDataSourceManager, _connectionManager.ProcessingJobManager, _connectionManager.RsapiClient);

			int timeZoneArtifactId = await processingHelper.GetTimeZoneArtifactIdAsync(_workspaceArtifactId);
			int destinationFolderArtifactId = await processingHelper.GetDefaultFolderArtifactIdAsync(_workspaceArtifactId);

			// Create Processing Profile
			int processingProfileArtifactId = await processingHelper.CreateProcessingProfileAsync(timeZoneArtifactId, destinationFolderArtifactId, _workspaceArtifactId);

			// Create Custodians
			int custodianArtifactId = await processingHelper.CreateCustodianAsync(_workspaceArtifactId);

			// Create Processing Sets
			int processingSetArtifactId = await processingHelper.CreateProcessingSetAsync(processingProfileArtifactId, _workspaceArtifactId);

			// Create Processing Data Source
			await processingHelper.CreateProcessingDataSourceAsync(processingSetArtifactId, custodianArtifactId, timeZoneArtifactId, destinationFolderArtifactId, _workspaceArtifactId);

			// Inventory Job
			await processingHelper.StartInventoryJobsAsync(processingSetArtifactId, _workspaceArtifactId);
			await processingHelper.WaitForInventoryJobToCompleteAsync(_workspaceArtifactId, processingSetArtifactId);

			// Discovery Job
			await processingHelper.StartDiscoveryJobAsync(processingSetArtifactId, _workspaceArtifactId);
			await processingHelper.WaitForDiscoveryJobToCompleteAsync(_workspaceArtifactId, processingSetArtifactId);

			// Publish Job
			await processingHelper.StartPublishJobAsync(processingSetArtifactId, _workspaceArtifactId);
			await processingHelper.WaitForPublishJobToCompleteAsync(_workspaceArtifactId, processingSetArtifactId);

			stopwatch.Stop();
			DisplayTimeElapsed(stopwatch.Elapsed);
		}

		private static async Task CreateAndBuildDtSearch()
		{
			Console2.WriteStartHeader("Create Search");
			Stopwatch stopwatch = new Stopwatch();
			stopwatch.Start();

			SearchHelper searchHelper = new SearchHelper(_connectionManager.KeywordSearchManager, _connectionManager.DtSearchManager, _connectionManager.DtSearchIndexManager, _connectionManager.RsapiClient);

			// Create Keyword Search
			int keywordSearchArtifactId = await searchHelper.CreateKeywordSearchAsync(_workspaceArtifactId);

			// Create dtSearch Index 
			int dtSearchIndexArtifactId = await searchHelper.CreateDtSearchIndexAsync(_workspaceArtifactId, keywordSearchArtifactId);

			// Build dtSearch index
			await searchHelper.BuildDtSearchIndexAsync(_workspaceArtifactId, dtSearchIndexArtifactId);

			// Create dtSearch
			_dtSearchArtifactId = await searchHelper.CreateDtSearchAsync(_workspaceArtifactId, Constants.Workspace.EXTRACTED_TEXT_FIELD_NAME);

			stopwatch.Stop();
			DisplayTimeElapsed(stopwatch.Elapsed);
		}

		private static async Task TagDocumentsAsResponsiveAsync()
		{
			Console2.WriteStartHeader("Tag Documents");
			Stopwatch stopwatch = new Stopwatch();
			stopwatch.Start();

			SearchHelper searchHelper = new SearchHelper(_connectionManager.KeywordSearchManager, _connectionManager.DtSearchManager, _connectionManager.DtSearchIndexManager, _connectionManager.RsapiClient);

			// Query / execute search in order to store the document IDs returned
			List<int> documentsToTag = await searchHelper.GetDocumentsToTagAsync(_dtSearchArtifactId, _workspaceArtifactId);

			// Tag the documents responsive
			ReviewHelper reviewHelper = new ReviewHelper(_connectionManager.RsapiClient);
			await reviewHelper.TagDocumentsResponsiveAsync(_workspaceArtifactId, documentsToTag);

			stopwatch.Stop();
			DisplayTimeElapsed(stopwatch.Elapsed);
		}

		private static async Task CreateAndRunImagingSetAsync()
		{
			Console2.WriteStartHeader("Imaging");
			Stopwatch stopwatch = new Stopwatch();
			stopwatch.Start();

			SearchHelper searchHelper = new SearchHelper(_connectionManager.KeywordSearchManager, _connectionManager.DtSearchManager, _connectionManager.DtSearchIndexManager, _connectionManager.RsapiClient);

			//	Perform another search for documents where responsive = yes
			int searchForProduction = await searchHelper.CreateDtSearchAsync(_workspaceArtifactId, Constants.Workspace.ResponsiveField.Name);

			ImagingHelper imagingHelper = new ImagingHelper(_connectionManager.ImagingProfileManager, _connectionManager.ImagingSetManager, _connectionManager.ImagingJobManager, _connectionManager.RsapiClient);

			// Create Imaging Profile and Set
			int imagingProfileArtifactId = await imagingHelper.CreateImagingProfileAsync(_workspaceArtifactId);
			int imagingSetArtifactId = await imagingHelper.CreateImagingSetAsync(_workspaceArtifactId, searchForProduction, imagingProfileArtifactId);

			// Run Imaging Job
			await imagingHelper.RunImagingJobAsync(_workspaceArtifactId, imagingSetArtifactId);
			await imagingHelper.WaitForImagingJobToCompleteAsync(_workspaceArtifactId, imagingSetArtifactId);

			stopwatch.Stop();
			DisplayTimeElapsed(stopwatch.Elapsed);
		}

		private static async Task CreateAndRunProductionAsync()
		{
			Console2.WriteStartHeader("Production");
			Stopwatch stopwatch = new Stopwatch();
			stopwatch.Start();

			ProductionHelper productionHelper = new ProductionHelper(_connectionManager.ProductionManager, _connectionManager.ProductionDataSourceManager);

			// Create the production
			_productionArtifactId = await productionHelper.CreatePageLevelProductionAsync(_workspaceArtifactId);

			// Add a data source
			await productionHelper.CreateDataSourceAsync(_workspaceArtifactId, _productionArtifactId, _dtSearchArtifactId);

			// Stage the production
			await productionHelper.StageProductionAsync(_workspaceArtifactId, _productionArtifactId);

			// Run the production
			await productionHelper.RunProductionAsync(_workspaceArtifactId, _productionArtifactId);

			// Read the production to confirm that it has completed
			await productionHelper.WaitForProductionJobToCompleteAsync(_workspaceArtifactId, _productionArtifactId);

			stopwatch.Stop();
			DisplayTimeElapsed(stopwatch.Elapsed);
		}

		private static async Task DownloadProductionAsync()
		{
			Console2.WriteStartHeader("Export Production");
			Stopwatch stopwatch = new Stopwatch();
			stopwatch.Start();

			// Download the Production
			await ExportProductionHelper.DownloadProductionAsync(_workspaceArtifactId, _productionArtifactId);

			stopwatch.Stop();
			DisplayTimeElapsed(stopwatch.Elapsed);
		}

		private static async Task DeleteWorkspaceAsync()
		{
			Console2.WriteStartHeader("Delete Workspace");
			Stopwatch stopwatch = new Stopwatch();
			stopwatch.Start();

			Console.WriteLine("Would you like to delete the created workspace (type 'y' or 'n'):");
			string response = Console.ReadLine();
			if (response == null || response.ToLower().Equals("y"))
			{
				WorkspaceHelper workspaceHelper = new WorkspaceHelper(_connectionManager.RsapiClient);

				// Delete workspace
				await workspaceHelper.DeleteWorkspaceAsync(_workspaceArtifactId);
			}

			stopwatch.Stop();
			DisplayTimeElapsed(stopwatch.Elapsed);
		}

		private static void DisplayTimeElapsed(TimeSpan stopwatchElapsed, string prefix = null)
		{
			Console2.WriteDisplayStartLine($"{prefix ?? string.Empty}Time elapsed: {stopwatchElapsed:hh\\:mm\\:ss}");
			Console2.WriteDisplayEmptyLine();
		}
	}
}
