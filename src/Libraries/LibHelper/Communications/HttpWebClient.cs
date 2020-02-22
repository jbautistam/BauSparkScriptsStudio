﻿using System;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Bau.Libraries.LibHelper.Communications
{
	/// <summary>
	///		Cliente de Http
	/// </summary>
    public class HttpWebClient
    {
		public HttpWebClient(string userAgent = null)
		{
			if (string.IsNullOrWhiteSpace(userAgent))
				UserAgent = "Mozilla/5.0 (compatible; Googlebot/2.1; +http://www.google.com/bot.html)";
			else
				UserAgent = userAgent;
		}

		/// <summary>
		///		Obtiene el contenido de una URL
		/// </summary>
		public string HttpGet(string uri)
		{
			return HttpGetAsync(uri, CancellationToken.None).Result;
		}

		/// <summary>
		///		Obtiene el contenido de una URL
		/// </summary>
		public Task<string> HttpGetAsync(string uri)
		{
			return HttpGetAsync(uri, CancellationToken.None);
		}

		/// <summary>
		///		Obtiene el contenido de una URL
		/// </summary>
		public async Task<string> HttpGetAsync(string uri, CancellationToken token)
		{
			using (HttpClient client = GetHttpClient())
			{
				try
				{
					HttpResponseMessage response = await client.GetAsync(new Uri(uri, UriKind.Absolute), token);

						return await response.Content.ReadAsStringAsync();
				}
				catch (Exception exception)
				{
					System.Diagnostics.Debug.WriteLine("Excepción: " + exception.Message);
					return string.Empty;
				}
			}
		}

		/// <summary>
		///		Descarga un archivo de forma asíncrona
		/// </summary>
		public async Task DownloadFileAsync(string uri, string fileName)
		{
			try
			{
				using (HttpClient client = GetHttpClient())
					using (HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, uri))
						using (System.IO.Stream responseStream = await (await client.SendAsync(request)).Content.ReadAsStreamAsync())
							using (System.IO.FileStream fileStream = new System.IO.FileStream(fileName, System.IO.FileMode.Create, 
																							  System.IO.FileAccess.Write, 
																							  System.IO.FileShare.None, 2000, true))
							{
								await responseStream.CopyToAsync(fileStream);
							}
			}
			catch (Exception exception)
			{
				System.Diagnostics.Debug.WriteLine($"Excepción cuando se descarga el archivo {exception.Message}");
			}
		}

		/// <summary>
		///		Obtiene el cliente Http
		/// </summary>
		private HttpClient GetHttpClient()
		{
			HttpClientHandler handler = new HttpClientHandler { AutomaticDecompression = DecompressionMethods.Deflate | DecompressionMethods.GZip };
			HttpClient client = new HttpClient(handler);

				// Añade las cabeceras predeterminadas
				client.DefaultRequestHeaders.Add("User-Agent", UserAgent);
				// Devuelve el cliente
				return client;
		}

		/// <summary>
		///		Descripción del agente por el que se hace la petición http
		/// </summary>
		public string UserAgent { get; set; }
	}
}
