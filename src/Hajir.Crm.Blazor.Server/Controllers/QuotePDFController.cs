using GN;
using Hajir.Crm.Features.Sales;
using Microsoft.AspNetCore.Mvc;
using System;
using System.IO;
using System.Net.Http;

namespace Hajir.Crm.Blazor.Server.Controllers
{
    [Route("pdf")]
    public class QuotePDFController : ControllerBase
    {

        [HttpGet]
        public string HHH()
        {
            return "jjj";
        }


        [HttpGet]
        [Route("{id}")]
        public ActionResult GetFile([FromRoute] string id)
        {
            id = "02-00104-VZel";
            var f = AppHost.GetService<IQuoteRepository>().LoadQuoteByNumber("02-00104-VZel");
            var url = $"http://192.168.20.61/print/{id}";

            var stream = GN.Library.PDF.PDFExtensions.Test(url);
            return this.File(stream.ToArray(), "application/pdf");
            
            ActionResult result = NotFound();
            if (Guid.TryParse(id, out var _id))
            {
                return this.Content("ok");
                //var file = storage.Get(_id);
                //if (file != null)
                //{
                //    result = File(file.Contents, file.ContentType);
                //}
            }
            return result;
        }
        [HttpGet]
        [Route("/pdf/quote/{id}")]
        public ActionResult GetFile1([FromRoute] string id)
        {
            id = "02-00104-VZel";
            //var f = AppHost.GetService<IQuoteRepository>().LoadQuoteByNumber("02-00104-VZel");
            var url = $"http://192.168.20.61/print/{id}";

            var stream = GN.Library.PDF.PDFExtensions.Test(url);
            return this.File(stream.ToArray(), "application/pdf");

            ActionResult result = NotFound();
            if (Guid.TryParse(id, out var _id))
            {
                return this.Content("ok");
                //var file = storage.Get(_id);
                //if (file != null)
                //{
                //    result = File(file.Contents, file.ContentType);
                //}
            }
            return result;
        }

    }
}
