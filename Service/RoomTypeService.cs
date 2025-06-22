using BusinessObjects;
using Reponsitory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service
{
    public class RoomTypeService : IRoomTypeService
    {
        private readonly IRoomTypeRepository repository = new RoomTypeReponsitory();

        public List<RoomType> GetAll() => repository.GetAll();
        public RoomType GetByID(int id) => repository.GetByID(id);
    }
}
