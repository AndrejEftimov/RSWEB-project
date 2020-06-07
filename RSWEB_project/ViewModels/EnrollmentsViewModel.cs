using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace RSWEB_project.ViewModels
{
    public class EnrollmentsViewModel
    {
        [Required]
        public int Year { get; set; }

        [Required]
        public string Semester { get; set; }

        [Required]
        public int CourseId { get; set; }

        [Required]
        public IList<long> StudentIds { get; set; }
    }
}
