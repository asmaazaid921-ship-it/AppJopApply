using AppJopApply.Application.DTOs;
using AppJopApply.Application.Feature.Jobs.Command.CreateJob;
using AppJopApply.Application.Feature.Jobs.Queries.GetAllGob;
using AppJopApply.Application.Feature.Jobs.Queries.GetJobById;
using AppJopApply.Domain.Constants;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AppJopApply.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class JobController : ControllerBase
    {
        private readonly IMediator _mediator;

        public JobController(IMediator mediator)
        {
            _mediator = mediator;
        }

        /// <summary>Create a new job (Admin and HR only).</summary>
        [HttpPost]
        [Authorize(Roles = $"{AppRoles.Admin},{AppRoles.HR}")]
        public async Task<IActionResult> CreateJob([FromBody] CreateJobDto createJobDto)
        {
            var id = await _mediator.Send(new CreateJobCommand { Title = createJobDto.Title, Description = createJobDto.Description });
            return Ok(new { JobId = id });
        }

        /// <summary>Get all jobs (Candidates, HR, and Admin).</summary>
        [HttpGet]
        [Authorize(Roles = $"{AppRoles.Admin},{AppRoles.HR},{AppRoles.Candidate}")]
        public async Task<IActionResult> GetAllJobs()
        {
            var jobs = await _mediator.Send(new GetAllJobsQuery());
            return Ok(jobs);
        }

        /// <summary>Get job by ID (Candidates, HR, and Admin).</summary>
        [HttpGet("{id}")]
        [Authorize(Roles = $"{AppRoles.Admin},{AppRoles.HR},{AppRoles.Candidate}")]
        public async Task<IActionResult> GetJobById(int id)
        {
            var job = await _mediator.Send(new GetJobByIdQuery { Id = id });

            if (job is null)
                return NotFound(new { Message = $"Job with id {id} was not found." });

            return Ok(job);
        }
    }
}
