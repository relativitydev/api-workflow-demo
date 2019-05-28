using E2EEDRM.Helpers;
using E2EEDRM.REST.Models.Processing;
using E2EEDRM.REST.Models.RDO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Object = E2EEDRM.REST.Models.RDO.Object;

namespace E2EEDRM.REST
{
	public class RESTProcessingHelper
	{
		public static async Task<int> CreateProcessingProfileAsync(HttpClient httpClient, int timeZoneArtifactId, int destinationFolderArtifactId, int workspaceId)
		{
			try
			{
				string url = "Relativity.REST/api/Relativity.Processing.Services.IProcessingModule/Processing Profile Manager/SaveAsync";
				ProcessingProfileSaveRequestStructure processingProfileSaveRequestStructure = new ProcessingProfileSaveRequestStructure
				{
					ProcessingProfileSaveRequest = new ProcessingProfileSaveRequest
					{
						ProcessingProfile = new ProcessingProfile
						{
							Name = Constants.Processing.Profile.NAME,
							NumberingSettings = new NumberingSettings
							{
								DefaultDocumentNumberingPrefix = Constants.Processing.Profile.DEFAULT_DOCUMENT_NUMBERING_PREFIX,
								NumberofDigits = Constants.Processing.Profile.NUMBER_OF_DIGITS4_AS_STRING,
								NumberingType = Constants.Processing.Profile.NUMBERING_TYPE_AS_STRING,
								ParentChildNumbering = Constants.Processing.Profile.PARENT_CHILD_NUMBERING_AS_STRING,
								Delimiter = Constants.Processing.Profile.DELIMITER,
							},
							DeduplicationSettings = new DeduplicationSettings
							{
								DeduplicationMethod = Constants.Processing.Profile.DE_DUPLICATION_METHOD_AS_STRING,
								PropagateDeduplicationData = Constants.Processing.Profile.PROPAGATE_DE_DUPLICATION_DATA
							},
							ExtractionSettings = new ExtractionSettings
							{
								Extractchildren = Constants.Processing.Profile.EXTRACT_CHILDREN,
								EmailOutput = Constants.Processing.Profile.EMAIL_OUTPUT_AS_STRING,
								ExcelTextExtractionMethod = Constants.Processing.Profile.EXCEL_TEXT_EXTRACTION_METHOD_AS_STRING,
								ExcelHeaderFooterExtraction = Constants.Processing.Profile.EXCEL_HEADER_FOOTER_EXTRACTION_AS_STRING,
								PowerPointTextExtractionMethod = Constants.Processing.Profile.POWER_POINT_TEXT_EXTRACTION_METHOD_AS_STRING,
								WordTextExtractionMethod = Constants.Processing.Profile.WORD_TEXT_EXTRACTION_METHOD_AS_STRING,
								OCR = Constants.Processing.Profile.OCR,
								OCRAccuracy = Constants.Processing.Profile.OCR_ACCURACY_AS_STRING,
								OCRTextSeparator = Constants.Processing.Profile.OCR_TEXT_SEPARATOR
							},
							InventoryDiscoverSettings = new InventoryDiscoverSettings
							{
								DeNIST = Constants.Processing.Profile.DE_NIST,
								DefaultTimeZoneID = timeZoneArtifactId,
								DefaultOCRlanguages = Constants.Processing.Profile.DEFAULT_OCR_LANGUAGES
							},
							PublishSettings = new PublishSettings
							{
								AutopublishSet = Constants.Processing.Profile.AUTO_PUBLISH_SET,
								DefaultDestinationFolder = new DefaultDestinationFolder
								{
									ArtifactID = destinationFolderArtifactId
								},
								UseSourceFolderStructure = Constants.Processing.Profile.USE_SOURCE_FOLDER_STRUCTURE
							}
						},
						WorkspaceId = workspaceId
					}
				};
				string request = JsonConvert.SerializeObject(processingProfileSaveRequestStructure);

				Console2.WriteDisplayStartLine($"Creating Processing Profile [Name: {Constants.Processing.Profile.NAME}]");
				HttpResponseMessage response = RESTConnectionManager.MakePost(httpClient, url, request);
				string result = await response.Content.ReadAsStringAsync();
				bool success = HttpStatusCode.OK == response.StatusCode;
				if (!success)
				{
					throw new Exception("Failed to Create Processing Profile");
				}

				JObject resultObject = JObject.Parse(result);
				int processingProfileArtifactId = resultObject["ProcessingProfileId"].Value<int>();
				Console2.WriteDebugLine($"Processing Profile ArtifactId: {processingProfileArtifactId}");
				Console2.WriteDisplayEndLine("Created Processing Profile!");

				return processingProfileArtifactId;
			}
			catch (Exception ex)
			{
				throw new Exception("An error occured when creating Processing Profile", ex);
			}
		}

		public static async Task<int> CreateCustodianAsync(HttpClient httpClient, int workspaceId)
		{
			try
			{
				string url = "Relativity.REST/api/Relativity.Processing.Services.IProcessingModule/Processing Custodian Manager/SaveAsync";
				CustodianSaveRequest custodianSaveRequest = new CustodianSaveRequest
				{
					Custodian = new Custodian
					{
						DocumentNumberingPrefix = Constants.Processing.Custodian.DOCUMENT_NUMBERING_PREFIX,
						FirstName = Constants.Processing.Custodian.FIRST_NAME,
						LastName = Constants.Processing.Custodian.LAST_NAME,
						CustodianType = "Person"
					},
					workspaceArtifactId = workspaceId
				};
				string request = JsonConvert.SerializeObject(custodianSaveRequest);

				Console2.WriteDisplayStartLine($"Creating Custodian [FirstName: {Constants.Processing.Custodian.FIRST_NAME}, LastName: {Constants.Processing.Custodian.LAST_NAME}]");
				HttpResponseMessage response = RESTConnectionManager.MakePost(httpClient, url, request);
				string result = await response.Content.ReadAsStringAsync();
				bool success = HttpStatusCode.OK == response.StatusCode;
				if (!success)
				{
					throw new Exception("Failed to Create Custodian");
				}

				int custodianArtifactId = Int32.Parse(result);
				Console2.WriteDebugLine($"Custodian ArtifactId: {custodianArtifactId}");
				Console2.WriteDisplayEndLine("Created Custodian!");

				return custodianArtifactId;
			}
			catch (Exception ex)
			{
				throw new Exception("An error occured when creating Custodian", ex);
			}
		}

		public static async Task<int> CreateProcessingSetAsync(HttpClient httpClient, int processingProfileArtifactId, int workspaceId)
		{
			try
			{
				string url = "Relativity.REST/api/Relativity.Processing.Services.IProcessingModule/Processing Set Manager/SaveAsync";
				ProcessingSetSaveRequest processingSetSaveRequest = new ProcessingSetSaveRequest
				{
					ProcessingSet = new Processingset
					{
						Name = Constants.Processing.Set.NAME,
						Profile = new Profile
						{
							ArtifactID = processingProfileArtifactId
						}
					},
					workspaceArtifactId = workspaceId
				};
				string request = JsonConvert.SerializeObject(processingSetSaveRequest);

				Console2.WriteDisplayStartLine($"Creating Processing Set [Name: {Constants.Processing.Set.NAME}]");
				HttpResponseMessage response = RESTConnectionManager.MakePost(httpClient, url, request);
				string result = await response.Content.ReadAsStringAsync();
				bool success = HttpStatusCode.OK == response.StatusCode;
				if (!success)
				{
					throw new Exception("Failed to Create Processing Set");
				}

				int processingSetArtifactId = Int32.Parse(result);
				Console2.WriteDebugLine($"Processing Set ArtifactId: {processingSetArtifactId}");
				Console2.WriteDisplayEndLine("Created Processing Set!");

				return processingSetArtifactId;
			}
			catch (Exception ex)
			{
				throw new Exception("An error occured when creating Processing Set", ex);
			}
		}

		public static async Task CreateProcessingDataSourceAsync(HttpClient httpClient, int processingSetArtifactId, int custodianArtifactId, int timeZoneArtifactId, int destinationFolderArtifactId, int workspaceId)
		{
			try
			{
				string url = "Relativity.REST/api/Relativity.Processing.Services.IProcessingModule/Processing Data Source Manager/SaveAsync";
				ProcessingDataSourceSaveRequest processingDataSourceSaveRequest = new ProcessingDataSourceSaveRequest
				{
					processingDataSource = new Processingdatasource
					{
						ProcessingSet = new ProcessingSet
						{
							ArtifactID = processingSetArtifactId
						},
						Custodian = custodianArtifactId,
						DestinationFolder = destinationFolderArtifactId,
						DocumentNumberingPrefix = Constants.Processing.DataSource.DOCUMENT_NUMBERING_PREFIX,
						InputPath = Constants.Processing.DataSource.INPUT_PATH,
						Name = Constants.Processing.DataSource.NAME,
						OcrLanguages = new[] { "English" },
						Order = Constants.Processing.DataSource.ORDER,
						TimeZone = timeZoneArtifactId,
						StartNumber = Constants.Processing.DataSource.START_NUMBER,
						IsStartNumberVisible = Constants.Processing.DataSource.IS_START_NUMBER_VISIBLE
					},
					workspaceArtifactId = workspaceId
				};
				string request = JsonConvert.SerializeObject(processingDataSourceSaveRequest);

				Console2.WriteDisplayStartLine($"Creating Processing Data Source [Name: {Constants.Processing.DataSource.NAME}]");
				HttpResponseMessage response = RESTConnectionManager.MakePost(httpClient, url, request);
				string result = await response.Content.ReadAsStringAsync();
				bool success = HttpStatusCode.OK == response.StatusCode;
				if (!success)
				{
					throw new Exception("Failed to Create Processing Data Source");
				}

				int processingDataSourceArtifactId = Int32.Parse(result);
				Console2.WriteDebugLine($"Processing Data Source ArtifactId: {processingDataSourceArtifactId}");
				Console2.WriteDisplayEndLine("Created Processing Data Source!");
			}
			catch (Exception ex)
			{
				throw new Exception("An error occured when creating Processing Data Source", ex);
			}
		}

		public static async Task InventoryJobAsync(HttpClient httpClient, int processingSetId, int workspaceId)
		{
			try
			{
				string url = "Relativity.REST/api/Relativity.Processing.Services.IProcessingModule/Processing Job Manager/SubmitInventoryJobsAsync";
				InventoryJobRequest inventoryJobRequest = new InventoryJobRequest
				{
					InventoryJob = new Iventoryjob
					{
						ProcessingSetId = processingSetId,
						WorkspaceArtifactId = workspaceId
					}
				};
				string request = JsonConvert.SerializeObject(inventoryJobRequest);

				Console2.WriteDisplayStartLine("Starting Inventory Job");
				HttpResponseMessage response = RESTConnectionManager.MakePost(httpClient, url, request);
				string result = await response.Content.ReadAsStringAsync();
				bool success = HttpStatusCode.OK == response.StatusCode;
				if (!success)
				{
					throw new Exception("Failed to Start Inventory Job");
				}

				Console2.WriteDisplayEndLine("Started Inventory Job!");
			}
			catch (Exception ex)
			{
				throw new Exception("An error occured when staring Processing Inventory Job", ex);
			}
		}

		public static async Task DiscoveryJobAsync(HttpClient httpClient, int processingSetId, int workspaceId)
		{
			try
			{
				string url = "Relativity.REST/api/Relativity.Processing.Services.IProcessingModule/Processing Job Manager/SubmitDiscoveryJobsAsync";
				DiscoveryJobRequest discoveryJobRequest = new DiscoveryJobRequest
				{
					DiscoveryJob = new Discoveryjob
					{
						ProcessingSetId = processingSetId,
						WorkspaceArtifactId = workspaceId
					}
				};
				string request = JsonConvert.SerializeObject(discoveryJobRequest);

				Console2.WriteDisplayStartLine("Starting Discovery Job");
				HttpResponseMessage response = RESTConnectionManager.MakePost(httpClient, url, request);
				string result = await response.Content.ReadAsStringAsync();
				bool success = HttpStatusCode.OK == response.StatusCode;
				if (!success)
				{
					throw new Exception("Failed to Start Discovery Job");
				}

				Console2.WriteDisplayEndLine("Started Discovery Job!");
			}
			catch (Exception ex)
			{
				throw new Exception("An error occured when staring Processing Discovery Job", ex);
			}
		}

		public static async Task PublishJobAsync(HttpClient httpClient, int processingSetId, int workspaceId)
		{
			try
			{
				string url = "Relativity.REST/api/Relativity.Processing.Services.IProcessingModule/Processing Job Manager/SubmitPublishJobsAsync";
				PublishJobRequest publishJobRequest = new PublishJobRequest
				{
					PublishJob = new Publishjob
					{
						ProcessingSetId = processingSetId,
						WorkspaceArtifactId = workspaceId
					}
				};
				string request = JsonConvert.SerializeObject(publishJobRequest);

				Console2.WriteDisplayStartLine("Starting Publish Job");
				HttpResponseMessage response = RESTConnectionManager.MakePost(httpClient, url, request);
				string result = await response.Content.ReadAsStringAsync();
				bool success = HttpStatusCode.OK == response.StatusCode;
				if (!success)
				{
					throw new Exception("Failed to Start Publish Job");
				}

				Console2.WriteDisplayEndLine("Started Publish Job!");
			}
			catch (Exception ex)
			{
				throw new Exception("An error occured when staring Processing Publish Job", ex);
			}
		}

		public static async Task WaitForInventoryJobToCompleteAsync(HttpClient httpClient, int workspaceId, int processingSetId)
		{
			Console2.WriteDisplayStartLine("Waiting for Inventory Job to finish...");
			bool inventoryComplete = await JobCompletedSuccessfullyAsync(httpClient, workspaceId, processingSetId, Constants.ProcessingJobType.Inventory);
			if (!inventoryComplete)
			{
				throw new Exception("Inventory Job failed to Complete.");
			}
			Console2.WriteDisplayEndLine("Inventory Job Complete!");
		}

		public static async Task WaitForDiscoveryJobToCompleteAsync(HttpClient httpClient, int workspaceId, int processingSetId)
		{
			Console2.WriteDisplayStartLine("Waiting for Discovery Job to finish...");
			bool discoveryComplete = await JobCompletedSuccessfullyAsync(httpClient, workspaceId, processingSetId, Constants.ProcessingJobType.Discover);
			if (!discoveryComplete)
			{
				throw new Exception("Discovery Job failed to Complete.");
			}
			Console2.WriteDisplayEndLine("Discovery Job Complete!");
		}

		public static async Task WaitForPublishJobToCompleteAsync(HttpClient httpClient, int workspaceId, int processingSetId)
		{
			Console2.WriteDisplayStartLine("Waiting for Publish Job to finish...");
			bool publishComplete = await JobCompletedSuccessfullyAsync(httpClient, workspaceId, processingSetId, Constants.ProcessingJobType.Publish);
			if (!publishComplete)
			{
				throw new Exception("Publish Job failed to Complete.");
			}
			Console2.WriteDisplayEndLine("Publish Job Complete!");
		}

		public static async Task<bool> JobCompletedSuccessfullyAsync(HttpClient httpClient, int workspaceId, int processingSetId, Constants.ProcessingJobType jobtype)
		{
			bool jobComplete = false;
			const int maxTimeInMilliseconds = (Constants.Waiting.MAX_WAIT_TIME_IN_MINUTES * 60 * 1000);
			const int sleepTimeInMilliSeconds = Constants.Waiting.SLEEP_TIME_IN_SECONDS * 1000;
			int currentWaitTimeInMilliseconds = 0;

			Guid fieldGuid;
			if (jobtype == Constants.ProcessingJobType.Inventory)
			{
				fieldGuid = Constants.Guids.Fields.ProcessingSet.InventoryStatus;
			}
			else if (jobtype == Constants.ProcessingJobType.Discover)
			{
				fieldGuid = Constants.Guids.Fields.ProcessingSet.DiscoverStatus;
			}
			else
			{
				fieldGuid = Constants.Guids.Fields.ProcessingSet.PublishStatus;
			}

			string url = $"Relativity.REST/api/Relativity.Objects/workspace/{workspaceId}/object/read";
			ReadRequest readRequest = new ReadRequest
			{
				Request = new request
				{
					Object = new Object
					{
						ArtifactID = processingSetId
					},
					Fields = new[]
					{
						new field
						{
							Guid = fieldGuid.ToString()
						}
					}
				}
			};
			string request = JsonConvert.SerializeObject(readRequest);

			try
			{
				while (currentWaitTimeInMilliseconds < maxTimeInMilliseconds && jobComplete == false)
				{
					Thread.Sleep(sleepTimeInMilliSeconds);

					HttpResponseMessage response = RESTConnectionManager.MakePost(httpClient, url, request);
					string result = await response.Content.ReadAsStringAsync();
					bool success = HttpStatusCode.OK == response.StatusCode;
					if (!success)
					{
						throw new Exception("Failed to Check if the Job is Complete.");
					}
					JObject resultObject = JObject.Parse(result);
					jobComplete = resultObject["Object"]["FieldValues"][0]["Value"]["Name"].Value<string>().Contains("Complete");

					currentWaitTimeInMilliseconds += sleepTimeInMilliSeconds;
				}

				return jobComplete;
			}
			catch (Exception ex)
			{
				throw new Exception($@"Error Checking for {jobtype.ToString()} Job Completion: {ex.ToString()}");
			}
		}

		public static async Task<int> GetTimeZoneArtifactIdAsync(HttpClient httpClient, int workspaceId)
		{
			try
			{
				string url = $"Relativity.REST/api/Relativity.Objects/workspace/{workspaceId}/object/queryslim";
				QueryRequest timeZoneRequest = new QueryRequest
				{
					request = new Request
					{
						ObjectType = new Objecttype
						{
							Guid = Constants.Guids.ObjectType.RelativityTimeZone.ToString()
						}
					},
					start = 0,
					length = 100
				};
				string request = JsonConvert.SerializeObject(timeZoneRequest);

				HttpResponseMessage response = RESTConnectionManager.MakePost(httpClient, url, request);
				string result = await response.Content.ReadAsStringAsync();
				bool success = HttpStatusCode.OK == response.StatusCode;
				if (!success)
				{
					throw new Exception("Failed to Query for Time Zone ArtifactId.");
				}

				JObject resultObject = JObject.Parse(result);
				int timeZoneArtifactId = resultObject["Objects"][0]["ArtifactID"].Value<int>();
				return timeZoneArtifactId;
			}
			catch (Exception ex)
			{
				throw new Exception($@"Error getting Time Zone ArtifactId {ex.ToString()}");
			}
		}

		public static async Task<int> GetDefaultFolderArtifactIdAsync(HttpClient httpClient, int workspaceId)
		{
			try
			{
				string url = $"Relativity.REST/api/Relativity.Objects/workspace/{workspaceId}/object/queryslim";
				QueryRequest defaultFolderRequest = new QueryRequest
				{
					request = new Request
					{
						ObjectType = new Objecttype
						{
							ArtifactTypeID = 9
						}
					},
					start = 0,
					length = 100
				};
				string request = JsonConvert.SerializeObject(defaultFolderRequest);

				HttpResponseMessage response = RESTConnectionManager.MakePost(httpClient, url, request);
				string result = await response.Content.ReadAsStringAsync();
				bool success = HttpStatusCode.OK == response.StatusCode;
				if (!success)
				{
					throw new Exception("Failed to Query for Default Folder ArtifactId.");
				}

				JObject resultObject = JObject.Parse(result);
				int defaultFolderArtifactId = resultObject["Objects"][0]["ArtifactID"].Value<int>();
				return defaultFolderArtifactId;
			}
			catch (Exception ex)
			{
				throw new Exception($@"Error getting Default Folder ArtifactId {ex.ToString()}");
			}
		}
	}
}
