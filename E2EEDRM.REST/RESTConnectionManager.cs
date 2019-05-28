using E2EEDRM.Helpers;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;

namespace E2EEDRM.REST
{
	public class RESTConnectionManager
	{
		public static HttpClient GetHttpClient()
		{
			//Set up the client
			HttpClient httpClient = new HttpClient
			{
				BaseAddress = new Uri(Constants.Instance.BASE_RELATIVITY_URL_NO_SERVICES)
			};

			string encoded = System.Convert.ToBase64String(Encoding.ASCII.GetBytes(Constants.Instance.RELATIVITY_ADMIN_USER_NAME + ":" + Constants.Instance.RELATIVITY_ADMIN_PASSWORD));

			//Set the required headers.
			httpClient.DefaultRequestHeaders.Add("X-CSRF-Header", string.Empty);
			httpClient.DefaultRequestHeaders.Add("Authorization", $"Basic {encoded}");

			return httpClient;
		}

		public static HttpResponseMessage MakePost(HttpClient httpClient, string url, string request)
		{
			StringContent content = new StringContent(request);
			content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
			HttpResponseMessage response = httpClient.PostAsync(url, content).Result;
			return response;
		}

		public static HttpResponseMessage MakePut(HttpClient httpClient, string url, string request)
		{
			StringContent content = new StringContent(request);
			content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
			HttpResponseMessage response = httpClient.PutAsync(url, content).Result;
			return response;
		}

		public static HttpResponseMessage MakeGet(HttpClient httpClient, string url)
		{
			HttpResponseMessage response = httpClient.GetAsync(url).Result;
			return response;
		}
	}
}
