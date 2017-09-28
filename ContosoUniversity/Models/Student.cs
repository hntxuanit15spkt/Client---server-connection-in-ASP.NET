using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ContosoUniversity.Models
{
    //Conventions (quy ước): Tên bảng không nên là số nhiều (you specified that table names shouldn't be pluralized)
    public class Student
    {
        public int ID { get; set; }//By default, the Entity Framework interprets a property that's named ID or classname ID as the primary key.
        public string LastName { get; set; }
        public string FirstMidName { get; set; }
        public DateTime EnrollmentDate { get; set; }

        public virtual ICollection<Enrollment> Enrollments { get; set; }//navigation property, can hold multiple entities ==> it's type must be a list ICollection, such as lazy loading, 1 Student can hold multipe Enrollment entities
    }
}