using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Trojan.Tray;

namespace Trojan.Tran.Win
{
    public partial class FrmMain : Form
    {
        TrojanHelper helper = new TrojanHelper();
        public FrmMain()
        {
            InitializeComponent();
            this.notifyIcon.Icon = this.Icon;
            helper.DataReceivedEvent += Helper_DataReceivedEvent; ;
            if (File.Exists(helper.ExePath))
            {
                btnStart.Enabled = true;
            }
            else
            {
                btnStart.Enabled = false;
                var errInfo = $"运行文件不存在：{helper.ExePath}";
                MessageBox.Show(errInfo, "发生错误！", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            btnEnd.Enabled = false;
        }


        private void Helper_DataReceivedEvent(string text, DataReceivedType dataReceivedType)
        {
            UpdataUIStatus(text);
        }

        //更新UI
        private void UpdataUIStatus(string text)
        {
            this.listBox1.BeginInvoke(new Action(() =>
            {
                if (listBox1.Items.Count > 5000)
                {
                    listBox1.Items.Clear();
                }
                listBox1.Items.Add(text);
                listBox1.SelectedIndex = listBox1.Items.Count - 1;
            }));
        }

        private void btnStart_Click(object sender, EventArgs e)
        {
            try
            {
                helper.Start();
                btnEnd.Enabled = true;
                btnStart.Enabled = false;
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, ex.Message, "发生错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void btnEnd_Click(object sender, EventArgs e)
        {
            helper.Close();
            btnEnd.Enabled = false;
            btnStart.Enabled = true;
        }

        private void NotifyIcon_MouseClick(object sender, MouseEventArgs e)
        {
            //点击鼠标"左键"发生
            if (e.Button == MouseButtons.Left)
            {
                this.Visible = true;                        //窗体可见
                this.WindowState = FormWindowState.Normal;  //窗体默认大小
                this.notifyIcon.Visible = true;            //设置图标可见
            }

        }

        //"显示窗体"单击事件
        private void toolStripMenuItem1_Click(object sender, EventArgs e)
        {
            this.Show();                                //窗体显示
            this.WindowState = FormWindowState.Normal;  //窗体状态默认大小
            this.Activate();                            //激活窗体给予焦点
        }
        //"隐藏窗体"单击事件
        private void toolStripMenuItem2_Click(object sender, EventArgs e)
        {
            this.Hide();                      //隐藏窗体
        }

        //"退出"单击事件
        private void toolStripMenuItem3_Click(object sender, EventArgs e)
        {
            //点击"是(YES)"退出程序
            if (MessageBox.Show("确定要退出程序?", "安全提示",
                        System.Windows.Forms.MessageBoxButtons.YesNo,
                        System.Windows.Forms.MessageBoxIcon.Warning)
                == System.Windows.Forms.DialogResult.Yes)
            {
                notifyIcon.Visible = false;   //设置图标不可见
                helper.Close();
                this.Close();                  //关闭窗体
                this.Dispose();                //释放资源
                Application.Exit();            //关闭应用程序窗体
            }
        }

        private void FrmMain_FormClosing(object sender, FormClosingEventArgs e)
        {
            //窗体关闭原因为单击"关闭"按钮或Alt+F4
            if (e.CloseReason == CloseReason.UserClosing)
            {
                e.Cancel = true;           //取消关闭操作 表现为不关闭窗体
                this.Hide();               //隐藏窗体
            }
            else
            {
                helper.Close();
            }
        }

        private void FrmMain_SizeChanged(object sender, EventArgs e)
        {
            if (this.WindowState == FormWindowState.Minimized)
            {
                this.ShowInTaskbar = false; //不显示在系统任务栏
                notifyIcon.Visible = true; //托盘图标可见
            }
        }
    }
}
