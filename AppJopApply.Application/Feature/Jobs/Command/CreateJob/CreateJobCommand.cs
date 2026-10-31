using System;
using System.Collections.Generic;
using System.Text;
using MediatR;


namespace AppJopApply.Application.Feature.Jobs.Command.CreateJob
{
    public class CreateJobCommand : IRequest<int>
    {
        public required string Title { get; set; }
        public required string Description { get; set; }
    }
}
