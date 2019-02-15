using System;

namespace B2CUtils
{
    class Program
    {

        static void Main(string[] args)
        {
            string userId = args[0];
            B2CGraphClient client = new B2CGraphClient();
            var result = client.GetUserById(userId).Result;
            Console.WriteLine(result);
            Console.ReadLine();
        }
    }
}
