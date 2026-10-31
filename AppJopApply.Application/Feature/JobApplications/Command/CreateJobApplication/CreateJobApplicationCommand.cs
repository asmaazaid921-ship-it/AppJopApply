using MediatR;

namespace AppJopApply.Application.Feature.JobApplications.Command.CreateJobApplication
{
    public class CreateJobApplicationCommand : IRequest<int>
    {
        public int CandidateId { get; set; }
        public int JobId { get; set; }
    }
}
