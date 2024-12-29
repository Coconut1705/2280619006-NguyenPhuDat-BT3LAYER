using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Lab05.BUS;
using Lab05.DAL.Entities;

namespace Lab05.GUI
{
    public partial class frmStudent : Form
    {
        private readonly StudentService studentService = new StudentService();
        private readonly FacultyService facultyService = new FacultyService();
        public frmStudent()
        {
            InitializeComponent();
        }

        private void frmStudent_Load(object sender, EventArgs e)
        {
            try
            {
                setGridViewStyle(dgvStudent);
                var listFacultys = facultyService.GetAll();
                var listStudents = studentService.GetAll();
                FillFalcultyCombobox(listFacultys);
                BindGrid(listStudents);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void BindGrid(List<Student> listStudents)
        {
            dgvStudent.Rows.Clear();
            foreach (var item in listStudents)
            {
                int index = dgvStudent.Rows.Add();
                dgvStudent.Rows[index].Cells[0].Value = item.StudentID;
                dgvStudent.Rows[index].Cells[1].Value = item.FullName;

                if (item.Faculty != null)
                    dgvStudent.Rows[index].Cells[2].Value = item.Faculty.FacultyName;

                dgvStudent.Rows[index].Cells[3].Value = item.AverageScore.ToString();

                if (item.MajorID != null)
                    dgvStudent.Rows[index].Cells[4].Value = item.Major.Name;

                //LoadAvatar(item.Avatar);
            }
        }

        private void FillFalcultyCombobox(List<Faculty> listFacultys)
        {
            try
            {
                cmbFaculty.DataSource = listFacultys;
                cmbFaculty.DisplayMember = "FacultyName";
                cmbFaculty.ValueMember = "FacultyID";
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void setGridViewStyle(DataGridView dgvStudent)
        {
            dgvStudent.BorderStyle = BorderStyle.None;
            dgvStudent.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(238, 239, 249);
            dgvStudent.CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal;
            dgvStudent.DefaultCellStyle.SelectionBackColor = Color.DarkTurquoise;
            dgvStudent.DefaultCellStyle.SelectionForeColor = Color.WhiteSmoke;
            dgvStudent.BackgroundColor = Color.White;
            dgvStudent.EnableHeadersVisualStyles = false;
            dgvStudent.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.None;
            dgvStudent.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(20, 25, 72);
            dgvStudent.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
        }
        private string avatarFilePath = string.Empty;
        private string studentID;

        private void LoadAvatar(string studentID)
        {
            string folderPath = Path.Combine(Application.StartupPath, "Images");
            var student = studentService.FindStudentById(studentID) as Student;

            if (student != null && !string.IsNullOrEmpty(student.Avatar))
            {
                string avatarFilePath = Path.Combine(folderPath, student.Avatar);
                if (File.Exists(avatarFilePath))
                {
                    picAvatar.Image = Image.FromFile(avatarFilePath);
                }
                else
                {
                    picAvatar.Image = null;
                }
            }
            else
            {
                picAvatar.Image = null;
            }
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            try
            {
                // Kiểm tra dữ liệu nhập vào
                if (!ValidateInput()) return;

                // Tạo đối tượng sinh viên hoặc tìm sinh viên nếu đã có
                var student = studentService.FindStudentById(txtStudentID.Text);

                // Nếu sinh viên không tồn tại, tạo mới
                if (student == null)
                {
                    student = new Student();
                }

                // Cập nhật thông tin sinh viên từ các textbox và combobox
                student.StudentID = txtStudentID.Text;
                student.FullName = txtName.Text;

                // Kiểm tra xem điểm trung bình có hợp lệ không
                if (double.TryParse(txtAverageScore.Text, out double averageScore))
                {
                    student.AverageScore = averageScore;
                }
                else
                {
                    MessageBox.Show("Please enter a valid Average Score.", "Input Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // Kiểm tra và lưu thông tin khoa nếu có
                if (cmbFaculty.SelectedIndex != -1)
                {
                    student.FacultyID = int.Parse(cmbFaculty.SelectedValue.ToString());
                }

                // Kiểm tra và lưu ảnh đại diện nếu có
                if (!string.IsNullOrEmpty(avatarFilePath))
                {
                    string avatarFileName = SaveAvatar(avatarFilePath, txtStudentID.Text);
                    if (!string.IsNullOrEmpty(avatarFileName))
                    {
                        student.Avatar = avatarFileName;
                    }
                }

                // Gọi phương thức InsertOrUpdateStudent để thêm hoặc cập nhật sinh viên
                studentService.InsertOrUpdateStudent(student);

                // Cập nhật lại giao diện với danh sách sinh viên mới
                BindGrid(studentService.GetAll());

                // Xóa dữ liệu đã nhập và reset avatar
                ClearData();
                avatarFilePath = string.Empty;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error adding data: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ClearData()
        {
            txtStudentID.Clear();
            txtName.Clear();
            txtAverageScore.Clear();
            cmbFaculty.SelectedIndex = -1; // Đặt lại ComboBox về trạng thái chưa chọn
            picAvatar.Image = null;  // Xóa ảnh avatar
            avatarFilePath = string.Empty; // Xóa đường dẫn ảnh
        }


        private string SaveAvatar(string avatarFilePath, string text)
        {
            try
            {
                // Tạo thư mục Images nếu chưa có
                string folderPath = Path.Combine(Application.StartupPath, "Images");
                if (!Directory.Exists(folderPath))
                {
                    Directory.CreateDirectory(folderPath);
                }

                // Tạo tên file ảnh mới
                string avatarFileName = studentID + Path.GetExtension(avatarFilePath);
                string destinationPath = Path.Combine(folderPath, avatarFileName);

                // Sao chép file ảnh vào thư mục Images
                File.Copy(avatarFilePath, destinationPath, true);  // true để ghi đè nếu file đã tồn tại

                return avatarFileName;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error saving avatar: {ex.Message}");
                return string.Empty;
            }
        }

        private bool ValidateInput()
        {
            // Kiểm tra xem các trường thông tin đã được điền đầy đủ chưa
            if (string.IsNullOrEmpty(txtStudentID.Text))
            {
                MessageBox.Show("Student ID is required.", "Input Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            if (string.IsNullOrEmpty(txtName.Text))
            {
                MessageBox.Show("Full Name is required.", "Input Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            if (string.IsNullOrEmpty(txtAverageScore.Text) || !double.TryParse(txtAverageScore.Text, out _))
            {
                MessageBox.Show("Please enter a valid Average Score.", "Input Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            if (cmbFaculty.SelectedIndex == -1)
            {
                MessageBox.Show("Please select a Faculty.", "Input Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            return true; // Dữ liệu hợp lệ
        }

        private void btnUpload_Click_1(object sender, EventArgs e)
        {
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.Filter = "Image Files (*.jpg; *.jpeg; *.png)|*.jpg; *.jpeg; *.png";
                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    avatarFilePath = openFileDialog.FileName;
                    picAvatar.Image = Image.FromFile(avatarFilePath);
                }
            }
        }

        private void chkUnregisterMajor_CheckedChanged(object sender, EventArgs e)
        {
            var listStudents = new List<Student>();
            if (this.chkUnregisterMajor.Checked)
                listStudents = studentService.GetAllHasNoMajor();
            else
                listStudents = studentService.GetAll();
            BindGrid(listStudents);
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            try
            {
                // Kiểm tra xem có dòng nào được chọn trong DataGridView không
                if (dgvStudent.SelectedRows.Count == 0)
                {
                    MessageBox.Show("Please select a student to delete.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // Lấy StudentID của sinh viên được chọn
                string selectedStudentID = dgvStudent.SelectedRows[0].Cells[0].Value.ToString();

                // Tìm sinh viên trong cơ sở dữ liệu
                var student = studentService.FindStudentById(selectedStudentID) as Student;

                if (student != null)
                {
                    // Xác nhận trước khi xóa
                    var confirmResult = MessageBox.Show("Are you sure you want to delete this student?", "Confirm Delete", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                    if (confirmResult == DialogResult.Yes)
                    {
                        // Gọi phương thức xóa sinh viên
                        studentService.DeleteStudent(student);

                        // Cập nhật lại danh sách sinh viên trong DataGridView
                        BindGrid(studentService.GetAll());
                    }
                }
                else
                {
                    MessageBox.Show("Student not found.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error deleting data: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            try
            {
                // Xác nhận người dùng có muốn thoát không
                var confirmResult = MessageBox.Show("Are you sure you want to exit?",
                                                     "Confirm Exit",
                                                     MessageBoxButtons.YesNo,
                                                     MessageBoxIcon.Question);
                if (confirmResult == DialogResult.Yes)
                {
                    // Đóng form và thoát ứng dụng
                    Application.Exit();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error while exiting: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
