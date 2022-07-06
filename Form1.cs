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

namespace EMGU_Example
{
    public partial class DetectBot : Form
    {
        private VideoCapture _capture = null;
        private Mat _frame;
        private int _detect_times = 0;
        private TG tgBot;
        private bool _IsCapturing;

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
            cbCamera.SelectedIndex = 0;

            tgBot = new TG(iniSetting.TGtoken, iniSetting.TGchatID, iniSetting.TGSendToGroup);
            tgBot.OnCmdRecvEvt += TgBot_CmdHandler;
        }

        private void TgBot_CmdHandler(string sCmd)
        {            
            if(this.InvokeRequired)
            {
                Action<string> bAct = new Action<string>(TgBot_CmdHandler);
                this.Invoke(bAct, sCmd);
            }
            else
            {
                if (sCmd.Contains("ON"))
                {
                    tgBot.robotMessage("開始偵測");
                    btnCapture_Click(null, null);
                }
                else if (sCmd.Contains("OFF"))
                {
                    tgBot.robotMessage("停止偵測");
                    btnStop_Click(null, null);
                }
                else if (sCmd.Contains("HI"))
                {
                    tgBot.robotMessage("HELLO 歡迎使用");
                    tgBot.robotSendMenu();
                }
                else if (sCmd.Contains("DEVS"))
                {                           
                    tgBot.robotMessage("Camera共有: " + cbCamera.Items.Count.ToString() + "台, 請輸入SEL加數字\\(從0開始\\)選擇您要的裝置");
                }
                else if (sCmd.Contains("STATE"))
                {
                    tgBot.robotMessage("偵測中: " + (_IsCapturing ? "是" : "否"));
                }
                else if (sCmd.Contains("SEL"))
                {
                    int selectCamera = Int32.Parse(sCmd.Replace("SEL", "").Trim());
                    cbCamera.SelectedIndex = selectCamera;
                    tgBot.robotMessage("切換Camera" + selectCamera + "成功");
                }
            }
        }

        private void btnCapture_Click(object sender, EventArgs e)
        {
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

                        /*
                        Mat diff_binary = new Mat();
                        CvInvoke.Threshold(Difference, diff_binary, 10, 255, Emgu.CV.CvEnum.ThresholdType.Binary);
                        double diff = CvInvoke.CountNonZero(diff_binary);
                        diff = (diff / (Current_Frame.Width * Current_Frame.Height)) * 100;
                        */
                    
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

        private void btnStop_Click(object sender, EventArgs e)
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

        private void btnApplySetting_Click(object sender, EventArgs e)
        {
            if (!txtLowerBound.Text.All(char.IsDigit) || !txtUpperBound.Text.All(char.IsDigit) || !txtFrameCount.Text.All(char.IsDigit))
            {
                System.Windows.Forms.MessageBox.Show("請輸入數字", "錯誤", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);
                return;
            }

            iniSetting.TGtoken = txtTGToken.Text;
            iniSetting.TGSendToGroup = chkSendToGroup.Checked ? "1" : "0";
            iniSetting.TGchatID = txtTGChatID.Text;
            iniSetting.LowerBound = Int32.Parse(txtLowerBound.Text);
            iniSetting.UpperBound = Int32.Parse(txtUpperBound.Text);
            iniSetting.FrameCount = Int32.Parse(txtFrameCount.Text);
            iniSetting.SaveFolder = txtSaveFolder.Text;
            iniSetting.writeAll();
            MessageBox.Show("完成, 程式重啟", "訊息", MessageBoxButtons.OK, MessageBoxIcon.Information);
            Application.Restart();
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
            toolTip1.Show("取消表示將會把通知送至您與BOT的私人對話框", chkSendToGroup);
        }
    }
}
