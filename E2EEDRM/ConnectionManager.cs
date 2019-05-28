using E2EEDRM.Helpers;
using kCura.Relativity.Client;
using Relativity.Imaging.Services.Interfaces;
using Relativity.Processing.Services;
using Relativity.Productions.Services;
using Relativity.Services.Interfaces.DtSearchIndexManager;
using Relativity.Services.Search;
using Relativity.Services.ServiceProxy;
using System;

namespace E2EEDRM
{
	public class ConnectionManager
	{
		private readonly Uri _serviceFactoryServicesUri = new Uri($"{Helpers.Constants.Instance.BASE_RELATIVITY_URL_NO_SERVICES}.Services");
		private readonly Uri _rsapiServicesUri = new Uri($"{Helpers.Constants.Instance.BASE_RELATIVITY_URL}.Services");
		private readonly Uri _restUri = new Uri($"{Helpers.Constants.Instance.BASE_RELATIVITY_URL_NO_SERVICES}.REST/api");
		private ServiceFactory _serviceFactory;
		private IRSAPIClient _rsapiClient;
		private IProcessingProfileManager _processingProfileManager;
		private IProcessingCustodianManager _processingCustodianManager;
		private IProcessingSetManager _processingSetManager;
		private IProcessingJobManager _processingJobManager;
		private IProcessingDataSourceManager _processingDataSourceManager;
		private IDtSearchIndexManager _dtSearchIndexManager;
		private IdtSearchManager _dtSearchManager;
		private IKeywordSearchManager _keywordSearchManager;
		private IImagingProfileManager _imagingProfileManager;
		private IImagingSetManager _imagingSetManager;
		private IImagingJobManager _imagingJobManager;
		private IProductionDataSourceManager _productionDataSourceManager;
		private IProductionManager _productionManager;

		private ServiceFactory ServiceFactory
		{
			get => _serviceFactory ?? (_serviceFactory = CreateServiceFactory());
			set => _serviceFactory = value;
		}

		public IRSAPIClient RsapiClient => _rsapiClient ?? (_rsapiClient = CreateRsapiClient());
		public IProcessingProfileManager ProcessingProfileManager => _processingProfileManager ?? (_processingProfileManager = CreateProcessingProfileManager());
		public IProcessingCustodianManager ProcessingCustodianManager => _processingCustodianManager ?? (_processingCustodianManager = CreateProcessingCustodianManager());
		public IProcessingSetManager ProcessingSetManager => _processingSetManager ?? (_processingSetManager = CreateProcessingSetManager());
		public IProcessingJobManager ProcessingJobManager => _processingJobManager ?? (_processingJobManager = CreateProcessingJobManager());
		public IProcessingDataSourceManager ProcessingDataSourceManager => _processingDataSourceManager ?? (_processingDataSourceManager = CreateProcessingDataSourceManager());
		public IDtSearchIndexManager DtSearchIndexManager => _dtSearchIndexManager ?? (_dtSearchIndexManager = CreateDtSearchIndexManager());
		public IdtSearchManager DtSearchManager => _dtSearchManager ?? (_dtSearchManager = CreateDtSearchManager());
		public IKeywordSearchManager KeywordSearchManager => _keywordSearchManager ?? (_keywordSearchManager = CreateKeywordSearchManager());
		public IImagingProfileManager ImagingProfileManager => _imagingProfileManager ?? (_imagingProfileManager = CreateImagingProfileManager());
		public IImagingSetManager ImagingSetManager => _imagingSetManager ?? (_imagingSetManager = CreateImagingSetManager());
		public IImagingJobManager ImagingJobManager => _imagingJobManager ?? (_imagingJobManager = CreateImagingJobManager());
		public IProductionDataSourceManager ProductionDataSourceManager => _productionDataSourceManager ?? (_productionDataSourceManager = CreateProductionDataSourceManager());
		public IProductionManager ProductionManager => _productionManager ?? (_productionManager = CreateProductionManager());

		private ServiceFactory CreateServiceFactory()
		{
			try
			{
				Relativity.Services.ServiceProxy.UsernamePasswordCredentials usernamePasswordCredentials = new Relativity.Services.ServiceProxy.UsernamePasswordCredentials(Helpers.Constants.Instance.RELATIVITY_ADMIN_USER_NAME, Helpers.Constants.Instance.RELATIVITY_ADMIN_PASSWORD);
				ServiceFactorySettings serviceFactorySettings = new ServiceFactorySettings(_serviceFactoryServicesUri, _restUri, usernamePasswordCredentials);
				ServiceFactory serviceFactory = new ServiceFactory(serviceFactorySettings);
				return serviceFactory;
			}
			catch (Exception ex)
			{
				string errorMessage = $"An error occured when creating an instance of {nameof(ServiceFactory)}";
				Console2.WriteErrorLine(errorMessage);
				throw new Exception(errorMessage, ex);
			}
		}

		private IRSAPIClient CreateRsapiClient()
		{
			try
			{
				IRSAPIClient rsapiClient = new RSAPIClient(_rsapiServicesUri, new kCura.Relativity.Client.UsernamePasswordCredentials(Helpers.Constants.Instance.RELATIVITY_ADMIN_USER_NAME, Helpers.Constants.Instance.RELATIVITY_ADMIN_PASSWORD));
				rsapiClient.APIOptions.WorkspaceID = -1;
				rsapiClient.Login();
				return rsapiClient;
			}
			catch (Exception ex)
			{
				string errorMessage = $"An error occured when creating an instance of {nameof(IRSAPIClient)}";
				Console2.WriteErrorLine(errorMessage);
				throw new Exception(errorMessage, ex);
			}
		}

		private IProcessingProfileManager CreateProcessingProfileManager()
		{
			try
			{
				IProcessingProfileManager processingProfileManager = ServiceFactory.CreateProxy<IProcessingProfileManager>();
				return processingProfileManager;
			}
			catch (Exception ex)
			{
				string errorMessage = $"An error occured when creating an instance of {nameof(IProcessingProfileManager)}";
				Console2.WriteErrorLine(errorMessage);
				throw new Exception(errorMessage, ex);
			}
		}

		private IProcessingCustodianManager CreateProcessingCustodianManager()
		{
			try
			{
				IProcessingCustodianManager processingCustodianManager = ServiceFactory.CreateProxy<IProcessingCustodianManager>();
				return processingCustodianManager;
			}
			catch (Exception ex)
			{
				string errorMessage = $"An error occured when creating an instance of {nameof(IProcessingCustodianManager)}";
				Console2.WriteErrorLine(errorMessage);
				throw new Exception(errorMessage, ex);
			}
		}

		private IProcessingSetManager CreateProcessingSetManager()
		{
			try
			{
				IProcessingSetManager processingSetManager = ServiceFactory.CreateProxy<IProcessingSetManager>();
				return processingSetManager;
			}
			catch (Exception ex)
			{
				string errorMessage = $"An error occured when creating an instance of {nameof(IProcessingSetManager)}";
				Console2.WriteErrorLine(errorMessage);
				throw new Exception(errorMessage, ex);
			}
		}

		private IProcessingJobManager CreateProcessingJobManager()
		{
			try
			{
				IProcessingJobManager processingJobManager = ServiceFactory.CreateProxy<IProcessingJobManager>();
				return processingJobManager;
			}
			catch (Exception ex)
			{
				string errorMessage = $"An error occured when creating an instance of {nameof(IProcessingJobManager)}";
				Console2.WriteErrorLine(errorMessage);
				throw new Exception(errorMessage, ex);
			}
		}

		private IProcessingDataSourceManager CreateProcessingDataSourceManager()
		{
			try
			{
				IProcessingDataSourceManager processingDataSourceManager = ServiceFactory.CreateProxy<IProcessingDataSourceManager>();
				return processingDataSourceManager;
			}
			catch (Exception ex)
			{
				string errorMessage = $"An error occured when creating an instance of {nameof(IProcessingDataSourceManager)}";
				Console2.WriteErrorLine(errorMessage);
				throw new Exception(errorMessage, ex);
			}
		}

		private IDtSearchIndexManager CreateDtSearchIndexManager()
		{
			try
			{
				IDtSearchIndexManager dtSearchIndexManager = ServiceFactory.CreateProxy<IDtSearchIndexManager>();
				return dtSearchIndexManager;
			}
			catch (Exception ex)
			{
				string errorMessage = $"An error occured when creating an instance of {nameof(IDtSearchIndexManager)}";
				Console2.WriteErrorLine(errorMessage);
				throw new Exception(errorMessage, ex);
			}
		}

		private IdtSearchManager CreateDtSearchManager()
		{
			try
			{
				IdtSearchManager dtSearchManager = ServiceFactory.CreateProxy<IdtSearchManager>();
				return dtSearchManager;
			}
			catch (Exception ex)
			{
				string errorMessage = $"An error occured when creating an instance of {nameof(IdtSearchManager)}";
				Console2.WriteErrorLine(errorMessage);
				throw new Exception(errorMessage, ex);
			}
		}

		private IKeywordSearchManager CreateKeywordSearchManager()
		{
			try
			{
				IKeywordSearchManager keywordSearchManager = ServiceFactory.CreateProxy<IKeywordSearchManager>();
				return keywordSearchManager;
			}
			catch (Exception ex)
			{
				string errorMessage = $"An error occured when creating an instance of {nameof(IKeywordSearchManager)}";
				Console2.WriteErrorLine(errorMessage);
				throw new Exception(errorMessage, ex);
			}
		}

		private IImagingProfileManager CreateImagingProfileManager()
		{
			try
			{
				IImagingProfileManager imagingProfileManager = ServiceFactory.CreateProxy<IImagingProfileManager>();
				return imagingProfileManager;
			}
			catch (Exception ex)
			{
				string errorMessage = $"An error occured when creating an instance of {nameof(IImagingProfileManager)}";
				Console2.WriteErrorLine(errorMessage);
				throw new Exception(errorMessage, ex);
			}
		}

		private IImagingSetManager CreateImagingSetManager()
		{
			try
			{
				IImagingSetManager imagingSetManager = ServiceFactory.CreateProxy<IImagingSetManager>();
				return imagingSetManager;
			}
			catch (Exception ex)
			{
				string errorMessage = $"An error occured when creating an instance of {nameof(IImagingSetManager)}";
				Console2.WriteErrorLine(errorMessage);
				throw new Exception(errorMessage, ex);
			}
		}

		private IImagingJobManager CreateImagingJobManager()
		{
			try
			{
				IImagingJobManager imagingJobManager = ServiceFactory.CreateProxy<IImagingJobManager>();
				return imagingJobManager;
			}
			catch (Exception ex)
			{
				string errorMessage = $"An error occured when creating an instance of {nameof(IImagingJobManager)}";
				Console2.WriteErrorLine(errorMessage);
				throw new Exception(errorMessage, ex);
			}
		}

		private IProductionDataSourceManager CreateProductionDataSourceManager()
		{
			try
			{
				IProductionDataSourceManager productionDataSourceManager = ServiceFactory.CreateProxy<IProductionDataSourceManager>();
				return productionDataSourceManager;
			}
			catch (Exception ex)
			{
				string errorMessage = $"An error occured when creating an instance of {nameof(IProductionDataSourceManager)}";
				Console2.WriteErrorLine(errorMessage);
				throw new Exception(errorMessage, ex);
			}
		}

		private IProductionManager CreateProductionManager()
		{
			try
			{
				IProductionManager productionManager = ServiceFactory.CreateProxy<IProductionManager>();
				return productionManager;
			}
			catch (Exception ex)
			{
				string errorMessage = $"An error occured when creating an instance of {nameof(IProductionManager)}";
				Console2.WriteErrorLine(errorMessage);
				throw new Exception(errorMessage, ex);
			}
		}
	}
}
