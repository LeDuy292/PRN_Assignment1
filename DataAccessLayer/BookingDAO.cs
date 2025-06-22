using BusinessObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer
{
    public class BookingDAO
    {
        public List<Booking> GetAll() => InMemoryDatabase.Instance.bookings.Where(b => b.BookingStatus == 1).ToList();

        public Booking GetByID(int id) => InMemoryDatabase.Instance.bookings.FirstOrDefault(b => b.BookingID == id && b.BookingStatus == 1);

        public List<Booking> GetByCustomerID(int id) => InMemoryDatabase.Instance.bookings.Where(b => b.CustomerID == id && b.BookingStatus == 1).ToList();
        public List<Booking> GetByPeriod(DateTime startDate, DateTime endDate) =>
            InMemoryDatabase.Instance.bookings.Where(b => b.BookingStatus == 1 && b.StartDate >= startDate && b.EndDate <= endDate)
                    .OrderByDescending(b => b.StartDate)
                    .ToList();
        public void Add(Booking booking)
        {
            if (InMemoryDatabase.Instance.bookings.Any(b => b.BookingID == booking.BookingID))
                throw new Exception("Booking ID already exists.");
            InMemoryDatabase.Instance.bookings.Add(booking);
        }

        public void Update(Booking booking)
        {
            var existing = GetByID(booking.BookingID);
            if (existing == null)
                throw new Exception("Booking not found.");
            existing.CustomerID = booking.CustomerID;
            existing.RoomID = booking.RoomID;
            existing.StartDate = booking.StartDate;
            existing.EndDate = booking.EndDate;
            existing.TotalPrice = booking.TotalPrice;
        }

        public void Delete(int id)
        {
            var booking = GetByID(id);
            if (booking == null)
                throw new Exception("Booking not found.");
            booking.BookingStatus = 2; 
        }
    }
}
