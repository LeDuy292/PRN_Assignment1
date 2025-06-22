using BusinessObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reponsitory
{
    public interface IRoomTypeRepository
    {
        List<RoomType> GetAll();
        RoomType GetByID(int id);
    }
}
