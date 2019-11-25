using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Net;
using System.Net.Cache;
using System.Net.Security;
using System.Reflection;
using System.Text;

namespace IL2CDR.Model
{
	public enum ContentType
	{
		UrlEncoded,
		UrlEncodedUTF8,
		Multipart,
		JsonUTF8
	}

	public class WebClientBase : WebClient
	{
		private readonly object downloadLock = new object();
		private readonly CookieContainer cookieContainer = new CookieContainer();
		private const string USER_AGENT = "Mozilla/5.0 (Windows NT 6.0; WOW64; rv:14.0) Gecko/20100101 Firefox/14.0.1";

		private readonly Dictionary<ContentType, string> contentTypes = new Dictionary<ContentType, string>() {
			{ContentType.JsonUTF8, "application/json; charset=UTF-8"},
			{ContentType.UrlEncodedUTF8, "application/x-www-form-urlencoded; charset=UTF-8"},
			{ContentType.UrlEncoded, "application/x-www-form-urlencoded"},
			{ContentType.Multipart, "multipart/form-data"},
		};

		public WebClientBase()
		{
			ServicePointManager.ServerCertificateValidationCallback = delegate { return true; };
			ServicePointManager.DefaultConnectionLimit = 25;
			ServicePointManager.Expect100Continue = false;
			ServicePointManager.UseNagleAlgorithm = false;
			ServicePointManager.DnsRefreshTimeout = 60;
			ServicePointManager.EnableDnsRoundRobin = true;
			this.KeepAlive = true;
			this.IsAnonymous = false;
			this.ErrorHandler = Log.WriteError;
			this.StartPos = -1;
			this.EndPos = -1;
			this.Timeout = 5000;
			this.Proxy = null;
			this.SuccessHandler = () => { };
		}

		public Action<string> ErrorHandler { get; set; }
		public Action SuccessHandler { get; set; }
		public bool KeepAlive { get; set; }
		public int Timeout { get; set; }
		public long StartPos { get; set; }
		public long EndPos { get; set; }
		public bool IsAnonymous { get; set; }

		protected override WebRequest GetWebRequest(Uri address)
		{
			lock (this.downloadLock) {
				var request = base.GetWebRequest(address);

				if (request is HttpWebRequest webRequest) {
					if (this.KeepAlive) {
						webRequest.ProtocolVersion = HttpVersion.Version10;
						webRequest.KeepAlive = true;
						var sp = webRequest.ServicePoint;
						var prop = sp.GetType().GetProperty("HttpBehaviour",
							BindingFlags.Instance | BindingFlags.NonPublic);
						prop.SetValue(sp, (byte) 0, null);
					} else {
						webRequest.KeepAlive = false;
					}

					if (this.StartPos != -1 && this.EndPos != -1) {
						webRequest.AddRange(this.StartPos, this.EndPos);
					}

					webRequest.Timeout = this.Timeout;
					if (!this.IsAnonymous) {
						webRequest.CookieContainer = this.cookieContainer;
					}

					webRequest.UserAgent = USER_AGENT;
					//webRequest.Proxy = null;	// <-- comented out, since NULL from the original code is an invalid value! 
					webRequest.CachePolicy = new HttpRequestCachePolicy(HttpRequestCacheLevel.NoCacheNoStore);
					webRequest.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;
				}

				return request;
			}
		}

		public string Download(string url)
		{
			return (string) this.TryWeb(url, () => {
				lock (this.downloadLock) {
					this.SuccessHandler();
					this.Encoding = Encoding.UTF8;
					return this.DownloadString(new Uri(url));
				}
			});
		}

		public byte[] DownloadToByteArray(string url)
		{
			try {
				lock (this.downloadLock) {
					this.SuccessHandler();
					return this.DownloadData(new Uri(url));
				}
			} catch {
				this.ErrorHandler($"Error downloading to byte array from {url}");
			}

			return new byte[] { };
		}

		public MemoryStream DownloadToMemoryStream(string url, string method = "GET")
		{
			try {
				lock (this.downloadLock) {
					var request = this.GetWebRequest(new Uri(url));
					if (request == null) {
						return null; 
					}

					request.Method = method;

					var response = this.GetWebResponse(request);
					if (response == null) {
						return null; 
					}

					this.SuccessHandler?.Invoke();

					var memoryStream = new MemoryStream();

					var stream = response.GetResponseStream();

					if (stream != null && stream.CanRead) {
						var buffer = new byte[4096];

						var bytesRead = 0;

						do {
							bytesRead = stream.Read(buffer, 0, buffer.Length);
							memoryStream.Write(buffer, 0, bytesRead);
						} while (bytesRead > 0);

						memoryStream.Position = 0;
						response.Close();
						return memoryStream;
					}
				}
			} catch {
				this.ErrorHandler($"Error downloading {url} to memorystream");
			}

			return null;
		}

		public Stream DownloadToStream(string url, bool cache = false)
		{
			try {
				lock (this.downloadLock) {
					var request = this.GetWebRequest(new Uri(url));
					if (request == null) {
						return null;
					}

					if (cache) {
						request.CachePolicy = new HttpRequestCachePolicy(HttpRequestCacheLevel.CacheIfAvailable);
					}

					var response = this.GetWebResponse(request);

					this.SuccessHandler?.Invoke();

					return response?.GetResponseStream();
				}
			} catch {
				this.ErrorHandler($"Error downloading {url} to stream");
			}

			return null;
		}

		public byte[] GZipBytes(string data)
		{
			using (var outputStream = new MemoryStream())
			using (var gzip = new GZipStream(outputStream, CompressionMode.Compress)) {
				var writer = new StreamWriter(gzip);
				writer.Write(data);
				writer.Flush();
				gzip.Flush();
				gzip.Close();

				return outputStream.ToArray();
			}
		}

		public void CopyTo(Stream src, Stream dest)
		{
			var bytes = new byte[4096];

			int cnt;

			while ((cnt = src.Read(bytes, 0, bytes.Length)) != 0) {
				dest.Write(bytes, 0, cnt);
			}
		}

		public string GUnzip(byte[] bytes)
		{
			using (var input = new MemoryStream(bytes))
			using (var output = new MemoryStream()) {
				using (var gs = new GZipStream(input, CompressionMode.Decompress)) {
					this.CopyTo(gs, output);
				}

				return Encoding.UTF8.GetString(output.ToArray());
			}
		}

		public string UploadCompressed(string url, string args)
		{
			return (string) this.TryWeb(url, () => {
				lock (this.downloadLock) {
					//this.Headers.Add("Content-Encoding", "gzip");
					var result = Encoding.UTF8.GetString(this.UploadData(url, "POST", this.GZipBytes(args)));
					this.SuccessHandler();
					return result;
				}
			});
		}

		public string Upload(string url, byte[] args)
		{
			return (string) this.TryWeb(url, () => {
				lock (this.downloadLock) {
					//this.Headers.Add("Content-Encoding", "gzip");
					var result = Encoding.UTF8.GetString(this.UploadData(url, "POST", args));
					this.SuccessHandler();
					return result;
				}
			});
		}

		public string Upload(string url, string args)
		{
			return (string) this.TryWeb(url, () => {
				lock (this.downloadLock) {
					var result = this.UploadString(url, "POST", args);
					this.SuccessHandler();
					return result;
				}
			});
		}

		public Stream PutStream(string url, Stream stream)
		{
			try {
				var request = (HttpWebRequest) WebRequest.Create(url);
				request.Method = "PUT";
				if (stream != null) {
					request.ContentLength = stream.Length;
					var dataStream = request.GetRequestStream();
					stream.CopyTo(dataStream);
					stream.Flush();
					dataStream.Flush();
					dataStream.Close();
				}

				var response = (HttpWebResponse) request.GetResponse();
				return response.GetResponseStream();
			} catch (Exception e) {
				Log.WriteInfo("PutStream: {0}", e.Message);
				return null;
			}
		}

		public void RequestPatchOptions(string url)
		{
			try {
				var request = (HttpWebRequest) WebRequest.Create(url);
				request.Method = "OPTIONS";

				var response = (HttpWebResponse) request.GetResponse();
				
			} catch (Exception e) {
				Log.WriteInfo("RequestPatchOptions {0}, {1}", e.Message, url);
			}
		}

		public Stream PatchStream(string url, Stream stream)
		{
			try {
				var request = (HttpWebRequest) WebRequest.Create(url);
				request.Method = "PATCH";
				request.Headers = this.Headers;
				request.ContentType = @"application/json;charset=UTF-8";

				if (stream != null) {
					request.ContentLength = stream.Length;
					var dataStream = request.GetRequestStream();
					stream.CopyTo(dataStream);
					stream.Flush();
					dataStream.Flush();
					dataStream.Close();
				}

				var response = (HttpWebResponse) request.GetResponse();
				return response.GetResponseStream();
			} catch (Exception e) {
				Log.WriteInfo("PatchStream {0}, {1}", e.Message, url);
				return null;
			}
		}


		public void SetCookie(string name, string value, string domain)
		{
			if (name == null || value == null) {
				return;
			}

			this.cookieContainer.Capacity += 1;
			this.cookieContainer.Add(new Cookie(name, value, "/", domain));
		}


		public ContentType ContentType
		{
			set => this.Headers[HttpRequestHeader.ContentType] = this.contentTypes[value];
		}

		public string CookieValue(string name, string url)
		{
			var coll = this.cookieContainer.GetCookies(new Uri(url));
			if (coll[name] == null) {
				return string.Empty;
			}

			return coll[name].Value;
		}

		public List<Cookie> CookiesTable
		{
			get
			{
				var table = (Hashtable) this.Cookies.GetType().InvokeMember("m_domainTable",
					BindingFlags.NonPublic |
					BindingFlags.GetField |
					BindingFlags.Instance,
					null, this.Cookies,
					new object[] { });
				var result = new List<Cookie>();
				foreach (var key in table.Keys) {
					var url = $"http://{key.ToString().TrimStart('.')}/";

					foreach (Cookie cookie in this.Cookies.GetCookies(new Uri(url))) {
						result.Add(cookie);
					}
				}

				return result;
			}

			set
			{
				try {
					foreach (var cookie in value) {
						this.cookieContainer.Add(cookie);
					}
				} catch {
					// ignored
				}
			}
		}

		public CookieContainer Cookies
		{
			get => this.cookieContainer;
			set
			{
				var table = (Hashtable) value.GetType().InvokeMember("m_domainTable",
					BindingFlags.NonPublic |
					BindingFlags.GetField |
					BindingFlags.Instance,
					null,
					value,
					new object[] { });
				foreach (var key in table.Keys) {
					var url = string.Format("http://{0}/", key.ToString().TrimStart('.'));

					foreach (Cookie cookie in value.GetCookies(new Uri(url))) {
						this.cookieContainer.Add(cookie);
					}
				}
			}
		}

		public long GetContentLength(string url)
		{
			lock (this.downloadLock) {
				try {
					var request = this.GetWebRequest(new Uri(url));
					if (request == null) {
						return 0; 
					}

					//request.Method = "HEAD";
					this.StartPos = -128;
					this.EndPos = -128;
					var result = request.GetResponse();
					this.StartPos = -1;
					this.EndPos = -1;

					var length = result.ContentLength;
					result.Close();
					return length;
			
				} catch (Exception e) {
					Log.WriteInfo("GetContentLength {0}, {1}", e.Message, url);
					this.StartPos = -1;
					this.EndPos = -1;
				}

				return -1;
			}
		}

		public Stream DownloadPartial(string url, long startPos, long endPos)
		{
			lock (this.downloadLock) {
				try {
					this.StartPos = startPos;
					this.EndPos = endPos;
					var request = this.GetWebRequest(new Uri(url));
					var result = request.GetResponse();
					this.StartPos = -1;
					this.EndPos = -1;
					return result.GetResponseStream();
				} catch (Exception e) {
					Log.WriteInfo("DownloadPartial {0}, {1}", e.Message, url);
					this.StartPos = -1;
					this.EndPos = -1;
				}

				return null;
			}
		}

		public string GetRedirectUrl(string url)
		{
			return (string) this.TryWeb(url, () => {
				if (Uri.TryCreate(url, UriKind.Absolute, out var uri)) {
					var request = this.GetWebRequest(uri);
					var response = this.GetWebResponse(request);
					response.Close();
					return response.ResponseUri.OriginalString;
				}

				return url;
			});
		}

		public object TryWeb(string url, Func<object> action)
		{
			try {
				return action();
			} catch (Exception e) {
				var msg = string.Concat(url, " ", e.Message);
				if (e.InnerException != null) {
					msg = string.Concat(url, " ", e.InnerException.Message);
				}

				if (this.ErrorHandler != null) {
					this.ErrorHandler(msg);
				} else {
					Log.WriteError(msg);
				}
			}

			return null;
		}

		public string PostMultipart(string url, string sData, string boundary)
		{
			var data = this.Encoding.GetBytes(sData);

			return (string) this.TryWeb(url, () => {
				lock (this.downloadLock) {
					this.Encoding = Encoding.UTF8;

					var request = WebRequest.Create(url) as HttpWebRequest;
					if (request == null) {
						return null; 
					}

					request.Headers = this.Headers;
					request.Method = "POST";
					request.ContentType = "multipart/form-data; boundary=" + boundary;
					request.UserAgent = USER_AGENT;
					request.CookieContainer = this.cookieContainer;
					request.ContentLength = data.Length;
					request.KeepAlive = true;
					using (var requestStream = request.GetRequestStream()) {
						requestStream.Write(data, 0, data.Length);
						requestStream.Close();
					}

					using (var response = (HttpWebResponse) request.GetResponse()) {
						this.SuccessHandler();
						using (var resStream = response.GetResponseStream()) {
							var reader = new StreamReader(resStream, Encoding.UTF8);
							return reader.ReadToEnd();
						}
					}
				}
			});
		}
	}

	public enum MultipartPostDataParamType
	{
		Field,
		File
	}

	public class MultipartPostData
	{
		private List<MultipartPostDataParam> m_Params;

		public List<MultipartPostDataParam> Params
		{
			get => this.m_Params;
			set => this.m_Params = value;
		}

		public MultipartPostData()
		{
			this.m_Params = new List<MultipartPostDataParam>();
		}

		public string Boundary { get; set; }

		/// <summary>
		/// Returns the parameters array formatted for multi-part/form data
		/// </summary>
		/// <returns></returns>
		public string GetPostData()
		{
			// Get boundary, default is --AaB03x

			this.Boundary = "----WebKitFormBoundary" + this.RandomString(16);

			var sb = new StringBuilder();
			foreach (var p in this.m_Params) {
				sb.AppendLine("--" + this.Boundary);

				if (p.Type1 == MultipartPostDataParamType.File) {
					sb.AppendLine($"Content-Disposition: file; name=\"{p.Name}\"; filename=\"{p.FileName}\"");
					sb.AppendLine("Content-Type: text/plain");
					sb.AppendLine();
					sb.AppendLine(p.Value);
				} else {
					sb.AppendLine($"Content-Disposition: form-data; name=\"{p.Name}\"");
					sb.AppendLine();
					sb.AppendLine(p.Value);
				}
			}

			sb.AppendLine("--" + this.Boundary + "--");

			return sb.ToString();
		}

		private string RandomString(int size)
		{
			const string input = "ABCDEFGHJIKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
			var builder = new StringBuilder();
			var random = new Random();
			char ch;
			for (var i = 0; i < size; i++) {
				ch = input[random.Next(0, input.Length)];
				builder.Append(ch);
			}

			return builder.ToString();
		}
	}

	public class MultipartPostDataParam
	{
		public MultipartPostDataParam(string name, string value, MultipartPostDataParamType type)
		{
			this.Name = name;
			this.Value = value;
			this.Type1 = type;
		}

		public string Name { get; }
		public string FileName { get; }
		public string Value { get; }
		public MultipartPostDataParamType Type1 { get; }
	}
}