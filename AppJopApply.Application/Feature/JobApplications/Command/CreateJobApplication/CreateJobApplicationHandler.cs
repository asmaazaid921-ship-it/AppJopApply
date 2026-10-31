using AppJopApply.Application.Interfaces;
using AppJopApply.Domain.Entities;
using AppJopApply.Domain.Enums;
using MediatR;

namespace AppJopApply.Application.Feature.JobApplications.Command.CreateJobApplication
{
    public class CreateJobApplicationHandler : IRequestHandler<CreateJobApplicationCommand, int>
    {
        private readonly IRepository<JobCandidateApplication> _applicationRepository;

        public CreateJobApplicationHandler(IRepository<JobCandidateApplication> applicationRepository)
        {
            _applicationRepository = applicationRepository;
        }

        public async Task<int> Handle(CreateJobApplicationCommand request, CancellationToken cancellationToken)
        {
            var application = new JobCandidateApplication
            {
                CandidateId = request.CandidateId,
                JobId = request.JobId,
                JobApplicationStatus = JobApplicationStatus.Applied,
                AppliedAt = DateTime.UtcNow,
                StatusUpdatedAt = DateTime.UtcNow
            };

            await _applicationRepository.AddAsync(application);
            await _applicationRepository.SaveChangesAsync();

            return application.Id;
        }
    }
}
