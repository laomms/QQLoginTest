
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
using System.Text.RegularExpressions;
using System.Net;

namespace QQSDK
{
	public partial class WebForm
	{
		int m_iMoveIndex = 0;
		Point m_TargetPoint = new Point();
		List<Point> m_posList = new List<Point>();
		MoveType m_MoveType = 0;
		public Timer MouseMoveTimer;
		private Timer CommonTimer;
		int m_iDelIndex = 0;
		int m_iActionStep = 0;
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
			m_WebView.OnLoadingFinish += m_WebView_OnLoadingFinish;
			m_WebView.OnDocumentReady += m_WebView_OnDocumentReady;
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
			//Debug.WriteLine(e.URL.ToString)
			if (e.URL.Contains("cap_union_new_verify"))
			{
				m_WebView.NetHookRequest(e.Job);
			}
		}
		void m_WebView_OnDocumentReady(object sender, DocumentReadyEventArgs e)
		{
			//测试自动滑块处理
			object value = m_WebView.RunJS("return document.getElementById('tcaptcha_iframe').getAttribute('src');");
			if (value!=null)
            {
				Debug.Print("tcaptcha_iframe_url:" + value.ToString());
				m_WebView.LoadURL(value.ToString());//加载iframe真实地址
			}           

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
			}
			this.Close();		
		}
		private void m_WebView_OnLoadingFinish(object sender, LoadingFinishEventArgs e)
		{
			//JsValue value = m_WebView.RunJS("return document.evaluate('//*[@id='slideBlock']', document, null, XPathResult.FIRST_ORDERED_NODE_TYPE, null).singleNodeValue;");
			//Debug.Print("xpath slideBlock:" + value.GetLength(m_WebView.GlobalExec()).ToString());
			//if (value.Value > 0)
			//{
			//}

			//自动滑块处理
			int tcOperationBkgWidth =0;
			object values =m_WebView.RunJS("return document.getElementById('tcOperation').offsetWidth;"); //整个背景图
			if (values != null)
            {
				tcOperationBkgWidth =int.Parse ( values.ToString());
			}

			object value = m_WebView.RunJS("return document.querySelector('#slideBlock').getBoundingClientRect().left;"); //获取滑动的X坐标
			if (value != null)
			{
				Console.WriteLine($"开始位置：{int.Parse(values.ToString())}");
			}

			value = m_WebView.RunJS("return document.getElementById('slideBg').getAttribute('src');"); //获取滑动背景图片地址 
			if (value != null)
			{
				Debug.Print("slideBg图像地址:" + "https://t.captcha.qq.com" + value.ToString());
				string slideBgUrl = "https://t.captcha.qq.com" + value.ToString();
				string oldUrl = Regex.Replace(slideBgUrl, @"=\d&", "=0&");
				Bitmap oldBmp = (Bitmap)GetImg(oldUrl);
				Bitmap slideBgBmp = (Bitmap)GetImg(slideBgUrl);
				int left = GetArgb(oldBmp, slideBgBmp);//得到阴影到图片左边界的像素 原始验证图起点
				Console.WriteLine($"原始验证图起点：{left}");
				var leftCount = (double)tcOperationBkgWidth / (double)slideBgBmp.Width * left; //浏览器验证图起点
				Console.WriteLine($"浏览器验证图起点：{leftCount}");
				int leftShift = (int)leftCount - 30; //实际移动
				Console.WriteLine($"实际移动：{leftShift}");
				//actions.DragAndDropToOffset(slideBlock, leftShift, 0).Build().Perform();//单击并在指定的元素上按下鼠标按钮,然后移动到指定位置
			}
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

		// 移动虚拟鼠标到指定的坐标（相对于浏览器窗口坐标）
		// 如果参数0为Element，则参数1为滚动过程间隔（速度），参数2为要移动到的元素，参数3为元素类型，参数4为元素索引
		// 如果参数0为Point，则参数1为滚动过程间隔（速度），参数2为当前鼠标位置X轴随机移动的正副值范围，参数3为Y轴随机移动的正副值范围
		private void mouseMoveTo(Point currentPoint, object[] args)
		{
			int m_iMoveIndex = 0;
			m_posList.Clear();

	
			float fSpeed = float.Parse(args[1].ToString());

			if ($"{args[0]}" == "Element")
			{
				m_TargetPoint = GetElementPointByJs(m_WebView, $"{args[2]}", $"{args[3]}", $"{args[4]}");
				Debug.Print($"移动鼠标到指定元素坐标，x={m_TargetPoint.X}，y={m_TargetPoint.Y}，间隔速度：{fSpeed}秒");
			}
			else if ($"{args[0]}" == "Point")
			{
				int iX = (int)args[2];
				int iY = (int)args[3];

				Random random = new Random();
				int iTargetX = currentPoint.X + random.Next(-iX, iX);
				iTargetX = iTargetX < 0 ? 0 : iTargetX;
				iTargetX = iTargetX > 1000 ? 1000 : iTargetX;

				int iTargetY = currentPoint.Y + random.Next(-iY, iY);
				iTargetY = iTargetY < 0 ? 0 : iTargetY;
				iTargetY = iTargetY > 920 ? 920 : iTargetY;

				m_TargetPoint = new Point(iTargetX, iTargetY);

				Debug.Print($"移动鼠标到随机位置，x={m_TargetPoint.X}，y={m_TargetPoint.Y}，间隔速度：{fSpeed}秒");
			}

			if (m_TargetPoint.X == -9999 && m_TargetPoint.Y == -9999)
			{
				CommonTimer.Interval = 2000;
				CommonTimer.Start();

				Debug.Print($"查找元素{args[2]}不存在，任务失败");
				return;
			}

			int iSubX = currentPoint.X - m_TargetPoint.X;
			int iSubY = currentPoint.Y - m_TargetPoint.Y;

			if (iSubX >= 0 && iSubY >= 0)    // 当前位置相对目标位置在右下
			{
				if (Math.Abs(iSubX) >= Math.Abs(iSubY))
				{
					m_MoveType = MoveType.RightDownX;
					if (iSubY == 0)
					{
						iSubY = 1;
					}
					for (int i = 0; i < Math.Abs(iSubY); i++)
					{
						m_posList.Add(new Point(currentPoint.X - (Math.Abs(iSubX) / Math.Abs(iSubY) + 1) * (i + 1), currentPoint.Y - (i + 1)));
					}
				}
				else
				{
					m_MoveType = MoveType.RightDownY;
					if (iSubX == 0)
					{
						iSubX = 1;
					}
					for (int i = 0; i < Math.Abs(iSubX); i++)
					{
						m_posList.Add(new Point(currentPoint.X - (i + 1), currentPoint.Y - (Math.Abs(iSubY) / Math.Abs(iSubX) + 1) * (i + 1)));
					}
				}
			}
			else if (iSubX >= 0 && iSubY < 0)    // 当前位置相对目标位置在右上
			{
				if (Math.Abs(iSubX) >= Math.Abs(iSubY))
				{
					m_MoveType = MoveType.RightUpX;
					if (iSubY == 0)
					{
						iSubY = 1;
					}
					for (int i = 0; i < Math.Abs(iSubY); i++)
					{
						m_posList.Add(new Point(currentPoint.X - (Math.Abs(iSubX) / Math.Abs(iSubY) + 1) * (i + 1), currentPoint.Y + (i + 1)));
					}
				}
				else
				{
					m_MoveType = MoveType.RightUpY;
					if (iSubX == 0)
					{
						iSubX = 1;
					}
					for (int i = 0; i < Math.Abs(iSubX); i++)
					{
						m_posList.Add(new Point(currentPoint.X - (i + 1), currentPoint.Y + (Math.Abs(iSubY) / Math.Abs(iSubX) + 1) * (i + 1)));
					}
				}
			}
			else if (iSubX < 0 && iSubY >= 0)    // 当前位置相对目标位置在左下
			{
				if (Math.Abs(iSubX) >= Math.Abs(iSubY))
				{
					m_MoveType = MoveType.LeftDownX;
					if (iSubY == 0)
					{
						iSubY = 1;
					}
					for (int i = 0; i < Math.Abs(iSubY); i++)
					{
						m_posList.Add(new Point(currentPoint.X + (Math.Abs(iSubX) / Math.Abs(iSubY) + 1) * (i + 1), currentPoint.Y - (i + 1)));
					}
				}
				else
				{
					m_MoveType = MoveType.LeftDownY;
					if (iSubX == 0)
					{
						iSubX = 1;
					}
					for (int i = 0; i < Math.Abs(iSubX); i++)
					{
						m_posList.Add(new Point(currentPoint.X + (i + 1), currentPoint.Y - (Math.Abs(iSubY) / Math.Abs(iSubX) + 1) * (i + 1)));
					}
				}
			}
			else if (iSubX < 0 && iSubY < 0)    // 当前位置相对目标位置在左上
			{
				if (Math.Abs(iSubX) >= Math.Abs(iSubY))
				{
					m_MoveType = MoveType.LeftUpX;
					if (iSubY == 0)
					{
						iSubY = 1;
					}
					for (int i = 0; i < Math.Abs(iSubY); i++)
					{
						m_posList.Add(new Point(currentPoint.X + (Math.Abs(iSubX) / Math.Abs(iSubY) + 1) * (i + 1), currentPoint.Y + (i + 1)));
					}
				}
				else
				{
					m_MoveType = MoveType.LeftUpY;
					if (iSubX == 0)
					{
						iSubX = 1;
					}
					for (int i = 0; i < Math.Abs(iSubX); i++)
					{
						m_posList.Add(new Point(currentPoint.X + (i + 1), currentPoint.Y + (Math.Abs(iSubY) / Math.Abs(iSubX) + 1) * (i + 1)));
					}
				}
			}

			MouseMoveTimer.Interval = (int)(fSpeed * 1000);
			MouseMoveTimer.Start();
		}

		private void CommonTimer_Tick(object sender, EventArgs e)
		{
			CommonTimer.Stop();
			m_TaskActionList[m_iActionStep].Item1(m_TaskActionList[m_iActionStep++].Item2);
		}
		private void MouseMoveTimer_Tick(Point currentPoint, object sender, EventArgs e)
		{
			if (m_iMoveIndex < m_posList.Count)
			{
				int iMoveX = 0;
				int iMoveY = 0;
				switch (m_MoveType)
				{
					case MoveType.RightDownX:
					case MoveType.RightUpX:
						iMoveX = m_posList[m_iMoveIndex].X < m_TargetPoint.X ? m_TargetPoint.X : m_posList[m_iMoveIndex].X;
						iMoveY = m_posList[m_iMoveIndex].Y;
						break;

					case MoveType.RightDownY:
					case MoveType.LeftDownY:
						iMoveX = m_posList[m_iMoveIndex].X;
						iMoveY = m_posList[m_iMoveIndex].Y < m_TargetPoint.Y ? m_TargetPoint.Y : m_posList[m_iMoveIndex].Y;
						break;

					case MoveType.RightUpY:
					case MoveType.LeftUpY:
						iMoveX = m_posList[m_iMoveIndex].X;
						iMoveY = m_posList[m_iMoveIndex].Y > m_TargetPoint.Y ? m_TargetPoint.Y : m_posList[m_iMoveIndex].Y;
						break;

					case MoveType.LeftDownX:
					case MoveType.LeftUpX:
						iMoveX = m_posList[m_iMoveIndex].X > m_TargetPoint.X ? m_TargetPoint.X : m_posList[m_iMoveIndex].X;
						iMoveY = m_posList[m_iMoveIndex].Y;
						break;
				}

				m_iMoveIndex++;
				currentPoint = new Point(iMoveX, iMoveY);
			}
			else
			{
				MouseMoveTimer.Stop();
				m_TaskActionList[m_iActionStep].Item1(m_TaskActionList[m_iActionStep++].Item2);
			}
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
				int iY = 10 + Convert.ToInt32(jv[1]) - GetSocrllHeightByJs(wView);

				return new Point(iX, iY);
			}
			else
			{
				return new Point(-9999, -9999);    // 指定查找的元素不存在
			}
		}
		// 获取当前滚动条滚动的高度
		public static int GetSocrllHeightByJs(WebView wView)
		{
			string strGetScrollTop = "$(window).scrollTop();";
			object jv = wView.RunJS("return " + strGetScrollTop);

			return Convert.ToInt32(jv);
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