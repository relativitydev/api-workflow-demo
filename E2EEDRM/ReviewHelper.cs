using E2EEDRM.Helpers;
using kCura.Relativity.Client;
using kCura.Relativity.Client.DTOs;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Constants = E2EEDRM.Helpers.Constants;

namespace E2EEDRM
{
	public class ReviewHelper
	{
		private IRSAPIClient RsapiClient { get; }

		public ReviewHelper(IRSAPIClient rsapiClient)
		{
			RsapiClient = rsapiClient;
		}

		public async Task TagDocumentsResponsiveAsync(int workspaceId, List<int> documentsToTag)
		{
			Console2.WriteDisplayStartLine("Tagging all documents as Responsive");

			RsapiClient.APIOptions.WorkspaceID = workspaceId;
			foreach (int currentDocumentArtifactId in documentsToTag)
			{
				// Read the document
				Document currentDocumentRdo = await Task.Run(() => RsapiClient.Repositories.Document.ReadSingle(currentDocumentArtifactId));

				// Code the document as Responsive
				currentDocumentRdo.Fields.Add(new FieldValue
				{
					Name = Constants.Workspace.ResponsiveField.Name,
					Value = Constants.Workspace.ResponsiveField.VALUE
				});

				try
				{
					// Perform the document update
					WriteResultSet<Document> documentWriteResultSet = await Task.Run(() => RsapiClient.Repositories.Document.Update(currentDocumentRdo));
					if (!documentWriteResultSet.Success)
					{
						Console2.WriteDebugLine($"Error: {documentWriteResultSet.Message} \r\n {documentWriteResultSet.Results[0].Message}");
						Console2.WriteDebugLine(string.Join(";", documentWriteResultSet.Results));
						throw new Exception("Failed to tag document as Responsive");
					}

					Console2.WriteDebugLine($"Tagged document as Responsive! [Name: {currentDocumentRdo.TextIdentifier}]");
				}
				catch (Exception ex)
				{
					throw new Exception("An error occured when tagging document as Responsive", ex);
				}
			}

			Console2.WriteDisplayEndLine("Tagged all documents as Responsive!");
		}
	}
}
