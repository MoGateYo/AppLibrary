using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Web.Http;

namespace GetVNIndex.Controllers
{
    public class HomeController : ApiController
    {
        // GET: api/Home
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET: api/Home/5
        public IHttpActionResult Get(int id)
        {

            string Url = "https://th.investing.com/indices/vn-historical-data";
            CookieContainer cookieJar = new CookieContainer();
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(Url);
            request.CookieContainer = cookieJar;
            request.Accept = @"text/html, application/xhtml+xml, */*";
            request.Referer = @"https://th.investing.com/indices/vn-historical-data";
            request.Headers.Add("Accept-Language", "en-GB");
            request.UserAgent = @"Mozilla/5.0 (compatible; MSIE 10.0; Windows NT 6.2; Trident/6.0)";
            request.Host = @"www.investing.com";
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            StreamReader reader = new StreamReader(response.GetResponseStream());
            string responseFromServer = reader.ReadToEnd();

            reader.Close();
            response.Close();


            int pFrom = responseFromServer.IndexOf("<table class=\"genTbl");
            int pTo = responseFromServer.LastIndexOf("</table>\n\n");
            string s = responseFromServer.Substring(pFrom, pTo - pFrom);
            string s1 = s.Replace("\n","");
            s1=s1.Replace(@" ","");

            pFrom = s1.IndexOf("<tbody><tr>");
            pTo = s1.LastIndexOf("</tbody>");
            string s2 = s1.Substring(pFrom, pTo - pFrom);
            string rep = s2.Replace("</tr><tr>", ":");
            Regex abc = new Regex(@"\<([^\>]+)\>");

            rep = abc.Replace(rep, ":");
            rep = rep.Replace(":::", "|");
            string[] rep2 = rep.Split('|');
            
            List<vn_index> vnlst = new List<vn_index>();
            foreach (string r in rep2.Skip(1))
            {
                string rep3 = r.Replace("::", "|");
                string[] sl = rep3.Split('|');
                DateTime dt = DateTime.Parse(sl[0], new CultureInfo("en-US"));
                DateTime dt_now = DateTime.Now.AddDays(-1).Date;
                if (dt == dt_now)
                {
                    vn_index vn = new vn_index
                    {
                        date = DateTime.Parse(sl[0], new CultureInfo("en-US")),
                        close = Convert.ToDecimal(sl[1]),
                        open = Convert.ToDecimal(sl[2]),
                        hight = Convert.ToDecimal(sl[3]),
                        low = Convert.ToDecimal(sl[4]),
                        volume = Convert.ToDecimal(sl[5].Replace("K", ""))
                    };
                    vnlst.Add(vn);
                }

            }

            return Json(vnlst);
        }

        public class vn_index
        {
            public DateTime date { get; set; }
            public decimal open { get; set; }
            public decimal hight { get; set; }
            public decimal low { get; set; }
            public decimal close { get; set; }
            public decimal volume { get; set; }
        }

        // POST: api/Home
        public void Post([FromBody]string value)
        {
        }

        // PUT: api/Home/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE: api/Home/5
        public void Delete(int id)
        {
        }
    }
}
