using Ses.AspNetCore.Entities.Enum;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Ses.AspNetCore.ViewModes
{
    public class AddUserViewModel : UserInputBase
    {
        [Required]
        public string UserId { get; set; }
        [Required]
        public string RealName { get; set; }
        [Required]
        public GenderEnum? Gender { get; set; }
    }
}
