using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Lab05.DAL.Entities;
using System.Data.Entity.Migrations;


namespace Lab05.BUS
{
    public class StudentService
    {
        public List<Student> GetAll()
        {
            StudentModel context = new StudentModel();
            return context.Student.ToList();
        }

        public List<Student> GetAllHasNoMajor()
        {
            StudentModel context = new StudentModel();
            return context.Student.Where(p => p.MajorID == null).ToList();
        }

        public List<Student> GetAllHasNoMajor(int facultyID)
        {
            StudentModel context = new StudentModel();
            return context.Student.Where(p => p.MajorID == null && p.FacultyID == facultyID).ToList();
        }

        public Student FindById(string studentId)
        {
            StudentModel context = new StudentModel();
            return context.Student.FirstOrDefault(p => p.StudentID == studentId);
        }

        public void InsertUpdate(Student s)
        {
            StudentModel context = new StudentModel();
            context.Student.AddOrUpdate(s);
            context.SaveChanges();
        }

        public void InsertOrUpdateStudent(Student student)
        {
            if (student == null) throw new ArgumentNullException(nameof(student));

            // Tìm sinh viên hiện tại trong cơ sở dữ liệu
            var existingStudent = FindStudentById(student.StudentID); // Không còn lỗi CS1061 vì đã sử dụng kiểu Student
            if (existingStudent != null)
            {
                // Cập nhật thông tin sinh viên
                existingStudent.FullName = student.FullName;
                existingStudent.AverageScore = student.AverageScore;
                existingStudent.FacultyID = student.FacultyID;
                existingStudent.Avatar = student.Avatar;
            }
            else
            {
                // Thêm sinh viên mới vào cơ sở dữ liệu
                using (var context = new StudentModel())
                {
                    context.Student.Add(student);
                    context.SaveChanges();
                }
            }
        }

        private object FindStudentById(object studentID)
        {
            throw new NotImplementedException();
        }

        public Student FindStudentById(string text)
        {
            throw new NotImplementedException();
        }



        public void DeleteStudent(Student student)
        {
            try
            {
                using (var context = new StudentModel())
                {
                    // Tìm sinh viên theo StudentID
                    var studentToDelete = context.Student.FirstOrDefault(s => s.StudentID == student.StudentID);

                    if (studentToDelete != null)
                    {
                        // Xóa sinh viên
                        context.Student.Remove(studentToDelete);
                        context.SaveChanges();
                    }
                    else
                    {
                        throw new Exception("Student not found.");
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error deleting student: {ex.Message}");
            }
        }

    }
}
