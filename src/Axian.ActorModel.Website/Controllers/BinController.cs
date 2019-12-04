using System;
using System.Collections;
using System.Collections.Generic;
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
        private readonly IActorRef _binManager;

        public BinController(AxSystem sys)
        {
            // TODO: Store the BinManager in a local
            _binManager = sys.BinManager;
        }

        [HttpPost]
        public async Task<IActionResult> CreateBin()
        {
            var command = new BinManager.CreateBin();

            // TODO: Ask the bin manager to create a bin
            // TODO: Build and return a response model using the created bin
            
            return Ok();
        }

        [HttpGet]
        public async Task<IActionResult> ListBins()
        {
            var command = new BinManager.ListBins();

            // TODO: Ask the bin manager to list all the bins
            // TODO: Build a response model from the collection of bins

            return Ok();
        }

        [Route("{bin}", Name = nameof(CaptureRequest))]
        [AcceptVerbs("GET","POST","PUT","DELETE","PATCH","OPTIONS","HEAD","TRACE")]
        public async Task<IActionResult> CaptureRequest(string bin)
        {
            object requestModel = await CaptureHttpRequestInfo();
            var command = new BinManager.CaptureRequest(bin, requestModel);

            // TODO: Ask the bin manager to capture the request

            return Ok();
        }

        [Route("/logs/{bin}", Name = nameof(ListRequests))]
        public async Task<IActionResult> ListRequests(string bin)
        {
            var command = new BinManager.ListRequests(bin);

            // TODO: Ask the bin manager to list the contents of a bin
            // TODO: Construct a response model using the bin and items in the response

            return Ok();
        }

        private object BuildResponseModel(IEnumerable<string> bins)
        {
            return new { bins = bins.Select(BuildResponseModel) };
        }

        private object BuildResponseModel(string bin)
        {
            return new
            {
                bin,
                CaptureUrl = Url.Link(nameof(CaptureRequest), new { bin }).ToLowerInvariant(),
                LogUrl = Url.Link(nameof(ListRequests), new { bin }).ToLowerInvariant()
            };
        }

        private object BuildResponseModel(string bin, IReadOnlyList<object> items)
        {
            return new
            {
                bin,
                Requests = items,
                CaptureUrl = Url.Link(nameof(CaptureRequest), new { bin }).ToLowerInvariant(),
                LogUrl = Url.Link(nameof(ListRequests), new { bin }).ToLowerInvariant()
            };
        }

        private async Task<object> CaptureHttpRequestInfo()
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
