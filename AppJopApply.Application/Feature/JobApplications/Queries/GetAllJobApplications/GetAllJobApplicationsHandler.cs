using AppJopApply.Application.Interfaces;
using AppJopApply.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace AppJopApply.Application.Feature.JobApplications.Queries.GetAllJobApplications
{
    public class GetAllJobApplicationsHandler : IRequestHandler<GetAllJobApplicationsQuery, IEnumerable<JobCandidateApplication>>
    {
        private readonly IRepository<JobCandidateApplication> _applicationRepository;

        public GetAllJobApplicationsHandler(IRepository<JobCandidateApplication> applicationRepository)
        {
            _applicationRepository = applicationRepository;
        }

        public async Task<IEnumerable<JobCandidateApplication>> Handle(GetAllJobApplicationsQuery request, CancellationToken cancellationToken)
        {
            return await _applicationRepository.Get().ToListAsync(cancellationToken);
        }
    }
}
