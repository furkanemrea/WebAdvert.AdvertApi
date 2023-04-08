using AdvertApi.Models;
using AdvertApi.Models.Response;
using AdvertApi.Services.Abstract;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.FileProviders;

namespace AdvertApi.Controllers
{
    [Route("adverts/v1")]
    [ApiController]
    public class AdvertController : ControllerBase
    {
        private readonly IAdvertStorageService _advertStorageService;

        public AdvertController(IAdvertStorageService advertStorageService)
        {
            _advertStorageService = advertStorageService;
        }

        [HttpPost("create")]
        [ProducesResponseType(400)]
        [ProducesResponseType(200,Type = typeof(CreateAdvertResponse))]
        public async Task<IActionResult> Create(AdvertModel advertModel)
        {
            string recordId = string.Empty;
            try
            {
                recordId = await _advertStorageService.Add(advertModel);
            }
            catch (KeyNotFoundException)
            {
                return new NotFoundResult() { };
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }


            return StatusCode(201, new CreateAdvertResponse() { Id = recordId });
        }

        [HttpPost]
        [Route("confirm")]
        [ProducesResponseType(400)]
        [ProducesResponseType(200)]
        public async Task<IActionResult> Confirm(ConfirmAdvertModel confirmAdvertModel)
        {
            try
            {
                await _advertStorageService.Confirm(confirmAdvertModel);
            }
            catch(KeyNotFoundException)
            {
                return new NotFoundResult();
            }
            catch (Exception ex )
            {
                return StatusCode(500,ex.Message);
            }

            return new OkResult();
        }           

    }
}
