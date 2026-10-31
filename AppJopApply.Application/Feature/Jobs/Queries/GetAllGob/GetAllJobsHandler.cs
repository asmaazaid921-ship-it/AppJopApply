using AppJopApply.Application.Interfaces;
using Microsoft.EntityFrameworkCore;
using AppJopApply.Domain.Entities;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace AppJopApply.Application.Feature.Jobs.Queries.GetAllGob
{
    public class GetAllJobsHandler: IRequestHandler<GetAllJobsQuery, IEnumerable<Job>>
    {
        private readonly IRepository<Job> _jobRepository;

        public GetAllJobsHandler(IRepository<Job> jobRepository)
        {
            _jobRepository = jobRepository;
        }

        public async Task<IEnumerable<Job>> Handle(GetAllJobsQuery request, CancellationToken cancellationToken)
        {
            var jobs = await _jobRepository.Get().ToListAsync();
            return jobs;
        }
    
    }
}
