using BusinessObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer
{
    public class CustomerDAO
    {
        public CustomerDAO()
        {
        }
        public List<Customer> GetAll() => InMemoryDatabase.Instance.customers.Where(c => c.CustomerStatus == 1).ToList();

        public Customer GetByID(int id) => InMemoryDatabase.Instance.customers.FirstOrDefault(c => c.CustomerID == id && c.CustomerStatus == 1);
        public void Add(Customer customer)
        {
            if (InMemoryDatabase.Instance.customers.Any(c => c.CustomerID == customer.CustomerID))
                throw new Exception("Customer ID  already exists.");
            InMemoryDatabase.Instance.customers.Add(customer);
        }
        public Customer GetByEmail(string email) => InMemoryDatabase.Instance.customers.FirstOrDefault(c => c.EmailAddress == email && c.CustomerStatus == 1);
        public void Update(Customer customer)
        {
            var existing = GetByID(customer.CustomerID);
            if (existing == null)
                throw new Exception("Customer not found.");
            existing.CustomerFullName = customer.CustomerFullName;
            existing.Telephone = customer.Telephone;
            existing.EmailAddress = customer.EmailAddress;
            existing.CustomerBirthday = customer.CustomerBirthday;
            existing.Password = customer.Password;
        }

        public void Delete(int id)
        {
            var customer = GetByID(id);
            if (customer == null)
                throw new Exception("Customer not found.");
            customer.CustomerStatus = 2; 
        }

    }
}
