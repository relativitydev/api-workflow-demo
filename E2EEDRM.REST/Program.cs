using E2EEDRM.Helpers;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Http;
using System.Threading.Tasks;

namespace E2EEDRM.REST
{
	public class Program
	{
		private static int _dtSearchIndexId;
		private static int _dtSearchArtifactId;
		private static int _workspaceArtifactId;
		private static int _productionArtifactId;
		private static HttpClient _httpClient;
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
				_httpClient = RESTConnectionManager.GetHttpClient();
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

		private static async Task CreateWorkspaceAsync()
		{
			Console2.WriteStartHeader("Create Workspace");
			Stopwatch stopwatch = new Stopwatch();
			stopwatch.Start();

			WorkspaceHelper workspaceHelper = new WorkspaceHelper(_connectionManager.RsapiClient);

			_workspaceArtifactId = await workspaceHelper.QueryTemplateAndCreateWorkspaceAsync();

			// Create the Responsive - E2E Field
			await RESTSearchHelper.CreateResponsiveFieldAsync(_httpClient, _workspaceArtifactId);

			stopwatch.Stop();
			DisplayTimeElapsed(stopwatch.Elapsed);
		}

		private static async Task TransferDocumentsAsync()
		{
			Console2.WriteStartHeader("Transfer Documents");
			Stopwatch stopwatch = new Stopwatch();
			stopwatch.Start();

			//Using Azure Blob .NET API
			//BlobHelper blobHelper = new BlobHelper();
			//await blobHelper.UploadDocumentsToBlobAsync();

			//Using Azure Blob REST API
			RESTBlobHelper restBlobHelper = new RESTBlobHelper();
			await restBlobHelper.UploadDocumentsToBlobAsync();

			stopwatch.Stop();
			DisplayTimeElapsed(stopwatch.Elapsed);
		}

		private static async Task CreateAndRunProcessingSetAsync()
		{
			Console2.WriteStartHeader("Processing");
			Stopwatch stopwatch = new Stopwatch();
			stopwatch.Start();

			int timeZoneArtifactId = await RESTProcessingHelper.GetTimeZoneArtifactIdAsync(_httpClient, _workspaceArtifactId);
			int destinationFolderArtifactId = await RESTProcessingHelper.GetDefaultFolderArtifactIdAsync(_httpClient, _workspaceArtifactId);

			// Create Processing Profile
			int processingProfileId = await RESTProcessingHelper.CreateProcessingProfileAsync(_httpClient, timeZoneArtifactId, destinationFolderArtifactId, _workspaceArtifactId);

			// Create Custodians
			int custodianId = await RESTProcessingHelper.CreateCustodianAsync(_httpClient, _workspaceArtifactId);

			// Create Processing Sets
			int processingSetId = await RESTProcessingHelper.CreateProcessingSetAsync(_httpClient, processingProfileId, _workspaceArtifactId);

			// Create Processing Data Source
			await RESTProcessingHelper.CreateProcessingDataSourceAsync(_httpClient, processingSetId, custodianId, timeZoneArtifactId, destinationFolderArtifactId, _workspaceArtifactId);

			// Inventory Job
			await RESTProcessingHelper.InventoryJobAsync(_httpClient, processingSetId, _workspaceArtifactId);
			await RESTProcessingHelper.WaitForInventoryJobToCompleteAsync(_httpClient, _workspaceArtifactId, processingSetId);

			// Discovery Job
			await RESTProcessingHelper.DiscoveryJobAsync(_httpClient, processingSetId, _workspaceArtifactId);
			await RESTProcessingHelper.WaitForDiscoveryJobToCompleteAsync(_httpClient, _workspaceArtifactId, processingSetId);

			// Publish Job
			await RESTProcessingHelper.PublishJobAsync(_httpClient, processingSetId, _workspaceArtifactId);
			await RESTProcessingHelper.WaitForPublishJobToCompleteAsync(_httpClient, _workspaceArtifactId, processingSetId);

			stopwatch.Stop();
			DisplayTimeElapsed(stopwatch.Elapsed);
		}

		private static async Task CreateAndBuildDtSearch()
		{
			Console2.WriteStartHeader("Create Search");
			Stopwatch stopwatch = new Stopwatch();
			stopwatch.Start();

			// Create Keyword Search
			int seedSearchId = await RESTSearchHelper.CreateKeywordSearchAsync(_httpClient, _workspaceArtifactId);

			// Create dtSearch Index
			_dtSearchIndexId = await RESTSearchHelper.CreateDtSearchIndexAsync(_httpClient, _workspaceArtifactId, seedSearchId);

			// Build dtSearch Index
			await RESTSearchHelper.BuildDtSearchIndexAsync(_httpClient, _workspaceArtifactId, _dtSearchIndexId);

			// Create dtSearch
			_dtSearchArtifactId = await RESTSearchHelper.CreateDtSearchAsync(_httpClient, _workspaceArtifactId, _dtSearchIndexId, Constants.Workspace.EXTRACTED_TEXT_FIELD_NAME);

			stopwatch.Stop();
			DisplayTimeElapsed(stopwatch.Elapsed);
		}

		private static async Task TagDocumentsAsResponsiveAsync()
		{
			Console2.WriteStartHeader("Tag Documents");
			Stopwatch stopwatch = new Stopwatch();
			stopwatch.Start();

			// Query / execute search in order to store the document IDs returned
			List<int> DocstoTag = await RESTReviewHelper.GetDocumentsToTagAsync(_httpClient, _dtSearchArtifactId, _workspaceArtifactId);

			// Tag the documents responsive
			await RESTReviewHelper.TagDocumentsAsync(_httpClient, _workspaceArtifactId, DocstoTag);

			stopwatch.Stop();
			DisplayTimeElapsed(stopwatch.Elapsed);
		}

		private static async Task CreateAndRunImagingSetAsync()
		{
			Console2.WriteStartHeader("Imaging");
			Stopwatch stopwatch = new Stopwatch();
			stopwatch.Start();

			//	Perform another search for documents where responsive = yes
			int savedSearchId = await RESTSearchHelper.CreateDtSearchAsync(_httpClient, _workspaceArtifactId, _dtSearchIndexId, Constants.Workspace.ResponsiveField.Name);

			// Create Imaging Profile and Set
			int imagingProfileId = await RESTImagingHelper.CreateImagingProfileAsync(_httpClient, _workspaceArtifactId);
			int imagingSetId = await RESTImagingHelper.CreateImagingSetAsync(_httpClient, savedSearchId, imagingProfileId, _workspaceArtifactId);

			// Run Imaging Job
			await RESTImagingHelper.RunImagingJobAsync(_httpClient, imagingSetId, _workspaceArtifactId);
			await RESTImagingHelper.WaitForImagingJobToCompleteAsync(_httpClient, _workspaceArtifactId, imagingSetId);

			stopwatch.Stop();
			DisplayTimeElapsed(stopwatch.Elapsed);
		}

		private static async Task CreateAndRunProductionAsync()
		{
			Console2.WriteStartHeader("Production");
			Stopwatch stopwatch = new Stopwatch();
			stopwatch.Start();

			// Create the Production
			_productionArtifactId = await RESTProductionHelper.CreateProductionAsync(_httpClient, _workspaceArtifactId);

			// Add the production Data Source
			await RESTProductionHelper.AddProductionDataSourceAsync(_httpClient, _productionArtifactId, _workspaceArtifactId, _dtSearchArtifactId);

			// Stage the Production
			await RESTProductionHelper.StageProductionAsync(_httpClient, _productionArtifactId, _workspaceArtifactId);
			await RESTProductionHelper.CheckProductionStatusForStagingAsync(_httpClient, _productionArtifactId, _workspaceArtifactId);

			// Run the Production
			await RESTProductionHelper.RunProductionAsync(_httpClient, _productionArtifactId, _workspaceArtifactId);
			await RESTProductionHelper.CheckProductionStatusAsync(_httpClient, _productionArtifactId, _workspaceArtifactId);

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
