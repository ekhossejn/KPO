using System;
using Microsoft.AspNetCore.Mvc;
using Zoo.Application.Interfaces;

namespace Zoo.Presentation.Controllers
{
    [Route("api/statistics")]
    [ApiController]
    public class StatisticsController : ControllerBase
    {
        private readonly IZooStatisticsService _statisticsService;

        public StatisticsController(IZooStatisticsService statisticsService)
        {
            _statisticsService = statisticsService;
        }

        [HttpGet("get_animals_number")]
        [ProducesResponseType(typeof(int), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAnimalsNumber()
        {
            var count = await _statisticsService.GetAnimalsNumberAsync();
            return Ok(count);
        }

        [HttpGet("get_animals_number/healthy")]
        [ProducesResponseType(typeof(int), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetHealthyAnimalsNumber()
        {
            var count = await _statisticsService.GetHealthyAnimalsNumberAsync();
            return Ok(count);
        }

        [HttpGet("get_enclosures_number")]
        [ProducesResponseType(typeof(int), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetEnclosuresNumber()
        {
            var count = await _statisticsService.GetEnclosuresNumberAsync();
            return Ok(count);
        }

        [HttpGet("get_schedules_number")]
        [ProducesResponseType(typeof(int), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetFeedingSchedulesNumber()
        {
            var count = await _statisticsService.GetFeedingSchedulesNumberAsync();
            return Ok(count);
        }
    }
}

