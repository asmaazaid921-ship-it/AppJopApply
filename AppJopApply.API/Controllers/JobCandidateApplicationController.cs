using AppJopApply.Application.DTOs;
using AppJopApply.Application.Feature.JobApplications.Command.CreateJobApplication;
using AppJopApply.Application.Feature.JobApplications.Command.UpdateJobApplicationStatus;
using AppJopApply.Application.Feature.JobApplications.Queries.GetAllJobApplications;
using AppJopApply.Application.Feature.JobApplications.Queries.GetJobApplicationById;
using AppJopApply.Domain.Constants;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AppJopApply.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class JobCandidateApplicationController : ControllerBase
    {
        private readonly IMediator _mediator;

        public JobCandidateApplicationController(IMediator mediator)
        {
            _mediator = mediator;
        }

        /// <summary>Create a new job application (Candidate or Admin).</summary>
        [HttpPost]
        [Authorize(Roles = $"{AppRoles.Candidate},{AppRoles.Admin}")]
        public async Task<IActionResult> Create([FromBody] CreateJobApplicationDto dto)
        {
            var id = await _mediator.Send(new CreateJobApplicationCommand
            {
                CandidateId = dto.CandidateId,
                JobId = dto.JobId
            });
            return Ok(new { ApplicationId = id });
        }

        /// <summary>Update the status of a job application (Admin and HR only).</summary>
        [HttpPut("{id}/status")]
        [Authorize(Roles = $"{AppRoles.Admin},{AppRoles.HR}")]
        public async Task<IActionResult> UpdateStatus(int id, [FromBody] UpdateJobApplicationStatusDto dto)
        {
            var success = await _mediator.Send(new UpdateJobApplicationStatusCommand
            {
                ApplicationId = id,
                Status = dto.Status
            });

            if (!success)
                return NotFound(new { Message = $"Application with id {id} was not found." });

            return Ok();
        }

        /// <summary>Get all job applications (Admin and HR only).</summary>
        [HttpGet]
        [Authorize(Roles = $"{AppRoles.Admin},{AppRoles.HR}")]
        public async Task<IActionResult> GetAll()
        {
            var applications = await _mediator.Send(new GetAllJobApplicationsQuery());
            return Ok(applications);
        }

        /// <summary>Get a job application by ID (Admin, HR, and Candidate).</summary>
        [HttpGet("{id}")]
        [Authorize(Roles = $"{AppRoles.Admin},{AppRoles.HR},{AppRoles.Candidate}")]
        public async Task<IActionResult> GetById(int id)
        {
            var application = await _mediator.Send(new GetJobApplicationByIdQuery { Id = id });

            if (application is null)
                return NotFound(new { Message = $"Application with id {id} was not found." });

            return Ok(application);
        }
    }
}
