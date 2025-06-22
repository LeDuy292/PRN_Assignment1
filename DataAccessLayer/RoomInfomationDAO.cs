using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BusinessObjects;
namespace DataAccessLayer
{
    public class RoomInfomationDAO
    {
        public RoomInfomationDAO() { }
        public List<RoomInformation> GetAll() => InMemoryDatabase.Instance.roomsInfo;

        public RoomInformation GetByID(int id) => InMemoryDatabase.Instance.roomsInfo.FirstOrDefault(r => r.RoomID == id );


        public void Add(RoomInformation room)
        {
            if (InMemoryDatabase.Instance.roomsInfo.Any(r => r.RoomID == room.RoomID))
                throw new Exception("Room ID already exists.");
            InMemoryDatabase.Instance.roomsInfo.Add(room);
        }

        public void Update(RoomInformation room)
        {
            var existing = GetByID(room.RoomID);
            if (existing == null)
                throw new Exception("Room not found.");
            existing.RoomNumber = room.RoomNumber;
            existing.RoomDescription = room.RoomDescription;
            existing.RoomMaxCapacity = room.RoomMaxCapacity;
            existing.RoomPricePerDate = room.RoomPricePerDate;
            existing.RoomTypeID = room.RoomTypeID;
        }

        public void Delete(int id)
        {
            InMemoryDatabase.Instance.roomsInfo.RemoveAll(r => r.RoomID == id);
        }
    }
}
