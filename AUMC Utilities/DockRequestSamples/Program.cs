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
    class Program {
        static void Main(string[] args) {
            try {
                XDocument doc = DockRequest.GetAllPeople();
                var ind = doc.Descendants("individuals").FirstOrDefault();
                var people = new List<Person>();
                foreach (var elm in ind.Elements("individual"))
                {
                    people.Add(new DockAPI_Routines.Person(elm));
                }
                Console.WriteLine("Located " + ind.Attribute("count").Value + " records from CCB database.");
                Console.WriteLine("Response: " + doc.ToString());
            } catch (Exception ex) {
                Console.WriteLine("Exception during request: " + ex);
            }
            Console.ReadLine();
        }
    }
}
