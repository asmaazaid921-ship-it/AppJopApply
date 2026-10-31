using AppJopApply.Application.DTOs;
using AppJopApply.Application.Feature.Candidates.Command.RegisterCandidate;
using AppJopApply.Application.Feature.JobApplications.Command.CreateJobApplication;
using AppJopApply.Application.Feature.JobApplications.Command.UpdateJobApplicationStatus;
using AppJopApply.Domain.Constants;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AppJopApply.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class CandidateController : ControllerBase
    {
        private readonly IMediator _mediator;

        public CandidateController(IMediator mediator)
        {
            _mediator = mediator;
        }

        /// <summary>Register a new candidate profile (Candidate or Admin).</summary>
        [HttpPost]
        [Authorize(Roles = $"{AppRoles.Candidate},{AppRoles.Admin}")]
        public async Task<IActionResult> Register([FromBody] CreateCandidateDto dto)
        {
            var id = await _mediator.Send(new RegisterCandidateCommand
            {
                Name = dto.Name,
                CvUrl = dto.CvUrl
            });
            return Ok(new { CandidateId = id });
        }

        /// <summary>Apply a candidate to a job (Candidate or Admin).</summary>
        [HttpPost("{candidateId}/apply")]
        [Authorize(Roles = $"{AppRoles.Candidate},{AppRoles.Admin}")]
        public async Task<IActionResult> ApplyToJob(int candidateId, [FromBody] ApplyToJobDto dto)
        {
            var id = await _mediator.Send(new CreateJobApplicationCommand
            {
                CandidateId = candidateId,
                JobId = dto.JobId
            });
            return Ok(new { ApplicationId = id });
        }

        /// <summary>Update the status of an existing job application (Admin and HR only).</summary>
        [HttpPut("applications/{applicationId}/status")]
        [Authorize(Roles = $"{AppRoles.Admin},{AppRoles.HR}")]
        public async Task<IActionResult> UpdateApplicationStatus(int applicationId, [FromBody] UpdateJobApplicationStatusDto dto)
        {
            var success = await _mediator.Send(new UpdateJobApplicationStatusCommand
            {
                ApplicationId = applicationId,
                Status = dto.Status
            });

            if (!success)
                return NotFound(new { Message = $"Application with id {applicationId} was not found." });

            return Ok();
        }
    }
}
