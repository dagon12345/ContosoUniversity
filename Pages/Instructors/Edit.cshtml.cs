using ContosoUniversity.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ContosoUniversity.Pages.Instructors
{
    public class EditModel : InstructorCoursesPageModel
    {
        private readonly ContosoUniversity.Data.SchoolContext _context;

        public EditModel(ContosoUniversity.Data.SchoolContext context)
        {
            _context = context;
        }

        [BindProperty]
        public Instructor Instructor { get; set; }
/*Gets the current Instructor entity from the database using eager loading
 for the OfficeAssignment and Courses navigation properties.*/
        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Instructor = await _context.Instructors
                .Include(i => i.OfficeAssignment)
                .Include(i => i.Courses)
                .AsNoTracking()
                .FirstOrDefaultAsync(m => m.ID == id);

            if (Instructor == null)
            {
                return NotFound();
            }
                        /*Calls PopulateAssignedCourseData in OnGetAsync to provide information for the checkboxes 
            using the AssignedCourseData view model class.*/
            PopulateAssignedCourseData(_context, Instructor);
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int? id, string[] selectedCourses)
        {
            if (id == null)
            {
                return NotFound();
            }

            var instructorToUpdate = await _context.Instructors
                .Include(i => i.OfficeAssignment)
                .Include(i => i.Courses)
                .FirstOrDefaultAsync(s => s.ID == id);

            if (instructorToUpdate == null)
            {
                return NotFound();
            }
/*Updates the retrieved Instructor entity with values from the model binder. 
TryUpdateModelAsync prevents overposting.*/
            if (await TryUpdateModelAsync<Instructor>(
                instructorToUpdate,
                "Instructor",
                i => i.FirstMidName, i => i.LastName,
                i => i.HireDate, i => i.OfficeAssignment))
            {
                /*If the office location is blank, sets Instructor.OfficeAssignment to null. 
                When Instructor.OfficeAssignment is null, the related row in the OfficeAssignment table is deleted.*/
                if (String.IsNullOrWhiteSpace(
                    instructorToUpdate.OfficeAssignment?.Location))
                {
                    instructorToUpdate.OfficeAssignment = null;
                }
                UpdateInstructorCourses(selectedCourses, instructorToUpdate);
                await _context.SaveChangesAsync();
                return RedirectToPage("./Index");
            }
            /*Calls PopulateAssignedCourseData and UpdateInstructorCourses in OnPostAsync if TryUpdateModelAsync fails.
             These method calls restore the assigned course data entered on the page when it is redisplayed with an error message.*/
            UpdateInstructorCourses(selectedCourses, instructorToUpdate);

            PopulateAssignedCourseData(_context, instructorToUpdate);
            return Page();
        }

        /*Calls UpdateInstructorCourses in OnPostAsync to apply information
         from the checkboxes to the Instructor entity being edited.*/
        public void UpdateInstructorCourses(string[] selectedCourses,Instructor instructorToUpdate)
        {
            /*If no checkboxes were selected, the code in UpdateInstructorCourses initializes the instructorToUpdate.Courses 
            with an empty collection and returns:*/
            if (selectedCourses == null)
            {
                instructorToUpdate.Courses = new List<Course>();
                return;
            }

            var selectedCoursesHS = new HashSet<string>(selectedCourses);
            var instructorCourses = new HashSet<int>(instructorToUpdate.Courses.Select(c => c.CourseID));
            foreach (var course in _context.Courses)
            {
                /*If the checkbox for a course is selected but the course is not in the Instructor.Courses navigation property,
                 the course is added to the collection in the navigation property.*/
                if (selectedCoursesHS.Contains(course.CourseID.ToString()))
                {
                    if (!instructorCourses.Contains(course.CourseID))
                    {
                        instructorToUpdate.Courses.Add(course);
                    }
                }
                else
                {
                    /*If the checkbox for a course is not selected, but the course is in the Instructor.Courses navigation property, 
                    the course is removed from the navigation property.*/
                    if (instructorCourses.Contains(course.CourseID))
                    {
                        var courseToRemove = instructorToUpdate.Courses.Single(c => c.CourseID == course.CourseID);
                        instructorToUpdate.Courses.Remove(courseToRemove);
                    }
                }
            }
        }
    }
}