using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Xml.Linq;

namespace CustomerApi.Model
{
    public class CustomerRepository
    {
        private readonly IList<Customer> customers;

        public CustomerRepository()
        {
            var doc = XDocument.Load("Customers.xml");
            customers = new ObservableCollection<Customer>(
                doc.Descendants("Customer").Select(c => new Customer(
                    GetValueOrDefault(c, "CustomerID"), GetValueOrDefault(c, "CompanyName"),
                    GetValueOrDefault(c, "ContactName"), GetValueOrDefault(c, "ContactTitle"),
                    GetValueOrDefault(c, "Address"), GetValueOrDefault(c, "City"),
                    GetValueOrDefault(c, "Region"), GetValueOrDefault(c, "PostalCode"),
                    GetValueOrDefault(c, "Country"), GetValueOrDefault(c, "Phone"),
                    GetValueOrDefault(c, "Fax")
            )).ToList());
        }

        #region ICustomerRepository Members

        public bool Add(Customer customer)
        {
            if (customers.FirstOrDefault(c => c.CustomerId == customer.CustomerId) == null)
            {
                customers.Add(customer);
                return true;
            }
            return false;
        }

        public bool Remove(Customer customer)
        {
            if (customers.IndexOf(customer) >= 0)
            {
                customers.Remove(customer);
                return true;
            }
            return false;
        }

        public bool Update(Customer customer)
        {
            var currentCustomer = GetCustomer(customer.CustomerId);
            if (currentCustomer == null)
                return false;
            var index = customers.IndexOf(currentCustomer);
            currentCustomer = new Customer(customer.CustomerId, customer.CompanyName,
              customer.ContactName, customer.ContactTitle, customer.Address, customer.City,
              customer.Region, customer.PostalCode, customer.Country, customer.Phone, customer.Fax);
            customers[index] = currentCustomer;
            return true;
        }

        public bool Commit()
        {
            try
            {
                var doc = new XDocument(new XDeclaration("1.0", "utf-8", "yes"));
                var root = new XElement("Customers");
                foreach (Customer customer in customers)
                {
                    root.Add(new XElement("Customer",
                                          new XElement("CustomerID", customer.CustomerId),
                                          new XElement("CompanyName", customer.CompanyName),
                                          new XElement("ContactName", customer.ContactName),
                                          new XElement("ContactTitle", customer.ContactTitle),
                                          new XElement("Address", customer.Address),
                                          new XElement("City", customer.City),
                                          new XElement("Region", customer.Region),
                                          new XElement("PostalCode", customer.PostalCode),
                                          new XElement("Country", customer.Country),
                                          new XElement("Phone", customer.Phone),
                                          new XElement("Fax", customer.Fax)
                                 ));
                }
                doc.Add(root);
                doc.Save("Customers.xml");
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public IEnumerable<Customer> GetAll() => customers;

        public Customer GetCustomer(string id) => customers.FirstOrDefault(c => string.Equals(c.CustomerId, id, StringComparison.CurrentCultureIgnoreCase));

        #endregion

        private static string GetValueOrDefault(XContainer el, string propertyName)
        {
            return el.Element(propertyName) == null ? string.Empty : el.Element(propertyName).Value;
        }
    }
}