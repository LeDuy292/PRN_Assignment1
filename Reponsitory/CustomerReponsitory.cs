using BusinessObjects;
using DataAccessLayer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reponsitory
{
    public class CustomerReponsitory : ICustomerReponsitory
    {
        private CustomerDAO customerDAO = new CustomerDAO();
        public void Add(Customer customer) => customerDAO.Add(customer);
        public Customer GetByEmail(string email) => customerDAO.GetByEmail(email);
        public void Delete(int id) => customerDAO.Delete(id);
        public List<Customer> GetAll() => customerDAO.GetAll();

        public Customer GetByID(int id) => customerDAO.GetByID(id);

        public void Update(Customer customer) => customerDAO.Update(customer);
    }
}
