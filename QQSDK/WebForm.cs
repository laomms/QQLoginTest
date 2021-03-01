
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

using System.IO;
using System.Web.Script.Serialization;
using Kyozy.MiniblinkNet;

namespace QQSDK
{
	public partial class WebForm
	{
		public WebForm()
		{
			InitializeComponent();
		}

		public static string Url;
		internal static WebForm MyInstance;
		private WebView m_WebView;
		private IntPtr m_AsynJob = new IntPtr();
		private string PostData = "";
		private void WebForm_Load(object sender, EventArgs e)
		{
			m_WebView = new WebView();
			m_WebView.Bind(this);
			m_WebView.SetDeviceParameter("screen.width", string.Empty, this.Width);
			m_WebView.NavigationToNewWindowEnable = false;

			m_WebView.OnLoadUrlBegin += m_WebView_OnLoadUrlBegin;
			m_WebView.OnLoadUrlEnd += m_WebView_OnLoadUrlEnd;
			m_WebView.LoadURL(Url);
		}

		private void m_WebView_OnDownload(object sender, DownloadEventArgs e)
		{
			Debug.WriteLine("{0}:{1}", "OnDownload", e.URL);
			e.Cancel = true;
		}


		private void m_WebView_OnLoadUrlBegin(object sender, LoadUrlBeginEventArgs e)
		{
			//Debug.WriteLine(e.URL.ToString)
			if (e.URL.Contains("cap_union_new_verify"))
			{
				m_WebView.NetHookRequest(e.Job);
			}
		}
		private void m_WebView_OnLoadUrlEnd(object sender, LoadUrlEndEventArgs e)
		{
			WebView.NetSetMIMEType(e.Job, "text/html");
			string htmls = System.Text.Encoding.UTF8.GetString(e.Data);
			if (htmls.Contains("ticket"))
			{
				dynamic Json = (new JavaScriptSerializer()).DeserializeObject(htmls);
				API.QQ.Ticket = Json["ticket"];
			}
			if (Directory.Exists(Application.StartupPath + "\\LocalStorage"))
			{
				try
				{
					Directory.Delete(Application.StartupPath + "\\LocalStorage");
				}
				catch (Exception ex)
				{
				}
			}
			if (File.Exists(Application.StartupPath + "\\cookies.dat"))
			{
				try
				{
					File.Delete(Application.StartupPath + "\\cookies.dat");
				}
				catch (Exception ex)
				{
				}
			}
			this.Close();
		}
		private void m_WebView_OnLoadingFinish(object sender, LoadingFinishEventArgs e)
		{
			Debug.Print(m_WebView.GetSource());
		}
		private void m_tsstbUrl_KeyDown(object sender, KeyEventArgs e)
		{
			if (e.KeyCode == Keys.Enter)
			{

			}
		}

		private void m_WebView_OnURLChange(object sender, UrlChangeEventArgs e)
		{

		}

		private void m_WebView_OnTitleChange(object sender, TitleChangeEventArgs e)
		{
			this.Text = e.Title;
		}
		private void tsbtnMain_Click(object sender, EventArgs e)
		{

		}
		private void tsbtnBack_Click(object sender, EventArgs e)
		{
			m_WebView.GoBack();

		}

		private void tsbtnForward_Click(object sender, EventArgs e)
		{
			m_WebView.GoForward();
		}

		private void tsbtnReload_Click(object sender, EventArgs e)
		{
			m_WebView.Reload();
		}

		private void tsbtnStop_Click(object sender, EventArgs e)
		{
			m_WebView.StopLoading();
		}


		private void WebForm_Closed(object sender, EventArgs e)
		{
			m_WebView.Dispose();
			if (Directory.Exists(Application.StartupPath + "\\LocalStorage"))
			{
				try
				{
					Directory.Delete(Application.StartupPath + "\\LocalStorage");
				}
				catch (Exception ex)
				{
				}
			}
			if (File.Exists(Application.StartupPath + "\\cookies.dat"))
			{
				try
				{
					File.Delete(Application.StartupPath + "\\cookies.dat");
				}
				catch (Exception ex)
				{
				}
			}
		}

		private static WebForm _DefaultInstance;
		public static WebForm DefaultInstance
		{
			get
			{
				if (_DefaultInstance == null || _DefaultInstance.IsDisposed)
					_DefaultInstance = new WebForm();

				return _DefaultInstance;
			}
		}
	}
}