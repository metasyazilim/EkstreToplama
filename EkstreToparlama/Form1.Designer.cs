namespace EkstreToparlama
{
    partial class Form1
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        private void InitializeComponent()
        {
            pnlTop = new Panel();
            btnResetFilter = new Button();
            cmbDateFilter = new ComboBox();
            lblDateFilterTitle = new Label();
            lblFileInfo = new Label();
            btnLoadPdf = new Button();
            pnlBottom = new Panel();
            flpCards = new FlowLayoutPanel();
            lblAuthor = new Label();
            btnTransfer = new Button();
            tlpMain = new TableLayoutPanel();
            gbLeft = new GroupBox();
            dgvLeft = new DataGridView();
            colDate = new DataGridViewTextBoxColumn();
            colAmount = new DataGridViewTextBoxColumn();
            colBalance = new DataGridViewTextBoxColumn();
            colDesc = new DataGridViewTextBoxColumn();
            gbRight = new GroupBox();
            dgvRight = new DataGridView();
            colRightDate = new DataGridViewTextBoxColumn();
            colRightTotal = new DataGridViewTextBoxColumn();
            colRightType = new DataGridViewTextBoxColumn();
            colRightBalance = new DataGridViewTextBoxColumn();
            pnlTop.SuspendLayout();
            pnlBottom.SuspendLayout();
            tlpMain.SuspendLayout();
            gbLeft.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)dgvLeft).BeginInit();
            gbRight.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)dgvRight).BeginInit();
            SuspendLayout();
            // 
            // pnlTop
            // 
            pnlTop.BackColor = Color.FromArgb(15, 23, 42); // Daha derin bir lacivert/slate
            pnlTop.Controls.Add(btnResetFilter);
            pnlTop.Controls.Add(cmbDateFilter);
            pnlTop.Controls.Add(lblDateFilterTitle);
            pnlTop.Controls.Add(lblFileInfo);
            pnlTop.Controls.Add(btnLoadPdf);
            pnlTop.Dock = DockStyle.Top;
            pnlTop.Location = new Point(0, 0);
            pnlTop.Name = "pnlTop";
            pnlTop.Size = new Size(1264, 75);
            pnlTop.TabIndex = 0;
            // 
            // btnResetFilter
            // 
            btnResetFilter.BackColor = Color.FromArgb(71, 85, 105);
            btnResetFilter.Cursor = Cursors.Hand;
            btnResetFilter.FlatAppearance.BorderSize = 0;
            btnResetFilter.FlatStyle = FlatStyle.Flat;
            btnResetFilter.Font = new Font("Segoe UI", 9.5F, FontStyle.Bold);
            btnResetFilter.ForeColor = Color.White;
            btnResetFilter.Location = new Point(1075, 20);
            btnResetFilter.Name = "btnResetFilter";
            btnResetFilter.Size = new Size(140, 35);
            btnResetFilter.TabIndex = 4;
            btnResetFilter.Text = "🔄 Tüm Tarihler";
            btnResetFilter.UseVisualStyleBackColor = false;
            btnResetFilter.Click += btnResetFilter_Click;
            // 
            // cmbDateFilter
            // 
            cmbDateFilter.DrawMode = DrawMode.OwnerDrawFixed;
            cmbDateFilter.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbDateFilter.Font = new Font("Segoe UI", 11F);
            cmbDateFilter.FormattingEnabled = true;
            cmbDateFilter.Location = new Point(880, 23);
            cmbDateFilter.Name = "cmbDateFilter";
            cmbDateFilter.Size = new Size(185, 28);
            cmbDateFilter.TabIndex = 3;
            cmbDateFilter.DrawItem += cmbDateFilter_DrawItem;
            cmbDateFilter.SelectedIndexChanged += cmbDateFilter_SelectedIndexChanged;
            // 
            // lblDateFilterTitle
            // 
            lblDateFilterTitle.AutoSize = true;
            lblDateFilterTitle.Font = new Font("Segoe UI Semibold", 10F, FontStyle.Bold);
            lblDateFilterTitle.ForeColor = Color.FromArgb(241, 245, 249);
            lblDateFilterTitle.Location = new Point(765, 27);
            lblDateFilterTitle.Name = "lblDateFilterTitle";
            lblDateFilterTitle.Size = new Size(111, 19);
            lblDateFilterTitle.TabIndex = 2;
            lblDateFilterTitle.Text = "Tarihe Göre Süz:";
            // 
            // lblFileInfo
            // 
            lblFileInfo.AutoSize = true;
            lblFileInfo.Font = new Font("Segoe UI", 10F);
            lblFileInfo.ForeColor = Color.FromArgb(148, 163, 184);
            lblFileInfo.Location = new Point(235, 28);
            lblFileInfo.Name = "lblFileInfo";
            lblFileInfo.Size = new Size(328, 19);
            lblFileInfo.TabIndex = 1;
            lblFileInfo.Text = "Lütfen işlem yapmak istediğiniz PDF ekstresini seçin.";
            // 
            // btnLoadPdf
            // 
            btnLoadPdf.BackColor = Color.FromArgb(2, 132, 199); // Sky Blue 600
            btnLoadPdf.Cursor = Cursors.Hand;
            btnLoadPdf.FlatAppearance.BorderSize = 0;
            btnLoadPdf.FlatStyle = FlatStyle.Flat;
            btnLoadPdf.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            btnLoadPdf.ForeColor = Color.White;
            btnLoadPdf.Location = new Point(15, 18);
            btnLoadPdf.Name = "btnLoadPdf";
            btnLoadPdf.Size = new Size(200, 38);
            btnLoadPdf.TabIndex = 0;
            btnLoadPdf.Text = "📂 Banka Ekstresi Yükle";
            btnLoadPdf.UseVisualStyleBackColor = false;
            btnLoadPdf.Click += btnLoadPdf_Click;
            // 
            // pnlBottom
            // 
            pnlBottom.BackColor = Color.FromArgb(248, 250, 252);
            pnlBottom.Controls.Add(flpCards);
            pnlBottom.Controls.Add(lblAuthor);
            pnlBottom.Controls.Add(btnTransfer);
            pnlBottom.Dock = DockStyle.Bottom;
            pnlBottom.Location = new Point(0, 501);
            pnlBottom.Name = "pnlBottom";
            pnlBottom.Size = new Size(1264, 180);
            pnlBottom.TabIndex = 1;
            // 
            // flpCards
            // 
            flpCards.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left;
            flpCards.AutoScroll = true;
            flpCards.Location = new Point(10, 10);
            flpCards.Name = "flpCards";
            flpCards.Size = new Size(1000, 160);
            flpCards.TabIndex = 4;
            // 
            // lblAuthor
            // 
            lblAuthor.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            lblAuthor.AutoSize = true;
            lblAuthor.Font = new Font("Segoe UI Semibold", 9F, FontStyle.Bold | FontStyle.Italic);
            lblAuthor.ForeColor = Color.FromArgb(148, 163, 184);
            lblAuthor.Location = new Point(1050, 150);
            lblAuthor.Name = "lblAuthor";
            lblAuthor.Size = new Size(196, 15);
            lblAuthor.TabIndex = 3;
            lblAuthor.Text = "Enis Kaman Tarafından Yapılmıştır";
            // 
            // btnTransfer
            // 
            btnTransfer.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            btnTransfer.BackColor = Color.FromArgb(16, 185, 129); // Emerald 500
            btnTransfer.Cursor = Cursors.Hand;
            btnTransfer.FlatAppearance.BorderSize = 0;
            btnTransfer.FlatStyle = FlatStyle.Flat;
            btnTransfer.Font = new Font("Segoe UI", 12F, FontStyle.Bold);
            btnTransfer.ForeColor = Color.White;
            btnTransfer.Location = new Point(1020, 15);
            btnTransfer.Name = "btnTransfer";
            btnTransfer.Size = new Size(230, 125);
            btnTransfer.TabIndex = 2;
            btnTransfer.Text = "➡ Seçilenleri Topla ve Sağ Panele Aktar";
            btnTransfer.UseVisualStyleBackColor = false;
            btnTransfer.Click += btnTransfer_Click;
            // 
            // tlpMain
            // 
            tlpMain.ColumnCount = 2;
            tlpMain.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            tlpMain.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            tlpMain.Controls.Add(gbLeft, 0, 0);
            tlpMain.Controls.Add(gbRight, 1, 0);
            tlpMain.Dock = DockStyle.Fill;
            tlpMain.Location = new Point(0, 75);
            tlpMain.Name = "tlpMain";
            tlpMain.RowCount = 1;
            tlpMain.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            tlpMain.Size = new Size(1264, 426);
            tlpMain.TabIndex = 2;
            // 
            // gbLeft
            // 
            gbLeft.BackColor = Color.FromArgb(241, 245, 249);
            gbLeft.Controls.Add(dgvLeft);
            gbLeft.Dock = DockStyle.Fill;
            gbLeft.Font = new Font("Segoe UI Semibold", 10F, FontStyle.Bold);
            gbLeft.ForeColor = Color.FromArgb(51, 65, 85);
            gbLeft.Location = new Point(12, 12);
            gbLeft.Margin = new Padding(12);
            gbLeft.Name = "gbLeft";
            gbLeft.Padding = new Padding(8);
            gbLeft.Size = new Size(608, 402);
            gbLeft.TabIndex = 0;
            gbLeft.TabStop = false;
            gbLeft.Text = "Sol Panel - Ekstreden Okunan Tüm İşlemler";
            // 
            // dgvLeft
            // 
            dgvLeft.AllowUserToAddRows = false;
            dgvLeft.AllowUserToDeleteRows = false;
            dgvLeft.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dgvLeft.Columns.AddRange(new DataGridViewColumn[] { colDate, colAmount, colBalance, colDesc });
            dgvLeft.Dock = DockStyle.Fill;
            dgvLeft.Location = new Point(8, 26);
            dgvLeft.Name = "dgvLeft";
            dgvLeft.ReadOnly = true;
            dgvLeft.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvLeft.Size = new Size(592, 368);
            dgvLeft.TabIndex = 0;
            dgvLeft.CellFormatting += dgvLeft_CellFormatting;
            // 
            // colDate
            // 
            colDate.HeaderText = "İşlem Tarihi";
            colDate.Name = "colDate";
            colDate.ReadOnly = true;
            colDate.Width = 110;
            // 
            // colAmount
            // 
            colAmount.HeaderText = "İşlem Tutarı";
            colAmount.Name = "colAmount";
            colAmount.ReadOnly = true;
            colAmount.Width = 120;
            // 
            // colBalance
            // 
            colBalance.HeaderText = "Bakiye";
            colBalance.Name = "colBalance";
            colBalance.ReadOnly = true;
            colBalance.Width = 120;
            // 
            // colDesc
            // 
            colDesc.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            colDesc.HeaderText = "Açıklama";
            colDesc.Name = "colDesc";
            colDesc.ReadOnly = true;
            // 
            // gbRight
            // 
            gbRight.BackColor = Color.FromArgb(241, 245, 249);
            gbRight.Controls.Add(dgvRight);
            gbRight.Dock = DockStyle.Fill;
            gbRight.Font = new Font("Segoe UI Semibold", 10F, FontStyle.Bold);
            gbRight.ForeColor = Color.FromArgb(51, 65, 85);
            gbRight.Location = new Point(644, 12);
            gbRight.Margin = new Padding(12);
            gbRight.Name = "gbRight";
            gbRight.Padding = new Padding(8);
            gbRight.Size = new Size(608, 402);
            gbRight.TabIndex = 1;
            gbRight.TabStop = false;
            gbRight.Text = "Sağ Panel - Özet Kayıt Tablosu (Satır seçip Delete ile silebilirsiniz)";
            // 
            // dgvRight
            // 
            dgvRight.AllowUserToAddRows = false;
            dgvRight.AllowUserToDeleteRows = false;
            dgvRight.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dgvRight.Columns.AddRange(new DataGridViewColumn[] { colRightDate, colRightTotal, colRightType, colRightBalance });
            dgvRight.Dock = DockStyle.Fill;
            dgvRight.Location = new Point(8, 26);
            dgvRight.Name = "dgvRight";
            dgvRight.ReadOnly = true;
            dgvRight.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvRight.Size = new Size(592, 368);
            dgvRight.TabIndex = 0;
            dgvRight.KeyDown += dgvRight_KeyDown;
            // 
            // colRightDate
            // 
            colRightDate.HeaderText = "İşlem Tarihi";
            colRightDate.Name = "colRightDate";
            colRightDate.ReadOnly = true;
            colRightDate.Width = 110;
            // 
            // colRightTotal
            // 
            colRightTotal.HeaderText = "Toplam Tutar";
            colRightTotal.Name = "colRightTotal";
            colRightTotal.ReadOnly = true;
            colRightTotal.Width = 130;
            // 
            // colRightType
            // 
            colRightType.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            colRightType.HeaderText = "Seçilen Açıklama / Tür";
            colRightType.Name = "colRightType";
            colRightType.ReadOnly = true;
            // 
            // colRightBalance
            // 
            colRightBalance.HeaderText = "Son Bakiye";
            colRightBalance.Name = "colRightBalance";
            colRightBalance.ReadOnly = true;
            colRightBalance.Width = 120;
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.FromArgb(241, 245, 249);
            ClientSize = new Size(1264, 681);
            Controls.Add(tlpMain);
            Controls.Add(pnlBottom);
            Controls.Add(pnlTop);
            MinimumSize = new Size(1024, 700);
            Name = "Form1";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Banka Hesap Hareketleri Düzenleyici Dashboard";
            WindowState = FormWindowState.Maximized;
            Load += Form1_Load;
            pnlTop.ResumeLayout(false);
            pnlTop.PerformLayout();
            pnlBottom.ResumeLayout(false);
            pnlBottom.PerformLayout();
            tlpMain.ResumeLayout(false);
            gbLeft.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)dgvLeft).EndInit();
            gbRight.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)dgvRight).EndInit();
            ResumeLayout(false);
        }

        #endregion

        private System.Windows.Forms.Panel pnlTop;
        private System.Windows.Forms.Button btnLoadPdf;
        private System.Windows.Forms.Label lblFileInfo;
        private System.Windows.Forms.Panel pnlBottom;
        private System.Windows.Forms.FlowLayoutPanel flpCards;
        private System.Windows.Forms.Button btnTransfer;
        private System.Windows.Forms.TableLayoutPanel tlpMain;
        private System.Windows.Forms.GroupBox gbLeft;
        private System.Windows.Forms.DataGridView dgvLeft;
        private System.Windows.Forms.GroupBox gbRight;
        private System.Windows.Forms.DataGridView dgvRight;
        private System.Windows.Forms.DataGridViewTextBoxColumn colDate;
        private System.Windows.Forms.DataGridViewTextBoxColumn colAmount;
        private System.Windows.Forms.DataGridViewTextBoxColumn colBalance;
        private System.Windows.Forms.DataGridViewTextBoxColumn colDesc;
        private System.Windows.Forms.DataGridViewTextBoxColumn colRightDate;
        private System.Windows.Forms.DataGridViewTextBoxColumn colRightTotal;
        private System.Windows.Forms.DataGridViewTextBoxColumn colRightType;
        private System.Windows.Forms.DataGridViewTextBoxColumn colRightBalance;
        private System.Windows.Forms.Label lblAuthor;
        private System.Windows.Forms.Label lblDateFilterTitle;
        private System.Windows.Forms.ComboBox cmbDateFilter;
        private System.Windows.Forms.Button btnResetFilter;
    }
}