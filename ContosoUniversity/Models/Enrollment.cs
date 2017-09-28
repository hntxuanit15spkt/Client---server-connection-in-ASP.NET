using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ContosoUniversity.Models
{
    public enum Grade
    {
        A, B, C, D, F
    }
    public class Enrollment
    {
        public int EnrollmentID { get; set; }
        public int StudentID { get; set; }//mặc nhiên là foreign key cho Student (navigation property) vì ở dưới có public virtual Student Student
        public int CourseID { get; set; }//foreign key
        public Grade? Grade { get; set; }//nullable (null isn't 0, null when isn't known or hasn't been assigned yet)

        public virtual Course Course { get; set; }//navigation property, An Enrollment entity is associated with one Course entity
        public virtual Student Student { get; set; }//An Enrollment entity is associated with one Student entity (can hold a single Student entity), StudentID for the Student navigation property since the Student entity's primary key is ID (Student có khóa chính là ID thì EF 6 tự hiểu là foreign key StudentID là của Student navigation property.
    } 
}