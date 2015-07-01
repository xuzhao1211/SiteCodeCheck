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
                MessageBox.Show("�������ʱ���ʽ����ȷ!"); return;
            }
            try
            {
                hour = int.Parse(time[0]);
                mini = int.Parse(time[1]);
            }
            catch { MessageBox.Show("�������ʱ���ʽ����ȷ!"); return; }
            if (!(hour >= 0 && hour <= 23))
            {
                MessageBox.Show("Сʱ������0-23֮��!"); return;
            }
            if (!(mini >= 0 && mini <= 59))
            {
                MessageBox.Show("���ֱ�����0-59֮��!"); return;
            }
            Setting set = Setting.GetSetting();
            set.AutoRunTime = this.textBox1.Text.Trim();
            Setting.Save(set);
            MessageBox.Show("���óɹ�");
            this.Dispose();
        }
    }
}