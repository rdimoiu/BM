using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BuildingManagement.Utils;

namespace BuildingManagement.Models
{
    public class User
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required, StringLength(100)]
        [DisplayName("First name")]
        public string FirstName { get; set; }

        [Required, StringLength(100)]
        [DisplayName("Last name")]
        public string LastName { get; set; }

        [Required, StringLength(250)]
        [DisplayName("Email address")]
        public string Email { get; set; }

        [Required]
        [DisplayName("Confirmed")]
        public bool AccountConfirmed { get; set; }

        [Required, StringLength(150)]
        public string Password { get; set; }

        [NotMapped]
        [DisplayName("User role")]
        public int RoleType { get; set; }

        public virtual ICollection<UserRole> UserRoles { get; set; }

        public IEnumerable<SelectListItem> RoleTypeOptions = new List<SelectListItem>
        {
            new SelectListItem
                {
                    Value = ((int) Constants.RoleTypes.User).ToString(),
                    Text = Enum.GetName(typeof(Constants.RoleTypes), (int) Constants.RoleTypes.User)
                },
                new SelectListItem
                {
                    Value = ((int) Constants.RoleTypes.Admin).ToString(),
                    Text = Enum.GetName(typeof(Constants.RoleTypes), (int) Constants.RoleTypes.Admin)
                },
                new SelectListItem
                {
                    Value = ((int) Constants.RoleTypes.Sysadmin).ToString(),
                    Text = Enum.GetName(typeof(Constants.RoleTypes), (int) Constants.RoleTypes.Sysadmin)
                }
        };
    }
}