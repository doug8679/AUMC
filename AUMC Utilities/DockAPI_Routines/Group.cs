using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace DockAPI_Routines
{
    public class Group : IComparable<Group>
    {
        public int GroupID { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }

        public String CalendarFeed { get; set; }

        public Group() { }

        public Group(XElement element) {
            GroupID = int.Parse(element.Attribute("id").Value);
            foreach (var elem in element.Elements()) {
                switch (elem.Name.LocalName) {
                    case "name":
                        Name = elem.Value;
                        break;
                    case "description":
                        Description = elem.Value;
                        break;
                    case "calendar_feed":
                        CalendarFeed = elem.Value;
                        break;
                }
            }
        }

        public int CompareTo(Group other) {
            return string.Compare(Name, other.Name, StringComparison.Ordinal);
        }

        public override string ToString() {
            return Name;
        }
    }
}
