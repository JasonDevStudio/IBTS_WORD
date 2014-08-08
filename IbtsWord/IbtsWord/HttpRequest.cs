using System;
using System.Net;
using System.Text;
using System.IO;
using System.Threading;
using System.Web;
using System.Text.RegularExpressions;
using System.Collections;

namespace IbtsWord
{
    class HttpRequest
    {
        private static HttpRequest httpRequest;

        public static HttpRequest getInstance()
        {
            if (httpRequest == null)
            {
                httpRequest = new HttpRequest();
            }

            return httpRequest;
        }

        private CookieContainer cookieCon;
        private string cookieHeader;

        public string doPost(string url, Hashtable param)
        {
            HttpWebResponse response = null;
            string result = " ";
 
            try
            {
                Encoding encoding = System.Text.Encoding.UTF8;
                Random rd = new Random((int)System.DateTime.Now.Ticks);

                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
                request.Method = "POST";

                // Post data 
                //string postData = string.Format("LoginName={0}&LoginPwd={1} ", userName, password);
                string postData = "";
                foreach(DictionaryEntry de in param) 
                {
                    if (!postData.Equals(""))
                    {
                        postData = postData + "&";
                    }
                    postData = postData + de.Key + "=" + de.Value;
                }

                byte[] buffer = encoding.GetBytes(postData);

                // Set the content type to a FORM 
                request.ContentType = "application/x-www-form-urlencoded ";

                // Get length of content 
                request.ContentLength = buffer.Length;

                //request.AllowAutoRedirect = false;
                if (cookieCon == null)
                {
                    cookieCon = new CookieContainer();
                }
                else
                {
                    cookieCon = new CookieContainer();
                    cookieCon.SetCookies(new Uri(url), cookieHeader);
                }
                request.CookieContainer = cookieCon;

                // Get request stream 
                Stream newStream = request.GetRequestStream();

                // Send the data. 
                newStream.Write(buffer, 0, buffer.Length);

                // Close stream 
                newStream.Close();

                response = (HttpWebResponse)request.GetResponse();
                
                if (cookieHeader == null)
                {
                    cookieHeader = request.CookieContainer.GetCookieHeader(new Uri(url));
                }

                Stream ReceiveStream = response.GetResponseStream();
                Encoding encode = System.Text.Encoding.Default;
                StreamReader sr = new StreamReader(ReceiveStream, encode);
                Char[] read = new Char[256];
                int count = sr.Read(read, 0, 256);
                while (count > 0)
                {
                    String str = new String(read, 0, count);
                    result += str;
                    count = sr.Read(read, 0, 256);
                }
            }
            catch (Exception e)
            {
                result = e.ToString();
            }
            finally
            {
                if (response != null)
                {
                    response.Close();
                }
            }

            return result;
        }

        public Stream doPostS(string url, Hashtable param)
        {
            HttpWebResponse response = null;
            string result = " ";

            try
            {
                Encoding encoding = System.Text.Encoding.UTF8;
                Random rd = new Random((int)System.DateTime.Now.Ticks);

                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
                request.Method = "POST";

                // Post data 
                //string postData = string.Format("LoginName={0}&LoginPwd={1} ", userName, password);
                string postData = "";
                foreach (DictionaryEntry de in param)
                {
                    if (!postData.Equals(""))
                    {
                        postData = postData + "&";
                    }
                    postData = postData + de.Key + "=" + de.Value;
                }

                byte[] buffer = encoding.GetBytes(postData);

                // Set the content type to a FORM 
                request.ContentType = "application/x-www-form-urlencoded ";

                // Get length of content 
                request.ContentLength = buffer.Length;

                //request.AllowAutoRedirect = false;
                if (cookieCon == null)
                {
                    cookieCon = new CookieContainer();
                }
                else
                {
                    cookieCon = new CookieContainer();
                    cookieCon.SetCookies(new Uri(url), cookieHeader);
                }
                request.CookieContainer = cookieCon;

                // Get request stream 
                Stream newStream = request.GetRequestStream();

                // Send the data. 
                newStream.Write(buffer, 0, buffer.Length);

                // Close stream 
                newStream.Close();

                response = (HttpWebResponse)request.GetResponse();

                if (cookieHeader == null)
                {
                    cookieHeader = request.CookieContainer.GetCookieHeader(new Uri(url));
                }

                Stream ReceiveStream = response.GetResponseStream();

                return ReceiveStream;
            }
            catch (Exception e)
            {
                result = e.ToString();
            }
            finally
            {
                //if (response != null)
                //{
                //    response.Close();
                //}
            }

            return null;
        }
    }
}
