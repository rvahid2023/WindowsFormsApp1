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
    public partial class LoginForm : Form
    {
        public LoginForm()
        {
            InitializeComponent();
        }

        private void btnLogin_Click(object sender, EventArgs e)
        {
            string username = txtUsername.Text.Trim();
            string password = txtPassword.Text.Trim();

            if (username == "" || password == "")
            {
                MessageBox.Show("لطفاً نام کاربری و رمز عبور را وارد کنید.", "خطا", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string connectionString = "Server=.;Database=Phase2-3;Trusted_Connection=True;"; // رشته اتصال مناسب پروژه خودت رو وارد کن
            string query = "SELECT COUNT(*) FROM Employee WHERE Username = @username AND Password = @password";

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@username", username);
                cmd.Parameters.AddWithValue("@password", password);

                try
                {
                    conn.Open();
                    int count = (int)cmd.ExecuteScalar();

                    if (count == 1)
                    {
                        MessageBox.Show("ورود موفقیت‌آمیز بود.", "خوش آمدید", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        this.Hide();
                        Main mainForm = new Main(); // یا هر فرم اصلی 
                        mainForm.Show();
                    }
                    else
                    {
                        MessageBox.Show("نام کاربری یا رمز عبور اشتباه است.", "خطا", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("خطا در اتصال به دیتابیس:\n" + ex.Message, "خطا", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }


        //**********************************************************************تست اتصال دیتابیس

        private void btnTestConnection_Click(object sender, EventArgs e)
        {
            string connectionString = "Server=.;Database=Phase2-3;Trusted_Connection=True;";

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                try
                {
                    conn.Open();
                    MessageBox.Show("✅ اتصال به دیتابیس موفق بود.");

                    txtUsername.Visible = true;
                    txtPassword.Visible = true;
                    btnLogin.Visible = true;
                    label1.Visible = true;
                    label2.Visible = true;
                    //btnTestConnection.Visible = false;

                }
                catch (Exception ex)
                {
                    MessageBox.Show("❌ خطا:\n" + ex.Message);
                }
            }

        }

        private void LoginForm_Load(object sender, EventArgs e)
        {

        }
        //*****************************************************************************************
    }
}

