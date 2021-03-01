using System;
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
            SDK func = new QQSDK.SDK();
            func.GetResultCallBack(SendMessageCallBack);
            func.GetLogCallBack(PrintLog);
            SDK.LoginIn(TextBox1.Text, TextBox2.Text);

        }
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
		private void Button20_Click(object sender, EventArgs e)
        {
            SDK.LoginOff();
        }

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
    }
}
