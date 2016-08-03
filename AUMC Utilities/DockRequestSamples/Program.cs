using DockAPI_Routines;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace DockRequestSamples
{
    public class Program {
        private static void Main(string[] args) {
            try {
                var options = new Dictionary<string, object> {{"last_name", "Comer"}, {"city", "Acworth"}};
                //var options = new Dictionary<string, object> { { "id", "133" }};
                //var doc = DockRequest.FindProfiles(options);
                var doc = DockRequest.ListService("significant_event");
                Console.WriteLine(doc.ToString());
                /*var ind = doc.Descendants("individuals").FirstOrDefault();
                if (ind != null) {
                    var people = ind.Elements("individual").Select(elm => new DockAPI_Routines.Person(elm)).ToList();
                    Console.WriteLine("Located " + ind.Attribute("count").Value + " records from CCB database.");
                    foreach (var person in people) {
                        Console.WriteLine($"\t{person}");
                    }
                }*/
            } catch (Exception ex) {
                Console.WriteLine("Exception during request: " + ex);
            }
            Console.ReadLine();
        }
    }
}
