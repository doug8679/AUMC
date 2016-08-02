using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace DockAPI_Routines
{
    public class DockRequest {
        private HttpClient client;

        public DockRequest() {
            client = new HttpClient();
            var requestContent = new FormUrlEncodedContent(new[] {new KeyValuePair<string, string>(), });
        }
    }
}
