using Hajir.Crm.Products;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Hajir.Crm.Blazor.Server.Controllers
{
    public class ProductCode
    {
        public string Name { get; set; }
        public string Id { get; set; }
        

    }
    [Route("api/[controller]")]
    public class DatasheetController : ControllerBase
    {
        private readonly ILogger<DatasheetController> _logger;
        private readonly IDatasheetProvider _datasheetProvider;

        public DatasheetController(ILogger<DatasheetController> logger, IDatasheetProvider datasheetProvider)
        {
            _logger = logger;
            _datasheetProvider = datasheetProvider;
        }

        // GET: api/Datasheet/Products
        [HttpGet("Products")]
        //[HttpGet]
        public async Task<IActionResult> GetProductCodes()
        {
            try
            {
                var data = await _datasheetProvider.GetDatasheets();
                var res = data.Select(x => new ProductCode { Id=x.ProductCode, Name=x.ProductName}).ToArray();
                return Ok(res);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while fetching product codes.");
                return StatusCode(500, "Internal server error");
            }
        }

        // GET: api/Datasheet/{id}
        [HttpGet("{id}")]
        public async Task<IActionResult> GetDatasheetById(string id)
        {
            try
            {
                var data = await _datasheetProvider.GetDatasheets();
                var datasheet = data.FirstOrDefault(x => x.ProductCode == id);

                if (datasheet == null)
                {
                    return NotFound($"Datasheet with ID {id} not found.");
                }

                return Ok(datasheet);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while fetching the datasheet.");
                return StatusCode(500, "Internal server error");
            }
        }

        // GET: api/Datasheet/Props
        [HttpGet("Props")]
        public async Task<IActionResult> GetProps()
        {
            try
            {
                var data = await _datasheetProvider.GetDatasheets();
                return Ok(data.Select(x => x.ProductCode).ToArray());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while fetching props.");
                return StatusCode(500, "Internal server error");
            }
        }
        [HttpGet("All")]
        public async Task<IActionResult> GetDatasheets()
        {
            try
            {
                var data = await _datasheetProvider.GetDatasheets();
                return Ok(data);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while fetching props.");
                return StatusCode(500, "Internal server error");
            }
        }

    }
}
