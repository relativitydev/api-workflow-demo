using E2EEDRM.Helpers;
using System;
using System.Globalization;
using System.IO;
using System.Net;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace E2EEDRM.REST
{
	public class RESTBlobHelper
	{
		public async Task UploadDocumentsToBlobAsync()
		{
			try
			{
				Console2.WriteDisplayStartLine("Transferring Documents");
				string sourceFolderPath = GetLocalDocumentsFolderPath();
				await UploadAllDocumentsFromFolderAsync(sourceFolderPath);
				Console2.WriteDisplayEndLine("Transferred Documents!");
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
			FileInfo fileInfo = new FileInfo(sourceFilePath);
			string fileName = fileInfo.Name;
			string fileContent = File.ReadAllText(sourceFilePath);
			int contentLength = Encoding.UTF8.GetByteCount(fileContent);
			string queryString = (new Uri(Constants.BlobFuse.AzureSecrets.AZURE_STORAGE_BLOB_CONTAINER_SAS_URI)).Query;
			string blobContainerUri = Constants.BlobFuse.AzureSecrets.AZURE_STORAGE_BLOB_CONTAINER_SAS_URI.Split('?')[0];
			string requestUri = string.Format(CultureInfo.InvariantCulture, "{0}{1}/{2}{3}", blobContainerUri, Constants.BlobFuse.AzureSecrets.CONTAINER_NAME, destinationFilePath, queryString);
			HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(requestUri);
			httpWebRequest.Method = "PUT";
			httpWebRequest.Headers.Add("x-ms-blob-type", "BlockBlob");
			httpWebRequest.Headers.Add("x-ms-version", "2015-12-11");
			string currentUtcDateTime = DateTime.UtcNow.ToString("R");
			httpWebRequest.Headers.Add("x-ms-date", currentUtcDateTime);
			httpWebRequest.ContentLength = contentLength;

			using (Stream requestStream = httpWebRequest.GetRequestStream())
			{
				requestStream.Write(Encoding.UTF8.GetBytes(fileContent), 0, contentLength);
			}
			using (HttpWebResponse httpWebResponse = (HttpWebResponse)(await httpWebRequest.GetResponseAsync()))
			{
				if (httpWebResponse.StatusCode != HttpStatusCode.Created)
				{
					throw new Exception($"An error occured when uploading document {nameof(fileName)}: {fileName}");
				}
			}
		}
	}
}
