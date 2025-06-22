using BusinessObjects;
using DataAccessLayer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reponsitory
{
    public class RoomTypeReponsitory : IRoomTypeRepository
    {
        private RoomTypeDAO roomTypeDAO = new RoomTypeDAO();
        public List<RoomType> GetAll() => roomTypeDAO.GetAll();
        public RoomType GetByID(int id) => roomTypeDAO.GetByID(id);
    }
}
