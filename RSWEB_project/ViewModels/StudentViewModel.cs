using Microsoft.AspNetCore.Http;
using RSWEB_project.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace RSWEB_project.ViewModels
{
    public class StudentViewModel
    {
        public Student Student { get; set; }

        [Display(Name = "Profile Image")]
        public IFormFile ProfileImage { get; set; }
    }
}
