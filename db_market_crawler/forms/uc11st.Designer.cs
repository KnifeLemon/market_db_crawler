namespace mm_db_market.forms
{
    partial class uc11st
    {
        /// <summary> 
        /// 필수 디자이너 변수입니다.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// 사용 중인 모든 리소스를 정리합니다.
        /// </summary>
        /// <param name="disposing">관리되는 리소스를 삭제해야 하면 true이고, 그렇지 않으면 false입니다.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region 구성 요소 디자이너에서 생성한 코드

        /// <summary> 
        /// 디자이너 지원에 필요한 메서드입니다. 
        /// 이 메서드의 내용을 코드 편집기로 수정하지 마세요.
        /// </summary>
        private void InitializeComponent()
        {
            this.label1 = new System.Windows.Forms.Label();
            this.cmbMainCategory = new System.Windows.Forms.ComboBox();
            this.cmbSubCategory = new System.Windows.Forms.ComboBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.txtSearchText = new System.Windows.Forms.TextBox();
            this.btnExtractStart = new System.Windows.Forms.Button();
            this.btnExtractStop = new System.Windows.Forms.Button();
            this.btnReset = new System.Windows.Forms.Button();
            this.btnSaveList = new System.Windows.Forms.Button();
            this.lvList = new System.Windows.Forms.ListView();
            this.btnAddColumn = new System.Windows.Forms.Button();
            this.btnLoadColumn = new System.Windows.Forms.Button();
            this.lblStatus = new System.Windows.Forms.Label();
            this.cmbMinCategory = new System.Windows.Forms.ComboBox();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(8, 12);
            this.label1.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(43, 15);
            this.label1.TabIndex = 0;
            this.label1.Text = "대분류";
            // 
            // cmbMainCategory
            // 
            this.cmbMainCategory.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbMainCategory.Enabled = false;
            this.cmbMainCategory.FormattingEnabled = true;
            this.cmbMainCategory.Location = new System.Drawing.Point(52, 8);
            this.cmbMainCategory.Margin = new System.Windows.Forms.Padding(2);
            this.cmbMainCategory.Name = "cmbMainCategory";
            this.cmbMainCategory.Size = new System.Drawing.Size(102, 21);
            this.cmbMainCategory.TabIndex = 1;
            this.cmbMainCategory.SelectedIndexChanged += new System.EventHandler(this.cmbMainCategory_SelectedIndexChanged);
            // 
            // cmbSubCategory
            // 
            this.cmbSubCategory.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbSubCategory.Enabled = false;
            this.cmbSubCategory.FormattingEnabled = true;
            this.cmbSubCategory.Location = new System.Drawing.Point(217, 9);
            this.cmbSubCategory.Margin = new System.Windows.Forms.Padding(2);
            this.cmbSubCategory.Name = "cmbSubCategory";
            this.cmbSubCategory.Size = new System.Drawing.Size(102, 21);
            this.cmbSubCategory.TabIndex = 3;
            this.cmbSubCategory.SelectedIndexChanged += new System.EventHandler(this.cmbSubCategory_SelectedIndexChanged);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(173, 12);
            this.label2.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(43, 15);
            this.label2.TabIndex = 2;
            this.label2.Text = "중분류";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(516, 12);
            this.label3.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(43, 15);
            this.label3.TabIndex = 4;
            this.label3.Text = "검색어";
            // 
            // txtSearchText
            // 
            this.txtSearchText.Location = new System.Drawing.Point(556, 9);
            this.txtSearchText.Margin = new System.Windows.Forms.Padding(2);
            this.txtSearchText.Name = "txtSearchText";
            this.txtSearchText.Size = new System.Drawing.Size(143, 20);
            this.txtSearchText.TabIndex = 5;
            this.txtSearchText.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // btnExtractStart
            // 
            this.btnExtractStart.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnExtractStart.Location = new System.Drawing.Point(599, 35);
            this.btnExtractStart.Margin = new System.Windows.Forms.Padding(2);
            this.btnExtractStart.Name = "btnExtractStart";
            this.btnExtractStart.Size = new System.Drawing.Size(71, 20);
            this.btnExtractStart.TabIndex = 6;
            this.btnExtractStart.Text = "추출시작";
            this.btnExtractStart.UseVisualStyleBackColor = true;
            this.btnExtractStart.Click += new System.EventHandler(this.btnExtractStart_Click);
            // 
            // btnExtractStop
            // 
            this.btnExtractStop.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnExtractStop.Enabled = false;
            this.btnExtractStop.Location = new System.Drawing.Point(674, 35);
            this.btnExtractStop.Margin = new System.Windows.Forms.Padding(2);
            this.btnExtractStop.Name = "btnExtractStop";
            this.btnExtractStop.Size = new System.Drawing.Size(71, 20);
            this.btnExtractStop.TabIndex = 7;
            this.btnExtractStop.Text = "추출중지";
            this.btnExtractStop.UseVisualStyleBackColor = true;
            this.btnExtractStop.Click += new System.EventHandler(this.btnExtractStop_Click);
            // 
            // btnReset
            // 
            this.btnReset.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnReset.Location = new System.Drawing.Point(748, 35);
            this.btnReset.Margin = new System.Windows.Forms.Padding(2);
            this.btnReset.Name = "btnReset";
            this.btnReset.Size = new System.Drawing.Size(71, 20);
            this.btnReset.TabIndex = 8;
            this.btnReset.Text = "초기화";
            this.btnReset.UseVisualStyleBackColor = true;
            this.btnReset.Click += new System.EventHandler(this.btnReset_Click);
            // 
            // btnSaveList
            // 
            this.btnSaveList.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnSaveList.Location = new System.Drawing.Point(823, 35);
            this.btnSaveList.Margin = new System.Windows.Forms.Padding(2);
            this.btnSaveList.Name = "btnSaveList";
            this.btnSaveList.Size = new System.Drawing.Size(71, 20);
            this.btnSaveList.TabIndex = 9;
            this.btnSaveList.Text = "파일저장";
            this.btnSaveList.UseVisualStyleBackColor = true;
            this.btnSaveList.Click += new System.EventHandler(this.btnSaveList_Click);
            // 
            // lvList
            // 
            this.lvList.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lvList.FullRowSelect = true;
            this.lvList.GridLines = true;
            this.lvList.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
            this.lvList.HideSelection = false;
            this.lvList.Location = new System.Drawing.Point(8, 60);
            this.lvList.Margin = new System.Windows.Forms.Padding(2);
            this.lvList.MultiSelect = false;
            this.lvList.Name = "lvList";
            this.lvList.Size = new System.Drawing.Size(887, 449);
            this.lvList.TabIndex = 10;
            this.lvList.UseCompatibleStateImageBehavior = false;
            this.lvList.View = System.Windows.Forms.View.Details;
            // 
            // btnAddColumn
            // 
            this.btnAddColumn.Location = new System.Drawing.Point(8, 35);
            this.btnAddColumn.Margin = new System.Windows.Forms.Padding(2);
            this.btnAddColumn.Name = "btnAddColumn";
            this.btnAddColumn.Size = new System.Drawing.Size(84, 20);
            this.btnAddColumn.TabIndex = 11;
            this.btnAddColumn.Text = "열 만들기";
            this.btnAddColumn.UseVisualStyleBackColor = true;
            this.btnAddColumn.Click += new System.EventHandler(this.btnAddColumn_Click);
            // 
            // btnLoadColumn
            // 
            this.btnLoadColumn.Location = new System.Drawing.Point(96, 35);
            this.btnLoadColumn.Margin = new System.Windows.Forms.Padding(2);
            this.btnLoadColumn.Name = "btnLoadColumn";
            this.btnLoadColumn.Size = new System.Drawing.Size(84, 20);
            this.btnLoadColumn.TabIndex = 14;
            this.btnLoadColumn.Text = "열 불러오기";
            this.btnLoadColumn.UseVisualStyleBackColor = true;
            this.btnLoadColumn.Click += new System.EventHandler(this.btnLoadColumn_Click);
            // 
            // lblStatus
            // 
            this.lblStatus.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lblStatus.Location = new System.Drawing.Point(8, 511);
            this.lblStatus.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lblStatus.Name = "lblStatus";
            this.lblStatus.Size = new System.Drawing.Size(887, 21);
            this.lblStatus.TabIndex = 15;
            this.lblStatus.Text = "총 0 개 추출완료";
            this.lblStatus.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // cmbMinCategory
            // 
            this.cmbMinCategory.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbMinCategory.Enabled = false;
            this.cmbMinCategory.FormattingEnabled = true;
            this.cmbMinCategory.Location = new System.Drawing.Point(381, 8);
            this.cmbMinCategory.Margin = new System.Windows.Forms.Padding(2);
            this.cmbMinCategory.Name = "cmbMinCategory";
            this.cmbMinCategory.Size = new System.Drawing.Size(102, 21);
            this.cmbMinCategory.TabIndex = 17;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(337, 11);
            this.label4.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(43, 15);
            this.label4.TabIndex = 16;
            this.label4.Text = "소분류";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(277, 39);
            this.label5.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(345, 15);
            this.label5.TabIndex = 19;
            this.label5.Text = "* 검색어를 입력하면 카테고리는 무시하고 검색어로 추출합니다.";
            // 
            // uc11st
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.label5);
            this.Controls.Add(this.cmbMinCategory);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.lblStatus);
            this.Controls.Add(this.btnLoadColumn);
            this.Controls.Add(this.btnAddColumn);
            this.Controls.Add(this.lvList);
            this.Controls.Add(this.btnSaveList);
            this.Controls.Add(this.btnReset);
            this.Controls.Add(this.btnExtractStop);
            this.Controls.Add(this.btnExtractStart);
            this.Controls.Add(this.txtSearchText);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.cmbSubCategory);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.cmbMainCategory);
            this.Controls.Add(this.label1);
            this.Margin = new System.Windows.Forms.Padding(2);
            this.Name = "uc11st";
            this.Padding = new System.Windows.Forms.Padding(6, 7, 6, 7);
            this.Size = new System.Drawing.Size(901, 539);
            this.Load += new System.EventHandler(this.ucNShop_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox cmbMainCategory;
        private System.Windows.Forms.ComboBox cmbSubCategory;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox txtSearchText;
        private System.Windows.Forms.Button btnExtractStart;
        private System.Windows.Forms.Button btnExtractStop;
        private System.Windows.Forms.Button btnReset;
        private System.Windows.Forms.Button btnSaveList;
        private System.Windows.Forms.ListView lvList;
        private System.Windows.Forms.Button btnAddColumn;
        private System.Windows.Forms.Button btnLoadColumn;
        private System.Windows.Forms.Label lblStatus;
        private System.Windows.Forms.ComboBox cmbMinCategory;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
    }
}
