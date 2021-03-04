using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.Drawing;
using System.IO;
using System.Reflection;
using System.Dynamic;
using System.Collections;

namespace Kyozy.MiniblinkNet
{
    public class WebView:IDisposable
    {
        private IntPtr Handles;
        private IntPtr m_hWnd;
        private IntPtr m_OldProc;
        private wkePaintUpdatedCallback m_wkePaintUpdatedCallback;
        private WndProcCallback m_WndProcCallback;
        private bool m_noDestory;

        [StructLayout(LayoutKind.Sequential)]
        public struct jsKeys
        {
            public int length;
            public IntPtr keys;
        }

        #region 事件
        private wkeTitleChangedCallback m_wkeTitleChangedCallback;
        private wkeMouseOverUrlChangedCallback m_wkeMouseOverUrlChangedCallback;
        private wkeURLChangedCallback2 m_wkeURLChangedCallback2;
        private wkeAlertBoxCallback m_wkeAlertBoxCallback;
        private wkeConfirmBoxCallback m_wkeConfirmBoxCallback;
        private wkePromptBoxCallback m_wkePromptBoxCallback;
        private wkeNavigationCallback m_wkeNavigationCallback;
        private wkeCreateViewCallback m_wkeCreateViewCallback;
        private wkeDocumentReady2Callback m_wkeDocumentReadyCallback;
        private wkeLoadingFinishCallback m_wkeLoadingFinishCallback;
        private wkeDownloadCallback m_wkeDownloadCallback;
        private wkeConsoleCallback m_wkeConsoleCallback;
        private wkeLoadUrlBeginCallback m_wkeLoadUrlBeginCallback;
        private wkeLoadUrlEndCallback m_wkeLoadUrlEndCallback;
        private wkeDidCreateScriptContextCallback m_wkeDidCreateScriptContextCallback;
        private wkeWillReleaseScriptContextCallback m_wkeWillReleaseScriptContextCallback;
        private wkeNetResponseCallback m_wkeNetResponseCallback;
        private wkeWillMediaLoadCallback m_wkeWillMediaLoadCallback;
        private wkeOnOtherLoadCallback m_wkeOnOtherLoadCallback;

        private EventHandler<TitleChangeEventArgs> m_titleChangeHandler = null;
        private EventHandler<MouseOverUrlChangedEventArgs> m_mouseOverUrlChangedHandler = null;
        private EventHandler<UrlChangeEventArgs> m_urlChangeHandler = null;
        private EventHandler<AlertBoxEventArgs> m_alertBoxHandler = null;
        private EventHandler<ConfirmBoxEventArgs> m_confirmBoxHandler = null;
        private EventHandler<PromptBoxEventArgs> m_promptBoxHandler = null;
        private EventHandler<NavigateEventArgs> m_navigateHandler = null;
        private EventHandler<CreateViewEventArgs> m_createViewHandler = null;
        private EventHandler<DocumentReadyEventArgs> m_documentReadyHandler = null;
        private EventHandler<LoadingFinishEventArgs> m_loadingFinishHandler = null;
        private EventHandler<DownloadEventArgs> m_downloadHandler = null;
        private EventHandler<ConsoleEventArgs> m_consoleHandler = null;
        private EventHandler<LoadUrlBeginEventArgs> m_loadUrlBeginHandler = null;
        private EventHandler<LoadUrlEndEventArgs> m_loadUrlEndHandler = null;
        private EventHandler<DidCreateScriptContextEventArgs> m_didCreateScriptContextHandler = null;
        private EventHandler<WillReleaseScriptContextEventArgs> m_willReleaseScriptContextHandler = null;
        private EventHandler<NetResponseEventArgs> m_netResponseHandler = null;
        private EventHandler<WillMediaLoadEventArgs> m_willMediaLoadHandler = null;
        private EventHandler<OtherLoadEventArgs> m_OtherLoadHandler = null;

        /// <summary>
        /// 窗口过程事件
        /// </summary>
        public event EventHandler<WindowProcEventArgs> OnWindowProc;


        /// <summary>
        /// 鼠标经过URL改变事件
        /// </summary>
        public event EventHandler<MouseOverUrlChangedEventArgs> OnMouseoverUrlChange
        {
            add
            {
                if (m_mouseOverUrlChangedHandler == null)
                {
                    MBApi.wkeOnMouseOverUrlChanged(Handles, m_wkeMouseOverUrlChangedCallback, IntPtr.Zero);
                }
                m_mouseOverUrlChangedHandler += value;
            }
            remove
            {
                m_mouseOverUrlChangedHandler -= value;
                if (m_mouseOverUrlChangedHandler == null)
                {
                    MBApi.wkeOnMouseOverUrlChanged(Handles, null, IntPtr.Zero);
                }
            }
        }


        /// <summary>
        /// 标题被改变事件
        /// </summary>
        public event EventHandler<TitleChangeEventArgs> OnTitleChange 
        {
            add 
            {
                if (m_titleChangeHandler == null)
                {
                    MBApi.wkeOnTitleChanged(Handles, m_wkeTitleChangedCallback, IntPtr.Zero);
                }
                m_titleChangeHandler += value;
            }
            remove 
            {
                m_titleChangeHandler -= value;
                if (m_titleChangeHandler == null)
                {
                    MBApi.wkeOnTitleChanged(Handles, null, IntPtr.Zero);
                }
            }
        }

        /// <summary>
        /// URL被改变事件
        /// </summary>
        public event EventHandler<UrlChangeEventArgs> OnURLChange 
        {
            add
            {
                if (m_urlChangeHandler == null)
                {
                    MBApi.wkeOnURLChanged2(Handles, m_wkeURLChangedCallback2, IntPtr.Zero);
                }
                m_urlChangeHandler += value;
            }
            remove
            {
                m_urlChangeHandler -= value;
                if (m_urlChangeHandler == null)
                {
                    MBApi.wkeOnURLChanged2(Handles, null, IntPtr.Zero);
                }
            }
        }
        /// <summary>
        /// alert被调用事件
        /// </summary>
        public event EventHandler<AlertBoxEventArgs> OnAlertBox 
        {
            add
            {
                if (m_alertBoxHandler == null)
                {
                    MBApi.wkeOnAlertBox(Handles, m_wkeAlertBoxCallback, IntPtr.Zero);
                }
                m_alertBoxHandler += value;
            }
            remove
            {
                m_alertBoxHandler -= value;
                if (m_alertBoxHandler == null)
                {
                    MBApi.wkeOnAlertBox(Handles, null, IntPtr.Zero);
                }
            }
        }
        /// <summary>
        /// confirm被调用事件
        /// </summary>
        public event EventHandler<ConfirmBoxEventArgs> OnConfirmBox
        {
            add
            {
                if (m_confirmBoxHandler == null)
                {
                    MBApi.wkeOnConfirmBox(Handles, m_wkeConfirmBoxCallback, IntPtr.Zero);
                }
                m_confirmBoxHandler += value;
            }
            remove
            {
                m_confirmBoxHandler -= value;
                if (m_confirmBoxHandler == null)
                {
                    MBApi.wkeOnConfirmBox(Handles, null, IntPtr.Zero);
                }
            }
        }
        /// <summary>
        /// prompt被调用事件
        /// </summary>
        public event EventHandler<PromptBoxEventArgs> OnPromptBox 
        {
            add
            {
                if (m_promptBoxHandler == null)
                {
                    MBApi.wkeOnPromptBox(Handles, m_wkePromptBoxCallback, IntPtr.Zero);
                }
                m_promptBoxHandler += value;
            }
            remove
            {
                m_promptBoxHandler -= value;
                if (m_promptBoxHandler == null)
                {
                    MBApi.wkeOnPromptBox(Handles, null, IntPtr.Zero);
                }
            }
        }
        /// <summary>
        /// 导航事件
        /// </summary>
        public event EventHandler<NavigateEventArgs> OnNavigate 
        {
            add
            {
                if (m_navigateHandler == null)
                {
                    MBApi.wkeOnNavigation(Handles, m_wkeNavigationCallback, IntPtr.Zero);
                }
                m_navigateHandler += value;
            }
            remove
            {
                m_navigateHandler -= value;
                if (m_navigateHandler == null)
                {
                    MBApi.wkeOnNavigation(Handles, null, IntPtr.Zero);
                }
            }
        }
        /// <summary>
        /// 将创建新窗口
        /// </summary>
        public event EventHandler<CreateViewEventArgs> OnCreateView {
            add
            {
                if (m_createViewHandler == null)
                {
                    MBApi.wkeOnCreateView(Handles, m_wkeCreateViewCallback, IntPtr.Zero);
                }
                m_createViewHandler += value;
            }
            remove
            {
                m_createViewHandler -= value;
                if (m_createViewHandler == null)
                {
                    MBApi.wkeOnCreateView(Handles, null, IntPtr.Zero);
                }
            }
        }
        /// <summary>
        /// 文档就绪
        /// </summary>
        public event EventHandler<DocumentReadyEventArgs> OnDocumentReady 
        {
            add
            {
                if (m_documentReadyHandler == null)
                {
                    MBApi.wkeOnDocumentReady2(Handles, m_wkeDocumentReadyCallback, IntPtr.Zero);
                }
                m_documentReadyHandler += value;
            }
            remove
            {
                m_documentReadyHandler -= value;
                if (m_documentReadyHandler == null)
                {
                    MBApi.wkeOnDocumentReady2(Handles, null, IntPtr.Zero);
                }
            }
        }
        /// <summary>
        /// 载入完成
        /// </summary>
        public event EventHandler<LoadingFinishEventArgs> OnLoadingFinish 
        {
            add
            {
                if (m_loadingFinishHandler == null)
                {
                    MBApi.wkeOnLoadingFinish(Handles, m_wkeLoadingFinishCallback, IntPtr.Zero);
                }
                m_loadingFinishHandler += value;
            }
            remove
            {
                m_loadingFinishHandler -= value;
                if (m_loadingFinishHandler == null)
                {
                    MBApi.wkeOnLoadingFinish(Handles, null, IntPtr.Zero);
                }
            }
        }
        /// <summary>
        /// 下载
        /// </summary>
        public event EventHandler<DownloadEventArgs> OnDownload 
        {
            add
            {
                if (m_downloadHandler == null)
                {
                    MBApi.wkeOnDownload(Handles, m_wkeDownloadCallback, IntPtr.Zero);
                }
                m_downloadHandler += value;
            }
            remove
            {
                m_downloadHandler -= value;
                if (m_downloadHandler == null)
                {
                    MBApi.wkeOnDownload(Handles, null, IntPtr.Zero);
                }
            }
        }
        /// <summary>
        /// 控制台
        /// </summary>
        public event EventHandler<ConsoleEventArgs> OnConsole 
        {
            add
            {
                if (m_consoleHandler == null)
                {
                    MBApi.wkeOnConsole(Handles, m_wkeConsoleCallback, IntPtr.Zero);
                }
                m_consoleHandler += value;
            }
            remove
            {
                m_consoleHandler -= value;
                if (m_consoleHandler == null)
                {
                    MBApi.wkeOnConsole(Handles, null, IntPtr.Zero);
                }
            }
        }
        /// <summary>
        /// 开始载入URL
        /// </summary>
        public event EventHandler<LoadUrlBeginEventArgs> OnLoadUrlBegin 
        {
            add
            {
                if (m_loadUrlBeginHandler == null)
                {
                    MBApi.wkeOnLoadUrlBegin(Handles, m_wkeLoadUrlBeginCallback, IntPtr.Zero);
                }
                m_loadUrlBeginHandler += value;
            }
            remove
            {
                m_loadUrlBeginHandler -= value;
                if (m_loadUrlBeginHandler == null)
                {
                    MBApi.wkeOnLoadUrlBegin(Handles, null, IntPtr.Zero);
                }
            }
        }
        /// <summary>
        /// 结束载入URL
        /// </summary>
        public event EventHandler<LoadUrlEndEventArgs> OnLoadUrlEnd
        {
            add
            {
                if (m_loadUrlEndHandler == null)
                {
                    MBApi.wkeOnLoadUrlEnd(Handles, m_wkeLoadUrlEndCallback, IntPtr.Zero);
                }
                m_loadUrlEndHandler += value;
            }
            remove
            {
                m_loadUrlEndHandler -= value;
                if (m_loadUrlEndHandler == null)
                {
                    MBApi.wkeOnLoadUrlEnd(Handles, null, IntPtr.Zero);
                }
            }
        }
        /// <summary>
        /// 脚本上下文已创建
        /// </summary>
        public event EventHandler<DidCreateScriptContextEventArgs> OnDidCreateScriptContext 
        {
            add
            {
                if (m_didCreateScriptContextHandler == null)
                {
                    MBApi.wkeOnDidCreateScriptContext(Handles, m_wkeDidCreateScriptContextCallback, IntPtr.Zero);
                }
                m_didCreateScriptContextHandler += value;
            }
            remove
            {
                m_didCreateScriptContextHandler -= value;
                if (m_didCreateScriptContextHandler == null)
                {
                    MBApi.wkeOnDidCreateScriptContext(Handles, null, IntPtr.Zero);
                }
            }
        }
        /// <summary>
        /// 脚本上下文将释放
        /// </summary>
        public event EventHandler<WillReleaseScriptContextEventArgs> OnWillReleaseScriptContext 
        {
            add
            {
                if (m_willReleaseScriptContextHandler == null)
                {
                    MBApi.wkeOnWillReleaseScriptContext(Handles, m_wkeWillReleaseScriptContextCallback, IntPtr.Zero);
                }
                m_willReleaseScriptContextHandler += value;
            }
            remove
            {
                m_willReleaseScriptContextHandler -= value;
                if (m_willReleaseScriptContextHandler == null)
                {
                    MBApi.wkeOnWillReleaseScriptContext(Handles, null, IntPtr.Zero);
                }
            }
        }

        /// <summary>
        /// 网络响应事件
        /// </summary>
        public event EventHandler<NetResponseEventArgs> OnNetResponse
        {
            add
            {
                if (m_netResponseHandler == null)
                {
                    MBApi.wkeNetOnResponse(Handles, m_wkeNetResponseCallback, IntPtr.Zero);
                }
                m_netResponseHandler += value;
            }
            remove
            {
                m_netResponseHandler -= value;
                if (m_netResponseHandler == null)
                {
                    MBApi.wkeNetOnResponse(Handles, null, IntPtr.Zero);
                }
            }
        }
        /// <summary>
        /// 媒体将被载入事件
        /// </summary>
        public event EventHandler<WillMediaLoadEventArgs> OnWillMediaLoad
        {
            add
            {
                if (m_willMediaLoadHandler == null)
                {
                    MBApi.wkeOnWillMediaLoad(Handles, m_wkeWillMediaLoadCallback, IntPtr.Zero);
                }
                m_willMediaLoadHandler += value;
            }
            remove
            {
                m_willMediaLoadHandler -= value;
                if (m_willMediaLoadHandler == null)
                {
                    MBApi.wkeOnWillMediaLoad(Handles, null, IntPtr.Zero);
                }
            }
        }
        /// <summary>
        /// 其他载入事件
        /// </summary>
        public event EventHandler<OtherLoadEventArgs> OnOtherLoad
        {
            add
            {
                if (m_OtherLoadHandler == null)
                {
                    MBApi.wkeOnOtherLoad(Handles, m_wkeOnOtherLoadCallback, IntPtr.Zero);
                }
                m_OtherLoadHandler += value;
            }
            remove
            {
                m_OtherLoadHandler -= value;
                if (m_OtherLoadHandler == null)
                {
                    MBApi.wkeOnOtherLoad(Handles, null, IntPtr.Zero);
                }
            }
        }


        private void SetEventCallBack()
        {
            m_wkeNetResponseCallback = new wkeNetResponseCallback((IntPtr WebView, IntPtr param, IntPtr url, IntPtr job) => {
                if (m_netResponseHandler != null)
                {
                    NetResponseEventArgs e = new NetResponseEventArgs(WebView, url, job);
                    m_netResponseHandler(this, e);
                    if (e.Cancel)
                        return 1;
                }
                return 0;
            });

            m_wkeTitleChangedCallback = new wkeTitleChangedCallback((IntPtr WebView, IntPtr param, IntPtr title) =>
            {
                if (m_titleChangeHandler != null)
                {
                    m_titleChangeHandler(this, new TitleChangeEventArgs(WebView, title));
                }

            });

            m_wkeMouseOverUrlChangedCallback = new wkeMouseOverUrlChangedCallback((IntPtr WebView, IntPtr param, IntPtr url) =>
            {
                if (m_titleChangeHandler != null)
                {
                    m_titleChangeHandler(this, new TitleChangeEventArgs(WebView, url));
                }

            });


            m_wkeURLChangedCallback2 = new wkeURLChangedCallback2((IntPtr WebView, IntPtr param, IntPtr frame, IntPtr url) =>
            {
                if (m_urlChangeHandler != null)
                {
                    m_urlChangeHandler(this, new UrlChangeEventArgs(WebView, url, frame));
                }

            });

            m_wkeAlertBoxCallback = new wkeAlertBoxCallback((IntPtr WebView, IntPtr param, IntPtr msg) =>
            {
                if (m_alertBoxHandler != null)
                {
                    m_alertBoxHandler(this, new AlertBoxEventArgs(WebView, msg));
                }
            });

            m_wkeConfirmBoxCallback = new wkeConfirmBoxCallback((IntPtr WebView, IntPtr param, IntPtr msg) =>
            {
                if (m_confirmBoxHandler != null)
                {
                    ConfirmBoxEventArgs e = new ConfirmBoxEventArgs(WebView, msg);
                    m_confirmBoxHandler(this, e);
                    return Convert.ToByte(e.Result);
                }
                return 0;
            });

            m_wkePromptBoxCallback = new wkePromptBoxCallback((IntPtr webView, IntPtr param, IntPtr msg, IntPtr defaultResult, IntPtr result) =>
            {
                if (m_promptBoxHandler != null)
                {
                    PromptBoxEventArgs e = new PromptBoxEventArgs(webView, msg, defaultResult, result);
                    m_promptBoxHandler(this, e);
                    return Convert.ToByte(e.Result);
                }
                return 0;
            });

            m_wkeNavigationCallback = new wkeNavigationCallback((IntPtr webView, IntPtr param, wkeNavigationType navigationType, IntPtr url) =>
            {
                if (m_navigateHandler != null)
                {
                    NavigateEventArgs e = new NavigateEventArgs(webView, navigationType, url);
                    m_navigateHandler(this, e);
                    return (byte)(e.Cancel ? 0 : 1);
                }
                return 1;
            });

            m_wkeCreateViewCallback = new wkeCreateViewCallback((IntPtr webView, IntPtr param, wkeNavigationType navigationType, IntPtr url, IntPtr windowFeatures) =>
            {
                if (m_createViewHandler != null)
                {
                    CreateViewEventArgs e = new CreateViewEventArgs(webView, navigationType, url, windowFeatures);
                    m_createViewHandler(this, e);
                    return e.NewWebViewHandle;
                }
                return webView;
            });


            m_wkeDocumentReadyCallback = new wkeDocumentReady2Callback((IntPtr webView, IntPtr param, IntPtr frame) =>
            {
                if (m_documentReadyHandler != null)
                {
                    m_documentReadyHandler(this, new DocumentReadyEventArgs(webView, frame));
                }
            });

            m_wkeLoadingFinishCallback = new wkeLoadingFinishCallback((IntPtr webView, IntPtr param, IntPtr url, wkeLoadingResult result, IntPtr failedReason) =>
            {
                if (m_loadingFinishHandler != null)
                {
                    m_loadingFinishHandler(this, new LoadingFinishEventArgs(webView, url, result, failedReason));
                }
            });

            m_wkeDownloadCallback = new wkeDownloadCallback((IntPtr webView, IntPtr param, IntPtr url) =>
            {
                if (m_downloadHandler != null)
                {
                    DownloadEventArgs e = new DownloadEventArgs(webView, url);
                    m_downloadHandler(this, e);
                    return (byte)(e.Cancel ? 0 : 1);
                }
                return 1;
            });

            m_wkeConsoleCallback = new wkeConsoleCallback((IntPtr webView, IntPtr param, wkeConsoleLevel level, IntPtr message, IntPtr sourceName, uint sourceLine, IntPtr stackTrace) =>
            {
                if (m_consoleHandler != null)
                {
                    m_consoleHandler(this, new ConsoleEventArgs(webView, level, message, sourceName, sourceLine, stackTrace));
                }
            });

            m_wkeLoadUrlBeginCallback = new wkeLoadUrlBeginCallback((IntPtr webView, IntPtr param, IntPtr url, IntPtr job) =>
            {
                if (m_loadUrlBeginHandler != null)
                {
                    LoadUrlBeginEventArgs e = new LoadUrlBeginEventArgs(webView, url, job);
                    m_loadUrlBeginHandler(this, e);
                    return (byte)(e.Cancel ? 1 : 0);
                }
                return 0;
            });

            m_wkeLoadUrlEndCallback = new wkeLoadUrlEndCallback((IntPtr webView, IntPtr param, IntPtr url, IntPtr job, IntPtr buf, int len) =>
            {
                if (m_loadUrlEndHandler != null)
                {
                    LoadUrlEndEventArgs e = new LoadUrlEndEventArgs(webView, url, job, buf, len);
                    m_loadUrlEndHandler(this, e);
                }
            });

            m_wkeDidCreateScriptContextCallback = new wkeDidCreateScriptContextCallback((IntPtr webView, IntPtr param, IntPtr frame, IntPtr context, int extensionGroup, int worldId) =>
            {
                if (m_didCreateScriptContextHandler != null)
                {
                    DidCreateScriptContextEventArgs e = new DidCreateScriptContextEventArgs(webView, frame, context, extensionGroup, worldId);
                    m_didCreateScriptContextHandler(this, e);
                }
            });

            m_wkeWillReleaseScriptContextCallback = new wkeWillReleaseScriptContextCallback((IntPtr webView, IntPtr param, IntPtr frame, IntPtr context, int worldId) =>
            {
                if (m_willReleaseScriptContextHandler != null)
                {
                    WillReleaseScriptContextEventArgs e = new WillReleaseScriptContextEventArgs(webView, frame, context, worldId);
                    m_willReleaseScriptContextHandler(this, e);
                }
            });

            m_wkeWillMediaLoadCallback = new wkeWillMediaLoadCallback((IntPtr webView, IntPtr param, IntPtr url, IntPtr info) => 
            {
                if (m_willMediaLoadHandler != null)
                {
                    WillMediaLoadEventArgs e = new WillMediaLoadEventArgs(webView, url, info);
                    m_willMediaLoadHandler(this, e);
                }
            });

            m_wkeOnOtherLoadCallback = new wkeOnOtherLoadCallback((IntPtr webView, IntPtr param, wkeOtherLoadType type, IntPtr info) =>
            {
                if (m_OtherLoadHandler != null)
                {
                    OtherLoadEventArgs e = new OtherLoadEventArgs(webView, type, info);
                    m_OtherLoadHandler(this, e);
                }
            });

        }


        #endregion

        
        public WebView()
        {
            if (MBApi.wkeIsInitialize() == 0)
            {
                MBApi.wkeInitialize();
            }
            m_wkePaintUpdatedCallback = new wkePaintUpdatedCallback(wkeOnPaintUpdated);
            m_WndProcCallback = new WndProcCallback(OnWndProc);
            this.SetEventCallBack();
            Handles = MBApi.wkeCreateWebView();

        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="window">窗口</param>
        /// <param name="isTransparent">true 表示为分层窗口，窗口必须是顶层</param>
        public WebView(IWin32Window window, bool isTransparent = false)
        {
            if (MBApi.wkeIsInitialize() == 0)
            {
                MBApi.wkeInitialize();
            }
            m_wkePaintUpdatedCallback = new wkePaintUpdatedCallback(wkeOnPaintUpdated);
            m_WndProcCallback = new WndProcCallback(OnWndProc);
            this.SetEventCallBack();
            this.Bind(window, isTransparent);
        }

        public WebView(IntPtr wkeWebView)
        {
            if (MBApi.wkeIsInitialize() == 0)
            {
                MBApi.wkeInitialize();
            }
            m_wkePaintUpdatedCallback = new wkePaintUpdatedCallback(wkeOnPaintUpdated);
            m_WndProcCallback = new WndProcCallback(OnWndProc);
            this.SetEventCallBack();
            Handles = wkeWebView;
            m_noDestory = true;
        }

        #region 实现IDisposable
        /// <summary>
        /// 销毁 WebView
        /// </summary>
        public void Dispose()
        {
            if (Handles != IntPtr.Zero)
            {
                if (m_OldProc != IntPtr.Zero)
                {
                    Help.SetWindowLong(m_hWnd, (int)WinConst.GWL_WNDPROC, m_OldProc.ToInt32());
                    m_OldProc = IntPtr.Zero;
                }
                if (!m_noDestory)
                {
                    MBApi.wkeSetHandle(Handles, IntPtr.Zero);
                    MBApi.wkeDestroyWebView(Handles);
                }
                Handles = IntPtr.Zero;
                m_hWnd = IntPtr.Zero;
                m_noDestory = false;
            }
        }
        #endregion

        protected void wkeOnPaintUpdated(IntPtr webView, IntPtr param, IntPtr hdc, int x, int y, int cx, int cy)
        {
            IntPtr hWnd = param;
            if ((int)WinConst.WS_EX_LAYERED == ((int)WinConst.WS_EX_LAYERED & Help.GetWindowLong(m_hWnd, (int)WinConst.GWL_EXSTYLE)))
            {
                RECT rectDest = new RECT();
                Help.GetWindowRect(m_hWnd, ref rectDest);
                Help.OffsetRect(ref rectDest, -rectDest.Left, -rectDest.Top);

                SIZE sizeDest = new SIZE(rectDest.Right - rectDest.Left, rectDest.Bottom - rectDest.Top);
                //POINT pointDest = new POINT(); // { rectDest.left, rectDest.top };
                POINT pointSource = new POINT();

                BITMAP bmp = new BITMAP();
                IntPtr hBmp = Help.GetCurrentObject(hdc, (int)WinConst.OBJ_BITMAP);
                Help.GetObject(hBmp, Marshal.SizeOf(typeof(BITMAP)), ref bmp);

                sizeDest.cx = bmp.bmWidth;
                sizeDest.cy = bmp.bmHeight;

                IntPtr hdcScreen = Help.GetDC(hWnd);

                BLENDFUNCTION blend = new BLENDFUNCTION();
                blend.BlendOp = (byte)WinConst.AC_SRC_OVER;
                blend.SourceConstantAlpha = 255;
                blend.AlphaFormat = (byte)WinConst.AC_SRC_ALPHA;
                int callOk = Help.UpdateLayeredWindow(m_hWnd, hdcScreen, IntPtr.Zero, ref sizeDest, hdc, ref pointSource, 0, ref blend, (int)WinConst.ULW_ALPHA);
                if (callOk == 0)
                {
                    IntPtr hdcMemory = Help.CreateCompatibleDC(hdcScreen);
                    IntPtr hbmpMemory = Help.CreateCompatibleBitmap(hdcScreen, sizeDest.cx, sizeDest.cy);
                    IntPtr hbmpOld = Help.SelectObject(hdcMemory, hbmpMemory);

                    Help.BitBlt(hdcMemory, 0, 0, sizeDest.cx, sizeDest.cy, hdc, 0, 0, (int)WinConst.SRCCOPY | (int)WinConst.CAPTUREBLT);

                    Help.BitBlt(hdc, 0, 0, sizeDest.cx, sizeDest.cy, hdcMemory, 0, 0, (int)WinConst.SRCCOPY | (int)WinConst.CAPTUREBLT); //!

                    callOk = Help.UpdateLayeredWindow(m_hWnd, hdcScreen, IntPtr.Zero, ref sizeDest, hdcMemory, ref pointSource, 0, ref blend, (int)WinConst.ULW_ALPHA);

                    Help.SelectObject(hdcMemory, hbmpOld);
                    Help.DeleteObject(hbmpMemory);
                    Help.DeleteDC(hdcMemory);
                }

                Help.ReleaseDC(m_hWnd, hdcScreen);
            }
            else
            {
                RECT rc = new RECT(x, y, x + cx, y + cy);
                Help.InvalidateRect(m_hWnd, ref rc, true);
            }

        }
        protected IntPtr OnWndProc(IntPtr hWnd, uint msg, IntPtr wParam, IntPtr lParam)
        {
            if (OnWindowProc != null)
            {
                WindowProcEventArgs e = new WindowProcEventArgs(hWnd, (int)msg, wParam, lParam);
                OnWindowProc(this, e);
                if (e.bHand)
                    return e.Result;
            }

            switch (msg)
            {
                case (uint)WinConst.WM_PAINT:
                    {
                        if ((int)WinConst.WS_EX_LAYERED != ((int)WinConst.WS_EX_LAYERED & (int)Help.GetWindowLong(hWnd, (int)WinConst.GWL_EXSTYLE)))
                        {
                            MBApi.wkeRepaintIfNeeded(Handles);

                            PAINTSTRUCT ps = new PAINTSTRUCT();
                            IntPtr hdc = Help.BeginPaint(hWnd, ref ps);

                            RECT rcClip = ps.rcPaint;

                            RECT rcClient = new RECT();
                            Help.GetClientRect(hWnd, ref rcClient);

                            RECT rcInvalid = rcClient;
                            if (rcClip.Right != rcClip.Left && rcClip.Bottom != rcClip.Top)
                                Help.IntersectRect(ref rcInvalid, ref rcClip, ref rcClient);

                            int srcX = rcInvalid.Left - rcClient.Left;
                            int srcY = rcInvalid.Top - rcClient.Top;
                            int destX = rcInvalid.Left;
                            int destY = rcInvalid.Top;
                            int width = rcInvalid.Right - rcInvalid.Left;
                            int height = rcInvalid.Bottom - rcInvalid.Top;

                            if (0 != width && 0 != height)
                                Help.BitBlt(hdc, destX, destY, width, height, MBApi.wkeGetViewDC(Handles), srcX, srcY, (int)WinConst.SRCCOPY);

                            Help.EndPaint(hWnd, ref ps);
                            return IntPtr.Zero;
                        }
                    }
                    break;
                case (uint)WinConst.WM_ERASEBKGND:
                    return new IntPtr(1);
                case (uint)WinConst.WM_SIZE:
                    {
                        int width = lParam.ToInt32() & 65535;
                        int height = lParam.ToInt32() >> 16;
                        MBApi.wkeResize(Handles, width, height);
                        MBApi.wkeRepaintIfNeeded(Handles);
                    }
                    break;
                case (uint)WinConst.WM_KEYDOWN:
                    {
                        int virtualKeyCode = wParam.ToInt32();
                        uint flags = 0;
                        if (((lParam.ToInt32() >> 16) & (int)WinConst.KF_REPEAT) != 0)
                            flags |= (uint)wkeKeyFlags.WKE_REPEAT;
                        if (((lParam.ToInt32() >> 16) & (int)WinConst.KF_EXTENDED) != 0)
                            flags |= (uint)wkeKeyFlags.WKE_EXTENDED;

                        if (MBApi.wkeFireKeyDownEvent(Handles, virtualKeyCode, flags, false)!=0)
                            return IntPtr.Zero;
                    }
                    break;
                case (uint)WinConst.WM_KEYUP:
                    {
                        int virtualKeyCode = wParam.ToInt32();
                        uint flags = 0;
                        if (((lParam.ToInt32() >> 16) & (int)WinConst.KF_REPEAT) != 0)
                            flags |= (uint)wkeKeyFlags.WKE_REPEAT;
                        if (((lParam.ToInt32() >> 16) & (int)WinConst.KF_EXTENDED) != 0)
                            flags |= (uint)wkeKeyFlags.WKE_EXTENDED;

                        if (MBApi.wkeFireKeyUpEvent(Handles, virtualKeyCode, flags, false) != 0)
                            return IntPtr.Zero;
                    }
                    break;
                case (uint)WinConst.WM_CHAR:
                    {
                        int charCode = wParam.ToInt32();
                        uint flags = 0;
                        if (((lParam.ToInt32() >> 16) & (int)WinConst.KF_REPEAT) != 0)
                            flags |= (uint)wkeKeyFlags.WKE_REPEAT;
                        if (((lParam.ToInt32() >> 16) & (int)WinConst.KF_EXTENDED) != 0)
                            flags |= (uint)wkeKeyFlags.WKE_EXTENDED;

                        if (MBApi.wkeFireKeyPressEvent(Handles, charCode, flags, false) != 0)
                            return IntPtr.Zero;
                    }
                    break;
                case (uint)WinConst.WM_LBUTTONDOWN:
                case (uint)WinConst.WM_MBUTTONDOWN:
                case (uint)WinConst.WM_RBUTTONDOWN:
                case (uint)WinConst.WM_LBUTTONDBLCLK:
                case (uint)WinConst.WM_MBUTTONDBLCLK:
                case (uint)WinConst.WM_RBUTTONDBLCLK:
                case (uint)WinConst.WM_LBUTTONUP:
                case (uint)WinConst.WM_MBUTTONUP:
                case (uint)WinConst.WM_RBUTTONUP:
                case (uint)WinConst.WM_MOUSEMOVE:
                    {
                        if (msg == (uint)WinConst.WM_LBUTTONDOWN || msg == (uint)WinConst.WM_MBUTTONDOWN || msg == (uint)WinConst.WM_RBUTTONDOWN)
                        {
                            if (Help.GetFocus() != hWnd)
                                Help.SetFocus(hWnd);
                            Help.SetCapture(hWnd);
                        }
                        else if (msg == (uint)WinConst.WM_LBUTTONUP || msg == (uint)WinConst.WM_MBUTTONUP || msg == (uint)WinConst.WM_RBUTTONUP)
                        {
                            Help.ReleaseCapture();
                        }

                        int x = Help.LOWORD(lParam);
                        int y = Help.HIWORD(lParam);

                        uint flags = 0;

                        if ((wParam.ToInt32() & (int)WinConst.MK_CONTROL) != 0)
                            flags |= (uint)wkeMouseFlags.WKE_CONTROL;
                        if ((wParam.ToInt32() & (int)WinConst.MK_SHIFT) != 0)
                            flags |= (uint)wkeMouseFlags.WKE_SHIFT;

                        if ((wParam.ToInt32() & (int)WinConst.MK_LBUTTON) != 0)
                            flags |= (uint)wkeMouseFlags.WKE_LBUTTON;
                        if ((wParam.ToInt32() & (int)WinConst.MK_MBUTTON) != 0)
                            flags |= (uint)wkeMouseFlags.WKE_MBUTTON;
                        if ((wParam.ToInt32() & (int)WinConst.MK_RBUTTON) != 0)
                            flags |= (uint)wkeMouseFlags.WKE_RBUTTON;

                        if (MBApi.wkeFireMouseEvent(Handles, msg, x, y, flags) != 0)
                        {
                            //System.Diagnostics.Debug.WriteLine(msg);
                            return IntPtr.Zero;
                        }
                    }
                    break;
                case (uint)WinConst.WM_CONTEXTMENU:
                    {
                        POINT pt;
                        pt.x = Help.LOWORD(lParam);
                        pt.y = Help.HIWORD(lParam);

                        if (pt.x != -1 && pt.y != -1)
                            Help.ScreenToClient(hWnd, ref pt);

                        uint flags = 0;

                        if ((wParam.ToInt32() & (int)WinConst.MK_CONTROL) != 0)
                            flags |= (uint)wkeMouseFlags.WKE_CONTROL;
                        if ((wParam.ToInt32() & (int)WinConst.MK_SHIFT) != 0)
                            flags |= (uint)wkeMouseFlags.WKE_SHIFT;

                        if ((wParam.ToInt32() & (int)WinConst.MK_LBUTTON) != 0)
                            flags |= (uint)wkeMouseFlags.WKE_LBUTTON;
                        if ((wParam.ToInt32() & (int)WinConst.MK_MBUTTON) != 0)
                            flags |= (uint)wkeMouseFlags.WKE_MBUTTON;
                        if ((wParam.ToInt32() & (int)WinConst.MK_RBUTTON) != 0)
                            flags |= (uint)wkeMouseFlags.WKE_RBUTTON;

                        if (MBApi.wkeFireContextMenuEvent(Handles, pt.x, pt.y, flags) != 0)
                            return IntPtr.Zero;

                    }
                    break;
                case (uint)WinConst.WM_MOUSEWHEEL:
                    {
                        POINT pt;
                        pt.x = Help.LOWORD(lParam);
                        pt.y = Help.HIWORD(lParam);
                        Help.ScreenToClient(hWnd, ref pt);

                        int delta = Help.HIWORD(wParam);

                        uint flags = 0;

                        if ((wParam.ToInt32() & (int)WinConst.MK_CONTROL) != 0)
                            flags |= (uint)wkeMouseFlags.WKE_CONTROL;
                        if ((wParam.ToInt32() & (int)WinConst.MK_SHIFT) != 0)
                            flags |= (uint)wkeMouseFlags.WKE_SHIFT;

                        if ((wParam.ToInt32() & (int)WinConst.MK_LBUTTON) != 0)
                            flags |= (uint)wkeMouseFlags.WKE_LBUTTON;
                        if ((wParam.ToInt32() & (int)WinConst.MK_MBUTTON) != 0)
                            flags |= (uint)wkeMouseFlags.WKE_MBUTTON;
                        if ((wParam.ToInt32() & (int)WinConst.MK_RBUTTON) != 0)
                            flags |= (uint)wkeMouseFlags.WKE_RBUTTON;

                        if (MBApi.wkeFireMouseWheelEvent(Handles, pt.x, pt.y, delta, flags) != 0)
                            return IntPtr.Zero;
                        break;
                    }
                case (uint)WinConst.WM_SETFOCUS:
                    MBApi.wkeSetFocus(Handles);
                    return IntPtr.Zero;

                case (uint)WinConst.WM_KILLFOCUS:
                    MBApi.wkeKillFocus(Handles);
                    return IntPtr.Zero;

                case (uint)WinConst.WM_SETCURSOR:
                    if (MBApi.wkeFireWindowsMessage(Handles, hWnd, (uint)WinConst.WM_SETCURSOR, IntPtr.Zero, IntPtr.Zero, IntPtr.Zero) != 0)
                        return IntPtr.Zero;
                    break;

                case (uint)WinConst.WM_IME_STARTCOMPOSITION:
                    {
                        wkeRect caret = MBApi.wkeGetCaretRect(Handles);

                        COMPOSITIONFORM COMPOSITIONFORM = new COMPOSITIONFORM();
                        COMPOSITIONFORM.dwStyle = (int)WinConst.CFS_POINT | (int)WinConst.CFS_FORCE_POSITION;
                        COMPOSITIONFORM.ptCurrentPos.x = caret.x;
                        COMPOSITIONFORM.ptCurrentPos.y = caret.y;

                        IntPtr hIMC = Help.ImmGetContext(hWnd);
                        Help.ImmSetCompositionWindow(hIMC, ref COMPOSITIONFORM);
                        Help.ImmReleaseContext(hWnd, hIMC);
                    }
                    return IntPtr.Zero;
                //case (uint)WinConst.WM_NCDESTROY:
                    //IntPtr ret = Help.CallWindowProc(m_OldProc, hWnd, msg, wParam, lParam);
                    //this.Dispose();
                    //return Help.DefWindowProc(hWnd, msg, wParam, lParam);
                case (uint)WinConst.WM_INPUTLANGCHANGE:
                    return Help.DefWindowProc(hWnd, msg, wParam, lParam);
                   
            }
            return Help.CallWindowProc(m_OldProc, hWnd, msg, wParam, lParam);
        }

        


        #region 静态方法
        /// <summary>
        /// 初始化miniblink，如果没有调用，则在 new WebView 时会自己初始化
        /// </summary>
        public static void wkeInitialize()
        {
            if (MBApi.wkeIsInitialize() == 0)
            {
                MBApi.wkeInitialize();
            }
        }
        /// <summary>
        /// 初始化miniblink，并可以设置一些参数，如果没有调用，则在 new WebView 时会自己初始化
        /// </summary>
        /// <param name="settings"></param>
        public static void wkeInitialize(wkeSettings settings)
        {
            if (MBApi.wkeIsInitialize() == 0)
            {
                MBApi.wkeInitializeEx(settings);
            }
        }
        /// <summary>
        /// 卸载miniblink
        /// </summary>
        public static void wkeFinalize()
        {
            if (MBApi.wkeIsInitialize() != 0)
            {
                MBApi.wkeFinalize();
            }
        }
        /// <summary>
        /// 获取版本
        /// </summary>
        public static uint Version
        {
            get { return MBApi.wkeGetVersion(); }
        }
        /// <summary>
        /// 获取版本信息
        /// </summary>
        public static string VersionString
        {
            get
            {
                IntPtr utf8 = MBApi.wkeGetVersionString();
                if (utf8 != IntPtr.Zero)
                {
                    return Marshal.PtrToStringAnsi(utf8);
                }
                return null;
            }
        }
        /// <summary>
        /// 设置代理
        /// </summary>
        /// <param name="proxy"></param>
        public static void SetProxy(wkeProxy proxy)
        {
            MBApi.wkeSetProxy(ref proxy);
        }
        /// <summary>
        /// 判断是否主框架
        /// </summary>
        /// <param name="WebFrame">框架句柄</param>
        /// <returns></returns>
        public static bool FrameIsMainFrame(IntPtr WebFrame)
        {
            return MBApi.wkeIsMainFrame(WebFrame) != 0;
        }
        /// <summary>
        /// 判断是否远程框架
        /// </summary>
        /// <param name="WebFrame"></param>
        /// <returns></returns>
        public static bool FrameIsRemoteFrame(IntPtr WebFrame)
        {
            return MBApi.wkeIsWebRemoteFrame(WebFrame) != 0;
        }
        /// <summary>
        /// 获取v8Context
        /// </summary>
        /// <param name="WebFrame">框架句柄</param>
        /// <returns></returns>
        public static IntPtr FrameGetMainWorldScriptContext(IntPtr WebFrame)
        {
            IntPtr v8ContextPtr = IntPtr.Zero;
            MBApi.wkeWebFrameGetMainWorldScriptContext(WebFrame,ref v8ContextPtr);
            return v8ContextPtr;
        }

        /// <summary>
        /// 获取v8Isolate
        /// </summary>
        /// <returns></returns>
        public static IntPtr GetBlinkMainThreadIsolate()
        {
            return MBApi.wkeGetBlinkMainThreadIsolate();
        }

        /// <summary>
        /// 设置mimeType。此方法应该在 OnLoadUrlBegin 事件中使用
        /// </summary>
        /// <param name="job"></param>
        /// <param name="type">MIMEType</param>
        public static void NetSetMIMEType(IntPtr job, string MIMEType)
        {
            MBApi.wkeNetSetMIMEType(job, MIMEType);
        }
        /// <summary>
        /// 获取mimeType。此方法应该在 OnNetResponse 事件中使用
        /// </summary>
        /// <param name="job"></param>
        /// <returns></returns>
        public static string NetGetMIMEType(IntPtr job)
        {
            IntPtr mime = MBApi.wkeCreateStringW(null, 0);
            if (mime == IntPtr.Zero)
            {
                return string.Empty;
            }
            MBApi.wkeNetGetMIMEType(job, mime);
            IntPtr pStr = MBApi.wkeGetStringW(mime);
            string mimeType = Help.PtrToStringUTF8(pStr);
            MBApi.wkeDeleteString(mime);
            return mimeType;

        }
        /// <summary>
        /// 获取HTTP请求头
        /// </summary>
        /// <param name="job"></param>
        /// <param name="key"></param>
        public string NetGetHTTPHeaderField(IntPtr job, string key)
        {
            return Help.PtrToStringUTF8(MBApi.wkeNetGetHTTPHeaderField(job, key));
        }

        /// <summary>
        /// 获取HTTP响应头
        /// </summary>
        /// <param name="job"></param>
        /// <param name="key"></param>
        public string NetGetHTTPHeaderFieldFromResponse(IntPtr job, string key)
        {
            return Help.PtrToStringUTF8(MBApi.wkeNetGetHTTPHeaderFieldFromResponse(job, key));
        }
        /// <summary>
        /// 获取请求方法
        /// </summary>
        /// <param name="job"></param>
        /// <returns></returns>
        public wkeRequestType GetRequestMethod(IntPtr job)
        {
            return MBApi.wkeNetGetRequestMethod(job);
        }
        /// <summary>
        /// 获取此请求中的post数据，注意：只有当请求是post时才可获取到，get请求直接从url上取就好了。
        /// </summary>
        /// <param name="job"></param>
        /// <returns></returns>
        public wkePostBodyElements NetGetPostBody(IntPtr job)
        {
            IntPtr ptr = MBApi.wkeNetGetPostBody(job);
            return (wkePostBodyElements)  Marshal.PtrToStructure(ptr,typeof(wkePostBodyElements));
        }
        /// <summary>
        /// 获取Raw格式的HTTP响应数据，wkeSlist是个保存了网络数据的链表结构，请参看wke.h文件。
        /// </summary>
        /// <param name="job"></param>
        /// <returns></returns>
        public wkeSlist NetGetRawResponseHead(IntPtr job)
        {
            IntPtr ptr = MBApi.wkeNetGetRawResponseHead(job);
            return (wkeSlist)Marshal.PtrToStructure(ptr, typeof(wkeSlist));
        }
        /// <summary>
        /// 取消本次网络请求，需要在wkeOnLoadUrlBegin里调用。
        /// </summary>
        /// <param name="job"></param>
        public void NetCancelRequest(IntPtr job)
        {
            MBApi.wkeNetCancelRequest(job);
        }

        /// <summary>
        /// 创建一组post数据内容
        /// </summary>
        /// <param name="job"></param>
        /// <param name="length"></param>
        /// <returns></returns>
        public wkePostBodyElements NetCreatePostBodyElements(long length)
        {
            IntPtr ptr = MBApi.wkeNetCreatePostBodyElements(Handle, length);
            return (wkePostBodyElements)Marshal.PtrToStructure(ptr, typeof(wkePostBodyElements));
        }
        /// <summary>
        /// 干掉指定的post数据内容组，释放内存。
        /// </summary>
        /// <param name="elements"></param>
        /// <returns></returns>
        public void NetFreePostBodyElements(IntPtr elements)
        {
            MBApi.wkeNetFreePostBodyElements(elements);
        }

        /// <summary>
        /// 创建一个post数据内容。
        /// </summary>
        /// <param name="webView"></param>
        /// <returns></returns>
        public wkePostBodyElement NetCreatePostBodyElement()
        {
            IntPtr ptr = MBApi.wkeNetCreatePostBodyElement(Handle);
            return (wkePostBodyElement)  Marshal.PtrToStructure( ptr,typeof(wkePostBodyElement));
        }
        /// <summary>
        /// 干掉指定的post数据内容，释放内存。
        /// </summary>
        /// <param name="elements"></param>
        public void NetFreePostBodyElement(IntPtr element)
        {
            MBApi.wkeNetFreePostBodyElement(element);
        }
        /// <summary>
        /// 获取Raw格式的HTTP请求数据，wkeSlist是个保存了网络数据的链表结构，请参看wke.h文件。
        /// </summary>
        /// <param name="job"></param>
        public wkeSlist NetGetRawHttpHead(IntPtr job)
        {

            IntPtr ptr = MBApi.wkeNetGetRawHttpHead(job);
            return (wkeSlist) Marshal.PtrToStructure(ptr, typeof(wkeSlist));

        }

        /// <summary>
        /// 网络访问经常存在异步操作，当wkeOnLoadUrlBegin里拦截到一个请求后如不能马上判断出结果，此时可以调用本接口，处理完成后需调用wkeNetContinueJob来让此请求继续。
        /// </summary>
        /// <param name="job"></param>
        /// <returns></returns>
        public bool NetHoldJobToAsynCommit(IntPtr job)
        {
            return MBApi.wkeNetHoldJobToAsynCommit(job) == 1 ? true : false;
        }

        /// <summary>
        /// 修改当前请求的url。
        /// </summary>
        /// <param name="job"></param>
        /// <param name="url"></param>
        /// <returns></returns>
        public static bool NetChangeRequestUrl(IntPtr job, string url)
        {
            return MBApi.wkeNetChangeRequestUrl(job, url) == 1 ? true : false;
        }
        /// <summary>
        /// 继续执行中断的网络任务，参看wkeNetHoldJobToAsynCommit接口。
        /// </summary>
        /// <param name="job"></param>
        public void NetContinueJob(IntPtr job)
        {
            MBApi.wkeNetContinueJob(job);
        }

        /// <summary>
        /// 设置HTTP头字段。此方法应该在 OnLoadUrlBegin 事件中使用
        /// </summary>
        /// <param name="job"></param>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <param name="response"></param>
        public static void NetSetHTTPHeaderField(IntPtr job, string key, string value, bool response)
        {
            MBApi.wkeNetSetHTTPHeaderField(job, key, value, response);
        }
        /// <summary>
        /// 设置URL。此方法应该在 OnLoadUrlBegin 事件中使用
        /// </summary>
        /// <param name="job"></param>
        /// <param name="URL"></param>
        public static void NetSetURL(IntPtr job, string URL)
        {
            MBApi.wkeNetSetURL(job, URL);
        }
        /// <summary>
        /// 设置网络数据。此方法应该在 OnLoadUrlBegin 事件中使用
        /// </summary>
        /// <param name="job"></param>
        /// <param name="data"></param>
        public static void NetSetData(IntPtr job, byte[] data)
        {
            MBApi.wkeNetSetData(job, data, data.Length);
        }
        /// <summary>
        /// 设置网络数据。此方法应该在 OnLoadUrlBegin 事件中使用
        /// </summary>
        /// <param name="job"></param>
        /// <param name="str">string数据</param>
        public static void NetSetData(IntPtr job, string str)
        {
            byte[] data = Encoding.UTF8.GetBytes(str);
            MBApi.wkeNetSetData(job, data, data.Length);
        }
        /// <summary>
        /// 设置网络数据。此方法应该在 OnLoadUrlBegin 事件中使用
        /// </summary>
        /// <param name="job"></param>
        /// <param name="png">PNG图片数据</param>
        public static void NetSetData(IntPtr job, System.Drawing.Image png)
        {
            byte[] data = null;
            using (MemoryStream ms = new MemoryStream())
            {
                png.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
                data = ms.GetBuffer();
            }
            MBApi.wkeNetSetData(job, data, data.Length);
        }
        /// <summary>
        /// 设置网络数据。此方法应该在 OnLoadUrlBegin 事件中使用
        /// </summary>
        /// <param name="job"></param>
        /// <param name="img">图片数据</param>
        /// <param name="fmt">图片格式</param>
        public static void NetSetData(IntPtr job, System.Drawing.Image img, System.Drawing.Imaging.ImageFormat fmt)
        {
            byte[] data = null;
            using (MemoryStream ms = new MemoryStream())
            {
                img.Save(ms, fmt);
                data = ms.GetBuffer();
            }
            MBApi.wkeNetSetData(job, data, data.Length);
        }


        /// <summary>
        /// 此方法应该在 OnLoadUrlBegin 事件中使用，调用此函数后,网络层收到数据会存储在一buf内,接收数据完成后响应OnLoadUrlEnd事件.#此调用严重影响性能,慎用，
        /// 此函数和WebView.NetSetData的区别是，WebView.NetHookRequest会在接受到真正网络数据后再调用回调，并允许回调修改网络数据。
        /// 而WebView.NetSetData是在网络数据还没发送的时候修改
        /// </summary>
        /// <param name="job"></param>
        public void NetHookRequest(IntPtr job)
        {
            MBApi.wkeNetHookRequest(job);
        }

        public static int NetGetFavicon(IntPtr WebView, wkeNetResponseCallback Callback, IntPtr param)
        {
            return MBApi.wkeNetGetFavicon(WebView, Callback, param);
        }
        

        /// <summary>
        /// 指定一个回调函数，访问所有Cookie
        /// </summary>
        /// <param name="visitor">wkeCookieVisitor 委托</param>
        /// <param name="userData">用户数据</param>
        public void VisitAllCookie(wkeCookieVisitor visitor, IntPtr userData)
        {
            MBApi.wkeVisitAllCookie(Handles, userData, visitor);
        }


        /// <summary>
        /// 执行cookie命令，清空等操作
        /// </summary>
        /// <param name="command"></param>
        public void PerformCookieCommand(wkeCookieCommand command)
        {
            MBApi.wkePerformCookieCommand(Handles, command);
        }

        /// <summary>
        /// 当你载入的不是URL，则可能需要设置文件系统，来自定义读取这些文件的方法
        /// </summary>
        /// <param name="fs">可继承自此类，且必须重写此类的所有 virtual 方法</param>
        public static void SetFileSystem(FileSystem fs)
        {
            if (fs != null)
            {
                MBApi.wkeSetFileSystem(fs.m_fileOpen, fs.m_fileClose, fs.m_fileSize, fs.m_fileRead, fs.m_fileSeek);
            }
            else 
            {
                MBApi.wkeSetFileSystem(null, null, null, null, null);
            }
        }



        #endregion

        #region 方法

        /// <summary>
        /// 绑定指定窗口
        /// </summary>
        /// <param name="window">窗口</param>
        /// <param name="isTransparent">是否透明模式，必须是顶层窗口才有效</param>
        /// <returns></returns>
        public bool Bind(IWin32Window window, bool isTransparent = false)
        {
            if (m_hWnd == window.Handle)
            {
                return true;
            }

            if (Handles == IntPtr.Zero)
            {
                Handles = MBApi.wkeCreateWebView();
                if (Handles == IntPtr.Zero)
                {
                    return false;
                }
            }
            m_hWnd = window.Handle;

            MBApi.wkeSetHandle(Handles, m_hWnd);
            MBApi.wkeOnPaintUpdated(Handles, m_wkePaintUpdatedCallback, m_hWnd);

            if (isTransparent)
            {
                MBApi.wkeSetTransparent(Handles, true);
                int exStyle = Help.GetWindowLong(m_hWnd, (int)WinConst.GWL_EXSTYLE);
                Help.SetWindowLong(m_hWnd, (int)WinConst.GWL_EXSTYLE, exStyle | (int)WinConst.WS_EX_LAYERED);
            }
            else
            {
                MBApi.wkeSetTransparent(Handles, false);
            }
            m_OldProc = Help.GetWindowLongIntPtr(m_hWnd, (int)WinConst.GWL_WNDPROC);
            if (m_OldProc != Marshal.GetFunctionPointerForDelegate(m_WndProcCallback))
            {
                m_OldProc = Help.SetWindowLongDelegate(m_hWnd, (int)WinConst.GWL_WNDPROC, m_WndProcCallback);
            }
            RECT rc = new RECT();
            Help.GetClientRect(m_hWnd, ref rc);
            MBApi.wkeResize(Handles, rc.Right - rc.Left, rc.Bottom - rc.Top);
            
            return true;
        }

        /// <summary>
        /// 载入URL
        /// </summary>
        /// <param name="URL"></param>
        public void Load(string URL)
        {
            MBApi.wkeLoadW(Handles, URL);
        }

        /// <summary>
        /// 载入URL
        /// </summary>
        /// <param name="URL"></param>
        public void LoadURL(string URL)
        {
            MBApi.wkeLoadURLW(Handles, URL);
        }
        /// <summary>
        /// 载入本地文件
        /// </summary>
        /// <param name="FileName">文件名</param>
        public void LoadFile(string FileName)
        {
            MBApi.wkeLoadFileW(Handles, FileName);
        }
        /// <summary>
        /// 载入内存HTML文本
        /// </summary>
        /// <param name="html">HTML文本</param>
        public void LoadHTML(string Html)
        {
            MBApi.wkeLoadHTMLW(Handles, Html);
        }
        /// <summary>
        /// POST方式载入URL
        /// </summary>
        /// <param name="URL"></param>
        /// <param name="PostData">提交的POST数据</param>
        public void PostURL(string URL, byte[] PostData)
        {
            MBApi.wkePostURLW(Handles, URL, PostData, PostData.Length);
        }
        /// <summary>
        /// 载入内存HTML文本，并指定 BaseURL
        /// </summary>
        /// <param name="Html">HTML文本</param>
        /// <param name="BaseURL"></param>
        public void LoadHtmlWithBaseUrl(string Html, string BaseURL)
        {
            MBApi.wkeLoadHtmlWithBaseUrl(Handles, Encoding.UTF8.GetBytes(Html), Encoding.UTF8.GetBytes(BaseURL));
        }

        /// <summary>
        /// 停止载入
        /// </summary>
        public void StopLoading()
        {
            MBApi.wkeStopLoading(Handles);
        }
        /// <summary>
        /// 重新载入
        /// </summary>
        public void Reload()
        {
            MBApi.wkeReload(Handles);
        }
        /// <summary>
        /// 跳转到指定偏移的浏览历史
        /// </summary>
        /// <param name="offset"></param>
        public void GoToOffset(int offset)
        {
            MBApi.wkeGoToOffset(Handles, offset);
        }
        /// <summary>
        /// 跳转到指定索引的浏览历史
        /// </summary>
        /// <param name="index"></param>
        public void GoToIndex(int index)
        {
            MBApi.wkeGoToIndex(Handles, index);
        }
        /// <summary>
        /// 获取Webview的ID
        /// </summary>
        /// <returns></returns>
        public int GetId()
        {
            return MBApi.wkeGetWebviewId(Handles);
        }
        /// <summary>
        /// 根据Webview的ID判断是否活动Webview
        /// </summary>
        /// <param name="webViewId"></param>
        /// <returns></returns>
        public bool IsWebviewAlive(int webViewId)
        {
            return MBApi.wkeIsWebviewAlive(Handles, webViewId) != 0;
        }

        /// <summary>
        /// 获取文档完成URL
        /// </summary>
        /// <param name="frameId">框架ID</param>
        /// <param name="partialURL"></param>
        /// <returns></returns>
        public string GetDocumentCompleteURL(IntPtr frameId, string partialURL)
        {
            byte[] utf8 = Encoding.UTF8.GetBytes(partialURL);
            return Help.PtrToStringUTF8(MBApi.wkeGetDocumentCompleteURL(Handles, frameId, utf8));
        }

        /// <summary>
        /// 获取当前URL
        /// </summary>
        public string GetURL()
        {
            IntPtr pUrl = MBApi.wkeGetURL(Handles);
            if (pUrl != IntPtr.Zero)
                return Help.PtrToStringUTF8(pUrl);
            return string.Empty;
        }
        /// <summary>
        /// 获取指定框架的URL
        /// </summary>
        /// <param name="FrameId">框架ID</param>
        /// <returns></returns>
        public string GetFrameURL(IntPtr FrameId)
        {
            IntPtr pUrl = MBApi.wkeGetFrameUrl(Handles, FrameId);
            if (pUrl != IntPtr.Zero)
                return Help.PtrToStringUTF8(pUrl);
            return string.Empty;
        }

        /// <summary>
        /// 垃圾回收
        /// </summary>
        /// <param name="delayMs">延迟的毫秒数</param>
        public void GC(int delayMs)
        {
            MBApi.wkeGC(Handles, delayMs);
        }

        /// <summary>
        /// 设置当前WebView的代理
        /// </summary>
        /// <param name="proxy"></param>
        public void SetViewProxy(wkeProxy proxy)
        {
            MBApi.wkeSetViewProxy(Handles, ref proxy);
        }
        /// <summary>
        /// 休眠
        /// </summary>
        public void Sleep()
        {
            MBApi.wkeSleep(Handles);
        }
        /// <summary>
        /// 唤醒
        /// </summary>
        public void Wake()
        {
            MBApi.wkeWake(Handles);
        }
        /// <summary>
        /// 设置UserAgent
        /// </summary>
        /// <param name="UserAgent"></param>
        public void SetUserAgent(string UserAgent)
        {
            MBApi.wkeSetUserAgentW(Handles, UserAgent);
        }
        /// <summary>
        /// 获取UserAgent
        /// </summary>
        /// <returns></returns>
        public string GetUserAgent()
        {
            IntPtr pstr = MBApi.wkeGetUserAgent(Handles);
            if (pstr != IntPtr.Zero)
            {
                return Help.PtrToStringUTF8(pstr);
            }
            return string.Empty;
        }

        /// <summary>
        /// 后退
        /// </summary>
        public bool GoBack()
        {
            return MBApi.wkeGoBack(Handles) != 0;
        }
        /// <summary>
        /// 前进
        /// </summary>
        public bool GoForward()
        {
            return MBApi.wkeGoForward(Handles) != 0;
        }
        /// <summary>
        /// 全选
        /// </summary>
        public void EditorSelectAll()
        {
            MBApi.wkeEditorSelectAll(Handles);
        }
        /// <summary>
        /// 取消选择
        /// </summary>
        public void EditorUnSelect()
        {
            MBApi.wkeEditorUnSelect(Handles);
        }
        /// <summary>
        /// 复制
        /// </summary>
        public void EditorCopy()
        {
            MBApi.wkeEditorCopy(Handles);
        }
        /// <summary>
        /// 剪切
        /// </summary>
        public void EditorCut()
        {
            MBApi.wkeEditorCut(Handles);
        }
        /// <summary>
        /// 粘贴
        /// </summary>
        public void EditorPaste()
        {
            MBApi.wkeEditorPaste(Handles);
        }
        /// <summary>
        /// 删除
        /// </summary>
        public void EditorDelete()
        {
            MBApi.wkeEditorDelete(Handles);
        }
        /// <summary>
        /// 撤销
        /// </summary>
        public void EditorUndo()
        {
            MBApi.wkeEditorUndo(Handles);
        }
        /// <summary>
        /// 重做
        /// </summary>
        public void EditorRedo()
        {
            MBApi.wkeEditorRedo(Handles);
        }
        /// <summary>
        /// 获取Cookie
        /// </summary>
        /// <returns></returns>
        public string GetCookie()
        {
            IntPtr pStr = MBApi.wkeGetCookieW(Handles);
            if (pStr != IntPtr.Zero)
            {
                return Marshal.PtrToStringUni(pStr);
            }
            return string.Empty;
        }
        /// <summary>
        /// 设置Cookie
        /// </summary>
        /// <param name="Url"></param>
        /// <param name="Cookie">cookie格式必须是:Set-cookie: PRODUCTINFO=webxpress; domain=.fidelity.com; path=/; secure</param>
        public void SetCookie(string Url, string Cookie)
        {
            byte[] url = Encoding.UTF8.GetBytes(Url);
            byte[] cookie = Encoding.UTF8.GetBytes(Cookie);
            MBApi.wkeSetCookie(Handles, url, cookie);
        }


        /// <summary>
        /// 设置Cookie目录
        /// </summary>
        /// <param name="Path"></param>
        public void SetCookieJarPath(string Path)
        {
            MBApi.wkeSetCookieJarPath(Handles, Path);
        }
        /// <summary>
        /// 设置Cookie全路径，包含文件名
        /// </summary>
        /// <param name="FileName"></param>
        public void SetCookieJarFullPath(string FileName)
        {
            MBApi.wkeSetCookieJarFullPath(Handles, FileName);
        }
        /// <summary>
        /// 设置 LocalStorage 目录
        /// </summary>
        /// <param name="Path"></param>
        public void SetLocalStorageFullPath(string Path)
        {
            MBApi.wkeSetLocalStorageFullPath(Handles, Path);
        }
        /// <summary>
        /// 添加插件目录
        /// </summary>
        /// <param name="Path"></param>
        public void AddPluginDirectory(string Path)
        {
            MBApi.wkeAddPluginDirectory(Handles, Path);
        }

        /// <summary>
        /// 获得焦点
        /// </summary>
        public void SetFocus()
        {
            MBApi.wkeSetFocus(Handles);
        }
        /// <summary>
        /// 失去焦点
        /// </summary>
        public void KillFocus()
        {
            MBApi.wkeKillFocus(Handles);
        }
        /// <summary>
        /// 运行JS
        /// </summary>
        /// <param name="JavaScript">脚本,如果需要返回值，则需要 return</param>
        /// <returns></returns>
        public object RunJS(string JavaScript)
        {
            long jsValue = MBApi.wkeRunJSW(Handle, JavaScript);
            return ToNetValue(jsValue);
        }
        public object CallJsFunc(string strFuncName, params object[] param)
        {
            IntPtr es = MBApi.wkeGlobalExec(Handle);
            var args = param.Select(arg => ToJsValue(arg)).ToArray();
            long jsValue = MBApi.jsCall(es, MBApi.jsGetGlobal(es, strFuncName), MBApi.jsUndefined(), args, args.Length);

            return ToNetValue(jsValue);
        }

        /// <summary>
        /// 全局执行
        /// </summary>
        /// <returns>返回全局 jsExecState</returns>
        public IntPtr GlobalExec()
        {
            return MBApi.wkeGlobalExec(Handles);
        }
        /// <summary>
        /// 获取指定框架jsExecState
        /// </summary>
        /// <param name="FrameId">框架ID</param>
        /// <returns>返回 jsExecState</returns>
        public IntPtr GetGlobalExecByFrame(IntPtr FrameId)
        {
            return MBApi.wkeGetGlobalExecByFrame(Handles, FrameId);
        }

        /// <summary>
        /// 启用或禁用编辑模式
        /// </summary>
        /// <param name="editable"></param>
        public void SetEditable(bool editable)
        {
            MBApi.wkeSetEditable(Handles, editable);
        }
        /// <summary>
        /// 启用或禁用跨域检查
        /// </summary>
        /// <param name="enable"></param>
        public void SetCspCheckEnable(bool enable)
        {
            MBApi.wkeSetCspCheckEnable(Handles, enable);
        }

        /// <summary>
        /// 设置网络接口
        /// </summary>
        /// <param name="NetInterface"></param>
        public void SetNetInterface(string NetInterface)
        {
            MBApi.wkeSetViewNetInterface(Handles, NetInterface);
        }
        /// <summary>
        /// 截图
        /// </summary>
        /// <returns></returns>
        public Bitmap PrintToBitmap()
        {
            if (Handles == IntPtr.Zero)
                return null;
            MBApi.wkeRunJSW(Handles, @"document.body.style.overflow='hidden'");
            MBApi.wkeRepaintIfNeeded(Handles);
            int w = MBApi.wkeGetContentWidth(Handles);
            int h = MBApi.wkeGetContentHeight(Handles);

            int oldwidth = MBApi.wkeGetWidth(Handles);
            int oldheight = MBApi.wkeGetHeight(Handles);

            MBApi.wkeResize(Handles, w, h);

            Bitmap bmp = new Bitmap(w, h);
            Rectangle rc = new Rectangle(0, 0, w, h);
            System.Drawing.Imaging.BitmapData data = bmp.LockBits(rc, System.Drawing.Imaging.ImageLockMode.ReadWrite, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
            MBApi.wkePaint(Handles, data.Scan0, 0);
            bmp.UnlockBits(data);

            MBApi.wkeResize(Handles, oldwidth, oldheight);
            MBApi.wkeRunJSW(Handles, @"document.body.style.overflow='visible'");

            return bmp;
        }
        /// <summary>
        /// 设置调试配置
        /// </summary>
        /// <param name="debugString">"showDevTools" 是开启devtools功能，参数为：front_end/inspector.html(utf8编码)</param>
        /// <param name="param"></param>
        public void SetDebugConfig(string debugString, string param)
        {
            MBApi.wkeSetDebugConfig(Handles, debugString, Encoding.UTF8.GetBytes(param));
        }

        /// <summary>
        /// 获取调试配置
        /// </summary>
        /// <param name="debugString"></param>
        /// <returns></returns>
        public string GetDebugConfig(string debugString)
        {
            return Help.PtrToStringUTF8(MBApi.wkeGetDebugConfig(Handles, debugString));
        }

        /// <summary>
        /// 设置上下文菜单项目是否显示
        /// </summary>
        /// <param name="item"></param>
        /// <param name="isShow"></param>
        public void SetContextMenuItemShow(wkeMenuItemId item, bool isShow)
        {
            MBApi.wkeSetContextMenuItemShow(Handles, item, isShow);
        }

        /// <summary>
        /// 设置语言
        /// </summary>
        /// <param name="language"></param>
        public void SetLanguage(string language)
        {
            MBApi.wkeSetLanguage(Handles, language);
        }

        /// <summary>
        /// 设置设备参数
        /// </summary>
        /// <param name="device">如：“navigator.platform”</param>
        /// <param name="paramStr"></param>
        /// <param name="paramInt"></param>
        /// <param name="paramFloat"></param>
        public void SetDeviceParameter(string device, string paramStr, int paramInt = 0, float paramFloat = 0)
        {
            MBApi.wkeSetDeviceParameter(Handles, device, paramStr, paramInt, paramFloat);
        }

        /// <summary>
        /// 显示DevTools窗口
        /// </summary>
        /// <param name="Path">路径</param>
        /// <param name="Callback"></param>
        public void ShowDevtools(string Path, wkeOnShowDevtoolsCallback Callback, IntPtr Param)
        {
            MBApi.wkeShowDevtools(Handles, Path, Callback, Param);
        }

        /// <summary>
        /// 删除将发送请求的信息
        /// </summary>
        /// <param name="WillSendRequestInfoPtr"></param>
        public void DeleteWillSendRequestInfo(IntPtr WillSendRequestInfoPtr)
        {
            MBApi.wkeDeleteWillSendRequestInfo(Handles, WillSendRequestInfoPtr);
        }
        /// <summary>
        /// 在指定框架插入CSS
        /// </summary>
        /// <param name="FrameId">框架ID</param>
        /// <param name="cssText"></param>
        public void InsertCSSByFrame(IntPtr FrameId, string cssText)
        {
            byte[] utf8 = Encoding.UTF8.GetBytes(cssText);
            MBApi.wkeInsertCSSByFrame(Handles, FrameId, utf8);
        }

        /// <summary>
        /// 序列化到MHTML
        /// </summary>
        /// <returns></returns>
        public string SerializeToMHTML()
        {
            return Help.PtrToStringUTF8(MBApi.wkeUtilSerializeToMHTML(Handles));
        }
        /// <summary>
        /// 获取源码
        /// </summary>
        /// <returns></returns>
        public string GetSource()
        {
            return Help.PtrToStringUTF8(MBApi.wkeGetSource(Handles));
        }
        /// <summary>
        /// 设置视图配置，可设置尺寸和背景色
        /// </summary>
        /// <param name="settings"></param>
        public void SetViewSettings(wkeViewSettings settings)
        {
            MBApi.wkeSetViewSettings(Handles, settings);
        }

        /// <summary>
        /// 在c#中封装一个js函数，用于下面的类型转换
        /// </summary>
        public class JsFunc
        {
            private string m_strName = null;
            private WebView Handles;

            public JsFunc(WebView webView, long jsValue)
            {
                Handles = webView;
                IntPtr es = MBApi.wkeGlobalExec(Handles.Handle);
                m_strName = "func" + Guid.NewGuid().ToString().Replace("-", "");

                MBApi.jsSetGlobal(es, m_strName, jsValue);
            }

            public object Invoke(params object[] param)
            {
                IntPtr es = MBApi.wkeGlobalExec(Handles.Handle);
                long value = MBApi.jsGetGlobal(es, m_strName);

                long[] jsps = param.Select(i => Handles.ToJsValue(i)).ToArray();
                object result = Handles.ToNetValue(MBApi.jsCall(es, value, MBApi.jsUndefined(), jsps, jsps.Length));

                MBApi.jsSetGlobal(es, m_strName, MBApi.jsUndefined());

                return result;
            }
        }
        public jsType GetValueType(long value)
        {
            jsType type = MBApi.jsTypeOf(value);
            return type;
        }

        /// <summary>
        /// 将js值转成c#值
        /// </summary>
        /// <param name="value"></param>
        /// <param name="bFunExecute">指定如果是函数是否执行该函数，如果执行，则返回函数执行的结果而不是函数本身</param>
        /// <returns></returns>
        public object ToNetValue(long value, bool bFunExecute = false)
        {
            jsType type = MBApi.jsTypeOf(value);
            IntPtr es = MBApi.wkeGlobalExec(Handles);

            switch (type)
            {
                case jsType.NULL:
                case jsType.UNDEFINED:
                    {
                        return null;
                    }

                case jsType.NUMBER:
                    {
                        return MBApi.jsToDouble(es, value);
                    }

                case jsType.BOOLEAN:
                    {
                        return MBApi.jsToBoolean(es, value);
                    }

                case jsType.STRING:
                    {
                        return Marshal.PtrToStringUni( MBApi.jsToTempStringW(es, value));
                    }

                case jsType.FUNCTION:
                    {
                        JsFunc function = new JsFunc(this, value);
                        if (bFunExecute)
                        {
                            return function.Invoke();    // 如果有参数一般是从另一个jsType传过来，具体情况具体分析
                        }

                        return function;
                    }

                case jsType.ARRAY:
                    {
                        int len = MBApi.jsGetLength(es, value);
                        object[] array = new object[len];
                        for (int i = 0; i < array.Length; i++)
                        {
                            array[i] = ToNetValue(MBApi.jsGetAt(es, value, i));
                        }

                        return array;
                    }

                case jsType.OBJECT:
                    {
                        IntPtr ptr = MBApi.jsGetKeys(es, value);
                        if (ptr == IntPtr.Zero)
                            return null;
                        jsKeys keys = (jsKeys)Marshal.PtrToStructure(ptr, typeof(jsKeys));
                        string[] strKeyArr =Help.PtrToStringArray(keys.keys, keys.length) ;

                        ExpandoObject exp = new ExpandoObject();
                        IDictionary<string, object> map = (IDictionary<string, object>)exp;
                        foreach (string key in strKeyArr)
                        {
                            map.Add(key, ToNetValue(MBApi.jsGet(es, value, key)));
                        }

                        return map;
                    }

                default:
                    {
                        throw new NotSupportedException();
                    }
            }
        }

        // 定义一个通用的函数形式
        public delegate object CommonFuncType(object[] param);

        /// <summary>
        /// 将c#值转成js值
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="bFunExecute">指定如果是函数是否执行该函数，如果执行则返回函数执行的结果，如果不执行则返回的是函数的参数，
        /// 注意不是函数本身，这个和ToNetValue不一样，这是由于jsData结构定义导致的</param>
        /// <returns></returns>
        public long ToJsValue(object obj, bool bFunExecute = false)
        {
            IntPtr es = MBApi.wkeGlobalExec(Handles);

            if (obj == null)
            {
                return MBApi.jsUndefined();
            }
            else if (obj is int)
            {
                return MBApi.jsInt((int)obj);
            }
            else if (obj is bool)
            {
                return MBApi.jsBoolean((byte)obj);
            }
            else if (obj is double)
            {
                return MBApi.jsDouble((double)obj);
            }
            else if (obj is long)
            {
                return ToJsValue(obj.ToString());
            }
            else if (obj is float)
            {
                return MBApi.jsFloat((float)obj);
            }
            else if (obj is DateTime)
            {
                return MBApi.jsDouble(double .Parse (((DateTime)obj).ToLongDateString()));
            }
            else if (obj is string)
            {
                return MBApi.jsStringW(es, obj.ToString());
            }
            else if (obj is decimal)
            {
                var dec = (decimal)obj;
                if (dec.ToString().Contains("."))
                {
                    return ToJsValue(Convert.ToDouble(dec.ToString()));
                }
                else
                {
                    return ToJsValue(Convert.ToInt32(dec.ToString()));
                }
            }
            else if (obj is IEnumerable)
            {
                List<object> list = new List<object>();
                foreach (object item in (IEnumerable)obj)
                {
                    list.Add(item);
                }

                long array = MBApi.jsEmptyArray(es);
                MBApi.jsSetLength(es, array, list.Count);

                for (int i = 0; i < list.Count; i++)
                {
                    MBApi.jsSetAt(es, array, i, ToJsValue(list[i]));
                }

                return array;
            }
            else if (obj is Delegate)
            {
                Delegate func = (Delegate)obj;
                IntPtr funcptr = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(jsData)));

                jsData funcdata = new jsData
                {
                    typeName = "function",

                    finalize = (ptr) =>
                    {
                        Marshal.FreeHGlobal(ptr);
                    },

                    callAsFunction = (fes, fobj, fargs, fcount) =>
                    {
                        if (func is CommonFuncType)
                        {
                            var fps = new List<object>();
                            for (var i = 0; i < fcount; i++)
                            {
                                fps.Add(ToNetValue(MBApi.jsArg(fes, i)));
                            }

                            if (bFunExecute)
                            {
                                return ToJsValue(((CommonFuncType)func)(fps.ToArray()));
                            }

                            return ToJsValue(fps.ToArray());
                        }
                        else    // 如果传过来的不是动态参数，而是具体的参数
                        {
                            object[] fps = new object[func.Method.GetParameters().Length];
                            for (int i = 0; i < fcount && i < fps.Length; i++)
                            {
                                fps[i] = ToNetValue(MBApi.jsArg(fes, i));
                            }

                            if (bFunExecute)
                            {
                                return ToJsValue(func.Method.Invoke(func.Target, fps));
                            }

                            return ToJsValue(fps);
                        }
                    },
                };

                Marshal.StructureToPtr(funcdata, funcptr, false);

                return MBApi.jsFunction(es, funcptr);
            }
            else
            {
                long jsobj = MBApi.jsEmptyObject(es);

                PropertyInfo[] PropertyArr = obj.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public);
                foreach (var ps in PropertyArr)
                {
                    object oValue = ps.GetValue(obj, null);
                    if (oValue != null)
                    {
                        MBApi.jsSet(es, jsobj, ps.Name, ToJsValue(oValue));
                    }
                }

                return jsobj;
            }
        }
        #endregion

        #region 属性
        /// <summary>
        /// 获取 WebView 句柄
        /// </summary>
        public IntPtr Handle
        {
            get { return Handles; }
        }

        /// <summary>
        /// 获取宿主窗口句柄
        /// </summary>
        public IntPtr HostHandle
        {
            get { return MBApi.wkeGetHostHWND(Handles); }
        }

        /// <summary>
        /// 获取或设置 WebView 的名称
        /// </summary>
        public string Name
        {
            get
            {
                IntPtr pName = MBApi.wkeGetName(Handles);
                if (pName != IntPtr.Zero)
                    return Marshal.PtrToStringAnsi(pName);
                return string.Empty;
            }
            set
            {
                MBApi.wkeSetName(Handles, value);
            }
        }
        /// <summary>
        /// 获取是否处于唤醒状态
        /// </summary>
        public bool IsWake
        {
            get { return MBApi.wkeIsAwake(Handles) != 0; }
        }
        /// <summary>
        /// 获取是否正在载入
        /// </summary>
        public bool IsLoading
        {
            get { return MBApi.wkeIsLoading(Handles) != 0; }
        }
        /// <summary>
        /// 获取是否载入成功
        /// </summary>
        public bool IsLoadingSucceeded
        {
            get { return MBApi.wkeIsLoadingSucceeded(Handles) != 0; }
        }
        /// <summary>
        /// 获取是否载入失败
        /// </summary>
        public bool IsLoadingFailed
        {
            get { return MBApi.wkeIsLoadingFailed(Handles) != 0; }
        }
        /// <summary>
        /// 获取是否载入完成
        /// </summary>
        public bool IsLoadingCompleted
        {
            get { return MBApi.wkeIsLoadingCompleted(Handles) != 0; }
        }
        /// <summary>
        /// 获取文档是否就绪
        /// </summary>
        public bool IsDocumentReady
        {
            get { return MBApi.wkeIsDocumentReady(Handles) != 0; }
        }
        /// <summary>
        /// 获取标题
        /// </summary>
        public string Title
        {
            get 
            {
                IntPtr pTitle = MBApi.wkeGetTitleW(Handles);
                if (pTitle != IntPtr.Zero)
                {
                    return Marshal.PtrToStringUni(pTitle);
                }
                return string.Empty;
            }
        }
        /// <summary>
        /// 获取宽度
        /// </summary>
        public int Width
        {
            get { return MBApi.wkeGetWidth(Handles); }
        }
        /// <summary>
        /// 获取高度
        /// </summary>
        public int Height
        {
            get { return MBApi.wkeGetHeight(Handles); }
        }
        /// <summary>
        /// 获取内容宽度
        /// </summary>
        public int ContentWidth
        {
            get { return MBApi.wkeGetContentWidth(Handles); }
        }
        /// <summary>
        /// 获取内容高度
        /// </summary>
        public int ContentHeight
        {
            get { return MBApi.wkeGetContentHeight(Handles); }
        }
        /// <summary>
        /// 获取是否能后退操作
        /// </summary>
        public bool CanGoBack
        {
            get { return MBApi.wkeCanGoBack(Handles) != 0; }
        }
        /// <summary>
        /// 获取是否能前进操作
        /// </summary>
        public bool CanGoForward
        {
            get { return MBApi.wkeCanGoForward(Handles) != 0; }
        }

        /// <summary>
        /// 获取或设置Cookie引擎是否启用
        /// </summary>
        public bool CookieEnabled
        {
            get { return MBApi.wkeIsCookieEnabled(Handles) != 0; }
            set { MBApi.wkeSetCookieEnabled(Handles, value); }
        }
        /// <summary>
        /// 获取或设置媒体音量
        /// </summary>
        public float MediaVolume
        {
            get { return MBApi.wkeGetMediaVolume(Handles); }
            set { MBApi.wkeSetMediaVolume(Handles, value); }
        }
        /// <summary>
        /// 获取或设置缩放因子
        /// </summary>
        public float ZoomFactor
        {
            get { return MBApi.wkeGetZoomFactor(Handles); }
            set { MBApi.wkeSetZoomFactor(Handles, value); }
        }
        /// <summary>
        /// 获取主框架句柄
        /// </summary>
        public IntPtr MainFrame
        {
            get { return MBApi.wkeWebFrameGetMainFrame(Handles); }
        }
        /// <summary>
        /// 获取或设置光标类型
        /// </summary>
        public wkeCursorInfo CursorInfoType
        {
            get { return MBApi.wkeGetCursorInfoType(Handles); }
            set { MBApi.wkeSetCursorInfoType(Handles, value); }
        }

        /// <summary>
        /// 设置是否启用内存缓存
        /// </summary>
        /// <param name="Enable"></param>
        public bool MemoryCacheEnable
        {
            set
            {
                MBApi.wkeSetMemoryCacheEnable(Handles, value);
            }
        }

        /// <summary>
        /// 设置是否导航到新窗口
        /// </summary>
        /// <param name="enable">如果为false 则不会弹出新窗口</param>
        public bool NavigationToNewWindowEnable
        {
            set { MBApi.wkeSetNavigationToNewWindowEnable(Handles, value); }
        }
        /// <summary>
        /// 设置是否启用触摸
        /// </summary>
        public bool TouchEnable
        {
            set { MBApi.wkeSetTouchEnabled(Handles, value); }
        }

        /// <summary>
        /// 设置是否启用 NPAPI 插件
        /// </summary>
        public bool NpapiPluginsEnabled
        {
            set { MBApi.wkeSetNpapiPluginsEnabled(Handles, value); }
        }
        /// <summary>
        /// 设置是否启用无头模式，可以关闭渲染
        /// </summary>
        public bool HeadlessEnabled
        {
            set { MBApi.wkeSetHeadlessEnabled(Handles, value); }
        }

        /// <summary>
        /// 设置是否启用拖拽，可关闭拖拽文件加载网页
        /// </summary>
        public bool DragEnable
        {
            set { MBApi.wkeSetDragEnable(Handles, value); }
        }
        /// <summary>
        /// 可关闭拖拽到其他进程
        /// </summary>
        public bool DragDropEnable
        {
            set { MBApi.wkeSetDragDropEnable(Handles, value); }
        }
        /// <summary>
        /// 设置资源回收间隔
        /// </summary>
        public int ResourceGc
        {
            set { MBApi.wkeSetResourceGc(Handles, value); }
        }

        /// <summary>
        /// 获取是否真正处理手势
        /// </summary>
        public bool IsProcessingUserGesture
        {
            get { return MBApi.wkeIsProcessingUserGesture(Handles) != 0; }
        }

        #endregion

    }

    #region 事件参数
    public class MiniblinkEventArgs : EventArgs
    {
        private IntPtr Handles;

        public MiniblinkEventArgs(IntPtr webView)
        {
            Handles = webView;
        }
        /// <summary>
        /// 获取WebView句柄
        /// </summary>
        public IntPtr Handle
        {
            get { return Handles; }
        }
    }

    /// <summary>
    /// OnMouseOverUrlChanged 事件参数
    /// </summary>
    public class MouseOverUrlChangedEventArgs : MiniblinkEventArgs
    {
        private IntPtr m_url;

        public MouseOverUrlChangedEventArgs(IntPtr webView, IntPtr url)
            : base(webView)
        {
            m_url = url;
        }
        /// <summary>
        /// 获取URL
        /// </summary>
        public string URL
        {
            get
            {
                if (m_url != IntPtr.Zero)
                {
                    IntPtr pTitle = MBApi.wkeGetStringW(m_url);
                    if (pTitle != IntPtr.Zero)
                        return Marshal.PtrToStringUni(pTitle);
                }
                return string.Empty;
            }
        }
    }

    /// <summary>
    /// OnTitleChange 事件参数
    /// </summary>
    public class TitleChangeEventArgs : MiniblinkEventArgs
    {
        private IntPtr m_title;

        public TitleChangeEventArgs(IntPtr webView, IntPtr title)
            : base(webView)
        {
            m_title = title;
        }
        /// <summary>
        /// 获取网页标题
        /// </summary>
        public string Title
        {
            get
            {
                if (m_title != IntPtr.Zero)
                {
                    IntPtr pTitle = MBApi.wkeGetStringW(m_title);
                    if (pTitle != IntPtr.Zero)
                        return Marshal.PtrToStringUni(pTitle);
                }
                return string.Empty;
            }
        }
    }
    /// <summary>
    /// OnUrlChange 事件参数
    /// </summary>
    public class UrlChangeEventArgs : MiniblinkEventArgs
    {
        private IntPtr m_url;
        private IntPtr m_frame;

        public UrlChangeEventArgs(IntPtr webView, IntPtr url, IntPtr webFrame)
            : base(webView)
        {
            m_url = url;
            m_frame = webFrame;
        }
        /// <summary>
        /// 获取网页URL
        /// </summary>
        public string URL
        {
            get
            {
                if (m_url != IntPtr.Zero)
                {
                    IntPtr pUrl = MBApi.wkeGetStringW(m_url);
                    if (pUrl != IntPtr.Zero)
                        return Marshal.PtrToStringUni(pUrl);
                }
                return string.Empty;
            }
        }
        /// <summary>
        /// 获取框架句柄
        /// </summary>
        public IntPtr WebFrame
        {
            get { return m_frame; }
        }

    }
    /// <summary>
    /// OnAlertBox事件参数
    /// </summary>
    public class AlertBoxEventArgs : MiniblinkEventArgs
    {
        private IntPtr m_msg;

        public AlertBoxEventArgs(IntPtr webView, IntPtr msg)
            : base(webView)
        {
            m_msg = msg;
        }
        /// <summary>
        /// 获取消息值
        /// </summary>
        public string Msg
        {
            get
            {
                if (m_msg != IntPtr.Zero)
                {
                    IntPtr pMsg = MBApi.wkeGetStringW(m_msg);
                    if (pMsg != IntPtr.Zero)
                        return Marshal.PtrToStringUni(pMsg);
                }
                return string.Empty;
            }
        }
    }
    /// <summary>
    /// OnConfirmBox事件参数
    /// </summary>
    public class ConfirmBoxEventArgs : MiniblinkEventArgs
    {
        private IntPtr m_msg;
        private bool m_result;

        public ConfirmBoxEventArgs(IntPtr webView, IntPtr msg)
            : base(webView)
        {
            m_msg = msg;
        }
        /// <summary>
        /// 获取消息值
        /// </summary>
        public string Msg
        {
            get
            {
                if (m_msg != IntPtr.Zero)
                {
                    IntPtr pMsg = MBApi.wkeGetStringW(m_msg);
                    if (pMsg != IntPtr.Zero)
                        return Marshal.PtrToStringUni(pMsg);
                }
                return string.Empty;
            }
        }
        /// <summary>
        /// 设置ConfirmBox的返回值
        /// </summary>
        public bool Result
        {
            get { return m_result; }
            set { m_result = value; }
        }

    }
    /// <summary>
    /// OnPromptBox事件参数
    /// </summary>
    public class PromptBoxEventArgs : MiniblinkEventArgs
    {
        private IntPtr m_msg;
        private bool m_result;
        private IntPtr m_defaultStr;
        private IntPtr m_resultStr;

        public PromptBoxEventArgs(IntPtr webView, IntPtr msg, IntPtr defaultResult, IntPtr result)
            : base(webView)
        {
            m_msg = msg;
            m_defaultStr = defaultResult;
            m_resultStr = result;
        }
        /// <summary>
        /// 获取提示信息
        /// </summary>
        public string Msg
        {
            get
            {
                if (m_msg != IntPtr.Zero)
                {
                    IntPtr pMsg = MBApi.wkeGetStringW(m_msg);
                    if (pMsg != IntPtr.Zero)
                        return Marshal.PtrToStringUni(pMsg);
                }
                return string.Empty;
            }
        }
        /// <summary>
        /// 设置PromptBox的返回值
        /// </summary>
        public bool Result
        {
            get { return m_result; }
            set { m_result = value; }
        }

        /// <summary>
        /// 获取默认文本
        /// </summary>
        public string DefaultResultString
        {
            get 
            {
                if (m_defaultStr != IntPtr.Zero)
                {
                    IntPtr pStr = MBApi.wkeGetStringW(m_defaultStr);
                    return Marshal.PtrToStringUni(pStr);
                }
                return string.Empty;
            }
        }
        /// <summary>
        /// 设置返回文本
        /// </summary>
        public string ResultString
        {
            set 
            {
                if (m_resultStr != IntPtr.Zero)
                {
                    MBApi.wkeSetStringW(m_resultStr, value, value.Length);
                }
            }
        }

    }
    /// <summary>
    /// OnNavigate事件参数
    /// </summary>
    public class NavigateEventArgs : MiniblinkEventArgs
    {
        private wkeNavigationType m_type;
        private IntPtr m_url;
        private bool m_result;

        public NavigateEventArgs(IntPtr webView, wkeNavigationType navigationType, IntPtr url)
            : base(webView)
        {
            m_type = navigationType;
            m_url = url;
        }
        /// <summary>
        /// 获取导航类型
        /// </summary>
        public wkeNavigationType NavigationType
        {
            get { return m_type; }
        }
        /// <summary>
        /// 获取URL
        /// </summary>
        public string URL
        {
            get 
            {
                if (m_url != IntPtr.Zero)
                {
                    return Marshal.PtrToStringUni(MBApi.wkeGetStringW(m_url));
                }
                return string.Empty;
            }
        }
        /// <summary>
        /// 设置是否取消导航，false 表示允许导航，true 表示禁止导航
        /// </summary>
        public bool Cancel
        {
            get { return m_result; }
            set { m_result = value; }
        }
    }
    /// <summary>
    /// OnCreateView事件参数
    /// </summary>
    public class CreateViewEventArgs : MiniblinkEventArgs
    {
        private wkeNavigationType m_type;
        private IntPtr m_url;
        private IntPtr m_windowFeatures;
        private IntPtr m_result;

        public CreateViewEventArgs(IntPtr webView, wkeNavigationType navigationType, IntPtr url, IntPtr windowFeatures)
            : base(webView)
        {
            m_type = navigationType;
            m_url = url;
            m_windowFeatures = windowFeatures;
            m_result = webView;
        }
        /// <summary>
        /// 获取导航类型
        /// </summary>
        public wkeNavigationType NavigationType
        {
            get { return m_type; }
        }
        /// <summary>
        /// 获取URL
        /// </summary>
        public string URL
        {
            get
            {
                if (m_url != IntPtr.Zero)
                {
                    return Marshal.PtrToStringUni(MBApi.wkeGetStringW(m_url));
                }
                return string.Empty;
            }
        }
        /// <summary>
        /// 获取新窗口的一些特征
        /// </summary>
        public wkeWindowFeatures WindowFeatures
        {
            get 
            {
                if (m_windowFeatures != IntPtr.Zero)
                {
                    return (wkeWindowFeatures)Marshal.PtrToStructure(m_windowFeatures, typeof(wkeWindowFeatures));
                }
                else
                {
                    return new wkeWindowFeatures();
                }
            }
        }

        /// <summary>
        /// 设置新窗口WebViewHandle
        /// </summary>
        public IntPtr NewWebViewHandle
        {
            get { return m_result; }
            set { m_result = value; }
        }

    }
    /// <summary>
    /// OnDocumentReady
    /// </summary>
    public class DocumentReadyEventArgs : MiniblinkEventArgs
    {
        private IntPtr m_frame;

        public DocumentReadyEventArgs(IntPtr webView, IntPtr frame)
            : base(webView)
        {
            m_frame = frame;
        }
        /// <summary>
        /// 获取框架句柄
        /// </summary>
        public IntPtr Frame
        {
            get { return m_frame; }
        }
    }
    /// <summary>
    /// OnLoadingFinish事件参数
    /// </summary>
    public class LoadingFinishEventArgs : MiniblinkEventArgs
    {
        private IntPtr m_url;
        private wkeLoadingResult m_loadingResult;
        private IntPtr m_failedReason;

        public LoadingFinishEventArgs(IntPtr webView, IntPtr url, wkeLoadingResult result, IntPtr failedReason)
            : base(webView)
        {
            m_url = url;
            m_loadingResult = result;
            m_failedReason = failedReason;
        }
        /// <summary>
        /// 获取URL
        /// </summary>
        public string URL
        {
            get 
            {
                if (m_url != IntPtr.Zero)
                {
                    return Marshal.PtrToStringUni(MBApi.wkeGetStringW(m_url));
                }
                return string.Empty;
            }
        }
        /// <summary>
        /// 获取载入的返回值
        /// </summary>
        public wkeLoadingResult LoadingResult
        {
            get { return m_loadingResult; }
        }
        /// <summary>
        /// 获取失败的原因
        /// </summary>
        public string FailedReason
        {
            get 
            {
                if (m_failedReason != IntPtr.Zero)
                {
                    return Marshal.PtrToStringUni(MBApi.wkeGetStringW(m_failedReason));
                }
                return string.Empty;
            }
        }
    }
    /// <summary>
    /// OnDownload事件参数
    /// </summary>
    public class DownloadEventArgs : MiniblinkEventArgs
    {
        private IntPtr m_url;
        private bool m_cancel;

        public DownloadEventArgs(IntPtr webView, IntPtr url)
            : base(webView)
        {
            m_url = url;
        }
        /// <summary>
        /// 获取URL
        /// </summary>
        public string URL
        {
            get
            {
                if (m_url != IntPtr.Zero)
                {
                    return Help.PtrToStringUTF8(m_url);
                }
                return string.Empty;
            }
        }
        /// <summary>
        /// 设置是否取消，true 表示取消
        /// </summary>
        public bool Cancel
        {
            get { return m_cancel; }
            set { m_cancel = value; }
        }
    }
    /// <summary>
    /// OnConsole事件参数
    /// </summary>
    public class ConsoleEventArgs : MiniblinkEventArgs
    {
        private wkeConsoleLevel m_level;
        private IntPtr m_message;
        private IntPtr m_sourceName;
        private uint m_sourceLine;
        private IntPtr m_stackTrace;

        public ConsoleEventArgs(IntPtr webView, wkeConsoleLevel level, IntPtr message, IntPtr sourceName, uint sourceLine, IntPtr stackTrace)
            : base(webView)
        {
            m_level = level;
            m_message = message;
            m_sourceName = sourceName;
            m_sourceLine = sourceLine;
            m_stackTrace = stackTrace;
        }
        /// <summary>
        /// 获取消息等级
        /// </summary>
        public wkeConsoleLevel Level
        {
            get { return m_level; }
        }
        /// <summary>
        /// 获取消息内容
        /// </summary>
        public string Message
        {
            get 
            {
                if (m_message != IntPtr.Zero)
                {
                    return Marshal.PtrToStringUni(MBApi.wkeGetStringW(m_message));
                }
                return string.Empty;
            }
        }
        /// <summary>
        /// 获取来源名
        /// </summary>
        public string SourceName
        {
            get
            {
                if (m_sourceName != IntPtr.Zero)
                {
                    return Marshal.PtrToStringUni(MBApi.wkeGetStringW(m_sourceName));
                }
                return string.Empty;
            }
        }
        /// <summary>
        /// 获取来源行号
        /// </summary>
        public uint SourceLine
        {
            get { return m_sourceLine; }
        }
        /// <summary>
        /// 获取堆栈跟踪
        /// </summary>
        public string StackTrace
        {
            get
            {
                if (m_stackTrace != IntPtr.Zero)
                {
                    return Marshal.PtrToStringUni(MBApi.wkeGetStringW(m_stackTrace));
                }
                return string.Empty;
            }
        }

    }
    /// <summary>
    /// OnLoadUrlBegin事件参数
    /// </summary>
    public class LoadUrlBeginEventArgs : MiniblinkEventArgs
    {
        private IntPtr m_url;
        private IntPtr m_job;
        private bool m_cancel;

        public LoadUrlBeginEventArgs(IntPtr webView, IntPtr url, IntPtr job)
            : base(webView)
        {
            m_url = url;
            m_job = job;
        }
        /// <summary>
        /// 获取URL
        /// </summary>
        public string URL
        {
            get 
            {
                if (m_url != IntPtr.Zero)
                {
                    return Help.PtrToStringUTF8(m_url);
                }
                return string.Empty;
            }
        }
        /// <summary>
        /// 获取job
        /// </summary>
        public IntPtr Job
        {
            get { return m_job; }
        }
        /// <summary>
        /// 是否取消载入，true 表示取消
        /// </summary>
        public bool Cancel
        {
            get { return m_cancel; }
            set { m_cancel = value; }
        }
    }
    /// <summary>
    /// OnLoadUrlEnd事件参数
    /// </summary>
    public class LoadUrlEndEventArgs : MiniblinkEventArgs
    {
        private IntPtr m_url;
        private IntPtr m_job;
        private IntPtr m_buf;
        private int m_len;

        public LoadUrlEndEventArgs(IntPtr webView, IntPtr url, IntPtr job, IntPtr buf, int len)
            : base(webView)
        {
            m_url = url;
            m_job = job;
            m_buf = buf;
            m_len = len;
        }
        /// <summary>
        /// 获取URL
        /// </summary>
        public string URL
        {
            get
            {
                if (m_url != IntPtr.Zero)
                {
                    return Help.PtrToStringUTF8(m_url);
                }
                return string.Empty;
            }
        }
        /// <summary>
        /// 获取job
        /// </summary>
        public IntPtr Job
        {
            get { return m_job; }
        }
        /// <summary>
        /// 获取数据
        /// </summary>
        public byte[] Data
        {
            get
            {
                if (m_buf != IntPtr.Zero)
                {
                    byte[] data = new byte[m_len];
                    Marshal.Copy(m_buf, data, 0, m_len);
                    return data;
                }
                return null;
            }
        }
    }
    /// <summary>
    /// OnDidCreateScriptContext
    /// </summary>
    public class DidCreateScriptContextEventArgs : MiniblinkEventArgs
    {
        private IntPtr m_frame;
        private IntPtr m_context;
        private int m_extensionGroup;
        private int m_worldId;

        public DidCreateScriptContextEventArgs(IntPtr webView, IntPtr frame, IntPtr context, int extensionGroup, int worldId)
            : base(webView)
        {
            m_frame = frame;
            m_context = context;
            m_extensionGroup = extensionGroup;
            m_worldId = worldId;
        }
        /// <summary>
        /// 获取框架句柄
        /// </summary>
        public IntPtr Frame
        {
            get { return m_frame; }
        }
        /// <summary>
        /// 获取脚本上下文
        /// </summary>
        public IntPtr Context
        {
            get { return m_context; }
        }

        public int ExtensionGroup
        {
            get { return m_extensionGroup; }
        }

        public int WorldId
        {
            get { return m_worldId; }
        }

    }
    /// <summary>
    /// OnWillReleaseScriptContext 事件参数
    /// </summary>
    public class WillReleaseScriptContextEventArgs : MiniblinkEventArgs
    {
        private IntPtr m_frame;
        private IntPtr m_context;
        private int m_worldId;

        public WillReleaseScriptContextEventArgs(IntPtr webView, IntPtr frame, IntPtr context, int worldId)
            : base(webView)
        {
            m_frame = frame;
            m_context = context;
            m_worldId = worldId;
        }
        /// <summary>
        /// 获取框架句柄
        /// </summary>
        public IntPtr Frame
        {
            get { return m_frame; }
        }
        /// <summary>
        /// 获取脚本上下文
        /// </summary>
        public IntPtr Context
        {
            get { return m_context; }
        }

        public int WorldId
        {
            get { return m_worldId; }
        }
    }
    /// <summary>
    /// OnNetResponse 事件参数
    /// </summary>
    public class NetResponseEventArgs : MiniblinkEventArgs
    {
        private IntPtr m_url;
        private IntPtr m_job;
        private bool m_cancel;

        public NetResponseEventArgs(IntPtr webView, IntPtr url, IntPtr job)
            : base(webView)
        {
            m_url = url;
            m_job = job;
        }

        public string URL
        {
            get
            {
                if (m_url != IntPtr.Zero)
                {
                    return Help.PtrToStringUTF8(m_url);
                }
                return string.Empty;
            }
        }

        public IntPtr Job
        {
            get { return m_job; }
        }

        public bool Cancel
        {
            get { return m_cancel; }
            set { m_cancel = value; }
        }
    }
    /// <summary>
    /// OnWillMediaLoad 事件参数
    /// </summary>
    public class WillMediaLoadEventArgs : MiniblinkEventArgs
    {
        private IntPtr m_url;
        private wkeMediaLoadInfo m_info;

        public WillMediaLoadEventArgs(IntPtr webView, IntPtr url, IntPtr info)
            : base(webView)
        {
            m_url = url;
            m_info = (wkeMediaLoadInfo)Marshal.PtrToStructure(info, typeof(wkeMediaLoadInfo));
        }

        public string URL
        {
            get
            {
                if (m_url != IntPtr.Zero)
                {
                    return Help.PtrToStringUTF8(m_url);
                }
                return string.Empty;
            }
        }

        public wkeMediaLoadInfo Info
        {
            get { return m_info; }
        }
    }
    /// <summary>
    /// OnOtherLoad 事件参数
    /// </summary>
    public class OtherLoadEventArgs : MiniblinkEventArgs
    {
        private wkeOtherLoadType m_type;
        private wkeTempCallbackInfo m_info;

        public OtherLoadEventArgs(IntPtr webView, wkeOtherLoadType type, IntPtr info)
            : base(webView)
        {
            m_type = type;
            m_info = (wkeTempCallbackInfo)Marshal.PtrToStructure(info, typeof(wkeTempCallbackInfo));
        }
        /// <summary>
        /// 载入类型
        /// </summary>
        public wkeOtherLoadType LoadType
        {
            get { return m_type; }
        }

        public int Size
        {
            get { return m_info.size; }
        }
        /// <summary>
        /// 框架
        /// </summary>
        public IntPtr Frame
        {
            get { return m_info.frame; }
        }

        public wkeWillSendRequestInfo WillSendRequestInfo
        {
            get 
            {
                wkeWillSendRequestInfo srInfo = new wkeWillSendRequestInfo();
                if (m_info.willSendRequestInfo != IntPtr.Zero)
                {
                    IntPtr ptr = IntPtr.Zero;
                    IntPtr strPtr = IntPtr.Zero;
                    srInfo.isHolded = Marshal.ReadInt32(m_info.willSendRequestInfo) != 0;

                    ptr = Marshal.ReadIntPtr(m_info.willSendRequestInfo, 4);
                    if (ptr != IntPtr.Zero)
                    {
                        strPtr = MBApi.wkeGetStringW(ptr);
                        if (strPtr != IntPtr.Zero)
                        {
                            srInfo.url = Marshal.PtrToStringUni(strPtr);
                        }
                    }
                    ptr = Marshal.ReadIntPtr(m_info.willSendRequestInfo, 8);
                    if (ptr != IntPtr.Zero)
                    {
                        strPtr = MBApi.wkeGetStringW(ptr);
                        if (strPtr != IntPtr.Zero)
                        {
                            srInfo.newUrl = Marshal.PtrToStringUni(strPtr);
                        }
                    }
                    srInfo.resourceType = (wkeResourceType)Marshal.ReadInt32(m_info.willSendRequestInfo, 12);
                    srInfo.httpResponseCode = Marshal.ReadInt32(m_info.willSendRequestInfo, 16);
                    ptr = Marshal.ReadIntPtr(m_info.willSendRequestInfo, 20);
                    if (ptr != IntPtr.Zero)
                    {
                        strPtr = MBApi.wkeGetStringW(ptr);
                        if (strPtr != IntPtr.Zero)
                        {
                            srInfo.method = Marshal.PtrToStringUni(strPtr);
                        }
                    }
                    ptr = Marshal.ReadIntPtr(m_info.willSendRequestInfo, 24);
                    if (ptr != IntPtr.Zero)
                    {
                        strPtr = MBApi.wkeGetStringW(ptr);
                        if (strPtr != IntPtr.Zero)
                        {
                            srInfo.referrer = Marshal.PtrToStringUni(strPtr);
                        }
                    }
                    srInfo.headers = Marshal.ReadIntPtr(m_info.willSendRequestInfo, 28);
                }
                return srInfo;
            }
        }

        public IntPtr WillSendRequestInfoPtr
        {
            get { return m_info.willSendRequestInfo; }
        }
    }


    /// <summary>
    /// 窗口过程事件参数
    /// </summary>
    public class WindowProcEventArgs : EventArgs
    {
        private IntPtr m_hWnd;
        private int m_msg;
        private IntPtr m_wParam;
        private IntPtr m_lParam;
        private IntPtr m_result;
        private bool m_bHand;

        public WindowProcEventArgs(IntPtr hWnd, int msg, IntPtr wParam, IntPtr lParam)
        {
            m_hWnd = hWnd;
            m_msg = msg;
            m_wParam = wParam;
            m_lParam = lParam;
        }
        /// <summary>
        /// 获取窗口句柄
        /// </summary>
        public IntPtr Handle
        {
            get { return m_hWnd; }
        }
        /// <summary>
        /// 获取消息值
        /// </summary>
        public int Msg
        {
            get { return m_msg; }
        }
        /// <summary>
        /// 获取 wParam
        /// </summary>
        public IntPtr wParam
        {
            get { return m_wParam; }
        }
        /// <summary>
        /// 获取 lParam
        /// </summary>
        public IntPtr lParam
        {
            get { return m_lParam; }
        }
        /// <summary>
        /// 设置返回值
        /// </summary>
        public IntPtr Result
        {
            get { return m_result; }
            set { m_result = value; }
        }
        /// <summary>
        /// 如果为 true 则会返回 Result ，false 则返回默认值
        /// </summary>
        public bool bHand
        {
            get { return m_bHand; }
            set { m_bHand = value; }
        }
    }


    #endregion

    internal delegate IntPtr OnWindowProcEventHandler(IntPtr hWnd, int msg, IntPtr wParam, IntPtr lParam);


}
