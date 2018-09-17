using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KaTalkEspresso
{
    class AdCloser
    {
        private static Logger LOG = Logger.getInstance();

        public class AdCloseResult
        {
            public const int NO_PROBLEM = 0b0000;                        // 문제 없음
            public const int NOT_IMPLEMENTED = 0b001;                    // 아직 구현되지 않음
            public const int NOT_CLOSED_ADS_ON_FRIENDS_LIST = 0b0100;    // 친구 목록 내 광고 제거 안됨
            public const int NOT_CLOSED_ADS_ON_POPUP = 0b0010;           // 팝업 광고 제거 안됨
            public const int EXCEPTION_RAISED = 0b1000;                  // 예외 발생
        }


        // 카카오톡 클래스 이름
        private const string CLASSNAME_KAKAOTALK = "EVA_ChildWindow";
        private const string CLASSNAME_KAKAOTALK_AD = "EVA_Window";
        private static readonly string[] CLASSNAMES_ADS_STRINGS = {"FAKE_WND_REACHPOP" };
        // 카카오톡 창 제목 문자열
        private static readonly string[] TITLE_KAKAOTALK_STRINGS = { "KakaoTalk", "카카오톡", "カカオトーク" };

        /// <summary>
        /// 카카오톡의 광고를 숨김 처리합니다.
        /// </summary>
        /// <param name="katalkHWnd">카카오톡 메인창 핸들</param>
        /// <returns>반환 결과, AdCloseResult 참고</returns>
        public static int closeAdsKakaoTalk(IntPtr katalkHWnd)
        {
            LOG.info("supplied Handle: " + katalkHWnd);

            int returnVal = AdCloseResult.NO_PROBLEM;

            bool adOnListHidden = false;
            //bool adOnPopupHidden = false;

            try
            {
                // intPtr는 null이 없는 타입 non-nullable
                IntPtr hWndParent = WinAPI.GetWindowLong(katalkHWnd, WinAPI.WindowLongFlags.GWLP_HWNDPARENT);
                bool parentIsNull = IntPtr.Zero.Equals(hWndParent);
                bool parentIsDesktop = WinAPI.GetDesktopWindow().Equals(hWndParent);
                // IntPtr 끼리는 비트연산자 사용불가.
                // 따라서 Int64로 변환 후 비트 연산자 사용
                bool isNotToolWindow = (WinAPI.GetWindowLong(katalkHWnd, WinAPI.WindowLongFlags.GWL_EXSTYLE).ToInt64() & WinAPI.ExtendedWindowStyles.WS_EX_TOOLWINDOW) == 0L;



                if ((parentIsNull || parentIsDesktop) && isNotToolWindow)
                {
                    //계속할 수 있습니다.
                    LOG.info("provided parent is valid and provided windows is not tool window.");

                    //메인 윈도우 찾기
                    //foreach (var title in TITLE_KAKAOTALK_STRINGS) {
                    //IntPtr handleMainWnd = WinAPI.FindWindowEx(katalkHWnd, IntPtr.Zero, CLASSNAME_KAKAOTALK, null);
                    //if (!IntPtr.Zero.Equals(handleMainWnd))
                    //{
                    //    LOG.info("found handle for main window: " + handleMainWnd);
                    //    katalkHWnd = handleMainWnd;
                    //    break;
                    //}
                    //}


                    WinAPI.WindowSpec mainWndSpec = WinAPI.WindowSpec.getSpec(katalkHWnd);

                    // 카톡인지 확인합니다.
                    bool isKatalkWnd = false;

                    

                    if (CLASSNAME_KAKAOTALK.Equals(mainWndSpec.ClassName))
                    {
                        // 클래스 이름이 일치합니다.

                        foreach (string validTitle in TITLE_KAKAOTALK_STRINGS)
                        {
                            if (validTitle.Equals(mainWndSpec.Title))
                            {
                                LOG.info("found Kakaotalk's main Window: "+ mainWndSpec.Title);
                                // 카카오톡 창 제목입니다.
                                isKatalkWnd = true;
                                break;
                            }
                        }
                    }
                    else
                    {
                        LOG.info("it may not be KakaoTalk's main window");
                        foreach (string suspectAdClassName in CLASSNAMES_ADS_STRINGS)
                        {
                            if (suspectAdClassName.Equals(mainWndSpec.ClassName))
                            {
                                LOG.info("closing popup ad: " + mainWndSpec.ClassName);
                                WinAPI.SendMessage(katalkHWnd, WinAPI.WmMessages.WM_CLOSE, 0, null);
                            }
                        }
                    }

                    if (isKatalkWnd || true)
                    {
                        LOG.info("It seems KakaoTalk Main window.");
                        // 카카오톡 창이 확실해보입니다.
                        WinAPI.RECT rectKatalkMain = new WinAPI.RECT();

                        WinAPI.GetWindowRect(katalkHWnd, out rectKatalkMain);
                        
                        IntPtr hwndChildAd = WinAPI.FindWindowEx(katalkHWnd, IntPtr.Zero, CLASSNAME_KAKAOTALK_AD, null);
                        LOG.info("found child ad: " + WinAPI.WindowSpec.getSpec(hwndChildAd));
                        if(!IntPtr.Zero.Equals(hwndChildAd))
                        {
                            LOG.info("trying to hide ad on friends list.");
                            // 의미 없다. 광고가 이미 숨겨진 경우에도 false가 나온다.
                            //adOnListHidden =
                            adOnListHidden = WinAPI.ShowWindow(hwndChildAd, WinAPI.ShowWindowCommands.SW_HIDE);
                            int listAdHideResult = WinAPI.GetLastWin32Error();
                            LOG.info("result after hiding ads on friend list: " + listAdHideResult);

                            adOnListHidden |= (listAdHideResult == 0) ;
                            
                            if (adOnListHidden)
                            {
                                LOG.info("tuning window to trim empty space");
                                IntPtr hwndFriendList = WinAPI.FindWindowEx(katalkHWnd, IntPtr.Zero, CLASSNAME_KAKAOTALK, null);
                                WinAPI.SetWindowPos(hwndChildAd, WinAPI.HwndInsertAfterInt.Bottom, 0, 0, 0, 0, WinAPI.SetWindowPosFlags.SWP_NOMOVE);
                                WinAPI.SetWindowPos(hwndFriendList, WinAPI.HwndInsertAfterInt.Bottom, 0, 0, (rectKatalkMain.Right - rectKatalkMain.Left), (rectKatalkMain.Bottom - rectKatalkMain.Top - 36), WinAPI.SetWindowPosFlags.SWP_NOMOVE);
                            }
                            else
                            {
                                LOG.error("could not hide ad on frineds list.");
                            }
                        }
                    }
                    else
                    {
                        LOG.error("could not verify it is KakaoTalk.");
                    }


                }
                else
                {
                    // 계속할 수 없습니다.
                    LOG.error("provided handle is not suitable.");
                }
            }
            catch (Exception e)
            {
                LOG.error("Exception raised.");
                LOG.error(e.ToString());
                returnVal = Util.setFlag(returnVal, AdCloseResult.EXCEPTION_RAISED);
            }

            if (!adOnListHidden)
            {
                LOG.error("ad on friend list is not hidden");
                returnVal = Util.setFlag(returnVal, AdCloseResult.NOT_CLOSED_ADS_ON_FRIENDS_LIST);
            }
            //if (!adOnPopupHidden)
            //{
            //    LOG.error("popup ad is not hidden");
            //    returnVal = Util.setFlag(returnVal, AdCloseResult.NOT_CLOSED_ADS_ON_POPUP);
            //}
            return returnVal;
        }
    }
}
