using AppJopApply.Domain.Entities;
using MediatR;

namespace AppJopApply.Application.Feature.JobApplications.Queries.GetJobApplicationById
{
    public class GetJobApplicationByIdQuery : IRequest<JobCandidateApplication?>
    {
        public int Id { get; set; }
    }
}
