using AppJopApply.Domain.Enums;
using MediatR;

namespace AppJopApply.Application.Feature.JobApplications.Command.UpdateJobApplicationStatus
{
    public class UpdateJobApplicationStatusCommand : IRequest<bool>
    {
        public int ApplicationId { get; set; }
        public JobApplicationStatus Status { get; set; }
    }
}
