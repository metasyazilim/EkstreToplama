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
        // Aktif olarak üzerinde çalýþýlan JSON dosyasýnýn tam yolu
        private string _mevcutJsonYolu = "";
        // ComboBox tetiklenme döngülerini engellemek için kontrol flag'i
        private bool _isBindingCombo = false;

        public Form1()
        {
            InitializeComponent();

            if (System.IO.File.Exists("Copilot_20260714_160227.ico"))
            {
                this.Icon = new System.Drawing.Icon("Copilot_20260714_160227.ico");
            }
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

            // Mevcut yerel JSON kayýtlarýný tara ve ComboBox'ý doldur
            RefreshSavedRecordsCombo();
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
                    Button btn = CreateCardButton(text, Color.FromArgb(220, 252, 231), Color.FromArgb(21, 128, 61), Color.FromArgb(134, 239, 172));
                    flpCards.Controls.Add(btn);
                    lastGreenButton = btn;
                }

                // *** Yeþil kartlardan sonra FlowLayout'u keserek Kýrmýzýlarý zorla alta atýyoruz ***
                if (lastGreenButton != null)
                {
                    flpCards.SetFlowBreak(lastGreenButton, true);
                }

                foreach (var text in redCards)
                {
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
            foreach (Control ctrl in flpCards.Controls)
            {
                if (ctrl is Button b)
                {
                    b.FlatAppearance.BorderSize = 1;
                    b.Font = new Font("Segoe UI Semibold", 9F, FontStyle.Bold);
                }
            }

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
                // Kullanýcýdan Kayýt Dosya Ýsmini Sor
                string dosyaAdiInput = PromptDialog.Show("Kayýt için dosya ismi giriniz (Örn: Haziran 2026):", "Yeni Kayýt Oluþtur");
                if (string.IsNullOrWhiteSpace(dosyaAdiInput))
                {
                    dosyaAdiInput = "Ekstre_" + DateTime.Now.ToString("yyyyMMdd_HHmmss");
                }

                // Geçersiz karakterleri temizle ve boþluklarý alt tire yap
                string temizDosyaAdi = string.Join("_", dosyaAdiInput.Split(Path.GetInvalidFileNameChars())).Replace(" ", "_");

                string klasorYolu = Path.Combine(Application.StartupPath, "Kayitlar");
                if (!Directory.Exists(klasorYolu))
                {
                    Directory.CreateDirectory(klasorYolu);
                }

                _mevcutJsonYolu = Path.Combine(klasorYolu, temizDosyaAdi + ".json");

                lblFileInfo.Text = "Dosya Okunuyor: " + Path.GetFileName(openFileDialog.FileName);
                Cursor = Cursors.WaitCursor;

                dgvLeft.Rows.Clear();
                dgvRight.Rows.Clear();

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

                    lblFileInfo.Text = $"Kayýt: {temizDosyaAdi}.json | Toplam {islemler.Count} iþlem yüklendi.";

                    // Ýlk haliyle JSON dosyasýna otomatik kaydet
                    AutoSave();
                    RefreshSavedRecordsCombo();

                    // Kaydedilen veriyi combobox üzerinde seçili göster
                    _isBindingCombo = true;
                    cmbSavedRecords.SelectedItem = temizDosyaAdi + ".json";
                    _isBindingCombo = false;
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
                                Id = Guid.NewGuid().ToString(), // Eþleþtirme için benzersiz ID
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
                cmbDateFilter.SelectedIndex = 0;
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
                    using (SolidBrush brush = new SolidBrush(Color.FromArgb(253, 224, 71)))
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
                    row.DefaultCellStyle.BackColor = Color.FromArgb(254, 252, 216);
                    row.DefaultCellStyle.SelectionBackColor = Color.FromArgb(253, 230, 138);
                }
                else
                {
                    row.DefaultCellStyle.BackColor = Color.Empty;
                    row.DefaultCellStyle.SelectionBackColor = Color.Empty;
                }

                if (e.ColumnIndex == 1 && e.Value != null)
                {
                    if (e.Value.ToString().Contains("-"))
                    {
                        e.CellStyle.ForeColor = Color.FromArgb(220, 38, 38);
                        e.CellStyle.SelectionForeColor = Color.FromArgb(220, 38, 38);
                    }
                }
            }
        }

        private void btnTransfer_Click(object sender, EventArgs e)
        {
            if (dgvLeft.SelectedRows.Count == 0)
            {
                MessageBox.Show("Lütfen sol panelden aktarmak istediðiniz iþlemleri seçiniz.", "Seçim Yapýlmadý", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (string.IsNullOrWhiteSpace(_secilenIslemTuru))
            {
                MessageBox.Show("Lütfen alt panelden bir iþlem türü kartý seçiniz.", "Tür Seçilmedi", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string secilenTur = _secilenIslemTuru;
            decimal toplamTutar = 0;
            string islemTarihi = "";

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
            dgvRight.Rows[rightRowIdx].Tag = aktarilanSatirlarListesi;

            bool isIncome = secilenTur.Contains("SATIÞ") || secilenTur.Contains("GÝRÝÞ") || secilenTur.StartsWith("721") || secilenTur.StartsWith("077");

            if (isIncome)
            {
                dgvRight.Rows[rightRowIdx].DefaultCellStyle.BackColor = Color.FromArgb(220, 252, 231);
                dgvRight.Rows[rightRowIdx].DefaultCellStyle.SelectionBackColor = Color.FromArgb(187, 247, 208);
            }
            else
            {
                dgvRight.Rows[rightRowIdx].DefaultCellStyle.BackColor = Color.FromArgb(254, 226, 226);
                dgvRight.Rows[rightRowIdx].DefaultCellStyle.SelectionBackColor = Color.FromArgb(254, 202, 202);
            }

            dgvRight.ClearSelection();

            if (cmbDateFilter != null)
            {
                cmbDateFilter.Invalidate();
            }

            // [OTOMATÝK KAYIT TETÝKLEME] VE [BÝLGÝLENDÝRME YAZISI]
            AutoSave();
            string suAnkiSaat = DateTime.Now.ToString("HH:mm:ss");
            lblStatusLog.Text = $"Saat {suAnkiSaat} te {islemTarihi} {toplamTutar.ToString("N2", new CultureInfo("tr-TR"))} ({secilenTur}) eklendi";
        }

        private void dgvRight_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Delete)
            {
                if (dgvRight.SelectedRows.Count > 0)
                {
                    var silinecekSatirlar = dgvRight.SelectedRows.Cast<DataGridViewRow>().ToList();

                    foreach (var rightRow in silinecekSatirlar)
                    {
                        if (rightRow.Tag is List<DataGridViewRow> leftRows)
                        {
                            foreach (var leftRow in leftRows)
                            {
                                if (leftRow.Tag is BankaIslem islem)
                                {
                                    islem.Aktarildi = false;
                                }
                                leftRow.DefaultCellStyle.BackColor = Color.Empty;
                                leftRow.DefaultCellStyle.SelectionBackColor = Color.Empty;
                            }
                        }
                        dgvRight.Rows.Remove(rightRow);
                    }

                    dgvLeft.Invalidate();
                    if (cmbDateFilter != null) cmbDateFilter.Invalidate();

                    // [OTOMATÝK KAYIT TETÝKLEME ENTEGRASYONU]
                    AutoSave();
                    string suAnkiSaat = DateTime.Now.ToString("HH:mm:ss");
                    lblStatusLog.Text = $"Saat {suAnkiSaat} te seçilen toplam verisi silindi, sol panel geri alýndý.";
                }
            }
        }

        // Klasördeki mevcut *.json uzantýlý kayýtlarý tarar ve üst ComboBox'a doldurur
        private void RefreshSavedRecordsCombo()
        {
            if (cmbSavedRecords == null) return;

            _isBindingCombo = true;
            cmbSavedRecords.Items.Clear();

            string klasorYolu = Path.Combine(Application.StartupPath, "Kayitlar");
            if (Directory.Exists(klasorYolu))
            {
                string[] dosyalar = Directory.GetFiles(klasorYolu, "*.json");
                foreach (var dosya in dosyalar)
                {
                    cmbSavedRecords.Items.Add(Path.GetFileName(dosya));
                }
            }
            _isBindingCombo = false;
        }

        private void cmbSavedRecords_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (_isBindingCombo || cmbSavedRecords.SelectedItem == null) return;

            string secilenDosya = cmbSavedRecords.SelectedItem.ToString();
            string tamYol = Path.Combine(Application.StartupPath, "Kayitlar", secilenDosya);

            if (!File.Exists(tamYol)) return;

            _mevcutJsonYolu = tamYol;

            try
            {
                string jsonIcerik = File.ReadAllText(tamYol);

                dgvLeft.Rows.Clear();
                dgvRight.Rows.Clear();
                if (cmbDateFilter != null) cmbDateFilter.Items.Clear();
                if (cmbDateFilter != null) cmbDateFilter.Items.Add("Tüm Tarihler");

                HashSet<string> benzersizTarihler = new HashSet<string>();
                Dictionary<string, DataGridViewRow> solSatirLookup = new Dictionary<string, DataGridViewRow>();

                // Sol Panel Verilerini Regex ile Çek ve Oluþtur
                var solRegex = new Regex(@"\{""Id"":""(?<id>[^""]*)"",""Tarih"":""(?<tarih>[^""]*)"",""Tutar"":""(?<tutar>[^""]*)"",""Bakiye"":""(?<bakiye>[^""]*)"",""Aciklama"":""(?<aciklama>.*?)"",""Aktarildi"":(?<aktarildi>true|false)\}");
                var solEslenmeler = solRegex.Matches(jsonIcerik);

                foreach (Match m in solEslenmeler)
                {
                    BankaIslem islem = new BankaIslem
                    {
                        Id = m.Groups["id"].Value,
                        Tarih = m.Groups["tarih"].Value,
                        Tutar = m.Groups["tutar"].Value,
                        Bakiye = m.Groups["bakiye"].Value,
                        Aciklama = m.Groups["aciklama"].Value,
                        Aktarildi = bool.Parse(m.Groups["aktarildi"].Value)
                    };

                    int rIdx = dgvLeft.Rows.Add(islem.Tarih, islem.Tutar, islem.Bakiye, islem.Aciklama);
                    dgvLeft.Rows[rIdx].Tag = islem;
                    solSatirLookup[islem.Id] = dgvLeft.Rows[rIdx];

                    if (!string.IsNullOrEmpty(islem.Tarih))
                    {
                        benzersizTarihler.Add(islem.Tarih);
                    }
                }

                if (cmbDateFilter != null)
                {
                    foreach (var t in benzersizTarihler.OrderBy(x => x))
                    {
                        cmbDateFilter.Items.Add(t);
                    }
                    cmbDateFilter.SelectedIndex = 0;
                }

                // Sað Panel Özet Kayýtlarýný Çek ve Tag Nesnelerini Yeniden Baðla
                var sagRegex = new Regex(@"\{""Tarih"":""(?<tarih>[^""]*)"",""ToplamTutar"":""(?<toplamTutar>[^""]*)"",""Tur"":""(?<tur>[^""]*)"",""SonBakiye"":""(?<sonBakiye>[^""]*)"",""IlgiliIslemIdleri"":\[(?<idler>[^\]]*)\]\}");
                var sagEslenmeler = sagRegex.Matches(jsonIcerik);

                foreach (Match m in sagEslenmeler)
                {
                    string tarih = m.Groups["tarih"].Value;
                    string toplamTutar = m.Groups["toplamTutar"].Value;
                    string tur = m.Groups["tur"].Value;
                    string sonBakiye = m.Groups["sonBakiye"].Value;
                    string idlerKumesi = m.Groups["idler"].Value;

                    List<DataGridViewRow> iliskiliSolSatirlar = new List<DataGridViewRow>();
                    if (!string.IsNullOrWhiteSpace(idlerKumesi))
                    {
                        var idDizisi = idlerKumesi.Replace("\"", "").Split(',');
                        foreach (var id in idDizisi)
                        {
                            string temizId = id.Trim();
                            if (solSatirLookup.ContainsKey(temizId))
                            {
                                iliskiliSolSatirlar.Add(solSatirLookup[temizId]);
                            }
                        }
                    }

                    int rightRowIdx = dgvRight.Rows.Add(tarih, toplamTutar, tur, sonBakiye);
                    dgvRight.Rows[rightRowIdx].Tag = iliskiliSolSatirlar;

                    bool isIncome = tur.Contains("SATIÞ") || tur.Contains("GÝRÝÞ") || tur.Contains("721") || tur.Contains("077");
                    if (isIncome)
                    {
                        dgvRight.Rows[rightRowIdx].DefaultCellStyle.BackColor = Color.FromArgb(220, 252, 231);
                        dgvRight.Rows[rightRowIdx].DefaultCellStyle.SelectionBackColor = Color.FromArgb(187, 247, 208);
                    }
                    else
                    {
                        dgvRight.Rows[rightRowIdx].DefaultCellStyle.BackColor = Color.FromArgb(254, 226, 226);
                        dgvRight.Rows[rightRowIdx].DefaultCellStyle.SelectionBackColor = Color.FromArgb(254, 202, 202);
                    }
                }

                lblFileInfo.Text = "Kayýt Baþarýyla Yüklendi: " + secilenDosya;
                string suAnkiSaat = DateTime.Now.ToString("HH:mm:ss");
                lblStatusLog.Text = $"Saat {suAnkiSaat} te {secilenDosya} çalýþmasý geri yüklendi.";
            }
            catch (Exception ex)
            {
                MessageBox.Show("Kayýt dosyasý okunurken hata oluþtu: " + ex.Message, "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Herhangi bir harici kütüphaneye baðýmlý olmadan güvenli JSON üreten AutoSave yapýsý
        private void AutoSave()
        {
            if (string.IsNullOrEmpty(_mevcutJsonYolu)) return;

            try
            {
                List<string> solJsonSatirlari = new List<string>();
                foreach (DataGridViewRow row in dgvLeft.Rows)
                {
                    if (row.IsNewRow) continue;
                    if (row.Tag is BankaIslem islem)
                    {
                        solJsonSatirlari.Add($"    {{\"Id\":\"{islem.Id}\",\"Tarih\":\"{EscapeJson(islem.Tarih)}\",\"Tutar\":\"{EscapeJson(islem.Tutar)}\",\"Bakiye\":\"{EscapeJson(islem.Bakiye)}\",\"Aciklama\":\"{EscapeJson(islem.Aciklama)}\",\"Aktarildi\":{(islem.Aktarildi ? "true" : "false")}}}");
                    }
                }

                List<string> sagJsonSatirlari = new List<string>();
                foreach (DataGridViewRow row in dgvRight.Rows)
                {
                    if (row.IsNewRow) continue;

                    string tarih = row.Cells[0].Value?.ToString() ?? "";
                    string toplamTutar = row.Cells[1].Value?.ToString() ?? "";
                    string tur = row.Cells[2].Value?.ToString() ?? "";
                    string sonBakiye = row.Cells[3].Value?.ToString() ?? "";

                    List<string> idListesi = new List<string>();
                    if (row.Tag is List<DataGridViewRow> leftRows)
                    {
                        foreach (var lr in leftRows)
                        {
                            if (lr.Tag is BankaIslem li) idListesi.Add($"\"{li.Id}\"");
                        }
                    }

                    string idlerDizesi = string.Join(",", idListesi);
                    sagJsonSatirlari.Add($"    {{\"Tarih\":\"{EscapeJson(tarih)}\",\"ToplamTutar\":\"{EscapeJson(toplamTutar)}\",\"Tur\":\"{EscapeJson(tur)}\",\"SonBakiye\":\"{EscapeJson(sonBakiye)}\",\"IlgiliIslemIdleri\":[{idlerDizesi}]}}");
                }

                string tamJson = "{\n  \"SolIslemler\": [\n" + string.Join(",\n", solJsonSatirlari) + "\n  ],\n  \"SagIslemler\": [\n" + string.Join(",\n", sagJsonSatirlari) + "\n  ]\n}";
                File.WriteAllText(_mevcutJsonYolu, tamJson);
            }
            catch
            {
                // Arka plan otomatik kaydetme hatasý durumunda akýþ kesilmez
            }
        }

        private string EscapeJson(string s)
        {
            if (string.IsNullOrEmpty(s)) return "";
            return s.Replace("\\", "\\\\").Replace("\"", "\\\"").Replace("\n", "\\n").Replace("\r", "\\r");
        }
    }

    public class BankaIslem
    {
        public string Id { get; set; } = Guid.NewGuid().ToString(); // Kayýtlarý eþleþtirmek için kritik alan
        public string Tarih { get; set; }
        public string Tutar { get; set; }
        public string Bakiye { get; set; }
        public string Aciklama { get; set; }
        public bool Aktarildi { get; set; } = false;
    }

    // InputBox yerine geçecek modern ve þýk diyalog sýnýfý
    public static class PromptDialog
    {
        public static string Show(string text, string caption)
        {
            Form prompt = new Form()
            {
                Width = 420,
                Height = 180,
                FormBorderStyle = FormBorderStyle.FixedDialog,
                Text = caption,
                StartPosition = FormStartPosition.CenterScreen,
                BackColor = Color.FromArgb(15, 23, 42),
                ForeColor = Color.White
            };
            Label textLabel = new Label() { Left = 25, Top = 20, Text = text, Width = 360, Font = new Font("Segoe UI", 10F), ForeColor = Color.FromArgb(241, 245, 249) };
            TextBox textBox = new TextBox() { Left = 25, Top = 55, Width = 350, Font = new Font("Segoe UI", 11F) };
            Button confirmation = new Button() { Text = "Kaydet", Left = 275, Width = 100, Top = 95, Height = 32, DialogResult = DialogResult.OK, FlatStyle = FlatStyle.Flat, BackColor = Color.FromArgb(16, 185, 129), ForeColor = Color.White };
            confirmation.FlatAppearance.BorderSize = 0;
            prompt.Controls.Add(textBox);
            prompt.Controls.Add(confirmation);
            prompt.Controls.Add(textLabel);
            prompt.AcceptButton = confirmation;
            return prompt.ShowDialog() == DialogResult.OK ? textBox.Text : "";
        }
    }
}