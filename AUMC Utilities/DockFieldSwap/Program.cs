using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DockAPI_Routines;

namespace DockFieldSwap
{
    /// <summary>
    /// This program was used to migrate data from the user-defined date fields in CCB to the Significant Events table.  High-School graduations and Baptisms
    /// were moved to Significant Events.  This was performed on 8/3/2016 @ 3:00PM.
    /// </summary>
    class Program
    {
        static void Main(string[] args)
        {
            List<ReportData> data = new List<ReportData>();
            var doc = DockRequest.GetAllPeople();
            //var doc = DockRequest.FindProfiles(new Dictionary<string, object> {{"last_name", "Braxton"}});
            var persons = doc.Descendants("individuals").FirstOrDefault();
            foreach (var person in persons.Elements("individual")) {
                string nm = person.Element("first_name").Value + " " + person.Element("last_name").Value;
                var rd = new ReportData() {Name = nm, ProfileID = int.Parse(person.Attribute("id").Value)};
                var dates = person.Descendants("user_defined_date_field");
                foreach (var date in dates) {
                    var name = date.Element("name");
                    if (name != null) {
                        switch (name.Value) {
                            case "udf_date_3":
                                rd.Graduation = true;
                                rd.GradDate = DateTime.Parse(date.Element("date").Value);
                                DockRequest.AddIndividualSignificantEvent(rd.ProfileID, (int)DockEvents.HighSchoolGraduation, rd.GradDate);
                                break;
                            case "udf_date_4":
                                rd.Baptism = true;
                                rd.BapDate = DateTime.Parse(date.Element("date").Value);
                                DockRequest.AddIndividualSignificantEvent(rd.ProfileID, (int)DockEvents.Baptism, rd.BapDate);
                                break;
                        }
                    }
                }
                data.Add(rd);
            }
            using (var csv = new StreamWriter("ConversionReport.csv")) {
                csv.WriteLine("{0,-25},{1,-7},{2,-10}", "NAME", "BAPTISM", "GRADUATION");
                foreach (var reportData in data) {
                    csv.WriteLine(reportData);
                }
            }
            Process.Start("ConversionReport.csv");
        }
    }

    class ReportData {
        public int ProfileID { get; set; }
        public string Name { get; set; }
        public bool Graduation { get; set; }
        public DateTime GradDate { get; set; }
        public bool Baptism { get; set; }
        public DateTime BapDate { get; set; }

        public override string ToString() { return $"{Name,-25},{Baptism,-7},{Graduation,-10}"; }
    }
}
