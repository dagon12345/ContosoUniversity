using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ContosoUniversity.Models
{
    public class Department
    {
        public int DepartmentID { get; set; }

        [StringLength(50, MinimumLength = 3)]
        public string Name { get; set; }

        /*Column mapping is generally not required. 
        EF Core chooses the appropriate SQL Server data type based on the CLR type for the property. 
        The CLR decimal type maps to a SQL Server decimal type. 
        Budget is for currency, and the money data type is more appropriate for currency.*/
        [DataType(DataType.Currency)]
        [Column(TypeName = "money")]
        public decimal Budget { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}",
                       ApplyFormatInEditMode = true)]
        [Display(Name = "Start Date")]
        public DateTime StartDate { get; set; }

/*A department may or may not have an administrator.
An administrator is always an instructor. 
Therefore the InstructorID property is included as the FK to the Instructor entity.*/
        public int? InstructorID { get; set; }

        public Instructor Administrator { get; set; }
        public ICollection<Course> Courses { get; set; }
    }
}