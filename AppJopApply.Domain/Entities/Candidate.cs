using System;
using System.Collections.Generic;
using System.Text;

namespace AppJopApply.Domain.Entities
{
    public class Candidate
    {
        public int Id { get; set; }
        public required string Name { get; set; }
        public required string CvUrl { get; set; }
    }
}
