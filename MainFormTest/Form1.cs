using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Threading;
using System.Windows.Forms;
using QQSDK;

namespace Main
{
    public partial class Form1 : Form
    {
		internal static Form1 MyInstance;
		public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
			MyInstance = this;
		}

        private void Button1_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(TextBox1.Text) || string.IsNullOrEmpty(TextBox2.Text))
                     return;
            SDK.GetResultCallBack(SendMessageCallBack);
            SDK.GetLogCallBack(PrintLog);
            SDK.LoginIn(TextBox1.Text, TextBox2.Text);

        }

        #region 回调函数
        public string PrintLog( string szContent)
        {
            Form1.MyInstance.Invoke(new MethodInvoker(() => Form1.MyInstance.RichTextBox1.AppendText("【" + DateTime.Now + "】" + szContent + "\r\n")));
            return szContent;
        }
        public string SendMessageCallBack(string szGroupID, string szQQID, string szContent) 
		{
			if (szGroupID == szQQID && !string.IsNullOrEmpty(szQQID) && !string.IsNullOrEmpty(szContent))
			{
				Form1.MyInstance.Invoke(new MethodInvoker(() => Form1.MyInstance.RichTextBox1.AppendText("【" + DateTime.Now + "】" + "[好友#" + szQQID.ToString() + "]" + szContent + "\r\n")));
			}
			else if (string.IsNullOrEmpty(szGroupID) && string.IsNullOrEmpty(szQQID))
			{
				Form1.MyInstance.Invoke(new MethodInvoker(() => Form1.MyInstance.RichTextBox1.AppendText("【" + DateTime.Now + "】" + "[好友#" + szQQID.ToString() + "]" + szContent + "\r\n")));
			}
			else
			{
                Form1.MyInstance.Invoke(new MethodInvoker(() => Form1.MyInstance.RichTextBox1.AppendText("【" + DateTime.Now + "】"  + "[群#" + szGroupID.ToString() + "]" + "["+ szQQID.ToString() + "]" +  szContent + "\r\n")));
            }
			return szContent;
		}
        public object GetValue(string szContent,object obj)
        {
            if (obj is string)
            {

            }
            else if (obj is IDictionary)
            {

            }          
            else if (obj is Array)
            {
               
            }
            else if (obj is IList)
            {
               
            }
            else if (obj is ICollection)
            {
                
            }
            return obj;
        }


        #endregion


        private void Button3_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(TextBox6.Text) || string.IsNullOrEmpty(RichTextBox1.Text))
                 return;
            SDK.SendPrivateMsg(long.Parse(TextBox1.Text), long.Parse(TextBox6.Text), RichTextBox2.Text);
        }

        private void Button2_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(TextBox5.Text) || string.IsNullOrEmpty(RichTextBox1.Text))
                return;
            if (string.IsNullOrEmpty(TextBox6.Text))
                 SDK.SendGroupMsg(long.Parse(TextBox1.Text),  long.Parse(TextBox5.Text), RichTextBox2.Text);
            else
                SDK.SendGroupMsg(long.Parse(TextBox1.Text), long.Parse(TextBox5.Text), RichTextBox2.Text, long.Parse(TextBox6.Text));
        }

        private void Button4_Click(object sender, EventArgs e)
        {
            byte[] picBytes = null;
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Image Files(*.jpg; *.png; *.jpeg; *.gif; *.bmp)|*.jpg; *.png; *.jpeg; *.gif; *.bmp";
            openFileDialog.RestoreDirectory = true;
            openFileDialog.FilterIndex = 1;
            DialogResult result = openFileDialog.ShowDialog();
            if (result == DialogResult.OK)
            {
                Bitmap image1 = new Bitmap(openFileDialog.FileName);
                picBytes = File.ReadAllBytes(openFileDialog.FileName);
                SDK.SendPrivatePicMsg(long.Parse(TextBox1.Text), long.Parse(TextBox6.Text), picBytes);
            }
            
        }

        private void Button5_Click(object sender, EventArgs e)
        {
            byte[] picBytes = null;
            System.Windows.Forms.OpenFileDialog openFileDialog = new System.Windows.Forms.OpenFileDialog();
            openFileDialog.Filter = "Image Files(*.jpg; *.png; *.jpeg; *.gif; *.bmp)|*.jpg; *.png; *.jpeg; *.gif; *.bmp";
            openFileDialog.RestoreDirectory = true;
            openFileDialog.FilterIndex = 1;
            DialogResult result = openFileDialog.ShowDialog();
            if (result == DialogResult.OK)
            {
                Bitmap image1 = new Bitmap(openFileDialog.FileName);
                picBytes = File.ReadAllBytes(openFileDialog.FileName);
                SDK.SendGroupPicMsg(long.Parse(TextBox1.Text), long.Parse(TextBox5.Text), picBytes);
            }
        }


        private void Button18_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(TextBox6.Text))
                return;
            List<string> list= SDK.GetGroupList(long.Parse(TextBox6.Text));
            Form1.MyInstance.Invoke(new MethodInvoker(() => Form1.MyInstance.RichTextBox1.AppendText("【" + DateTime.Now + "】" + string.Join(",", list) + "\r\n")));
        }

        private void Button15_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(TextBox5.Text))
                return;
            List<string> list = SDK.GetGroupMemberList(long.Parse(TextBox5.Text));
            Form1.MyInstance.Invoke(new MethodInvoker(() => Form1.MyInstance.RichTextBox1.AppendText("【" + DateTime.Now + "】" + string.Join(",", list) + "\r\n")));
        }

        private void Button19_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(TextBox5.Text))
                return;
            List<string> list = SDK.GetGroupAdminList(long.Parse(TextBox5.Text));
            Form1.MyInstance.Invoke(new MethodInvoker(() => Form1.MyInstance.RichTextBox1.AppendText("【" + DateTime.Now + "】" + string.Join(",", list) + "\r\n")));
        }

        private void Button10_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(TextBox6.Text))
                return;
            List<string> list = SDK.GetQQNick(long.Parse(TextBox6.Text));
            Form1.MyInstance.Invoke(new MethodInvoker(() => Form1.MyInstance.RichTextBox1.AppendText("【" + DateTime.Now + "】" + string.Join(",", list) + "\r\n")));
        }

        private void Button17_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(TextBox6.Text))
                return;
            List<string> list = SDK.GetFriendList(long.Parse(TextBox6.Text));
            Form1.MyInstance.Invoke(new MethodInvoker(() => Form1.MyInstance.RichTextBox1.AppendText("【" + DateTime.Now + "】" + string.Join(",", list) + "\r\n")));
        }

        private void Button7_Click(object sender, EventArgs e)
        {
            byte[] AudioBytes = null;
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Amr Files(*.amr|*.amr";
            openFileDialog.RestoreDirectory = true;
            openFileDialog.FilterIndex = 1;
            DialogResult result = openFileDialog.ShowDialog();
            if (result == DialogResult.OK)
            {
                AudioBytes = File.ReadAllBytes(openFileDialog.FileName);
                SDK.SendPrivateAudioMsg(long.Parse(TextBox1.Text), long.Parse(TextBox6.Text), AudioBytes);
            }
        }

        private void Button6_Click(object sender, EventArgs e)
        {
            byte[] AudioBytes = null;
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Amr Files(*.amr|*.amr";
            openFileDialog.RestoreDirectory = true;
            openFileDialog.FilterIndex = 1;
            DialogResult result = openFileDialog.ShowDialog();
            if (result == DialogResult.OK)
            {
                AudioBytes = File.ReadAllBytes(openFileDialog.FileName);
                SDK.SendGroupAudioMsg(long.Parse(TextBox1.Text), long.Parse(TextBox5.Text), AudioBytes);
            }
        }
    }
}
