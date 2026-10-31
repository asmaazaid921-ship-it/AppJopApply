
using AppJopApply.Application.DTOs;
using AppJopApply.Application.Interfaces;
using AppJopApply.Domain.Entities;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace AppJopApply.Application.Feature.Jobs.Command.CreateJob
{
    
    public class CreateJobHandler : IRequestHandler<CreateJobCommand, int>
    {
        private readonly IRepository<Job> _jobRepositry;

        public CreateJobHandler(IRepository<Job> jobRepositry)
        {
            _jobRepositry = jobRepositry;
        }

        public async Task<int> Handle(CreateJobCommand request, CancellationToken cancellationToken)
        {

            var job = new Job
            {
                Title = request.Title,
                Description = request.Description,
                IsActive = true
            };
            await _jobRepositry.AddAsync(job);
            await _jobRepositry.SaveChangesAsync();

            return job.Id;
        }
    }
}
