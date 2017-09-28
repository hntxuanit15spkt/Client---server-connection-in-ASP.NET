using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using ContosoUniversity.DAL;
using ContosoUniversity.Models;
using PagedList;
using System.Data.Entity.Infrastructure;
namespace ContosoUniversity.Controllers
{
	public class StudentController : Controller
	{
		private SchoolContext db = new SchoolContext();
		// GET: Student
		public ActionResult Index(string sortOrder, string searchString, string currentFilter, int? page)//sortOrder parameter from the query string in the URL
		{
			ViewBag.CurrentSort = sortOrder;//this must be included in the paging links in order to keep the sort order the same while paging
			ViewBag.NameSortParm = String.IsNullOrEmpty(sortOrder) ? "name_desc" : "";
			ViewBag.DateSortParm = sortOrder == "Date" ? "date_desc" : "Date";
			if (searchString != null)
			{
				page = 1;
			}
			else
			{
				searchString = currentFilter;
			}
			ViewBag.CurrentFilter = searchString;
			var students = from s in db.Students
										 select s;
			if (!String.IsNullOrEmpty(searchString))//students is IQueryable variables // the .NET Framework implementation of the Contains method returns all rows when you pass an empty string to it, but the Entity Framework provider for SQL Server Compact 4.0 returns zero rows for empty strings. Therefore the code in the example (putting the Where statement inside an if statement) makes sure that you get the same results for all versions of SQL Server
			{
				students = students.Where(s => s.LastName.Contains(searchString) || s.FirstMidName.Contains(searchString));
			}
			switch (sortOrder)
			{
				case "name_desc":
					students = students.OrderByDescending(s => s.LastName);
					break;
				case "Date":
					students = students.OrderBy(s => s.EnrollmentDate);
					break;
				case "date_desc":
					students = students.OrderByDescending(s => s.EnrollmentDate);
					break;
				default:
					students = students.OrderBy(s => s.LastName);
					break;
			}
			int pageSize = 3;
			int pageNumber = (page ?? 1);
			return View(students.ToPagedList(pageNumber, pageSize));//The query is not executed until you convert the IQueryable object (students variables) into a collection by calling a method such as ToList, the ToPagedList extension method on the students IQueryable object converts the student query to a single page of students in a collection type that supports paging
		}

		// GET: Student/Details/5
		public ActionResult Details(int? id)//id comes from route data in the Details hyperlink on the Index page
		{
			if (id == null)
			{
				return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
				//return HttpNotFound();
			}
			Student student = db.Students.Find(id);
			if (student == null)
			{
				return HttpNotFound();
			}
			return View(student);
		}

		// GET: Student/Create
		public ActionResult Create()
		{
			return View();
		}

		// POST: Student/Create
		// To protect from overposting attacks, please enable the specific properties you want to bind to, for 
		// more details see https://go.microsoft.com/fwlink/?LinkId=317598.
		//a model binder converts posted form values to CLR types and passes them to the action method in parameters
		[HttpPost]
		[ValidateAntiForgeryToken]// helps prevent cross-site request forgery attacks 
		public ActionResult Create([Bind(Include = "LastName,FirstMidName,EnrollmentDate")] Student student)//removed ID from the Bind attribute because ID is the primary key value which SQL Server will set automatically when the row is inserted. Input from the user does not set the ID value.The Bind attribute is one way to protect against over-posting in create scenarios. For example, suppose the Student entity includes a Secret property that you don't want this web page to set. Nếu không có Bind attribute thì hacker có thể dùng fiddler hoặc viết mã js để lấy giá trị của thuộc tính
		{
			try
			{
				if (ModelState.IsValid)
				{
					db.Students.Add(student);
					db.SaveChanges();
					return RedirectToAction("Index");
				}
			}
			catch (RetryLimitExceededException /*dex*/)//turn on the retry policy, the only errors likely to be transient will already have been tried and failed several times
			{
				//Log the error (uncomment dex variable name and add a line here to write a log.)
				ModelState.AddModelError("", "Unable to save changes. Try again, and if the problem persists see your system administrator.");
			}
			return View(student);
		}

		// GET: Student/Edit/5
		public ActionResult Edit(int? id)
		{
			if (id == null)
			{
				return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
			}
			Student student = db.Students.Find(id);
			if (student == null)
			{
				return HttpNotFound();
			}
			return View(student);
		}
		[HttpPost, ActionName("Edit")]
		[ValidateAntiForgeryToken]
		public ActionResult EditPost(int? id)
		{
			if (id == null)
			{
				return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
			}
			var studentToUpdate = db.Students.Find(id);
			if (TryUpdateModel(studentToUpdate, "", new string[] { "LastName", "FirstMidName", "EnrollmentDate" }))
			{
				try
				{
					db.SaveChanges();//When the SaveChanges method is called, the Modified flag causes the Entity Framework to create SQL statements to update the database row
					return RedirectToAction("Index");
				}
				catch (RetryLimitExceededException /* dex */)
				{
					//Log the error (uncomment dex variable name and add a line here to write a log.
					ModelState.AddModelError("", "Unable to save changes. Try again, and if the problem persists, see your system administrator.");
				}
			}
			return View(studentToUpdate);
		}
		// POST: Student/Edit/5
		// To protect from overposting attacks, please enable the specific properties you want to bind to, for 
		// more details see https://go.microsoft.com/fwlink/?LinkId=317598.
		//[HttpPost]
		//[ValidateAntiForgeryToken]
		//public ActionResult Edit([Bind(Include = "ID,LastName,FirstMidName,EnrollmentDate")] Student student)
		//{
		//    if (ModelState.IsValid)
		//    {
		//        db.Entry(student).State = EntityState.Modified;
		//        db.SaveChanges();
		//        return RedirectToAction("Index");
		//    }
		//    return View(student);
		//}

		// GET: Student/Delete/5
		public ActionResult Delete(int? id, bool? saveChangesError = false)
		{
			if (id == null)
			{
				return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
			}
			if (saveChangesError.GetValueOrDefault())
			{
				ViewBag.ErrorMessage = "Delete failed. Try again, and if the problem persists see your system administrator";
			}
			Student student = db.Students.Find(id);
			if (student == null)
			{
				return HttpNotFound();
			}
			return View(student);
		}
		//public ActionResult Delete(int? id)
		//{
		//    if (id == null)
		//    {
		//        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
		//    }
		//    Student student = db.Students.Find(id);
		//    if (student == null)
		//    {
		//        return HttpNotFound();
		//    }
		//    return View(student);
		//}

		// POST: Student/Delete/5
		[HttpPost, ActionName("Delete")]
		[ValidateAntiForgeryToken]
		public ActionResult DeleteConfirmed(int id)
		{
			try
			{
				//Student student = db.Students.Find(id);
				//db.Students.Remove(student);
				Student studentToDelete = new Student() { ID = id };
				db.Entry(studentToDelete).State = EntityState.Deleted;
				db.SaveChanges();
			}
			catch (RetryLimitExceededException /*dex*/)
			{
				//Log the error (uncomment dex variable name and add a line here to write a log.
				return RedirectToAction("Delete", new { id = id, saveChangesError = true });
			}
			return RedirectToAction("Index");
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				db.Dispose();
			}
			base.Dispose(disposing);
		}
	}
}
