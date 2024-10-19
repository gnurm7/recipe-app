using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Windows.Forms;

namespace SQLDENEME
{
    public partial class Form1 : Form
    {
        SqlConnection conn = new SqlConnection("Data Source=DESKTOP-PMBMQKM\\SQLEXPRESS;Initial Catalog=Yazlab;Integrated Security=True;");

        public Form1()
        {
            InitializeComponent();

            // Form yüklendiğinde malzemeleri ComboBox'lara ekle
            MalzemeleriYukle();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            // ComboBox'lardaki seçim değişikliklerini dinle
            comboBox1.SelectedIndexChanged += comboBox_SelectedIndexChanged;
            comboBox2.SelectedIndexChanged += comboBox_SelectedIndexChanged;
            comboBox3.SelectedIndexChanged += comboBox_SelectedIndexChanged;
        }

        private void MalzemeleriYukle()
        {
            try
            {
                conn.Open();
                SqlCommand komut = new SqlCommand("SELECT DISTINCT MalzemeAdi FROM Tbl_Malzemeler", conn);
                SqlDataReader dr = komut.ExecuteReader();

                // ComboBox'ları temizle ve malzemeleri ekle
                comboBox1.Items.Clear();
                comboBox2.Items.Clear();
                comboBox3.Items.Clear();

                while (dr.Read())
                {
                    string malzemeAdi = dr["MalzemeAdi"].ToString();
                    comboBox1.Items.Add(malzemeAdi);
                    comboBox2.Items.Add(malzemeAdi);
                    comboBox3.Items.Add(malzemeAdi);
                }
                dr.Close();
            }
            catch (SqlException ex)
            {
                MessageBox.Show($"Hata: {ex.Message}", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                conn.Close();
            }
        }

        private void comboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Seçim değiştikçe tarifleri filtrele
            FiltreliTarifleriGoster();
        }

        private void FiltreliTarifleriGoster()
        {
            // ComboBox'lardan seçilen malzeme ID'lerini al
            List<int?> malzemeIds = new List<int?>
    {
        GetMalzemeId(comboBox1.SelectedItem?.ToString()),
        GetMalzemeId(comboBox2.SelectedItem?.ToString()),
        GetMalzemeId(comboBox3.SelectedItem?.ToString())
    };

            // Seçilen geçerli malzeme sayısını hesapla
            int secilenMalzemeSayisi = malzemeIds.Count(id => id.HasValue);

            if (secilenMalzemeSayisi == 0)
            {
                MessageBox.Show("Lütfen en az bir malzeme seçiniz.", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // SQL Sorgusu: Tariflerin malzemelerle eşleşme yüzdesini hesapla
            string query = @"
    SELECT T.TarifID, T.TarifAdi, T.Kategori, T.HazirlanmaSuresi,
           COUNT(DISTINCT TM.MalzemeID) * 100.0 / @SecilenMalzemeSayisi AS EslesmeYuzdesi
    FROM Tbl_Tarifler T
    INNER JOIN Tbl_TarifMalzeme_iliskisi TM ON T.TarifID = TM.TarifID
    WHERE TM.MalzemeID IN (@MalzemeId1, @MalzemeId2, @MalzemeId3)
    GROUP BY T.TarifID, T.TarifAdi, T.Kategori, T.HazirlanmaSuresi
    HAVING COUNT(DISTINCT TM.MalzemeID) > 0
    ORDER BY EslesmeYuzdesi DESC";

            try
            {
                conn.Open();
                SqlCommand komut = new SqlCommand(query, conn);

                // Parametreleri ekle
                komut.Parameters.AddWithValue("@MalzemeId1", malzemeIds[0] ?? (object)DBNull.Value);
                komut.Parameters.AddWithValue("@MalzemeId2", malzemeIds[1] ?? (object)DBNull.Value);
                komut.Parameters.AddWithValue("@MalzemeId3", malzemeIds[2] ?? (object)DBNull.Value);
                komut.Parameters.AddWithValue("@SecilenMalzemeSayisi", secilenMalzemeSayisi);

                SqlDataAdapter adtr = new SqlDataAdapter(komut);
                DataTable dt = new DataTable();
                adtr.Fill(dt);

                // DataGridView'e verileri yükle
                dataGridView1.DataSource = dt;
            }
            catch (SqlException ex)
            {
                MessageBox.Show($"Hata: {ex.Message}", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                conn.Close();
            }
        }

        // Malzeme adını kullanarak malzeme ID'sini getirir
        private int? GetMalzemeId(string malzemeAdi)
        {
            if (string.IsNullOrEmpty(malzemeAdi)) return null;

            string query = "SELECT MalzemeID FROM Tbl_Malzemeler WHERE MalzemeAdi = @MalzemeAdi";

            try
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@MalzemeAdi", malzemeAdi);
                var result = cmd.ExecuteScalar();
                return result != null ? (int?)Convert.ToInt32(result) : null;
            }
            catch (SqlException ex)
            {
                MessageBox.Show($"Hata: {ex.Message}", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return null;
            }
            finally
            {
                conn.Close();
            }
        }


        private void button1_Click(object sender, EventArgs e)
        {
            // Yeni formu aç
            Form2 menu = new Form2();
            menu.Show();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            // Butona tıklanınca yapılacak işlemler
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }
    }
}
