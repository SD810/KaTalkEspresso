using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace KaTalkEspresso
{
    public partial class Form1 : Form
    {
		private static readonly string[] TITLE_KAKAOTALK_STRINGS = { "KakaoTalk", "카카오톡", "カカオトーク" };

		//로그
		private readonly Logger log = Logger.getInstance();

        //카카오톡 경로
        private string predefinedKaTalkPath = null;

        // 카카오톡 메인 창 핸들
        private IntPtr hWndKaTalk = IntPtr.Zero;

        //현재 스텝
        private int step = -1;

        //종료 타이머;
        private int exitTimer = 5;

        public Form1()
        {
            InitializeComponent();
        }
        
        private void Form1_Load(object sender, EventArgs e)
        {
            // 기본 색 지정
            this.BackColor = Color.Yellow;
            displayStep();
            
            log.info("KaTalkEspresso started");

            // 시작
            waitForKatalk.Enabled = true;
            step = 0;
        }

        /// <summary>
        /// 현재 단계에 맞춰 연한 색에서 진한색을 늘려갑니다.
        /// </summary>
        private void displayStep()
        {
            Color colorNonActive = Color.LightGray;
            Color colorActive = Color.Black;
            step00.ForeColor = colorNonActive;
            step01.ForeColor = colorNonActive;
            step02.ForeColor = colorNonActive;
            step03.ForeColor = colorNonActive;
            step03.ForeColor = colorNonActive;

            switch (step)
            {
                default:
                    break;
                case 0:
                    step00.ForeColor = colorActive;
                    break;
                case 1:
                case 2:
                    step00.ForeColor = colorActive;
                    step01.ForeColor = colorActive;
                    break;
                case 3:
                    step00.ForeColor = colorActive;
                    step01.ForeColor = colorActive;
                    step02.ForeColor = colorActive;
                    break;
                case 4:
                    step00.ForeColor = colorActive;
                    step01.ForeColor = colorActive;
                    step02.ForeColor = colorActive;
                    step03.ForeColor = colorActive;
                    break;
            }
        }

        /// <summary>
        /// 프로세스 목록에서 지정된 경로의 카카오톡 프로세스를 찾아 메인 창 핸들을 반환합니다.
        /// </summary>
        /// <param name="predefinedKaTalkPath">카카오톡 경로</param>
        /// <returns>카카오톡 메인 창 핸들</returns>
        private IntPtr detectKakaoTalk(string predefinedKaTalkPath)
        {
            IntPtr handle = IntPtr.Zero;

            if(predefinedKaTalkPath == null || "".Equals(predefinedKaTalkPath))
            {
                // 경로가 비어있으면 진행 불가
                log.error("path not provided. cannot continue.");

                return handle;
            }

            log.info("checking for running KakaoTalk's path provided as -> " + predefinedKaTalkPath);

            FileInfo katalkExe = new FileInfo(predefinedKaTalkPath);

            if (!katalkExe.Exists)
            {
                // 경로의 파일이 존재하지 않습니다.
                // 진행 불가
                log.error("Could not find -> " + katalkExe.FullName);

                return handle;
            }


            string katalkExeStr = katalkExe.Name;
            int lastExeFound = katalkExeStr.ToLower().LastIndexOf(".exe");
            if (lastExeFound < 0)
            {
                // exe 확장명을 경로로부터 찾지 못함. 검색 불가
                log.warn("\".exe\" from name not found. it may cannot find for \"" + katalkExeStr + "\" from running processes.");

                return handle;
            }
            katalkExeStr = katalkExeStr.Substring(0, lastExeFound);

            // 이 경로로 된 프로세스가 있는지 찾기 위해 프로세스 목록 수집.
            Process[] procs = Process.GetProcesses();


			// 카카오톡 친구목록 윈도우를 가져오기 위해 다른 채팅방을 닫는 작업
			foreach (Process proc in procs)
			{
				if (proc.ProcessName.Equals(katalkExeStr)) // 카카오톡 프로세스를 찾는다
				{
					if (TITLE_KAKAOTALK_STRINGS.Contains(proc.MainWindowTitle))
					{
						break; // 카카오톡 메인 페이지를 찾으면 반복문 중단
					}
					else // 프로세스에 해당하는 윈도우를 찾았으나, 친구 목록 윈도우가 아님
					{
						log.info("close subWindow : " + proc.MainWindowTitle);
						proc.CloseMainWindow();
						procs = Process.GetProcesses(); // 윈도우를 닫고 프로세스 목록을 다시 읽어옴
					}
				}
			}


			//카톡 exe 이름은 소문자로 바꾸고 이걸 비교에 사용
			katalkExeStr = katalkExeStr.ToLower();

            log.info("Checking for running " + procs.Length + " processes to pinpoint target executable.");
            foreach (Process proc in procs)
            {
                if (katalkExeStr.Equals(proc.ProcessName.ToLower()))//Util.toLowerComplyingFs(proc.ProcessName)))
                {
                    log.info("Found \"" + proc.ProcessName + "\" as candidate.");
                    //kakaotalk 프로세스 발견
                    // MainModule 속성 접근시 권한 문제 발생할 수 있으므로 catch
                    try
                    {
                        // 지정된 경로와 카카오톡 경로와 일치하는 경우
                        if (katalkExe.FullName.Equals(proc.MainModule.FileName))
                        {
                            log.info("this process matches with provided path. using this as further use.");
                            handle = proc.MainWindowHandle;//카톡 메인창 핸들
                            //handle = proc.Handle;// 카톡 핸들    
                        //MessageBox.Show("detection successful");
                        }
                    }
                    catch (Win32Exception w32e)
                    {
                        // Access Denied 문제 발생
                        log.error("Exception raised. Maybe could not access process info?");
                        log.error(w32e.ToString());
                    }
                }
            }
            return handle;
        }

        /// <summary>
        /// 카카오톡이 같은 폴더에 있으면  반환합니다.
        /// </summary>
        /// <returns>카톡 exe, 같은 폴더에 있으니 파일 이름만 있어도 된다</returns>
        private string openKakaoSameDir()
        {
            string exeOnSameDir = null;
            try
            {
                //같은 폴더에 카카오톡 실행파일이 있는지 확인
                string filename = "KakaoTalk.exe";
                FileInfo katalkExe = new FileInfo(filename);
                if (katalkExe.Exists)
                {
                    // 같은 폴더에 있음
                    exeOnSameDir = filename;
                }
                else
                {
                    //같은 경로에 없음
                    log.warn(filename + " not found on same dir");
                }
            }
            catch (Exception e)
            {
                log.warn("Exception raised during registry search for kakaoopen");
                log.warn(e.ToString());
            }
            return exeOnSameDir;
        }

        /// <summary>
        /// 레지스트리의 카카오톡 kakaoopen 키를 읽어 경로를 찾아냅니다.
        /// </summary>
        /// <returns>kakaoopen키 상의 카카오톡 경로</returns>
        private string openKakaoRegistryLocation()
        {
            // 추출한 경로가 기억될 변수
            string openKakaoReg = null;

            // 레지스트리를 닫아야 하므로 try 밖에서 선언
            RegistryKey kakaoOpen = null;
            try
            {
                // HKCR에서 카카오open 커맨드 레지스트리를 읽어 가져옵니다.
                kakaoOpen = Registry.ClassesRoot.OpenSubKey(@"kakaoopen\shell\open\command\", false);

                if (kakaoOpen == null)
                {
                    //레지스트리 값이 존재하지 않으면 검색할 필요가 없음.
                    log.warn("registry for kakaoopen not found");

                    return null;
                }
                // 레지스트리 (기본값)을 읽어옴
                //"C:\Program Files (x86)\Kakao\KakaoTalk\KakaoTalk.exe" "%1"
                openKakaoReg = (string)kakaoOpen.GetValue("");

                log.info("registry entry (default) found for kakaoopen, value -> " + openKakaoReg);

                bool exeExtracted = false;
                //\".*?\" 이전 원소 0 이상 최소한으로 일치.
                foreach (Match match in Regex.Matches(openKakaoReg, "\"([^\"]*)\""))
                {
                    // 앞뒤 따옴표 제거
                    string matchSub = match.ToString();

                    // subString시 startIndex의 글자는 포함되므로 + 1
                    int firstQuote = matchSub.IndexOf('\"') + 1;
                    // subString시 endIndex의 글자는 포함되지 않으므로 그대로 둠
                    int lastQuote = matchSub.LastIndexOf('\"');

                    if (firstQuote < 0 || firstQuote == lastQuote)
                    {
                        // 따옴표를 못 찾았습니다.
                        // 또는
                        // 마지막 따옴표가 첫 따옴표가 같으면 완전히 감싸진 모양이 아님

                        //다음 항목으로 넘어감
                        continue;
                    }
                    if (matchSub.ToLower().Contains(".exe"))
                    {
                        // 경로에 실행파일이 포함된 경우 따옴표 제거된 문자열을 알아둠
                        matchSub = matchSub.Substring(firstQuote, lastQuote - firstQuote);
                        openKakaoReg = matchSub;
                        
                        exeExtracted = true;
                    }
                }

                if (exeExtracted)
                {
                    // 경로 추출 성공
                    log.info("file path from registry extracted -> " + openKakaoReg);
                }
                else
                {
                    // 경로 추출 실패
                    log.warn("failed to extract file path from registry"); 
                }

            }
            catch (Exception e)
            {
                log.warn("Exception raised during registry search for kakaoopen");
                log.warn(e.ToString());
            }
            finally
            {
                // 레지스트리를 접근했으면 닫아줘야 한다.
                if(kakaoOpen != null)
                {
                    try
                    {
                        log.info("closing registry handle for kakaoopen");

                        kakaoOpen.Close();
                    }
                    catch (Exception e)
                    {
                        log.warn("Exception raised during closing registry handle for kakaoopen");
                        log.warn(e.ToString());
                    }
                }
            }

            return openKakaoReg;
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            log.info("closing window");
        }

        private void waitForKatalk_Tick(object sender, EventArgs e)
        {
            doStep();
        }

        /// <summary>
        /// 단계별 작업을 실행합니다.
        /// 0. 카카오톡 경로를 찾기
        /// 1. 카카오톡 프로세스 실행
        /// 2. 카카오톡 프로세스 찾기
        /// 3. 광고 숨기기
        /// 4. 종료
        /// </summary>
        private void doStep()
        {
            displayStep();

            switch (step)
            {
                case 0:
                    //카톡 경로를 검색하는 동안 타이머 중지
                    waitForKatalk.Enabled = false;
                    //레지스트리에서 카톡 검색
                    predefinedKaTalkPath = openKakaoRegistryLocation();

                    if (predefinedKaTalkPath == null)
                    {
                        //레지스트리에 카톡이 없으면 같은 폴더에서 카톡 검색
                        predefinedKaTalkPath = openKakaoSameDir();
                    }

                    if (predefinedKaTalkPath != null)
                    {
                        // 카톡 경로 찾음
                        step00.Text = "Found KakaoTalk.\n카카오톡을 찾았습니다.";

                        step++;
                        doStep();
                    }
                    else
                    {
                        // 카톡 경로 못 찾음
                        step00.Text = "Could not find KakaoTalk\n카카오톡 찾지 못 함";
                    }
                    break;

                case 1:
                    //실행중인지 확인
                    if (runningKakaoTalk())
                    {
                        //카톡 핸들을 찾았다. 다음 단계로
                        step++;
                        doStep();
                    }
                    else
                    {
                        try
                        {
                            //카톡 시작
                            
                            ProcessStartInfo katalkExe = new ProcessStartInfo(predefinedKaTalkPath);
                            Process.Start(katalkExe);
                            
                            //다음 단계로
                            step++;

                            //카톡 경로를 검색하는 동안 타이머 시작
                            waitForKatalk.Enabled = true;
                        }
                        catch (Exception launchEx)
                        {
                            // 실행도중 예외 발생
                            log.warn("Cannot start " + predefinedKaTalkPath);
                            log.warn(launchEx.ToString());
                        }
                    }
                    break;

                case 2:

                    // 카톡 실행 대기 중
                    if (runningKakaoTalk())
                    {
                        //카톡 핸들을 찾았다. 
                        step01.Text = "KakaoTalk is running.\n카카오톡이 실행중입니다.";
                        //카톡 대기 타이머 중지
                        //waitForKatalk.Enabled = false;
                        //다음 단계로
                        step++;
                        doStep();
                    }
                    break;

                case 3:
                    //광고 숨김
                    int hideResult = hideAd();
                    log.info("hide ads Result: " + hideResult);
                    step02.Text = "Tried hiding ads\n광고 숨김 시도함";

                    //광고가 숨겨진 경우에만 타이머를 멈춤.
                    if((hideResult & AdCloser.AdCloseResult.NOT_CLOSED_ADS_ON_FRIENDS_LIST) == 0)
                    {

                        //카톡 대기 타이머 중지
                        waitForKatalk.Enabled = false;
                    
                        // 다음 단계로
                        step++;

                        doStep();

                    }
                    else
                    {
                        // 이전 단계로 돌아가 다시 카카오톡을 찾습니다.
                        step--;
                    }

                    break;
                case 4:
                    //완료
                    waitForExit.Enabled = true;
                    break;
            }

            displayStep();
        }

        private bool runningKakaoTalk()
        {
            // 카톡 실행 대기 중
            // 카톡 핸들을 찾고 알아둡니다.
            hWndKaTalk = detectKakaoTalk(predefinedKaTalkPath);

            // 핸들이 null이 아닌지 확인
            return !IntPtr.Zero.Equals(hWndKaTalk);
        }

        private int hideAd()
        {
            // 카톡 닫기 결과를 반환합니다.
            return AdCloser.closeAdsKakaoTalk(hWndKaTalk);
        }
            
        private void waitForExit_Tick(object sender, EventArgs e)
        {
            exitTimer--;

            if(exitTimer <= 0)
            {
                //지정한 시간이 지남

                //닫기
                this.Close();
            }
        }

        private void btnShowLog_Click(object sender, EventArgs e)
        {
            if (waitForExit.Enabled)
            {
                // 자동 종료 끄기
                waitForExit.Enabled = false;
            }

            if (log.showLogNow())
            {
                // 창이 열린 동안 이구간으로 진입되지 않는다. Show는 모달인듯
            }
        }
    }
}
