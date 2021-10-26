using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace WebApplication_EProject.Models.NhanVienModel
{
    public class Role
    {
        [Key]
        public int Role_ID { get; set; }

        [Required]
        [Display(Name = "Roll Name")]
        public string RoleName { get; set; }

        //public virtual ICollection<NhanVien> NhanVienss { get; set; }

        //public ICollection<Request.Request> Request { get; set; }

        public ICollection<NhanVien> NhanVien { get; set; }

        public ICollection<QvA.Question> Question { get; set; }
    }
}