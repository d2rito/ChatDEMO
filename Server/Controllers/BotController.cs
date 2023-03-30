using CsvHelper;
using Microsoft.AspNetCore.Mvc;
using System.Formats.Asn1;
using System.Globalization;
using System.Net.Http;
using System.Reflection.Metadata.Ecma335;
using System.Reflection.PortableExecutable;

namespace BlazorApp1.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BotController : ControllerBase
    {
        private readonly HttpClient _httpClient;

        public BotController(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient();
        }
        [HttpGet]
        public async Task<Dictionary<string,string>?> GetStockValues(string stock_code)
        {
            Dictionary<string, string> headerValuePairs = new Dictionary<string, string>();
            var response = await _httpClient.GetAsync($"https://stooq.com/q/l/?s={stock_code}&f=sd2t2ohlcv&h&e=csv");
            var stringcsv = response.Content.ReadAsStringAsync().Result;
            string[] splitString = stringcsv.Split(new string[] { "\r\n" }, StringSplitOptions.None);
            //string de headers y values
            string headersString = splitString[0];
            string valuesString = splitString[1];
            //separar los valores por coma
            string[] headers = headersString.Split(',');
            string[] values = valuesString.Split(',');

            if (values.Contains("N/D"))
            {
                return null;
            }

            for (int i = 0; i < headers.Length; i++)
            {
                headerValuePairs.Add(headers[i], values[i]);
            }


            return headerValuePairs;
        }

        //wrong solution, the csv isnt stored locally
        //[HttpGet]
        //public IActionResult Get(string header)
        //{
        //    var columnValues = new List<string>();
        //    using (var reader = new StreamReader("CSV/aapl.us.csv"))
        //    {
        //        // headers
        //        var headerLine = reader.ReadLine();
        //        var columnNames = headerLine.Split(',');

        //        //indice 
        //        var columnIndex = columnNames.ToList().IndexOf(header);
        //        if (columnIndex == -1)
        //        {
        //            // columna no existe
        //            return NotFound();
        //        }

        //        while (!reader.EndOfStream)
        //        {
        //            var line = reader.ReadLine();
        //            var values = line.Split(',');

        //            // agregar valor a la lista
        //            columnValues.Add(values[columnIndex]);
        //        }
        //    }
        //    return Ok(columnValues);
        //}
    }
}

