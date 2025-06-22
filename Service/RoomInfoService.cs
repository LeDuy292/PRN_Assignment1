using BusinessObjects;
using Reponsitory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service
{
    public class RoomInfoService : IRoomInfoService
    {
        private readonly IRoomInfoReponsitory roomInfoReponsitory;
        private readonly IBookingReponsitory bookingRepository;

        public RoomInfoService() {
            roomInfoReponsitory = new RoomInfoReponsitory();
            bookingRepository = new BookingReponsitory();
        }
        public void Add(RoomInformation room) => roomInfoReponsitory.Add(room);
        public void Delete(int id) => roomInfoReponsitory.Delete(id);

        public List<RoomInformation> GetAll() => roomInfoReponsitory.GetAll();

        public List<RoomInformation> GetAvailableRooms(DateTime date)
        {
            var bookedRoomIds = bookingRepository.GetByPeriod(date, date)
               .Select(b => b.RoomID)
               .ToList();

            return roomInfoReponsitory.GetAll()
                .Where(r => r.RoomStatus == 1 && !bookedRoomIds.Contains(r.RoomID))
                .ToList();
        }

        public RoomInformation GetByID(int id) => roomInfoReponsitory.GetRoomByID(id);

        public void Update(RoomInformation room) => roomInfoReponsitory.Update(room);

        public void UpdateExpiredRoomStatus()
        {
            var today = DateTime.Today;
            var bookings = bookingRepository.GetAll()
                .Where(b => b.EndDate < today)
                .Select(b => b.RoomID)
                .Distinct()
                .ToList();

            foreach (var roomId in bookings)
            {
                var room = roomInfoReponsitory.GetRoomByID(roomId);
                if (room != null && room.RoomStatus == 2)
                {
                    room.RoomStatus = 1;
                    roomInfoReponsitory.Update(room);
                }
            }
        }
    }
}
