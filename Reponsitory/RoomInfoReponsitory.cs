using BusinessObjects;
using DataAccessLayer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace Reponsitory
{
    public class RoomInfoReponsitory : IRoomInfoReponsitory
    {
        private RoomInfomationDAO dao = new RoomInfomationDAO();

        public void Add(RoomInformation room) => dao.Add(room);

        public void Update(RoomInformation room) => dao.Update(room);

        public List<RoomInformation> GetAll() => dao.GetAll();

        public void Delete(int id) => dao.Delete(id);

        public RoomInformation GetRoomByID(int id) => dao.GetByID(id);
    }
}
