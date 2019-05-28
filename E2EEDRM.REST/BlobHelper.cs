using E2EEDRM.Helpers;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using System;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;

namespace E2EEDRM.REST
{
	public class BlobHelper
	{
		private readonly CloudStorageAccount _cloudStorageAccount = CloudStorageAccount.Parse(Constants.BlobFuse.AzureSecrets.AZURE_STORAGE_CONNECTION_STRING);

		public async Task UploadDocumentsToBlobAsync()
		{
			try
			{
				Console2.WriteDisplayStartLine("Transferring Documents");
				string sourceFolderPath = GetLocalDocumentsFolderPath();
				await UploadAllDocumentsFromFolderAsync(sourceFolderPath);
				Console2.WriteDisplayStartLine("Transferred Documents!");
			}
			catch (Exception ex)
			{
				throw new Exception("An error occured when writing documents to Blob storage", ex);
			}
		}

		private string GetLocalDocumentsFolderPath()
		{
			Assembly executingAssembly = Assembly.GetExecutingAssembly();
			string binFolderPath = Path.GetDirectoryName(executingAssembly.Location);
			if (binFolderPath == null)
			{
				throw new Exception("Bin folder path is empty");
			}

			string path = Path.Combine(binFolderPath, Constants.Transfer.LOCAL_DOCUMENTS_FOLDER_PATH);
			return path;
		}

		private async Task UploadAllDocumentsFromFolderAsync(string sourceFolderPath)
		{
			DirectoryInfo directoryInfo = new DirectoryInfo(sourceFolderPath);
			FileInfo[] fileInfos = directoryInfo.GetFiles();
			foreach (FileInfo fileInfo in fileInfos)
			{
				string sourceFilePath = fileInfo.FullName;
				string fileName = fileInfo.Name;
				string destinationPath = $"{Constants.BlobFuse.REMOTE_DOCUMENTS_FOLDER_PATH}/{fileName}";
				await UploadLocalFileToAzureBlobAsync(sourceFilePath, destinationPath);
			}
		}

		private async Task UploadLocalFileToAzureBlobAsync(string sourceFilePath, string destinationFilePath)
		{
			CloudBlockBlob destinationCloudBlockBlob = GetBlob(destinationFilePath);
			using (FileStream fileStream = File.OpenRead(sourceFilePath))
			{
				await destinationCloudBlockBlob.UploadFromStreamAsync(fileStream);
			}
		}

		private CloudBlockBlob GetBlob(string blobName)
		{
			CloudBlobClient cloudBlobClient = _cloudStorageAccount.CreateCloudBlobClient();
			CloudBlobContainer cloudBlobContainer = cloudBlobClient.GetContainerReference(Constants.BlobFuse.AzureSecrets.CONTAINER_NAME);
			cloudBlobContainer.CreateIfNotExistsAsync().Wait();
			CloudBlockBlob cloudBlockBlob = cloudBlobContainer.GetBlockBlobReference(blobName);
			return cloudBlockBlob;
		}
	}
}