using System;
using System.IO;
using System.Net;
using System.Text;

namespace Budong.Common.Net
{
    /// <summary>
    /// 网络请求方法类
    /// </summary>
    public class WebClient
    {
        #region 私有变量集合
        private CookieContainer cookies = new CookieContainer();
        private Encoding encoding = Encoding.Default;
        private long total = 0;
        private long position = 0;
        private bool stop = false;
        #endregion

        #region 公共属性集合
        /// <summary>
        /// 流总大小
        /// </summary>
        public long Total
        {
            get
            {
                return this.total;
            }
        }
        /// <summary>
        /// 当前进度
        /// </summary>
        public long Position
        {
            get
            {
                return this.position;
            }
        }
        /// <summary>
        /// 中止当前进程
        /// </summary>
        public void Stop()
        {
            this.stop = true;
        }
        #endregion

        #region 公共方法
        /// <summary>
        /// 组合 Url 链接
        /// </summary>
        /// <param name="host">string 域名</param>
        /// <param name="url">string 链接</param>
        /// <returns>string 返回绝对 Url</returns>
        public string GroupUrl(string host, string url)
        {
            if (url.StartsWith("http://", true, null) || url.StartsWith("https://", true, null) || url.StartsWith("ftp:", true, null))
            {
                try
                {
                    return new Uri(url).AbsoluteUri;
                }
                catch
                {
                    return String.Empty;
                }
            }
            Uri hostUri = new Uri(host);
            if (url.StartsWith("?", true, null))
            {
                return hostUri.AbsoluteUri.Replace(hostUri.Query, "") + url;
            }
            else
            {
                Uri uri;
                Uri.TryCreate(hostUri, new Uri(url), out uri);
                return uri.AbsoluteUri;
            }
        }
        /// <summary>
        /// 从指定 Url 地址下载文件至本地
        /// </summary>
        /// <param name="url">string Url 地址</param>
        /// <param name="fileName">string 文件保存路径</param>
        public void Download(string url, string fileName)
        {
            this.stop = false;
            HttpWebRequest webRequest = (HttpWebRequest)WebRequest.Create(new Uri(url));
            webRequest.KeepAlive = true;
            webRequest.Timeout = 60000;
            webRequest.Method = "GET";
            webRequest.UserAgent = "Mozilla/4.0 (compatible; MSIE 6.0; Windows NT 5.1; SV1; Maxthon)"; ;
            webRequest.CookieContainer = this.cookies;
            WebResponse webResponse = null;
            try
            {
                webResponse = webRequest.GetResponse();
            }
            catch (WebException e)
            {
                if (e.Status == WebExceptionStatus.ProtocolError)
                {
                    webResponse = (HttpWebResponse)e.Response;
                }
            }
            if (webResponse != null)
            {
                this.total = webResponse.ContentLength;
                Stream webStream = webResponse.GetResponseStream();
                FileStream webFile = new FileStream(fileName, FileMode.Append);
                byte[] buffer = new byte[1024];
                long seek = webStream.Read(buffer, 0, buffer.Length);
                while (seek > 0 && !this.stop)
                {
                    webFile.Write(buffer, 0, (int)seek);
                    seek = webStream.Read(buffer, 0, buffer.Length);
                    this.position += seek;
                }
                webFile.Close();
                webFile.Dispose();
                webResponse.Close();
            }
            webRequest.Abort();
        }
        #endregion

    }
}
