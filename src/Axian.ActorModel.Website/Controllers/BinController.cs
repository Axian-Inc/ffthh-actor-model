using System;
using System.Collections;
using System.Linq;
using System.Threading.Tasks;
using Akka.Actor;
using Microsoft.AspNetCore.Mvc;

namespace Axian.ActorModel.Website.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class BinController : ControllerBase
    {
        private readonly IActorRef _binMgr;

        public BinController(AxSystem sys)
        {
            _binMgr = sys.BinManager;
        }

        [HttpPost]
        public async Task<IActionResult> CreateBin()
        {
            var command = new BinManager.CreateBin();
            var response = await _binMgr.Ask<BinManager.CreateBinResponse>(command);

            var model = new
            {
                response.Bin,
                CaptureUrl = Url.Link(nameof(CaptureRequest), new { response.Bin }).ToLowerInvariant(),
                LogUrl = Url.Link(nameof(ListRequests), new { response.Bin }).ToLowerInvariant()
            };

            return Ok(model);
        }

        [HttpGet]
        public async Task<IActionResult> ListBins()
        {
            var command = new BinManager.ListBins();
            var response = await _binMgr.Ask<BinManager.ListBinsResponse>(command);

            var model = new
            {
                Bins = response.Bins.Select(bin => new
                {
                    bin,
                    CaptureUrl = Url.Link(nameof(CaptureRequest), new { bin }).ToLowerInvariant(),
                    LogUrl = Url.Link(nameof(ListRequests), new { bin }).ToLowerInvariant()
                })
            };

            return Ok(model);
        }

        [Route("{bin}", Name = nameof(CaptureRequest))]
        [AcceptVerbs("GET","POST","PUT","DELETE","PATCH","OPTIONS","HEAD","TRACE")]
        public async Task<IActionResult> CaptureRequest(string bin)
        {
            try
            {
                object requestModel = await CreateRequestModel();
                var command = new BinManager.CaptureRequest(bin, requestModel);
                await _binMgr.Ask<Status.Success>(command, TimeSpan.FromSeconds(3));
                return Ok();
            }
            catch (AskTimeoutException)
            {
                return NotFound();
            }
        }

        [Route("/logs/{bin}", Name = nameof(ListRequests))]
        public async Task<IActionResult> ListRequests(string bin)
        {
            try
            {
                var command = new BinManager.ListRequests(bin);
                var response = await _binMgr.Ask<Bin.ListResponse>(command, TimeSpan.FromSeconds(3));

                var model = new
                {
                    bin,
                    response.Items,
                    CaptureUrl = Url.Link(nameof(CaptureRequest), new { bin }).ToLowerInvariant(),
                    LogUrl = Url.Link(nameof(ListRequests), new { bin }).ToLowerInvariant()
                };

                return Ok(model);

            }
            catch (AskTimeoutException)
            {
                return NotFound();
            }            
        }

        private async Task<object> CreateRequestModel()
        {
            var empty = Array.Empty<object>();

            string content = null;
            if (Request.Body != null && Request.Body.CanRead) {
                using (var textReader = new System.IO.StreamReader(Request.Body)) {
                    content = await textReader.ReadToEndAsync();
                }
            }

            IEnumerable form = null;
            if (Request.HasFormContentType) {
                form = Request.Form.AsEnumerable();
            }

            var model = new {
                Method = Request.Method,
                Headers = Request.Headers?.AsEnumerable(),
                Query = Request.Query.AsEnumerable(),
                Form = form,
                Content = content
            };

            return model;
        }
    }
}
