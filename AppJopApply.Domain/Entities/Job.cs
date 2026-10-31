using System;
using System.Collections.Generic;
using System.Text;

namespace AppJopApply.Domain.Entities
{
    public class Job
    {
        public int Id { get; set; }
        public required string Title { get; set; }
        public required string Description { get; set; }
        public bool IsActive { get; set; }
    }
}
