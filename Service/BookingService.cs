using BusinessObjects;
using Reponsitory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service
{
    public class BookingService : IBookingService
    {
        private readonly IBookingReponsitory repository = new BookingReponsitory();

        public List<Booking> GetAll() => repository.GetAll();
        public Booking GetByID(int id) => repository.GetByID(id);
        public List<Booking> GetByCustomerID(int customerId) => repository.GetByCustomerID(customerId);
        public List<Booking> GetByPeriod(DateTime startDate, DateTime endDate) => repository.GetByPeriod(startDate, endDate);
        public void Add(Booking booking) => repository.Add(booking);
        public void Update(Booking booking) => repository.Update(booking);
        public void Delete(int id) => repository.Delete(id);

    }
}
