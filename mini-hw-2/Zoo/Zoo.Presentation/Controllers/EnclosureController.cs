using System;
using Microsoft.AspNetCore.Mvc;
using Zoo.Domain.Entities;
using Zoo.Domain.ValueObjects;
using Zoo.Infrastructure.Interfaces;

namespace Zoo.Presentation.Controllers
{
    [Route("api/enclosures")]
    [ApiController]
    public class EnclosureController : ControllerBase
    {
        private readonly IEnclosureRepository _enclosureRepository;

        public EnclosureController(IEnclosureRepository enclosureRepository)
        {
            _enclosureRepository = enclosureRepository;
        }

        [ProducesResponseType(typeof(Enclosure), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
        [HttpPut("add_enclosure")]
        public async Task<IActionResult> AddEnclosure([FromBody] AddEnclosureDto dto)
        {
            try
            {
                Enclosure enclosure = new(dto.Type, new PositiveInt(dto.AreaM2), new PositiveInt(dto.Capacity));
                await _enclosureRepository.AddEnclosureAsync(enclosure);
                return CreatedAtAction(nameof(GetEnclosureById), new { id = enclosure.Id }, enclosure);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [ProducesResponseType(typeof(IEnumerable<Enclosure>), StatusCodes.Status200OK)]
        [HttpGet("get_enclosures")]
        public async Task<IActionResult> GetEnclosures()
        {
            var enclosures = await _enclosureRepository.GetEnclosuresAsync();
            return Ok(enclosures);
        }

        [ProducesResponseType(typeof(Enclosure), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
        [HttpGet("get_enclosure/{id}")]
        public async Task<IActionResult> GetEnclosureById(Guid id)
        {
            var enclosure = await _enclosureRepository.GetEnclosureByIdAsync(id);
            return enclosure != null ? Ok(enclosure) : NotFound();
        }

        [ProducesResponseType(typeof(Enclosure), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
        [HttpGet("get_enclosure_by_animalId/{animalId}")]
        public async Task<IActionResult> GetEnclosureByAnimalId(Guid animalId)
        {
            var enclosure = await _enclosureRepository.GetEnclosureByAnimalIdAsync(animalId);
            return enclosure != null ? Ok(enclosure) : NotFound();
        }

        [ProducesResponseType(typeof(string), StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
        [HttpPatch("clean_enclosure/{id}")]
        public async Task<IActionResult> CleanEnclosure(Guid id)
        {
            var enclosure = await _enclosureRepository.GetEnclosureByIdAsync(id);
            if (enclosure == null)
            {
                return NotFound();
            }
            enclosure.CleanUp();
            return NoContent();
        }

        [ProducesResponseType(typeof(string), StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
        [HttpPatch("enclosure_became_dirty/{id}")]
        public async Task<IActionResult> BecameDirty(Guid id)
        {
            var enclosure = await _enclosureRepository.GetEnclosureByIdAsync(id);
            if (enclosure == null)
            {
                return NotFound();
            }
            enclosure.BecameDirty();
            return NoContent();
        }

        public record AddEnclosureDto(
            EnclosureType Type,
            int AreaM2,
            int Capacity
            );
    }
}

