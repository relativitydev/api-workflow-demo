using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using E2EEDRM.Helpers;
using E2EEDRM.Helpers.Models.Review;
using E2EEDRM.REST.Models.Searching;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace E2EEDRM.REST
{
	public class RESTSearchHelper
	{
		public static async Task<int> CreateKeywordSearchAsync(HttpClient httpClient, int workspaceId)
		{
			try
			{
				string url = "Relativity.REST/api/Relativity.Services.Search.ISearchModule/Keyword Search Manager/CreateSingleAsync";
				KeywordSearchCreateRequest keywordSearchCreateRequest = new KeywordSearchCreateRequest
				{
					workspaceArtifactID = workspaceId,
					searchDTO = new Searchdto
					{
						ArtifactTypeID = 10,
						Name = Constants.Search.KeywordSearch.NAME,
						SearchCriteria = new Searchcriteria
						{
							Conditions = new []
							{
								new Condition
								{
									condition = new Condition1
									{
										Operator = "Is Set",
										ConditionType = "Criteria",
										FieldIdentifier = new Fieldidentifier
										{
											Name = Constants.Search.KeywordSearch.CONDITION_FIELD_EXTRACTED_TEXT
										}
									}
								} 
							}
						},
						Fields = new []
						{
							new Field
							{
								Name = "Edit"
							}, 
							new Field
							{
								Name = "File Icon"
							}, 
							new Field
							{
								Name = "Control Number"
							}
						}
					}
				};
				string request = JsonConvert.SerializeObject(keywordSearchCreateRequest);

				Console2.WriteDisplayStartLine("Creating Keyword search for DtSearch Index");
				HttpResponseMessage response = RESTConnectionManager.MakePost(httpClient, url, request);
				string result = await response.Content.ReadAsStringAsync();
				bool success = HttpStatusCode.OK == response.StatusCode;
				if (!success)
				{
					throw new Exception("Failed to Create Keyword Search");
				}

				int keywordSearchArtifactId = Int32.Parse(result);
				Console2.WriteDebugLine($"Keyword Search ArtifactId: {keywordSearchArtifactId}");
				Console2.WriteDisplayEndLine("Created Keyword search for DtSearch Index!");

				return keywordSearchArtifactId;
			}
			catch (Exception ex)
			{
				throw new Exception("An error occured when creating Keyword Search", ex);
			}
		}

		public static async Task<int> GetIndexShareLocationIdAsync(HttpClient httpClient, int workspaceId)
		{
			try
			{
				string url = $"Relativity.Rest/API/Relativity.DtSearchIndexes/workspace/{workspaceId}/dtsearchindexes/indexShares";
				HttpResponseMessage response = RESTConnectionManager.MakeGet(httpClient, url);
				string result = await response.Content.ReadAsStringAsync();
				bool success = HttpStatusCode.OK == response.StatusCode;
				if (!success)
				{
					Console.WriteLine($"Getting Index Share Location Failed. Response Status Code: {response.StatusCode}");
					throw new Exception("Failed to Get Index Share Location");
				}

				result = result.Substring(1, result.Length - 2);
				JObject resultObject = JObject.Parse(result);
				int indexShareLocationId = resultObject["ID"].Value<int>();
	
				return indexShareLocationId;
			}
			catch (Exception ex)
			{
				throw new Exception($@"Error Getting Index Share Location {ex.ToString()}");
			}
		}

		public static async Task<int> CreateDtSearchIndexAsync(HttpClient httpClient, int workspaceId, int keywordSearchArtifactId)
		{
			try
			{
				int indexShareLocationId = await GetIndexShareLocationIdAsync(httpClient, workspaceId);
				string url = $"Relativity.REST/api/Relativity.DtSearchIndexes/workspace/{workspaceId}/dtsearchindexes/";
				DtSearchIndexSaveRequest dtSearchIndexSaveRequest = new DtSearchIndexSaveRequest
				{
					dtSearchIndexRequest = new DtSearchIndexRequest
					{
						Name = Constants.Search.DtSearchIndex.NAME,
						Order = Constants.Search.DtSearchIndex.ORDER,
						SearchSearchID = keywordSearchArtifactId,
						RecognizeDates = Constants.Search.DtSearchIndex.RECOGNIZE_DATES,
						SkipNumericValues = Constants.Search.DtSearchIndex.SKIP_NUMERIC_VALUES,
						IndexShareCodeArtifactID = indexShareLocationId,
						EmailAddress = Constants.Search.DtSearchIndex.EMAIL_ADDRESS,
						NoiseWords = Constants.Search.DtSearchIndex.NOISE_WORDS,
						AlphabetText = Constants.Search.DtSearchIndex.ALPHABET_TEXT,
						DirtySettings = Constants.Search.DtSearchIndex.DIRTY_SETTINGS,
						SubIndexSize = Constants.Search.DtSearchIndex.SUB_INDEX_SIZE,
						FragmentationThreshold = Constants.Search.DtSearchIndex.FRAGMENTATION_THRESHOLD,
						Priority = Constants.Search.DtSearchIndex.PRIORITY
					}
				};
				string request = JsonConvert.SerializeObject(dtSearchIndexSaveRequest);

				Console2.WriteDisplayStartLine("Creating DtSearch Index");
				HttpResponseMessage response = RESTConnectionManager.MakePost(httpClient, url, request);
				string result = await response.Content.ReadAsStringAsync();
				bool success = HttpStatusCode.OK == response.StatusCode;
				if (!success)
				{
					throw new Exception("Failed to Create dtSearch Index");
				}

				int dtSearchIndexArtifactId = Int32.Parse(result);
				Console2.WriteDebugLine($"DtSearch Index ArtifactId: {dtSearchIndexArtifactId}");
				Console2.WriteDisplayEndLine("Created DtSearch Index!");

				return dtSearchIndexArtifactId;
			}
			catch (Exception ex)
			{
				throw new Exception("An error occured when creating DtSearch Index", ex);
			}
		}

		public static async Task BuildDtSearchIndexAsync(HttpClient httpClient, int workspaceId, int dtSearchIndexId)
		{
			try
			{
				string url = $"Relativity.Rest/API/Relativity.DtSearchIndexes/workspace/{workspaceId}/dtsearchindexes/{dtSearchIndexId}/fullBuildIndex";
				BuilddtSearchIndexRequest buildDtSearchIndexRequest = new BuilddtSearchIndexRequest
				{
					isActive = true
				};
				string request = JsonConvert.SerializeObject(buildDtSearchIndexRequest);

				Console2.WriteDisplayStartLine("Building DtSearch Index");
				HttpResponseMessage response = RESTConnectionManager.MakePost(httpClient, url, request);
				string result = await response.Content.ReadAsStringAsync();
				bool success = HttpStatusCode.OK == response.StatusCode;
				if (!success)
				{
					throw new Exception("Failed to Build dtSearch Index");
				}

				Console2.WriteDisplayEndLine("Built DtSearch Index!");
			}
			catch (Exception ex)
			{
				throw new Exception("An error occured when building DtSearch Index", ex);
			}
		}

		public static async Task<int> CreateDtSearchAsync(HttpClient httpClient, int workspaceId, int dtSearchIndexId, string fieldName)
		{
			try
			{
				string url = "Relativity.REST/api/Relativity.Services.Search.ISearchModule/dtSearch Manager/CreateSingleAsync";
				string searchName = Constants.Search.DtSearch.NAME;
				if (fieldName == Constants.Workspace.EXTRACTED_TEXT_FIELD_NAME)
				{
					searchName = Constants.Search.DtSearch.NAME_EXTRACTED_TEXT;
				}
				DtSearchSaveRequest dtSearchSaveRequest = new DtSearchSaveRequest
				{
					workspaceArtifactID = workspaceId,
					searchDTO = new searchdto
					{
						ArtifactTypeID = 10,
						Name = searchName,
						SearchIndex = new searchindex
						{
							ArtifactID = dtSearchIndexId,
							Name = Constants.Search.DtSearchIndex.NAME
						},
						SearchCriteria = new searchcriteria
						{
							Conditions = new []
							{
								new condition
								{
									condition1 = new condition1
									{
										Operator = "Is Set",
										ConditionType = "Criteria",
										FieldIdentifier = new fieldidentifier
										{
											Name = fieldName
										}
									}
								}, 
							}
						},
						Fields = new []
						{
							new Fields
							{
								Name = "Edit"
							},
							new Fields
							{
								Name = "File Icon"
							},
							new Fields
							{
								Name = "Control Number"
							}
						}
					}
				};
				string request = JsonConvert.SerializeObject(dtSearchSaveRequest);

				Console2.WriteDisplayStartLine($"Creating DtSearch for Field [Name: {fieldName}]");
				HttpResponseMessage response = RESTConnectionManager.MakePost(httpClient, url, request);
				string result = await response.Content.ReadAsStringAsync();
				bool success = HttpStatusCode.OK == response.StatusCode;
				if (!success)
				{
					throw new Exception("Failed to Create dtSearch");
				}

				int dtSearchArtifactId = Int32.Parse(result);
				Console2.WriteDebugLine($"DtSearch ArtifactId: {dtSearchArtifactId}");
				Console2.WriteDisplayEndLine("Created DtSearch!");

				return dtSearchArtifactId;
			}
			catch (Exception ex)
			{
				throw new Exception("An error occured when creating DtSearch", ex);
			}
		}

		public static async Task CreateResponsiveFieldAsync(HttpClient httpClient, int workspaceId)
		{
			try
			{
				string url = $"Relativity.REST/Workspace/{workspaceId}/Field";
				CreateResponsiveFieldRequest createResponsiveFieldRequest = new CreateResponsiveFieldRequest
				{
					ArtifactTypeName = "Field",
					Name = Constants.Workspace.ResponsiveField.Name,
					ParentArtifact = new ParentArtifact
					{
						ArtifactID = workspaceId
					},
					ObjectType = new ObjectType
					{
						DescriptorArtifactTypeID = Constants.DOCUMENT_ARTIFACT_TYPE
					},
					FieldTypeID = (int)Constants.Workspace.ResponsiveField.FIELD_TYPE_ID,
					IsRequired = Constants.Workspace.ResponsiveField.IS_REQUIRED,
					OpenToAssociations = Constants.Workspace.ResponsiveField.OPEN_TO_ASSOCIATIONS,
					Linked = Constants.Workspace.ResponsiveField.LINKED,
					AllowSortTally = Constants.Workspace.ResponsiveField.ALLOW_SORT_TALLY,
					Wrapping = Constants.Workspace.ResponsiveField.WRAPPING,
					AllowGroupBy = Constants.Workspace.ResponsiveField.ALLOW_GROUP_BY,
					AllowPivot = Constants.Workspace.ResponsiveField.ALLOW_PIVOT,
					Width = 123,
					IgnoreWarnings = Constants.Workspace.ResponsiveField.IGNORE_WARNINGS,
					NoValue = Constants.Workspace.ResponsiveField.NO_VALUE,
					YesValue = Constants.Workspace.ResponsiveField.YES_VALUE
				};
				string request = JsonConvert.SerializeObject(createResponsiveFieldRequest);

				Console2.WriteDisplayStartLine($"Creating Responsive Field");
				HttpResponseMessage response = RESTConnectionManager.MakePost(httpClient, url, request);
				string result = await response.Content.ReadAsStringAsync();
				bool success = (HttpStatusCode.OK == response.StatusCode) || (HttpStatusCode.Created == response.StatusCode);
				if (!success)
				{
					throw new Exception("Failed to Create Responsive field");
				}

				JObject resultObject = JObject.Parse(result);
				if (!resultObject["Results"][0]["Success"].Value<bool>())
				{
					throw new Exception("Failed to Create Responsive field");
				}
				int responsiveFieldArtifactId = resultObject["Results"][0]["ArtifactID"].Value<int>();
				Console2.WriteDebugLine($"Responsive Field ArtifactId: {responsiveFieldArtifactId}");
				Console2.WriteDisplayEndLine("Created Responsive Field!");
			}
			catch (Exception ex)
			{
				throw new Exception("An error occurred when creating the Responsive field", ex);
			}
		}
	}
}
