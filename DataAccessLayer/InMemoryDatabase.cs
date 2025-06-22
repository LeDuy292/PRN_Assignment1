using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BusinessObjects;
namespace DataAccessLayer
{
    public class InMemoryDatabase
    {
        private static InMemoryDatabase _instance;
        public List<RoomType> roomsType { get; private set; }

        public List<RoomInformation> roomsInfo { get; private set; }
        public List<Customer> customers { get; private set; }

        public List<Booking> bookings { get; private set; }

        private static readonly Object _lock = new object();
        public static InMemoryDatabase Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (_lock)
                    {
                        if (_instance == null)
                        {
                            _instance = new InMemoryDatabase();
                        }
                    }
                }
                return _instance;
            }
        }
        private InMemoryDatabase()
        {
          
                        roomsType = new List<RoomType>
        {
            new RoomType { RoomTypeID = 1, RoomTypeName = "Single", TypeDescription = "1 person", TypenNote = "Basic room" },
            new RoomType { RoomTypeID = 2, RoomTypeName = "Double", TypeDescription = "2 persons", TypenNote = "Couple" },
            new RoomType { RoomTypeID = 3, RoomTypeName = "Suite", TypeDescription = "Luxury suite", TypenNote = "VIP" },
        };

                        roomsInfo = new List<RoomInformation>
        {
            new RoomInformation { RoomID = 1, RoomNumber = "101", RoomDescription = "Sea view", RoomMaxCapacity = 2, RoomStatus = 1, RoomPricePerDate = 50.0m, RoomTypeID = 2 },
            new RoomInformation { RoomID = 2, RoomNumber = "102", RoomDescription = "Garden view", RoomMaxCapacity = 1, RoomStatus = 1, RoomPricePerDate = 30.0m, RoomTypeID = 1 },
        };

                        customers = new List<Customer>
        {
            new Customer { CustomerID = 1, CustomerFullName = "Nguyen Van A", Telephone = "0909123456", EmailAddress = "a@example.com", CustomerBirthday = new DateTime(1999, 1, 1), CustomerStatus = 1, Password = "123456" },
            new Customer { CustomerID = 2, CustomerFullName = "Le Thi B", Telephone = "0909876543", EmailAddress = "b@example.com", CustomerBirthday = new DateTime(1998, 5, 20), CustomerStatus = 1, Password = "123456" },
        };
            bookings = new List<Booking> {
            new Booking { BookingID = 1, CustomerID = 1, RoomID = 1, StartDate = DateTime.Now, EndDate = DateTime.Now.AddDays(2), TotalPrice = 100.00m, BookingStatus = 1 }
            };
                    }
             
            }
        }

