using System.Collections.ObjectModel;
using System.Drawing.Imaging;
using System.IO;
using System.Net;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using ProtoBuf;

//INSTANT C# NOTE: Formerly VB project-level imports:
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Diagnostics;
using System.Windows.Forms;
using System.Linq;
using System.Xml.Linq;
using System.Threading.Tasks;

namespace QQ_Login
{
	public partial class Form1 : System.Windows.Forms.Form
	{
		//Form 重写 Dispose,以清理组件列表。
		[System.Diagnostics.DebuggerNonUserCode()]
		protected override void Dispose(bool disposing)
		{
			try
			{
				if (disposing && components != null)
				{
					components.Dispose();
				}
			}
			finally
			{
				base.Dispose(disposing);
			}
		}

		//Windows 窗体设计器所必需的
		private System.ComponentModel.IContainer components;

		//注意: 以下过程是 Windows 窗体设计器所必需的
		//可以使用 Windows 窗体设计器修改它。  
		//不要使用代码编辑器修改它。
		[System.Diagnostics.DebuggerStepThrough()]
		private void InitializeComponent()
		{
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            this.Label1 = new System.Windows.Forms.Label();
            this.TextBox2 = new System.Windows.Forms.TextBox();
            this.Label2 = new System.Windows.Forms.Label();
            this.Button1 = new System.Windows.Forms.Button();
            this.GroupBox1 = new System.Windows.Forms.GroupBox();
            this.Button20 = new System.Windows.Forms.Button();
            this.TextBox1 = new System.Windows.Forms.TextBox();
            this.RichTextBox1 = new System.Windows.Forms.RichTextBox();
            this.RichTextBox2 = new System.Windows.Forms.RichTextBox();
            this.Button2 = new System.Windows.Forms.Button();
            this.TextBox5 = new System.Windows.Forms.TextBox();
            this.Label5 = new System.Windows.Forms.Label();
            this.Button3 = new System.Windows.Forms.Button();
            this.TextBox6 = new System.Windows.Forms.TextBox();
            this.Label6 = new System.Windows.Forms.Label();
            this.Button4 = new System.Windows.Forms.Button();
            this.Button5 = new System.Windows.Forms.Button();
            this.OpenFileDialog1 = new System.Windows.Forms.OpenFileDialog();
            this.GroupBox2 = new System.Windows.Forms.GroupBox();
            this.Button22 = new System.Windows.Forms.Button();
            this.Button21 = new System.Windows.Forms.Button();
            this.Button19 = new System.Windows.Forms.Button();
            this.Button18 = new System.Windows.Forms.Button();
            this.Button15 = new System.Windows.Forms.Button();
            this.Button12 = new System.Windows.Forms.Button();
            this.Button11 = new System.Windows.Forms.Button();
            this.Button8 = new System.Windows.Forms.Button();
            this.Button6 = new System.Windows.Forms.Button();
            this.GroupBox4 = new System.Windows.Forms.GroupBox();
            this.Button17 = new System.Windows.Forms.Button();
            this.Button16 = new System.Windows.Forms.Button();
            this.Button13 = new System.Windows.Forms.Button();
            this.Button14 = new System.Windows.Forms.Button();
            this.Button10 = new System.Windows.Forms.Button();
            this.Button9 = new System.Windows.Forms.Button();
            this.Button7 = new System.Windows.Forms.Button();
            this.GroupBox1.SuspendLayout();
            this.GroupBox2.SuspendLayout();
            this.GroupBox4.SuspendLayout();
            this.SuspendLayout();
            // 
            // Label1
            // 
            this.Label1.AutoSize = true;
            this.Label1.Location = new System.Drawing.Point(14, 23);
            this.Label1.Name = "Label1";
            this.Label1.Size = new System.Drawing.Size(50, 13);
            this.Label1.TabIndex = 0;
            this.Label1.Text = "QQ账号:";
            // 
            // TextBox2
            // 
            this.TextBox2.Location = new System.Drawing.Point(82, 56);
            this.TextBox2.Name = "TextBox2";
            this.TextBox2.PasswordChar = '*';
            this.TextBox2.Size = new System.Drawing.Size(187, 20);
            this.TextBox2.TabIndex = 3;
            this.TextBox2.Text = "hgyjbhk6k";
            this.TextBox2.DoubleClick += new System.EventHandler(this.TextBox2_DoubleClick);
            // 
            // Label2
            // 
            this.Label2.AutoSize = true;
            this.Label2.Location = new System.Drawing.Point(14, 60);
            this.Label2.Name = "Label2";
            this.Label2.Size = new System.Drawing.Size(50, 13);
            this.Label2.TabIndex = 2;
            this.Label2.Text = "QQ密码:";
            // 
            // Button1
            // 
            this.Button1.Location = new System.Drawing.Point(25, 94);
            this.Button1.Name = "Button1";
            this.Button1.Size = new System.Drawing.Size(87, 28);
            this.Button1.TabIndex = 4;
            this.Button1.Text = "登录";
            this.Button1.UseVisualStyleBackColor = true;
            this.Button1.Click += new System.EventHandler(this.Button1_Click);
            // 
            // GroupBox1
            // 
            this.GroupBox1.Controls.Add(this.Button20);
            this.GroupBox1.Controls.Add(this.TextBox1);
            this.GroupBox1.Controls.Add(this.Label1);
            this.GroupBox1.Controls.Add(this.Label2);
            this.GroupBox1.Controls.Add(this.TextBox2);
            this.GroupBox1.Controls.Add(this.Button1);
            this.GroupBox1.Location = new System.Drawing.Point(12, 1);
            this.GroupBox1.Name = "GroupBox1";
            this.GroupBox1.Size = new System.Drawing.Size(293, 200);
            this.GroupBox1.TabIndex = 18;
            this.GroupBox1.TabStop = false;
            // 
            // Button20
            // 
            this.Button20.Location = new System.Drawing.Point(182, 94);
            this.Button20.Name = "Button20";
            this.Button20.Size = new System.Drawing.Size(87, 28);
            this.Button20.TabIndex = 19;
            this.Button20.Text = "下线";
            this.Button20.UseVisualStyleBackColor = true;
            this.Button20.Click += new System.EventHandler(this.Button20_Click);
            // 
            // TextBox1
            // 
            this.TextBox1.Location = new System.Drawing.Point(82, 19);
            this.TextBox1.Name = "TextBox1";
            this.TextBox1.Size = new System.Drawing.Size(187, 20);
            this.TextBox1.TabIndex = 1;
            this.TextBox1.Text = "2175528818";
            this.TextBox1.DoubleClick += new System.EventHandler(this.TextBox1_DoubleClick);
            // 
            // RichTextBox1
            // 
            this.RichTextBox1.Location = new System.Drawing.Point(331, 7);
            this.RichTextBox1.Name = "RichTextBox1";
            this.RichTextBox1.Size = new System.Drawing.Size(587, 541);
            this.RichTextBox1.TabIndex = 19;
            this.RichTextBox1.Text = "";
            // 
            // RichTextBox2
            // 
            this.RichTextBox2.Location = new System.Drawing.Point(12, 140);
            this.RichTextBox2.Name = "RichTextBox2";
            this.RichTextBox2.Size = new System.Drawing.Size(293, 128);
            this.RichTextBox2.TabIndex = 20;
            this.RichTextBox2.Text = "消息测试";
            // 
            // Button2
            // 
            this.Button2.Location = new System.Drawing.Point(115, 17);
            this.Button2.Name = "Button2";
            this.Button2.Size = new System.Drawing.Size(71, 28);
            this.Button2.TabIndex = 21;
            this.Button2.Text = "发群消息";
            this.Button2.UseVisualStyleBackColor = true;
            this.Button2.Click += new System.EventHandler(this.Button2_Click);
            // 
            // TextBox5
            // 
            this.TextBox5.Location = new System.Drawing.Point(46, 19);
            this.TextBox5.Name = "TextBox5";
            this.TextBox5.Size = new System.Drawing.Size(63, 20);
            this.TextBox5.TabIndex = 25;
            this.TextBox5.TextChanged += new System.EventHandler(this.TextBox5_TextChanged);
            // 
            // Label5
            // 
            this.Label5.AutoSize = true;
            this.Label5.Location = new System.Drawing.Point(6, 22);
            this.Label5.Name = "Label5";
            this.Label5.Size = new System.Drawing.Size(34, 13);
            this.Label5.TabIndex = 24;
            this.Label5.Text = "群号:";
            // 
            // Button3
            // 
            this.Button3.Location = new System.Drawing.Point(124, 12);
            this.Button3.Name = "Button3";
            this.Button3.Size = new System.Drawing.Size(74, 28);
            this.Button3.TabIndex = 27;
            this.Button3.Text = "私人消息";
            this.Button3.UseVisualStyleBackColor = true;
            this.Button3.Click += new System.EventHandler(this.Button3_Click);
            // 
            // TextBox6
            // 
            this.TextBox6.Location = new System.Drawing.Point(41, 17);
            this.TextBox6.Name = "TextBox6";
            this.TextBox6.Size = new System.Drawing.Size(68, 20);
            this.TextBox6.TabIndex = 29;
            this.TextBox6.TextChanged += new System.EventHandler(this.TextBox6_TextChanged);
            // 
            // Label6
            // 
            this.Label6.AutoSize = true;
            this.Label6.Location = new System.Drawing.Point(1, 20);
            this.Label6.Name = "Label6";
            this.Label6.Size = new System.Drawing.Size(38, 13);
            this.Label6.TabIndex = 28;
            this.Label6.Text = "QQ号:";
            // 
            // Button4
            // 
            this.Button4.Location = new System.Drawing.Point(6, 46);
            this.Button4.Name = "Button4";
            this.Button4.Size = new System.Drawing.Size(61, 28);
            this.Button4.TabIndex = 30;
            this.Button4.Text = "发图片";
            this.Button4.UseVisualStyleBackColor = true;
            this.Button4.Click += new System.EventHandler(this.Button4_Click);
            // 
            // Button5
            // 
            this.Button5.Location = new System.Drawing.Point(11, 51);
            this.Button5.Name = "Button5";
            this.Button5.Size = new System.Drawing.Size(77, 28);
            this.Button5.TabIndex = 31;
            this.Button5.Text = "发群图片";
            this.Button5.UseVisualStyleBackColor = true;
            this.Button5.Click += new System.EventHandler(this.Button5_Click);
            // 
            // OpenFileDialog1
            // 
            this.OpenFileDialog1.FileName = "OpenFileDialog1";
            // 
            // GroupBox2
            // 
            this.GroupBox2.Controls.Add(this.Button22);
            this.GroupBox2.Controls.Add(this.Button21);
            this.GroupBox2.Controls.Add(this.Button19);
            this.GroupBox2.Controls.Add(this.Button18);
            this.GroupBox2.Controls.Add(this.Button15);
            this.GroupBox2.Controls.Add(this.Button12);
            this.GroupBox2.Controls.Add(this.Button11);
            this.GroupBox2.Controls.Add(this.Button8);
            this.GroupBox2.Controls.Add(this.Button6);
            this.GroupBox2.Controls.Add(this.TextBox5);
            this.GroupBox2.Controls.Add(this.Button5);
            this.GroupBox2.Controls.Add(this.Label5);
            this.GroupBox2.Controls.Add(this.Button2);
            this.GroupBox2.Location = new System.Drawing.Point(15, 269);
            this.GroupBox2.Name = "GroupBox2";
            this.GroupBox2.Size = new System.Drawing.Size(289, 156);
            this.GroupBox2.TabIndex = 32;
            this.GroupBox2.TabStop = false;
            // 
            // Button22
            // 
            this.Button22.Location = new System.Drawing.Point(198, 119);
            this.Button22.Name = "Button22";
            this.Button22.Size = new System.Drawing.Size(85, 28);
            this.Button22.TabIndex = 40;
            this.Button22.Text = "关闭禁言";
            this.Button22.UseVisualStyleBackColor = true;
            this.Button22.Click += new System.EventHandler(this.Button22_Click);
            // 
            // Button21
            // 
            this.Button21.Location = new System.Drawing.Point(98, 119);
            this.Button21.Name = "Button21";
            this.Button21.Size = new System.Drawing.Size(88, 28);
            this.Button21.TabIndex = 39;
            this.Button21.Text = "全员禁言";
            this.Button21.UseVisualStyleBackColor = true;
            this.Button21.Click += new System.EventHandler(this.Button21_Click);
            // 
            // Button19
            // 
            this.Button19.Location = new System.Drawing.Point(11, 119);
            this.Button19.Name = "Button19";
            this.Button19.Size = new System.Drawing.Size(77, 28);
            this.Button19.TabIndex = 38;
            this.Button19.Text = "取管理列表";
            this.Button19.UseVisualStyleBackColor = true;
            this.Button19.Click += new System.EventHandler(this.Button19_Click);
            // 
            // Button18
            // 
            this.Button18.Location = new System.Drawing.Point(198, 17);
            this.Button18.Name = "Button18";
            this.Button18.Size = new System.Drawing.Size(85, 28);
            this.Button18.TabIndex = 37;
            this.Button18.Text = "取群列表";
            this.Button18.UseVisualStyleBackColor = true;
            this.Button18.Click += new System.EventHandler(this.Button18_Click);
            // 
            // Button15
            // 
            this.Button15.Location = new System.Drawing.Point(11, 85);
            this.Button15.Name = "Button15";
            this.Button15.Size = new System.Drawing.Size(77, 28);
            this.Button15.TabIndex = 36;
            this.Button15.Text = "群成员列表";
            this.Button15.UseVisualStyleBackColor = true;
            this.Button15.Click += new System.EventHandler(this.Button15_Click);
            // 
            // Button12
            // 
            this.Button12.Location = new System.Drawing.Point(97, 85);
            this.Button12.Name = "Button12";
            this.Button12.Size = new System.Drawing.Size(89, 28);
            this.Button12.TabIndex = 35;
            this.Button12.Text = "发群json消息";
            this.Button12.UseVisualStyleBackColor = true;
            this.Button12.Click += new System.EventHandler(this.Button12_Click);
            // 
            // Button11
            // 
            this.Button11.Location = new System.Drawing.Point(198, 85);
            this.Button11.Name = "Button11";
            this.Button11.Size = new System.Drawing.Size(85, 28);
            this.Button11.TabIndex = 34;
            this.Button11.Text = "发群XML消息";
            this.Button11.UseVisualStyleBackColor = true;
            this.Button11.Click += new System.EventHandler(this.Button11_Click);
            // 
            // Button8
            // 
            this.Button8.Location = new System.Drawing.Point(198, 51);
            this.Button8.Name = "Button8";
            this.Button8.Size = new System.Drawing.Size(85, 28);
            this.Button8.TabIndex = 33;
            this.Button8.Text = "撤回群消息";
            this.Button8.UseVisualStyleBackColor = true;
            this.Button8.Click += new System.EventHandler(this.Button8_Click);
            // 
            // Button6
            // 
            this.Button6.Location = new System.Drawing.Point(98, 51);
            this.Button6.Name = "Button6";
            this.Button6.Size = new System.Drawing.Size(89, 28);
            this.Button6.TabIndex = 32;
            this.Button6.Text = "发群语音";
            this.Button6.UseVisualStyleBackColor = true;
            this.Button6.Click += new System.EventHandler(this.Button6_Click);
            // 
            // GroupBox4
            // 
            this.GroupBox4.Controls.Add(this.Button17);
            this.GroupBox4.Controls.Add(this.Button16);
            this.GroupBox4.Controls.Add(this.Button13);
            this.GroupBox4.Controls.Add(this.Button14);
            this.GroupBox4.Controls.Add(this.Button10);
            this.GroupBox4.Controls.Add(this.Button9);
            this.GroupBox4.Controls.Add(this.Button7);
            this.GroupBox4.Controls.Add(this.Button3);
            this.GroupBox4.Controls.Add(this.TextBox6);
            this.GroupBox4.Controls.Add(this.Button4);
            this.GroupBox4.Controls.Add(this.Label6);
            this.GroupBox4.Location = new System.Drawing.Point(12, 431);
            this.GroupBox4.Name = "GroupBox4";
            this.GroupBox4.Size = new System.Drawing.Size(289, 117);
            this.GroupBox4.TabIndex = 33;
            this.GroupBox4.TabStop = false;
            // 
            // Button17
            // 
            this.Button17.Location = new System.Drawing.Point(4, 80);
            this.Button17.Name = "Button17";
            this.Button17.Size = new System.Drawing.Size(64, 28);
            this.Button17.TabIndex = 39;
            this.Button17.Text = "好友列表";
            this.Button17.UseVisualStyleBackColor = true;
            this.Button17.Click += new System.EventHandler(this.Button17_Click);
            // 
            // Button16
            // 
            this.Button16.Location = new System.Drawing.Point(73, 80);
            this.Button16.Name = "Button16";
            this.Button16.Size = new System.Drawing.Size(57, 28);
            this.Button16.TabIndex = 38;
            this.Button16.Text = "上传";
            this.Button16.UseVisualStyleBackColor = true;
            this.Button16.Click += new System.EventHandler(this.Button16_Click);
            // 
            // Button13
            // 
            this.Button13.Location = new System.Drawing.Point(139, 80);
            this.Button13.Name = "Button13";
            this.Button13.Size = new System.Drawing.Size(59, 28);
            this.Button13.TabIndex = 37;
            this.Button13.Text = "json消息";
            this.Button13.UseVisualStyleBackColor = true;
            this.Button13.Click += new System.EventHandler(this.Button13_Click);
            // 
            // Button14
            // 
            this.Button14.Location = new System.Drawing.Point(204, 80);
            this.Button14.Name = "Button14";
            this.Button14.Size = new System.Drawing.Size(72, 28);
            this.Button14.TabIndex = 36;
            this.Button14.Text = "XML消息";
            this.Button14.UseVisualStyleBackColor = true;
            this.Button14.Click += new System.EventHandler(this.Button14_Click);
            // 
            // Button10
            // 
            this.Button10.Location = new System.Drawing.Point(139, 46);
            this.Button10.Name = "Button10";
            this.Button10.Size = new System.Drawing.Size(59, 28);
            this.Button10.TabIndex = 35;
            this.Button10.Text = "取昵称";
            this.Button10.UseVisualStyleBackColor = true;
            this.Button10.Click += new System.EventHandler(this.Button10_Click);
            // 
            // Button9
            // 
            this.Button9.Location = new System.Drawing.Point(204, 46);
            this.Button9.Name = "Button9";
            this.Button9.Size = new System.Drawing.Size(74, 28);
            this.Button9.TabIndex = 34;
            this.Button9.Text = "撤回消息";
            this.Button9.UseVisualStyleBackColor = true;
            this.Button9.Click += new System.EventHandler(this.Button9_Click);
            // 
            // Button7
            // 
            this.Button7.Location = new System.Drawing.Point(73, 46);
            this.Button7.Name = "Button7";
            this.Button7.Size = new System.Drawing.Size(57, 28);
            this.Button7.TabIndex = 33;
            this.Button7.Text = "发语音";
            this.Button7.UseVisualStyleBackColor = true;
            this.Button7.Click += new System.EventHandler(this.Button7_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(930, 560);
            this.Controls.Add(this.GroupBox4);
            this.Controls.Add(this.GroupBox2);
            this.Controls.Add(this.RichTextBox2);
            this.Controls.Add(this.RichTextBox1);
            this.Controls.Add(this.GroupBox1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "Form1";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "QQ Login V1.01 测试(网中行)";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.GroupBox1.ResumeLayout(false);
            this.GroupBox1.PerformLayout();
            this.GroupBox2.ResumeLayout(false);
            this.GroupBox2.PerformLayout();
            this.GroupBox4.ResumeLayout(false);
            this.GroupBox4.PerformLayout();
            this.ResumeLayout(false);

		}

		internal Label Label1;
		internal TextBox TextBox2;
		internal Label Label2;
		internal Button Button1;
		internal GroupBox GroupBox1;
		internal RichTextBox RichTextBox1;
		internal RichTextBox RichTextBox2;
		internal Button Button2;
		private TextBox TextBox5;
		private Label Label5;
		internal Button Button3;
		private TextBox TextBox6;
		private Label Label6;
		internal TextBox TextBox1;
		internal Button Button4;
		internal Button Button5;
		internal OpenFileDialog OpenFileDialog1;
		internal GroupBox GroupBox2;
		internal Button Button6;
		internal GroupBox GroupBox4;
		internal Button Button7;
		internal Button Button8;
		internal Button Button9;
		internal Button Button10;
		internal Button Button12;
		internal Button Button11;
		internal Button Button13;
		internal Button Button14;
		internal Button Button15;
		internal Button Button16;
		internal Button Button17;
		internal Button Button18;
		internal Button Button19;
		internal Button Button20;
		internal Button Button21;
		internal Button Button22;
	}

}