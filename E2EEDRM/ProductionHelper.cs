using E2EEDRM.Helpers;
using Relativity.Productions.Services;
using Relativity.Productions.Services.Interfaces.DTOs;
using Relativity.Services.Exceptions;
using Relativity.Services.Search;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using kCura.Relativity.Client;
using Constants = E2EEDRM.Helpers.Constants;
using kCura.Relativity.Client.DTOs;

namespace E2EEDRM
{
	public class ProductionHelper
	{
		private IProductionManager ProductionManager { get; }
		private IProductionDataSourceManager ProductionDataSourceManager { get; }

		public ProductionHelper(IProductionManager productionManager, IProductionDataSourceManager productionDataSourceManager)
		{
			ProductionManager = productionManager;
			ProductionDataSourceManager = productionDataSourceManager;
		}

		// Create a production with page level numbering
		public async Task<int> CreatePageLevelProductionAsync(int workspaceArtifactId)
		{
			Console2.WriteDisplayStartLine($"Creating Production [Name: {Constants.Production.NAME}]");

			try
			{
				// Construct the production object that you want to create
				Production production = new Production
				{
					Name = Constants.Production.NAME,
					Details = new ProductionDetails
					{
						DateProduced = Constants.Production.DateProduced,
						EmailRecipients = Constants.Production.EMAIL_RECIPIENTS,
						ScaleBrandingFont = Constants.Production.SCALE_BRANDING_FONT,
						BrandingFontSize = Constants.Production.BRANDING_FONT_SIZE,
						PlaceholderImageFormat = Constants.Production.PLACEHOLDER_IMAGE_FORMAT
					},

					Numbering = new PageLevelNumbering
					{
						BatesPrefix = Constants.Production.BATES_PREFIX,
						BatesSuffix = Constants.Production.BATES_SUFFIX,
						BatesStartNumber = Constants.Production.BATES_START_NUMBER,
						NumberOfDigitsForDocumentNumbering = Constants.Production.NUMBER_OF_DIGITS_FOR_DOCUMENT_NUMBERING
					},

					Footers = new ProductionFooters
					{
						LeftFooter = new HeaderFooter(Constants.Production.HEADER_FOOTER_FRIENDLY_NAME)
						{
							Type = Constants.Production.HEADER_FOOTER_TYPE,
							FreeText = Constants.Production.HEADER_FOOTER_FREE_TEXT
						}
					},

					Keywords = Constants.Production.KEYWORDS,
					Notes = Constants.Production.NAME
				};

				// Save the production into the specified workspace
				int productionArtifactId = await ProductionManager.CreateSingleAsync(workspaceArtifactId, production);

				Console2.WriteDebugLine($"Production ArtifactId: {productionArtifactId}");
				Console2.WriteDisplayEndLine("Created Production!");

				return productionArtifactId;
			}
			catch (ValidationException validationException)
			{
				throw new Exception("There are validation errors when creating Production", validationException);
			}
			catch (Exception ex)
			{
				throw new Exception("An error occured when creating Production", ex);
			}
		}

		// Add a data source to the production
		public async Task CreateDataSourceAsync(int workspaceArtifactId, int productionArtifactId, int savedSearchArtifactId)
		{
			Console2.WriteDisplayStartLine($"Creating Production Data Source [Name: {Constants.Production.DataSource.Name}]");

			try
			{
				ProductionDataSource dataSource = new ProductionDataSource()
				{
					Name = Constants.Production.DataSource.Name,
					SavedSearch = new SavedSearchRef(savedSearchArtifactId),
					ProductionType = Constants.Production.DataSource.PRODUCTION_TYPE,
					UseImagePlaceholder = Constants.Production.DataSource.USE_IMAGE_PLACEHOLDER,
					Placeholder = new ProductionPlaceholderRef(),
					BurnRedactions = Constants.Production.DataSource.BURN_REDACTIONS,
					MarkupSet = new MarkupSetRef()
				};

				int dataSourceArtifactId = await ProductionDataSourceManager.CreateSingleAsync(workspaceArtifactId, productionArtifactId, dataSource);

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

		// Stage the production
		public async Task StageProductionAsync(int workspaceArtifactId, int productionArtifactId)
		{
			Console2.WriteDisplayStartLine("Staging Production");

			try
			{
				ProductionJobResult productionJobResult = await ProductionManager.StageProductionAsync(workspaceArtifactId, productionArtifactId);

				ProductionStatusDetailsResult productionStatusDetailsResult = await ProductionManager.GetProductionStatusDetails(workspaceArtifactId, productionArtifactId);

				const int maxTimeInMilliseconds = (Constants.Waiting.MAX_WAIT_TIME_IN_MINUTES * 60 * 1000);
				const int sleepTimeInMilliSeconds = Constants.Waiting.SLEEP_TIME_IN_SECONDS * 1000;
				int currentWaitTimeInMilliseconds = 0;

				while (currentWaitTimeInMilliseconds < maxTimeInMilliseconds && (string)productionStatusDetailsResult.StatusDetails.FirstOrDefault().Value != "Staged")
				{
					Thread.Sleep(sleepTimeInMilliSeconds);

					productionStatusDetailsResult = await ProductionManager.GetProductionStatusDetails(workspaceArtifactId, productionArtifactId);

					currentWaitTimeInMilliseconds += sleepTimeInMilliSeconds;
				}

				if (productionJobResult.Errors.Count != 0)
				{
					const string errorMessage = "There was an error when Staging Production";
					Console2.WriteDebugLine(errorMessage);
					foreach (string item in productionJobResult.Errors)
					{
						Console2.WriteDebugLine(item);
					}

					throw new Exception(errorMessage);
				}

				foreach (string item in productionJobResult.Messages)
				{
					Console2.WriteDebugLine(item);
				}

				Console2.WriteDebugLine(productionStatusDetailsResult.StatusDetails.Last() + "\r\n");
				Console2.WriteDisplayEndLine("Staged Production!");
			}
			catch (Exception ex)
			{
				throw new Exception("An error occured when Staging Production", ex);
			}
		}

		// Run the Production
		public async Task RunProductionAsync(int workspaceArtifactId, int productionArtifactId)
		{
			Console2.WriteDisplayStartLine("Running Production");

			try
			{
				ProductionJobResult productionJobResult = await ProductionManager.RunProductionAsync(workspaceArtifactId, productionArtifactId, true);

				bool wasJobCreated = productionJobResult.WasJobCreated;

				const int maxTimeInMilliseconds = (Constants.Waiting.MAX_WAIT_TIME_IN_MINUTES * 60 * 1000);
				const int sleepTimeInMilliSeconds = Constants.Waiting.SLEEP_TIME_IN_SECONDS * 1000;
				int currentWaitTimeInMilliseconds = 0;

				while (currentWaitTimeInMilliseconds < maxTimeInMilliseconds && wasJobCreated == false)
				{
					Thread.Sleep(sleepTimeInMilliSeconds);

					string errors = productionJobResult.Errors.FirstOrDefault();
					Console2.WriteDebugLine($"Errors: {errors}");

					List<string> warnings = productionJobResult.Warnings;
					Console2.WriteDebugLine($"Warnings: {string.Join("; ", warnings)}");

					List<string> messages = productionJobResult.Messages;
					Console2.WriteDebugLine($"Message: {string.Join("; ", messages)}");

					// Okay, so maybe you've looked at the errors and found some document conflicts 
					// and you want to override it anyway. 
					//bool overrideConflicts = true;
					//bool suppressWarnings = false;

					productionJobResult = await ProductionManager.RunProductionAsync(workspaceArtifactId, productionArtifactId, true);
					wasJobCreated = productionJobResult.WasJobCreated;

					currentWaitTimeInMilliseconds += sleepTimeInMilliSeconds;
				}

				Console2.WriteDisplayEndLine("Ran Production!");
			}
			catch (ValidationException validationException)
			{
				throw new Exception("There are validation errors when Running Production", validationException);
			}
			catch (Exception ex)
			{
				throw new Exception("An error occured when Running Production", ex);
			}
		}

		public async Task WaitForProductionJobToCompleteAsync(int workspaceArtifactId, int productionSetArtifactId)
		{
			Console2.WriteDisplayStartLine("Waiting for Production Job to finish");
			bool publishComplete = await JobCompletedSuccessfullyAsync(workspaceArtifactId, productionSetArtifactId, 5);
			if (!publishComplete)
			{
				throw new Exception("Production Job failed to Complete");
			}
			Console2.WriteDisplayEndLine("Production Job Complete!");
		}

		public async Task<bool> JobCompletedSuccessfullyAsync(int workspaceArtifactId, int productionSetArtifactId, int maxWaitInMinutes)
		{
			bool jobComplete = false;
			const int maxTimeInMilliseconds = (Constants.Waiting.MAX_WAIT_TIME_IN_MINUTES * 60 * 1000);
			const int sleepTimeInMilliSeconds = Constants.Waiting.SLEEP_TIME_IN_SECONDS * 1000;
			int currentWaitTimeInMilliseconds = 0;

			Guid fieldGuid = Constants.Guids.Fields.ProductionSet.Status;

			try
			{
				while (currentWaitTimeInMilliseconds < maxTimeInMilliseconds && jobComplete == false)
				{
					Thread.Sleep(sleepTimeInMilliSeconds);

					Production production = await ProductionManager.ReadSingleAsync(workspaceArtifactId, productionSetArtifactId);
					ProductionStatus productionStatus = production.ProductionMetadata.Status;
					if (productionStatus == ProductionStatus.Produced || productionStatus == ProductionStatus.ProducedWithErrors)
					{
						jobComplete = true;
					}

					currentWaitTimeInMilliseconds += sleepTimeInMilliSeconds;
				}

				return jobComplete;
			}
			catch (Exception ex)
			{
				throw new Exception($@"Error when checking for Production Job Completion. [ErrorMessage: {ex}]", ex);
			}
		}
	}
}

