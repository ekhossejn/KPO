using Microsoft.AspNetCore.Mvc;
using Zoo.Application.Interfaces;
using Zoo.Application.Services;
using Zoo.Domain.Entities;
using Zoo.Domain.ValueObjects;
using Zoo.Infrastructure.Interfaces;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Zoo.Presentation.Controllers
{
    [ApiController]
    [Route("api/animals")]
    public class AnimalsController : ControllerBase
    {
        private readonly IAnimalRepository _animalRepository;
        private readonly IAnimalTransferService _transferService;
        private readonly IAnimalReleaseService _animalReleaseService;
        private readonly IFeedingOrganizationService _feedingOrganizationService;

        public AnimalsController(
            IAnimalRepository animalRepository,
            IAnimalTransferService transferService,
            IAnimalReleaseService animalReleaseService,
            IFeedingOrganizationService feedingOrganizationService)
        {
            _animalRepository = animalRepository;
            _transferService = transferService;
            _animalReleaseService = animalReleaseService;
            _feedingOrganizationService = feedingOrganizationService;
        }

        [ProducesResponseType(typeof(Animal), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
        [HttpPut("add_animal")]
        public async Task<ActionResult<Guid>> AddAnimal(AddAnimalDto dto)
        {
            try
            {
                var animal = new Animal(dto.Species, dto.Name, new DateField(dto.BirthDate), dto.Gender, dto.FavouriteFood,
                    dto.HealthStatus, dto.SuitableEnclosureType);
                await _animalRepository.AddAnimalAsync(animal);
                return CreatedAtAction(nameof(GetAnimalById), new { id = animal.Id }, animal);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [ProducesResponseType(typeof(Animal), StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
        [HttpPost("release_animal/{id}")]
        public async Task<ActionResult<Guid>> ReleaseAnimal(Guid id)
        {
            var animal = await _animalRepository.GetAnimalByIdAsync(id);
            if (animal == null)
            {
                return NotFound();
            }
            await _animalReleaseService.ReleaseAnimalAsync(id);
            return NoContent();
        }

        [ProducesResponseType(typeof(IEnumerable<Animal>), StatusCodes.Status200OK)]
        [HttpGet("get_animals")]
        public async Task<IActionResult> GetAnimals()
        {
            var animals = await _animalRepository.GetAnimalsAsync();
            return Ok(animals);
        }

        [ProducesResponseType(typeof(Animal), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
        [HttpGet("get_animal/{id}")]
        public async Task<IActionResult> GetAnimalById(Guid id)
        {
            var animal = await _animalRepository.GetAnimalByIdAsync(id);
            return animal != null ? Ok(animal) : NotFound();
        }

        [HttpPatch("transfer_animal")]
        [ProducesResponseType(typeof(string), StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> TransferAnimalToEnclosure([FromBody] TransferAnimalDto data)
        {
            try
            {
                await _transferService.TransferAnimalAsync(data.AnimalId, data.FinalEnclosureId);
            }
            catch (ArgumentException ex)
            {
                return NotFound(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
            return NoContent();
        }

        [ProducesResponseType(typeof(string), StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
        [HttpPatch("feed_animal_without_schedule/{id}")]
        public async Task<IActionResult> FeedAnimalWithoutSchedule(Guid id)
        {
            var animal = await _animalRepository.GetAnimalByIdAsync(id);
            if (animal == null)
            {
                return NotFound();
            }

            animal.Feed();
            await _animalRepository.UpdateAnimalAsync(animal);
            return NoContent();
        }

        [ProducesResponseType(typeof(string), StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
        [HttpPatch("feed_animal_with_schedule/{id}")]
        public async Task<IActionResult> FeedAnimalWithSchedule(Guid id, Guid scheduleId)
        {
            try
            {
                await _feedingOrganizationService.FeedAnimalAsync(id, scheduleId);
            }
            catch (ArgumentException ex)
            {
                return NotFound(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
            return NoContent();
        }

        [ProducesResponseType(typeof(string), StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
        [HttpPatch("cure_animal/{id}")]
        public async Task<IActionResult> CureAnimal(Guid id)
        {
            var animal = await _animalRepository.GetAnimalByIdAsync(id);
            if (animal == null)
            {
                return NotFound();
            }
            animal.Cure();
            return NoContent();
        }

        [ProducesResponseType(typeof(string), StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
        [HttpPatch("animal_became_hungry/{id}")]
        public async Task<IActionResult> BecameHungry(Guid id)
        {
            var animal = await _animalRepository.GetAnimalByIdAsync(id);
            if (animal == null)
            {
                return NotFound();
            }
            animal.BecameHungry();
            await _animalRepository.UpdateAnimalAsync(animal);
            return NoContent();
        }

        [ProducesResponseType(typeof(string), StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
        [HttpPatch("animal_became_sick/{id}")]
        public async Task<IActionResult> BecameSick(Guid id)
        {
            var animal = await _animalRepository.GetAnimalByIdAsync(id);
            if (animal == null)
            {
                return NotFound();
            }
            animal.BecameSick();
            await _animalRepository.UpdateAnimalAsync(animal);
            return NoContent();
        }

        public record AddAnimalDto(
            AnimalSpecies Species,
            string Name,
            string BirthDate,
            AnimalGender Gender,
            string FavouriteFood,
            AnimalHealthStatus HealthStatus,
            EnclosureType SuitableEnclosureType
            );

        public record TransferAnimalDto(
            Guid AnimalId,
            Guid FinalEnclosureId
            );
    }
}

