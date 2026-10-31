using AppJopApply.Application.Interfaces;
using AppJopApply.Domain.Entities;
using MediatR;

namespace AppJopApply.Application.Feature.Candidates.Command.RegisterCandidate
{
    public class RegisterCandidateHandler : IRequestHandler<RegisterCandidateCommand, int>
    {
        private readonly IRepository<Candidate> _candidateRepository;

        public RegisterCandidateHandler(IRepository<Candidate> candidateRepository)
        {
            _candidateRepository = candidateRepository;
        }

        public async Task<int> Handle(RegisterCandidateCommand request, CancellationToken cancellationToken)
        {
            var candidate = new Candidate
            {
                Name = request.Name,
                CvUrl = request.CvUrl
            };

            await _candidateRepository.AddAsync(candidate);
            await _candidateRepository.SaveChangesAsync();

            return candidate.Id;
        }
    }
}
