using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types.InputFiles;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Extensions.Polling;
using Telegram.Bot.Types.ReplyMarkups;
using System.Threading;
using System.Net;
using Newtonsoft.Json;
using System.IO;

namespace EMGU_Example
{
    public class TG
    {
        public delegate void OnSelCameraChanged(int nIdx);
        public event OnSelCameraChanged OnSelCameraChangedEvt;

        public delegate void OnThresholdChanged(string nLow, string nUp);
        public event OnThresholdChanged OnThresholdChangedEvt;

        private string _token, _chatId, _botprivatechatId;
        private Settings _setting;
        bool _bSentGroup;
        TelegramBotClient _botClient;
        TelegramBotClient _botClient_Listen;
        CancellationTokenSource _cts;

        DetectBot _form = null;
        

        public TG(DetectBot form, Settings setting)
        {
            _form = form;
            _setting = setting;
            _token = _setting.TGtoken;
            _chatId = _setting.TGchatID;
            _botClient = new TelegramBotClient(_token);
            _botClient_Listen = new TelegramBotClient(_token);
            _cts = new CancellationTokenSource();
            _botprivatechatId = getRobotPrivateChatId();
            _bSentGroup = _setting.TGSendToGroup.Equals("1") ? true : false;

            Handlers.msgDele = TgBot_CmdHandler;            

            var receiverOptions = new ReceiverOptions
            {
                AllowedUpdates = Array.Empty<UpdateType>() // receive all update types
            };

            _botClient_Listen.StartReceiving(updateHandler: Handlers.HandleUpdateAsync,
                  errorHandler: Handlers.HandleErrorAsync,
                  receiverOptions: receiverOptions,
                  cancellationToken: _cts.Token);
        }

        public void StopTGBotReceiving()
        {
            _cts.Cancel();
        }

        public async Task<Telegram.Bot.Types.Message> SendImgAsync(string path)
        {
            string[] timestamp = path.Split('\\');
            string targetChatId = _bSentGroup ? _chatId : _botprivatechatId;
            string caption = "[" + timestamp[timestamp.Length - 1].Split('.')[0] + "] 偵測到移動!";

            using (var stream = System.IO.File.OpenRead(path))
            {
                InputOnlineFile file = new InputOnlineFile(stream);
                Telegram.Bot.Types.Message msg = await _botClient.SendPhotoAsync(
                chatId: targetChatId,
                photo: file,
                caption: caption);
                return msg;
            }
        }

        public void robotMessage(string v)
        {
            if (_botprivatechatId.Equals(""))
                _botprivatechatId = getRobotPrivateChatId();

            _botClient.SendTextMessageAsync(
           chatId: _botprivatechatId,
           text: v,
           parseMode: ParseMode.MarkdownV2,
           disableNotification: true,
           cancellationToken: _cts.Token);            
        }

        public void robotSendMenu()
        {
            ReplyKeyboardMarkup replyKeyboardMarkup = new ReplyKeyboardMarkup(new KeyboardButton[] {
                "🖐HI", "📷DEVS" ,"🤔STATE","🔛ON", "❎OFF", "THR"
            });
            replyKeyboardMarkup.ResizeKeyboard = true;

            _botClient.SendTextMessageAsync(
               chatId: _botprivatechatId,
               text: "請按需求按下面按鈕😄",
               replyMarkup: replyKeyboardMarkup,
               disableNotification: true,
               cancellationToken: _cts.Token);
        }

        private string getRobotPrivateChatId()
        {
            string chatId = "";
            string API_URL = $"https://api.telegram.org/bot{_token}/getUpdates";
            var request = WebRequest.Create(API_URL);
            var response = (HttpWebResponse)request.GetResponse();
            var myJsonResponse = new StreamReader(response.GetResponseStream()).ReadToEnd();
            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(myJsonResponse);
                if(myDeserializedClass.result.Count > 0)
                {
                    chatId = myDeserializedClass.result[0].message.from.id.ToString();
                }
            }

            return chatId;
        }

        private void TgBot_CmdHandler(string sCmd)
        {
            if (this._form.InvokeRequired)
            {
                Action<string> bAct = new Action<string>(TgBot_CmdHandler);
                this._form.Invoke(bAct, sCmd);
            }
            else
            {
                sCmd = sCmd.ToUpper();
                if (sCmd.Contains("ON"))
                {
                    robotMessage("開始偵測");
                    this._form.btnCapture_Click(null, null);
                }
                else if (sCmd.Contains("OFF"))
                {
                    robotMessage("停止偵測");
                    this._form.btnStop_Click(null, null);
                }
                else if (sCmd.Contains("HI"))
                {
                    robotMessage("HELLO 歡迎使用");
                    robotSendMenu();
                }
                else if (sCmd.Contains("DEVS"))
                {
                    robotMessage("Camera共有: " + this._form.nCameraCount + "台, 請輸入SEL加數字\\(從0開始\\)選擇您要的裝置, 如SEL0");
                }
                else if (sCmd.Contains("STATE"))
                {
                    robotMessage("偵測中: " + (this._form._IsCapturing ? "是" : "否"));
                }
                else if (sCmd.Contains("SEL"))
                {
                    int selectCamera = Int32.Parse(sCmd.Replace("SEL", "").Trim());
                    if (selectCamera >= this._form.nCameraCount || selectCamera < 0)
                    {
                        robotMessage("❌輸入錯誤請修正");
                        return;
                    }                    
                    OnSelCameraChangedEvt(selectCamera);
                    robotMessage("切換Camera" + selectCamera + "成功");
                }
                else if (sCmd.Contains("THR"))
                {
                    string sThreshold = sCmd.Replace("THR", "");

                    if (sThreshold.Equals(""))
                    {
                        robotMessage("⁉設置偵測閾值上下限方式如右➡ THR85:100");
                        return;
                    }
                    else
                    {
                        string lower = sThreshold.Split(':')[0];
                        string upper = sThreshold.Split(':')[1];

                        int nLower, nUpper;
                        if (Int32.TryParse(lower, out nLower) && Int32.TryParse(upper, out nUpper))
                        {
                            //No need to be changed
                            if (_setting.UpperBound == nUpper && _setting.LowerBound == nLower)
                                return;

                            //txtLowerBound.Text = lower;
                            //txtUpperBound.Text = upper;

                            OnThresholdChangedEvt(lower, upper);
                            robotMessage("✅設置偵測閾值成功");
                            Thread.Sleep(2000);
                            this._form.btnApplySetting_Click(null, null);
                        }
                        else
                        {
                            robotMessage("❌輸入錯誤請修正");
                        }
                    }

                }
                else
                {
                    robotMessage("Q Q 我不懂你的明白");
                }
            }
        }
    }


    public class Handlers
    {
        public static Action<string> msgDele;
        public static string robotChatId;

        public static Task HandleErrorAsync(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
        {
            Console.WriteLine(exception.Message);
            return Task.CompletedTask;
        }

        public static async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
        {
            Task handle;
            switch (update.Type)
            {
                default:
                case UpdateType.Message:
                    handle = BotOnMessageReceived(botClient, update.Message);
                    break;
            }

            await handle;
        }

        private static Task BotOnMessageReceived(ITelegramBotClient botClient, Message message)
        {
            if (message == null)
            {
                return Task.CompletedTask;
            }

            if (message.Type == MessageType.Text)
            {
                msgDele.Invoke(message.Text);
            }

            return Task.CompletedTask;
        }
    }


    #region Transform JSON format to Class with https://json2csharp.com/
    public class Chat
    {
        public int id { get; set; }
        public string first_name { get; set; }
        public string username { get; set; }
        public string type { get; set; }
    }

    public class From
    {
        public int id { get; set; }
        public bool is_bot { get; set; }
        public string first_name { get; set; }
        public string username { get; set; }
        public string language_code { get; set; }
    }

    public class Messages
    {
        public int message_id { get; set; }
        public From from { get; set; }
        public Chat chat { get; set; }
        public int date { get; set; }
        public string text { get; set; }
    }

    public class Result
    {
        public int update_id { get; set; }
        public Messages message { get; set; }
    }

    public class Root
    {
        public bool ok { get; set; }
        public List<Result> result { get; set; }
    }
    #endregion
}
