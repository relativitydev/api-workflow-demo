using E2EEDRM.Helpers;
using kCura.Relativity.Client;
using kCura.Relativity.Client.DTOs;
using Relativity.Processing.Services;
using System;
using System.Threading;
using System.Threading.Tasks;
using Constants = E2EEDRM.Helpers.Constants;

namespace E2EEDRM
{
	public class ProcessingHelper
	{
		private IProcessingProfileManager ProcessingProfileManager { get; }
		private IProcessingCustodianManager ProcessingCustodianManager { get; }
		private IProcessingSetManager ProcessingSetManager { get; }
		private IProcessingDataSourceManager ProcessingDataSourceManager { get; }
		private IProcessingJobManager ProcessingJobManager { get; }
		private IRSAPIClient RsapiClient { get; }

		public ProcessingHelper(IProcessingProfileManager processingProfileManager, IProcessingCustodianManager processingCustodianManager, IProcessingSetManager processingSetManager, IProcessingDataSourceManager processingDataSourceManager, IProcessingJobManager processingJobManager, IRSAPIClient rsapiClient)
		{
			ProcessingProfileManager = processingProfileManager;
			ProcessingCustodianManager = processingCustodianManager;
			ProcessingSetManager = processingSetManager;
			ProcessingDataSourceManager = processingDataSourceManager;
			ProcessingJobManager = processingJobManager;
			RsapiClient = rsapiClient;
		}

		//This should cover profile, custodians, processing sets, inventory job, discovery, publish
		public async Task<int> CreateProcessingProfileAsync(int timeZoneArtifactId, int destinationFolderArtifactId, int workspaceArtifactId)
		{
			try
			{
				Console2.WriteDisplayStartLine($"Creating Processing Profile [Name: {Constants.Processing.Profile.NAME}]");

				// Build Basic Processing Profile
				ProcessingProfile processingProfile = BuildBasicProfile(timeZoneArtifactId, destinationFolderArtifactId);

				//Build the ProcessingProfileSaveRequest
				ProcessingProfileSaveRequest processingProfileSaveRequest = new ProcessingProfileSaveRequest
				{
					ProcessingProfile = processingProfile,
					WorkspaceId = workspaceArtifactId
				};

				//Create the ProcessingProfile object. The service returns a ProcessingProfileSaveResponse object.
				ProcessingProfileSaveResponse saveResponse = await ProcessingProfileManager.SaveAsync(processingProfileSaveRequest);
				int processingProfileArtifactId = saveResponse.ProcessingProfileId;
				if (processingProfileArtifactId == 0)
				{
					throw new Exception("Failed to Create Processing Profile");
				}

				Console2.WriteDebugLine($"Processing Profile ArtifactId: {processingProfileArtifactId}");
				Console2.WriteDisplayEndLine("Created Processing Profile!");

				return processingProfileArtifactId;
			}
			catch (Exception ex)
			{
				// Error Creating Processing Profile
				throw new Exception("An error occured when creating Processing Profile", ex);
			}
		}

		public async Task<int> CreateCustodianAsync(int workspaceArtifactId)
		{
			try
			{
				Console2.WriteDisplayStartLine($"Creating Custodian [FirstName: {Constants.Processing.Custodian.FIRST_NAME}, LastName: {Constants.Processing.Custodian.LAST_NAME}]");

				//Build the ProcessingCustodian object.
				ProcessingCustodian processingCustodian = new ProcessingCustodian
				{
					ArtifactID = Constants.Processing.Custodian.ARTIFACT_ID,
					DocumentNumberingPrefix = Constants.Processing.Custodian.DOCUMENT_NUMBERING_PREFIX,
					FirstName = Constants.Processing.Custodian.FIRST_NAME,
					LastName = Constants.Processing.Custodian.LAST_NAME
				};

				int custodianArtifactId = await ProcessingCustodianManager.SaveAsync(processingCustodian, workspaceArtifactId);
				if (custodianArtifactId == 0)
				{
					throw new Exception("Failed to Create Custodian");
				}

				Console2.WriteDebugLine($"Custodian ArtifactId: {custodianArtifactId}");
				Console2.WriteDisplayEndLine("Created Custodian!");

				return custodianArtifactId;
			}
			catch (Exception ex)
			{
				throw new Exception("An error occured when creating Custodian", ex);
			}
		}

		public async Task<int> CreateProcessingSetAsync(int processingProfileArtifactId, int workspaceArtifactId)
		{
			try
			{
				Console2.WriteDisplayStartLine($"Creating Processing Set [Name: {Constants.Processing.Set.NAME}]");

				//Build the ProcessingSet object.
				ProcessingSet processingSet = new ProcessingSet
				{
					ArtifactID = Constants.Processing.Set.ARTIFACT_ID,
					Name = Constants.Processing.Set.NAME,
					Profile = new ProcessingProfileRef(processingProfileArtifactId)
				};

				if (Constants.Processing.Set.EmailNotificationRecipients.Length > 0)
				{
					processingSet.EmailNotificationRecipients = Constants.Processing.Set.EmailNotificationRecipients;
				}

				//Create the ProcessingSet object. The service returns the Artifact ID of the object.
				int processingSetArtifactId = await ProcessingSetManager.SaveAsync(processingSet, workspaceArtifactId);
				if (processingSetArtifactId == 0)
				{
					throw new Exception("Failed to Create Processing Set");
				}

				Console2.WriteDebugLine($"Processing Set ArtifactId: {processingSetArtifactId}");
				Console2.WriteDisplayEndLine("Created Processing Set!");

				return processingSetArtifactId;
			}
			catch (Exception ex)
			{
				throw new Exception("An error occured when creating Processing Set", ex);
			}
		}

		public async Task CreateProcessingDataSourceAsync(int processingSetArtifactId, int custodianArtifactId, int timeZoneArtifactId, int destinationFolderArtifactId, int workspaceArtifactId)
		{
			try
			{
				Console2.WriteDisplayStartLine($"Creating Processing Data Source [Name: {Constants.Processing.DataSource.NAME}]");

				//Build the processing ProcessingDataSource object.
				ProcessingDataSource processingDataSource = new ProcessingDataSource
				{
					ArtifactID = Constants.Processing.DataSource.ARTIFACT_ID,
					ProcessingSet = new ProcessingSetRef
					{
						ArtifactID = processingSetArtifactId
					},
					Custodian = custodianArtifactId,
					DestinationFolder = destinationFolderArtifactId,
					DocumentNumberingPrefix = Constants.Processing.DataSource.DOCUMENT_NUMBERING_PREFIX,
					InputPath = Constants.Processing.DataSource.INPUT_PATH,
					Name = Constants.Processing.DataSource.NAME,
					OcrLanguages = Constants.Processing.DataSource.OcrLanguages,
					Order = Constants.Processing.DataSource.ORDER,
					TimeZone = timeZoneArtifactId,
					StartNumber = Constants.Processing.DataSource.START_NUMBER,
					IsStartNumberVisible = Constants.Processing.DataSource.IS_START_NUMBER_VISIBLE
				};

				//Create the ProcessingDataSource object. The service returns the Artifact ID for the object.
				int processingDataSourceArtifactId = await ProcessingDataSourceManager.SaveAsync(processingDataSource, workspaceArtifactId);
				if (processingDataSourceArtifactId == 0)
				{
					throw new Exception("Failed to Create Processing Data Source");
				}

				Console2.WriteDebugLine($"Processing Data Source ArtifactId: {processingDataSourceArtifactId}");
				Console2.WriteDisplayEndLine("Created Processing Data Source!");
			}
			catch (Exception ex)
			{
				throw new Exception("An error occured when creating Processing Data Source", ex);
			}
		}

		public async Task StartInventoryJobsAsync(int processingSetArtifactId, int workspaceArtifactId)
		{
			try
			{
				Console2.WriteDisplayStartLine("Starting Inventory Job");

				//Create an inventory job object.
				InventoryJob inventoryJob = new InventoryJob
				{
					ProcessingSetId = processingSetArtifactId,
					WorkspaceArtifactId = workspaceArtifactId
				};

				//Submit the job for inventory.
				await ProcessingJobManager.SubmitInventoryJobsAsync(inventoryJob);

				Console2.WriteDisplayEndLine("Started Inventory Job!");
			}
			catch (Exception ex)
			{
				throw new Exception("An error occured when staring Processing Inventory Job", ex);
			}
		}

		public async Task StartDiscoveryJobAsync(int processingSetArtifactId, int workspaceArtifactId)
		{
			try
			{
				Console2.WriteDisplayStartLine("Starting Discovery Job");

				// Create a discovery job object.
				DiscoveryJob discoveryJob = new DiscoveryJob
				{
					ProcessingSetId = processingSetArtifactId,
					WorkspaceArtifactId = workspaceArtifactId
				};

				// Submit the job for discovery.
				await ProcessingJobManager.SubmitDiscoveryJobsAsync(discoveryJob);

				Console2.WriteDisplayEndLine("Started Discovery Job!");
			}
			catch (Exception ex)
			{
				throw new Exception("An error occured when staring Processing Discovery Job", ex);
			}
		}

		public async Task StartPublishJobAsync(int processingSetArtifactId, int workspaceArtifactId)
		{
			try
			{
				Console2.WriteDisplayStartLine("Starting Publish Job");

				// Create a publish job object.
				PublishJob publishJob = new PublishJob
				{
					ProcessingSetId = processingSetArtifactId,
					WorkspaceArtifactId = workspaceArtifactId
				};

				// Submit the job for publish.
				await ProcessingJobManager.SubmitPublishJobsAsync(publishJob);

				Console2.WriteDisplayEndLine("Started Publish Job!");
			}
			catch (Exception ex)
			{
				throw new Exception("An error occured when staring Processing Publish Job", ex);
			}
		}

		#region Helpers

		public async Task<int> GetTimeZoneArtifactIdAsync(int workspaceArtifactId)
		{
			try
			{
				RsapiClient.APIOptions.WorkspaceID = workspaceArtifactId;

				Query<RDO> rdoQuery = new Query<RDO>
				{
					ArtifactTypeGuid = Constants.Guids.ObjectType.RelativityTimeZone,
					Fields = FieldValue.NoFields
				};
				QueryResultSet<RDO> rdoQueryResultSet = await Task.Run(() => RsapiClient.Repositories.RDO.Query(rdoQuery));
				if (!rdoQueryResultSet.Success)
				{
					throw new Exception(rdoQueryResultSet.Message);
				}
				return rdoQueryResultSet.Results[0].Artifact.ArtifactID;
			}
			catch (Exception ex)
			{
				throw new Exception($@"Error getting Time Zone ArtifactID {ex}", ex);
			}
		}

		public async Task<int> GetDefaultFolderArtifactIdAsync(int workspaceArtifactId)
		{
			try
			{
				RsapiClient.APIOptions.WorkspaceID = workspaceArtifactId;
				Query<RDO> rdoQuery = new Query<RDO>()
				{
					ArtifactTypeID = (int?)ArtifactType.Folder,
					Fields = FieldValue.NoFields
				};
				QueryResultSet<RDO> rdoQueryResultSet = await Task.Run(() => RsapiClient.Repositories.RDO.Query(rdoQuery));
				if (!rdoQueryResultSet.Success)
				{
					throw new Exception(rdoQueryResultSet.Message);
				}
				return rdoQueryResultSet.Results[0].Artifact.ArtifactID;
			}
			catch (Exception ex)
			{
				throw new Exception($@"Unable to get the default folder artifactID: {ex}", ex);
			}
		}

		public ProcessingProfile BuildBasicProfile(int timeZoneArtifactId, int folderArtifactId)
		{
			ProcessingProfile profile = new ProcessingProfile
			{
				Name = Constants.Processing.Profile.NAME,
				NumberingSettings =
					new NumberingSettings
					{
						DefaultDocumentNumberingPrefix = Constants.Processing.Profile.DEFAULT_DOCUMENT_NUMBERING_PREFIX,
						NumberOfDigits = Constants.Processing.Profile.NUMBER_OF_DIGITS4,
						NumberingType = Constants.Processing.Profile.NUMBERING_TYPE,
						ParentChildNumbering = Constants.Processing.Profile.PARENT_CHILD_NUMBERING,
					},
				DeduplicationSettings = new DeduplicationSettings
				{
					DeduplicationMethod = Constants.Processing.Profile.DE_DUPLICATION_METHOD,
					PropagateDeduplicationData = Constants.Processing.Profile.PROPAGATE_DE_DUPLICATION_DATA
				},
				ExtractionSettings =
					new ExtractionSettings
					{
						Extractchildren = Constants.Processing.Profile.EXTRACT_CHILDREN,
						EmailOutput = Constants.Processing.Profile.EMAIL_OUTPUT,
						ExcelTextExtractionMethod = Constants.Processing.Profile.EXCEL_TEXT_EXTRACTION_METHOD,
						ExcelHeaderFooterExtraction = Constants.Processing.Profile.EXCEL_HEADER_FOOTER_EXTRACTION,
						PowerPointTextExtractionMethod = Constants.Processing.Profile.POWER_POINT_TEXT_EXTRACTION_METHOD,
						WordTextExtractionMethod = Constants.Processing.Profile.WORD_TEXT_EXTRACTION_METHOD,
						OCR = Constants.Processing.Profile.OCR,
						OCRAccuracy = Constants.Processing.Profile.OCR_ACCURACY,
						OCRTextSeparator = Constants.Processing.Profile.OCR_TEXT_SEPARATOR
					},
				InventoryDiscoverSettings =
					new InventoryDiscoverSettings
					{
						DeNIST = Constants.Processing.Profile.DE_NIST,
						DefaultTimeZoneID = timeZoneArtifactId,
						DefaultOCRLanguages = Constants.Processing.Profile.DefaultOcrLanguages
					},
				PublishSettings = new PublishSettings
				{
					AutoPublishSet = Constants.Processing.Profile.AUTO_PUBLISH_SET,
					DefaultDestinationFolder = new Relativity.Services.Folder.FolderRef
					{
						ArtifactID = folderArtifactId
					},
					UseSourceFolderStructure = Constants.Processing.Profile.USE_SOURCE_FOLDER_STRUCTURE
				}
			};

			return profile;
		}

		public async Task WaitForInventoryJobToCompleteAsync(int workspaceArtifactId, int processingSetArtifactId)
		{
			RsapiClient.APIOptions.WorkspaceID = workspaceArtifactId;
			Console2.WriteDisplayStartLine("Waiting for Inventory Job to finish...");
			bool inventoryComplete = await IsJobCompletedSuccessfullyAsync(workspaceArtifactId, processingSetArtifactId, Constants.ProcessingJobType.Inventory);
			if (!inventoryComplete)
			{
				throw new Exception("Inventory Job failed to Complete.");
			}
			Console2.WriteDisplayEndLine("Inventory Job Complete!");
		}

		public async Task WaitForDiscoveryJobToCompleteAsync(int workspaceArtifactId, int processingSetArtifactId)
		{
			RsapiClient.APIOptions.WorkspaceID = workspaceArtifactId;
			Console2.WriteDisplayStartLine("Waiting for Discovery Job to finish...");
			bool discoveryComplete = await IsJobCompletedSuccessfullyAsync(workspaceArtifactId, processingSetArtifactId, Constants.ProcessingJobType.Discover);
			if (!discoveryComplete)
			{
				throw new Exception("Discovery Job failed to Complete.");
			}
			Console2.WriteDisplayEndLine("Discovery Job Complete!");
		}

		public async Task WaitForPublishJobToCompleteAsync(int workspaceArtifactId, int processingSetArtifactId)
		{
			RsapiClient.APIOptions.WorkspaceID = workspaceArtifactId;
			Console2.WriteDisplayStartLine("Waiting for Publish Job to finish...");
			bool publishComplete = await IsJobCompletedSuccessfullyAsync(workspaceArtifactId, processingSetArtifactId, Constants.ProcessingJobType.Publish);
			if (!publishComplete)
			{
				throw new Exception("Publish Job failed to Complete.");
			}
			Console2.WriteDisplayEndLine("Publish Job Complete!");
		}

		public async Task<bool> IsJobCompletedSuccessfullyAsync(int workspaceArtifactId, int processingSetArtifactId, Constants.ProcessingJobType jobType)
		{
			bool jobComplete = false;
			const int maxTimeInMilliseconds = (Constants.Waiting.MAX_WAIT_TIME_IN_MINUTES * 60 * 1000);
			const int sleepTimeInMilliSeconds = Constants.Waiting.SLEEP_TIME_IN_SECONDS * 1000;
			int currentWaitTimeInMilliseconds = 0;

			Guid fieldGuid;
			switch (jobType)
			{
				case Constants.ProcessingJobType.Inventory:
					fieldGuid = Constants.Guids.Fields.ProcessingSet.InventoryStatus;
					break;
				case Constants.ProcessingJobType.Discover:
					fieldGuid = Constants.Guids.Fields.ProcessingSet.DiscoverStatus;
					break;
				default:
					fieldGuid = Constants.Guids.Fields.ProcessingSet.PublishStatus;
					break;
			}

			try
			{
				while (currentWaitTimeInMilliseconds < maxTimeInMilliseconds && jobComplete == false)
				{
					Thread.Sleep(sleepTimeInMilliSeconds);

					RDO job = await Task.Run(() => RsapiClient.Repositories.RDO.ReadSingle(processingSetArtifactId));
					jobComplete = job[fieldGuid].ValueAsSingleChoice.Name.Contains("Completed");

					currentWaitTimeInMilliseconds += sleepTimeInMilliSeconds;
				}

				return jobComplete;
			}
			catch (Exception ex)
			{
				throw new Exception($@"Error Checking for {jobType.ToString()} Job Completion: {ex}", ex);
			}
		}

		#endregion
	}
}
