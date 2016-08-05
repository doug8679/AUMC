using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace DockAPI_Routines
{
    public enum DockEvents {
        HighSchoolGraduation = 1,
        CollegeGraduation=2,
        MovedOutOfArea=4,
        DeathOfALovedOne=6,
        Confirmatin = 7,
        Baptism = 8,
        FirstContact=9,
        FiftiethAnniversary=10,
        BaptismReaffirmation=11
    }

	public class DockRequest {
		private static string userName = "dbraxton";
		private static string passwd = "Ell@Fay3";
		private static ManualResetEvent allDone = new ManualResetEvent(false);

		private static string baseURL = "https://acworthumc.ccbchurch.com/api.php?srv=";
		
		private static void AddRequestAuthorizationHeader(WebRequest request) {
			request.Headers.Add("Authorization", "Basic " + System.Convert.ToBase64String(System.Text.Encoding.GetEncoding("ISO-8859-1").GetBytes(userName + ":" + passwd)));
		}

		private static bool BuildSearchQuery(Dictionary<string, object> options, out string query) {
			query = options.Aggregate("", (current, pair) => current + ("&" + pair.Key + "=" + pair.Value.ToString()));
		    return string.IsNullOrWhiteSpace(query);
		}
		
		public static XDocument ListService(string service) {
			XDocument result = null;
			var request = WebRequest.Create(baseURL + service + "_list");
			AddRequestAuthorizationHeader(request);
			try {
				using(var response =request.GetResponse()) {
					result = XDocument.Load(response.GetResponseStream());
				}
			} catch (Exception ex) {
				throw new Exception("Exception during request.", ex);
			}
			return result;
		}

		/// <summary>
		///	Retrieves a single person from the CCB database by searching on firt, last or both names.
		/// </summary>		
		///
		/// <remarks>
		/// https://acworthumc.ccbchurch.com/api.php?srv=individual_search
		/// </remarks>
		public static XDocument FindProfiles(Dictionary<string, object> options) {
			XDocument result = null;
			string query;
			bool hasQuery = BuildSearchQuery(options, out query);
			var request = WebRequest.Create(baseURL + "individual_search" + query);
			AddRequestAuthorizationHeader(request);
			try {
				using(var response =request.GetResponse()) {
					result = XDocument.Load(response.GetResponseStream());
				}
			} catch (Exception ex) {
				throw new Exception("Exception during request.", ex);
			}
			return result;
		}

		public static XDocument GetAllPeople() {
			XDocument result = null;
			//https://yourchurch.ccbchurch.com/api.php?srv=individual_profiles
			var request = WebRequest.Create(baseURL + "individual_profiles");
			AddRequestAuthorizationHeader(request);
			var state = new RequestState {Request = request};
			Console.WriteLine("Request intialized, submitting...");
			try {
				IAsyncResult r = request.BeginGetResponse(ResponseCallback, state);
				Console.WriteLine("Waiting for request to complete...");
				allDone.WaitOne();
				result = XDocument.Load(new StringReader(state.RequestData.ToString()));
				/*using (var response = request.GetResponse())
				{
					result = XDocument.Load(response.GetResponseStream());
				}*/
			} catch (Exception ex) {
				throw new Exception("Exception during request.", ex);
			}
			return result;

		}

        /// <summary>
        /// Creates a significant event entry for an individual.
        /// </summary>
        /// <param name="id">The individual profile id to update.</param>
        /// <param name="eventId">The event id to attach to the profile.</param>
        /// <param name="date">The value for the event date.</param>
        /// <returns></returns>
	    public static XDocument AddIndividualSignificantEvent(int id, int eventId, DateTime date) {
            XDocument result = null;
            var request = WebRequest.Create($"{baseURL}add_individual_significant_event&event_id={eventId}&id={id}&date={date:yyyy-MM-dd}");
            AddRequestAuthorizationHeader(request);
            try {
                using (var response = request.GetResponse()) {
                    result = XDocument.Load(response.GetResponseStream());
                }
            } catch (Exception ex) {
                throw new Exception("Exception during request.",ex);
            }
            return result;
        }
	    public static XDocument SignificantEvents(Dictionary<string, object> options) {
            XDocument result = null;
            string query;
            bool hasQuery = BuildSearchQuery(options, out query);
            var request = WebRequest.Create(baseURL + "individual_significant_events" + query);
            AddRequestAuthorizationHeader(request);
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

		private static void ResponseCallback(IAsyncResult ar) {
			Console.WriteLine("Response received, processing...");
			RequestState state = ar.AsyncState as RequestState;
			if (state != null) {
				WebRequest request = state.Request;
				WebResponse response = request.EndGetResponse(ar);
				Stream ResponseStream = response.GetResponseStream();
				state.ResponseStream = ResponseStream;
				Console.WriteLine("Beginning read of response stream...");
				IAsyncResult iarRead = ResponseStream.BeginRead(state.BufferRead, 0, RequestState.BUFFER_SIZE, ReadCallback, state);
			}
		}

		private static void ReadCallback(IAsyncResult ar) {
			var sw = Stopwatch.StartNew();
			Console.WriteLine("Read callabck processing data...");
			// Get the RequestState object from AsyncResult.
			RequestState state = (RequestState)ar.AsyncState;

			// Retrieve the ResponseStream that was set in RespCallback. 
			Stream responseStream = state.ResponseStream;

			// Read rs.BufferRead to verify that it contains data. 
			int read = responseStream.EndRead(ar);
			if (read > 0) {
				// Prepare a Char array buffer for converting to Unicode.
				Char[] charBuffer = new Char[RequestState.BUFFER_SIZE];

				// Convert byte stream to Char array and then to String.
				// len contains the number of characters converted to Unicode.
				int len = state.StreamDecode.GetChars(state.BufferRead, 0, read, charBuffer, 0);
				Console.WriteLine($"Read {len} bytes of response into buffer...");

				String str = new String(charBuffer, 0, len);

				// Append the recently read data to the RequestData stringbuilder
				// object contained in RequestState.
				state.RequestData.Append(Encoding.ASCII.GetString(state.BufferRead, 0, read));
				Console.WriteLine("Repsonse data added to buffer, continuing to read...");

				// Continue reading data until 
				// responseStream.EndRead returns –1.
				IAsyncResult result = responseStream.BeginRead(state.BufferRead, 0, RequestState.BUFFER_SIZE, ReadCallback, state);
			} else {
				if (state.RequestData.Length > 0) {
					Console.WriteLine("Response received, completing request process and closign stream...");
					//  Display data to the console.
					string strContent;
					strContent = state.RequestData.ToString();
				}
				// Close down the response stream.
				responseStream.Close();
				// Set the ManualResetEvent so the main thread can exit.
				allDone.Set();
			}
			sw.Stop();
			Console.WriteLine($"ReadCallaback processed in {sw.ElapsedMilliseconds} milliseconds.");
			return;
		}

	    public static List<Group> GroupSearch(Dictionary<string, object> options) {
            List<Group> result = new List<Group>();
            string query;
            bool hasQuery = BuildSearchQuery(options, out query);
            var request = WebRequest.Create(baseURL + "group_search" + query);
            AddRequestAuthorizationHeader(request);
	        try {
	            using (var response = request.GetResponse()) {
	                var doc = XDocument.Load(response.GetResponseStream());
	                var groups = doc.Descendants("items").FirstOrDefault();
	                if (groups != null) {
	                    foreach (var group in groups.Elements("item")) {
	                        result.Add(new Group(group));
	                    }
                        result.Sort();
                    }
	            }
	        } catch (Exception ex) {
	            throw new Exception("Exception during request.", ex);
	        }
	        return result;
        }

	    public static List<Group> GetAllGroups(bool participants = false) {
	        List<Group> result = new List<Group>();
	        var request = WebRequest.Create(baseURL + "group_profiles&include_participants=" + participants);
	        AddRequestAuthorizationHeader(request);
	        try {
	            using (var response = request.GetResponse()) {
	                var doc = XDocument.Load(response.GetResponseStream());
	                var groups = doc.Descendants("groups").FirstOrDefault();
	                if (groups != null) {
	                    foreach (var group in groups.Elements("group")) {
	                        result.Add(new Group(group));
	                    }
                        result.Sort();
                    }
	            }
	        } catch (Exception ex) {
	            throw new Exception("Exception during request.", ex);
	        }
	        return result;
	    }

	    public static object GetEventsFromLink(string link) {
	        var request = WebRequest.Create(link.Replace("webcal", "http"));
	        try {
	            using (var response = request.GetResponse()) {
	                using (var reader = new StreamReader(response.GetResponseStream())) {
	                    string line;
	                    while ((line = reader.ReadLine()) != null) {
	                        Console.WriteLine(line);
	                    }
	                }
	            }
	        } catch (Exception ex) {
	            Console.WriteLine(ex.ToString());
	        }
	        return null;
	    }
	}

	public class RequestState {
		public const int BUFFER_SIZE = 65536;
		public StringBuilder RequestData;
		public byte[] BufferRead;
		public WebRequest Request;
		public Stream ResponseStream;
		public Decoder StreamDecode = Encoding.UTF8.GetDecoder();

		public RequestState() {
			BufferRead = new byte[BUFFER_SIZE];
			RequestData = new StringBuilder(string.Empty);
			Request = null;
			ResponseStream = null;
		}
	}
}
