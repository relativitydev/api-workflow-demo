using E2EEDRM.Helpers;
using kCura.Relativity.Client;
using kCura.Relativity.Client.DTOs;
using Relativity.Imaging.Services.Interfaces;
using Relativity.Services.Exceptions;
using System;
using System.Threading;
using System.Threading.Tasks;
using Constants = E2EEDRM.Helpers.Constants;

namespace E2EEDRM
{
	public class ImagingHelper
	{
		private IImagingProfileManager ImagingProfileManager { get; }
		private IImagingSetManager ImagingSetManager { get; }
		private IImagingJobManager ImagingJobManager { get; }
		private IRSAPIClient RsapiClient { get; }

		public ImagingHelper(IImagingProfileManager imagingProfileManager, IImagingSetManager imagingSetManager, IImagingJobManager imagingJobManager, IRSAPIClient rsapiClient)
		{
			ImagingProfileManager = imagingProfileManager;
			ImagingSetManager = imagingSetManager;
			ImagingJobManager = imagingJobManager;
			RsapiClient = rsapiClient;
		}

		//Handles imaging operations (create profile and set, run job)
		#region Create Imaging Profile / Set

		public async Task<int> CreateImagingProfileAsync(int workspaceArtifactId)
		{
			Console2.WriteDisplayStartLine($"Creating Imaging Profile [Name: {Constants.Imaging.Profile.NAME}]");

			try
			{
				ImagingProfile basicImagingProfile = new ImagingProfile
				{
					BasicOptions = new BasicImagingEngineOptions
					{
						ImageOutputDpi = Constants.Imaging.Profile.IMAGE_OUTPUT_DPI,
						BasicImageFormat = Constants.Imaging.Profile.BASIC_IMAGE_FORMAT,
						ImageSize = Constants.Imaging.Profile.IMAGE_SIZE
					},
					Name = Constants.Imaging.Profile.NAME,
					ImagingMethod = Constants.Imaging.Profile.IMAGING_METHOD
				};

				// Save the ImagingProfile. Successful saves returns the ArtifactID of the ImagingProfile.
				int imagingProfileArtifactId = await ImagingProfileManager.SaveAsync(basicImagingProfile, workspaceArtifactId);

				Console2.WriteDebugLine($"Imaging Profile ArtifactId: {imagingProfileArtifactId}");
				Console2.WriteDisplayEndLine("Created Imaging Profile!");

				return imagingProfileArtifactId;
			}
			catch (ServiceException ex)
			{
				//The service throws an exception of type ServiceException, performs logging and rethrows the exception.
				throw new Exception("An error occured when creating Imaging Profile", ex);
			}
		}

		public async Task<int> CreateImagingSetAsync(int workspaceArtifactId, int savedSearchArtifactId, int imagingProfileArtifactId)
		{
			Console2.WriteDisplayStartLine($"Creating Imaging Set [Name: {Constants.Imaging.Set.NAME}]");

			try
			{
				ImagingSet imagingSet = new ImagingSet
				{
					DataSource = savedSearchArtifactId,
					Name = Constants.Imaging.Set.NAME,
					ImagingProfile = new ImagingProfileRef
					{
						ArtifactID = imagingProfileArtifactId
					},
					EmailNotificationRecipients = Constants.Imaging.Set.EMAIL_NOTIFICATION_RECIPIENTS
				};

				// Save the ImagingSet. Successful saves return the ArtifactID of the ImagingSet.
				int imagingSetArtifactId = await ImagingSetManager.SaveAsync(imagingSet, workspaceArtifactId);

				Console2.WriteDebugLine($"Imaging Set ArtifactId: {imagingSetArtifactId}");
				Console2.WriteDisplayEndLine("Created Imaging Set!");

				return imagingSetArtifactId;
			}
			catch (ServiceException ex)
			{
				//The service throws an exception of type ServiceException, performs logging and rethrows the exception.
				throw new Exception("An error occured when creating Imaging Set", ex);
			}
		}

		#endregion

		#region Run Imaging Job

		public async Task RunImagingJobAsync(int workspaceArtifactId, int imagingSetArtifactId)
		{
			Console2.WriteDisplayStartLine("Creating Imaging Job");

			try
			{
				ImagingJob imagingJob = new ImagingJob
				{
					ImagingSetId = imagingSetArtifactId,
					WorkspaceId = workspaceArtifactId,
					QcEnabled = Constants.Imaging.Job.QC_ENABLED
				};

				//Run an ImagingSet job.
				Console2.WriteDebugLine($"[{nameof(imagingSetArtifactId)}: {imagingSetArtifactId}, {nameof(workspaceArtifactId)}: {workspaceArtifactId}]");
				Guid? jobGuid = (await ImagingJobManager.RunImagingSetAsync(imagingJob)).ImagingJobId;

				Console2.WriteDebugLine($"Imaging Job Guid: {jobGuid.ToString()}");
				Console2.WriteDisplayEndLine("Created Imaging Job!");
			}
			catch (ServiceException ex)
			{
				//The service throws an exception of type ServiceException, performs logging and rethrows the exception.
				throw new Exception("An error occured when running Imaging Job", ex);
			}
		}

		public async Task WaitForImagingJobToCompleteAsync(int workspaceArtifactId, int imagingSetArtifactId)
		{
			RsapiClient.APIOptions.WorkspaceID = workspaceArtifactId;
			Console2.WriteDisplayStartLine("Waiting for Imaging Job to finish");
			bool publishComplete = await JobCompletedSuccessfullyAsync(workspaceArtifactId, imagingSetArtifactId);
			if (!publishComplete)
			{
				throw new Exception("Imaging Job failed to Complete");
			}
			Console2.WriteDisplayEndLine("Imaging Job Complete!");
		}

		public async Task<bool> JobCompletedSuccessfullyAsync(int workspaceArtifactId, int imagingSetArtifactId)
		{
			bool jobComplete = false;
			const int maxTimeInMilliseconds = (Constants.Waiting.MAX_WAIT_TIME_IN_MINUTES * 60 * 1000);
			const int sleepTimeInMilliSeconds = Constants.Waiting.SLEEP_TIME_IN_SECONDS * 1000;
			int currentWaitTimeInMilliseconds = 0;

			Guid fieldGuid = Constants.Guids.Fields.ImagingSet.Status;

			try
			{
				while (currentWaitTimeInMilliseconds < maxTimeInMilliseconds && jobComplete == false)
				{
					Thread.Sleep(sleepTimeInMilliSeconds);

					RDO job = await Task.Run(() => RsapiClient.Repositories.RDO.ReadSingle(imagingSetArtifactId));
					jobComplete = job[fieldGuid].ValueAsFixedLengthText.Contains("Completed");

					currentWaitTimeInMilliseconds += sleepTimeInMilliSeconds;
				}

				return jobComplete;
			}
			catch (Exception ex)
			{
				throw new Exception($@"Error when checking for Imaging Job Completion. [ErrorMessage: {ex}]", ex);
			}
		}

		#endregion
	}
}

