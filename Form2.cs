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
    public partial class PaymentForm : Form
    {
        public PaymentForm()
        {
            InitializeComponent();
        }



        private void ClearPaymentForm()
        {
            cmbAppointment.SelectedIndex = -1;
            txtAmount.Clear();
            cmbMethod.SelectedIndex = -1;
            dtpPaymentDate.Value = DateTime.Today;
            txtAppointmentDate.Clear();
        }








        private string connectionString = "Server=.;Database=Phase2-3;Trusted_Connection=True;";

        private void PaymentForm_Load(object sender, EventArgs e)
        {
            LoadAppointmentsForPayment();  // بارگذاری نوبت‌ها برای نمایش در کمبوباکس
            LoadPaymentMethods();          // بارگذاری روش‌های پرداخت در کمبوباکس روش پرداخت
            LoadPayments();                // بارگذاری لیست پرداخت‌ها در دیتاگرایدویو
        }

        // این متد نوبت‌ها رو از جدول Appointment می‌خونه و داخل cmbAppointment می‌ریزه
        private void LoadAppointmentsForPayment()
        {
            // کوئری برای گرفتن شناسه نوبت و تاریخ نوبت به صورت رشته نمایش برای کاربر
            string query = @"
        SELECT 
            AppointmentID, 
            CONCAT('ID: ', AppointmentID, ' - Date: ', CONVERT(varchar, Date, 23)) AS DisplayText 
        FROM Appointment";

            using (SqlConnection con = new SqlConnection(connectionString))
            using (SqlDataAdapter da = new SqlDataAdapter(query, con))
            {
                DataTable dt = new DataTable();
                da.Fill(dt);  // پر کردن DataTable با نتایج کوئری

                // تنظیمات نمایش کمبوباکس:
                cmbAppointment.DataSource = dt;         // منبع داده‌ها
                cmbAppointment.ValueMember = "AppointmentID";   // مقداری که پشت صحنه استفاده میشه
                cmbAppointment.DisplayMember = "DisplayText";   // متنی که کاربر میبینه
                cmbAppointment.SelectedIndex = -1;  // بدون انتخاب اولیه
            }
        }

        // این متد گزینه‌های روش پرداخت رو به cmbMethod اضافه میکنه
        private void LoadPaymentMethods()
        {
            cmbMethod.Items.Clear();     // ابتدا خالی میکنیم
            cmbMethod.Items.Add("Cash"); // اضافه کردن گزینه‌ها
            cmbMethod.Items.Add("Card");
            cmbMethod.Items.Add("Online");
            cmbMethod.SelectedIndex = -1;  // بدون انتخاب اولیه
        }

        // این متد پرداخت‌ها رو از جدول Payment میخونه و داخل dgvPayments نمایش میده
        private void LoadPayments()
        {
            string query = "SELECT PaymentID, AppointmentID, Amount, Method, PaymentDate FROM Payment";

            using (SqlConnection con = new SqlConnection(connectionString))
            using (SqlDataAdapter da = new SqlDataAdapter(query, con))
            {
                DataTable dt = new DataTable();
                da.Fill(dt);  // گرفتن داده‌ها
                dgvPayments.DataSource = dt;  // نمایش در DataGridView
            }
        }

        private void btnAddPayment_Click(object sender, EventArgs e)
        {
            if (cmbAppointment.SelectedIndex == -1)
            {
                MessageBox.Show("لطفا یک نوبت را انتخاب کنید.");
                return;
            }
            if (string.IsNullOrWhiteSpace(txtAmount.Text) || !int.TryParse(txtAmount.Text, out int amount) || amount < 0)
            {
                MessageBox.Show("لطفا مبلغ معتبر وارد کنید.");
                return;
            }
            if (cmbMethod.SelectedIndex == -1)
            {
                MessageBox.Show("لطفا روش پرداخت را انتخاب کنید.");
                return;
            }

            int appointmentId = (int)cmbAppointment.SelectedValue;
            string method = cmbMethod.SelectedItem.ToString();
            DateTime paymentDate = dtpPaymentDate.Value.Date;

            string query = "INSERT INTO Payment (AppointmentID, Amount, Method, PaymentDate) VALUES (@AppointmentID, @Amount, @Method, @PaymentDate)";

            using (SqlConnection con = new SqlConnection(connectionString))
            using (SqlCommand cmd = new SqlCommand(query, con))
            {
                cmd.Parameters.AddWithValue("@AppointmentID", appointmentId);
                cmd.Parameters.AddWithValue("@Amount", amount);
                cmd.Parameters.AddWithValue("@Method", method);
                cmd.Parameters.AddWithValue("@PaymentDate", paymentDate);

                try
                {
                    con.Open();
                    cmd.ExecuteNonQuery();
                    MessageBox.Show("پرداخت با موفقیت ثبت شد.");
                    LoadPayments();       // بارگذاری مجدد داده‌ها در DataGridView
                    ClearPaymentForm();   // پاکسازی فیلدهای فرم (تابع رو بعدا می‌نویسیم)
                }
                catch (Exception ex)
                {
                    MessageBox.Show("خطا در ثبت پرداخت: " + ex.Message);
                }
            }
        }

        private void dgvPayments_SelectionChanged(object sender, EventArgs e)
        {
            if (dgvPayments.CurrentRow == null)
            {
                ClearPaymentForm();
                return;
            }

            DataGridViewRow row = dgvPayments.CurrentRow;

            // گرفتن مقدار ستون‌ها
            int appointmentId = Convert.ToInt32(row.Cells["AppointmentID"].Value);
            int amount = Convert.ToInt32(row.Cells["Amount"].Value);
            string method = row.Cells["Method"].Value.ToString();
            DateTime paymentDate = Convert.ToDateTime(row.Cells["PaymentDate"].Value);

            // پر کردن فرم
            cmbAppointment.SelectedValue = appointmentId;
            txtAmount.Text = amount.ToString();
            cmbMethod.SelectedItem = method;
            dtpPaymentDate.Value = paymentDate;
        }

        private void btnUpdatePayment_Click(object sender, EventArgs e)
        {
            if (dgvPayments.CurrentRow == null)
            {
                MessageBox.Show("لطفا یک پرداخت را از جدول انتخاب کنید.");
                return;
            }
            if (cmbAppointment.SelectedIndex == -1)
            {
                MessageBox.Show("لطفا یک نوبت را انتخاب کنید.");
                return;
            }
            if (string.IsNullOrWhiteSpace(txtAmount.Text) || !int.TryParse(txtAmount.Text, out int amount) || amount < 0)
            {
                MessageBox.Show("لطفا مبلغ معتبر وارد کنید.");
                return;
            }
            if (cmbMethod.SelectedIndex == -1)
            {
                MessageBox.Show("لطفا روش پرداخت را انتخاب کنید.");
                return;
            }

            int paymentId = Convert.ToInt32(dgvPayments.CurrentRow.Cells["PaymentID"].Value);
            int appointmentId = (int)cmbAppointment.SelectedValue;
            string method = cmbMethod.SelectedItem.ToString();
            DateTime paymentDate = dtpPaymentDate.Value.Date;

            string query = @"UPDATE Payment 
                     SET AppointmentID = @AppointmentID, Amount = @Amount, Method = @Method, PaymentDate = @PaymentDate 
                     WHERE PaymentID = @PaymentID";

            using (SqlConnection con = new SqlConnection(connectionString))
            using (SqlCommand cmd = new SqlCommand(query, con))
            {
                cmd.Parameters.AddWithValue("@PaymentID", paymentId);
                cmd.Parameters.AddWithValue("@AppointmentID", appointmentId);
                cmd.Parameters.AddWithValue("@Amount", amount);
                cmd.Parameters.AddWithValue("@Method", method);
                cmd.Parameters.AddWithValue("@PaymentDate", paymentDate);

                try
                {
                    con.Open();
                    cmd.ExecuteNonQuery();
                    MessageBox.Show("پرداخت با موفقیت به‌روزرسانی شد.");
                    LoadPayments();
                    ClearPaymentForm();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("خطا در به‌روزرسانی پرداخت: " + ex.Message);
                }
            }
        }

        private void btnDeletePayment_Click(object sender, EventArgs e)
        {
            if (dgvPayments.CurrentRow == null)
            {
                MessageBox.Show("لطفا یک پرداخت را از جدول انتخاب کنید.");
                return;
            }

            int paymentId = Convert.ToInt32(dgvPayments.CurrentRow.Cells["PaymentID"].Value);

            if (MessageBox.Show("آیا مطمئن هستید که می‌خواهید این پرداخت را حذف کنید؟", "تأیید حذف", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                string query = "DELETE FROM Payment WHERE PaymentID = @PaymentID";

                using (SqlConnection con = new SqlConnection(connectionString))
                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@PaymentID", paymentId);

                    try
                    {
                        con.Open();
                        cmd.ExecuteNonQuery();
                        MessageBox.Show("پرداخت حذف شد.");
                        LoadPayments();
                        ClearPaymentForm();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("خطا در حذف پرداخت: " + ex.Message);
                    }
                }
            }
        }

        private void btnClearPayment_Click(object sender, EventArgs e)
        {
            ClearPaymentForm();
        }
    }
}
