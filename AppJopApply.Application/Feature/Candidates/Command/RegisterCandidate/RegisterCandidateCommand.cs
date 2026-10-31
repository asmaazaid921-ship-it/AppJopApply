using MediatR;

namespace AppJopApply.Application.Feature.Candidates.Command.RegisterCandidate
{
    public class RegisterCandidateCommand : IRequest<int>
    {
        public required string Name { get; set; }
        public required string CvUrl { get; set; }
    }
}
