namespace Targ20
{
    partial class Main
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Main));
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.txtHostIP = new System.Windows.Forms.TextBox();
            this.txtUserID = new System.Windows.Forms.TextBox();
            this.txtConsole = new System.Windows.Forms.TextBox();
            this.btnConnect = new System.Windows.Forms.Button();
            this.radio72hours = new System.Windows.Forms.RadioButton();
            this.radio48hours = new System.Windows.Forms.RadioButton();
            this.radio24hours = new System.Windows.Forms.RadioButton();
            this.radio8hours = new System.Windows.Forms.RadioButton();
            this.btnRemoveRights = new System.Windows.Forms.Button();
            this.comboAdminList = new System.Windows.Forms.ComboBox();
            this.selectUserBox = new System.Windows.Forms.PictureBox();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.pictureBox2 = new System.Windows.Forms.PictureBox();
            this.btnGrantRights = new System.Windows.Forms.Button();
            this.btnDisconnect = new System.Windows.Forms.Button();
            this.label3 = new System.Windows.Forms.Label();
            this.picBoxSettings = new System.Windows.Forms.PictureBox();
            this.chkBoxAutoCopy = new System.Windows.Forms.CheckBox();
            this.chkBoxScrollTxt = new System.Windows.Forms.CheckBox();
            this.groupBoxSettings = new System.Windows.Forms.GroupBox();
            this.pixBoxSettings1 = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.selectUserBox)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.picBoxSettings)).BeginInit();
            this.groupBoxSettings.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pixBoxSettings1)).BeginInit();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.BackColor = System.Drawing.Color.Transparent;
            this.label1.Font = new System.Drawing.Font("Verdana", 12F, System.Drawing.FontStyle.Bold);
            this.label1.ForeColor = System.Drawing.SystemColors.ControlLightLight;
            this.label1.Location = new System.Drawing.Point(16, 50);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(73, 18);
            this.label1.TabIndex = 0;
            this.label1.Text = "Host IP";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.BackColor = System.Drawing.Color.Transparent;
            this.label2.Font = new System.Drawing.Font("Verdana", 12F, System.Drawing.FontStyle.Bold);
            this.label2.ForeColor = System.Drawing.Color.White;
            this.label2.Location = new System.Drawing.Point(15, 93);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(75, 18);
            this.label2.TabIndex = 0;
            this.label2.Text = "User ID";
            this.label2.DoubleClick += new System.EventHandler(this.label2_DoubleClick);
            // 
            // txtHostIP
            // 
            this.txtHostIP.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(70)))), ((int)(((byte)(74)))), ((int)(((byte)(85)))));
            this.txtHostIP.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.txtHostIP.Font = new System.Drawing.Font("Verdana", 9F, System.Drawing.FontStyle.Bold);
            this.txtHostIP.ForeColor = System.Drawing.Color.White;
            this.txtHostIP.Location = new System.Drawing.Point(108, 51);
            this.txtHostIP.MaxLength = 15;
            this.txtHostIP.Name = "txtHostIP";
            this.txtHostIP.Size = new System.Drawing.Size(137, 15);
            this.txtHostIP.TabIndex = 2;
            this.txtHostIP.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.txtHostIP.Click += new System.EventHandler(this.txtHostIP_Click);
            this.txtHostIP.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtHostIP_KeyDown);
            // 
            // txtUserID
            // 
            this.txtUserID.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(70)))), ((int)(((byte)(74)))), ((int)(((byte)(85)))));
            this.txtUserID.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.txtUserID.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
            this.txtUserID.Font = new System.Drawing.Font("Verdana", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtUserID.ForeColor = System.Drawing.Color.White;
            this.txtUserID.Location = new System.Drawing.Point(108, 95);
            this.txtUserID.MaxLength = 15;
            this.txtUserID.Name = "txtUserID";
            this.txtUserID.Size = new System.Drawing.Size(137, 15);
            this.txtUserID.TabIndex = 3;
            this.txtUserID.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.txtUserID.Click += new System.EventHandler(this.txtUserID_Click);
            this.txtUserID.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtUserID_KeyDown);
            // 
            // txtConsole
            // 
            this.txtConsole.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(70)))), ((int)(((byte)(74)))), ((int)(((byte)(85)))));
            this.txtConsole.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.txtConsole.Cursor = System.Windows.Forms.Cursors.IBeam;
            this.txtConsole.Font = new System.Drawing.Font("Verdana", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtConsole.ForeColor = System.Drawing.Color.White;
            this.txtConsole.Location = new System.Drawing.Point(20, 141);
            this.txtConsole.Multiline = true;
            this.txtConsole.Name = "txtConsole";
            this.txtConsole.ReadOnly = true;
            this.txtConsole.Size = new System.Drawing.Size(595, 212);
            this.txtConsole.TabIndex = 4;
            this.txtConsole.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtConsole_KeyDown);
            // 
            // btnConnect
            // 
            this.btnConnect.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(70)))), ((int)(((byte)(74)))), ((int)(((byte)(85)))));
            this.btnConnect.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btnConnect.Font = new System.Drawing.Font("Verdana", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnConnect.ForeColor = System.Drawing.Color.White;
            this.btnConnect.Location = new System.Drawing.Point(256, 38);
            this.btnConnect.Name = "btnConnect";
            this.btnConnect.Size = new System.Drawing.Size(111, 40);
            this.btnConnect.TabIndex = 5;
            this.btnConnect.Text = "Connect";
            this.btnConnect.UseVisualStyleBackColor = false;
            this.btnConnect.Click += new System.EventHandler(this.btnConnect_Click);
            // 
            // radio72hours
            // 
            this.radio72hours.AutoSize = true;
            this.radio72hours.BackColor = System.Drawing.Color.Transparent;
            this.radio72hours.Font = new System.Drawing.Font("Verdana", 12F, System.Drawing.FontStyle.Bold);
            this.radio72hours.ForeColor = System.Drawing.SystemColors.ControlLightLight;
            this.radio72hours.Location = new System.Drawing.Point(506, 88);
            this.radio72hours.Name = "radio72hours";
            this.radio72hours.Size = new System.Drawing.Size(105, 22);
            this.radio72hours.TabIndex = 3;
            this.radio72hours.Text = "72 Hours";
            this.radio72hours.UseVisualStyleBackColor = false;
            this.radio72hours.CheckedChanged += new System.EventHandler(this.Radio72HoursCheckedChanged);
            // 
            // radio48hours
            // 
            this.radio48hours.AutoSize = true;
            this.radio48hours.BackColor = System.Drawing.Color.Transparent;
            this.radio48hours.Font = new System.Drawing.Font("Verdana", 12F, System.Drawing.FontStyle.Bold);
            this.radio48hours.ForeColor = System.Drawing.SystemColors.ControlLightLight;
            this.radio48hours.Location = new System.Drawing.Point(387, 88);
            this.radio48hours.Name = "radio48hours";
            this.radio48hours.Size = new System.Drawing.Size(105, 22);
            this.radio48hours.TabIndex = 2;
            this.radio48hours.Text = "48 Hours";
            this.radio48hours.UseVisualStyleBackColor = false;
            this.radio48hours.CheckedChanged += new System.EventHandler(this.Radio48HoursCheckedChanged);
            // 
            // radio24hours
            // 
            this.radio24hours.AutoSize = true;
            this.radio24hours.BackColor = System.Drawing.Color.Transparent;
            this.radio24hours.Font = new System.Drawing.Font("Verdana", 12F, System.Drawing.FontStyle.Bold);
            this.radio24hours.ForeColor = System.Drawing.SystemColors.ControlLightLight;
            this.radio24hours.Location = new System.Drawing.Point(506, 51);
            this.radio24hours.Name = "radio24hours";
            this.radio24hours.Size = new System.Drawing.Size(105, 22);
            this.radio24hours.TabIndex = 1;
            this.radio24hours.Text = "24 Hours";
            this.radio24hours.UseVisualStyleBackColor = false;
            this.radio24hours.CheckedChanged += new System.EventHandler(this.Radio24HoursCheckedChanged);
            // 
            // radio8hours
            // 
            this.radio8hours.AutoSize = true;
            this.radio8hours.BackColor = System.Drawing.Color.Transparent;
            this.radio8hours.Checked = true;
            this.radio8hours.Font = new System.Drawing.Font("Verdana", 12F, System.Drawing.FontStyle.Bold);
            this.radio8hours.ForeColor = System.Drawing.Color.White;
            this.radio8hours.Location = new System.Drawing.Point(387, 51);
            this.radio8hours.Name = "radio8hours";
            this.radio8hours.Size = new System.Drawing.Size(104, 22);
            this.radio8hours.TabIndex = 0;
            this.radio8hours.TabStop = true;
            this.radio8hours.Text = "  8 Hours";
            this.radio8hours.UseVisualStyleBackColor = false;
            this.radio8hours.CheckedChanged += new System.EventHandler(this.Radio8HoursCheckedChanged);
            // 
            // btnRemoveRights
            // 
            this.btnRemoveRights.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(70)))), ((int)(((byte)(74)))), ((int)(((byte)(85)))));
            this.btnRemoveRights.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btnRemoveRights.Font = new System.Drawing.Font("Verdana", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnRemoveRights.ForeColor = System.Drawing.Color.White;
            this.btnRemoveRights.Location = new System.Drawing.Point(256, 83);
            this.btnRemoveRights.Name = "btnRemoveRights";
            this.btnRemoveRights.Size = new System.Drawing.Size(111, 40);
            this.btnRemoveRights.TabIndex = 7;
            this.btnRemoveRights.Text = "Remove";
            this.btnRemoveRights.UseVisualStyleBackColor = false;
            this.btnRemoveRights.Click += new System.EventHandler(this.btnRemove_Click);
            // 
            // comboAdminList
            // 
            this.comboAdminList.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(70)))), ((int)(((byte)(74)))), ((int)(((byte)(85)))));
            this.comboAdminList.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboAdminList.Font = new System.Drawing.Font("Verdana", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.comboAdminList.ForeColor = System.Drawing.Color.White;
            this.comboAdminList.FormattingEnabled = true;
            this.comboAdminList.Location = new System.Drawing.Point(27, 308);
            this.comboAdminList.Name = "comboAdminList";
            this.comboAdminList.Size = new System.Drawing.Size(236, 24);
            this.comboAdminList.TabIndex = 0;
            this.comboAdminList.Visible = false;
            this.comboAdminList.SelectedIndexChanged += new System.EventHandler(this.comboAdminList_SelectedIndexChanged);
            // 
            // selectUserBox
            // 
            this.selectUserBox.BackColor = System.Drawing.Color.Transparent;
            this.selectUserBox.Image = ((System.Drawing.Image)(resources.GetObject("selectUserBox.Image")));
            this.selectUserBox.Location = new System.Drawing.Point(20, 270);
            this.selectUserBox.Name = "selectUserBox";
            this.selectUserBox.Size = new System.Drawing.Size(250, 83);
            this.selectUserBox.TabIndex = 11;
            this.selectUserBox.TabStop = false;
            this.selectUserBox.Visible = false;
            // 
            // pictureBox1
            // 
            this.pictureBox1.Image = ((System.Drawing.Image)(resources.GetObject("pictureBox1.Image")));
            this.pictureBox1.Location = new System.Drawing.Point(602, 6);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(17, 17);
            this.pictureBox1.TabIndex = 12;
            this.pictureBox1.TabStop = false;
            this.pictureBox1.Click += new System.EventHandler(this.pictureBox1_Click_1);
            this.pictureBox1.MouseLeave += new System.EventHandler(this.pictureBox1_MouseLeave);
            this.pictureBox1.MouseMove += new System.Windows.Forms.MouseEventHandler(this.pictureBox1_MouseMove);
            // 
            // pictureBox2
            // 
            this.pictureBox2.Image = ((System.Drawing.Image)(resources.GetObject("pictureBox2.Image")));
            this.pictureBox2.Location = new System.Drawing.Point(580, 6);
            this.pictureBox2.Name = "pictureBox2";
            this.pictureBox2.Size = new System.Drawing.Size(17, 17);
            this.pictureBox2.TabIndex = 13;
            this.pictureBox2.TabStop = false;
            this.pictureBox2.Click += new System.EventHandler(this.pictureBox2_Click);
            this.pictureBox2.MouseLeave += new System.EventHandler(this.pictureBox2_MouseLeave);
            this.pictureBox2.MouseMove += new System.Windows.Forms.MouseEventHandler(this.pictureBox2_MouseMove);
            // 
            // btnGrantRights
            // 
            this.btnGrantRights.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(70)))), ((int)(((byte)(74)))), ((int)(((byte)(85)))));
            this.btnGrantRights.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btnGrantRights.Font = new System.Drawing.Font("Verdana", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnGrantRights.ForeColor = System.Drawing.Color.White;
            this.btnGrantRights.Location = new System.Drawing.Point(18, 178);
            this.btnGrantRights.Name = "btnGrantRights";
            this.btnGrantRights.Size = new System.Drawing.Size(111, 40);
            this.btnGrantRights.TabIndex = 14;
            this.btnGrantRights.Text = "Grant";
            this.btnGrantRights.UseVisualStyleBackColor = false;
            this.btnGrantRights.Visible = false;
            this.btnGrantRights.Click += new System.EventHandler(this.btnGrantRights_Click);
            // 
            // btnDisconnect
            // 
            this.btnDisconnect.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(70)))), ((int)(((byte)(74)))), ((int)(((byte)(85)))));
            this.btnDisconnect.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btnDisconnect.Font = new System.Drawing.Font("Verdana", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnDisconnect.ForeColor = System.Drawing.Color.White;
            this.btnDisconnect.Location = new System.Drawing.Point(18, 224);
            this.btnDisconnect.Name = "btnDisconnect";
            this.btnDisconnect.Size = new System.Drawing.Size(111, 40);
            this.btnDisconnect.TabIndex = 15;
            this.btnDisconnect.Text = "Disconnect";
            this.btnDisconnect.UseVisualStyleBackColor = false;
            this.btnDisconnect.Visible = false;
            this.btnDisconnect.Click += new System.EventHandler(this.btnDisconnect_Click);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.BackColor = System.Drawing.Color.Transparent;
            this.label3.Font = new System.Drawing.Font("Verdana", 40F, System.Drawing.FontStyle.Bold);
            this.label3.ForeColor = System.Drawing.Color.White;
            this.label3.Location = new System.Drawing.Point(282, 216);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(0, 65);
            this.label3.TabIndex = 16;
            // 
            // picBoxSettings
            // 
            this.picBoxSettings.Image = ((System.Drawing.Image)(resources.GetObject("picBoxSettings.Image")));
            this.picBoxSettings.Location = new System.Drawing.Point(564, 304);
            this.picBoxSettings.Name = "picBoxSettings";
            this.picBoxSettings.Size = new System.Drawing.Size(48, 42);
            this.picBoxSettings.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.picBoxSettings.TabIndex = 17;
            this.picBoxSettings.TabStop = false;
            this.picBoxSettings.Click += new System.EventHandler(this.pictureBox3_Click);
            // 
            // chkBoxAutoCopy
            // 
            this.chkBoxAutoCopy.AutoSize = true;
            this.chkBoxAutoCopy.BackColor = System.Drawing.Color.Transparent;
            this.chkBoxAutoCopy.Checked = true;
            this.chkBoxAutoCopy.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkBoxAutoCopy.Font = new System.Drawing.Font("Verdana", 9F, System.Drawing.FontStyle.Bold);
            this.chkBoxAutoCopy.ForeColor = System.Drawing.Color.White;
            this.chkBoxAutoCopy.Location = new System.Drawing.Point(9, 37);
            this.chkBoxAutoCopy.Name = "chkBoxAutoCopy";
            this.chkBoxAutoCopy.Size = new System.Drawing.Size(141, 18);
            this.chkBoxAutoCopy.TabIndex = 19;
            this.chkBoxAutoCopy.Text = "Enable Auto Copy";
            this.chkBoxAutoCopy.UseVisualStyleBackColor = false;
            this.chkBoxAutoCopy.Visible = false;
            this.chkBoxAutoCopy.CheckedChanged += new System.EventHandler(this.chkBoxAutoCopy_CheckedChanged);
            // 
            // chkBoxScrollTxt
            // 
            this.chkBoxScrollTxt.AutoSize = true;
            this.chkBoxScrollTxt.BackColor = System.Drawing.Color.Transparent;
            this.chkBoxScrollTxt.Checked = true;
            this.chkBoxScrollTxt.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkBoxScrollTxt.Font = new System.Drawing.Font("Verdana", 9F, System.Drawing.FontStyle.Bold);
            this.chkBoxScrollTxt.ForeColor = System.Drawing.Color.White;
            this.chkBoxScrollTxt.Location = new System.Drawing.Point(9, 19);
            this.chkBoxScrollTxt.Name = "chkBoxScrollTxt";
            this.chkBoxScrollTxt.Size = new System.Drawing.Size(165, 18);
            this.chkBoxScrollTxt.TabIndex = 20;
            this.chkBoxScrollTxt.Text = "Enable Scrolling Text";
            this.chkBoxScrollTxt.UseVisualStyleBackColor = false;
            this.chkBoxScrollTxt.Visible = false;
            this.chkBoxScrollTxt.CheckedChanged += new System.EventHandler(this.chkBoxScrollTxt_CheckedChanged);
            // 
            // groupBoxSettings
            // 
            this.groupBoxSettings.BackColor = System.Drawing.Color.Transparent;
            this.groupBoxSettings.Controls.Add(this.pixBoxSettings1);
            this.groupBoxSettings.Controls.Add(this.chkBoxAutoCopy);
            this.groupBoxSettings.Controls.Add(this.chkBoxScrollTxt);
            this.groupBoxSettings.Font = new System.Drawing.Font("Verdana", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.groupBoxSettings.ForeColor = System.Drawing.Color.White;
            this.groupBoxSettings.Location = new System.Drawing.Point(390, 288);
            this.groupBoxSettings.Name = "groupBoxSettings";
            this.groupBoxSettings.Size = new System.Drawing.Size(228, 63);
            this.groupBoxSettings.TabIndex = 21;
            this.groupBoxSettings.TabStop = false;
            this.groupBoxSettings.Text = "TARG 2.0 Settings";
            this.groupBoxSettings.Visible = false;
            // 
            // pixBoxSettings1
            // 
            this.pixBoxSettings1.Image = ((System.Drawing.Image)(resources.GetObject("pixBoxSettings1.Image")));
            this.pixBoxSettings1.Location = new System.Drawing.Point(174, 16);
            this.pixBoxSettings1.Name = "pixBoxSettings1";
            this.pixBoxSettings1.Size = new System.Drawing.Size(48, 42);
            this.pixBoxSettings1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pixBoxSettings1.TabIndex = 23;
            this.pixBoxSettings1.TabStop = false;
            this.pixBoxSettings1.Click += new System.EventHandler(this.pixBoxSettings1_Click);
            // 
            // Main
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("$this.BackgroundImage")));
            this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.ClientSize = new System.Drawing.Size(635, 370);
            this.Controls.Add(this.groupBoxSettings);
            this.Controls.Add(this.picBoxSettings);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.btnDisconnect);
            this.Controls.Add(this.btnGrantRights);
            this.Controls.Add(this.pictureBox2);
            this.Controls.Add(this.pictureBox1);
            this.Controls.Add(this.comboAdminList);
            this.Controls.Add(this.selectUserBox);
            this.Controls.Add(this.radio8hours);
            this.Controls.Add(this.radio24hours);
            this.Controls.Add(this.radio72hours);
            this.Controls.Add(this.radio48hours);
            this.Controls.Add(this.btnRemoveRights);
            this.Controls.Add(this.btnConnect);
            this.Controls.Add(this.txtConsole);
            this.Controls.Add(this.txtUserID);
            this.Controls.Add(this.txtHostIP);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.Name = "Main";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "TARG 2.0";
            this.TransparencyKey = System.Drawing.SystemColors.Control;
            this.Load += new System.EventHandler(this.Main_Load);
            this.MouseDown += new System.Windows.Forms.MouseEventHandler(this.Main_MouseDown);
            ((System.ComponentModel.ISupportInitialize)(this.selectUserBox)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.picBoxSettings)).EndInit();
            this.groupBoxSettings.ResumeLayout(false);
            this.groupBoxSettings.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pixBoxSettings1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox txtHostIP;
        private System.Windows.Forms.TextBox txtUserID;
        private System.Windows.Forms.TextBox txtConsole;
        private System.Windows.Forms.Button btnConnect;
        private System.Windows.Forms.RadioButton radio72hours;
        private System.Windows.Forms.RadioButton radio48hours;
        private System.Windows.Forms.RadioButton radio24hours;
        private System.Windows.Forms.RadioButton radio8hours;
        private System.Windows.Forms.Button btnRemoveRights;
        private System.Windows.Forms.ComboBox comboAdminList;
        private System.Windows.Forms.PictureBox selectUserBox;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.PictureBox pictureBox2;
        private System.Windows.Forms.Button btnGrantRights;
        private System.Windows.Forms.Button btnDisconnect;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.PictureBox picBoxSettings;
        private System.Windows.Forms.CheckBox chkBoxAutoCopy;
        private System.Windows.Forms.CheckBox chkBoxScrollTxt;
        private System.Windows.Forms.GroupBox groupBoxSettings;
        private System.Windows.Forms.PictureBox pixBoxSettings1;
    }
}

