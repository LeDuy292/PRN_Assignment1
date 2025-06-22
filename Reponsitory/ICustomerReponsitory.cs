using BusinessObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reponsitory
{
    public interface ICustomerReponsitory
    {
        List<Customer> GetAll();
        Customer GetByID(int id);
        Customer GetByEmail(string email);
        void Add(Customer customer);
        void Update(Customer customer);
        void Delete(int id);
    }
}
