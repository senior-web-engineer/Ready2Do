using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using IdentityModel.Client;
using System.Threading.Tasks;
using System.Net.Http;
using System.Text;
using System.Web;
using Newtonsoft.Json;
using PalestreGoGo.WebAPI.ViewModel;

namespace WebAPI.Tests
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public async Task TestMethod1()
        {
            var disco = await DiscoveryClient.GetAsync("http://localhost:40021");
            Assert.IsFalse(disco.IsError, disco.Error);
            var tokenClient = new TokenClient(disco.TokenEndpoint, "provisioning.client", "b753a50a8966dccb0c99248d2aa1fe2d65a6dca43de88cc1a2");
            var tokenResponse = await tokenClient.RequestClientCredentialsAsync("palestregogo.api.clienti.provisioning");
            Assert.IsFalse(tokenResponse.IsError, tokenResponse.Error);

            // call api
            var client = new HttpClient();
            client.SetBearerToken(tokenResponse.AccessToken);

            var response = await client.PostAsync("http://localhost:1952/api/Provisioning", null);
            Assert.IsTrue(response.IsSuccessStatusCode);
        }

        [TestMethod]
        public async Task CreateNuovoClienteTest()
        {
            var cliente = new NuovoClienteViewModel()
            {
                Citta = "Roma",
                Coordinate = new Coordinate(12.231F, 23.4984F),
                Country = "Italia",
                Email = "cliente1@test.test",
                IdTipologia = 0,
                Indirizzo = "Via roma",
                Nome = "Cliente 1",
                NumTelefono = "123456",
                RagioneSociale = "Rag. Sociale Cliente 1",
                ZipOrPostalCode = "00120",
                NuovoUtente = new NuovoUtenteViewModel()
                {
                    Cognome = "Rossi",
                    Nome = "Mario",
                    Email = "mario.rossi@test.test",
                    Password = "password",
                    Telefono = "1234567890"
                }
            };

            /*StringContent content = new StringContent(@"{
            ""email"":""testuser@demo.it"",
            ""nome"":""nome"",
            ""cognome"":""cognome"",
            ""phone"":""12345678"",
            ""password"":""password"",
            ""token"":""token123"",
            }", Encoding.UTF8, "application/json");
            */
            var content = new StringContent(JsonConvert.SerializeObject(cliente), Encoding.UTF8, "application/json");
            HttpClient client = new HttpClient();
            var resp = await client.PostAsync("http://localhost:1952/api/clienti", content);
            resp.EnsureSuccessStatusCode();
            //resp = await client.PostAsync("http://localhost:1952/api/users/testuser@demo.it/confirmation/",)
            //    resp.EnsureSuccessStatusCode();
        }

        [TestMethod]
        public async Task ConfirmUser()
        {
            HttpClient client = new HttpClient();

            string code = @"CfDJ8DOkObUZa7FIny0fgxBcCRHbUCiIaXeCQM2cU8tJUpsLETq0tv4wEubZ4yAvySIjIzpVVCZqH5G4AJOAZax2mRdiTt/6CGb69elGNfOwImEVpL6qPWnPTx0lVdzDQ8kQc0RE/59q/Su6+75/KWM3d1jwynqMiTnrJ3ndrfbXmV5rqpalCCxMX4tikYyQYHOS/Ta2eyKcW4Eg59IjDuVTx1uFc8fBJYa7S5KYiOwmwfUdDy4MocmGeZnr6uth2WFz9w==";
            string url = $"http://localhost:1952/api/users/{Uri.EscapeDataString("testuser@demo.it")}/confirmation";
            var content = new StringContent(JsonConvert.SerializeObject(code), Encoding.UTF8, "application/json");
            var resp = await client.PostAsync(url, content);
            Assert.IsTrue(resp.IsSuccessStatusCode);
        }
    }
}
