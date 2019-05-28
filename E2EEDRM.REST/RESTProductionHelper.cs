using E2EEDRM.Helpers;
using E2EEDRM.REST.Models.Production;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace E2EEDRM.REST
{
	public class RESTProductionHelper
	{
		public static async Task<int> CreateProductionAsync(HttpClient httpClient, int workspaceId)
		{
			try
			{
				string url = $"/Relativity.REST/api/Relativity.Productions.Services.IProductionModule/Production%20Manager/CreateSingleAsync";
				Production prodSettings = new Production()
				{
					Details = new Details() { BrandingFontSize = Constants.Production.BRANDING_FONT_SIZE, EmailRecipients = "", ScaleBrandingFont = false },
					Footers = new Footers() { LeftFooter = new Leftfooter() { Type = "None", Field = new Field1() { ArtifactID = 0, Name = "", ViewFieldID = 0 }, FreeText = "", FriendlyName = "Left Header" } },
					Headers = new Headers() { LeftHeader = new Leftheader() { Type = "None", Field = new Field() { ArtifactID = 0, Name = "", ViewFieldID = 0 }, FreeText = "", FriendlyName = "Left Header" } },
					Name = Constants.Production.NAME,
					Numbering = new Numbering() { AttachmentRelationalField = new Attachmentrelationalfield() { ArtifactID = 0, Name = "", ViewFieldID = 0 }, BatesPrefix = Constants.Production.BATES_PREFIX, BatesStartNumber = Constants.Production.BATES_START_NUMBER, BatesSuffix = Constants.Production.BATES_SUFFIX, NumberOfDigitsForDocumentNumbering = Constants.Production.NUMBER_OF_DIGITS_FOR_DOCUMENT_NUMBERING, NumberingType = "PageLevel" },
					ShouldCopyInstanceOnWorkspaceCreate = false,
					SortOrder = ""
				};
				ProductionObject newProduction = new ProductionObject()
				{
					Production = prodSettings,
					workspaceArtifactID = workspaceId
				};
				string request = JsonConvert.SerializeObject(newProduction);

				Console2.WriteDisplayStartLine($"Creating Production [Name: {Constants.Production.NAME}]");
				HttpResponseMessage response = RESTConnectionManager.MakePost(httpClient, url, request);
				string result = response.Content.ReadAsStringAsync().Result;
				bool success = HttpStatusCode.OK == response.StatusCode;
				if (!success)
				{
					throw new Exception("Failed to create production set.");
				}

				int productionArtifactId = Convert.ToInt32(result);
				Console2.WriteDebugLine($"Production ArtifactId: {productionArtifactId}");
				Console2.WriteDisplayEndLine("Created Production!");

				return productionArtifactId;
			}
			catch (Exception ex)
			{
				throw new Exception("An error occured when creating Production", ex);
			}
		}

		public static async Task AddProductionDataSourceAsync(HttpClient httpClient, int productionId, int workspaceId, int savedSearchArtifactId)
		{
			try
			{
				string url =
					$"/Relativity.REST/api/Relativity.Productions.Services.IProductionModule/Production%20Data%20Source%20Manager/CreateSingleAsync";
				ProductionDataSourceObject productionDataSourceObject = new ProductionDataSourceObject
				{
					dataSource = new Datasource()
					{
						ProductionType = Constants.Production.DataSource.PRODUCTION_TYPE_AS_STRING,
						SavedSearch = new Savedsearch() { ArtifactID = savedSearchArtifactId },
						Name = Constants.Production.DataSource.Name,
						MarkupSet = new Markupset(),
						BurnRedactions = Constants.Production.DataSource.BURN_REDACTIONS,
						UseImagePlaceholder = Constants.Production.DataSource.USE_IMAGE_PLACEHOLDER_AS_STRING,
						PlaceHolder = new ProductionPlaceholderRef()
					},
					workspaceArtifactID = workspaceId,
					productionID = productionId,
				};
				string request = JsonConvert.SerializeObject(productionDataSourceObject);

				Console2.WriteDisplayStartLine($"Creating Production Data Source [Name: {Constants.Production.DataSource.Name}]");
				HttpResponseMessage response = RESTConnectionManager.MakePost(httpClient, url, request);
				string result = await response.Content.ReadAsStringAsync();
				bool success = HttpStatusCode.OK == response.StatusCode;
				if (!success)
				{
					throw new Exception("Failed to create production data source.");
				}

				int dataSourceArtifactId = Int32.Parse(result);
				if (dataSourceArtifactId != 0)
				{
					Console2.WriteDebugLine($"Production Data Source ArtifactId: {dataSourceArtifactId}");
					Console2.WriteDisplayEndLine("Created Production Data Source!");
				}
				else
				{
					throw new Exception("Failed to create Production Data Source");
				}
			}
			catch (Exception ex)
			{
				throw new Exception("An error occured when creating Production Data Source", ex);
			}
		}

		public static async Task StageProductionAsync(HttpClient httpClient, int productionId, int workspaceId)
		{
			try
			{
				string url = $"/Relativity.REST/api/Relativity.Productions.Services.IProductionModule/Production%20Manager/StageProductionAsync";
				ProductionStageRequest productionStageRequest =
					new ProductionStageRequest() { productionArtifactId = productionId, workspaceArtifactId = workspaceId };
				string request = JsonConvert.SerializeObject(productionStageRequest);

				Console2.WriteDisplayStartLine("Staging Production");
				HttpResponseMessage response = RESTConnectionManager.MakePost(httpClient, url, request);
				string result = await response.Content.ReadAsStringAsync();
				bool success = HttpStatusCode.OK == response.StatusCode;
				if (!success)
				{
					throw new Exception("Failed to stage production set.");
				}

				Console2.WriteDisplayEndLine("Staged Production!");
			}
			catch (Exception ex)
			{
				throw new Exception("An error occured when Staging Production", ex);
			}
		}

		public static async Task RunProductionAsync(HttpClient httpClient, int productionId, int workspaceId)
		{
			try
			{
				Console2.WriteDisplayStartLine("Running Production");

				string url = $"/Relativity.REST/api/Relativity.Productions.Services.IProductionModule/Production%20Manager/RunProductionAsync";
				ProductionRunRequest runProductionRequest = new ProductionRunRequest()
				{
					workspaceArtifactID = workspaceId,
					overrideConflicts = true,
					productionArtifactID = productionId,
					suppressWarnings = true
				};
				string request = JsonConvert.SerializeObject(runProductionRequest);

				HttpResponseMessage response = RESTConnectionManager.MakePost(httpClient, url, request);
				string result = await response.Content.ReadAsStringAsync();
				bool success = HttpStatusCode.OK == response.StatusCode;
				if (!success)
				{
					throw new Exception("Failed to run production set.");
				}

				Console2.WriteDisplayEndLine("Ran Production!");
			}
			catch (Exception ex)
			{
				throw new Exception("An error occured when Running Production", ex);
			}
		}

		public static async Task CheckProductionStatusForStagingAsync(HttpClient httpClient, int productionId, int workspaceId)
		{
			try
			{
				string url = $"/Relativity.REST/api/Relativity.Productions.Services.IProductionModule/Production%20Manager/ReadSingleAsync";
				string productionStatus = "";
				ReadProductionRequest readProductionRequest = new ReadProductionRequest()
				{
					DataSourceReadMode = 0,
					productionArtifactID = productionId,
					workspaceArtifactID = workspaceId
				};
				string request = JsonConvert.SerializeObject(readProductionRequest);

				Console2.WriteDisplayStartLine("Waiting for Production Staging to finish");
				while (productionStatus != "Staged")
				{
					HttpResponseMessage response = RESTConnectionManager.MakePost(httpClient, url, request);
					string result = await response.Content.ReadAsStringAsync();
					bool success = HttpStatusCode.OK == response.StatusCode;
					if (!success)
					{
						throw new Exception("Failed to read production set.");
					}
					JToken productionReadResults = JToken.Parse(result);
					var test2 = productionReadResults["ProductionMetadata"];
					productionStatus = test2["Status"].Value<string>();
				}

				Console2.WriteDisplayEndLine("Production Staging Complete!");
			}
			catch (Exception ex)
			{
				throw new Exception("Production Staging failed to Complete", ex);
			}
		}

		public static async Task CheckProductionStatusAsync(HttpClient httpClient, int productionId, int workspaceId)
		{
			try
			{
				string url = $"/Relativity.REST/api/Relativity.Productions.Services.IProductionModule/Production%20Manager/ReadSingleAsync";
				string productionStatus = "Staged";
				ReadProductionRequest readProductionRequest = new ReadProductionRequest()
				{
					DataSourceReadMode = 0,
					productionArtifactID = productionId,
					workspaceArtifactID = workspaceId
				};
				string request = JsonConvert.SerializeObject(readProductionRequest);

				Console2.WriteDisplayStartLine("Waiting for Production Job to finish");
				while (productionStatus != "Produced")
				{
					HttpResponseMessage response = RESTConnectionManager.MakePost(httpClient, url, request);
					string result = await response.Content.ReadAsStringAsync();
					bool success = HttpStatusCode.OK == response.StatusCode;
					if (!success)
					{
						throw new Exception("Failed to read production set.");
					}
					JToken productionReadResults = JToken.Parse(result);
					var test2 = productionReadResults["ProductionMetadata"];
					productionStatus = test2["Status"].Value<string>();
				}

				Console2.WriteDisplayEndLine("Production Job Complete!");
			}
			catch (Exception ex)
			{
				throw new Exception("Production Job failed to Complete", ex);
			}
		}
	}
}




