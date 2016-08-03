using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace DockAPI_Routines
{
    public class Person {
        public int id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string MiddleName { get; set; }

        public Person() { }
        public Person(XElement element)
        {
            id = int.Parse(element.Attribute("id").Value);
            foreach (var elem in element.Elements())
            {
                switch (elem.Name.LocalName)
                {
                    case "first_name":
                        FirstName = elem.Value;
                        break;
                    case "last_name":
                        LastName = elem.Value;
                        break;
                    case "middle_name":
                        MiddleName = elem.Value;
                        break;
                }
            }
        }

        public override string ToString()
        {
            return $"{FirstName} {MiddleName} {LastName}";
        }
    }
}
