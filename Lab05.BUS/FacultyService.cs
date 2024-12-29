using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Lab05.DAL.Entities;

namespace Lab05.BUS
{
    public class FacultyService
    {
            public List<Faculty> GetAll()
            {
                StudentModel context = new StudentModel();
                return context.Faculty.ToList();
            }
     }
}
