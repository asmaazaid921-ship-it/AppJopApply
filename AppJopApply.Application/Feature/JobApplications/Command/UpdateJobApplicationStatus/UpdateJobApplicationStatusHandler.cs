using AppJopApply.Application.Interfaces;
using AppJopApply.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace AppJopApply.Application.Feature.JobApplications.Command.UpdateJobApplicationStatus
{
    public class UpdateJobApplicationStatusHandler : IRequestHandler<UpdateJobApplicationStatusCommand, bool>
    {
        private readonly IRepository<JobCandidateApplication> _applicationRepository;

        public UpdateJobApplicationStatusHandler(IRepository<JobCandidateApplication> applicationRepository)
        {
            _applicationRepository = applicationRepository;
        }

        public async Task<bool> Handle(UpdateJobApplicationStatusCommand request, CancellationToken cancellationToken)
        {
            var application = await _applicationRepository.Get()
                .FirstOrDefaultAsync(a => a.Id == request.ApplicationId, cancellationToken);

            if (application is null)
                return false;

            application.JobApplicationStatus = request.Status;
            application.StatusUpdatedAt = DateTime.UtcNow;

            _applicationRepository.Update(application);
            await _applicationRepository.SaveChangesAsync();

            return true;
        }
    }
}
