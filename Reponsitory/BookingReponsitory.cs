using BusinessObjects;
using DataAccessLayer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reponsitory
{
    public class BookingReponsitory : IBookingReponsitory
    {
        private BookingDAO bookingDAO = new BookingDAO();
        public void Add(Booking booking) => bookingDAO.Add(booking);

        public void Delete(int id) => bookingDAO.Delete(id);

        public List<Booking> GetAll() => bookingDAO.GetAll();

        public List<Booking> GetByCustomerID(int customerId) => bookingDAO.GetByCustomerID(customerId);

        public Booking GetByID(int id) => bookingDAO.GetByID(id);

        public List<Booking> GetByPeriod(DateTime startDate, DateTime endDate) => bookingDAO.GetByPeriod(startDate, endDate);

        public void Update(Booking booking) => bookingDAO.Update(booking);
    }
}
