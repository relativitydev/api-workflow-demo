using E2EEDRM.Helpers;
using E2EEDRM.REST.Models.Imaging;
using E2EEDRM.REST.Models.RDO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace E2EEDRM.REST
{
	public class RESTImagingHelper
	{
		public static async Task<int> CreateImagingProfileAsync(HttpClient httpClient, int workspaceArtifactId)
		{
			try
			{
				string url = "Relativity.REST/api/Relativity.Imaging.Services.Interfaces.IImagingModule/Imaging Profile Service/SaveAsync";
				ImagingProfileSaveRequest imagingProfileSaveRequest = new ImagingProfileSaveRequest
				{
					imagingProfile = new Imagingprofile
					{
						BasicOptions = new Basicoptions
						{
							ImageOutputDpi = Constants.Imaging.Profile.IMAGE_OUTPUT_DPI,
							BasicImageFormat = Constants.Imaging.Profile.BASIC_IMAGE_FORMAT_AS_STRING,
							ImageSize = Constants.Imaging.Profile.IMAGE_SIZE_AS_STRING
						},
						Name = Constants.Imaging.Profile.NAME,
						ImagingMethod = Constants.Imaging.Profile.IMAGING_METHOD_AS_STRING
					},
					workspaceId = workspaceArtifactId
				};
				string request = JsonConvert.SerializeObject(imagingProfileSaveRequest);

				Console2.WriteDisplayStartLine($"Creating Imaging Profile [Name: {Constants.Imaging.Profile.NAME}]");
				HttpResponseMessage response = RESTConnectionManager.MakePost(httpClient, url, request);
				string result = await response.Content.ReadAsStringAsync();
				bool success = HttpStatusCode.OK == response.StatusCode;
				if (!success)
				{
					throw new Exception("Failed to Create Imaging Profile");
				}

				int imagingProfileArtifactId = Int32.Parse(result);
				Console2.WriteDebugLine($"Imaging Profile ArtifactId: {imagingProfileArtifactId}");
				Console2.WriteDisplayEndLine("Created Imaging Profile!");

				return imagingProfileArtifactId;
			}
			catch (Exception ex)
			{
				throw new Exception("An error occured when creating Imaging Profile", ex);
			}
		}

		public static async Task<int> CreateImagingSetAsync(HttpClient httpClient, int savedSearchArtifactId, int imagingProfileId, int workspaceArtifactId)
		{
			try
			{
				string url = "Relativity.REST/api/Relativity.Imaging.Services.Interfaces.IImagingModule/Imaging Set Service/SaveAsync";
				ImagingSetSaveRequest imagingSetSaveRequest = new ImagingSetSaveRequest
				{
					imagingSet = new Imagingset
					{
						DataSource = savedSearchArtifactId,
						Name = Constants.Imaging.Set.NAME,
						ImagingProfile = new ImagingProfile
						{
							ArtifactID = imagingProfileId
						},
						EmailNotificationRecipients = Constants.Imaging.Set.EMAIL_NOTIFICATION_RECIPIENTS
					},
					workspaceId = workspaceArtifactId
				};
				string request = JsonConvert.SerializeObject(imagingSetSaveRequest);

				Console2.WriteDisplayStartLine($"Creating Imaging Set [Name: {Constants.Imaging.Set.NAME}]");
				HttpResponseMessage response = RESTConnectionManager.MakePost(httpClient, url, request);
				string result = await response.Content.ReadAsStringAsync();
				bool success = HttpStatusCode.OK == response.StatusCode;
				if (!success)
				{
					throw new Exception("Failed to Create Imaging Set");
				}

				int imagingSetArtifactId = Int32.Parse(result);
				Console2.WriteDebugLine($"Imaging Set ArtifactId: {imagingSetArtifactId}");
				Console2.WriteDisplayEndLine("Created Imaging Set!");

				return imagingSetArtifactId;
			}
			catch (Exception ex)
			{
				throw new Exception("An error occured when creating Imaging Set", ex);
			}
		}

		public static async Task RunImagingJobAsync(HttpClient httpClient, int imagingSetId, int workspaceArtifactId)
		{
			try
			{
				string url = "Relativity.REST/api/Relativity.Imaging.Services.Interfaces.IImagingModule/Imaging Job Service/RunImagingSetAsync";
				RunImagingJobRequest runImagingJobRequest = new RunImagingJobRequest
				{
					imagingJob = new Imagingjob
					{
						imagingSetId = imagingSetId,
						workspaceId = workspaceArtifactId
					}
				};
				string request = JsonConvert.SerializeObject(runImagingJobRequest);

				Console2.WriteDisplayStartLine("Creating Imaging Job");
				HttpResponseMessage response = RESTConnectionManager.MakePost(httpClient, url, request);
				string result = await response.Content.ReadAsStringAsync();
				bool success = HttpStatusCode.OK == response.StatusCode;
				if (!success)
				{
					throw new Exception("Failed to Run Imaging Job");
				}

				Console2.WriteDisplayEndLine("Created Imaging Job!");
			}
			catch (Exception ex)
			{
				throw new Exception("An error occured when running Imaging Job", ex);
			}
		}

		public static async Task WaitForImagingJobToCompleteAsync(HttpClient httpClient, int workspaceId, int imagingSetId)
		{
			Console2.WriteDisplayStartLine("Waiting for Imaging Job to finish");
			bool publishComplete = await JobCompletedSuccessfullyAsync(httpClient, workspaceId, imagingSetId);
			if (!publishComplete)
			{
				throw new Exception("Imaging Job failed to Complete.");
			}
			Console2.WriteDisplayEndLine("Imaging Job Complete!");
		}

		public static async Task<bool> JobCompletedSuccessfullyAsync(HttpClient httpClient, int workspaceId, int imagingSetId)
		{
			bool jobComplete = false;
			const int maxTimeInMilliseconds = (Constants.Waiting.MAX_WAIT_TIME_IN_MINUTES * 60 * 1000);
			const int sleepTimeInMilliSeconds = Constants.Waiting.SLEEP_TIME_IN_SECONDS * 1000;
			int currentWaitTimeInMilliseconds = 0;

			Guid fieldGuid = Constants.Guids.Fields.ImagingSet.Status;

			string url = $"Relativity.REST/api/Relativity.Objects/workspace/{workspaceId}/object/read";
			ReadRequest readRequest = new ReadRequest
			{
				Request = new request
				{
					Object = new Models.RDO.Object
					{
						ArtifactID = imagingSetId
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
					jobComplete = resultObject["Object"]["FieldValues"][0]["Value"].Value<string>().Contains("Complete");

					currentWaitTimeInMilliseconds += sleepTimeInMilliSeconds;
				}

				return jobComplete;
			}
			catch (Exception ex)
			{
				throw new Exception($@"Error Checking for Imaging Job Completion: {ex.ToString()}");
			}
		}
	}
}
