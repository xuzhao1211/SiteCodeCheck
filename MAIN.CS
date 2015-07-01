using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Threading;
using LzSoft.SysXmlIO;

namespace LzSoft.SysService
{
    public delegate void InvokeMethod(object obj);
   
    public partial class main : Form
    {
        #region 域成员字段
        private object objlock = new object();
        private System.Threading.Timer _taskTimer;
        private System.Collections.Queue _posQueue;
        private System.Collections.Queue _pageQueue;

        //当前检测数量统计
        private int _siteCount = 0;
        private int _checkSiteCount = 0;

        //当前正在处理站点
        private string _cur_Name = string.Empty;
        private string _cur_url = string.Empty;
        private string _cur_pid = string.Empty;

        //运行状态
        private string _prostatus = "已停止";
        //运行类型
        private string _taskType = string.Empty;
        
        /// <summary>
        /// 上一次检查的无效站点
        /// </summary>
        private int _errorsiteCount = 0;

        ///启动时间
        private DateTime _startTime;

        //task线程
        Thread thread = null;
        #endregion

        #region　构造
        /// <summary>
        /// 构造
        /// </summary>
        public main()
        {
            InitializeComponent();
            this.SizeChanged += new EventHandler(main_SizeChanged);
            this.notifyIcon1.DoubleClick += new EventHandler(notifyIcon1_DoubleClick);

            _posQueue = new System.Collections.Queue();
            _pageQueue = new System.Collections.Queue();
            _taskTimer = new System.Threading.Timer(new TimerCallback(CheckTime));
            _taskTimer.Change(0, 1000);

            IList<Log> logs = Log.GetLogs();
            if (logs.Count != 0)
            {
                this.label20.Text = logs[logs.Count - 1].EndTime.ToString();
                this.label22.Text = logs[logs.Count - 1].ErrorSiteCount.ToString();
                this.label24.Text = logs[logs.Count - 1].CheckSiteCount.ToString();
            }
        }
        #endregion

        #region 最小化到任务栏中
        void notifyIcon1_DoubleClick(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Normal;
            this.notifyIcon1.Visible = true;
            this.ShowInTaskbar = true;
        }

        void main_SizeChanged(object sender, EventArgs e)
        {
            if (this.WindowState == FormWindowState.Minimized)
            {
                this.ShowInTaskbar = false;
                this.notifyIcon1.Visible = true;
            }
        }
        #endregion

        #region 终止线程
        private void DisponseThread()
        {
            WriteLog(); 
            
            Setting set = Setting.GetSetting();
            set.ClickStatus = "3";
            Setting.Save(set);

            this._taskType = string.Empty;
            this._cur_pid = string.Empty;
            this._cur_url = string.Empty;
            this._cur_Name = string.Empty;
            this._siteCount = 0;
            this._checkSiteCount = 0;
            this._prostatus = "已停止";
            this._errorsiteCount = 0;
            thread.Abort();
        }
        #endregion
        
        #region 创建线程
        private void CreateThread()
        {
            InitProcc();
            thread = new Thread(new ThreadStart(Run));
            thread.IsBackground = true;
            thread.Start();
            _prostatus = "运行中";
            _startTime = System.DateTime.Now;
            this.button1.Invoke(new InvokeMethod(WriteButText), "停止");
            Setting set = Setting.GetSetting();
            set.ClickStatus = "2";
            Setting.Save(set);
        }
        #endregion

        #region 运行任务
        /// <summary>
        /// 运行任务
        /// </summary>
        private void Run()
        {
            while (true)
            {
                lock (objlock)
                {
                    string status = string.Empty;

                    ///队列中无数据
                    if (_posQueue.Count == 0)
                    {
                        FullPosq();
                    }
                    if (_posQueue.Count != 0)
                    {
                        object[] os = (object[])_posQueue.Dequeue();
                        _cur_pid = os[0].ToString();
                        _cur_Name = os[2].ToString();
                        _cur_url = os[3].ToString();

                        status = ReaderSite(_cur_pid);
                        this._checkSiteCount++;
                        if (status != "1")
                        {
                            this._errorsiteCount++;
                            Common.Provider.InsertErrorPosition(_cur_pid,LzSoft.SysService.Common.RandomHepler.GetSystemID());
                        }
                        _cur_pid = string.Empty;
                        _cur_Name = string.Empty;
                        _cur_url = string.Empty;
                    }
                    else
                    {
                        //if (_taskType == "网站命令")
                        //{
                            
                        //}
                        Setting set = Setting.GetSetting();
                        set.ClickStatus = "3";
                        Setting.Save(set);

                        WriteLog();
                        this._taskType = string.Empty;
                        this.button1.Invoke(new InvokeMethod(WriteButText), "开始");
                        this._cur_pid = string.Empty;
                        this._cur_url = string.Empty;
                        this._cur_Name = string.Empty;
                        this._siteCount = 0;
                        this._checkSiteCount = 0;
                        this._errorsiteCount = 0;
                        this._prostatus = "已停止";
                        Thread.CurrentThread.Abort();
                    }
                }
                Thread.Sleep(1000);
            }
        }
        #endregion

        #region 写入处理日志
        //写入处理日志
        private void WriteLog()
        {
            try
            {
                this.label20.Invoke(new InvokeMethod(WriteLastCheckTime), _startTime);
                this.label22.Invoke(new InvokeMethod(WriteLastErrorCount), _errorsiteCount);
                this.label24.Invoke(new InvokeMethod(WriteLastCheckCount), _checkSiteCount);

                int LogId = 0;
                Setting Set = Setting.GetSetting();
                Set.LogCount = Set.LogCount + 1;
                LogId = Set.LogCount;
                Setting.Save(Set);
                List<Log> list = Log.GetLogs();
                Log log = new Log();
                log.LogID = LogId.ToString();
                log.CheckTime = _startTime;
                log.EndTime = System.DateTime.Now;
                log.SiteCount = this._siteCount;
                log.CheckSiteCount = this._checkSiteCount;
                log.NoCheckSiteCount = this._siteCount - this._checkSiteCount;
                log.ErrorSiteCount = this._errorsiteCount;
                list.Add(log);
                Log.SaveLogs(list);
            }
            catch { }
        }
        #endregion

        #region 初始化数据
        /// <summary>
        /// 初始化数据
        /// </summary>
        public void InitProcc()
        {
            FullPageq();
        }
        #endregion

        #region 填充队列
        public void FullPageq()
        {
            int re = 0;
            DataTable tbl = Common.Provider.GetPositionList(10, 1, out re);
            int pageCount = (int)Math.Ceiling((double)re / (double)10);
            this._siteCount = re;
            for (int i = 1; i <= pageCount; i++)
            {
                this._pageQueue.Enqueue(i);
            }
            //for (int j = 0; j < pageCount - 1; j++)
            //{
            //    this._pageQueue.Dequeue();
            //}
        }
        public void FullPosq()
        {
            if (_pageQueue.Count != 0)
            {
                int r = 0;
                object p = _pageQueue.Dequeue();
                DataTable tbl = Common.Provider.GetPositionList(10, Convert.ToInt32(p), out r);
                if (tbl != null && tbl.Rows.Count > 0)
                {
                    for (int i = 0; i < tbl.Rows.Count; i++)
                    {
                        object[] obj = new object[]{tbl.Rows[i][0],
                                tbl.Rows[i][1],
                                tbl.Rows[i][2],
                                tbl.Rows[i][3],
                                tbl.Rows[i][4],
                                tbl.Rows[i][5],
                                tbl.Rows[i][6],
                                tbl.Rows[i][7],
                                tbl.Rows[i][8],
                                tbl.Rows[i][9],
                                tbl.Rows[i][10],
                                tbl.Rows[i][11],
                                tbl.Rows[i][12],
                                tbl.Rows[i][13],
                                tbl.Rows[i][14],
                                tbl.Rows[i][15],
                                tbl.Rows[i][16],
                                tbl.Rows[i][17]};

                        this._posQueue.Enqueue(obj);
                    }
                }
            }
        }
        #endregion

        #region 读取广告位状态
        /// <summary>
        /// 读取广告位状态
        /// </summary>
        /// <param name="positionid"></param>
        /// <returns>返回 0 代表已被删除广告位 返回 1 代表正常投放中  返回 2 代表代码已撤消  返回 3 代表广告位url错误</returns>
        public string ReaderSite(string positionid)
        {
            DataTable tbl = Common.Provider.GetPositionById(positionid);
            if (tbl != null && tbl.Rows.Count > 0)
            {
                string url = tbl.Rows[0]["C_CodeUrl"].ToString().Trim().ToLower();
                if (url != string.Empty)
                {
                    if (url.IndexOf("http://") == -1)
                        url = "http://" + url;

                    string pageContent = Get(url);

                    if (pageContent.IndexOf("http://xuxiaozhao.sinaapp.com/") != -1) return "1";
                    if (pageContent.IndexOf("http://xuxiaozhao.sinaapp.com/") != -1) return "1";

                    if (pageContent.IndexOf("http://xuxiaozhao.sinaapp.com/") != -1) return "1";
                    if (pageContent.IndexOf("http://xuxiaozhao.sinaapp.com/") != -1) return "1";

                    return "2";
                }
                else { return "3"; }
            }
            else { return "0"; }
        }
        #endregion

        #region 读取页面内容
        /// <summary>
        /// 获页面内容
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public string Get(string url)
        {
            try
            {
                string revalue = "";
                System.Net.HttpWebRequest request = 
                    (System.Net.HttpWebRequest)System.Net.HttpWebRequest.Create(url);
                request.Method = "GET";
                request.ContentType = "application/x-www-form-urlencoded";
                request.Timeout = 10000;
                System.Net.WebResponse response = request.GetResponse();
                System.IO.StreamReader sr = new System.IO.StreamReader(
                    response.GetResponseStream(), 
                    System.Text.Encoding.Default);
                revalue = sr.ReadToEnd();
                sr.Close();
                return revalue;
            }
            catch (Exception ex)
            {
                return string.Empty;
            }
        }
        #endregion

        #region 显示数据在UI中
        /// <summary>
        /// 写入UI
        /// </summary>
        /// <param name="obj"></param>
        private void WriteUi(object obj)
        {
            this.label14.Text = obj.ToString();
        }
        /// <summary>
        /// 写入UI时间
        /// </summary>
        /// <param name="obj"></param>
        private void WriteCurTime(object obj)
        {
            //this.textBox2.Text = obj.ToString();
            this.label15.Text = obj.ToString();
        }
        /// <summary>
        /// 写入任务执行时间
        /// </summary>
        /// <param name="obj"></param>
        private void WriteButText(object text)
        {
            this.button1.Text = text.ToString();
        }
        private void WriteTaskTime(object obj)
        {
            this.label18.Text = obj.ToString();
        }
        private void WriteCurUrl(object text)
        {
            this.label12.Text = text.ToString();
        }
        private void WriteCurName(object text)
        {
            this.label10.Text = text.ToString();
        }
        private void WriteSiteCount(object text)
        {
            this.label2.Text = text.ToString();
        }
        private void WriteNoCheckSite(object text)
        {
            this.label6.Text = text.ToString();
        }
        private void WriteCheckedCount(object text)
        {
            this.label4.Text = text.ToString();
        }
        private void WriteProStatus(object text)
        {
            this.label8.Text = text.ToString();
        }
        private void WriteTaskType(object text)
        {
            this.label16.Text = text.ToString();
        }
        private void WriteButttonStatus(object status)
        {
            if (status.ToString() == "已停止")
                this.button2.Enabled = true;
            else if (status.ToString() == "运行中")
                this.button2.Enabled = false;
        }
        private void WriteLastCheckTime(object text)
        {
            this.label20.Text = text.ToString();
        }
        private void WriteLastCheckCount(object text)
        {
            this.label24.Text =text.ToString();
        }
        private void WriteLastErrorCount(object text)
        {
            this.label22.Text = text.ToString();
        }
        #endregion

        #region 检测是否时间已到达,如果到达则启动网站点击线程
        /// <summary>
        /// 检测是否时间已到达
        /// </summary>
        /// <param name="obj"></param>
        private void CheckTime(object obj)
        {
            string _time = Setting.GetSetting().AutoRunTime;
            if (_prostatus == "已停止" && Setting.GetSetting().ClickStatus == "1")
            {
                this._taskType = "网站命令";
                CreateThread();
            }

            lock (_time)
            {
                try
                {
                    DateTime _nextTaskTime = _nextTaskTime = new DateTime(System.DateTime.Now.Year, System.DateTime.Now.Month, System.DateTime.Now.Day, int.Parse(_time.Split(':')[0]), int.Parse(_time.Split(':')[1]), 0);

                    this.label18.Invoke(new InvokeMethod(WriteTaskTime), "每天 " + _time + " 开始");
                    this.label15.Invoke(new InvokeMethod(WriteCurTime), System.DateTime.Now.ToString());
                    this.label14.Invoke(new InvokeMethod(WriteUi), _cur_pid);
                    this.label12.Invoke(new InvokeMethod(WriteCurUrl), _cur_url);
                    this.label10.Invoke(new InvokeMethod(WriteCurName), _cur_Name);
                    this.label2.Invoke(new InvokeMethod(WriteSiteCount), _siteCount);
                    this.label4.Invoke(new InvokeMethod(WriteCheckedCount), _checkSiteCount);
                    this.label6.Invoke(new InvokeMethod(WriteNoCheckSite), _siteCount - _checkSiteCount);
                    this.label8.Invoke(new InvokeMethod(WriteProStatus), _prostatus);
                    this.label16.Invoke(new InvokeMethod(WriteTaskType), _taskType);
                    this.button2.Invoke(new InvokeMethod(WriteButttonStatus), _prostatus);

                    if (System.DateTime.Now.ToString() == _nextTaskTime.ToString())
                    {
                        this._taskType = "自动运行";
                        CreateThread();
                    }
                }
                catch { }
            }
        }
        #endregion

        #region 事件
        /// <summary>
        /// 系统设置
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button2_Click(object sender, EventArgs e)
        {
            lock (objlock)
            {
                setFrom _setFrom = new setFrom();
                _setFrom.ShowInTaskbar = false;
                _setFrom.ShowDialog(this);
            }
        }

        /// <summary>
        /// 用户点击
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button1_Click(object sender, EventArgs e)
        {
            this._taskType = "手动扫描";
            if (button1.Text == "开始")
            {
                button1.Text = "停止";
                CreateThread();
            }
            else
            {
                button1.Text = "开始";
                DisponseThread();
            }
        }
        #endregion
    }
}