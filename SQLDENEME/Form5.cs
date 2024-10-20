using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace SQLDENEME
{
    public partial class Form5 : Form
    {
        private SqlConnection conn = new SqlConnection("Data Source=DESKTOP-PMBMQKM\\SQLEXPRESS;Initial Catalog=Yazlab;Integrated Security=True;");
        private int selectedTarifId;

        public Form5(int tarifID)
        {
            InitializeComponent();
            selectedTarifId = tarifID; // Geçirilen TarifID'yi kaydediyoruz
        }

        private void Form5_Load(object sender, EventArgs e)
        {
            LoadTarifler();
            LoadMalzemeler();
            comboBox1.SelectedValue = selectedTarifId; // Eklenen tarifi otomatik olarak seç
        }

        private void LoadTarifler()
        {
            string query = "SELECT TarifID, TarifAdi FROM Tbl_Tarifler ORDER BY TarifAdi";
            DataTable dataTable = new DataTable();

            using (SqlCommand command = new SqlCommand(query, conn))
            {
                try
                {
                    conn.Open();
                    SqlDataAdapter adapter = new SqlDataAdapter(command);
                    adapter.Fill(dataTable);

                    comboBox1.DataSource = dataTable;
                    comboBox1.DisplayMember = "TarifAdi";
                    comboBox1.ValueMember = "TarifID";
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Tarifler yüklenirken bir hata oluştu: " + ex.Message);
                }
                finally
                {
                    conn.Close();
                }
            }
        }

        private void LoadMalzemeler()
        {
            string query = "SELECT MalzemeID, MalzemeAdi FROM Tbl_Malzemeler ORDER BY MalzemeAdi";
            DataTable dataTable = new DataTable();

            using (SqlCommand command = new SqlCommand(query, conn))
            {
                try
                {
                    conn.Open();
                    SqlDataAdapter adapter = new SqlDataAdapter(command);
                    adapter.Fill(dataTable);

                    comboBox2.DataSource = dataTable;
                    comboBox2.DisplayMember = "MalzemeAdi";
                    comboBox2.ValueMember = "MalzemeID";
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Malzemeler yüklenirken bir hata oluştu: " + ex.Message);
                }
                finally
                {
                    conn.Close();
                }
            }
        }



        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Eğer gerekli bir işlem yapmak istersen buraya ekleyebilirsin.
        }



        private void button3_Click_1(object sender, EventArgs e)
        {
            LoadMalzemeler();
        }

        private void button2_Click_1(object sender, EventArgs e)
        {
            // Yeni malzeme ekleme formunu açma işlemi
            Form4 malzemeekle = new Form4();
            malzemeekle.Show();
        }

        private void button1_Click_1(object sender, EventArgs e)
        {
            int tarifID = (int)comboBox1.SelectedValue;
            int malzemeID = (int)comboBox2.SelectedValue;

            if (!float.TryParse(textBox1.Text, out float malzemeMiktar))
            {
                MessageBox.Show("Lütfen geçerli bir miktar girin.");
                return;
            }

            string query = "INSERT INTO Tbl_TarifMalzeme_iliskisi (TarifID, MalzemeID, MalzemeMiktar) VALUES (@TarifID, @MalzemeID, @MalzemeMiktar)";

            using (SqlCommand command = new SqlCommand(query, conn))
            {
                command.Parameters.AddWithValue("@TarifID", tarifID);
                command.Parameters.AddWithValue("@MalzemeID", malzemeID);
                command.Parameters.AddWithValue("@MalzemeMiktar", malzemeMiktar);

                try
                {
                    conn.Open();
                    command.ExecuteNonQuery();
                    MessageBox.Show("Veri başarıyla eklendi.");
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Veri eklenirken bir hata oluştu: " + ex.Message);
                }
                finally
                {
                    conn.Close();
                }
            }
        }
    }

}
