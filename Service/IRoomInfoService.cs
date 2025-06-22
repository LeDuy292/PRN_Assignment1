using BusinessObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service
{
    public  interface IRoomInfoService
    {
        List<RoomInformation> GetAll();
        RoomInformation GetByID(int id);
        void Add(RoomInformation room);
        void Update(RoomInformation room);
        void Delete(int id);
        public List<RoomInformation> GetAvailableRooms(DateTime date);
        public void UpdateExpiredRoomStatus();
    }
}
