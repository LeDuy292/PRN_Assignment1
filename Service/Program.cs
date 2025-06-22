namespace Service
{
    internal class Program
    {
        private static CustomerService customerService = new CustomerService();
        static void Main(string[] args)
        {
            Console.WriteLine(customerService.Authenticate("admin@FUMiniHotelSystem.com", "@@abc123@@"));
        }
    }
}
