using AutoMapper;
using MagicVilla.Logging;
using MagicVilla.Models;
using MagicVilla.Models.DTO;
using MagicVilla.Repository.IRepository;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Net;

namespace MagicVilla.Controllers
{
    [Route("api/VillaNumberAPI")]
    [ApiController]
    public class VillaNumberAPIController : ControllerBase
    {
        private readonly IVillaNumberRepository _contextVillaNumber;
        private readonly IVillaRepository _contextVilla;
        private readonly IMapper _mapper;
        protected APIResponse _response;
        private readonly ILogging _logger;
        public VillaNumberAPIController(IVillaNumberRepository contextVillaNumber, IMapper mapper, ILogging logger, IVillaRepository contextVilla)
        {
            _contextVillaNumber = contextVillaNumber;
            _mapper = mapper;
            this._response = new();
            _logger = logger;
            _contextVilla = contextVilla;

        }
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public ActionResult<APIResponse> GetVillaNumbers()
        {
            try
            {
                IEnumerable<VillaNumber> villaNumbers = _contextVillaNumber.GetAll(includeproperties:"Villa");
                _response.Result = _mapper.Map<List<VillaNumberDTO>>(villaNumbers);
                _response.StatusCode = HttpStatusCode.OK;
                _logger.Log("Get all villa Numbers", "");
                return _response;
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.ErrorMessages = new List<string> { ex.ToString() };
            }
            return _response;
        }
        [HttpGet("{id:int}", Name = "GetVillaNumber")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public ActionResult<APIResponse> GetVillaNumber(int id)
        {
            try
            {
                if (id == 0)
                {
                    _logger.Log("Get Villa Number erroe with id" + id, "error");
                    _response.IsSuccess = false;
                    return BadRequest();
                }
                var villaNumber = _contextVillaNumber.Get(n => n.VillaNo == id);
                if (villaNumber == null)
                {
                    _response.IsSuccess = false;
                    return NotFound();
                }
                _response.Result = _mapper.Map<VillaNumberDTO>(villaNumber);
                _response.StatusCode = HttpStatusCode.OK;
                return Ok(_response);
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.ErrorMessages = new List<string> { ex.ToString() };
            }
            return _response;
        }
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status201Created)]
        public ActionResult<APIResponse> CreateVillaNumber([FromBody] VillaNumberCreateDTO createVillaNumber)
        {
            try
            {
                if (_contextVillaNumber.Get(n => n.VillaNo == createVillaNumber.VillaNo) != null)
                {
                    ModelState.AddModelError("ErrorMessages", "Villa Number already exist");
                    _response.IsSuccess = false;
                    return BadRequest(ModelState);
                }
                if (_contextVilla.Get(n=>n.Id== createVillaNumber.VillaID)==null)
                {
                    ModelState.AddModelError("ErrorMessages", "Villa ID is not valid");
                    _response.IsSuccess = false;
                    return BadRequest(ModelState);
                }

                if (createVillaNumber == null)
                {
                    _response.IsSuccess = false;
                    return BadRequest();
                }
                VillaNumber model = _mapper.Map<VillaNumber>(createVillaNumber);
                _contextVillaNumber.Create(model);
                _response.Result = _mapper.Map<VillaNumberCreateDTO>(model);
                _response.StatusCode = HttpStatusCode.Created;
                return Ok(_response);
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.ErrorMessages = new List<string> { ex.ToString() };
            }
            return _response;
        }
        [HttpDelete("{id:int}", Name = "DeleteVillaNumber")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<APIResponse> DeleteVillaNumber(int id)
        {
            try
            {
                if (id == 0)
                {
                    _response.IsSuccess = false;
                    return BadRequest();
                }
                var villaNumber = _contextVillaNumber.Get(n => n.VillaNo == id);
                if (villaNumber == null)
                {
                    _response.IsSuccess = false;
                    return NotFound();
                }
                _contextVillaNumber.Remove(villaNumber);
                _response.StatusCode = HttpStatusCode.NoContent;
                _response.IsSuccess = true;
                return Ok(_response);
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.ErrorMessages = new List<string> { ex.ToString() };
            }
            return _response;
        }
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [HttpPut("{id:int}", Name = "UpdateVillaNumber")]
        public ActionResult<APIResponse> UpdateVillaNumber(int id, [FromBody] VillaNumberUpdateDTO villaNumberUpdate)
        {
            try
            {
                if (villaNumberUpdate == null || id != villaNumberUpdate.VillaNo)
                {
                    _response.IsSuccess = false;
                    return BadRequest();
                }
                if (_contextVilla.Get(n => n.Id == villaNumberUpdate.VillaID) == null)
                {
                    ModelState.AddModelError("ErrorMessages", "Villa ID is not valid");
                    _response.IsSuccess = false;
                    return BadRequest(ModelState);
                }
                VillaNumber model = _mapper.Map<VillaNumber>(villaNumberUpdate);
                _contextVillaNumber.Update(model);
                _response.StatusCode = HttpStatusCode.NoContent;
                _response.IsSuccess = true;
                return Ok(_response);

            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.ErrorMessages = new List<string> { ex.ToString() };
            }
            return _response;
        }
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [HttpPatch("{id:int}", Name = "UpdatePartialVillaNumber")]
        public IActionResult UpdatePartialVillaNumeber(int id, JsonPatchDocument<VillaNumberUpdateDTO> patchDTO)
        {
            if (patchDTO == null || id == 0)
            {
                _response.IsSuccess = false;
                return BadRequest();
            }

            var villaNumber = _contextVillaNumber.Get(n => n.VillaNo == id, tracked: false);

            VillaNumberUpdateDTO villaNumberDTO = _mapper.Map<VillaNumberUpdateDTO>(villaNumber);

            if (villaNumber == null)
            {
                _response.IsSuccess = false;
                return NotFound();
            }
            patchDTO.ApplyTo(villaNumberDTO, ModelState);

            VillaNumber model = _mapper.Map<VillaNumber>(villaNumberDTO);

            _contextVillaNumber.Update(model);
            if (!ModelState.IsValid)
            {
                _response.IsSuccess = false;
                return BadRequest(ModelState);
            }
            return NoContent();

        }

    }
}
