using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace DockAPI_Routines
{
    public class DockRequest {
        private static string userName = "dbraxton";
        private static string passwd = "Ell@Fay3";

        private static string baseURL = "https://acworthumc.ccbchurch.com/api.php?srv=";

        public static XDocument ListService(string service)
        {
            XDocument result = null;
            var request = WebRequest.Create(baseURL + service + "_list");
            request.Headers.Add("Authorization", "Basic " + System.Convert.ToBase64String(System.Text.Encoding.GetEncoding("ISO-8859-1").GetBytes(userName + ":" + passwd)));
            try
            {
                using(var response =request.GetResponse()) {
                    result = XDocument.Load(response.GetResponseStream());
                }
            } catch (Exception ex)
            {
                throw new Exception("Exception during request.", ex);
            }
            return result;
        }

        public static XDocument GetPerson(string fName, string lName)
        {
            XDocument result = null;
            //https://yourchurch.ccbchurch.com/api.php?srv=individual_search&last_name=Bob
            var request = WebRequest.Create(baseURL + "individual_search&first_name=" + fName + "&last_name=" + lName);
            request.Headers.Add("Authorization", "Basic " + System.Convert.ToBase64String(System.Text.Encoding.GetEncoding("ISO-8859-1").GetBytes(userName + ":" + passwd)));
            try
            {
                using(var response =request.GetResponse()) {
                    result = XDocument.Load(response.GetResponseStream());
                }
            } catch (Exception ex)
            {
                throw new Exception("Exception during request.", ex);
            }
            return result;
        }

        public static XDocument GetAllPeople()
        {
            XDocument result = null;
            //https://yourchurch.ccbchurch.com/api.php?srv=individual_profiles
            var request = WebRequest.Create(baseURL + "individual_profiles&per_page=2");
            request.Headers.Add("Authorization", "Basic " + System.Convert.ToBase64String(System.Text.Encoding.GetEncoding("ISO-8859-1").GetBytes(userName + ":" + passwd)));
            try
            {
                using (var response = request.GetResponse())
                {
                    result = XDocument.Load(response.GetResponseStream());
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Exception during request.", ex);
            }
            return result;

        }
    }
}
