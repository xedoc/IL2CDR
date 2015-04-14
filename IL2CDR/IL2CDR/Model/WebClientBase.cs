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
            private object downloadLock = new object();
            private readonly CookieContainer m_container = new CookieContainer();
            private const string userAgent = "Mozilla/5.0 (Windows NT 6.0; WOW64; rv:14.0) Gecko/20100101 Firefox/14.0.1";
            private Dictionary<ContentType, string> contentTypes = new Dictionary<ContentType,string>() {
                 {ContentType.JsonUTF8, "application/json; charset=UTF-8"},
                 {ContentType.UrlEncodedUTF8, "application/x-www-form-urlencoded; charset=UTF-8"},
                 {ContentType.UrlEncoded, "application/x-www-form-urlencoded"},
                 {ContentType.Multipart, "multipart/form-data"},
                 
            };
            public WebClientBase()
            {
                ServicePointManager.ServerCertificateValidationCallback =
                    new RemoteCertificateValidationCallback(
                        delegate
                        { return true; }
                    );
                ServicePointManager.DefaultConnectionLimit = 25;
                ServicePointManager.Expect100Continue = false;
                ServicePointManager.UseNagleAlgorithm = false;
                ServicePointManager.DnsRefreshTimeout = 60;
                ServicePointManager.EnableDnsRoundRobin = true;
                KeepAlive = true;
                IsAnonymous = false;
                ErrorHandler = (error) => {
                    Log.WriteError(error);
                };
                StartPos = -1;
                EndPos = -1;
                Timeout = 5000;
                Proxy = null;
                SuccessHandler = () => { };
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
                lock(downloadLock )
                {
                    WebRequest request = base.GetWebRequest(address);

                    HttpWebRequest webRequest = request as HttpWebRequest;
                    if (webRequest != null)
                    {
                        if (KeepAlive)
                        {
                            webRequest.ProtocolVersion = HttpVersion.Version10;
                            webRequest.KeepAlive = true;
                            var sp = webRequest.ServicePoint;
                            var prop = sp.GetType().GetProperty("HttpBehaviour", BindingFlags.Instance | BindingFlags.NonPublic);
                            prop.SetValue(sp, (byte)0, null);
                        }
                        else
                        {
                            webRequest.KeepAlive = false;
                        }

                        if (StartPos != -1 && EndPos != -1)
                        {
                            webRequest.AddRange(StartPos, EndPos);
                        }
                        webRequest.Timeout = Timeout;
                        if (!IsAnonymous)
                            webRequest.CookieContainer = m_container;

                        webRequest.UserAgent = userAgent;
                        webRequest.Proxy = null;
                        webRequest.CachePolicy = new HttpRequestCachePolicy(HttpRequestCacheLevel.NoCacheNoStore);
                        webRequest.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;
                    }
                    return request;
                }
            }
            public String Download(String url)
            {
                return(string)TryWeb(url,() => {                
                    lock (downloadLock)
                    {
                        SuccessHandler();
                        Encoding = Encoding.UTF8;
                        return DownloadString(new Uri(url));
                    }
                });
            }
            public byte[] DownloadToByteArray( String url )
            {
                try
                {
                    lock (downloadLock)
                    {
                        SuccessHandler();
                        return DownloadData(new Uri(url));
                    }
                }
                catch
                {
                    ErrorHandler(String.Format("Error downloading to byte array from {0}", url));
                }
                return new byte[] {};
            }
            public MemoryStream DownloadToMemoryStream( String url, string method = "GET" )
            {
                try
                {
                    lock (downloadLock)
                    {
                        var request = GetWebRequest(new Uri(url));
                        request.Method = method;

                        var response = GetWebResponse(request);

                        if (SuccessHandler != null)
                            SuccessHandler();
                        
                        MemoryStream memoryStream = new MemoryStream();
                        
                        var stream = response.GetResponseStream();
                        
                        if( stream.CanRead )
                        {
                            byte[] buffer = new byte[4096];

                            int bytesRead = 0;

                            do
                            {
                                bytesRead = stream.Read(buffer, 0, buffer.Length);
                                memoryStream.Write(buffer, 0, bytesRead);
                            } while( bytesRead > 0);

                            memoryStream.Position = 0;
                            response.Close();
                            return memoryStream;
                        }
                    }
                }
                catch
                {
                    ErrorHandler(String.Format("Error downloading {0} to memorystream", url));
                }
                return null;

            }
            public Stream DownloadToStream(String url, bool cache = false)
            {
                try
                {
                    lock (downloadLock)
                    {
                        var request = GetWebRequest(new Uri(url));
                        if( cache )
                        {
                            request.CachePolicy = new HttpRequestCachePolicy(HttpRequestCacheLevel.CacheIfAvailable);
                        }
                        var response = GetWebResponse(request);
                        
                        if( SuccessHandler != null )
                            SuccessHandler();
                        
                        return response.GetResponseStream();
                    }
                }
                catch
                {
                    ErrorHandler(String.Format("Error downloading {0} to stream", url));
                }
                return null;
            }
            public byte[] GZipBytes(string data)
            {                
                using (System.IO.MemoryStream outputStream = new System.IO.MemoryStream())
                using (GZipStream gzip = new GZipStream(outputStream, System.IO.Compression.CompressionMode.Compress))
                {
                    StreamWriter writer = new StreamWriter(gzip);
                    writer.Write(data);
                    writer.Flush();
                    gzip.Flush();
                    gzip.Close();

                    return outputStream.ToArray();
                }
            }
            public void CopyTo(Stream src, Stream dest)
            {
                byte[] bytes = new byte[4096];

                int cnt;

                while ((cnt = src.Read(bytes, 0, bytes.Length)) != 0)
                {
                    dest.Write(bytes, 0, cnt);
                }
            }
            public string GUnzip(byte[] bytes)
            {
                using (var input = new MemoryStream(bytes))
                using (var output = new MemoryStream())
                {
                    using (var gs = new GZipStream(input, CompressionMode.Decompress))
                    {
                        CopyTo(gs, output);
                    }

                    return Encoding.UTF8.GetString(output.ToArray());
                }
            }
            public string UploadCompressed(string url, string args)
            {
                return (string)TryWeb(url, () =>
                {
                    lock (downloadLock)
                    {
                        //this.Headers.Add("Content-Encoding", "gzip");
                        var result = Encoding.UTF8.GetString(UploadData(url, "POST", GZipBytes(args)));
                        SuccessHandler();
                        return result;
                    }
                });

            }
            public String Upload(string url, byte[] args)
            {
                return (string)TryWeb(url, () =>
                {
                    lock (downloadLock)
                    {
                        //this.Headers.Add("Content-Encoding", "gzip");
                        var result = Encoding.UTF8.GetString(UploadData(url, "POST", args));
                        SuccessHandler();
                        return result;
                    }
                });
            }
            public String Upload(string url, string args)
            {
                return (string)TryWeb(url, () =>
                {
                    lock (downloadLock)
                    {
                        var result = base.UploadString(url, "POST", args);
                        SuccessHandler();
                        return result;
                    }
                });
            }

            public Stream PutStream(string url, Stream stream)
            {
                try
                {
                    var request = (HttpWebRequest)WebRequest.Create(url);
                    request.Method = "PUT";
                    if (stream != null)
                    {
                        request.ContentLength = stream.Length;
                        Stream dataStream = request.GetRequestStream();
                        stream.CopyTo(dataStream);
                        stream.Flush();
                        dataStream.Flush();
                        dataStream.Close();
                    }

                    HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                    return response.GetResponseStream();
                }
                catch ( Exception e)
                {
                    Log.WriteInfo("PutStream: {0}", e.Message);
                    return null;
                }
            }
            public void RequestPatchOptions(string url)
            {
                try
                {
                    var request = (HttpWebRequest)WebRequest.Create(url);
                    request.Method = "OPTIONS";

                    HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                    if (response != null)
                        return;
                }
                catch( Exception e)
                {
                    Log.WriteInfo("RequestPatchOptions {0}, {1}", e.Message, url);
                }
            }

            public Stream PatchStream(string url, Stream stream)
            {
                try
                {
                    var request = (HttpWebRequest)WebRequest.Create(url);
                    request.Method = "PATCH";
                    request.Headers = Headers;
                    request.ContentType = @"application/json;charset=UTF-8";

                    if (stream != null)
                    {
                        request.ContentLength = stream.Length;
                        Stream dataStream = request.GetRequestStream();
                        stream.CopyTo(dataStream);
                        stream.Flush();
                        dataStream.Flush();
                        dataStream.Close();
                    }

                    HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                    return response.GetResponseStream();
                }
                catch( Exception e )
                {
                    Log.WriteInfo("PatchStream {0}, {1}", e.Message, url);
                    return null;
                }
            }


            public void SetCookie(string name, string value, string domain)
            {
                if (name == null || value == null)
                    return;
                m_container.Capacity += 1;
                m_container.Add(new Cookie(name, value, "/", domain));
            }


            public ContentType ContentType
            {
                set { this.Headers[HttpRequestHeader.ContentType] = contentTypes[value]; }
            }

            public string CookieValue(string name, string url)
            {
                var coll = m_container.GetCookies(new Uri(url));
                if (coll == null || coll[name] == null)
                    return String.Empty;

                return coll[name].Value;
            }
            public List<Cookie> CookiesTable
            {
                get
                {
                    Hashtable table = (Hashtable)Cookies.GetType().InvokeMember("m_domainTable",
                                                                 BindingFlags.NonPublic |
                                                                 BindingFlags.GetField |
                                                                 BindingFlags.Instance,
                                                                 null,
                                                                 Cookies,
                                                                 new object[] { });
                    List<Cookie> result = new List<Cookie>();
                    foreach (var key in table.Keys)
                    {
                        var url = String.Format("http://{0}/", key.ToString().TrimStart('.'));

                        foreach (Cookie cookie in Cookies.GetCookies(new Uri(url)))
                        {                  
                            result.Add(cookie);
                        }
                    }

                    return result;
                }

                set
                {
                    try
                    {
                        foreach (Cookie cookie in value)
                        {
                            m_container.Add(cookie);
                        }
                    }
                    catch { }
                }
            }

            public CookieContainer Cookies
            {
                get { return m_container; }
                set
                {
                    Hashtable table = (Hashtable)value.GetType().InvokeMember("m_domainTable",
                                                                 BindingFlags.NonPublic |
                                                                 BindingFlags.GetField |
                                                                 BindingFlags.Instance,
                                                                 null,
                                                                 value,
                                                                 new object[] { });
                    foreach (var key in table.Keys)
                    {
                        var url = String.Format("http://{0}/", key.ToString().TrimStart('.'));

                        foreach (Cookie cookie in value.GetCookies(new Uri(url)))
                        {
                            m_container.Add(cookie);
                        }
                    }


                }
            }        
            public long GetContentLength( string url )
            {                
                lock(downloadLock)
                {
                    try
                    {
                        WebRequest request = GetWebRequest(new Uri(url));
                        //request.Method = "HEAD";
                        StartPos = -128;
                        EndPos = -128;
                        WebResponse result = request.GetResponse();
                        StartPos = -1;
                        EndPos = -1;
                        if (result != null)
                        {
                            var length = result.ContentLength;
                            result.Close();
                            return length;
                        }
                    }
                    catch( Exception e )
                    {
                        Log.WriteInfo("GetContentLength {0}, {1}", e.Message, url);
                        StartPos = -1;
                        EndPos = -1;
                    }
                    return -1;
                }
            }

            public Stream DownloadPartial( string url, long startPos, long endPos )
            {
                lock (downloadLock)
                {
                    try
                    {
                        StartPos = startPos;
                        EndPos = endPos;
                        WebRequest request = GetWebRequest(new Uri(url));
                        WebResponse result = request.GetResponse();
                        StartPos = -1;
                        EndPos = -1;
                        return result.GetResponseStream();
                    }
                    catch(Exception e)
                    {
                        Log.WriteInfo("DownloadPartial {0}, {1}", e.Message, url);
                        StartPos = -1;
                        EndPos = -1;
                    }
                    return null;
                }
            }

            public string GetRedirectUrl( string url )
            {
                return (string)TryWeb(url, () => {
                    Uri uri;
                    if (Uri.TryCreate(url, UriKind.Absolute, out uri))
                    {
                        var request = GetWebRequest(uri);
                        var response = GetWebResponse(request);
                        response.Close();
                        return response.ResponseUri.OriginalString;


                    }
                    return url;
                });
            }

            public object TryWeb(string url, Func<object> action)
            {
                try
                {
                    return action();
                }
                catch ( Exception e )
                {
                    var msg = String.Concat( url, " ", e.Message);
                    if (e.InnerException != null)
                        msg = String.Concat( url, " ", e.InnerException.Message);

                    if (ErrorHandler != null)
                        ErrorHandler(msg);
                    else
                        Log.WriteError(msg);

                }
                return null;
            }

            public string PostMultipart(string url, string sData, string boundary)
            {
                byte[] data = Encoding.GetBytes(sData);

                return (string)TryWeb(url, () =>
                {
                    lock (downloadLock)
                    {
                        Encoding = Encoding.UTF8;

                        HttpWebRequest request = WebRequest.Create(url) as HttpWebRequest;
                        request.Headers = Headers;
                        request.Method = "POST";
                        request.ContentType = "multipart/form-data; boundary=" + boundary;
                        request.UserAgent = userAgent;
                        request.CookieContainer = m_container;
                        request.ContentLength = data.Length;
                        request.KeepAlive = true;
                        using (var requestStream = request.GetRequestStream())
                        {
                            requestStream.Write(data, 0, data.Length);
                            requestStream.Close();
                        }

                        using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                        {
                            SuccessHandler();
                            using (Stream resStream = response.GetResponseStream())
                            {
                                StreamReader reader = new StreamReader(resStream, Encoding.UTF8);
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
            get { return m_Params; }
            set { m_Params = value; }
        }

        public MultipartPostData()
        {
            m_Params = new List<MultipartPostDataParam>();

        }

        public String Boundary
        {
            get;
            set;
        }
        /// <summary>
        /// Returns the parameters array formatted for multi-part/form data
        /// </summary>
        /// <returns></returns>
        public string GetPostData()
        {
            // Get boundary, default is --AaB03x

            Boundary = "----WebKitFormBoundary" + RandomString(16);

            StringBuilder sb = new StringBuilder();
            foreach (MultipartPostDataParam p in m_Params)
            {
                sb.AppendLine("--" + Boundary);

                if (p.Type == MultipartPostDataParamType.File)
                {
                    sb.AppendLine(string.Format("Content-Disposition: file; name=\"{0}\"; filename=\"{1}\"", p.Name, p.FileName));
                    sb.AppendLine("Content-Type: text/plain");
                    sb.AppendLine();
                    sb.AppendLine(p.Value);
                }
                else
                {
                    sb.AppendLine(string.Format("Content-Disposition: form-data; name=\"{0}\"", p.Name));
                    sb.AppendLine();
                    sb.AppendLine(p.Value);
                }
            }

            sb.AppendLine("--" + Boundary + "--");

            return sb.ToString();
        }
        private string RandomString(int Size)
        {
            string input = "ABCDEFGHJIKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            StringBuilder builder = new StringBuilder();
            Random random = new Random();
            char ch;
            for (int i = 0; i < Size; i++)
            {
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
            Name = name;
            Value = value;
            Type = type;
        }

        public string Name;
        public string FileName;
        public string Value;
        public MultipartPostDataParamType Type;
    }

}