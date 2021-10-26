using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace WebApplication_EProject.Models.NhanVienModel
{
    public class NhanVien
    {
        [Key]
        [Display(Name = "Employee ID")]
        public int Employee_ID { get; set; }

        [Display(Name = "Full Name")]
        [Required(ErrorMessage = "Please enter Last Name")]
        [StringLength(maximumLength: 50, ErrorMessage = "Please input Last Name from 2 to 50", MinimumLength = 2)]
        public string Ten { get; set; }

        [Required(ErrorMessage = "Please input email")]
        [RegularExpression(@"^([A-Za-z0-9_.]+)\@+(gmail|outlook|icloud|hotmail)*(.com|.uk|.vn|.us)$",
            ErrorMessage = "Wrong email form Ex: nameemail@gmail|outlook|icloud|hotmail.com|uk|vn|us")]
        public string Email { get; set; }

        [Required]
        [StringLength(maximumLength: 11, ErrorMessage = "Phone number invalid", MinimumLength = 10)]
        [RegularExpression(@"(0[3|5|7|8|9])+([0-9]{8})\b", ErrorMessage = "Nhập đúng theo dạng số của việt nam")]
        public string SDT { get; set; }

        [Editable(false)]
        [DataType(DataType.DateTime)]
        public DateTime NgayTao { get; set; }

        [Editable(false)]
        [DataType(DataType.DateTime)]
        public DateTime NgaySua { get; set; }


        [DataType(DataType.Date)]
        [Display(Name = "DOB")]
        public DateTime DOB { get; set; }

        [Required(ErrorMessage = "Please enter password")]
        [DataType(DataType.Password)]
        [StringLength(100, MinimumLength = 6)]
        [RegularExpression(@"^(?=.*?[A-Z])(?=.*?[a-z])(?=.*?[0-9])(?=.*?[#?!@$%<>'^&*-/|;:=_+{}()[~`]).{6,}$", ErrorMessage = "Password must contain at least: 1 UpperCase Alphabet, 1 LowerCase Alphabet, 1 Special Characters and 1 Number")]
        [Display(Name = "Mat Khau")]
        public string MatKhau { get; set; }

        [Required(ErrorMessage = "Phải gán role")]
        public int? Role_ID { get; set; }

        [Required]
        public int Status { get; set; }
        public virtual ICollection<ResetPass> ResetPass { get; set; }

        //public virtual ICollection<Request.Request> Request { get; set; }

        public virtual Role Role { get; set; }

        public virtual ICollection<QvA.Question> Question { get; set; }

        public virtual ICollection<QvA.Answer> Answers { get; set; }
    }
}