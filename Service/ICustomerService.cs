using BusinessObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service
{
    public interface ICustomerService
    {
        List<Customer> GetAll();
        Customer GetByID(int id);
        Customer GetByEmail(string email);
        void Add(Customer customer);
        void Update(Customer customer);
        void Delete(int id);
        bool Authenticate(string email, string password);
    }
}
