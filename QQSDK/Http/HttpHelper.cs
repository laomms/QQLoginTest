//https://github.com/laomms


using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Diagnostics;
using System.Windows.Forms;
using System.Linq;
using System.Xml.Linq;
using System.Threading.Tasks;

using System.IO;
using System.Net;
using System.Text;
using System.Security.Cryptography.X509Certificates;
using System.Net.Security;
using System.Net.Http;
using System.IO.Compression;


namespace QQSDK
{
	internal static class HttpHelper
	{
		public static async Task<string> HttpClientPostAsync2(string url, Dictionary<string, object> Headerdics, byte[] postdata, string datatype, CookieContainer cookieContainers, string redirecturl)
		{
			if (string.IsNullOrEmpty(url))
			{
				return "";
			}
			ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Ssl3 | SecurityProtocolType.Tls | SecurityProtocolType.Tls11;
			ServicePointManager.ServerCertificateValidationCallback = (object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors) =>
			{
																	  return true;
			};
			var res = "";
			try
			{
				using (var handler = new HttpClientHandler())
				{
					handler.CookieContainer = cookieContainers;
					using (var client = new HttpClient(handler))
					{
						foreach (var pair in Headerdics)
						{
							client.DefaultRequestHeaders.TryAddWithoutValidation(pair.Key,(string) pair.Value);
						}
						System.Net.Http.ByteArrayContent content = new System.Net.Http.ByteArrayContent(postdata);
						using (HttpResponseMessage HttpResponse = await client.PostAsync(url, content))
						{
							HttpResponse.EnsureSuccessStatusCode();
							if (HttpResponse.Headers.Location != null)
							{
								redirecturl = HttpResponse.Headers.Location.ToString();
							}
							if (HttpResponse.ToString().ToLower().Contains("gzip"))
							{
								using (Stream HttpResponseStream = await client.GetStreamAsync(url))
								{
									using (var gzipStream = new GZipStream(HttpResponseStream, CompressionMode.Decompress))
									{
										using (var streamReader = new StreamReader(gzipStream, Encoding.UTF8))
										{
											res = streamReader.ReadToEnd();
										}
									}
								}
							}
							else
							{
								using (HttpContent HttpContent = HttpResponse.Content)
								{
									res = await HttpContent.ReadAsStringAsync();
								}
							}
						}
					}
				}
			}
			catch (WebException e)
			{
				using (WebResponse response = e.Response)
				{
					HttpWebResponse httpResponse = (HttpWebResponse)response;
					Console.WriteLine("Error code: {0}", httpResponse.StatusCode);
					using (Stream data = response.GetResponseStream())
					{
						using (var reader = new StreamReader(data))
						{
							res = reader.ReadToEnd();
						}
					}
				}
			}
			catch (Exception ex)
			{
				if (ex.InnerException != null)
				{
					Debug.Print(ex.GetBaseException().Message.ToString());
				}
				else
				{
					Debug.Print(ex.Message.ToString());
				}
			}

			return res;
		}

		public static async Task<string> HttpClientPostAsync(string url, Dictionary<string, object> Headerdics, string postdata, string datatype, CookieContainer cookieContainers, string redirecturl)
		{
			if (string.IsNullOrEmpty(url))
			{
				return "";
			}
			ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Ssl3 | SecurityProtocolType.Tls | SecurityProtocolType.Tls11;
			ServicePointManager.ServerCertificateValidationCallback = (object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors) =>
			{
																	  return true;
			};
			var res = "";
			try
			{
				using (var handler = new HttpClientHandler())
				{
					handler.CookieContainer = cookieContainers;
					using (var client = new HttpClient(handler))
					{
						foreach (var pair in Headerdics)
						{
							client.DefaultRequestHeaders.TryAddWithoutValidation(pair.Key, (string)pair.Value);
						}
						System.Net.Http.StringContent content = new System.Net.Http.StringContent(postdata, System.Text.Encoding.UTF8, datatype);
						using (HttpResponseMessage HttpResponse = await client.PostAsync(url, content))
						{
							HttpResponse.EnsureSuccessStatusCode();
							if (HttpResponse.Headers.Location != null)
							{
								redirecturl = HttpResponse.Headers.Location.ToString();
							}
							if (HttpResponse.ToString().ToLower().Contains("gzip"))
							{
								using (Stream HttpResponseStream = await client.GetStreamAsync(url))
								{
									using (var gzipStream = new GZipStream(HttpResponseStream, CompressionMode.Decompress))
									{
										using (var streamReader = new StreamReader(gzipStream, Encoding.UTF8))
										{
											res = streamReader.ReadToEnd();
										}
									}
								}
							}
							else
							{
								using (HttpContent HttpContent = HttpResponse.Content)
								{
									res = await HttpContent.ReadAsStringAsync();
								}
							}
						}
					}
				}
			}
			catch (WebException e)
			{
				using (WebResponse response = e.Response)
				{
					HttpWebResponse httpResponse = (HttpWebResponse)response;
					Console.WriteLine("Error code: {0}", httpResponse.StatusCode);
					using (Stream data = response.GetResponseStream())
					{
						using (var reader = new StreamReader(data))
						{
							res = reader.ReadToEnd();
						}
					}
				}
			}
			catch (Exception ex)
			{
				if (ex.InnerException != null)
				{
					Debug.Print(ex.GetBaseException().Message.ToString());
				}
				else
				{
					Debug.Print(ex.Message.ToString());
				}
			}

			return res;
		}
		public static async Task<string> HttpClientGetAsync(string url, Dictionary<string, object> Headerdics, CookieContainer cookieContainers, string redirecturl)
		{
			ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Ssl3 | SecurityProtocolType.Tls | SecurityProtocolType.Tls11;
			ServicePointManager.ServerCertificateValidationCallback = (object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors) =>
			{
																	  return true;
			};
			var res = "";
			try
			{
				using (var handler = new HttpClientHandler())
				{
					handler.CookieContainer = cookieContainers;
					using (var client = new HttpClient(handler))
					{
						foreach (var pair in Headerdics)
						{
							client.DefaultRequestHeaders.TryAddWithoutValidation(pair.Key, (string)pair.Value);
						}
						using (HttpResponseMessage HttpResponse = await client.GetAsync(url))
						{
							if (HttpResponse.StatusCode == System.Net.HttpStatusCode.OK)
							{
								if (HttpResponse.Headers.Location != null)
								{
									redirecturl = HttpResponse.Headers.Location.ToString();
								}
								if (HttpResponse.ToString().ToLower().Contains("gzip"))
								{
									using (Stream HttpResponseStream = await client.GetStreamAsync(url))
									{
										using (var gzipStream = new GZipStream(HttpResponseStream, CompressionMode.Decompress))
										{
											using (var streamReader = new StreamReader(gzipStream, Encoding.UTF8))
											{
												res = streamReader.ReadToEnd();
											}
										}
									}
								}
								else
								{
									using (HttpContent HttpContent = HttpResponse.Content)
									{
										res = await HttpContent.ReadAsStringAsync();
									}
								}
							}
						}
					}
				}
			}
			catch (WebException e)
			{
				using (WebResponse response = e.Response)
				{
					HttpWebResponse httpResponse = (HttpWebResponse)response;
					Console.WriteLine("Error code: {0}", httpResponse.StatusCode);
					using (Stream data = response.GetResponseStream())
					{
						using (var reader = new StreamReader(data))
						{
							res = reader.ReadToEnd();
						}
					}
				}
			}
			catch (Exception ex)
			{
				if (ex.InnerException != null)
				{
					Debug.Print(ex.GetBaseException().Message.ToString());
				}
				else
				{
					Debug.Print(ex.Message.ToString());
				}
			}

			return res;
		}
		public static async Task<string> HttpClientPostFormAsync(string url, Dictionary<string, object> Headerdics, MultipartFormDataContent form, CookieContainer cookieContainers, string redirecturl)
		{
			if (string.IsNullOrEmpty(url))
			{
				return "";
			}
			ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Ssl3 | SecurityProtocolType.Tls | SecurityProtocolType.Tls11;
			ServicePointManager.ServerCertificateValidationCallback = (object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors) =>
			{
																	  return true;
			};
			var res = "";
			try
			{
				using (var handler = new HttpClientHandler())
				{
					handler.CookieContainer = cookieContainers;
					using (var client = new HttpClient(handler))
					{
						foreach (var pair in Headerdics)
						{
							client.DefaultRequestHeaders.TryAddWithoutValidation(pair.Key, (string)pair.Value);
						}
						using (HttpResponseMessage HttpResponse = await client.PostAsync(url, form))
						{
							HttpResponse.EnsureSuccessStatusCode();
							if (HttpResponse.Headers.Location != null)
							{
								redirecturl = HttpResponse.Headers.Location.ToString();
							}
							if (HttpResponse.ToString().ToLower().Contains("gzip"))
							{
								using (Stream HttpResponseStream = await client.GetStreamAsync(url))
								{
									using (var gzipStream = new GZipStream(HttpResponseStream, CompressionMode.Decompress))
									{
										using (var streamReader = new StreamReader(gzipStream, Encoding.UTF8))
										{
											res = streamReader.ReadToEnd();
										}
									}
								}
							}
							else
							{
								using (HttpContent HttpContent = HttpResponse.Content)
								{
									res = await HttpContent.ReadAsStringAsync();
								}
							}
						}
					}
				}
			}
			catch (WebException e)
			{
				using (WebResponse response = e.Response)
				{
					HttpWebResponse httpResponse = (HttpWebResponse)response;
					Console.WriteLine("Error code: {0}", httpResponse.StatusCode);
					using (Stream data = response.GetResponseStream())
					{
						using (var reader = new StreamReader(data))
						{
							res = reader.ReadToEnd();
						}
					}
				}
			}
			catch (Exception ex)
			{
				if (ex.InnerException != null)
				{
					Debug.Print(ex.GetBaseException().Message.ToString());
				}
				else
				{
					Debug.Print(ex.Message.ToString());
				}
			}

			return res;
		}
		public static async Task<bool> HttpClientDownloadFileAsync(string url, Dictionary<string, object> Headerdics, CookieContainer cookieContainers, string filepath)
		{
			if (string.IsNullOrEmpty(url))
			{
				return false ;
			}
			ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Ssl3 | SecurityProtocolType.Tls | SecurityProtocolType.Tls11;
			ServicePointManager.ServerCertificateValidationCallback = (object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors) =>
			{
																	  return true;
			};
			try
			{

				using (var client = new HttpClient(new HttpClientHandler()
				{
					CookieContainer = cookieContainers,
					AutomaticDecompression = DecompressionMethods.None | DecompressionMethods.Deflate | DecompressionMethods.GZip
				}))
				{
					foreach (var pair in Headerdics)
					{
						client.DefaultRequestHeaders.TryAddWithoutValidation(pair.Key, (string)pair.Value);
					}

					using (HttpResponseMessage HttpResponse = await client.GetAsync(url))
					{
						if (HttpResponse.StatusCode == System.Net.HttpStatusCode.OK)
						{
							try
							{
								var totalDownloadSize = HttpResponse.Content.Headers.ContentLength;
								var stream = await HttpResponse.Content.ReadAsStreamAsync();
								using (var fileStream = File.Create(filepath))
								{
									using (stream)
									{
										byte[] buffer = new byte[1024];
										var totalBytesRead = 0;
										int length = await stream.ReadAsync(buffer, 0, buffer.Length);
										while (length != 0)
										{
											totalBytesRead += length;
											double percentage =Convert .ToDouble ( totalBytesRead / totalDownloadSize * 100);
											// 写入到文件
											fileStream.Write(buffer, 0, length);
											length = await stream.ReadAsync(buffer, 0, buffer.Length);
										}
										MessageBox.Show("下载完成!");
									}
								}
							}
							catch (Exception e)
							{
							}
						}
					}
				}

			}
			catch (WebException e)
			{
				using (WebResponse response = e.Response)
				{
					HttpWebResponse httpResponse = (HttpWebResponse)response;
					Console.WriteLine("Error code: {0}", httpResponse.StatusCode);
					using (Stream data = response.GetResponseStream())
					{
						using (var reader = new StreamReader(data))
						{
							return false;
						}
					}
				}
			}
			catch (Exception ex)
			{
				if (ex.InnerException != null)
				{
					Debug.Print(ex.GetBaseException().Message.ToString());
				}
				else
				{
					Debug.Print(ex.Message.ToString());
				}
			}

			return false;
		}
		public static async Task<string> HttpClientGetRedirectLink(string url, Dictionary<string, object> Headerdics, CookieContainer cookieContainers)
		{
			if (string.IsNullOrEmpty(url))
			{
				return "";
			}
			var res = "";
			ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Ssl3 | SecurityProtocolType.Tls | SecurityProtocolType.Tls11;
			ServicePointManager.ServerCertificateValidationCallback = (object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors) =>
			{
																	  return true;
			};
			try
			{
				using (var client = new HttpClient(new HttpClientHandler()
				{
					CookieContainer = cookieContainers,
					AutomaticDecompression = DecompressionMethods.None | DecompressionMethods.Deflate | DecompressionMethods.GZip
				}) {Timeout = TimeSpan.FromSeconds(30)})
				{
					foreach (var pair in Headerdics)
					{
						client.DefaultRequestHeaders.TryAddWithoutValidation(pair.Key, (string)pair.Value);
					}
					HttpResponseMessage response = await client.GetAsync(url);
					var statusCode = response.StatusCode;
					if ((int)statusCode >= 300 && (int)statusCode <= 399)
					{
						return response.Headers.Location.ToString();
					}
					else if (!response.IsSuccessStatusCode)
					{
						string responseUri = response.RequestMessage.RequestUri.ToString();
						Console.Out.WriteLine(responseUri);
					}
					else
					{
						return response.RequestMessage.RequestUri.ToString();
					}
				}
			}
			catch (WebException e)
			{
				using (WebResponse response = e.Response)
				{
					HttpWebResponse httpResponse = (HttpWebResponse)response;
					Console.WriteLine("Error code: {0}", httpResponse.StatusCode);
					using (Stream data = response.GetResponseStream())
					{
						using (var reader = new StreamReader(data))
						{
							res = reader.ReadToEnd();
						}
					}
				}
			}
			catch (Exception ex)
			{
				if (ex.InnerException != null)
				{
					Debug.Print(ex.GetBaseException().Message.ToString());
				}
				else
				{
					Debug.Print(ex.Message.ToString());
				}
			}

			return res;
		}
		public static async Task<string> UploadFile(string url, string filepath)
		{
			var request = new HttpClient {Timeout = TimeSpan.FromSeconds(3600)};
			var form = new MultipartFormDataContent();
			string responseString = null;
			using (FileStream fileStream = new FileStream(filepath, mode:FileMode.Open))
			{
				using (BufferedStream bufferedStream = new BufferedStream(fileStream))
				{
					form.Add(new StreamContent(bufferedStream), "file", (new FileInfo(filepath)).FullName);
					var response = await request.PostAsync(url, form);
					responseString = await response.Content.ReadAsStringAsync();
					fileStream.Close();
				}
			}
			return responseString;
		}
		public static string PostImage(string Url, string ImagePath, string FileName)
		{
			HttpClient httpClient = new HttpClient();
			MultipartFormDataContent form = new MultipartFormDataContent();

			byte[] imagebytearraystring = ImageFileToByteArray(ImagePath);
			form.Add(new ByteArrayContent(imagebytearraystring, 0, imagebytearraystring.Count()), FileName, FileName);
			HttpResponseMessage response = httpClient.PostAsync(Url, form).Result;

			httpClient.Dispose();
			return response.Content.ReadAsStringAsync().Result;

		}
		private static byte[] ImageFileToByteArray(string fullFilePath)
		{
			FileStream fs = File.OpenRead(fullFilePath);
			byte[] bytes = new byte[fs.Length];
			fs.Read(bytes, 0, Convert.ToInt32(fs.Length));
			fs.Close();
			return bytes;
		}
		public static void UploadMultipart(byte[] file, string filename, string contentType, string url)
		{
			WebClient webClient = new WebClient();
			string boundary = "------------------------" + DateTime.Now.Ticks.ToString("x");
			webClient.Headers.Add("Content-Type", "multipart/form-data; boundary=" + boundary);
			var fileData = webClient.Encoding.GetString(file);
			var package = string.Format("--{0}" + "\r\n" + "Content-Disposition: form-data; name=\"file\"; filename=\"{1}\"" + "\r\n" + "Content-Type: {2}" + "\r\n" + "\r\n" + "{3}" + "\r\n" + "--{0}--" + "\r\n", boundary, filename, contentType, fileData);
			var nfile = webClient.Encoding.GetBytes(package);
			byte[] resp = webClient.UploadData(url, "POST", nfile);
		}

	}

}