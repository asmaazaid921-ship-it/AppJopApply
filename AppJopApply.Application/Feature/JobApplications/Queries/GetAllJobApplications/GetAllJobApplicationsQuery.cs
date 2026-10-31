using AppJopApply.Domain.Entities;
using MediatR;

namespace AppJopApply.Application.Feature.JobApplications.Queries.GetAllJobApplications
{
    public class GetAllJobApplicationsQuery : IRequest<IEnumerable<JobCandidateApplication>>
    {
    }
}
