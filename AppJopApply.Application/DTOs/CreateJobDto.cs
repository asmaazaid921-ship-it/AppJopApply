using System;
using System.Collections.Generic;
using System.Text;

namespace AppJopApply.Application.DTOs
{
    public class CreateJobDto
    {
        public required string Title { get; set; }
        public required string Description { get; set; }
    }
}
