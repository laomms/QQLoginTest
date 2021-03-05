using Kyozy.MiniblinkNet;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Script.Serialization;
using System.Windows.Forms;
using WindowsInput;

namespace QQSDK
{
    public partial class WebForm : Form
    {
        public WebForm()
        {
            InitializeComponent();
        }
		public static string Url;
		private WebView m_WebView;
		private bool LoadFinshed = false;
		Point m_CurrentPoint = new Point();
		Point m_TargetPoint = new Point();
		List<Point> m_posList = new List<Point>();
		public System.Timers.Timer MouseMoveTimer;
		List<Tuple<Action<object[]>, object[]>> m_TaskActionList = new List<Tuple<Action<object[]>, object[]>>();
		enum MoveType
		{
			RightDownX,
			RightDownY,
			RightUpX,
			RightUpY,
			LeftDownX,
			LeftDownY,
			LeftUpX,
			LeftUpY
		}
		private void WebForm_Load(object sender, EventArgs e)
        {
			m_WebView = new WebView();
			m_WebView.Bind(this);

			m_WebView.SetDeviceParameter("screen.width", string.Empty, this.Width);
			m_WebView.NavigationToNewWindowEnable = false;

			m_WebView.OnLoadUrlBegin += m_WebView_OnLoadUrlBegin;
			m_WebView.OnLoadUrlEnd += m_WebView_OnLoadUrlEnd;
			m_WebView.OnLoadingFinish += m_WebView_OnLoadingFinish;
			m_WebView.OnDocumentReady += m_WebView_OnDocumentReady;
			//m_WebView.OnURLChange += m_WebView_OnURLChange;
			m_WebView.OnNetResponse += m_WebView_OnNetResponse;
			m_WebView.OnMouseoverUrlChange += m_WebView_OnMouseoverUrlChange;
			//m_WebView.onImageBufferToDataURL += WebView_OnImageBufferToDataURL;
			m_WebView.LoadURL(Url);
			//测试绑定JsFunction
			//JsValue.BindFunction("OnBtnClick", new wkeJsNativeFunction(OnBtnClick), 0);
		}

		//js中的 OnBtnClick() 会调用到此处
		//long OnBtnClick(IntPtr es, IntPtr param)
		//{
		//	JsValue v = m_WebView.RunJS("return document.getElementsByTagName('html')[0].outerHTML;");
		//	System.Diagnostics.Debug.WriteLine(v.ToString(es)); // 带参数的 ToString 方法来取文本
		//	return JsValue.UndefinedValue();
		//}
		private void m_WebView_OnDownload(object sender, DownloadEventArgs e)
		{
			Debug.WriteLine("{0}:{1}", "OnDownload", e.URL);
			e.Cancel = true;
		}


		private void m_WebView_OnLoadUrlBegin(object sender, LoadUrlBeginEventArgs e)
		{
			Debug.WriteLine("OnLoadUrlBegin:" + e.URL.ToString());
			if (e.URL.Contains("https://t.captcha.qq.com/cap_union_new_verify"))
			{
				m_WebView.NetHookRequest(e.Job);
			}
			//if (e.URL.Contains("https://t.captcha.qq.com/cap_union_new_show"))
			//{
			//    m_WebView.NetHookRequest(e.Job);
			//}
		}
		void m_WebView_OnDocumentReady(object sender, DocumentReadyEventArgs e)
		{
			//测试自动滑块处理
			object value = m_WebView.RunJS("return document.getElementById('tcaptcha_iframe').getAttribute('src');");
			if (value != null)
			{
				Debug.Print("tcaptcha_iframe_url:" + value.ToString());
				m_WebView.LoadURL(value.ToString());//加载iframe真实地址
			}
			Debug.Print("OnDocumentReady:" + m_WebView.GetSource());
		}
		private void m_WebView_OnLoadUrlEnd(object sender, LoadUrlEndEventArgs e)
		{
			WebView.NetSetMIMEType(e.Job, "text/html");
			string htmls = System.Text.Encoding.UTF8.GetString(e.Data);
			if (htmls.Contains("ticket"))
			{
				Debug.Print(htmls);
				dynamic Json = new JavaScriptSerializer().DeserializeObject(htmls);
				API.QQ.Ticket = Json["ticket"];
				this.Close();
			}
			

		}
		private void m_WebView_OnLoadingFinish(object sender, LoadingFinishEventArgs e)
		{
			if (LoadFinshed == true)
			{
				Drag(this.Left + m_CurrentPoint.X + 20, this.Top + m_CurrentPoint.Y + 50, this.Left + m_TargetPoint.X + 20, this.Top + m_TargetPoint.Y + 50);
				LoadFinshed = false;
			}
		}

		static void Drag(int startX, int startY, int endX, int endY)
		{
			int screenWidth = Screen.PrimaryScreen.Bounds.Width;
			int screenHeight = Screen.PrimaryScreen.Bounds.Height;
			var sim = new InputSimulator();
			sim.Mouse.MoveMouseToPositionOnVirtualDesktop(Convert.ToDouble(startX * 65535 / screenWidth), Convert.ToDouble(startY * 65535 / screenHeight));
			Thread.Sleep(1000);
			sim.Mouse.LeftButtonDown();
			Thread.Sleep(1000);
			sim.Mouse.MoveMouseTo(Convert.ToDouble(endX * 65535 / screenWidth), Convert.ToDouble(endY * 65535 / screenHeight));
			Thread.Sleep(1000);
			sim.Mouse.LeftButtonUp();
		}

		private void m_WebView_OnNetResponse(object sender, NetResponseEventArgs e)
		{
			if (e.URL.Contains("https://t.captcha.qq.com/caplog?appid="))
			{
				object value = null;
				double tcOperationBkgWidth = 0;
				int slideBlock_X = 0;
				int slideBlock_Y = 0;

				object values = m_WebView.RunJS("return document.getElementById('tcOperation').getBoundingClientRect().width;"); //整个背景图坐标
				if (values != null)
				{
					tcOperationBkgWidth = double.Parse(values.ToString());
					Debug.Print("背景图宽度:" + values.ToString());
				}

				value = m_WebView.RunJS("return document.querySelector('#slideBlock').getBoundingClientRect().left;"); //获取滑动的X坐标
				if (value != null)
				{
					m_CurrentPoint = GetElementPointByJs(m_WebView, "slideBlock", "ID", "");
					slideBlock_X = m_CurrentPoint.X;
					slideBlock_Y = m_CurrentPoint.Y;
					Debug.Print($"滑块图片相对浏览器坐标，x={m_CurrentPoint.X}，y={m_CurrentPoint.Y}");
					//Debug.Print($"滑块图片屏幕坐标，x={this.Left + m_CurrentPoint.X}，y={this.Top + m_CurrentPoint.Y}");
				}
				//Debug.Print("OnLoadingFinish:" + e.URL);

				value = m_WebView.RunJS("return document.getElementById('slideBg').getAttribute('src');"); //获取滑动背景图片地址 
				if (value != null)
				{

					string slideBgUrl = "https://t.captcha.qq.com" + value.ToString();
					string oldUrl = Regex.Replace(slideBgUrl, @"=\d&", "=0&");
					Bitmap oldBmp = (Bitmap)GetImg(oldUrl);
					Bitmap slideBgBmp = (Bitmap)GetImg(slideBgUrl);
					int left = GetArgb(oldBmp, slideBgBmp);// 比较两张图片的像素，确定阴影图片位置  得到阴影到图片左边界的像素 原始验证图起点
					Debug.Print($"缺口图片到背景图片边距：{left}");

					var leftCount = tcOperationBkgWidth / (double)slideBgBmp.Width * left; //浏览器验证图起点
					Debug.Print($"需要移动距离：{leftCount}");

					int leftShift = (int)leftCount - 30; //实际移动
					Debug.Print($"实际移动：{leftShift}");

					m_TargetPoint = new Point(slideBlock_X + leftShift, slideBlock_Y);
					Debug.Print($"目标缺口图像元素坐标，x={m_TargetPoint.X}，y={m_TargetPoint.Y}");

					LoadFinshed = true;

				}
			}

		}


		private void m_tsstbUrl_KeyDown(object sender, KeyEventArgs e)
		{
			Debug.Print("KeyDown:" + e.KeyCode.ToString());
			if (e.KeyCode == Keys.Enter)
			{

			}
		}

		private void m_WebView_OnURLChange(object sender, UrlChangeEventArgs e)
		{
			Debug.Print("OnURLChange:" + e.URL.ToString());
		}

		private void m_WebView_OnMouseoverUrlChange(object sender, MouseOverUrlChangedEventArgs e)
		{
			Debug.Print("OnMouseoverUrlChange:" + e.URL);
		}
		private void m_WebView_OnTitleChange(object sender, TitleChangeEventArgs e)
		{
			this.Text = e.Title;
		}
		private void tsbtnMain_Click(object sender, EventArgs e)
		{
			Debug.Print("Click:" + e.ToString());
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

		public static Point GetElementPointByJs(WebView wView, string strElement, string strType, string strIndex)
		{
			string strGetTopJs = "function getOffsetTop(el){return el.offsetParent? el.offsetTop + getOffsetTop(el.offsetParent): el.offsetTop}\n";
			string strGetLeftJs = "function getOffsetLeft(el){return el.offsetParent? el.offsetLeft + getOffsetLeft(el.offsetParent): el.offsetLeft}\n";
			string strJs = null;
			switch (strType)
			{
				case "ID":
					strJs = strGetTopJs + strGetLeftJs + $"return new Array(getOffsetLeft(document.getElementById(\"{strElement}\"){strIndex}), getOffsetTop(document.getElementById(\"{strElement}\"){strIndex}))";
					break;

				case "Name":
					strJs = strGetTopJs + strGetLeftJs + $"return new Array(getOffsetLeft(document.getElementByName(\"{strElement}\"){strIndex}), getOffsetTop(document.getElementByName(\"{strElement}\"){strIndex}))";
					break;

				case "Tag":
					strJs = strGetTopJs + strGetLeftJs + $"return new Array(getOffsetLeft(document.getELementsByTagName(\"{strElement}\"){strIndex}), getOffsetTop(document.getELementsByTagName(\"{strElement}\"){strIndex}))";
					break;

				case "Class":
					strJs = strGetTopJs + strGetLeftJs + $"return new Array(getOffsetLeft(document.getElementsByClassName(\"{strElement}\"){strIndex}), getOffsetTop(document.getElementsByClassName(\"{strElement}\"){strIndex}))";
					break;
			}

			object[] jv = (object[])wView.RunJS(strJs);

			if (jv.Length == 2)
			{
				int iX = 25 + Convert.ToInt32(jv[0]);     // 加一点偏移，方便点击
				int iY = 10 + Convert.ToInt32(jv[1]);

				return new Point(iX, iY);
			}
			else
			{
				return new Point(-9999, -9999);    // 指定查找的元素不存在
			}
		}

		/// <summary>
		/// 根据图片地址，得到图片对象
		/// </summary>
		/// <param name="url"></param>
		/// <returns></returns>
		public static Image GetImg(string url)
		{

			WebRequest webreq = WebRequest.Create(url);
			WebResponse webres = webreq.GetResponse();
			Image img;
			using (System.IO.Stream stream = webres.GetResponseStream())
			{
				img = Image.FromStream(stream);
			}
			return img;
		}
		/// <summary>
		/// 比较两张图片的像素，确定阴影图片位置
		/// </summary>
		/// <param name="oldBmp"></param>
		/// <param name="newBmp"></param>
		/// <returns></returns>
		public static int GetArgb(Bitmap oldBmp, Bitmap newBmp)
		{
			//由于阴影图片四个角存在黑点(矩形1*1) 
			for (int i = 0; i < newBmp.Width; i++)
			{

				for (int j = 0; j < newBmp.Height; j++)
				{
					if ((i >= 0 && i <= 1) && ((j >= 0 && j <= 1) || (j >= (newBmp.Height - 2) && j <= (newBmp.Height - 1))))
					{
						continue;
					}
					if ((i >= (newBmp.Width - 2) && i <= (newBmp.Width - 1)) && ((j >= 0 && j <= 1) || (j >= (newBmp.Height - 2) && j <= (newBmp.Height - 1))))
					{
						continue;
					}

					//获取该点的像素的RGB的颜色
					Color oldColor = oldBmp.GetPixel(i, j);
					Color newColor = newBmp.GetPixel(i, j);
					if (Math.Abs(oldColor.R - newColor.R) > 60 || Math.Abs(oldColor.G - newColor.G) > 60 || Math.Abs(oldColor.B - newColor.B) > 60)
					{
						return i;
					}


				}
			}
			return 0;
		}
	}
}
