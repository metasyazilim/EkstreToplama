using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using UglyToad.PdfPig;
using UglyToad.PdfPig.Content;

namespace EkstreToparlama
{
    public partial class Form1 : Form
    {
        // Seçilen kartýn türünü tutacak deðiþken
        private string _secilenIslemTuru = "";

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            // 1. Ýþlem Türleri Kartlarýný Oluþtur (Bootstrap tarzý)
            CreateTypeCards();

            // 2. DataGridView Baþlýk ve Genel Tasarým Ayarlarý
            SetupDataGridViewStyle(dgvLeft);
            SetupDataGridViewStyle(dgvRight);

            // Tarih filtresi ComboBox ayarlarý
            if (cmbDateFilter != null)
            {
                cmbDateFilter.DrawMode = DrawMode.OwnerDrawFixed;
                cmbDateFilter.Items.Clear();
                cmbDateFilter.Items.Add("Tüm Tarihler");
                cmbDateFilter.SelectedIndex = 0;
            }
        }

        private void SetupDataGridViewStyle(DataGridView dgv)
        {
            dgv.EnableHeadersVisualStyles = false;
            dgv.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(51, 65, 85); // Koyu Slate
            dgv.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            dgv.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            dgv.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.None;
            dgv.ColumnHeadersHeight = 40;

            dgv.BorderStyle = BorderStyle.None;
            dgv.BackgroundColor = Color.White;
            dgv.GridColor = Color.FromArgb(226, 232, 240); // Soft Gri Çizgiler
            dgv.RowHeadersVisible = false;
            dgv.RowTemplate.Height = 35; // Satýr yüksekliðini artýrdýk daha ferah oldu

            dgv.DefaultCellStyle.Font = new Font("Segoe UI", 9.5F);
            dgv.DefaultCellStyle.SelectionBackColor = Color.FromArgb(226, 232, 240);
            dgv.DefaultCellStyle.SelectionForeColor = Color.Black;
        }

        // Bootstrap mantýðýnda Card butonlarý oluþturur
        private void CreateTypeCards()
        {
            if (flpCards != null)
            {
                flpCards.Controls.Clear();
                flpCards.Padding = new Padding(5);

                // GÝRÝÞ (YEÞÝL) KARTLARI
                string[] greenCards = {
                    "POS SATIÞ",
                    "721 NOLU HESAPTAN 080 HESABA AKTARMA",
                    "077 NOLU HESAPTAN 080 HESABA AKTARMA",
                    "DÝÐER (GÝRÝÞ)"
                };

                // ÇIKIÞ (KIRMIZI) KARTLARI
                string[] redCards = {                    
                    "POS KOMÝSYON",
                    "080 NOLU HESAPTAN 721 HESABA AKTARMA",
                    "080 NOLU HESAPTAN 077 HESABA AKTARMA",
                    "POS AÝDAT",
                    "DÝÐER (ÇIKIÞ)"
                };

                Button lastGreenButton = null;

                foreach (var text in greenCards)
                {
                    // Daha soft yeþil tasarým
                    Button btn = CreateCardButton(text, Color.FromArgb(220, 252, 231), Color.FromArgb(21, 128, 61), Color.FromArgb(134, 239, 172));
                    flpCards.Controls.Add(btn);
                    lastGreenButton = btn;
                }

                // *** ÖNEMLÝ: Yeþil kartlardan sonra FlowLayout'u keserek Kýrmýzýlarý zorla alta atýyoruz ***
                if (lastGreenButton != null)
                {
                    flpCards.SetFlowBreak(lastGreenButton, true);
                }

                foreach (var text in redCards)
                {
                    // Daha soft kýrmýzý tasarým
                    Button btn = CreateCardButton(text, Color.FromArgb(254, 226, 226), Color.FromArgb(185, 28, 28), Color.FromArgb(252, 165, 165));
                    flpCards.Controls.Add(btn);
                }
            }
        }

        private Button CreateCardButton(string text, Color backColor, Color foreColor, Color borderColor)
        {
            Button btn = new Button();
            btn.Text = text;
            btn.BackColor = backColor;
            btn.ForeColor = foreColor;
            btn.FlatStyle = FlatStyle.Flat;
            btn.FlatAppearance.BorderSize = 1;
            btn.FlatAppearance.BorderColor = borderColor;
            btn.Size = new Size(220, 65);
            btn.Font = new Font("Segoe UI Semibold", 9F, FontStyle.Bold);
            btn.Cursor = Cursors.Hand;
            btn.Margin = new Padding(5);
            btn.Click += Card_Click;
            return btn;
        }

        private void Card_Click(object sender, EventArgs e)
        {
            // Önceki seçimlerin kalýnlýk ve yazý fontlarýný sýfýrla
            foreach (Control ctrl in flpCards.Controls)
            {
                if (ctrl is Button b)
                {
                    b.FlatAppearance.BorderSize = 1;
                    b.Font = new Font("Segoe UI Semibold", 9F, FontStyle.Bold);
                }
            }

            // Týklanan Card'ý kalýn çerçeve ile vurgula
            Button clickedBtn = (Button)sender;
            clickedBtn.FlatAppearance.BorderSize = 3;
            clickedBtn.Font = new Font("Segoe UI", 9.5F, FontStyle.Bold);

            _secilenIslemTuru = clickedBtn.Text;
        }

        private void btnLoadPdf_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Filter = "PDF Dosyalarý (*.pdf)|*.pdf",
                Title = "Banka Hesap Ekstresi Seçin"
            };

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                lblFileInfo.Text = "Dosya Okunuyor: " + Path.GetFileName(openFileDialog.FileName);
                Cursor = Cursors.WaitCursor;

                dgvLeft.Rows.Clear();
                dgvRight.Rows.Clear(); // Yeni dosya yüklenince sað tarafý da temizler

                if (cmbDateFilter != null)
                {
                    cmbDateFilter.Items.Clear();
                    cmbDateFilter.Items.Add("Tüm Tarihler");
                }

                try
                {
                    List<BankaIslem> islemler = ParseBankStatement(openFileDialog.FileName);
                    HashSet<string> benzersizTarihler = new HashSet<string>();

                    foreach (var islem in islemler)
                    {
                        int rowIndex = dgvLeft.Rows.Add(islem.Tarih, islem.Tutar, islem.Bakiye, islem.Aciklama);
                        dgvLeft.Rows[rowIndex].Tag = islem;

                        if (!string.IsNullOrEmpty(islem.Tarih))
                        {
                            benzersizTarihler.Add(islem.Tarih);
                        }
                    }

                    if (cmbDateFilter != null)
                    {
                        foreach (var tarih in benzersizTarihler.OrderBy(t => t))
                        {
                            cmbDateFilter.Items.Add(tarih);
                        }
                        cmbDateFilter.SelectedIndex = 0;
                    }

                    lblFileInfo.Text = $"Baþarýyla Yüklendi! Toplam {islemler.Count} iþlem bulundu. Dosya: " + Path.GetFileName(openFileDialog.FileName);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("PDF okunurken bir hata oluþtu: " + ex.Message, "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    lblFileInfo.Text = "Hata oluþtu.";
                }
                finally
                {
                    Cursor = Cursors.Default;
                }
            }
        }

        private List<BankaIslem> ParseBankStatement(string filePath)
        {
            List<BankaIslem> liste = new List<BankaIslem>();
            BankaIslem mevcutIslem = null;

            var rowRegex = new Regex(@"(\d{2}-\d{2}-\d{4})\s*[^0-9-]*\s*(-?[\d.]+,\d{2})\s*[^0-9-]*\s*(-?[\d.]+,\d{2})\s*(.*)");

            using (PdfDocument document = PdfDocument.Open(filePath))
            {
                foreach (var page in document.GetPages())
                {
                    var words = page.GetWords().ToList();
                    if (!words.Any()) continue;

                    double tolerance = 3.5;

                    var sortedWords = words.OrderByDescending(w => w.BoundingBox.Bottom)
                                           .ThenBy(w => w.BoundingBox.Left)
                                           .ToList();

                    var visualLines = new List<string>();
                    var currentLineWords = new List<Word>();

                    foreach (var word in sortedWords)
                    {
                        if (!currentLineWords.Any())
                        {
                            currentLineWords.Add(word);
                        }
                        else
                        {
                            if (Math.Abs(word.BoundingBox.Bottom - currentLineWords[0].BoundingBox.Bottom) <= tolerance)
                            {
                                currentLineWords.Add(word);
                            }
                            else
                            {
                                var lineText = string.Join(" ", currentLineWords.OrderBy(w => w.BoundingBox.Left).Select(w => w.Text));
                                visualLines.Add(lineText);
                                currentLineWords = new List<Word> { word };
                            }
                        }
                    }

                    if (currentLineWords.Any())
                    {
                        var lineText = string.Join(" ", currentLineWords.OrderBy(w => w.BoundingBox.Left).Select(w => w.Text));
                        visualLines.Add(lineText);
                    }

                    foreach (var line in visualLines)
                    {
                        string trimmedLine = line.Trim();
                        if (string.IsNullOrEmpty(trimmedLine)) continue;

                        var match = rowRegex.Match(trimmedLine);
                        if (match.Success)
                        {
                            string tarih = match.Groups[1].Value;
                            string tutarStr = match.Groups[2].Value;
                            string bakiyeStr = match.Groups[3].Value;
                            string aciklama = match.Groups[4].Value.Replace("\"", "").Replace(" , ", " ").Trim();

                            mevcutIslem = new BankaIslem
                            {
                                Tarih = tarih,
                                Tutar = tutarStr,
                                Bakiye = bakiyeStr,
                                Aciklama = aciklama
                            };
                            liste.Add(mevcutIslem);
                        }
                        else
                        {
                            if (mevcutIslem != null &&
                                !trimmedLine.Contains("HALKBANK") &&
                                !trimmedLine.Contains("HESAP ÖZETÝ") &&
                                !trimmedLine.Contains("Müþteri Bilgileriniz") &&
                                !trimmedLine.Contains("Hesap Özeti Bilgileriniz") &&
                                !trimmedLine.Contains("Sayfa No") &&
                                !trimmedLine.StartsWith(">") &&
                                !trimmedLine.Contains("Ýþlem Tarihi"))
                            {
                                mevcutIslem.Aciklama += " " + trimmedLine;
                                mevcutIslem.Aciklama = Regex.Replace(mevcutIslem.Aciklama, @"\s+", " ").Trim();
                            }
                        }
                    }
                }
            }

            return liste;
        }

        private void cmbDateFilter_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cmbDateFilter.SelectedItem == null) return;

            string selectedDate = cmbDateFilter.SelectedItem.ToString();

            CurrencyManager currencyManager = null;
            if (dgvLeft.DataSource != null)
            {
                currencyManager = (CurrencyManager)dgvLeft.BindingContext[dgvLeft.DataSource];
                currencyManager.SuspendBinding();
            }

            foreach (DataGridViewRow row in dgvLeft.Rows)
            {
                if (row.IsNewRow) continue;

                if (selectedDate == "Tüm Tarihler")
                {
                    row.Visible = true;
                }
                else
                {
                    if (row.Cells[0].Value != null && row.Cells[0].Value.ToString() == selectedDate)
                        row.Visible = true;
                    else
                        row.Visible = false;
                }
            }

            if (currencyManager != null)
            {
                currencyManager.ResumeBinding();
            }
        }

        private void btnResetFilter_Click(object sender, EventArgs e)
        {
            if (cmbDateFilter != null && cmbDateFilter.Items.Count > 0)
            {
                cmbDateFilter.SelectedIndex = 0; // "Tüm Tarihler" seçeneðine geri döndürür
            }
        }

        private void cmbDateFilter_DrawItem(object sender, DrawItemEventArgs e)
        {
            if (e.Index < 0) return;

            ComboBox combo = sender as ComboBox;
            string text = combo.Items[e.Index].ToString();

            bool hepsiSari = false;

            if (text != "Tüm Tarihler")
            {
                int totalSatir = 0;
                int aktarilanSatir = 0;

                foreach (DataGridViewRow row in dgvLeft.Rows)
                {
                    if (row.IsNewRow) continue;

                    if (row.Cells[0].Value != null && row.Cells[0].Value.ToString() == text)
                    {
                        totalSatir++;
                        if (row.Tag is BankaIslem islem && islem.Aktarildi)
                        {
                            aktarilanSatir++;
                        }
                    }
                }

                if (totalSatir > 0 && totalSatir == aktarilanSatir)
                {
                    hepsiSari = true;
                }
            }

            if (hepsiSari)
            {
                if ((e.State & DrawItemState.Selected) == DrawItemState.Selected)
                {
                    using (SolidBrush brush = new SolidBrush(Color.Gold))
                    {
                        e.Graphics.FillRectangle(brush, e.Bounds);
                    }
                }
                else
                {
                    using (SolidBrush brush = new SolidBrush(Color.FromArgb(253, 224, 71))) // Soft Sarý
                    {
                        e.Graphics.FillRectangle(brush, e.Bounds);
                    }
                }

                using (SolidBrush brush = new SolidBrush(Color.Black))
                {
                    e.Graphics.DrawString(text, e.Font, brush, e.Bounds);
                }
            }
            else
            {
                e.DrawBackground();
                using (SolidBrush brush = new SolidBrush(e.ForeColor))
                {
                    e.Graphics.DrawString(text, e.Font, brush, e.Bounds);
                }
            }

            e.DrawFocusRectangle();
        }

        private void dgvLeft_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (e.RowIndex < 0 || e.RowIndex >= dgvLeft.Rows.Count) return;

            DataGridViewRow row = dgvLeft.Rows[e.RowIndex];

            if (row.Tag is BankaIslem islem)
            {
                if (islem.Aktarildi)
                {
                    // Daha estetik ve göz yormayan bir sarý (Tailwind Yellow-100)
                    row.DefaultCellStyle.BackColor = Color.FromArgb(254, 252, 216);
                    row.DefaultCellStyle.SelectionBackColor = Color.FromArgb(253, 230, 138); // Yellow-200
                }
                else
                {
                    // Silme iþleminden sonra sarýyý kaldýrmak için
                    row.DefaultCellStyle.BackColor = Color.Empty;
                    row.DefaultCellStyle.SelectionBackColor = Color.Empty;
                }

                // Eksi bakiye/tutar renklendirmesi
                if (e.ColumnIndex == 1 && e.Value != null)
                {
                    if (e.Value.ToString().Contains("-"))
                    {
                        e.CellStyle.ForeColor = Color.FromArgb(220, 38, 38); // Modern Kýrmýzý (Red-600)
                        e.CellStyle.SelectionForeColor = Color.FromArgb(220, 38, 38);
                    }
                }
            }
        }

        private void btnTransfer_Click(object sender, EventArgs e)
        {
            if (dgvLeft.SelectedRows.Count == 0)
            {
                MessageBox.Show("Lütfen sol panelden aktarmak istediðiniz iþlemleri seçiniz.\n(Ctrl veya Shift tuþuna basýlý tutarak çoklu seçim yapabilirsiniz.)", "Seçim Yapýlmadý", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (string.IsNullOrWhiteSpace(_secilenIslemTuru))
            {
                MessageBox.Show("Lütfen alt panelden bir iþlem türü kartý (Giriþ veya Çýkýþ) seçiniz.", "Tür Seçilmedi", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string secilenTur = _secilenIslemTuru;
            decimal toplamTutar = 0;
            string islemTarihi = "";

            // Silme iþlemi (geri alma) için sol taraftan aktarýlan satýrlarý tutacaðýmýz liste
            List<DataGridViewRow> aktarilanSatirlarListesi = new List<DataGridViewRow>();

            var secilenSatirlar = dgvLeft.SelectedRows.Cast<DataGridViewRow>()
                                                     .OrderBy(r => r.Index)
                                                     .ToList();

            foreach (DataGridViewRow row in secilenSatirlar)
            {
                if (string.IsNullOrEmpty(islemTarihi) && row.Cells[0].Value != null)
                {
                    islemTarihi = row.Cells[0].Value.ToString();
                }

                if (row.Cells[1].Value != null)
                {
                    string tutarStr = row.Cells[1].Value.ToString();
                    if (decimal.TryParse(tutarStr, NumberStyles.Any, new CultureInfo("tr-TR"), out decimal tutar))
                    {
                        toplamTutar += tutar;
                    }
                }

                if (row.Tag is BankaIslem islem)
                {
                    islem.Aktarildi = true;
                }

                aktarilanSatirlarListesi.Add(row);
            }

            dgvLeft.ClearSelection();

            // Günün Son Bakiyesini Bulma Mantýðý
            string gunlukSonBakiye = "0,00";
            if (!string.IsNullOrEmpty(islemTarihi))
            {
                var gununSatirlari = dgvLeft.Rows.Cast<DataGridViewRow>()
                    .Where(r => !r.IsNewRow && r.Cells[0].Value != null && r.Cells[0].Value.ToString() == islemTarihi)
                    .ToList();

                if (gununSatirlari.Any())
                {
                    var sonIslem = gununSatirlari.Last();
                    if (sonIslem.Cells[2].Value != null)
                    {
                        gunlukSonBakiye = sonIslem.Cells[2].Value.ToString();
                    }
                }
            }

            int rightRowIdx = dgvRight.Rows.Add(islemTarihi, toplamTutar.ToString("N2", new CultureInfo("tr-TR")), secilenTur, gunlukSonBakiye);

            // Sað satýra, sol panelden gelen satýrlarýn listesini "Tag" olarak ekliyoruz
            dgvRight.Rows[rightRowIdx].Tag = aktarilanSatirlarListesi;

            bool isIncome = secilenTur.Contains("SATIÞ") || secilenTur.Contains("GÝRÝÞ") || secilenTur.StartsWith("721") || secilenTur.StartsWith("077");

            if (isIncome)
            {
                // Soft Yeþil
                dgvRight.Rows[rightRowIdx].DefaultCellStyle.BackColor = Color.FromArgb(220, 252, 231);
                dgvRight.Rows[rightRowIdx].DefaultCellStyle.SelectionBackColor = Color.FromArgb(187, 247, 208);
            }
            else
            {
                // Soft Kýrmýzý
                dgvRight.Rows[rightRowIdx].DefaultCellStyle.BackColor = Color.FromArgb(254, 226, 226);
                dgvRight.Rows[rightRowIdx].DefaultCellStyle.SelectionBackColor = Color.FromArgb(254, 202, 202);
            }

            dgvRight.ClearSelection();

            if (cmbDateFilter != null)
            {
                cmbDateFilter.Invalidate();
            }
        }

        // Sað Paneldeki veriyi DELETE tuþu ile silip, sol paneldeki sarýyý eski haline getirme iþlemi
        private void dgvRight_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Delete)
            {
                if (dgvRight.SelectedRows.Count > 0)
                {
                    // Sað taraftan seçilen satýrlarý listeye alýyoruz
                    var silinecekSatirlar = dgvRight.SelectedRows.Cast<DataGridViewRow>().ToList();

                    foreach (var rightRow in silinecekSatirlar)
                    {
                        // Bu sað satýrý oluþturan sol satýrlarýn listesi "Tag" içindeydi
                        if (rightRow.Tag is List<DataGridViewRow> leftRows)
                        {
                            foreach (var leftRow in leftRows)
                            {
                                if (leftRow.Tag is BankaIslem islem)
                                {
                                    islem.Aktarildi = false; // Aktarýldý iþaretini kaldýr
                                }
                                // Rengi sýfýrla (CellFormatting olayýnýn sarýyý boyamayý býrakmasýný saðlar)
                                leftRow.DefaultCellStyle.BackColor = Color.Empty;
                                leftRow.DefaultCellStyle.SelectionBackColor = Color.Empty;
                            }
                        }

                        // Sað panelden satýrý kaldýr
                        dgvRight.Rows.Remove(rightRow);
                    }

                    // Ekraný güncelle ki sarýlar kaybolsun
                    dgvLeft.Invalidate();
                    if (cmbDateFilter != null) cmbDateFilter.Invalidate();
                }
            }
        }
    }

    public class BankaIslem
    {
        public string Tarih { get; set; }
        public string Tutar { get; set; }
        public string Bakiye { get; set; }
        public string Aciklama { get; set; }
        public bool Aktarildi { get; set; } = false;
    }
}