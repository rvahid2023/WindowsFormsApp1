using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsFormsApp1
{
    public partial class EmployeeForm : Form
    {
     
        
        public EmployeeForm()
        {
            InitializeComponent();

        }

        private void EmployeeForm_Load(object sender, EventArgs e)
        {
            dgvEmployees.AutoGenerateColumns = false;// غیرفعال کردن ساخت خودکار ستون‌ها
            LoadEmployees();
        }


        private void LoadEmployees()
        {
            string connectionString = "Server=.;Database=Phase2-3;Trusted_Connection=True;";
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                string query = "SELECT EmployeeID, FirstName, LastName, Username FROM Employee";
                SqlDataAdapter adapter = new SqlDataAdapter(query, conn);
                DataTable dt = new DataTable();
                adapter.Fill(dt);

                dgvEmployees.DataSource = dt;
            }
        }
        private void btnAdd_Click(object sender, EventArgs e)
        {
            // 1. گرفتن مقادیر از تکست باکس‌ها
            string firstName = txtFirstName.Text.Trim();
            string lastName = txtLastName.Text.Trim();
            string username = txtUsername.Text.Trim();
            string password = txtPassword.Text.Trim();

            // 2. اعتبارسنجی ساده
            if (firstName == "" || lastName == "" || username == "" || password == "")
            {
                MessageBox.Show("لطفا همه فیلدها را پر کنید.", "هشدار", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // 3. رشته اتصال به دیتابیس
            string connectionString = "Server=.;Database=Phase2-3;Trusted_Connection=True;";

            // 4. کوئری درج
            string query = "INSERT INTO Employee (FirstName, LastName, Username, Password) VALUES (@FirstName, @LastName, @Username, @Password)";

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    // پارامترها
                    cmd.Parameters.AddWithValue("@FirstName", firstName);
                    cmd.Parameters.AddWithValue("@LastName", lastName);
                    cmd.Parameters.AddWithValue("@Username", username);
                    cmd.Parameters.AddWithValue("@Password", password);

                    conn.Open();
                    try
                    {
                        cmd.ExecuteNonQuery();
                        MessageBox.Show("کارمند با موفقیت اضافه شد.", "پیام", MessageBoxButtons.OK, MessageBoxIcon.Information);

                        // بارگذاری مجدد دیتا
                        LoadEmployees();

                        // پاک کردن تکست باکس‌ها
                        txtFirstName.Clear();
                        txtLastName.Clear();
                        txtUsername.Clear();
                        txtPassword.Clear();
                    }
                    catch (SqlException ex)
                    {
                        MessageBox.Show("خطا در اضافه کردن کارمند: " + ex.Message, "خطا", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        private void dgvEmployees_SelectionChanged(object sender, EventArgs e)
        {
            if (dgvEmployees.CurrentRow != null)
            {
                txtFirstName.Text = dgvEmployees.CurrentRow.Cells["FirstName"].Value.ToString();
                txtLastName.Text = dgvEmployees.CurrentRow.Cells["LastName"].Value.ToString();
                txtUsername.Text = dgvEmployees.CurrentRow.Cells["Username"].Value.ToString();
                // توجه:  پسورد رو نمایش داده نشده.
                txtPassword.Text = "";
            }
        }

        private void btnEdit_Click(object sender, EventArgs e)
        {
            if (dgvEmployees.CurrentRow == null) return;

            int employeeId = Convert.ToInt32(dgvEmployees.CurrentRow.Cells["EmployeeID"].Value);
            string firstName = txtFirstName.Text.Trim();
            string lastName = txtLastName.Text.Trim();
            string username = txtUsername.Text.Trim();
            string password = txtPassword.Text.Trim();

            if (firstName == "" || lastName == "" || username == "")
            {
                MessageBox.Show("نام، نام خانوادگی و نام کاربری نمی‌توانند خالی باشند.", "هشدار", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            string connectionString = "Server=.;Database=Phase2-3;Trusted_Connection=True;";
            string query;
            if (string.IsNullOrEmpty(password))
            {
                // اگر پسورد تغییر نکرده، آپدیت نکن
                query = "UPDATE Employee SET FirstName=@FirstName, LastName=@LastName, Username=@Username WHERE EmployeeID=@EmployeeID";
            }
            else
            {
                query = "UPDATE Employee SET FirstName=@FirstName, LastName=@LastName, Username=@Username, Password=@Password WHERE EmployeeID=@EmployeeID";
            }

            using (SqlConnection conn = new SqlConnection(connectionString))
            using (SqlCommand cmd = new SqlCommand(query, conn))
            {
                cmd.Parameters.AddWithValue("@FirstName", firstName);
                cmd.Parameters.AddWithValue("@LastName", lastName);
                cmd.Parameters.AddWithValue("@Username", username);
                cmd.Parameters.AddWithValue("@EmployeeID", employeeId);

                if (!string.IsNullOrEmpty(password))
                    cmd.Parameters.AddWithValue("@Password", password);

                conn.Open();

                try
                {
                    cmd.ExecuteNonQuery();
                    MessageBox.Show("اطلاعات کارمند با موفقیت به‌روزرسانی شد.", "پیام", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    LoadEmployees();
                }
                catch (SqlException ex)
                {
                    MessageBox.Show("خطا در به‌روزرسانی: " + ex.Message, "خطا", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (dgvEmployees.CurrentRow == null) return;

            int employeeId = Convert.ToInt32(dgvEmployees.CurrentRow.Cells["EmployeeID"].Value);

            var result = MessageBox.Show("آیا مطمئن هستید که می‌خواهید این کارمند را حذف کنید؟", "تأیید حذف", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

            if (result == DialogResult.Yes)
            {
                string connectionString = "Server=.;Database=Phase2-3;Trusted_Connection=True;";
                string query = "DELETE FROM Employee WHERE EmployeeID=@EmployeeID";

                using (SqlConnection conn = new SqlConnection(connectionString))
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@EmployeeID", employeeId);
                    conn.Open();

                    try
                    {
                        cmd.ExecuteNonQuery();
                        MessageBox.Show("کارمند حذف شد.", "پیام", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        LoadEmployees();

                        // پاک کردن تکست‌باکس‌ها
                        txtFirstName.Clear();
                        txtLastName.Clear();
                        txtUsername.Clear();
                        txtPassword.Clear();
                    }
                    catch (SqlException ex)
                    {
                        MessageBox.Show("خطا در حذف: " + ex.Message, "خطا", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }
    }
}
