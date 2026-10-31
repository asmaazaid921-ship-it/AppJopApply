using AppJopApply.Domain.Entities;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace AppJopApply.Application.Feature.Jobs.Queries.GetJobById
{
    public class GetJobByIdQuery: IRequest<Job?>
    {
        public int Id { get; set; }
    }
}
