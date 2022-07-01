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

        Image<Bgr, Byte> Current_Frame_RGB; //current Frame from camera (The raw image)

        #region Grayscale Image
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

        private void btnCapture_Click(object sender, EventArgs e)
        {
            _capture = new VideoCapture(0);
            _capture.ImageGrabbed += ProcessFrame;
            _frame = new Mat();
            _framecount = 0;

            Difference = new Image<Gray, byte>(picOutput.Width, picOutput.Height);

            if (_capture != null)
                _capture.Start();
        }

        private void ProcessFrame(object sender, EventArgs e)
        {
            if(_capture!=null && _capture.Ptr != IntPtr.Zero)
            {
                _capture.Retrieve(_frame, 0);
                Bitmap bmp = Emgu.CV.BitmapExtension.ToBitmap(_frame);
                picOutput.Image = bmp;                

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

                        // Send image to the telegram if the diff value is exceeded threshold.
                        if (diff >= iniSetting.LowerBound && diff <= iniSetting.UpperBound)
                        {
                            this.Invoke((MethodInvoker)delegate
                            {
                                lblDetected.Text = "狀態: 偵測到移動";
                            });
                            
                            string path = iniSetting.SaveFolder + "\\" + DateTime.Now.ToString("yyyy-MM-dd_HHmmssff") + ".jpg";
                            CvInvoke.Imwrite(path, Current_Frame_RGB);
                            SendImage(path);
                        }
                    }

                    Previous_Frame = Current_Frame.Copy();                  
                    _framecount = 0;
                }

                this.Invoke((MethodInvoker)delegate
                {
                    lblDetected.Text = "狀態:無移動";
                });

                _framecount++;
            }
        }

        public void SendImage(string filepath)
        {
            Task<Telegram.Bot.Types.Message> task = SendImgAsync(filepath, iniSetting.TGtoken, iniSetting.TGchatID);
            //Message msg = task.Result;            
        }

        public async Task<Telegram.Bot.Types.Message> SendImgAsync(string path, string token, string chatID)
        {
            var botClient = new TelegramBotClient(token);
            string[] timestamp = path.Split('\\');

            string caption = "[" + timestamp[timestamp.Length - 1].Split('.')[0] + "] 偵測到移動!";
            using (var stream = System.IO.File.OpenRead(path))
            {
                InputOnlineFile file = new InputOnlineFile(stream);
                Telegram.Bot.Types.Message msg = await botClient.SendPhotoAsync(
                chatId: chatID,
                photo: file,
                caption: caption);
                return msg;
            }
        }

        private void btnStop_Click(object sender, EventArgs e)
        {
            if (_capture != null)
                _capture.Stop();
        }

        private void btnBrowserFolder_Click(object sender, EventArgs e)
        {
            folderBrowserDlg.ShowDialog();
            txtSaveFolder.Text = folderBrowserDlg.SelectedPath;
        }

        private void loadSettingToUI()
        {
            txtTGToken.Text = iniSetting.TGtoken;
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
            iniSetting.TGchatID = txtTGChatID.Text;
            iniSetting.LowerBound = Int32.Parse(txtLowerBound.Text);
            iniSetting.UpperBound = Int32.Parse(txtUpperBound.Text);
            iniSetting.FrameCount = Int32.Parse(txtFrameCount.Text);
            iniSetting.SaveFolder = txtSaveFolder.Text;
            iniSetting.writeAll();
            MessageBox.Show("完成", "訊息", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void btnRecoverSetting_Click(object sender, EventArgs e)
        {
            iniSetting.readAll();
            loadSettingToUI();
            MessageBox.Show("完成", "訊息", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
    }
}
