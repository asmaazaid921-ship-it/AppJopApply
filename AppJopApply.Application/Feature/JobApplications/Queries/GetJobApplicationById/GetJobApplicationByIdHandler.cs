using AppJopApply.Application.Interfaces;
using AppJopApply.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace AppJopApply.Application.Feature.JobApplications.Queries.GetJobApplicationById
{
    public class GetJobApplicationByIdHandler : IRequestHandler<GetJobApplicationByIdQuery, JobCandidateApplication?>
    {
        private readonly IRepository<JobCandidateApplication> _applicationRepository;

        public GetJobApplicationByIdHandler(IRepository<JobCandidateApplication> applicationRepository)
        {
            _applicationRepository = applicationRepository;
        }

        public async Task<JobCandidateApplication?> Handle(GetJobApplicationByIdQuery request, CancellationToken cancellationToken)
        {
            return await _applicationRepository.Get()
                .FirstOrDefaultAsync(a => a.Id == request.Id, cancellationToken);
        }
    }
}
