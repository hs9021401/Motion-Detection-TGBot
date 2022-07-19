using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Emgu.CV;
using Emgu.CV.Structure;
using Telegram.Bot;
using Telegram.Bot.Types.InputFiles;
using Telegram.Bot.Types;
using System.Threading;

namespace EMGU_Example
{
    public partial class DetectBot : Form
    {
        private VideoCapture _capture = null;
        private Mat _frame;
        private int _detect_times = 0;
        private TG tgBot;
        public bool _IsCapturing;
        public int nCameraCount = 0;

        #region CV Image declare
        Image<Bgr, Byte> Current_Frame_RGB; //current Frame from camera (The raw image)        
        Image<Gray, Byte> Current_Frame; //current Frame from camera (gray)
        Image<Gray, Byte> Previous_Frame; //Previiousframe aquired
        Image<Gray, Byte> Difference; //Difference between the two frames        
        #endregion
        private int _framecount;
        
        Settings iniSetting;

        public DetectBot()
        {            
            InitializeComponent();
            this.Text = "Movement Detection Bot [v" + this.ProductVersion + "]";
            iniSetting = new Settings();
            loadSettingToUI();
        }

        private void DetectBot_Load(object sender, EventArgs e)
        {
            int i = 0;
            while(true)
            {
                _capture = new VideoCapture(i);
                if(_capture.IsOpened)
                {
                    cbCamera.Items.Insert(i, "Camera " + i);
                    i++;
                }
                else
                {
                    break;
                }
            }

            nCameraCount = i;

            cbCamera.SelectedIndex = 0;

            //檢查ini設定
            if (!checkIniSetting())
            {
                tabControl1.SelectedIndex = 1;                
                return;
            }

            initTGBot();
        }


        private void initTGBot()
        { 
            if(tgBot == null)
            {
                tgBot = new TG(this, iniSetting);
                tgBot.OnSelCameraChangedEvt += ChangeSelectedCameraHandler;
                tgBot.OnThresholdChangedEvt += ChangeThresholdHandler;
            }
        
        }

        public bool checkIniSetting()
        {
            StringBuilder sb = new StringBuilder();
            
            if (iniSetting.TGtoken.Equals(""))
            {
                sb.AppendLine("🚫請設定Telegram Bot的token");
            }
            if (iniSetting.SaveFolder.Equals(""))
            {
                sb.AppendLine("🚫請設定存圖路徑folder");             
            }
            if (iniSetting.TGSendToGroup.Equals("1"))
            {
                if (iniSetting.TGchatID.Equals(""))
                    sb.AppendLine("🚫您已設定發送通知到群組, 請設定群組的chatid");
            }

            if (sb.Length != 0)
            {
                MessageBox.Show(sb.ToString(), "錯誤", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
            else return true;
        }

        private void ChangeSelectedCameraHandler(int nIdx)
        {
            cbCamera.SelectedIndex = nIdx;
        }

        private void ChangeThresholdHandler(string sLower, string sUpper)
        {
            txtLowerBound.Text = sLower;
            txtUpperBound.Text = sUpper;
        }

        public void btnCapture_Click(object sender, EventArgs e)
        {
            //檢查ini設定
            if (!checkIniSetting())
            {
                tabControl1.SelectedIndex = 1;
                return;
            }

            _capture = new VideoCapture(cbCamera.SelectedIndex);

            _capture.ImageGrabbed += ProcessFrame;
            _frame = new Mat();
            _framecount = 0;

            _detect_times = 0;

            Difference = new Image<Gray, byte>(picOutput.Width, picOutput.Height);

            if (_capture != null)
                _capture.Start();

            _IsCapturing = true;
        }

        private void ProcessFrame(object sender, EventArgs e)
        {
            if(_capture!=null && _capture.Ptr != IntPtr.Zero)
            {
                _capture.Retrieve(_frame, 0);
                Bitmap bmp = Emgu.CV.BitmapExtension.ToBitmap(_frame);
                picOutput.Image = (Image)bmp.Clone();                

                if (_framecount % iniSetting.FrameCount == 0)
                {
                    Current_Frame_RGB = _frame.ToImage<Bgr, byte>();
                    Current_Frame = Current_Frame_RGB.Convert<Gray, byte>();

                    // Calculate diff
                    if (Previous_Frame != null)
                    {
                        CvInvoke.AbsDiff(Previous_Frame, Current_Frame, Difference);

                        double diff = CvInvoke.CountNonZero(Difference);
                        diff = (diff / (Current_Frame.Width * Current_Frame.Height)) * 100;
                        Debug.WriteLine($"Diff: {diff}%\r\n");

                        this.Invoke((MethodInvoker)delegate
                        {
                            lblDiff.Text = "Diff(%): " + diff.ToString("F2");
                        });

                        // Send image to the telegram if the diff value is exceeded threshold.
                        if (diff >= iniSetting.LowerBound && diff <= iniSetting.UpperBound)
                        {
                            _detect_times++;

                            this.Invoke((MethodInvoker)delegate
                            {
                                lblDetected.Text = "已偵測次數: " + _detect_times;
                            });
                            
                            string path = iniSetting.SaveFolder + "\\" + DateTime.Now.ToString("yyyy-MM-dd_HHmmssff") + ".jpg";
                            CvInvoke.Imwrite(path, Current_Frame_RGB);

                            //Send Image
                            Task<Telegram.Bot.Types.Message> task = tgBot.SendImgAsync(path);
                        }
                    }

                    Previous_Frame = Current_Frame.Copy();                  
                    _framecount = 0;
                }

                _framecount++;
            }
        }

        public void btnStop_Click(object sender, EventArgs e)
        {
            if (_capture != null)
            {
                _capture.Stop();
                _capture.Dispose();
            }

            _IsCapturing = false;
        }

        private void btnBrowserFolder_Click(object sender, EventArgs e)
        {
            folderBrowserDlg.ShowDialog();
            txtSaveFolder.Text = folderBrowserDlg.SelectedPath;
        }

        private void loadSettingToUI()
        {
            txtTGToken.Text = iniSetting.TGtoken;     
            if(iniSetting.TGSendToGroup == "1")
            {
                chkSendToGroup.Checked = true;
                txtTGChatID.Enabled = true;
            }
            else
            {
                chkSendToGroup.Checked = false;
                txtTGChatID.Enabled = false;
            }            
            txtTGChatID.Text = iniSetting.TGchatID;
            txtLowerBound.Text = iniSetting.LowerBound.ToString();
            txtUpperBound.Text = iniSetting.UpperBound.ToString();
            txtFrameCount.Text = iniSetting.FrameCount.ToString();
            txtSaveFolder.Text = iniSetting.SaveFolder;
        }

        internal void btnApplySetting_Click(object sender, EventArgs e)
        {
            if (!txtLowerBound.Text.All(char.IsDigit) || !txtUpperBound.Text.All(char.IsDigit) || !txtFrameCount.Text.All(char.IsDigit))
            {
                System.Windows.Forms.MessageBox.Show("請輸入數字", "錯誤", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);
                return;
            }

            if(chkSendToGroup.Checked)
            {
                if(txtTGChatID.Text.Equals(""))
                {
                    System.Windows.Forms.MessageBox.Show("請輸入Group Chat Id", "錯誤", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);
                    return;
                }
            }

            if (!checkIniSetting())
                return;

            iniSetting.TGtoken = txtTGToken.Text;
            iniSetting.TGSendToGroup = chkSendToGroup.Checked ? "1" : "0";
            iniSetting.TGchatID = txtTGChatID.Text;
            iniSetting.LowerBound = Int32.Parse(txtLowerBound.Text);
            iniSetting.UpperBound = Int32.Parse(txtUpperBound.Text);
            iniSetting.FrameCount = Int32.Parse(txtFrameCount.Text);
            iniSetting.SaveFolder = txtSaveFolder.Text;
            iniSetting.writeAll();
            iniSetting.readAll();   //reload ini setting

            if(sender != null)
            {
                MessageBox.Show("設置成功", "訊息", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            initTGBot();
        }

        private void btnRecoverSetting_Click(object sender, EventArgs e)
        {
            iniSetting.readAll();
            loadSettingToUI();
            MessageBox.Show("完成", "訊息", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void chkSendToGroup_CheckedChanged(object sender, EventArgs e)
        {
            CheckBox chk = (CheckBox)sender;
            if (chk.Checked)
                txtTGChatID.Enabled = true;
            else
                txtTGChatID.Enabled = false;
        }

        private void chkSendToGroup_MouseHover(object sender, EventArgs e)
        {
            toolTip1.Show("取消則表示會把通知送至您與BOT的私人對話框", chkSendToGroup);
        }

        private void linkBotCreateTutorial_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            linkBotCreateTutorial.LinkVisited = true;
            System.Diagnostics.Process.Start("https://sendpulse.com/knowledge-base/chatbot/create-telegram-chatbot");
        }
    }
}
