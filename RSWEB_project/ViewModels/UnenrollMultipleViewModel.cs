using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace RSWEB_project.ViewModels
{
    public class UnenrollMultipleViewModel
    {
        public int? CourseId { get; set; }

        public int? Year { get; set; }

        public IList<long> EnrollmentIds { get; set; }

        [Required]
        [Display(Name = "Finish Date")]
        [DataType(DataType.Date)]
        public DateTime FinishDate { get; set; }
    }
}
