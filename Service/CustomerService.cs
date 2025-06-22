using BusinessObjects;
using Reponsitory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Service
{
    public class CustomerService : ICustomerService
    {
        private readonly ICustomerReponsitory repository = new CustomerReponsitory();

        public List<Customer> GetAll() => repository.GetAll();
        public Customer GetByID(int id) => repository.GetByID(id);
        public Customer GetByEmail(string email) => repository.GetByEmail(email);
        public void Add(Customer customer) => repository.Add(customer);
        public void Update(Customer customer) => repository.Update(customer);
        public void Delete(int id) => repository.Delete(id);

        public bool Authenticate(string email, string password)
        {
            try
            {
                string json = File.ReadAllText("appsettings.json");
                Console.WriteLine(json);
                var config = JsonSerializer.Deserialize<Dictionary<string, Dictionary<string, string>>>(json);

                if (config != null && config.ContainsKey("AdminAccount"))
                {
                    var adminEmail = config["AdminAccount"].GetValueOrDefault("Email");
                    var adminPassword = config["AdminAccount"].GetValueOrDefault("Password");
                    Console.WriteLine(adminEmail);
                    Console.WriteLine(adminPassword);
                    if (email == adminEmail && password == adminPassword)
                        return true;
                }
                var customer = repository.GetByEmail(email);
                return customer != null && customer.Password == password;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }
        }
    }
}
