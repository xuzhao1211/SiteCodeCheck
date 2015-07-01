using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using LzSoft.SysXmlIO;

namespace LzSoft.SysService
{
    public partial class setFrom : Form
    {
        public setFrom()
        {
            InitializeComponent();
            Setting set = Setting.GetSetting();
            this.textBox1.Text = set.AutoRunTime;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string[] time = this.textBox1.Text.Split(':');
            int hour = 0;
            int mini = 0;
            if (time.Length != 2)
            {
                MessageBox.Show("您输入的时间格式不正确!"); return;
            }
            try
            {
                hour = int.Parse(time[0]);
                mini = int.Parse(time[1]);
            }
            catch { MessageBox.Show("您输入的时间格式不正确!"); return; }
            if (!(hour >= 0 && hour <= 23))
            {
                MessageBox.Show("小时必须在0-23之间!"); return;
            }
            if (!(mini >= 0 && mini <= 59))
            {
                MessageBox.Show("分种必须在0-59之间!"); return;
            }
            Setting set = Setting.GetSetting();
            set.AutoRunTime = this.textBox1.Text.Trim();
            Setting.Save(set);
            MessageBox.Show("设置成功");
            this.Dispose();
        }
    }
}