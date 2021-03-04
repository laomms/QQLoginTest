
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
	public partial class WebForm2
	{
		public WebForm2()
		{
			InitializeComponent();
		}

		public static string Url;
		internal static WebForm MyInstance;
		private WebView m_WebView;
		private IntPtr m_AsynJob = new IntPtr();
		private string PostData = "";
		private void WebForm2_Load(object sender, EventArgs e)
		{
			m_WebView = new WebView();
			m_WebView.Bind(this);
			m_WebView.SetDeviceParameter("screen.width", string.Empty, this.Width);
			m_WebView.NavigationToNewWindowEnable = false;

			m_WebView.OnURLChange += m_WebView_OnURLChange;
			m_WebView.OnLoadingFinish += m_WebView_OnLoadingFinish;
			m_WebView.OnLoadUrlBegin += m_WebView_OnLoadUrlBegin;

			m_WebView.LoadURL(Url);
		}
		private List<string[]> GetAllCookies()
		{
			List<string[]> list = new List<string[]>();
			wkeCookieVisitor visitor = new wkeCookieVisitor((IntPtr usetData, string name, string value, string domain, string path, int secure, int httpOnly, ref int expires) =>
			{
												//Debug.WriteLine("name={0},value={1},domain={2},path={3},secure={4},httpOnly={5},expires={6}", name, value, domain, path, secure, httpOnly, expires)
												var stringList = new string[] {name, value, domain, path, secure.ToString(), httpOnly.ToString(), expires.ToString()};
												list.Add(stringList);
												return false;
			});
			m_WebView.VisitAllCookie(visitor, IntPtr.Zero);
			return list;
		}
		private void m_WebView_OnDownload(object sender, DownloadEventArgs e)
		{
			Debug.WriteLine("{0}:{1}", "OnDownload", e.URL);
			e.Cancel = true;
		}


		private void m_WebView_OnLoadUrlBegin(object sender, LoadUrlBeginEventArgs e)
		{
			Debug.WriteLine("OnLoadUrlBegin:" + "\r\n" + e.URL.ToString());
		}
		private void m_WebView_OnLoadUrlEnd(object sender, LoadUrlEndEventArgs e)
		{
			WebView.NetSetMIMEType(e.Job, "text/html");
			string htmls = System.Text.Encoding.UTF8.GetString(e.Data);
			Debug.Print("OnLoadUrlEnd:" + "\r\n" + htmls);
		}
		private void m_WebView_OnLoadingFinish(object sender, LoadingFinishEventArgs e)
		{
			Debug.Print(e.URL.ToString());
			Debug.Print(m_WebView.GetSource());
			if (m_WebView.GetSource().ToString().Contains("验证成功"))
			{
				this.Close();
			}
		}
		private void m_tsstbUrl_KeyDown(object sender, KeyEventArgs e)
		{
			if (e.KeyCode == Keys.Enter)
			{
				// Debug.WriteLine("Keys.Enter")
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
		}


	}
}