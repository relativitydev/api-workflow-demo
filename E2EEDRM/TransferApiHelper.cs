using E2EEDRM.Helpers;
using Relativity.Transfer;
using Relativity.Transfer.Aspera;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace E2EEDRM
{
	namespace TransferApiConsole
	{
		public class TransferApiHelper
		{
			private string RelativityUrl { get; }
			private string RelativityUserName { get; }
			private string RelativityPassword { get; }
			private int WorkspaceArtifactId { get; }
			private string LocalDocumentsFolderPath { get; }
			private string RemoteDocumentsFolderPath { get; }

			public TransferApiHelper(string relativityUrl, string relativityUserName, string relativityPassword, int workspaceArtifactId, string localDocumentsFolderPath, string remoteDocumentsFolderPath)
			{
				RelativityUrl = relativityUrl;
				RelativityUserName = relativityUserName;
				RelativityPassword = relativityPassword;
				WorkspaceArtifactId = workspaceArtifactId;
				LocalDocumentsFolderPath = localDocumentsFolderPath;
				RemoteDocumentsFolderPath = remoteDocumentsFolderPath;
			}

			public async Task TransferDocumentsAsync()
			{
				Console2.WriteDisplayStartLine("Transferring Documents");

				try
				{
					InitializeGlobalSettings();

					using (ITransferLog transferLog = CreateTransferLog())
					{
						using (IRelativityTransferHost relativityTransferHost = CreateRelativityTransferHost(transferLog))
						{
							using (CancellationTokenSource cancellationTokenSource = new CancellationTokenSource())
							{
								CancellationToken cancellationToken = cancellationTokenSource.Token;
								await UploadMultipleDocumentsAsync(relativityTransferHost, cancellationToken).ConfigureAwait(false);
							}
						}
					}

					Console2.WriteDisplayEndLine("Transferred Documents!");
				}
				catch (TransferException e)
				{
					if (e.Fatal)
					{
						Console2.WriteDebugLine(ConsoleColor.Red, "A fatal transfer failure has occurred. Error: " + e);
					}
					else
					{
						Console2.WriteDebugLine(ConsoleColor.Red, "A non-fatal transfer failure has occurred. Error: " + e);
					}
				}
				catch (ApplicationException e)
				{
					// No need to include the stacktrace.
					Console2.WriteDebugLine(ConsoleColor.Red, e.Message);
				}
				catch (Exception e)
				{
					Console2.WriteDebugLine(ConsoleColor.Red, "An unexpected error has occurred. Error: " + e);
				}
			}

			private static void InitializeGlobalSettings()
			{
				Console2.WriteTapiStartHeader("Initialize GlobalSettings");

				// A meaningful application name is encoded within monitoring data.
				GlobalSettings.Instance.ApplicationName = "sample-app";

				// Configure for a console-based application.
				GlobalSettings.Instance.CommandLineModeEnabled = true;
				Console2.WriteDebugLine("Configured console settings.");

				// This will automatically write real-time entries into the transfer log.
				GlobalSettings.Instance.StatisticsLogEnabled = true;
				GlobalSettings.Instance.StatisticsLogIntervalSeconds = .5;
				Console2.WriteDebugLine("Configured statistics settings.");

				// Limit the max target rate and throw exceptions when invalid paths are specified.
				GlobalSettings.Instance.MaxAllowedTargetDataRateMbps = 10;
				Console2.WriteDebugLine("Configured miscellaneous settings.");
				Console2.WriteTapiEndHeader();
			}

			private static ITransferLog CreateTransferLog()
			{
				return new NullTransferLog();
			}

			private IRelativityTransferHost CreateRelativityTransferHost(ITransferLog transferLog)
			{
				Uri url = new Uri(RelativityUrl);
				IHttpCredential credential = new BasicAuthenticationCredential(RelativityUserName, RelativityPassword);
				RelativityConnectionInfo connectionInfo = new RelativityConnectionInfo(url, credential, WorkspaceArtifactId);
				return new RelativityTransferHost(connectionInfo, transferLog);
			}

			private static async Task<ITransferClient> CreateClientAsync(IRelativityTransferHost relativityTransferHost, ClientConfiguration clientConfiguration, CancellationToken cancellationToken)
			{
				Console2.WriteTapiStartHeader("Create Client");
				ITransferClient client;
				if (clientConfiguration.Client == WellKnownTransferClient.Unassigned)
				{
					// The CreateClientAsync method chooses the best client at runtime.
					Console2.WriteDebugLine("TAPI is choosing the best transfer client...");
					client = await relativityTransferHost.CreateClientAsync(clientConfiguration, cancellationToken).ConfigureAwait(false);
				}
				else
				{
					// The CreateClient method creates the specified client.
					Console2.WriteDebugLine("The API caller specified the {0} transfer client.", clientConfiguration.Client);
					client = relativityTransferHost.CreateClient(clientConfiguration);
				}

				if (client == null)
				{
					throw new InvalidOperationException("This operation cannot be performed because a transfer client could not be created.");
				}

				Console2.WriteDebugLine("TAPI created the {0} transfer client.", client.DisplayName);
				Console2.WriteTapiEndHeader();
				return client;
			}

			private static TransferContext CreateTransferContext()
			{
				// The context object is used to decouple operations such as progress from other TAPI objects.
				TransferContext context = new TransferContext { StatisticsRateSeconds = 0.5, StatisticsEnabled = true };
				context.TransferPathIssue += (sender, args) =>
				{
					Console2.WriteDebugLine("Event=TransferPathIssue, Attributes={0}", args.Issue.Attributes);
				};

				context.TransferRequest += (sender, args) =>
				{
					Console2.WriteDebugLine("Event=TransferRequest, Status={0}", args.Status);
				};

				context.TransferPathProgress += (sender, args) =>
				{
					Console2.WriteDebugLine(
										"Event=TransferPathProgress, Filename={0}, Status={1}",
										Path.GetFileName(args.Path.SourcePath),
										args.Status);
				};

				context.TransferJobRetry += (sender, args) =>
				{
					Console2.WriteDebugLine("Event=TransferJobRetry, Retry={0}", args.Count);
				};

				context.TransferStatistics += (sender, args) =>
				{
					// Progress has already factored in file-level vs byte-level progress.
					Console2.WriteDebugLine(
							"Event=TransferStatistics, Progress: {0:00.00}%, Transfer rate: {1:00.00} Mbps, Remaining: {2:hh\\:mm\\:ss}",
							args.Statistics.Progress,
							args.Statistics.TransferRateMbps,
							args.Statistics.RemainingTime);
				};

				return context;
			}

			private static AsperaClientConfiguration CreateAsperaClientConfiguration()
			{
				// Each transfer client can provide a specialized The specialized configuration object provides numerous options to customize the transfer.
				return new AsperaClientConfiguration
				{
					// Common properties
					BadPathErrorsRetry = false,
					FileNotFoundErrorsRetry = false,
					MaxHttpRetryAttempts = 2,
					PreserveDates = true,
					TargetDataRateMbps = 5,

					// Aspera specific properties
					EncryptionCipher = "AES_256",
					OverwritePolicy = "ALWAYS",
					Policy = "FAIR",
				};
			}

			private async Task<RelativityFileShare> GetFileShareAsync(IRelativityTransferHost relativityTransferHost, int number, CancellationToken cancellationToken)
			{
				Console2.WriteTapiStartHeader("Get Specified File Share");
				IFileStorageSearch fileStorageSearch = relativityTransferHost.CreateFileStorageSearch();

				// Admin rights are required but this allows searching for all possible file shares within the instance.
				FileStorageSearchContext context = new FileStorageSearchContext { WorkspaceId = Workspace.AdminWorkspaceId };
				FileStorageSearchResults results = await fileStorageSearch.SearchAsync(context, cancellationToken).ConfigureAwait(false);

				// Specify the cloud-based logical file share number - or just the 1st file share when all else fails.
				RelativityFileShare fileShare = results.GetRelativityFileShare(number) ?? results.FileShares.FirstOrDefault();
				if (fileShare == null)
				{
					throw new InvalidOperationException("This operation cannot be performed because there are no file shares available.");
				}

				DisplayFileShare(fileShare);
				Console2.WriteTapiEndHeader();
				return fileShare;
			}

			private async Task<IList<TransferPath>> SearchLocalSourcePathsAsync(ITransferClient transferClient, string uploadTargetPath, CancellationToken cancellationToken)
			{
				Console2.WriteTapiStartHeader("Search Paths");
				string searchLocalPath = GetLocalDocumentsFolderPath();
				const bool local = true;
				PathEnumeratorContext pathEnumeratorContext = new PathEnumeratorContext(transferClient.Configuration, new[] { searchLocalPath }, uploadTargetPath)
				{
					PreserveFolders = false
				};
				IPathEnumerator pathEnumerator = transferClient.CreatePathEnumerator(local);
				EnumeratedPathsResult result = await pathEnumerator.EnumerateAsync(pathEnumeratorContext, cancellationToken).ConfigureAwait(false);
				Console2.WriteDebugLine("Local Paths: {0}", result.LocalPaths);
				Console2.WriteDebugLine("Elapsed time: {0:hh\\:mm\\:ss}", result.Elapsed);
				Console2.WriteDebugLine("Total files: {0:n0}", result.TotalFileCount);
				Console2.WriteDebugLine("Total bytes: {0:n0}", result.TotalByteCount);
				Console2.WriteTapiEndHeader();
				return result.Paths.ToList();
			}

			private async Task UploadMultipleDocumentsAsync(IRelativityTransferHost relativityTransferHost, CancellationToken cancellationToken)
			{
				// Search for the first logical file share.
				const int logicalFileShareNumber = 1;
				RelativityFileShare fileShare = await GetFileShareAsync(relativityTransferHost, logicalFileShareNumber, cancellationToken).ConfigureAwait(false);

				// Configure an Aspera specific transfer.
				AsperaClientConfiguration configuration = CreateAsperaClientConfiguration();

				// Assigning the file share bypasses auto-configuration that will normally use the default workspace repository.
				configuration.TargetFileShare = fileShare;
				using (ITransferClient client = await CreateClientAsync(relativityTransferHost, configuration, cancellationToken).ConfigureAwait(false))
				using (AutoDeleteDirectory directory = new AutoDeleteDirectory())
				{
					// Create a job-based upload transfer request.
					Console2.WriteTapiStartHeader("Advanced Transfer - Upload");
					string uploadTargetPath = GetUniqueRemoteTargetPath(fileShare);
					IList<TransferPath> localSourcePaths = await SearchLocalSourcePathsAsync(client, uploadTargetPath, cancellationToken).ConfigureAwait(false);
					TransferContext context = CreateTransferContext();
					TransferRequest uploadJobRequest = TransferRequest.ForUploadJob(uploadTargetPath, context);
					uploadJobRequest.Application = "Github Sample";
					uploadJobRequest.Name = "Advanced Upload Sample";

					// Create a transfer job to upload the local sample data set to the target remote path.
					using (ITransferJob job = await client.CreateJobAsync(uploadJobRequest, cancellationToken).ConfigureAwait(false))
					{
						Console2.WriteDebugLine("Advanced upload started.");

						// Paths added to the async job are transferred immediately.
						await job.AddPathsAsync(localSourcePaths, cancellationToken).ConfigureAwait(false);

						// Await completion of the job.
						ITransferResult result = await job.CompleteAsync(cancellationToken).ConfigureAwait(false);
						Console2.WriteDebugLine("Advanced upload completed.");
						DisplayTransferResult(result);
						Console2.WriteTapiEndHeader();
					}

					// Create a job-based download transfer request.
					Console2.WriteTapiStartHeader("Advanced Transfer - Download");
					string downloadTargetPath = directory.Path;
					TransferRequest downloadJobRequest = TransferRequest.ForDownloadJob(downloadTargetPath, context);
					downloadJobRequest.Application = "Github Sample";
					downloadJobRequest.Name = "Advanced Download Sample";
					Console2.WriteDebugLine("Advanced download started.");

					// Create a transfer job to download the sample data set to the target local path.
					using (ITransferJob job = await client.CreateJobAsync(downloadJobRequest, cancellationToken).ConfigureAwait(false))
					{
						IEnumerable<TransferPath> remotePaths = localSourcePaths.Select(localPath => new TransferPath
						{
							SourcePath = uploadTargetPath + "\\" + Path.GetFileName(localPath.SourcePath),
							PathAttributes = TransferPathAttributes.File,
							TargetPath = downloadTargetPath
						});

						await job.AddPathsAsync(remotePaths, cancellationToken).ConfigureAwait(false);
						await ChangeDataRateAsync(job, cancellationToken).ConfigureAwait(false);

						// Await completion of the job.
						ITransferResult result = await job.CompleteAsync(cancellationToken).ConfigureAwait(false);
						Console2.WriteDebugLine("Advanced download completed.");
						DisplayTransferResult(result);
						Console2.WriteTapiEndHeader();
					}
				}
			}

			private static async Task ChangeDataRateAsync(ITransferJob transferJob, CancellationToken cancellationToken)
			{
				if (transferJob.IsDataRateChangeSupported)
				{
					Console2.WriteDebugLine("Changing the transfer data rate...");
					await transferJob.ChangeDataRateAsync(0, 10, cancellationToken).ConfigureAwait(false);
					Console2.WriteDebugLine("Changed the transfer data rate.");
				}
			}

			private static void DisplayFileShare(RelativityFileShare relativityFileShare)
			{
				Console2.WriteDebugLine("Artifact ID: {0}", relativityFileShare.ArtifactId);
				Console2.WriteDebugLine("Name: {0}", relativityFileShare.Name);
				Console2.WriteDebugLine("UNC Path: {0}", relativityFileShare.Url);
				Console2.WriteDebugLine("Cloud Instance: {0}", relativityFileShare.CloudInstance);

				// RelativityOne specific properties.
				Console2.WriteDebugLine("Number: {0}", relativityFileShare.Number);
				Console2.WriteDebugLine("Tenant ID: {0}", relativityFileShare.TenantId);
			}

			private static void DisplayTransferResult(ITransferResult transferResult)
			{
				// The original request can be accessed within the transfer result.
				Console2.WriteDebugLine();
				Console2.WriteDebugLine("Transfer Summary");
				Console2.WriteDebugLine("Name: {0}", transferResult.Request.Name);
				Console2.WriteDebugLine("Direction: {0}", transferResult.Request.Direction);
				if (transferResult.Status == TransferStatus.Successful || transferResult.Status == TransferStatus.Canceled)
				{
					Console2.WriteDebugLine("Result: {0}", transferResult.Status);
				}
				else
				{
					Console2.WriteDebugLine(ConsoleColor.Red, "Result: {0}", transferResult.Status);
					if (transferResult.TransferError != null)
					{
						Console2.WriteDebugLine(ConsoleColor.Red, "Error: {0}", transferResult.TransferError.Message);
					}
					else
					{
						Console2.WriteDebugLine(ConsoleColor.Red, "Error: Check the error log for more details.");
					}
				}

				// Display useful transfer metrics.
				Console2.WriteDebugLine("Elapsed time: {0:hh\\:mm\\:ss}", transferResult.Elapsed);
				Console2.WriteDebugLine("Total files: Files: {0:n0}", transferResult.TotalTransferredFiles);
				Console2.WriteDebugLine("Total bytes: Files: {0:n0}", transferResult.TotalTransferredBytes);
				Console2.WriteDebugLine("Total files not found: {0:n0}", transferResult.TotalFilesNotFound);
				Console2.WriteDebugLine("Total bad path errors: {0:n0}", transferResult.TotalBadPathErrors);
				Console2.WriteDebugLine("Data rate: {0:#.##} Mbps", transferResult.TransferRateMbps);
				Console2.WriteDebugLine("Retry count: {0}", transferResult.RetryCount);
			}

			private string GetUniqueRemoteTargetPath(RelativityFileShare relativityFileShare)
			{
				//string path = string.Join("\\", relativityFileShare.Url.TrimEnd('\\'), RemoteDocumentsFolderPath);
				return RemoteDocumentsFolderPath;
			}

			private string GetLocalDocumentsFolderPath()
			{
				//string path = Path.Combine(Environment.CurrentDirectory, "Resources");
				Assembly executingAssembly = Assembly.GetExecutingAssembly();
				string binFolderPath = Path.GetDirectoryName(executingAssembly.Location);
				if (binFolderPath == null)
				{
					throw new Exception("Bin folder path is empty");
				}

				string path = Path.Combine(binFolderPath, LocalDocumentsFolderPath);
				return path;
			}
		}
	}
}
