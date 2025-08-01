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
    public partial class Main : Form
    {


        private string connectionString = "Server=.;Database=Phase2-3;Trusted_Connection=True;";

        public Main()
        {
            InitializeComponent();
            LoadPatients();
            LoadDoctors();
            LoadMedicalRecords();
        }

        private void ClearDoctorInputs()
        {
            txtDoctorFirstName.Clear();
            txtDoctorLastName.Clear();
            txtSpecialty.Clear();
            txtDoctorPhone.Clear();
        }


        private void ClearPatientInputs()
        {
            txtFirstName.Clear();
            txtLastName.Clear();
            txtNationalID.Clear();
            numAge.Value = numAge.Minimum; // ریست کردن عدد سن
            cmbGender.SelectedIndex = -1;
            txtPhone.Clear();
        }

        private void ClearMedicalRecordInputs()
        {
            txtRecordID.Clear();
            cmbAppointment.SelectedIndex = -1;
            dtpRecordDate.Value = DateTime.Today;
            txtDiagnosis.Clear();
            txtMedications.Clear();
        }


        private void FormMedicalRecord_Load(object sender, EventArgs e)
        {
            LoadMedicalRecords();
        }



        //--------------------------------------------------------------------------فراخوانی اطلاعات بیماران

        private void LoadPatients()
        {
            string connectionString = "Server=.;Database=Phase2-3;Trusted_Connection=True;";
            string query = "SELECT PatientID, FirstName, LastName, NationalID, Age, Gender, Phone FROM Patient";

            using (SqlConnection conn = new SqlConnection(connectionString))
            using (SqlCommand cmd = new SqlCommand(query, conn))
            using (SqlDataAdapter adapter = new SqlDataAdapter(cmd))
            {
                try
                {
                    DataTable dt = new DataTable();
                    adapter.Fill(dt);

                    dgvPatients.DataSource = dt;
                }
                catch (Exception ex)
                {
                    MessageBox.Show("خطا در بارگذاری داده‌ها:\n" + ex.Message);
                }
            }
        }

        //=============================================================================فراخوانی اطلاعات پزشکان
        private void LoadDoctors()
        {
            string connectionString = "Server=.;Database=Phase2-3;Trusted_Connection=True;";
            string query = "SELECT DoctorID, FirstName, LastName, Specialty, Phone FROM Doctor";

            using (SqlConnection conn = new SqlConnection(connectionString))
            using (SqlCommand cmd = new SqlCommand(query, conn))
            using (SqlDataAdapter adapter = new SqlDataAdapter(cmd))
            {
                try
                {
                    DataTable dt = new DataTable();
                    adapter.Fill(dt);

                    dgvDoctors.DataSource = dt;
                }
                catch (Exception ex)
                {
                    MessageBox.Show("خطا در بارگذاری داده‌ها:\n" + ex.Message);
                }
            }
        }





        //+++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++فراخوانی نوبتها و اطلاعات بیماران و پزشکان


        private void LoadAppointments()
        {
            string query = @"
        SELECT a.AppointmentID, 
               p.FirstName + ' ' + p.LastName AS PatientName,
               d.FirstName + ' ' + d.LastName AS DoctorName,
               a.Date, a.Time, a.Status
        FROM Appointment a
        INNER JOIN Patient p ON a.PatientID = p.PatientID
        INNER JOIN Doctor d ON a.DoctorID = d.DoctorID";

            using (SqlConnection conn = new SqlConnection(connectionString))
            using (SqlCommand cmd = new SqlCommand(query, conn))
            using (SqlDataAdapter adapter = new SqlDataAdapter(cmd))
            {
                DataTable dt = new DataTable();
                adapter.Fill(dt);
                dgvAppointments.DataSource = dt;
            }
        }


        private int GetPatientIDByName(string fullName)
        {
            string query = "SELECT PatientID FROM Patient WHERE FirstName + ' ' + LastName = @FullName";
            using (SqlConnection conn = new SqlConnection(connectionString))
            using (SqlCommand cmd = new SqlCommand(query, conn))
            {
                cmd.Parameters.AddWithValue("@FullName", fullName);
                conn.Open();
                var result = cmd.ExecuteScalar();
                return result != null ? Convert.ToInt32(result) : -1;
            }
        }

        private int GetDoctorIDByName(string fullName)
        {
            string query = "SELECT DoctorID FROM Doctor WHERE FirstName + ' ' + LastName = @FullName";
            using (SqlConnection conn = new SqlConnection(connectionString))
            using (SqlCommand cmd = new SqlCommand(query, conn))
            {
                cmd.Parameters.AddWithValue("@FullName", fullName);
                conn.Open();
                var result = cmd.ExecuteScalar();
                return result != null ? Convert.ToInt32(result) : -1;
            }
        }



        private void ClearAppointmentInputs()
        {
            cmbPatients.SelectedIndex = -1;
            cmbDoctors.SelectedIndex = -1;
            dtpDate.Value = DateTime.Today;
            dtpTime.Value = DateTime.Now;
            cmbStatus.SelectedIndex = 0;
        }



        private void LoadPatientsForCombo()
        {
            string query = "SELECT PatientID, FirstName + ' ' + LastName AS FullName FROM Patient";
            using (SqlConnection conn = new SqlConnection(connectionString))
            using (SqlCommand cmd = new SqlCommand(query, conn))
            using (SqlDataAdapter adapter = new SqlDataAdapter(cmd))
            {
                DataTable dt = new DataTable();
                adapter.Fill(dt);
                cmbPatients.DataSource = dt;
                cmbPatients.DisplayMember = "FullName";
                cmbPatients.ValueMember = "PatientID";
                cmbPatients.SelectedIndex = -1;
            }
        }

        private void LoadDoctorsForCombo()
        {
            string query = "SELECT DoctorID, FirstName + ' ' + LastName AS FullName FROM Doctor";
            using (SqlConnection conn = new SqlConnection(connectionString))
            using (SqlCommand cmd = new SqlCommand(query, conn))
            using (SqlDataAdapter adapter = new SqlDataAdapter(cmd))
            {
                DataTable dt = new DataTable();
                adapter.Fill(dt);
                cmbDoctors.DataSource = dt;
                cmbDoctors.DisplayMember = "FullName";
                cmbDoctors.ValueMember = "DoctorID";
                cmbDoctors.SelectedIndex = -1;
            }
        }

        //+++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

        

        //**********************************************************************تست اتصال دیتابیس

        private void button1_Click(object sender, EventArgs e)
        {
            string connectionString = "Server=.;Database=Phase2-3;Trusted_Connection=True;";

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                try
                {
                    conn.Open();
                    MessageBox.Show("✅ اتصال به دیتابیس موفق بود.");
                }
                catch (Exception ex)
                {
                    MessageBox.Show("❌ خطا:\n" + ex.Message);
                }
            }

        }
        //*****************************************************************************************




        private void Form1_Load(object sender, EventArgs e)
        {           
            LoadAppointments();

            cmbStatus.Items.Clear();
            cmbStatus.Items.AddRange(new string[] { "Planned", "Finished", "Cancel" });
            cmbStatus.SelectedIndex = 0;

            dtpDate.Value = DateTime.Today;
            dtpTime.Value = DateTime.Now;
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        //-------------------------------------------------------------------ثبت بیمار جدید



        private void btnAddPatient_Click(object sender, EventArgs e)
        {
            string firstName = txtFirstName.Text.Trim();
            string lastName = txtLastName.Text.Trim();
            string nationalID = txtNationalID.Text.Trim();
            int age = (int)numAge.Value;
            string gender = cmbGender.SelectedItem?.ToString();
            string phone = txtPhone.Text.Trim();

            if (string.IsNullOrEmpty(firstName) || string.IsNullOrEmpty(lastName) ||
                string.IsNullOrEmpty(nationalID) || string.IsNullOrEmpty(gender) || string.IsNullOrEmpty(phone))
            {
                MessageBox.Show("لطفاً همه فیلدها را پر کنید.");
                return;
            }

            string connectionString = "Server=.;Database=Phase2-3;Trusted_Connection=True;";
            string query = @"INSERT INTO Patient (PatientID, FirstName, LastName, NationalID, Age, Gender, Phone) 
                     VALUES (@PatientID, @FirstName, @LastName, @NationalID, @Age, @Gender, @Phone)";

            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    int newPatientID = 1;
                    using (SqlCommand cmdMax = new SqlCommand("SELECT ISNULL(MAX(PatientID), 0) FROM Patient", conn))
                    {
                        newPatientID = (int)cmdMax.ExecuteScalar() + 1;
                    }

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@PatientID", newPatientID);
                        cmd.Parameters.AddWithValue("@FirstName", firstName);
                        cmd.Parameters.AddWithValue("@LastName", lastName);
                        cmd.Parameters.AddWithValue("@NationalID", nationalID);
                        cmd.Parameters.AddWithValue("@Age", age);
                        cmd.Parameters.AddWithValue("@Gender", gender);
                        cmd.Parameters.AddWithValue("@Phone", phone);

                        int rows = cmd.ExecuteNonQuery();

                        if (rows > 0)
                        {
                            MessageBox.Show("بیمار با موفقیت ثبت شد.");
                            LoadPatients();
                        }
                        else
                        {
                            MessageBox.Show("ثبت بیمار موفق نبود.");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("خطا در ثبت بیمار:\n" + ex.Message);
            }
        }

        private void dgvPatients_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }








        //-------------------------------------------------------------------فراخوانی مشخصات بیمار در جدول
        private void dgvPatients_SelectionChanged(object sender, EventArgs e)
        {
            if (dgvPatients.CurrentRow != null)
            {
                DataGridViewRow row = dgvPatients.CurrentRow;

                txtFirstName.Text = row.Cells["FirstName"].Value?.ToString();
                txtLastName.Text = row.Cells["LastName"].Value?.ToString();
                txtNationalID.Text = row.Cells["NationalID"].Value?.ToString();
                numAge.Value = Convert.ToDecimal(row.Cells["Age"].Value);
                cmbGender.SelectedItem = row.Cells["Gender"].Value?.ToString();
                txtPhone.Text = row.Cells["Phone"].Value?.ToString();
            }
        }






        //-------------------------------------------------------------------ذخیره تغییرات بیمار 

        private void btnUpdatePatient_Click(object sender, EventArgs e)
        {
            if (dgvPatients.CurrentRow == null)
            {
                MessageBox.Show("یک بیمار را از جدول انتخاب کنید.");
                return;
            }

            int patientID = Convert.ToInt32(dgvPatients.CurrentRow.Cells["PatientID"].Value);
            string firstName = txtFirstName.Text.Trim();
            string lastName = txtLastName.Text.Trim();
            string nationalID = txtNationalID.Text.Trim();
            int age = (int)numAge.Value;
            string gender = cmbGender.SelectedItem?.ToString();
            string phone = txtPhone.Text.Trim();

            if (string.IsNullOrEmpty(firstName) || string.IsNullOrEmpty(lastName) ||
                string.IsNullOrEmpty(nationalID) || string.IsNullOrEmpty(gender) || string.IsNullOrEmpty(phone))
            {
                MessageBox.Show("لطفاً همه فیلدها را پر کنید.");
                return;
            }

            string connectionString = "Server=.;Database=Phase2-3;Trusted_Connection=True;";
            string query = @"UPDATE Patient 
                     SET FirstName = @FirstName, LastName = @LastName, NationalID = @NationalID, 
                         Age = @Age, Gender = @Gender, Phone = @Phone
                     WHERE PatientID = @PatientID";

            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    conn.Open();

                    cmd.Parameters.AddWithValue("@FirstName", firstName);
                    cmd.Parameters.AddWithValue("@LastName", lastName);
                    cmd.Parameters.AddWithValue("@NationalID", nationalID);
                    cmd.Parameters.AddWithValue("@Age", age);
                    cmd.Parameters.AddWithValue("@Gender", gender);
                    cmd.Parameters.AddWithValue("@Phone", phone);
                    cmd.Parameters.AddWithValue("@PatientID", patientID);

                    int rows = cmd.ExecuteNonQuery();

                    if (rows > 0)
                    {
                        MessageBox.Show("اطلاعات بیمار با موفقیت بروزرسانی شد.");
                        LoadPatients();
                    }
                    else
                    {
                        MessageBox.Show("بروزرسانی موفق نبود.");
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("خطا در بروزرسانی:\n" + ex.Message);
            }
        }





        //-------------------------------------------------------------------حذف بیمار

        private void btnDeletePatient_Click(object sender, EventArgs e)
        {
            if (dgvPatients.CurrentRow == null)
            {
                MessageBox.Show("لطفاً یک بیمار را از جدول انتخاب کنید.");
                return;
            }

            int patientID = Convert.ToInt32(dgvPatients.CurrentRow.Cells["PatientID"].Value);

            var confirmResult = MessageBox.Show("آیا مطمئن هستید که می‌خواهید این بیمار را حذف کنید؟",
                                                "تأیید حذف",
                                                MessageBoxButtons.YesNo);
            if (confirmResult == DialogResult.No)
                return;

            string connectionString = "Server=.;Database=Phase2-3;Trusted_Connection=True;";
            string query = "DELETE FROM Patient WHERE PatientID = @PatientID";

            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    conn.Open();
                    cmd.Parameters.AddWithValue("@PatientID", patientID);

                    int rows = cmd.ExecuteNonQuery();

                    if (rows > 0)
                    {
                        MessageBox.Show("بیمار با موفقیت حذف شد.");
                        LoadPatients();            // بروز رسانی جدول
                        ClearPatientInputs();      // پاک‌سازی فیلدهای ورودی
                    }
                    else
                    {
                        MessageBox.Show("حذف بیمار موفق نبود.");
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("خطا در حذف بیمار:\n" + ex.Message);
            }
        }




        





        private void dgvDoctors_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void tabPage2_Click(object sender, EventArgs e)
        {

        }

        private void tabPage1_Click(object sender, EventArgs e)
        {

        }
        //=============================================================================ثبت پزشک جدید
        private void button1_Click_1(object sender, EventArgs e)
        {
            string firstName = txtDoctorFirstName.Text.Trim();
            string lastName = txtDoctorLastName.Text.Trim();
            string specialty = txtSpecialty.Text.Trim();
            string phone = txtDoctorPhone.Text.Trim();

            if (string.IsNullOrEmpty(firstName) || string.IsNullOrEmpty(lastName) ||
                string.IsNullOrEmpty(specialty) || string.IsNullOrEmpty(phone))
            {
                MessageBox.Show("لطفاً همه فیلدها را پر کنید.");
                return;
            }

            string connectionString = "Server=.;Database=Phase2-3;Trusted_Connection=True;";
            string query = "INSERT INTO Doctor (DoctorID, FirstName, LastName, Specialty, Phone) " +
                           "VALUES (@DoctorID, @FirstName, @LastName, @Specialty, @Phone)";

            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    // گرفتن بزرگترین DoctorID موجود برای ایجاد DoctorID جدید
                    int newDoctorID = 1;
                    using (SqlCommand cmdMax = new SqlCommand("SELECT ISNULL(MAX(DoctorID), 0) FROM Doctor", conn))
                    {
                        newDoctorID = (int)cmdMax.ExecuteScalar() + 1;
                    }

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@DoctorID", newDoctorID);
                        cmd.Parameters.AddWithValue("@FirstName", firstName);
                        cmd.Parameters.AddWithValue("@LastName", lastName);
                        cmd.Parameters.AddWithValue("@Specialty", specialty);
                        cmd.Parameters.AddWithValue("@Phone", phone);

                        int rows = cmd.ExecuteNonQuery();

                        if (rows > 0)
                        {
                            MessageBox.Show("دکتر با موفقیت اضافه شد.");
                            LoadDoctors(); // بارگذاری مجدد داده‌ها برای نمایش
                        }
                        else
                        {
                            MessageBox.Show("خطا در افزودن دکتر.");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("خطا:\n" + ex.Message);
            }
        }


        //=============================================================================ویرایش اطلاعات پزشک

        private void btnEditDoctor_Click(object sender, EventArgs e)
        {
            if (dgvDoctors.CurrentRow == null)
            {
                MessageBox.Show("لطفا یک دکتر را انتخاب کنید.");
                return;
            }

            int doctorID = Convert.ToInt32(dgvDoctors.CurrentRow.Cells["DoctorID"].Value);
            string firstName = txtDoctorFirstName.Text.Trim();
            string lastName = txtDoctorLastName.Text.Trim();
            string specialty = txtSpecialty.Text.Trim();
            string phone = txtDoctorPhone.Text.Trim();

            if (string.IsNullOrEmpty(firstName) || string.IsNullOrEmpty(lastName) ||
                string.IsNullOrEmpty(specialty) || string.IsNullOrEmpty(phone))
            {
                MessageBox.Show("لطفاً همه فیلدها را پر کنید.");
                return;
            }

            string connectionString = "Server=.;Database=Phase2-3;Trusted_Connection=True;";
            string query = "UPDATE Doctor SET FirstName = @FirstName, LastName = @LastName, Specialty = @Specialty, Phone = @Phone WHERE DoctorID = @DoctorID";

            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@DoctorID", doctorID);
                        cmd.Parameters.AddWithValue("@FirstName", firstName);
                        cmd.Parameters.AddWithValue("@LastName", lastName);
                        cmd.Parameters.AddWithValue("@Specialty", specialty);
                        cmd.Parameters.AddWithValue("@Phone", phone);

                        int rows = cmd.ExecuteNonQuery();

                        if (rows > 0)
                        {
                            MessageBox.Show("اطلاعات دکتر با موفقیت ویرایش شد.");
                            LoadDoctors();
                        }
                        else
                        {
                            MessageBox.Show("خطا در ویرایش اطلاعات دکتر.");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("خطا:\n" + ex.Message);
            }
        }

        private void dgvDoctors_SelectionChanged(object sender, EventArgs e)
        {
            if (dgvDoctors.CurrentRow != null)
            {
                txtDoctorFirstName.Text = dgvDoctors.CurrentRow.Cells["FirstName"].Value?.ToString() ?? "";
                txtDoctorLastName.Text = dgvDoctors.CurrentRow.Cells["LastName"].Value?.ToString() ?? "";
                txtSpecialty.Text = dgvDoctors.CurrentRow.Cells["Specialty"].Value?.ToString() ?? "";
                txtDoctorPhone.Text = dgvDoctors.CurrentRow.Cells["Phone"].Value?.ToString() ?? "";
            }
        }


        //=============================================================================حذف اطلاعات پزشک

        private void btnDeleteDoctor_Click(object sender, EventArgs e)
        {
            if (dgvDoctors.CurrentRow == null)
            {
                MessageBox.Show("لطفاً یک دکتر را از جدول انتخاب کنید.");
                return;
            }

            int doctorID = Convert.ToInt32(dgvDoctors.CurrentRow.Cells["DoctorID"].Value);

            var confirmResult = MessageBox.Show("آیا از حذف این دکتر مطمئن هستید؟", "تأیید حذف", MessageBoxButtons.YesNo);
            if (confirmResult != DialogResult.Yes)
                return;

            string connectionString = "Server=.;Database=Phase2-3;Trusted_Connection=True;";
            string query = "DELETE FROM Doctor WHERE DoctorID = @DoctorID";

            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@DoctorID", doctorID);

                    conn.Open();
                    int rows = cmd.ExecuteNonQuery();

                    if (rows > 0)
                    {
                        MessageBox.Show("دکتر با موفقیت حذف شد.");
                        LoadDoctors(); // بروز رسانی جدول
                        ClearDoctorInputs(); // پاک کردن فیلدها
                    }
                    else
                    {
                        MessageBox.Show("خطا در حذف.");
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("خطا در حذف:\n" + ex.Message);
            }
        }

        private void dgvAppointments_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void dgvAppointments_SelectionChanged(object sender, EventArgs e)
        {
            if (dgvAppointments.CurrentRow == null)
                return;

            DataGridViewRow row = dgvAppointments.CurrentRow;

            cmbPatients.SelectedValue = GetPatientIDByName(row.Cells["PatientName"].Value?.ToString());
            cmbDoctors.SelectedValue = GetDoctorIDByName(row.Cells["DoctorName"].Value?.ToString());

            if (DateTime.TryParse(row.Cells["Date"].Value?.ToString(), out DateTime dateValue))
                dtpDate.Value = dateValue;
            else
                dtpDate.Value = DateTime.Today;

            if (TimeSpan.TryParse(row.Cells["Time"].Value?.ToString(), out TimeSpan timeValue))
                dtpTime.Value = DateTime.Today.Add(timeValue);
            else
                dtpTime.Value = DateTime.Now;

            string status = row.Cells["Status"].Value?.ToString();
            if (!string.IsNullOrEmpty(status) && cmbStatus.Items.Contains(status))
                cmbStatus.SelectedItem = status;
            else
                cmbStatus.SelectedIndex = 0;

            
        }






        //=============================================================================اضافه کردن نوبیت

        private void btnAddAppointment_Click(object sender, EventArgs e)
        {
            if (cmbPatients.SelectedIndex == -1 || cmbDoctors.SelectedIndex == -1)
            {
                MessageBox.Show("لطفاً بیمار و دکتر را انتخاب کنید.");
                return;
            }

            int patientID = Convert.ToInt32(cmbPatients.SelectedValue);
            int doctorID = Convert.ToInt32(cmbDoctors.SelectedValue);
            DateTime date = dtpDate.Value.Date;
            TimeSpan time = dtpTime.Value.TimeOfDay;
            string status = cmbStatus.SelectedItem?.ToString() ?? "Planned";

            int newAppointmentID = 0;

            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    // دریافت بزرگترین AppointmentID و ساختن یک عدد جدید
                    SqlCommand cmdMax = new SqlCommand("SELECT ISNULL(MAX(AppointmentID), 0) + 1 FROM Appointment", conn);
                    newAppointmentID = (int)cmdMax.ExecuteScalar();

                    string query = @"
                INSERT INTO Appointment (AppointmentID, PatientID, DoctorID, Date, Time, Status)
                VALUES (@AppointmentID, @PatientID, @DoctorID, @Date, @Time, @Status)";

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@AppointmentID", newAppointmentID);
                        cmd.Parameters.AddWithValue("@PatientID", patientID);
                        cmd.Parameters.AddWithValue("@DoctorID", doctorID);
                        cmd.Parameters.AddWithValue("@Date", date);
                        cmd.Parameters.AddWithValue("@Time", time);
                        cmd.Parameters.AddWithValue("@Status", status);

                        int rows = cmd.ExecuteNonQuery();

                        if (rows > 0)
                        {
                            MessageBox.Show("نوبت با موفقیت ثبت شد.");
                            LoadAppointments();
                            ClearAppointmentInputs();
                        }
                        else
                        {
                            MessageBox.Show("ثبت نوبت موفق نبود.");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("خطا در ثبت نوبت:\n" + ex.Message);
            }
        }






        //=============================================================================ویرایش نوبت

        private void btnEditAppointment_Click(object sender, EventArgs e)
        {
            if (dgvAppointments.CurrentRow == null)
            {
                MessageBox.Show("لطفاً یک نوبت را از جدول انتخاب کنید.");
                return;
            }

            int appointmentID = Convert.ToInt32(dgvAppointments.CurrentRow.Cells["AppointmentID"].Value);

            if (cmbPatients.SelectedIndex == -1 || cmbDoctors.SelectedIndex == -1)
            {
                MessageBox.Show("لطفاً بیمار و دکتر را انتخاب کنید.");
                return;
            }

            int patientID = Convert.ToInt32(cmbPatients.SelectedValue);
            int doctorID = Convert.ToInt32(cmbDoctors.SelectedValue);
            DateTime date = dtpDate.Value.Date;
            TimeSpan time = dtpTime.Value.TimeOfDay;
            string status = cmbStatus.SelectedItem.ToString();

            string query = @"
        UPDATE Appointment
        SET PatientID = @PatientID,
            DoctorID = @DoctorID,
            Date = @Date,
            Time = @Time,
            Status = @Status
        WHERE AppointmentID = @AppointmentID";

            using (SqlConnection conn = new SqlConnection(connectionString))
            using (SqlCommand cmd = new SqlCommand(query, conn))
            {
                cmd.Parameters.AddWithValue("@PatientID", patientID);
                cmd.Parameters.AddWithValue("@DoctorID", doctorID);
                cmd.Parameters.AddWithValue("@Date", date);
                cmd.Parameters.AddWithValue("@Time", time);
                cmd.Parameters.AddWithValue("@Status", status);
                cmd.Parameters.AddWithValue("@AppointmentID", appointmentID);

                try
                {
                    conn.Open();
                    int rows = cmd.ExecuteNonQuery();

                    if (rows > 0)
                    {
                        MessageBox.Show("نوبت با موفقیت ویرایش شد.");
                        LoadAppointments();
                        ClearAppointmentInputs();
                    }
                    else
                    {
                        MessageBox.Show("ویرایش نوبت موفق نبود.");
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("خطا در ویرایش نوبت:\n" + ex.Message);
                }
            }
        }




        //=============================================================================حذف نوبت
        private void btnDeleteAppointment_Click(object sender, EventArgs e)
        {
            if (dgvAppointments.CurrentRow == null)
            {
                MessageBox.Show("لطفاً یک نوبت را از جدول انتخاب کنید.");
                return;
            }

            int appointmentID = Convert.ToInt32(dgvAppointments.CurrentRow.Cells["AppointmentID"].Value);

            var confirmResult = MessageBox.Show("آیا مطمئن هستید که می‌خواهید این نوبت را حذف کنید؟",
                                                "تأیید حذف",
                                                MessageBoxButtons.YesNo);

            if (confirmResult == DialogResult.No)
                return;

            string query = "DELETE FROM Appointment WHERE AppointmentID = @AppointmentID";

            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@AppointmentID", appointmentID);
                    conn.Open();

                    int rows = cmd.ExecuteNonQuery();

                    if (rows > 0)
                    {
                        MessageBox.Show("نوبت با موفقیت حذف شد.");
                        LoadAppointments();  // دوباره جدول رو بارگذاری کن
                    }
                    else
                    {
                        MessageBox.Show("حذف نوبت موفق نبود.");
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("خطا در حذف نوبت:\n" + ex.Message);
            }
        }

        private void cmbPatients_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void tabPage3_Click(object sender, EventArgs e)
        {
            
        }

        private void tabControl1_Selected(object sender, TabControlEventArgs e)
        {
            LoadPatientsForCombo();
            LoadDoctorsForCombo();


            switch (tabControl1.SelectedIndex)
            {
                case 0:
                    pictureBox1.Image = Properties.Resources.patient;      // اسم resource عکس بیماران
                    break;
                case 1:
                    pictureBox1.Image = Properties.Resources.Doctor;       // اسم resource عکس پزشکان
                    break;
                case 2:
                    pictureBox1.Image = Properties.Resources.visit;  // اسم resource عکس نوبت‌ها
                    break;
                case 3:
                    pictureBox1.Image = Properties.Resources.medical;       // اسم resource سوابق
                    break;
            }




        }

        private void cmbGender_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void tabControl1_Selecting(object sender, TabControlCancelEventArgs e)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string query = @"
            SELECT 
                a.AppointmentID, 
                N'نوبت ' + CAST(a.AppointmentID AS NVARCHAR) + N' - ' + 
                p.FirstName + N' ' + p.LastName + N' / ' + 
                d.FirstName + N' ' + d.LastName + N' / ' + 
                FORMAT(a.Date, 'yyyy-MM-dd') AS DisplayText
            FROM Appointment a
            JOIN Patient p ON a.PatientID = p.PatientID
            JOIN Doctor d ON a.DoctorID = d.DoctorID";

                SqlDataAdapter da = new SqlDataAdapter(query, conn);
                DataTable dt = new DataTable();
                da.Fill(dt);

                cmbAppointment.DisplayMember = "DisplayText";
                cmbAppointment.ValueMember = "AppointmentID";
                cmbAppointment.DataSource = dt;
            }

        }

        private void LoadMedicalRecords()
            {
            using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    string query = @"
                SELECT r.RecordID, r.AppointmentID, r.RecordDate, r.DiagnosisDescription, 
                       r.PrescribedMedications,
                       p.FirstName + ' ' + p.LastName AS PatientName,
                       d.FirstName + ' ' + d.LastName AS DoctorName
                FROM MedicalRecord r
                JOIN Appointment a ON r.AppointmentID = a.AppointmentID
                JOIN Patient p ON a.PatientID = p.PatientID
                JOIN Doctor d ON a.DoctorID = d.DoctorID";

                    SqlDataAdapter da = new SqlDataAdapter(query, conn);
                    DataTable dt = new DataTable();
                    da.Fill(dt);
                    dgvMedicalRecords.DataSource = dt;
                }
            }

        

        private void dgvMedicalRecords_SelectionChanged(object sender, EventArgs e)
        {
            if (dgvMedicalRecords.CurrentRow != null)
            {
                var row = dgvMedicalRecords.CurrentRow;

                txtRecordID.Text = row.Cells["RecordID"].Value?.ToString() ?? "";
                cmbAppointment.SelectedValue = row.Cells["AppointmentID"].Value ?? null;
                dtpRecordDate.Value = row.Cells["RecordDate"].Value != DBNull.Value
                    ? Convert.ToDateTime(row.Cells["RecordDate"].Value)
                    : DateTime.Today;
                txtDiagnosis.Text = row.Cells["DiagnosisDescription"].Value?.ToString() ?? "";
                txtMedications.Text = row.Cells["PrescribedMedications"].Value?.ToString() ?? "";
            }
        }

        private void cmbAppointment_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void btnAddRecord_Click(object sender, EventArgs e)
        {

            if (cmbAppointment.SelectedValue == null || string.IsNullOrWhiteSpace(txtDiagnosis.Text))
            {
                MessageBox.Show("لطفاً نوبت و شرح بیماری را وارد کنید.");
                return;
            }

            int appointmentId = Convert.ToInt32(cmbAppointment.SelectedValue);
            DateTime recordDate = dtpRecordDate.Value;
            string diagnosis = txtDiagnosis.Text.Trim();
            string medications = txtMedications.Text.Trim();

            string query = @"INSERT INTO MedicalRecord 
                     (AppointmentID, RecordDate, DiagnosisDescription, PrescribedMedications)
                     VALUES (@AppointmentID, @RecordDate, @DiagnosisDescription, @PrescribedMedications)";

            using (SqlConnection con = new SqlConnection(connectionString))
            using (SqlCommand cmd = new SqlCommand(query, con))
            {
                cmd.Parameters.AddWithValue("@AppointmentID", appointmentId);
                cmd.Parameters.AddWithValue("@RecordDate", recordDate);
                cmd.Parameters.AddWithValue("@DiagnosisDescription", diagnosis);
                cmd.Parameters.AddWithValue("@PrescribedMedications", string.IsNullOrWhiteSpace(medications) ? (object)DBNull.Value : medications);

                con.Open();
                cmd.ExecuteNonQuery();
                con.Close();
            }

            MessageBox.Show("پرونده پزشکی با موفقیت اضافه شد.");
            LoadMedicalRecords(); // بروزرسانی جدول
        }

        private void btnUpdateRecord_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtRecordID.Text))
            {
                MessageBox.Show("لطفاً ابتدا یک رکورد را انتخاب کنید.");
                return;
            }

            int recordId = int.Parse(txtRecordID.Text);
            int appointmentId = Convert.ToInt32(cmbAppointment.SelectedValue);
            DateTime recordDate = dtpRecordDate.Value;
            string diagnosis = txtDiagnosis.Text.Trim();
            string medications = txtMedications.Text.Trim();

            string query = @"
        UPDATE MedicalRecord
        SET AppointmentID = @AppointmentID,
            RecordDate = @RecordDate,
            DiagnosisDescription = @DiagnosisDescription,
            PrescribedMedications = @PrescribedMedications
        WHERE RecordID = @RecordID";

            using (SqlConnection con = new SqlConnection(connectionString))
            using (SqlCommand cmd = new SqlCommand(query, con))
            {
                cmd.Parameters.AddWithValue("@AppointmentID", appointmentId);
                cmd.Parameters.AddWithValue("@RecordDate", recordDate);
                cmd.Parameters.AddWithValue("@DiagnosisDescription", diagnosis);
                cmd.Parameters.AddWithValue("@PrescribedMedications", string.IsNullOrWhiteSpace(medications) ? (object)DBNull.Value : medications);
                cmd.Parameters.AddWithValue("@RecordID", recordId);

                con.Open();
                int rowsAffected = cmd.ExecuteNonQuery();
                con.Close();

                if (rowsAffected > 0)
                {
                    MessageBox.Show("رکورد با موفقیت ویرایش شد.");
                    LoadMedicalRecords();
                }
                else
                {
                    MessageBox.Show("ویرایش رکورد ناموفق بود.");
                }
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtRecordID.Text))
            {
                MessageBox.Show("لطفاً ابتدا یک رکورد را انتخاب کنید.");
                return;
            }

            int recordId = int.Parse(txtRecordID.Text);

            var confirmResult = MessageBox.Show("آیا از حذف این رکورد مطمئن هستید؟",
                                                "تأیید حذف",
                                                MessageBoxButtons.YesNo);
            if (confirmResult == DialogResult.No)
                return;

            string query = "DELETE FROM MedicalRecord WHERE RecordID = @RecordID";

            using (SqlConnection con = new SqlConnection(connectionString))
            using (SqlCommand cmd = new SqlCommand(query, con))
            {
                cmd.Parameters.AddWithValue("@RecordID", recordId);

                con.Open();
                int rowsAffected = cmd.ExecuteNonQuery();
                con.Close();

                if (rowsAffected > 0)
                {
                    MessageBox.Show("رکورد با موفقیت حذف شد.");
                    LoadMedicalRecords();
                    ClearMedicalRecordInputs();
                }
                else
                {
                    MessageBox.Show("حذف رکورد ناموفق بود.");
                }
            }
        }

        private void button1_Click_2(object sender, EventArgs e)
        {
            PaymentForm paymentForm = new PaymentForm();
            paymentForm.ShowDialog(); // فرم PaymentForm رو به صورت مودال باز می‌کنه (کاربر باید ببنده تا برگرده فرم اول)
        }

        private void EmployeeShow_Click(object sender, EventArgs e)
        {
            EmployeeForm EmployeeForm = new EmployeeForm();
            EmployeeForm.ShowDialog(); // فرم PaymentForm رو به صورت مودال باز می‌کنه (کاربر باید ببنده تا برگرده فرم اول)
        }

        private void txtDoctorLastName_TextChanged(object sender, EventArgs e)
        {

        }

        private void txtSpecialty_TextChanged(object sender, EventArgs e)
        {

        }

        private void txtDoctorFirstName_TextChanged(object sender, EventArgs e)
        {

        }

        private void txtDoctorPhone_TextChanged(object sender, EventArgs e)
        {

        }
    }
    

}
