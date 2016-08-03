using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
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

        public List<Address> Addreses { get; set; }
        public Dictionary<string, string> Phones { get; set; }

        public DateTime Birthdate { get; set; }

        public string XML { get; set; }

        public Person() {
            Addreses = new List<Address>();
            Phones = new Dictionary<string, string>();
        }
        public Person(XElement element) : this() {
            XML = element.ToString();
            id = int.Parse(element.Attribute("id").Value);
            foreach (var elem in element.Elements())
            {
                switch (elem.Name.LocalName) {
                    case "first_name":
                        FirstName = elem.Value;
                        break;
                    case "last_name":
                        LastName = elem.Value;
                        break;
                    case "middle_name":
                        MiddleName = elem.Value;
                        break;
                    case "birthday":
                        Birthdate = DateTime.Parse(elem.Value);
                        break;
                    case "addresses":
                        foreach (var addy in elem.Elements()) {
                            Addreses.Add(new Address(addy));
                        }
                        break;
                    case "phones":
                        foreach (var fon in elem.Elements()) {
                            Phones.Add(fon.Attribute("type").Value, fon.Value);
                        }
                        break;
                }
            }
        }

        public override string ToString()
        {
            return $"{id:D5} - {FirstName} {MiddleName} {LastName} ({Birthdate:MMM dd yyy})";
        }
    }

    public class Address {
        public string Type { get; set; }
        public string Street { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string ZIP { get; set; }

        public Address(XElement element) {
            Type = element.Attribute("type").Value;
            foreach (var elem in element.Elements()) {
                switch (elem.Name.LocalName) {
                    case "street_address":
                        Street = elem.Value;
                        break;
                    case "city":
                        City = elem.Value;
                        break;
                    case "state":
                        State = elem.Value;
                        break;
                    case "zip":
                        ZIP = elem.Value;
                        break;
                }
            }
        }
    }
}
