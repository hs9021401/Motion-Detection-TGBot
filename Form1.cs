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
using Emgu.CV.Util;
using Newtonsoft.Json.Linq;
using Emgu.CV.CvEnum;
using System.Security.Cryptography;
using System.Xml.Linq;

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
        private int _frameArea = 0;

        #region CV Image declare
        Image<Bgr, Byte> Current_Frame_RGB; //current Frame from camera (The raw image)        
        Image<Bgr, Byte> Current_Frame_RGB_Processed; //current Frame from camera (The raw image)        

        Image<Gray, Byte> Gray_Frame; //current Frame from camera (gray)
        Image<Bgr, Byte> Previous_Frame; //Previiousframe aquired
        Image<Bgr, Byte> Difference; //Difference between the two frames        
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
            if (tgBot != null)
            {
                tgBot.StopTGBotReceiving();
                tgBot = null;
            }    

            tgBot = new TG(this, iniSetting);
            tgBot.OnSelCameraChangedEvt += ChangeSelectedCameraHandler;
            tgBot.OnThresholdChangedEvt += ChangeThresholdHandler;
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

        private void ChangeThresholdHandler(string sLower)
        {
            txtLowerBound.Text = sLower;
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
            _frameArea = picOutput.Width * picOutput.Height;

            _detect_times = 0;

            Difference = new Image<Bgr, byte>(picOutput.Width, picOutput.Height);

            if (_capture != null)
                _capture.Start();

            _IsCapturing = true;
        }

        /// <summary>
        /// 使用Canny邊緣檢測演算法, 依照觸發數值, 於原圖像框出符合觸發條件的矩形, 並傳送圖片給Telegram Bot
        /// </summary>
        /// <param name="GrayImage">經差異+灰階+二值處理的圖像</param>
        /// <param name="DrawImage">原圖像</param>
        private void DrawBoundingBoxAndAlert(Image<Gray, byte> GrayImage, Image<Bgr, byte> DrawImage)
        {
            Image<Gray, byte> CannyImage = GrayImage.Clone();
            CvInvoke.Canny(GrayImage, CannyImage, 255, 255, 5, true);

            using (VectorOfVectorOfPoint contours = new VectorOfVectorOfPoint())
            {
                CvInvoke.FindContours(CannyImage, contours, null, RetrType.External, ChainApproxMethod.ChainApproxSimple);

                int count = contours.Size;
                int maxContIdx = -1;
                double maxContArea = 0;
                
                //Find maximum area
                for (int i = 0; i < count; i++)
                {
                    using (VectorOfPoint contour = contours[i])
                    {
                        double tmpArea = CvInvoke.ContourArea(contour);

                        if (tmpArea < iniSetting.TriggerBound)
                            continue;

                        if (tmpArea > maxContArea)
                        {
                            maxContIdx = i;
                            maxContArea = tmpArea;
                        }
                    }
                }

                //Draw rectangle
                if(contours.Size != 0 && maxContIdx != -1)
                {
                    Rectangle BoundingBox = CvInvoke.BoundingRectangle(contours[maxContIdx]);
                    CvInvoke.Rectangle(DrawImage, BoundingBox, new MCvScalar(255, 0, 255, 255), 3);
                    
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
        }

        /// <summary>
        /// Callback function for processing each incoming frame.
        /// </summary>
        private void ProcessFrame(object sender, EventArgs e)
        {
            if(_capture!=null && _capture.Ptr != IntPtr.Zero)
            {
                _capture.Retrieve(_frame, 0);

                //原圖轉Image格式
                Current_Frame_RGB = _frame.ToImage<Bgr, byte>();

                Current_Frame_RGB_Processed = Current_Frame_RGB.Copy();

                //濾波
                CvInvoke.GaussianBlur(Current_Frame_RGB, Current_Frame_RGB_Processed, new Size(3, 3), 3);
                CvInvoke.Blur(Current_Frame_RGB_Processed, Current_Frame_RGB_Processed, new Size(3, 3), new Point(-1, -1));

                if (_framecount % iniSetting.FrameCount == 0)
                {
                    if (Previous_Frame != null)
                    {
                        //差異
                        CvInvoke.AbsDiff(Previous_Frame, Current_Frame_RGB_Processed, Difference);

                        //灰化
                        Gray_Frame = Difference.Convert<Gray, byte>();

                        //二值化
                        CvInvoke.Threshold(Gray_Frame, Gray_Frame, 30, 255, ThresholdType.BinaryInv);

                        //使用型態轉換函數去除雜訊:
                        //MorphOp.Open為"開"運算, 進行先腐蝕後膨脹的操作, 消除物體外的雜訊
                        //MorphOp.Close為"閉"運算, 進行先膨脹後腐蝕的操作, 填充物體內的黑洞
                        Mat element = CvInvoke.GetStructuringElement(Emgu.CV.CvEnum.ElementShape.Cross, new Size(5, 5), new Point(-1, -1));
                        CvInvoke.MorphologyEx(Gray_Frame, Gray_Frame, Emgu.CV.CvEnum.MorphOp.Open, element,  new Point(-1, -1), 2, Emgu.CV.CvEnum.BorderType.Default, new MCvScalar(0, 0, 0));
                        CvInvoke.MorphologyEx(Gray_Frame, Gray_Frame, Emgu.CV.CvEnum.MorphOp.Close, element, new Point(-1, -1), 2, Emgu.CV.CvEnum.BorderType.Default, new MCvScalar(0, 0, 0));


                        //偵測邊緣,畫框,送訊息
                        DrawBoundingBoxAndAlert(Gray_Frame, Current_Frame_RGB);
                    }

                    Previous_Frame = Current_Frame_RGB_Processed.Copy();
                    _framecount = 0;
                }

                picOutput.Image = Current_Frame_RGB.ToBitmap();
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
            txtLowerBound.Text = iniSetting.TriggerBound.ToString();            
            txtFrameCount.Text = iniSetting.FrameCount.ToString();
            txtSaveFolder.Text = iniSetting.SaveFolder;
        }

        internal void btnApplySetting_Click(object sender, EventArgs e)
        {
            if (!txtLowerBound.Text.All(char.IsDigit) || !txtFrameCount.Text.All(char.IsDigit))
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

            iniSetting.TGtoken = txtTGToken.Text;
            iniSetting.TGSendToGroup = chkSendToGroup.Checked ? "1" : "0";
            iniSetting.TGchatID = txtTGChatID.Text;
            iniSetting.TriggerBound = Int32.Parse(txtLowerBound.Text);
            iniSetting.FrameCount = Int32.Parse(txtFrameCount.Text);
            iniSetting.SaveFolder = txtSaveFolder.Text;

            if (!checkIniSetting())
                return;

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
