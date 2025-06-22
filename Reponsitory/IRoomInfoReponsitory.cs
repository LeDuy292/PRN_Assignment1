using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BusinessObjects;
namespace Reponsitory
{
    public interface  IRoomInfoReponsitory
    {
        public List<RoomInformation> GetAll();
        public RoomInformation GetRoomByID(int id);
        public void Add(RoomInformation room);
        public void Update(RoomInformation room);
        public void Delete(int id);
    }
}
