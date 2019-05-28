using E2EEDRM.Helpers;
using kCura.Relativity.Client;
using kCura.Relativity.Client.DTOs;
using Relativity.Services.Field;
using Relativity.Services.Interfaces.DtSearchIndexManager;
using Relativity.Services.Interfaces.DtSearchIndexManager.Models;
using Relativity.Services.Search;
using Relativity.Services.SearchIndex;
using Relativity.Services.User;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Constants = E2EEDRM.Helpers.Constants;

namespace E2EEDRM
{
	public class SearchHelper
	{
		private IKeywordSearchManager KeywordSearchManager { get; }
		private IdtSearchManager DtSearchManager { get; }
		private IDtSearchIndexManager DtSearchIndexManager { get; }
		private IRSAPIClient RsapiClient { get; }

		public SearchHelper(IKeywordSearchManager keywordSearchManager, IdtSearchManager dtSearchManager, IDtSearchIndexManager dtSearchIndexManager, IRSAPIClient rsapiClient)
		{
			KeywordSearchManager = keywordSearchManager;
			DtSearchManager = dtSearchManager;
			DtSearchIndexManager = dtSearchIndexManager;
			RsapiClient = rsapiClient;
		}

		public async Task<int> CreateKeywordSearchAsync(int workspaceArtifactId)
		{
			Console2.WriteDisplayStartLine("Creating Keyword search for DtSearch Index");

			try
			{
				SearchContainerRef searchFolder = new SearchContainerRef();

				KeywordSearch keywordSearch = new KeywordSearch
				{
					Name = Constants.Search.KeywordSearch.NAME,
					SearchContainer = searchFolder
				};

				// Get all the query fields available to the current user.
				SearchResultViewFields searchResultViewFields = await KeywordSearchManager.GetFieldsForSearchResultViewAsync(workspaceArtifactId, Constants.DOCUMENT_ARTIFACT_TYPE);

				// Set the owner to the current user, in this case "Admin, Relativity," or "0" for public.
				List<UserRef> searchOwners = await KeywordSearchManager.GetSearchOwnersAsync(workspaceArtifactId);
				keywordSearch.Owner = searchOwners.First(o => o.Name == Constants.Search.KeywordSearch.OWNER);

				// Add the fields to the Fields collection.
				// If a field Name, ArtifactID, Guid, or ViewFieldID is known, a field can be set with that information as well.

				FieldRef fieldRef = searchResultViewFields.FieldsNotIncluded.First(f => f.Name == Constants.Search.KeywordSearch.FIELD_EDIT);
				keywordSearch.Fields.Add(fieldRef);

				fieldRef = searchResultViewFields.FieldsNotIncluded.First(f => f.Name == Constants.Search.KeywordSearch.FIELD_FILE_ICON);
				keywordSearch.Fields.Add(fieldRef);

				fieldRef = searchResultViewFields.FieldsNotIncluded.First(f => f.Name == Constants.Search.KeywordSearch.FIELD_CONTROL_NUMBER);
				keywordSearch.Fields.Add(fieldRef);

				// Create a Criteria for the field named "Extracted Text" where the value is set

				Criteria criteria = new Criteria
				{
					Condition = new CriteriaCondition(
						new FieldRef
						{
							Name = Constants.Search.KeywordSearch.CONDITION_FIELD_EXTRACTED_TEXT
						}, CriteriaConditionEnum.IsSet)
				};

				// Add the search condition criteria to the collection.
				keywordSearch.SearchCriteria.Conditions.Add(criteria);

				// Add a note.

				keywordSearch.Notes = Constants.Search.KeywordSearch.NOTES;
				keywordSearch.ArtifactTypeID = Constants.DOCUMENT_ARTIFACT_TYPE;

				// Create the search.
				int keywordSearchArtifactId = await KeywordSearchManager.CreateSingleAsync(workspaceArtifactId, keywordSearch);

				if (keywordSearchArtifactId == 0)
				{
					throw new Exception("Failed to create the Keyword Search");
				}

				Console2.WriteDebugLine($"Keyword Search ArtifactId: {keywordSearchArtifactId}");
				Console2.WriteDisplayEndLine("Created Keyword search for DtSearch Index!");

				return keywordSearchArtifactId;
			}
			catch (Exception ex)
			{
				throw new Exception("An error occured when creating Keyword Search", ex);
			}
		}

		public async Task<int> CreateDtSearchAsync(int workspaceArtifactId, string fieldName)
		{
			Console2.WriteDisplayStartLine($"Creating DtSearch for Field [Name: {fieldName}]");

			try
			{
				string searchName = Constants.Search.DtSearch.NAME;
				if (fieldName == Constants.Workspace.EXTRACTED_TEXT_FIELD_NAME)
				{
					searchName = Constants.Search.DtSearch.NAME_EXTRACTED_TEXT;
				}
				SearchContainerRef searchContainerRef = new SearchContainerRef();
				dtSearch dtSearch = new dtSearch
				{
					Name = searchName,
					SearchContainer = searchContainerRef
				};

				// Get all the query fields available to the current user.
				SearchResultViewFields searchResultViewFields = await DtSearchManager.GetFieldsForSearchResultViewAsync(workspaceArtifactId, Constants.DOCUMENT_ARTIFACT_TYPE);

				// Get a dtSearch SearchIndex and set it.
				List<SearchIndexRef> searchIndexes = await DtSearchManager.GetSearchIndexesAsync(workspaceArtifactId);
				dtSearch.SearchIndex = searchIndexes.FirstOrDefault();

				// Set the owner to "Public".
				List<UserRef> searchOwners = await DtSearchManager.GetSearchOwnersAsync(workspaceArtifactId);

				dtSearch.Owner = searchOwners.First(o => o.Name == Constants.Search.DtSearch.OWNER);

				// Add the fields to the Fields collection.
				// If a field Name, ArtifactID, Guid, or ViewFieldID is known, a field can be set with that information as well.

				FieldRef field = searchResultViewFields.FieldsNotIncluded.First(f => f.Name == Constants.Search.DtSearch.FIELD_EDIT);
				dtSearch.Fields.Add(field);

				field = searchResultViewFields.FieldsNotIncluded.First(f => f.Name == Constants.Search.DtSearch.FIELD_FILE_ICON);
				dtSearch.Fields.Add(field);

				field = searchResultViewFields.FieldsNotIncluded.First(f => f.Name == Constants.Search.DtSearch.FIELD_CONTROL_NUMBER);
				dtSearch.Fields.Add(field);

				// Create a Criteria for the field named "Extracted Text" where the value is set

				Criteria criteria = new Criteria
				{
					Condition = new CriteriaCondition(new FieldRef
					{
						Name = fieldName
					}, CriteriaConditionEnum.IsSet)
				};

				// Add the search condition criteria to the collection.
				dtSearch.SearchCriteria.Conditions.Add(criteria);

				// Search for the text string "John" with a fuzziness level of 5 and stemming enabled. 
				//search.SearchText = "John";
				//search.FuzzinessLevel = 5;
				//search.EnableStemming = true;

				// Add a note.
				dtSearch.Notes = Constants.Search.DtSearch.NOTES;
				dtSearch.ArtifactTypeID = Constants.DOCUMENT_ARTIFACT_TYPE;

				// Create the search.
				int dtSearchArtifactId = await DtSearchManager.CreateSingleAsync(workspaceArtifactId, dtSearch);

				if (dtSearchArtifactId == 0)
				{
					throw new Exception("Failed to create the DtSearch");
				}

				Console2.WriteDebugLine($"DtSearch ArtifactId: {dtSearchArtifactId}");
				Console2.WriteDisplayEndLine("Created DtSearch!");

				return dtSearchArtifactId;
			}
			catch (Exception ex)
			{
				throw new Exception("An error occured when creating DtSearch", ex);
			}
		}

		public async Task<List<int>> GetDocumentsToTagAsync(int searchArtifactId, int workspaceArtifactId)
		{
			Console2.WriteDisplayStartLine("Querying documents to Tag");

			RsapiClient.APIOptions.WorkspaceID = workspaceArtifactId;
			List<int> documentsToTag = new List<int>();

			Query<Document> documentQuery = new Query<Document>
			{
				Condition = new SavedSearchCondition(searchArtifactId),
				Fields = FieldValue.AllFields
			};

			ResultSet<Document> docResults = await Task.Run(() => RsapiClient.Repositories.Document.Query(documentQuery));
			foreach (Result<Document> singleDoc in docResults.Results)
			{
				// This loop will add the artifactID of each document that met our search Criteria and as such should be tagged responsive or w/e
				documentsToTag.Add(singleDoc.Artifact.ArtifactID);
			}

			Console2.WriteDisplayEndLine($"Queried documents to Tag [Count: {documentsToTag.Count}]");
			return documentsToTag;
		}

		public async Task<int> CreateDtSearchIndexAsync(int workspaceArtifactId, int keywordSearchArtifactId)
		{
			Console2.WriteDisplayStartLine("Creating DtSearch Index");

			try
			{
				int indexShareCodeArtifactId = DtSearchIndexManager.GetIndexShareLocationAsync(workspaceArtifactId).Result[0].ID;

				DtSearchIndexRequest dtSearchIndexRequest = new DtSearchIndexRequest
				{
					Name = Constants.Search.DtSearchIndex.NAME,
					Order = Constants.Search.DtSearchIndex.ORDER,
					SearchSearchID = keywordSearchArtifactId,
					RecognizeDates = Constants.Search.DtSearchIndex.RECOGNIZE_DATES,
					SkipNumericValues = Constants.Search.DtSearchIndex.SKIP_NUMERIC_VALUES,
					IndexShareCodeArtifactID = indexShareCodeArtifactId,
					EmailAddress = Constants.Search.DtSearchIndex.EMAIL_ADDRESS,
					NoiseWords = Constants.Search.DtSearchIndex.NOISE_WORDS,
					AlphabetText = Constants.Search.DtSearchIndex.ALPHABET_TEXT,
					DirtySettings = Constants.Search.DtSearchIndex.DIRTY_SETTINGS,
					SubIndexSize = Constants.Search.DtSearchIndex.SUB_INDEX_SIZE,
					FragmentationThreshold = Constants.Search.DtSearchIndex.FRAGMENTATION_THRESHOLD,
					Priority = Constants.Search.DtSearchIndex.PRIORITY
				};

				int dtSearchIndexArtifactId = await DtSearchIndexManager.CreateAsync(workspaceArtifactId, dtSearchIndexRequest);

				if (dtSearchIndexArtifactId == 0)
				{
					throw new Exception("Failed to create the DtSearch Index");
				}

				Console2.WriteDebugLine($"DtSearch Index ArtifactId: {dtSearchIndexArtifactId}");
				Console2.WriteDisplayEndLine("Created DtSearch Index!");

				return dtSearchIndexArtifactId;
			}
			catch (Exception ex)
			{
				throw new Exception("An error occured when creating DtSearch Index", ex);
			}
		}

		public async Task BuildDtSearchIndexAsync(int workspaceArtifactId, int dtSearchIndexArtifactId)
		{
			try
			{
				Console2.WriteDisplayStartLine("Building DtSearch Index");

				await DtSearchIndexManager.FullBuildIndexAsync(workspaceArtifactId, dtSearchIndexArtifactId, true);

				Console2.WriteDisplayEndLine("Built DtSearch Index!");
				// we will probably need to do some sort of status check. Any larger batch of documents probably would not build fast enough to just move on?
			}
			catch (Exception ex)
			{
				throw new Exception("An error occured when building DtSearch Index", ex);
			}
		}
	}
}
