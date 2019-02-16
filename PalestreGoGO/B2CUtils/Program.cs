using Newtonsoft.Json.Linq;
using System;

namespace B2CUtils
{
    class Program
    {

        static void Main(string[] args)
        {
            string messge = @"{'email':'lord_ryder@hotmail.com','code':'JTMY5gwEtUwtM6OXGogOHW78MUwsHvOQu2g + TDN4NcI = ','confirmationurl':'https://localhost:44320/confirm-email','isCliente':true}";
            dynamic json = JObject.Parse(messge);
            Console.WriteLine($"Nome:{json.nome}");
            Console.WriteLine($"Email:{json.email}");
            Console.WriteLine($"confirmationurl:{json.confirmationurl}");
            Console.ReadLine();
            return;
            string userId = args[0];
            B2CGraphClient client = new B2CGraphClient();
            var result = client.GetUserById(userId).Result;
            Console.WriteLine(result);
            Console.ReadLine();
        }
    }
}
