using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using E2EEDRM.Helpers;
using E2EEDRM.REST.Models.Review;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace E2EEDRM.REST
{
	public class RESTReviewHelper
	{
		public static async Task<List<int>> GetDocumentsToTagAsync(HttpClient httpClient, int searchID, int workspaceID)
		{
			try
			{
				List<int> DocsToTag = new List<int>();
				string url = $"/Relativity.REST/Workspace/{workspaceID}/Document/QueryResult";
				string fields = "*";
				GetDocs docsToTag = new GetDocs()
				{
					Condition = $"'ArtifactId' IN SAVEDSEARCH {searchID}",
					Fields = new string[] { fields }
				};
				string request = JsonConvert.SerializeObject(docsToTag);

				Console2.WriteDisplayStartLine("Querying documents to Tag");
				HttpResponseMessage response = RESTConnectionManager.MakePost(httpClient, url, request);
				string result = await response.Content.ReadAsStringAsync();
				bool success = (HttpStatusCode.OK == response.StatusCode) || (HttpStatusCode.Created == response.StatusCode);
				if (!success)
				{
					throw new Exception("Failed to obtain documents.");
				}

				JObject resultObject = JObject.Parse(result);
				foreach (JToken token in resultObject["Results"])
				{
					DocsToTag.Add(token["Artifact ID"].Value<int>());
				}
				Console2.WriteDisplayEndLine($"Queried documents to Tag [Count: {DocsToTag.Count}]");

				return DocsToTag;
			}
			catch (Exception ex)
			{
				throw new Exception("An error occurred when Querying for documents to tag", ex);
			}
		}
		public static async Task TagDocumentsAsync(HttpClient httpClient, int workspaceId, List<int> documentsToTag)
		{
			try
			{
				Console2.WriteDisplayStartLine("Tagging all documents as Responsive");
				foreach (var document in documentsToTag)
				{
					try
					{
						string url = $"Relativity.REST/Workspace/{workspaceId}/Document/{document}";
						TaggingDoc newRequestForTagging = new TaggingDoc()
						{
							ArtifactId = document,
							TypeName = "Document",
							FieldValue = Constants.Workspace.ResponsiveField.VALUE

						};
						string request = JsonConvert.SerializeObject(newRequestForTagging);

						HttpResponseMessage response = RESTConnectionManager.MakePut(httpClient, url, request);
						string result = await response.Content.ReadAsStringAsync();
						bool success = HttpStatusCode.OK == response.StatusCode;
						if (!success)
						{
							throw new Exception("Failed to tag documents.");
						}

						//	result = result.Substring(1, result.Length - 2);
						//	JObject resultObject = JObject.Parse(result);
						Console2.WriteDebugLine($"Tagged document as Responsive! [Id: {document}]");
					}
					catch (Exception ex)
					{
						throw new Exception($@"Failed to tag documents {ex.ToString()}");
					}
				}

				Console2.WriteDisplayEndLine("Tagged all documents as Responsive!");
			}
			catch (Exception ex)
			{
				throw new Exception("An error occurred tagging documents as responsive", ex);
			}
		}
	}
}
