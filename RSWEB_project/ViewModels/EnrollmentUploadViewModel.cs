using Microsoft.AspNetCore.Http;
using RSWEB_project.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace RSWEB_project.ViewModels
{
    public class EnrollmentUploadViewModel
    {
        public Enrollment Enrollment { get; set; }

        [Display(Name = "Seminal File")]
        public IFormFile SeminalFile { get; set; }
    }
}
