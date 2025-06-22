using BusinessObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service
{
    public interface IBookingService
    {
        List<Booking> GetAll();
        Booking GetByID(int id);
        List<Booking> GetByCustomerID(int customerId);
        List<Booking> GetByPeriod(DateTime startDate, DateTime endDate);
        void Add(Booking booking);
        void Update(Booking booking);
        void Delete(int id);
    }
}
