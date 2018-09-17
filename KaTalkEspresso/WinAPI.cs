using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace KaTalkEspresso
{
    class WinAPI
    {
        // 현재 윈도가 64비트 윈도인지 확인
        private static readonly bool RUNNING_ON_64BIT_WINDOWS = IntPtr.Size == 8;
        public static bool bitAndIntPtr(IntPtr value, long and, long should)
        {
            if (RUNNING_ON_64BIT_WINDOWS)
            {
                return (value.ToInt64() & and) == should;
            }
            else
            {
                return (value.ToInt32() & and) == should;
            }
        }

        // SetLastError 는 최근 오류 여부를 Marshal.GetLastWin32Error()로 추출 가능. 기본값 false
        //https://msdn.microsoft.com/en-us/library/system.runtime.interopservices.marshal.getlastwin32error(v=vs.110).aspx
        private const bool DEFAULT_LAST_ERROR = false;
        private const bool SET_LAST_ERROR = true;

        public static int GetLastWin32Error()
        {
            return Marshal.GetLastWin32Error();
        }

        //32비트, Long 만 붙는 것을 확인 https://www.pinvoke.net/default.aspx/user32.getwindowlong
        [DllImport("user32.dll", EntryPoint = "GetWindowLong", SetLastError = DEFAULT_LAST_ERROR)]
        private static extern IntPtr GetWindowLongPtr32(IntPtr hWnd, int nIndex);

        //64비트, Long 뒤에 Ptr가 붙는 것을 확인 https://www.pinvoke.net/default.aspx/user32.getwindowlong
        [DllImport("user32.dll", EntryPoint = "GetWindowLongPtr", SetLastError = DEFAULT_LAST_ERROR)]
        private static extern IntPtr GetWindowLongPtr64(IntPtr hWnd, int nIndex);

        // https://www.pinvoke.net/default.aspx/user32.getwindowlong
        // 이 static 메소드는 Win32가 GetWIndowLongPtr를 직접적으로 부를 수 없기에 필요합니다.
        public static IntPtr GetWindowLong(IntPtr hWnd, int nFlags)
        {
            if (RUNNING_ON_64BIT_WINDOWS)
            {
                return GetWindowLongPtr64(hWnd, nFlags);
            }
            else
            {
                return GetWindowLongPtr32(hWnd, nFlags);
            }
        }

        //https://www.pinvoke.net/default.aspx/Enums.WindowLongFlags
        public class WindowLongFlags
        {
            public const int GWL_EXSTYLE = -20;
            public const int GWLP_HINSTANCE = -6;
            public const int GWLP_HWNDPARENT = -8;
            public const int GWL_ID = -12;
            public const int GWL_STYLE = -16;
            public const int GWL_USERDATA = -21;
            public const int GWL_WNDPROC = -4;
            public const int DWLP_USER = 0x8;
            public const int DWLP_MSGRESULT = 0x0;
            public const int DWLP_DLGPROC = 0x4;
        }

        //https://docs.microsoft.com/en-us/windows/desktop/winmsg/extended-window-styles
        // 사용시 IntPtr로 변환해야함
        public class ExtendedWindowStyles
        {
            public const Int64 WS_EX_ACCEPTFILES = 0x00000010L;
            //The window accepts drag-drop files.

            public const Int64 WS_EX_APPWINDOW = 0x00040000L;
            //Forces a top-level window onto the taskbar when the window is visible. 

            public const Int64 WS_EX_CLIENTEDGE = 0x00000200L;
            //The window has a border with a sunken edge.

            public const int WS_EX_COMPOSITED = 0x02000000;
            //Paints all descendants of a window in bottom-to-top painting order using double-buffering. For more information, see Remarks. This cannot be used if the window has a class style of either CS_OWNDC or CS_CLASSDC. 
            //Windows 2000: This style is not supported.

            public const Int64 WS_EX_CONTEXTHELP = 0x00000400L;
            // title bar of the window includes a question mark. When the user clicks the question mark, the cursor changes to a question mark with a pointer. If the user then clicks a child window, the child receives a WM_HELP message. The child window should pass the message to the parent window procedure, which should call the WinHelp function using the HELP_WM_HELP command. The Help application displays a pop-up window that typically contains help for the child window.
            //WS_EX_CONTEXTHELP cannot be used with the WS_MAXIMIZEBOX or WS_MINIMIZEBOX styles.

            public const Int64 WS_EX_CONTROLPARENT = 0x00010000L;
            //The window itself contains child windows that should take part in dialog box navigation. If this style is specified, the dialog manager recurses into children of this window when performing navigation operations such as handling the TAB key, an arrow key, or a keyboard mnemonic.

            public const Int64 WS_EX_DLGMODALFRAME = 0x00000001L;
            //The window has a double border; the window can, optionally, be created with a title bar by specifying the WS_CAPTION style in the dwStyle parameter.

            public const Int64 WS_EX_LAYERED = 0x00080000;
            //The window is a layered window. This style cannot be used if the window has a class style of either CS_OWNDC or CS_CLASSDC.
            //Windows 8: The WS_EX_LAYERED style is supported for top-level windows and child windows. Previous Windows versions support WS_EX_LAYERED only for top-level windows.

            public const Int64 WS_EX_LAYOUTRTL = 0x00400000L;
            //If the shell language is Hebrew, Arabic, or another language that supports reading order alignment, the horizontal origin of the window is on the right edge. Increasing horizontal values advance to the left. 

            public const Int64 WS_EX_LEFT = 0x00000000L;
            //The window has generic left-aligned properties. This is the default.

            public const Int64 WS_EX_LEFTSCROLLBAR = 0x00004000L;
            //If the shell language is Hebrew, Arabic, or another language that supports reading order alignment, the vertical scroll bar (if present) is to the left of the client area. For other languages, the style is ignored.

            public const Int64 WS_EX_LTRREADING = 0x00000000L;
            //The window text is displayed using left-to-right reading-order properties. This is the default.

            public const Int64 WS_EX_MDICHILD = 0x00000040L;
            //The window is a MDI child window.

            public const Int64 WS_EX_NOACTIVATE = 0x08000000L;
            //A top-level window created with this style does not become the foreground window when the user clicks it. The system does not bring this window to the foreground when the user minimizes or closes the foreground window.
            //The window should not be activated through programmatic access or via keyboard navigation by accessible technology, such as Narrator.
            //To activate the window, use the SetActiveWindow or SetForegroundWindow function.
            //The window does not appear on the taskbar by default. To force the window to appear on the taskbar, use the WS_EX_APPWINDOW style.

            public const Int64 WS_EX_NOINHERITLAYOUT = 0x00100000L;
            //The window does not pass its window layout to its child windows.

            public const Int64 WS_EX_NOPARENTNOTIFY = 0x00000004L;
            //The child window created with this style does not send the WM_PARENTNOTIFY message to its parent window when it is created or destroyed.

            public const Int64 WS_EX_NOREDIRECTIONBITMAP = 0x00200000L;
            //The window does not render to a redirection surface. This is for windows that do not have visible content or that use mechanisms other than surfaces to provide their visual.

            public const Int64 WS_EX_OVERLAPPEDWINDOW = (WS_EX_WINDOWEDGE | WS_EX_CLIENTEDGE);
            //The window is an overlapped window.

            public const Int64 WS_EX_PALETTEWINDOW = (WS_EX_WINDOWEDGE | WS_EX_TOOLWINDOW | WS_EX_TOPMOST);
            //The window is palette window, which is a modeless dialog box that presents an array of commands. 

            public const Int64 WS_EX_RIGHT = 0x00001000L;
            //The window has generic "right-aligned" properties. This depends on the window class. This style has an effect only if the shell language is Hebrew, Arabic, or another language that supports reading-order alignment; otherwise, the style is ignored.
            //Using the WS_EX_RIGHT style for static or edit controls has the same effect as using the SS_RIGHT or ES_RIGHT style, respectively. Using this style with button controls has the same effect as using BS_RIGHT and BS_RIGHTBUTTON styles. 

            public const Int64 WS_EX_RIGHTSCROLLBAR = 0x00000000L;
            //The vertical scroll bar (if present) is to the right of the client area. This is the default.

            public const Int64 WS_EX_RTLREADING = 0x00002000L;
            //If the shell language is Hebrew, Arabic, or another language that supports reading-order alignment, the window text is displayed using right-to-left reading-order properties. For other languages, the style is ignored.

            public const Int64 WS_EX_STATICEDGE = 0x00020000L;
            //The window has a three-dimensional border style intended to be used for items that do not accept user input.

            public const Int64 WS_EX_TOOLWINDOW = 0x00000080L;
            //The window is intended to be used as a floating toolbar. A tool window has a title bar that is shorter than a normal title bar, and the window title is drawn using a smaller font. A tool window does not appear in the taskbar or in the dialog that appears when the user presses ALT+TAB. If a tool window has a system menu, its icon is not displayed on the title bar. However, you can display the system menu by right-clicking or by typing ALT+SPACE. 

            public const Int64 WS_EX_TOPMOST = 0x00000008L;
            //The window should be placed above all non-topmost windows and should stay above them, even when the window is deactivated. To add or remove this style, use the SetWindowPos function.

            public const Int64 WS_EX_TRANSPARENT = 0x00000020L;
            //The window should not be painted until siblings beneath the window (that were created by the same thread) have been painted. The window appears transparent because the bits of underlying sibling windows have already been painted.
            //To achieve transparency without these restrictions, use the SetWindowRgn function.

            public const Int64 WS_EX_WINDOWEDGE = 0x00000100L;
            //The window has a border with a raised edge.
        }


        // https://www.pinvoke.net/default.aspx/user32.getdesktopwindow
        [DllImport("user32.dll", EntryPoint = "GetDesktopWindow", SetLastError = DEFAULT_LAST_ERROR)]
        public static extern IntPtr GetDesktopWindow();

        // https://www.pinvoke.net/default.aspx/user32.FindWindowEx
        [DllImport("user32.dll", EntryPoint = "FindWindowEx", SetLastError = DEFAULT_LAST_ERROR)]
        public static extern IntPtr FindWindowEx(IntPtr hwndParent, IntPtr hwndChildAfter, string lpszClass, string lpszWindow);

        // https://www.pinvoke.net/default.aspx/user32.showwindow
        [DllImport("user32.dll", EntryPoint = "ShowWindow", SetLastError = SET_LAST_ERROR)]
        public static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

        //https://msdn.microsoft.com/en-us/library/windows/desktop/ms633548(v=vs.85).aspx
        public class ShowWindowCommands
        {
            public const int SW_FORCEMINIMIZE = 11;
            //Minimizes a window, even if the thread that owns the window is not responding. This flag should only be used when minimizing windows from a different thread.

            public const int SW_HIDE = 0;
            //Hides the window and activates another window.

            public const int SW_MAXIMIZE = 3;
            //Maximizes the specified window.

            public const int SW_MINIMIZE = 6;
            //Minimizes the specified window and activates the next top-level window in the Z order.

            public const int SW_RESTORE = 9;
            //Activates and displays the window. If the window is minimized or maximized, the system restores it to its original size and position. An application should specify this flag when restoring a minimized window.

            public const int SW_SHOW = 5;
            //Activates the window and displays it in its current size and position.

            public const int SW_SHOWDEFAULT = 10;
            //Sets the show state based on the SW_ value specified in the STARTUPINFO structure passed to the CreateProcess function by the program that started the application.

            public const int SW_SHOWMAXIMIZED = 3;
            //Activates the window and displays it as a maximized window.

            public const int SW_SHOWMINIMIZED = 2;
            //Activates the window and displays it as a minimized window.

            public const int SW_SHOWMINNOACTIVE = 7;
            //Displays the window as a minimized window. This value is similar to SW_SHOWMINIMIZED, except the window is not activated.

            public const int SW_SHOWNA = 8;
            //Displays the window in its current size and position. This value is similar to SW_SHOW, except that the window is not activated.

            public const int SW_SHOWNOACTIVATE = 4;
            //Displays a window in its most recent size and position. This value is similar to SW_SHOWNORMAL, except that the window is not activated.

            public const int SW_SHOWNORMAL = 1;
            //Activates and displays a window. If the window is minimized or maximized, the system restores it to its original size and position. An application should specify this flag when displaying the window for the first time.
        }


        //https://www.pinvoke.net/default.aspx/user32.setwindowpos
        [DllImport("user32.dll", EntryPoint = "SetWindowPos", SetLastError = DEFAULT_LAST_ERROR)]
        public static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int X, int Y, int cx, int cy, int uFlags);

        // Window handles (HWND) used for hWndInsertAfter
        public static class HwndInsertAfterInt
        {
            public static readonly IntPtr NoTopMost = new IntPtr(-2);
            // Places the window at the bottom of the Z order. If the hWnd parameter identifies a topmost window, the window loses its topmost status and is placed at the bottom of all other windows.

            public static readonly IntPtr TopMost = new IntPtr(-1);
            //Places the window above all non-topmost windows (that is, behind all topmost windows). This flag has no effect if the window is already a non-topmost window.

            public static readonly IntPtr Top = new IntPtr(0);
            // Places the window at the top of the Z order.

            public static readonly IntPtr Bottom = new IntPtr(1);
            //Places the window above all non-topmost windows. The window maintains its topmost position even when it is deactivated.
        }

        // https://msdn.microsoft.com/en-us/library/windows/desktop/ms633545(v=vs.85).aspx
        public class SetWindowPosFlags
        {
            public const int SWP_ASYNCWINDOWPOS = 0x4000;
            //If the calling thread and the thread that owns the window are attached to different input queues, the system posts the request to the thread that owns the window.This prevents the calling thread from blocking its execution while other threads process the request.

            public const int SWP_DEFERERASE = 0x2000;
            //Prevents generation of the WM_SYNCPAINT message.

            public const int SWP_DRAWFRAME = 0x0020;
            // Draws a frame (defined in the window's class description) around the window.

            public const int SWP_FRAMECHANGED = 0x0020;
            //Applies new frame styles set using the SetWindowLong function.Sends a WM_NCCALCSIZE message to the window, even if the window's size is not being changed. If this flag is not specified, WM_NCCALCSIZE is sent only when the window's size is being changed.

            public const int SWP_HIDEWINDOW = 0x0080;
            //Hides the window.

            public const int SWP_NOACTIVATE = 0x0010;
            //Does not activate the window.If this flag is not set, the window is activated and moved to the top of either the topmost or non-topmost group (depending on the setting of the hWndInsertAfter parameter).

            public const int SWP_NOCOPYBITS = 0x0100;
            //Discards the entire contents of the client area.If this flag is not specified, the valid contents of the client area are saved and copied back into the client area after the window is sized or repositioned.

            public const int SWP_NOMOVE = 0x0002;
            //Retains the current position (ignores X and Y parameters).

            public const int SWP_NOOWNERZORDER = 0x0200;
            //Does not change the owner window's position in the Z order.

            public const int SWP_NOREDRAW = 0x0008;
            //Does not redraw changes.If this flag is set, no repainting of any kind occurs. This applies to the client area, the nonclient area (including the title bar and scroll bars), and any part of the parent window uncovered as a result of the window being moved.When this flag is set, the application must explicitly invalidate or redraw any parts of the window and parent window that need redrawing.

            public const int SWP_NOREPOSITION = 0x0200;
            //Same as the SWP_NOOWNERZORDER flag.

            public const int SWP_NOSENDCHANGING = 0x0400;
            //Prevents the window from receiving the WM_WINDOWPOSCHANGING message.

            public const int SWP_NOSIZE = 0x0001;
            //Retains the current size (ignores the cx and cy parameters).

            public const int SWP_NOZORDER = 0x0004;
            //Retains the current Z order (ignores the hWndInsertAfter parameter).

            public const int SWP_SHOWWINDOW = 0x0040;
            //Displays the window.
        }

        // https://www.pinvoke.net/default.aspx/user32.getclassname
        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = DEFAULT_LAST_ERROR)]
        public static extern int GetClassName(IntPtr hWnd, StringBuilder lpClassName, int nMaxCount);


        // https://www.pinvoke.net/default.aspx/user32.sendmessage
        // SendMessage에 사용할 메시지 참고 https://docs.microsoft.com/en-us/windows/desktop/winmsg/about-messages-and-message-queues#system_defined
        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = DEFAULT_LAST_ERROR)]
        public static extern IntPtr SendMessage(IntPtr hWnd, int Msg, IntPtr wParam, StringBuilder lParam);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = DEFAULT_LAST_ERROR)]
        public static extern IntPtr SendMessage(IntPtr hWnd, int Msg, IntPtr wParam, [MarshalAs(UnmanagedType.LPWStr)] string lParam);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = DEFAULT_LAST_ERROR)]
        public static extern IntPtr SendMessage(IntPtr hWnd, int Msg, int wParam, [MarshalAs(UnmanagedType.LPWStr)] string lParam);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = DEFAULT_LAST_ERROR)]
        public static extern IntPtr SendMessage(IntPtr hWnd, int Msg, int wParam, ref IntPtr lParam);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = DEFAULT_LAST_ERROR)]
        public static extern IntPtr SendMessage(IntPtr hWnd, int Msg, int wParam, IntPtr lParam);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = DEFAULT_LAST_ERROR)]
        public static extern IntPtr SendMessage(IntPtr hWnd, uint Msg, int wParam, StringBuilder lParam);

        public class WmMessages
        {
            // https://docs.microsoft.com/en-us/windows/desktop/winmsg/window-notifications
            //Copies the text that corresponds to a window into a buffer provided by the caller.
            public const int WM_CLOSE = 0x0010;

            // https://docs.microsoft.com/en-us/windows/desktop/winmsg/window-messages
            //Sent as a signal that a window or an application should terminate.
            public const int WM_GETTEXT = 0x000D;
        }



        // https://www.pinvoke.net/default.aspx/user32.getwindowrect
        [DllImport("user32.dll", SetLastError = DEFAULT_LAST_ERROR)]
        public static extern bool GetWindowRect(IntPtr hwnd, out RECT lpRect);

        [StructLayout(LayoutKind.Sequential)]
        public struct RECT
        {
            public int Left;        // x position of upper-left corner
            public int Top;         // y position of upper-left corner
            public int Right;       // x position of lower-right corner
            public int Bottom;      // y position of lower-right corner
        }

        public static System.Drawing.Rectangle RECT2Rectangle(RECT rect)
        {
            //바이너리 호환이 되지 않으므로 이걸 Rectangle 로 변환합니다.
            return new System.Drawing.Rectangle(rect.Left, rect.Top, rect.Right, rect.Bottom);
        }

        public static RECT RectangleToRECT(System.Drawing.Rectangle rect)
        {
            //바이너리 호환이 되지 않으므로 이걸 RECT 로 변환합니다.
            RECT returnVal = new RECT();
            returnVal.Left = rect.X;
            returnVal.Top = rect.Y;
            returnVal.Right = rect.Width;
            returnVal.Bottom = rect.Height;
            return returnVal;
        }


        public class WindowSpec
        {
            private const int MAX_CHARS = 64;

            private IntPtr handle = IntPtr.Zero;
            public IntPtr Handle
            {
                get
                {
                    return handle;
                }
            }

            private string title = null;
            public string Title
            {
                get
                {
                    if (title == null)
                    {
                        return "";
                    }
                    else
                    {
                        return title;
                    }
                }
            }

            private string className = null;
            public string ClassName
            {
                get
                {
                    if (className == null)
                    {
                        return "";
                    }
                    else
                    {
                        return className;
                    }
                }
            }

            public static WindowSpec getSpec(IntPtr handle)
            {
                WindowSpec spec = new WindowSpec();

                spec.handle = handle;

                //해당 창으로부터 창제목을 가져옵니다.
                StringBuilder sbTitle = new StringBuilder(64);
                SendMessage(handle, WmMessages.WM_GETTEXT, sbTitle.Capacity, sbTitle);
                spec.title = sbTitle.ToString();

                //해당 창의 클래스 이름을 가져옵니다.
                StringBuilder sbClassName = new StringBuilder(64);
                GetClassName(handle, sbClassName, sbClassName.Capacity);
                spec.className = sbClassName.ToString();

                return spec;
            }

            public override string ToString()
            {
                StringBuilder sbStr = new StringBuilder();
                sbStr.Append("WindowSpec {");
                sbStr.Append("handle=");
                sbStr.Append(handle);
                sbStr.Append(", ");
                sbStr.Append("title=");
                sbStr.Append(title);
                sbStr.Append(", ");
                sbStr.Append("className=");
                sbStr.Append(className);
                sbStr.Append('}');
                return sbStr.ToString();
            }
        }
    }
}
