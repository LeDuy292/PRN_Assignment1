using Azure.Core.Serialization;
using BusinessObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer
{
    public class RoomTypeDAO
    {
        public List<RoomType> GetAll() => InMemoryDatabase.Instance.roomsType.ToList();

        public RoomType GetByID(int id) => InMemoryDatabase.Instance.roomsType.FirstOrDefault(rt => rt.RoomTypeID == id);
    }
}
