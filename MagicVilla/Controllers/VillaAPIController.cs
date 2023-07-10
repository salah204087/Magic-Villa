using AutoMapper;
using MagicVilla.Data;
using MagicVilla.Logging;
using MagicVilla.Models;
using MagicVilla.Models.DTO;
using MagicVilla.Repository.IRepository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Net;

namespace MagicVilla.Controllers
{
    [Route("api/VillaAPI")]
    [ApiController]
    public class VillaAPIController:ControllerBase
    {
        private readonly ILogging _logger;
        private readonly IVillaRepository _contextVilla;
        private readonly IMapper _mapper;
        protected APIResponse _response;
        public VillaAPIController(ILogging logger, IVillaRepository contextVilla,IMapper mapper)
        {
            _logger = logger;
            _contextVilla = contextVilla;
            _mapper=mapper;
            this._response = new();
        }


        [HttpGet]
        [ResponseCache(CacheProfileName = "Default30")]
		[ProducesResponseType(StatusCodes.Status403Forbidden)]
		[ProducesResponseType(StatusCodes.Status401Unauthorized)]
		[ProducesResponseType(StatusCodes.Status200OK)]
        public ActionResult <APIResponse> GetVillas([FromQuery]string? search) 
        {
            try
            {
                IEnumerable<Villa> villas= _contextVilla.GetAll(); ;
                if (!string.IsNullOrEmpty(search))
                {
                    villas=villas.Where(n=>n.Name.ToLower().Contains(search)
                    );
                }
                
                _response.Result = _mapper.Map<List<VillaDTO>>(villas);
                _response.StatusCode = HttpStatusCode.OK;
                _logger.Log("Getting all villas", "");
                return Ok(_response);
            }catch (Exception ex) 
            {
                _response.IsSuccess = false;
                _response.ErrorMessages=new List<string> {ex.ToString() };
            }
            return _response;
           

        }
        [HttpGet("{id:int}",Name ="GetVilla")]
        //[ResponseCache(Duration = 30)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
		[ProducesResponseType(StatusCodes.Status401Unauthorized)]
		[ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult <APIResponse> GetVilla(int id)
        {
            try
            {
                if (id == 0)
                {
                    _logger.Log("Get villa error with id:" + id, "error");
                    _response.IsSuccess = false;
                    return BadRequest();
                }
                var villa = _contextVilla.Get(n => n.Id == id);
                if (villa == null)
                {
                    _response.IsSuccess = false;
                    return NotFound();
                }
                _response.Result = _mapper.Map<VillaDTO>(villa);
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
        [Authorize(Roles = "admin")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult <APIResponse> CreateVilla([FromBody]VillaCreateDTO createDTO)
        {
            try
            {
                if (_contextVilla.Get(n => n.Name.ToLower() == createDTO.Name.ToLower()) != null)
                {
                    ModelState.AddModelError("ErrorMessages", "Villa already exist");
                    _response.IsSuccess = false;
                    return BadRequest(ModelState);
                }
                if (createDTO == null)
                {
                    _response.IsSuccess = false;
                    return BadRequest(createDTO); 
                }
                Villa model = _mapper.Map<Villa>(createDTO);
                _contextVilla.Create(model);
                _response.Result = _mapper.Map<VillaDTO>(model);
                _response.StatusCode = HttpStatusCode.Created;
                return CreatedAtRoute("GetVilla", new { id = model.Id }, _response);
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.ErrorMessages = new List<string> { ex.ToString() };
            }
            return _response;

        }
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [HttpDelete("{id:int}",Name ="DeleteVilla")]
        [Authorize(Roles = "admin")]
        public ActionResult<APIResponse> DeleteVilla(int id)
        {
            try
            {
                if (id == 0)
                {
                    _response.IsSuccess = false;
                    return BadRequest(); 
                }
                var villa = _contextVilla.Get(n => n.Id == id);
                if (villa == null)
                {
                    _response.IsSuccess = false;
                    return NotFound(); 
                }
                _contextVilla.Remove(villa);
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
		[ProducesResponseType(StatusCodes.Status403Forbidden)]
		[ProducesResponseType(StatusCodes.Status401Unauthorized)]
		[ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [HttpPut("{id:int}", Name = "UpdateVilla")]
		[Authorize(Roles = "admin")]
		public ActionResult<APIResponse> UpdateVilla(int id, [FromBody]VillaUpdateDTO updateDTO)
        {
            try
            {
                if (updateDTO == null || id != updateDTO.Id)
                {
                    _response.IsSuccess = false;
                    return BadRequest();
                }

                Villa model = _mapper.Map<Villa>(updateDTO);

                _contextVilla.Update(model);
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
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [HttpPatch("{id:int}", Name = "UpdatePartialVilla")]
		[Authorize(Roles = "admin")]
		public IActionResult UpdatePartialVilla(int id,JsonPatchDocument<VillaUpdateDTO> patchDTO)
        {
            if (patchDTO == null || id == 0)
            {
                _response.IsSuccess = false;
                return BadRequest();
            }
            var villa = _contextVilla.Get(n => n.Id == id,tracked:false);

            VillaUpdateDTO villaDTO =_mapper.Map<VillaUpdateDTO>(villa);

            if (villa == null)
            {
                _response.IsSuccess = false;
                return NotFound();
            }
            patchDTO.ApplyTo(villaDTO, ModelState);

            Villa model = _mapper.Map<Villa>(villaDTO);

            _contextVilla.Update(model);
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            return NoContent();

        }
       
    }
}
