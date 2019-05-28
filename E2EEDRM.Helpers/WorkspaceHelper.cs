using E2EEDRM.Helpers;
using kCura.Relativity.Client;
using kCura.Relativity.Client.DTOs;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Constants = E2EEDRM.Helpers.Constants;

namespace E2EEDRM
{
	public class WorkspaceHelper
	{
		private IRSAPIClient RsapiClient { get; }

		public WorkspaceHelper(IRSAPIClient rsapiClient)
		{
			RsapiClient = rsapiClient;
		}

		public async Task CleanupWorkspacesAsync()
		{
			List<int> workspaceArtifactIds = await WorkspaceQueryAsync(Constants.Workspace.NAME);
			if (workspaceArtifactIds.Count > 0)
			{
				Console2.WriteDisplayEndLine("Cleaning up previous Workspaces");
				foreach (int workspaceArtifactId in workspaceArtifactIds)
				{
					await DeleteWorkspaceAsync(workspaceArtifactId);
				}
				Console2.WriteDisplayEndLine("Cleaned up previous Workspaces!");
			}
		}

		public async Task<int> QueryTemplateAndCreateWorkspaceAsync()
		{
			// Query for the RelativityOne Quick Start Template
			List<int> workspaceArtifactIds = await WorkspaceQueryAsync(Constants.Workspace.WORKSPACE_TEMPLATE_NAME);
			if (workspaceArtifactIds.Count > 1)
			{
				throw new Exception($"Multiple Template workspaces exist with the same name [Name: {Constants.Workspace.WORKSPACE_TEMPLATE_NAME}]");
			}

			int templateArtifactId = workspaceArtifactIds.First();

			// Create the workspace 
			return await CreateWorkspaceAsync(templateArtifactId);
		}

		public async Task<List<int>> WorkspaceQueryAsync(string workspaceName)
		{
			Console2.WriteDisplayStartLine($"Querying for Workspaces [Name: {workspaceName}]");

			try
			{
				RsapiClient.APIOptions.WorkspaceID = -1;

				TextCondition textCondition = new TextCondition(WorkspaceFieldNames.Name, TextConditionEnum.EqualTo, workspaceName);
				Query<Workspace> workspaceQuery = new Query<Workspace>
				{
					Fields = FieldValue.AllFields,
					Condition = textCondition
				};

				QueryResultSet<Workspace> workspaceQueryResultSet = await Task.Run(() => RsapiClient.Repositories.Workspace.Query(workspaceQuery));

				if (!workspaceQueryResultSet.Success || workspaceQueryResultSet.Results == null)
				{
					throw new Exception("Failed to query Workspaces");
				}

				List<int> workspaceArtifactIds = workspaceQueryResultSet.Results.Select(x => x.Artifact.ArtifactID).ToList();

				Console2.WriteDisplayEndLine($"Queried for Workspaces! [Count: {workspaceArtifactIds.Count}]");

				return workspaceArtifactIds;
			}
			catch (Exception ex)
			{
				throw new Exception("An error occured when querying Workspaces", ex);
			}
		}

		public async Task<int> CreateWorkspaceAsync(int templateArtifactId)
		{
			Console2.WriteDisplayStartLine("Creating new Workspace");

			try
			{
				const string workspaceCreationFailErrorMessage = "Failed to create new workspace";
				RsapiClient.APIOptions.WorkspaceID = -1;

				//Create the workspace object and apply any desired properties.
				Workspace newWorkspace = new Workspace
				{
					Name = Constants.Workspace.NAME,
					Accessible = Constants.Workspace.ACCESSIBLE,
					DatabaseLocation = Constants.Workspace.DATABASE_LOCATION
				};

				ProcessOperationResult processOperationResult = await Task.Run(() => RsapiClient.Repositories.Workspace.CreateAsync(templateArtifactId, newWorkspace));

				if (!processOperationResult.Success)
				{
					throw new Exception(workspaceCreationFailErrorMessage);
				}

				ProcessInformation processInformation = await Task.Run(() => RsapiClient.GetProcessState(RsapiClient.APIOptions, processOperationResult.ProcessID));

				const int maxTimeInMilliseconds = (Constants.Waiting.MAX_WAIT_TIME_IN_MINUTES * 60 * 1000);
				const int sleepTimeInMilliSeconds = Constants.Waiting.SLEEP_TIME_IN_SECONDS * 1000;
				int currentWaitTimeInMilliseconds = 0;

				while ((currentWaitTimeInMilliseconds < maxTimeInMilliseconds) && (processInformation.State != ProcessStateValue.Completed))
				{
					Thread.Sleep(sleepTimeInMilliSeconds);

					processInformation = await Task.Run(() => RsapiClient.GetProcessState(RsapiClient.APIOptions, processOperationResult.ProcessID));

					currentWaitTimeInMilliseconds += sleepTimeInMilliSeconds;
				}

				int? workspaceArtifactId = processInformation.OperationArtifactIDs.FirstOrDefault();
				if (workspaceArtifactId == null)
				{
					throw new Exception(workspaceCreationFailErrorMessage);
				}

				Console2.WriteDebugLine($"Workspace ArtifactId: {workspaceArtifactId.Value}");
				Console2.WriteDisplayEndLine("Created new Workspace!");

				return workspaceArtifactId.Value;
			}
			catch (Exception ex)
			{
				throw new Exception("An error occured when creating Workspace", ex);
			}
		}

		public async Task<int> CreateResponsiveFieldAsync(int workspaceArtifactId)
		{
			Console2.WriteDisplayStartLine("Creating Responsive Field");

			try
			{
				RsapiClient.APIOptions.WorkspaceID = workspaceArtifactId;

				kCura.Relativity.Client.DTOs.Field responsiveField = new kCura.Relativity.Client.DTOs.Field
				{
					ObjectType = new ObjectType
					{
						DescriptorArtifactTypeID = Constants.DOCUMENT_ARTIFACT_TYPE
					},
					Name = Constants.Workspace.ResponsiveField.Name,
					FieldTypeID = Constants.Workspace.ResponsiveField.FIELD_TYPE_ID,
					IsRequired = Constants.Workspace.ResponsiveField.IS_REQUIRED,
					OpenToAssociations = Constants.Workspace.ResponsiveField.OPEN_TO_ASSOCIATIONS,
					Linked = Constants.Workspace.ResponsiveField.LINKED,
					AllowSortTally = Constants.Workspace.ResponsiveField.ALLOW_SORT_TALLY,
					Wrapping = Constants.Workspace.ResponsiveField.WRAPPING,
					AllowGroupBy = Constants.Workspace.ResponsiveField.ALLOW_GROUP_BY,
					AllowPivot = Constants.Workspace.ResponsiveField.ALLOW_PIVOT,
					IgnoreWarnings = Constants.Workspace.ResponsiveField.IGNORE_WARNINGS,
					Width = Constants.Workspace.ResponsiveField.WIDTH,
					NoValue = Constants.Workspace.ResponsiveField.NO_VALUE,
					YesValue = Constants.Workspace.ResponsiveField.YES_VALUE
				};

				int responsiveFieldArtifactId = await Task.Run(() => RsapiClient.Repositories.Field.CreateSingle(responsiveField));
				if (responsiveFieldArtifactId == 0)
				{
					throw new Exception("Failed to create Responsive Field");
				}

				Console2.WriteDebugLine($"Responsive Field ArtifactId: {responsiveFieldArtifactId}");
				Console2.WriteDisplayEndLine("Created Responsive Field!");

				return responsiveFieldArtifactId;
			}
			catch (Exception ex)
			{
				throw new Exception("An error occured when creating Responsive field", ex);
			}
		}

		public async Task DeleteWorkspaceAsync(int workspaceArtifactId)
		{
			Console2.WriteDisplayStartLine("Deleting Workspace ");

			try
			{
				await Task.Run(() => RsapiClient.Repositories.Workspace.DeleteSingle(workspaceArtifactId));

				Console2.WriteDisplayEndLine("Deleted Workspace!");
			}
			catch (Exception ex)
			{
				throw new Exception("An error occured when deleting Workspace", ex);
			}
		}
	}
}
