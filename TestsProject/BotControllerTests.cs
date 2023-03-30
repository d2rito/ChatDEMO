using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestsProject
{
    [TestClass]
    public class BotControllerTests
    {
        private readonly HttpClient _httpClient = new HttpClient();

        [TestMethod]
        public async Task GetStockValues_ReturnsDictionary()
        {
            //arrange
            var controller = new BotController(new TestHttpClientFactory(_httpClient));
            var stockCode = "AAPL.US"; // Apple Inc. stock code

            //act
            var result = await controller.GetStockValues(stockCode);

            //assert
            Assert.IsNotNull(result);
            Assert.IsTrue(result.Count > 0);
            Assert.IsTrue(result.ContainsKey("Symbol"));
            Assert.IsTrue(result.ContainsKey("Close"));
        }
        //setting up the httpclient
        private class TestHttpClientFactory : IHttpClientFactory
        {
            private readonly HttpClient _httpClient;

            public TestHttpClientFactory(HttpClient httpClient)
            {
                _httpClient = httpClient;
            }

            public HttpClient CreateClient(string name)
            {
                return _httpClient;
            }

            public HttpClient CreateClient()
            {
                return _httpClient;
            }
        }
    }
}
