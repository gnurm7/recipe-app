using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace SQLDENEME
{
    public partial class Form1 : Form
    {
        SqlConnection conn = new SqlConnection("Data Source=DESKTOP-PMBMQKM\\SQLEXPRESS;Initial Catalog=Yazlab;Integrated Security=True;");

        public Form1()
        {
            InitializeComponent();
            LoadTariflerToGrid(); // Form yüklendiğinde tarifleri yükle
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            dataGridView1.CellContentClick += dataGridView1_CellContentClick;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Form2 menu = new Form2();
            menu.Show();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Form3 tarifOnerisi = new Form3();
            tarifOnerisi.Show();
        }

        private void LoadTariflerToGrid()
        {
            using (conn)
            {
                string query = "SELECT TarifID, TarifAdi FROM Tbl_Tarifler";
                DataTable dataTable = new DataTable();

                try
                {
                    conn.Open();
                    SqlDataAdapter adapter = new SqlDataAdapter(query, conn);
                    adapter.Fill(dataTable);

                    // Maliyet sütununu ekle
                    dataTable.Columns.Add("Maliyet", typeof(string));

                    // Her tarif için maliyeti hesapla ve tabloya ekle
                    foreach (DataRow row in dataTable.Rows)
                    {
                        int tarifID = Convert.ToInt32(row["TarifID"]);
                        float maliyet = HesaplaToplamMaliyet(tarifID);
                        row["Maliyet"] = maliyet.ToString("0.00") + " TL";
                    }

                    // DataGridView'e veriyi bağla
                    dataGridView1.DataSource = dataTable;

                    // Sütun başlıklarını düzenle
                    dataGridView1.Columns["TarifID"].Visible = false; // TarifID gizli
                    dataGridView1.Columns["TarifAdi"].HeaderText = "Tarif Adı";
                    dataGridView1.Columns["Maliyet"].HeaderText = "Toplam Maliyet";
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Tarifler yüklenirken bir hata oluştu: " + ex.Message);
                }
            }
        }

        private float HesaplaToplamMaliyet(int selectedTarifID)
        {
            string connectionString = "Data Source=DESKTOP-PMBMQKM\\SQLEXPRESS;Initial Catalog=Yazlab;Integrated Security=True;";

            // İlk liste: Tbl_TarifMalzeme_iliskisi tablosundan MalzemeID ve MalzemeMiktarlarını al (int, string)
            List<Tuple<int, string>> malzemeMiktarListesi = new List<Tuple<int, string>>();
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string queryMalzemeler = "SELECT MalzemeID, CAST(MalzemeMiktar AS VARCHAR) AS MalzemeMiktar FROM Tbl_TarifMalzeme_iliskisi WHERE TarifID = @TarifID";
                SqlCommand command = new SqlCommand(queryMalzemeler, connection);
                command.Parameters.AddWithValue("@TarifID", selectedTarifID);

                try
                {
                    connection.Open();
                    SqlDataReader reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        int malzemeID = reader.GetInt32(0);
                        string malzemeMiktarStr = reader.GetString(1);
                        malzemeMiktarListesi.Add(new Tuple<int, string>(malzemeID, malzemeMiktarStr));
                    }
                    reader.Close();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Malzemeler yüklenirken bir hata oluştu: " + ex.Message);
                    return -1; // Hata durumunda -1 döndür
                }
            }

            if (malzemeMiktarListesi.Count == 0)
            {
                MessageBox.Show("Bu tarif için malzeme bulunamadı.");
                return -1;
            }

            // İkinci liste: MalzemeID'leri kullanarak Tbl_Malzemeler tablosundan BirimFiyat ve MalzemeMiktarlarını al (string, string)
            List<Tuple<string, string>> fiyatMiktarListesi = new List<Tuple<string, string>>();
            foreach (var malzeme in malzemeMiktarListesi)
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    string queryBirimFiyat = "SELECT CAST(BirimFiyat AS VARCHAR) AS BirimFiyat FROM Tbl_Malzemeler WHERE MalzemeID = @MalzemeID";
                    SqlCommand command = new SqlCommand(queryBirimFiyat, connection);
                    command.Parameters.AddWithValue("@MalzemeID", malzeme.Item1);

                    try
                    {
                        connection.Open();
                        var result = command.ExecuteScalar();
                        if (result != null)
                        {
                            string birimFiyatStr = result.ToString();
                            fiyatMiktarListesi.Add(new Tuple<string, string>(birimFiyatStr, malzeme.Item2));
                        }
                        else
                        {
                            MessageBox.Show("Birim fiyat bulunamadı: MalzemeID " + malzeme.Item1);
                            return -1;
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Birim fiyat hesaplanırken bir hata oluştu: " + ex.Message);
                        return -1;
                    }
                }
            }

            if (fiyatMiktarListesi.Count == 0)
            {
                MessageBox.Show("Birim fiyatlar bulunamadı.");
                return -1;
            }

            // Üçüncü liste: String değerleri floata dönüştür ve ekle (float, float)
            List<Tuple<float, float>> finalList = new List<Tuple<float, float>>();
            foreach (var fiyatMiktar in fiyatMiktarListesi)
            {
                try
                {
                    float birimFiyat = float.Parse(fiyatMiktar.Item1, System.Globalization.CultureInfo.InvariantCulture);
                    float malzemeMiktar = float.Parse(fiyatMiktar.Item2, System.Globalization.CultureInfo.InvariantCulture);
                    finalList.Add(new Tuple<float, float>(birimFiyat, malzemeMiktar));
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Değerler dönüştürülürken bir hata oluştu: " + ex.Message);
                    return -1;
                }
            }

            // Toplam maliyeti hesapla
            float toplamMaliyet = 0;
            foreach (var fiyatMiktar in finalList)
            {
                toplamMaliyet += fiyatMiktar.Item1 * fiyatMiktar.Item2;
            }

            // Toplam maliyeti geri döndür
            return toplamMaliyet;
        }


        private float GetBirimFiyat(int malzemeID)
        {
            using (SqlConnection connection = new SqlConnection(conn.ConnectionString))
            {
                string query = "SELECT CAST(BirimFiyat AS FLOAT) FROM Tbl_Malzemeler WHERE MalzemeID = @MalzemeID";
                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@MalzemeID", malzemeID);

                try
                {
                    connection.Open();
                    var result = command.ExecuteScalar();
                    if (result != null)
                    {
                        return Convert.ToSingle(result); // Convert result to float
                    }
                    else
                    {
                        MessageBox.Show($"Birim fiyat bulunamadı: MalzemeID {malzemeID}");
                        return 0; // Return 0 for not found
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Birim fiyat hesaplanırken hata: {ex.Message}");
                    return 0; // Return 0 on error
                }
            }
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                int tarifID = Convert.ToInt32(dataGridView1.Rows[e.RowIndex].Cells["TarifID"].Value);
                ShowTarifDetails(tarifID);
            }
        }

        private void ShowTarifDetails(int tarifID)
        {
            using (SqlConnection connection = new SqlConnection(conn.ConnectionString))
            {
                string query = "SELECT TarifAdi, HazirlanmaSuresi FROM Tbl_Tarifler WHERE TarifID = @TarifID";
                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@TarifID", tarifID);

                try
                {
                    connection.Open();
                    SqlDataReader reader = command.ExecuteReader();

                    if (reader.Read())
                    {
                        string tarifAdi = reader["TarifAdi"].ToString();
                        string hazirlanmaSuresi = reader["HazirlanmaSuresi"].ToString();
                        float maliyet = HesaplaToplamMaliyet(tarifID);

                        MessageBox.Show($"Tarif Adı: {tarifAdi}\n" +
                                        $"Hazırlanma Süresi: {hazirlanmaSuresi} dakika\n" +
                                        $"Toplam Maliyet: {maliyet.ToString("0.00")} TL");
                    }
                    else
                    {
                        MessageBox.Show("Tarif bilgileri bulunamadı.");
                    }
                    reader.Close();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Tarif bilgileri yüklenirken hata: " + ex.Message);
                }
            }
        }
    }
}