using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace SQLDENEME
{
    public partial class Form4 : Form
    {
        SqlConnection conn = new SqlConnection("Data Source=DESKTOP-PMBMQKM\\SQLEXPRESS;Initial Catalog=Yazlab;Integrated Security=True;;");

        public Form4()
        {
            InitializeComponent();
            BirimTurleriniYukle();
            Malzemelistele();
        }
        private void BirimTurleriniYukle()
        {
            try
            {
                conn.Open(); // Veritabanı bağlantısını aç

                //  MalzemeBirim almak için SQL sorgusu
                SqlCommand komut = new SqlCommand("SELECT DISTINCT MalzemeBirim  FROM Tbl_Malzemeler", conn);
                SqlDataReader dr = komut.ExecuteReader();

                // ComboBox'ları temizle
                comboBox4.Items.Clear();


                // Her  MalzemeBirimi için comboBox'lara ekleme yap
                while (dr.Read())
                {
                    comboBox4.Items.Add(dr["MalzemeBirim"].ToString());

                }

                dr.Close(); // DataReader'ı kapat
            }
            catch (SqlException ex)
            {
                MessageBox.Show("Hata oluştu: " + ex.Message, "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                conn.Close(); // Bağlantıyı kapat
            }
        }
        public void Malzemelistele()
        {
            try
            {
                //conn.Open();

                DataSet ds = new DataSet();
                SqlDataAdapter ad = new SqlDataAdapter("select * from Tbl_Malzemeler", conn);
                ad.Fill(ds);

                dataGridView2.DataSource = ds.Tables[0];

                dataGridView2.Columns[1].HeaderText = " Malzeme Adi";
                dataGridView2.Columns[2].HeaderText = "Toplam Miktar";
                dataGridView2.Columns[3].HeaderText = "Malzeme Birim";
                dataGridView2.Columns[4].HeaderText = "Birim Fiyat";
            }
            catch (SqlException edf)
            {
                MessageBox.Show("Hata oluştu: " + edf.Message, "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                //if (conn.State == ConnectionState.Open)
                conn.Close();
            }
        }
        private void button1_Click(object sender, EventArgs e)
        {
            // Kullanıcıdan malzeme bilgilerini al
            string malzemeAdi = textBox8.Text;
            string miktarStr = textBox9.Text;
            string birimturu = comboBox4.SelectedItem?.ToString() ?? ""; // ComboBox'tan seçilen birim
            string birimFiyatStr = textBox11.Text;

            // Veritabanı bağlantısı için connection string
            string connectionString = "Data Source=DESKTOP-PMBMQKM\\SQLEXPRESS;Initial Catalog=Yazlab;Integrated Security=True;";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string query = "INSERT INTO Tbl_Malzemeler (MalzemeAdi, ToplamMiktar, MalzemeBirim, BirimFiyat) " +
                               "VALUES (@MalzemeAdi, @ToplamMiktar, @MalzemeBirim, @BirimFiyat)";
                SqlCommand command = new SqlCommand(query, connection);

                // Parametreleri ekle
                command.Parameters.AddWithValue("@MalzemeAdi", malzemeAdi);
                command.Parameters.AddWithValue("@ToplamMiktar", Convert.ToDecimal(miktarStr));
                command.Parameters.AddWithValue("@MalzemeBirim", birimturu);
                command.Parameters.AddWithValue("@BirimFiyat", Convert.ToDecimal(birimFiyatStr));

                try
                {
                    connection.Open();
                    int result = command.ExecuteNonQuery();

                    if (result > 0)
                    {
                        MessageBox.Show("Malzeme başarıyla eklendi.", "Başarılı", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        Malzemelistele(); // Güncel verileri yükle
                    }
                    else
                    {
                        MessageBox.Show("Malzeme eklenirken bir hata oluştu.", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                catch (SqlException ex)
                {
                    MessageBox.Show("Veritabanında bir hata oluştu: " + ex.Message, "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void Form4_Load(object sender, EventArgs e)
        {

        }
    }
}
