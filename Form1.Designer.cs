
namespace EMGU_Example
{
    partial class DetectBot
    {
        /// <summary>
        /// 設計工具所需的變數。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 清除任何使用中的資源。
        /// </summary>
        /// <param name="disposing">如果應該處置受控資源則為 true，否則為 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form 設計工具產生的程式碼

        /// <summary>
        /// 此為設計工具支援所需的方法 - 請勿使用程式碼編輯器修改
        /// 這個方法的內容。
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(DetectBot));
            this.picOutput = new System.Windows.Forms.PictureBox();
            this.btnCapture = new System.Windows.Forms.Button();
            this.btnStop = new System.Windows.Forms.Button();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.lblDiff = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.cbCamera = new System.Windows.Forms.ComboBox();
            this.lblDetected = new System.Windows.Forms.Label();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.btnRecoverSetting = new System.Windows.Forms.Button();
            this.btnApplySetting = new System.Windows.Forms.Button();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.label7 = new System.Windows.Forms.Label();
            this.btnBrowserFolder = new System.Windows.Forms.Button();
            this.label6 = new System.Windows.Forms.Label();
            this.txtSaveFolder = new System.Windows.Forms.TextBox();
            this.txtFrameCount = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.txtUpperBound = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.txtLowerBound = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.txtTGChatID = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.txtTGToken = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.folderBrowserDlg = new System.Windows.Forms.FolderBrowserDialog();
            this.chkSendToGroup = new System.Windows.Forms.CheckBox();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            ((System.ComponentModel.ISupportInitialize)(this.picOutput)).BeginInit();
            this.tabControl1.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.tabPage2.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // picOutput
            // 
            this.picOutput.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.picOutput.Location = new System.Drawing.Point(0, 0);
            this.picOutput.Name = "picOutput";
            this.picOutput.Size = new System.Drawing.Size(640, 480);
            this.picOutput.TabIndex = 0;
            this.picOutput.TabStop = false;
            // 
            // btnCapture
            // 
            this.btnCapture.Location = new System.Drawing.Point(471, 493);
            this.btnCapture.Name = "btnCapture";
            this.btnCapture.Size = new System.Drawing.Size(75, 23);
            this.btnCapture.TabIndex = 1;
            this.btnCapture.Text = "開始";
            this.btnCapture.UseVisualStyleBackColor = true;
            this.btnCapture.Click += new System.EventHandler(this.btnCapture_Click);
            // 
            // btnStop
            // 
            this.btnStop.Location = new System.Drawing.Point(552, 493);
            this.btnStop.Name = "btnStop";
            this.btnStop.Size = new System.Drawing.Size(75, 23);
            this.btnStop.TabIndex = 2;
            this.btnStop.Text = "停止";
            this.btnStop.UseVisualStyleBackColor = true;
            this.btnStop.Click += new System.EventHandler(this.btnStop_Click);
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabPage1);
            this.tabControl1.Controls.Add(this.tabPage2);
            this.tabControl1.Location = new System.Drawing.Point(12, 12);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(648, 548);
            this.tabControl1.TabIndex = 3;
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.lblDiff);
            this.tabPage1.Controls.Add(this.label8);
            this.tabPage1.Controls.Add(this.cbCamera);
            this.tabPage1.Controls.Add(this.lblDetected);
            this.tabPage1.Controls.Add(this.picOutput);
            this.tabPage1.Controls.Add(this.btnStop);
            this.tabPage1.Controls.Add(this.btnCapture);
            this.tabPage1.Location = new System.Drawing.Point(4, 22);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(640, 522);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "即時影像";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // lblDiff
            // 
            this.lblDiff.AutoSize = true;
            this.lblDiff.Location = new System.Drawing.Point(144, 498);
            this.lblDiff.Name = "lblDiff";
            this.lblDiff.Size = new System.Drawing.Size(47, 12);
            this.lblDiff.TabIndex = 6;
            this.lblDiff.Text = "Diff(%): ";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(263, 498);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(49, 12);
            this.label8.TabIndex = 5;
            this.label8.Text = "Webcam:";
            // 
            // cbCamera
            // 
            this.cbCamera.FormattingEnabled = true;
            this.cbCamera.Location = new System.Drawing.Point(318, 493);
            this.cbCamera.Name = "cbCamera";
            this.cbCamera.Size = new System.Drawing.Size(121, 20);
            this.cbCamera.TabIndex = 4;
            // 
            // lblDetected
            // 
            this.lblDetected.AutoSize = true;
            this.lblDetected.Location = new System.Drawing.Point(17, 498);
            this.lblDetected.Name = "lblDetected";
            this.lblDetected.Size = new System.Drawing.Size(71, 12);
            this.lblDetected.TabIndex = 3;
            this.lblDetected.Text = "已偵測次數: ";
            // 
            // tabPage2
            // 
            this.tabPage2.Controls.Add(this.btnRecoverSetting);
            this.tabPage2.Controls.Add(this.btnApplySetting);
            this.tabPage2.Controls.Add(this.groupBox2);
            this.tabPage2.Controls.Add(this.groupBox1);
            this.tabPage2.Location = new System.Drawing.Point(4, 22);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage2.Size = new System.Drawing.Size(640, 522);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "參數";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // btnRecoverSetting
            // 
            this.btnRecoverSetting.Location = new System.Drawing.Point(478, 493);
            this.btnRecoverSetting.Name = "btnRecoverSetting";
            this.btnRecoverSetting.Size = new System.Drawing.Size(75, 23);
            this.btnRecoverSetting.TabIndex = 3;
            this.btnRecoverSetting.Text = "還原設定";
            this.btnRecoverSetting.UseVisualStyleBackColor = true;
            this.btnRecoverSetting.Click += new System.EventHandler(this.btnRecoverSetting_Click);
            // 
            // btnApplySetting
            // 
            this.btnApplySetting.Location = new System.Drawing.Point(559, 493);
            this.btnApplySetting.Name = "btnApplySetting";
            this.btnApplySetting.Size = new System.Drawing.Size(75, 23);
            this.btnApplySetting.TabIndex = 2;
            this.btnApplySetting.Text = "應用設定";
            this.btnApplySetting.UseVisualStyleBackColor = true;
            this.btnApplySetting.Click += new System.EventHandler(this.btnApplySetting_Click);
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.label7);
            this.groupBox2.Controls.Add(this.btnBrowserFolder);
            this.groupBox2.Controls.Add(this.label6);
            this.groupBox2.Controls.Add(this.txtSaveFolder);
            this.groupBox2.Controls.Add(this.txtFrameCount);
            this.groupBox2.Controls.Add(this.label5);
            this.groupBox2.Controls.Add(this.txtUpperBound);
            this.groupBox2.Controls.Add(this.label4);
            this.groupBox2.Controls.Add(this.txtLowerBound);
            this.groupBox2.Controls.Add(this.label3);
            this.groupBox2.Location = new System.Drawing.Point(6, 149);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(628, 130);
            this.groupBox2.TabIndex = 1;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "影像相關";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(271, 29);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(55, 12);
            this.label7.TabIndex = 9;
            this.label7.Text = "( 0 ~ 100 )";
            // 
            // btnBrowserFolder
            // 
            this.btnBrowserFolder.Location = new System.Drawing.Point(597, 92);
            this.btnBrowserFolder.Name = "btnBrowserFolder";
            this.btnBrowserFolder.Size = new System.Drawing.Size(25, 23);
            this.btnBrowserFolder.TabIndex = 8;
            this.btnBrowserFolder.Text = "...";
            this.btnBrowserFolder.UseVisualStyleBackColor = true;
            this.btnBrowserFolder.Click += new System.EventHandler(this.btnBrowserFolder_Click);
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(46, 102);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(56, 12);
            this.label6.TabIndex = 7;
            this.label6.Text = "存圖路徑:";
            // 
            // txtSaveFolder
            // 
            this.txtSaveFolder.Location = new System.Drawing.Point(111, 92);
            this.txtSaveFolder.Name = "txtSaveFolder";
            this.txtSaveFolder.Size = new System.Drawing.Size(480, 22);
            this.txtSaveFolder.TabIndex = 6;
            // 
            // txtFrameCount
            // 
            this.txtFrameCount.Location = new System.Drawing.Point(111, 54);
            this.txtFrameCount.Name = "txtFrameCount";
            this.txtFrameCount.Size = new System.Drawing.Size(48, 22);
            this.txtFrameCount.TabIndex = 5;
            this.txtFrameCount.Text = "30";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(22, 64);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(80, 12);
            this.label5.TabIndex = 4;
            this.label5.Text = "偵測間隔幀數:";
            // 
            // txtUpperBound
            // 
            this.txtUpperBound.Location = new System.Drawing.Point(222, 21);
            this.txtUpperBound.Name = "txtUpperBound";
            this.txtUpperBound.Size = new System.Drawing.Size(43, 22);
            this.txtUpperBound.TabIndex = 3;
            this.txtUpperBound.Text = "100";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(170, 29);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(47, 12);
            this.label4.TabIndex = 2;
            this.label4.Text = "≦ 值 ≦";
            // 
            // txtLowerBound
            // 
            this.txtLowerBound.Location = new System.Drawing.Point(111, 21);
            this.txtLowerBound.Name = "txtLowerBound";
            this.txtLowerBound.Size = new System.Drawing.Size(48, 22);
            this.txtLowerBound.TabIndex = 1;
            this.txtLowerBound.Text = "80";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(34, 29);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(68, 12);
            this.label3.TabIndex = 0;
            this.label3.Text = "觸發上下限:";
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.chkSendToGroup);
            this.groupBox1.Controls.Add(this.txtTGChatID);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.txtTGToken);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Location = new System.Drawing.Point(6, 6);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(628, 127);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Telegram Bot設定";
            // 
            // txtTGChatID
            // 
            this.txtTGChatID.Location = new System.Drawing.Point(138, 86);
            this.txtTGChatID.Name = "txtTGChatID";
            this.txtTGChatID.Size = new System.Drawing.Size(467, 22);
            this.txtTGChatID.TabIndex = 3;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(44, 96);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(78, 12);
            this.label2.TabIndex = 2;
            this.label2.Text = "Group Chat ID:";
            // 
            // txtTGToken
            // 
            this.txtTGToken.Location = new System.Drawing.Point(111, 27);
            this.txtTGToken.Name = "txtTGToken";
            this.txtTGToken.Size = new System.Drawing.Size(494, 22);
            this.txtTGToken.TabIndex = 1;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(17, 37);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(58, 12);
            this.label1.TabIndex = 0;
            this.label1.Text = "Bot Token:";
            // 
            // chkSendToGroup
            // 
            this.chkSendToGroup.AutoSize = true;
            this.chkSendToGroup.Location = new System.Drawing.Point(19, 65);
            this.chkSendToGroup.Name = "chkSendToGroup";
            this.chkSendToGroup.Size = new System.Drawing.Size(167, 16);
            this.chkSendToGroup.TabIndex = 4;
            this.chkSendToGroup.Text = "偵測通知發至Group聊天室?";
            this.chkSendToGroup.UseVisualStyleBackColor = true;
            this.chkSendToGroup.CheckedChanged += new System.EventHandler(this.chkSendToGroup_CheckedChanged);
            this.chkSendToGroup.MouseHover += new System.EventHandler(this.chkSendToGroup_MouseHover);
            // 
            // toolTip1
            // 
            this.toolTip1.ToolTipIcon = System.Windows.Forms.ToolTipIcon.Info;
            // 
            // DetectBot
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.Control;
            this.ClientSize = new System.Drawing.Size(668, 563);
            this.Controls.Add(this.tabControl1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "DetectBot";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Movement Detection Bot";
            this.Load += new System.EventHandler(this.DetectBot_Load);
            ((System.ComponentModel.ISupportInitialize)(this.picOutput)).EndInit();
            this.tabControl1.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.tabPage1.PerformLayout();
            this.tabPage2.ResumeLayout(false);
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.PictureBox picOutput;
        private System.Windows.Forms.Button btnCapture;
        private System.Windows.Forms.Button btnStop;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.TabPage tabPage2;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.TextBox txtTGChatID;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox txtTGToken;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox txtUpperBound;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox txtLowerBound;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox txtFrameCount;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Button btnBrowserFolder;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.TextBox txtSaveFolder;
        private System.Windows.Forms.Button btnRecoverSetting;
        private System.Windows.Forms.Button btnApplySetting;
        private System.Windows.Forms.FolderBrowserDialog folderBrowserDlg;
        private System.Windows.Forms.Label lblDetected;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.ComboBox cbCamera;
        private System.Windows.Forms.Label lblDiff;
        private System.Windows.Forms.CheckBox chkSendToGroup;
        private System.Windows.Forms.ToolTip toolTip1;
    }
}

