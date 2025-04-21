using System;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Zoo.Domain.Entities;
using Zoo.Domain.Events;
using Zoo.Domain.ValueObjects;
using Zoo.Infrastructure.Interfaces;

namespace Zoo.Presentation.Controllers
{
    [Route("api/schedule")]
    [ApiController]
    public class FeedingScheduleController : ControllerBase
    {
        private readonly IFeedingScheduleRepository _feedingScheduleRepository;
        private readonly IAnimalRepository _animalRepository;

        public FeedingScheduleController(IFeedingScheduleRepository feedingScheduleRepository,
            IAnimalRepository animalRepository)
        {
            _feedingScheduleRepository = feedingScheduleRepository;
            _animalRepository = animalRepository;
        }

        [ProducesResponseType(typeof(FeedingSchedule), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
        [HttpPut("add_schedule")]
        public async Task<IActionResult> AddSchedule([FromBody] AddFeedingScheduleDto dto)
        {
            FeedingSchedule feedingSchedule;
            try
            {
                feedingSchedule = new FeedingSchedule(
                    dto.AnimalId,
                    new TimeField(dto.FeedingTime),
                    dto.Food
                );
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }

            var animal = await _animalRepository.GetAnimalByIdAsync(dto.AnimalId);
            if (animal == null)
            {
                return NotFound("Animal not found.");
            }

            await _feedingScheduleRepository.AddScheduleAsync(feedingSchedule);
            return CreatedAtAction(nameof(GetScheduleById), new { id = feedingSchedule.Id }, feedingSchedule);
        }

        [ProducesResponseType(typeof(IEnumerable<FeedingSchedule>), StatusCodes.Status200OK)]
        [HttpGet("get_schedules")]
        public async Task<IActionResult> GetSchedules()
        {
            var schedules = await _feedingScheduleRepository.GetSchedulesAsync();
            return Ok(schedules.OrderBy(schedule => schedule.FeedingTime));
        }

        [ProducesResponseType(typeof(FeedingSchedule), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
        [HttpGet("get_schedule/{id}")]
        public async Task<IActionResult> GetScheduleById(Guid id)
        {
            var schedule = await _feedingScheduleRepository.GetScheduleByIdAsync(id);
            return schedule != null ? Ok(schedule) : NotFound();
        }

        [ProducesResponseType(typeof(IEnumerable<FeedingSchedule>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
        [HttpGet("get_schedule_by_animalId/{animalId}")]
        public async Task<IActionResult> GetScheduleByAnimalId(Guid animalId)
        {
            var schedules = await _feedingScheduleRepository.GetScheduleByAnimalIdAsync(animalId);
            return schedules != null ? Ok(schedules) : NotFound();
        }

        public record AddFeedingScheduleDto(
            Guid AnimalId,
            string FeedingTime,
            string Food
            );
    }
}

