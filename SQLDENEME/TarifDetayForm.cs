using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace SQLDENEME
{
    public partial class TarifDetayForm : Form
    {
        public TarifDetayForm(string tarifAdi, string hazirlanmaSuresi, float maliyet, string resimYolu)
        {
            InitializeComponent();
            label1.Text = "Tarif Adı: " + tarifAdi;
            label2.Text = "Hazırlanma Süresi: " + hazirlanmaSuresi + " dakika";
            label3.Text = "Toplam Maliyet: " + maliyet.ToString("0.00") + " TL";

            // Resim yolunu kontrol etmek için debug çıktısı
            resimYolu = resimYolu.Trim();
            MessageBox.Show($"Resim Yolu: {resimYolu}\nDosya Var mı?: {File.Exists(resimYolu)}"); // Yol ve kontrol sonucu

            // Resmi yükle
            if (!string.IsNullOrEmpty(resimYolu) && File.Exists(resimYolu))
            {
                try
                {
                    pictureBox1.Image = Image.FromFile(resimYolu);
                    pictureBox1.SizeMode = PictureBoxSizeMode.StretchImage;
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Resim yüklenirken bir hata oluştu: " + ex.Message);
                }
            }
            else
            {
                label4.Text = "Resim bulunamadı veya geçersiz dosya yolu.";
            }
        }

        private void TarifDetayForm_Load(object sender, EventArgs e)
        {

        }

        private void TarifDetayForm_Load_1(object sender, EventArgs e)
        {

        }
    }
}
