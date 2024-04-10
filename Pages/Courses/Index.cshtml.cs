using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using ContosoUniversity.Data;
using ContosoUniversity.Models;

namespace ContosoUniversity.Pages.Courses
{
    public class IndexModel : PageModel
    {
        private readonly ContosoUniversity.Data.SchoolContext _context;

        public IndexModel(ContosoUniversity.Data.SchoolContext context)
        {
            _context = context;
        }

        public IList<Course> Courses { get;set; }

        public async Task OnGetAsync()
        {
            /*
            Course = await _context.Courses
                .Include(c => c.Department).ToListAsync();
                */


                /*NOTE:
                No-tracking queries are useful when the results are used in a read-only scenario. 
                They're generally quicker to execute because there's no need to set up the change tracking information.
                If the entities retrieved from the database don't need to be updated, 
                then a no-tracking query is likely to perform better than a tracking query.
                
                AsNoTracking is called because the entities aren't updated in the current context.*/

                Courses = await _context.Courses
                .Include(c => c.Department)
                .AsNoTracking()
                .ToListAsync();


        }
    }
}
