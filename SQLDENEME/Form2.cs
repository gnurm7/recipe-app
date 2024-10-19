using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace SQLDENEME
{
    public partial class Form2 : Form
    {
        // SQL bağlantı dizesi
        SqlConnection conn = new SqlConnection("Data Source=DESKTOP-PMBMQKM\\SQLEXPRESS;Initial Catalog=Yazlab;Integrated Security=True;;");
        public Form2()
        {
            InitializeComponent();
            
        }

        

        private void Form2_Load(object sender, EventArgs e)
        {
         

            listele();
            KategorileriYukle();
            // ComboBox3'e sıralama kriterlerini ekleme
            comboBox3.Items.Add("Hazırlama Süresi (Artan)");
            comboBox3.Items.Add("Hazırlama Süresi (Azalan)");
            comboBox3.Items.Add("Tarif Maliyeti (Artan)");
            comboBox3.Items.Add("Tarif Maliyeti (Azalan)");

            // ComboBox3'e SelectedIndexChanged olayını bağlama
            comboBox3.SelectedIndexChanged += new EventHandler(comboBox3_SelectedIndexChanged);
            textBox7.TextChanged += new EventHandler(textBox7_TextChanged);
        }
        public void listele()
        {
            try
            {
                //conn.Open();

                DataSet ds = new DataSet();
                SqlDataAdapter ad = new SqlDataAdapter("select * from Tbl_Tarifler", conn);
                ad.Fill(ds);

                dataGridView1.DataSource = ds.Tables[0];

                dataGridView1.Columns[1].HeaderText = "Tarif Adı";
                dataGridView1.Columns[2].HeaderText = "Kategori";
                dataGridView1.Columns[3].HeaderText = "Hazırlanma Süresi";
                dataGridView1.Columns[4].HeaderText = "Talimatlar";
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
        // DataGridView'e tıklandığında bilgileri TextBox ve ComboBox'a doldur
        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                // Seçili satırdaki hücrelere ulaşma
                DataGridViewRow row = dataGridView1.Rows[e.RowIndex];

                // TextBox ve ComboBox'ları doldur
                textBox1.Text = row.Cells[1].Value.ToString(); // Tarif Adı
                comboBox2.Text = row.Cells[2].Value.ToString(); // Kategori
                textBox2.Text = row.Cells[3].Value.ToString(); // Hazırlanma Süresi
                textBox3.Text = row.Cells[4].Value.ToString(); // Talimatlar
            }
        }

        private void KategorileriYukle()
        {
            try
            {
                conn.Open(); // Veritabanı bağlantısını aç

                // Kategorileri almak için SQL sorgusu
                SqlCommand komut = new SqlCommand("SELECT DISTINCT Kategori FROM Tbl_Tarifler", conn);
                SqlDataReader dr = komut.ExecuteReader();

                // ComboBox'ları temizle
                comboBox1.Items.Clear();
               comboBox2.Items.Clear();

                // Her kategori için comboBox'lara ekleme yap
                while (dr.Read())
                {
                    comboBox1.Items.Add(dr["Kategori"].ToString());
                    comboBox2.Items.Add(dr["Kategori"].ToString());
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

        private void tarifEkle_Click(object sender, EventArgs e)
        {
            // Hata kontrolü: Eğer herhangi bir TextBox veya ComboBox boşsa hata oluştur
            int hata = 0;

            if (string.IsNullOrWhiteSpace(textBox4.Text)) // Tarif Adı
                hata = 1;
            if (string.IsNullOrWhiteSpace(comboBox1.Text)) // Kategori
                hata = 1;
            if (string.IsNullOrWhiteSpace(textBox5.Text)) // Hazırlanma Süresi
                hata = 1;
            if (string.IsNullOrWhiteSpace(textBox6.Text)) // Talimatlar
                hata = 1;

            // Eğer hata varsa uyarı mesajı göster
            if (hata == 1)
            {
                MessageBox.Show("Lütfen tüm alanları doldurun", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else
            {
                try
                {
                    // Veritabanı bağlantısını aç
                    conn.Open();

                    // SQL Insert komutu
                    SqlCommand ekle = new SqlCommand(
                        "INSERT INTO Tbl_Tarifler (TarifAdi, Kategori, HazirlanmaSuresi, Talimatlar)" +
                        " VALUES (@TarifAdi, @Kategori, @HazirlanmaSuresi, @Talimatlar)", conn
                    );

                    // Parametreleri ekle
                    ekle.Parameters.AddWithValue("@TarifAdi", textBox4.Text); // Tarif Adı
                    ekle.Parameters.AddWithValue("@Kategori", comboBox1.SelectedItem?.ToString() ?? ""); // Kategori
                    ekle.Parameters.AddWithValue("@HazirlanmaSuresi", textBox5.Text); // Hazırlanma Süresi
                    ekle.Parameters.AddWithValue("@Talimatlar", textBox6.Text); // Talimatlar

                    // Komutu çalıştır
                    int basari = ekle.ExecuteNonQuery();

                    // İşlem başarılı olduysa
                    if (basari > 0)
                    {
                        MessageBox.Show("Kayıt başarıyla eklendi", "Başarılı", MessageBoxButtons.OK, MessageBoxIcon.Information);

                        // TextBox ve ComboBox temizlenir
                        textBox4.Text = "";
                        comboBox1.SelectedIndex = -1; // ComboBox'ı temizlemek için
                        textBox5.Text = "";
                        textBox6.Text = "";
                    }
                    else
                    {
                        MessageBox.Show("Kayıt eklenirken bir hata oluştu", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                catch (SqlException edf)
                {
                    // SQL hatasını yakala ve göster
                    MessageBox.Show("Hata oluştu: " + edf.Message, "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                finally
                {
                    // Veritabanı bağlantısını kapat
                    if (conn.State == ConnectionState.Open)
                    {
                        conn.Close();
                    }
                }
            }
        }

        private void tarifGuncelle_Click(object sender, EventArgs e)
        {
            if (dataGridView1.CurrentRow == null)
            {
                MessageBox.Show("Lütfen güncellemek için bir tarif seçin.", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            int tarifID = Convert.ToInt32(dataGridView1.CurrentRow.Cells[0].Value); // TarifID'yi al
            TarifGuncelle(tarifID);
        }

        private void TarifGuncelle(int tarifID)
        {
            try
            {
                conn.Open();
                SqlCommand guncelle = new SqlCommand("UPDATE Tbl_Tarifler SET TarifAdi = @TarifAdi, Kategori = @Kategori, " +
                                                      "HazirlanmaSuresi = @HazirlanmaSuresi, Talimatlar = @Talimatlar " +
                                                      "WHERE TarifID = @TarifID", conn);
                guncelle.Parameters.AddWithValue("@TarifAdi", textBox1.Text);
                guncelle.Parameters.AddWithValue("@Kategori", comboBox2.SelectedItem.ToString());
                guncelle.Parameters.AddWithValue("@HazirlanmaSuresi", textBox2.Text);
                guncelle.Parameters.AddWithValue("@Talimatlar", textBox3.Text);
                guncelle.Parameters.AddWithValue("@TarifID", tarifID);

                int etkilenenSatir = guncelle.ExecuteNonQuery();
                if (etkilenenSatir > 0)
                {
                    MessageBox.Show("Güncelleme başarılı.");
                    listele(); // Güncelleme sonrası listeyi yenile
                }
                else
                {
                    MessageBox.Show("Güncelleme başarısız; kayıt bulunamadı.");
                }
            }
            catch (SqlException edf)
            {
                MessageBox.Show("Hata oluştu: " + edf.Message, "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                conn.Close(); // Bağlantıyı kapat  
            }
        }

        private void tarifSil_Click(object sender, EventArgs e)
        {
            if (dataGridView1.CurrentRow != null)
            {
                try
                {
                    int tarifID = Convert.ToInt32(dataGridView1.CurrentRow.Cells["TarifID"].Value);

                    conn.Open();
                    SqlCommand sil = new SqlCommand("DELETE FROM Tbl_Tarifler WHERE TarifID = @TarifID", conn);
                    sil.Parameters.AddWithValue("@TarifID", tarifID);

                    int silinenSatir = sil.ExecuteNonQuery();

                    if (silinenSatir > 0)
                    {
                        MessageBox.Show("Kayıt başarıyla silindi.");
                        listele(); // Tabloyu güncellemek için tekrar listele
                    }
                    else
                    {
                        MessageBox.Show("Silme işlemi başarısız oldu; kayıt bulunamadı.");
                    }
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
            else
            {
                MessageBox.Show("Lütfen silmek için bir tarif seçin.", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void comboBox3_SelectedIndexChanged(object sender, EventArgs e)
        {
           
                // Kullanıcının seçtiği sıralama kriterine göre SQL sorgusu
                string sortQuery = "";

                switch (comboBox3.SelectedItem.ToString())
                {
                    case "Hazırlama Süresi (Artan)":
                        sortQuery = "SELECT * FROM Tbl_Tarifler ORDER BY HazirlanmaSuresi ASC"; 
                        break;

                    case "Hazırlama Süresi (Azalan)":
                        sortQuery = "SELECT * FROM Tbl_Tarifler ORDER BY HazirlanmaSuresi DESC";
                        break;

                    case "Tarif Maliyeti (Artan)":
                        sortQuery = "SELECT * FROM Tbl_Tarifler ORDER BY TarifMaliyeti ASC";
                        break;

                    case "Tarif Maliyeti (Azalan)":
                        sortQuery = "SELECT * FROM Tbl_Tarifler ORDER BY TarifMaliyeti DESC";
                        break;

                    default:
                        sortQuery = "SELECT * FROM Tbl_Tarifler"; // Varsayılan, eğer bir şey seçilmezse
                        break;
                }

                // Sıralama sorgusunu çalıştırma ve DataGridView'e verileri yükleme
                try
                {
                    conn.Open();
                    SqlDataAdapter adtr = new SqlDataAdapter(sortQuery, conn);
                    DataTable dt = new DataTable();
                    adtr.Fill(dt);
                    dataGridView1.DataSource = dt;
                }
                catch (SqlException ex)
                {
                    MessageBox.Show("Hata oluştu: " + ex.Message, "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                finally
                {
                    conn.Close();
                }
            
        }

        private void textBox7_TextChanged(object sender, EventArgs e)
        {
            AramaYap();
        }


        private void AramaYap()
        {
            DataTable table = new DataTable(); // Yeni bir DataTable oluştur
            table.Clear(); // Daha önceki verileri temizle

            try
            {
                conn.Open(); // Bağlantıyı aç

                // SQL sorgusu, textBox7'ye girilen kelimeyi kullanarak LIKE ile filtreler
                SqlDataAdapter adtr = new SqlDataAdapter("SELECT * FROM Tbl_Tarifler WHERE TarifAdi LIKE @arama OR Kategori LIKE @arama OR HazirlanmaSuresi LIKE @arama OR Talimatlar LIKE @arama", conn);

                // Parametreleri ekle
                adtr.SelectCommand.Parameters.AddWithValue("@arama", "%" + textBox7.Text + "%"); // Parametreli arama

                adtr.Fill(table); // DataTable'a verileri doldur
                dataGridView1.DataSource = table; // DataGridView'e verileri bağla
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

    }
}
