using DAL;
using Microsoft.Win32;
using mshtml;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;
//using System.Threading.Tasks;

namespace BLL
{
    public enum ProcessStatus
    {
        初始化,
        检索页面,
        结果页面,
        账号正确,
        账号错误,
        第二页面,
        第三页面
    }

    public class clsAllnew
    {
        private SHDocVw.InternetExplorer MyWebBrower;
        private object o = null;
        private bool isReadyForSearch = false;
        private bool isOneFinished = false;
        private ProcessStatus isrun = ProcessStatus.初始化;
        private string tsbPassword;


        public List<DBC> ReadWeb_Report(string PASS)
        {

            //InitDefaultBrowser();
            //btnSetDefault_Click();
            btnReset_Click();
            tsbPassword = PASS;
            isrun = ProcessStatus.初始化;
            WEBconnection();

            return null;


        }
        #region IE
        public void WEBconnection()
        {
            MyWebBrower = new SHDocVw.InternetExplorer();

            MyWebBrower.Visible = true;
            MyWebBrower.DocumentComplete += new SHDocVw.DWebBrowserEvents2_DocumentCompleteEventHandler(AnalysisWebInfo);
            InitialBusinessLink();
        }
        public void InitialBusinessLink()
        {
                                //https://app.singlewindow.cn/cas/login?service=http%3A%2F%2Fwww.singlewindow.cn%2Fsinglewindow%2Flogin.jspx&logoutFlag=1&_swCardF=1
            MyWebBrower.Navigate("https://app.singlewindow.cn/cas/login?service=http%3A%2F%2Fwww.singlewindow.cn%2Fsinglewindow%2Flogin.jspx&logoutFlag=1&_swCardF=1", ref o, ref o, ref o, ref o);
            DateTime strFileName = DateTime.Now;//开始时间
            while (!isReadyForSearch)
            {

                System.Windows.Forms.Application.DoEvents();
            }
            MyWebBrower.Quit();
            isReadyForSearch = false;

        }

        protected void AnalysisWebInfo(object sender, ref object URL)
        {
            SetIE(IeVersion.标准ie8);
            #region  读取网站
            HTMLDocument myDoc = MyWebBrower.Document as HTMLDocument;
            //MessageBox.Show("1");

            if (URL.ToString().IndexOf("https://app.singlewindow.cn/cas/login?service=http") >= 0 && isrun == ProcessStatus.初始化)
            {
                //MessageBox.Show("2");


                IHTMLElement submit = null;
                IHTMLElement KEYTX = null;
                IHTMLInputElement txtkw = myDoc.getElementsByName("Login") as IHTMLInputElement;
                IHTMLElementCollection inputs = myDoc.getElementsByTagName("Input");



                foreach (IHTMLElement item in inputs)
                {
                    if (item.outerHTML.IndexOf("password") > 0)
                    {
                        KEYTX = item;

                    }
                    if (item.outerHTML.IndexOf("loginbutton") > 0)
                    {
                        submit = item;
                        break;
                    }
                }

                if (KEYTX != null && KEYTX != null)
                {
                    KEYTX.setAttribute("Value", tsbPassword, 0);
                }
                if (submit != null && submit != null)
                {
                    //MessageBox.Show("32");
                    isrun = ProcessStatus.账号正确;
                    //linshi 
                    //isrun = ProcessStatus.第二页面;
                    
                    submit.click();
                }

            }
            if (URL.ToString().IndexOf("https://www.singlewindow.cn/") >= 0 && isrun == ProcessStatus.账号正确)
            {
                IHTMLElementCollection inputs = myDoc.getElementsByTagName("a");
                foreach (IHTMLElement item in inputs)
                {
                    if (item.outerHTML.IndexOf("https://app.singlewindow.cn/userserver/user/index") > 0)
                    {

                        MessageBox.Show("" + item.outerText);


                    }
                    if (item.outerHTML.IndexOf("http://www.chinaport.gov.cn/kafgk/hgzs/18058.htm") > 0)
                    {

                        MessageBox.Show("" + item.outerText);
                        isReadyForSearch = true;
                    }
                }
                isrun = ProcessStatus.检索页面;
            }
            #endregion
        }
        #endregion

        #region MyRegion


        private void InitDefaultBrowser()
        {
            string defaultName = GetDefaultBrowerName();
            try
            {
                string browserPath = GetText(defaultName, "\"(?<key>.*?)\"", 1);
                FileInfo fileInfo = new FileInfo(browserPath);
                string fileName = fileInfo.Name.Replace(fileInfo.Extension, "");

            }
            catch (Exception ex)
            {

            }
        }

        private void btnSetDefault_Click()
        {
            SetDefaultBrowser(Application.ExecutablePath);

            InitDefaultBrowser();
        }

        private void btnReset_Click()
        {
            ResetIEDefaultBrowser();

            InitDefaultBrowser();
        }

        public static string GetText(string sInput, string sRegex, int iGroupIndex)
        {
            Regex re = new Regex(sRegex, RegexOptions.IgnoreCase | RegexOptions.IgnorePatternWhitespace | RegexOptions.Multiline);
            Match mc = re.Match(sInput);
            if (mc.Success)
            {
                if (iGroupIndex > 0)
                    return mc.Groups[iGroupIndex].Value;
                else
                    return mc.Value;
            }
            else
                return "";
        }
        public static string GetDefaultBrowerName()
        {
            string mainKey = @"http\shell\open\command";
            string nameKey = @"http\shell\open\ddeexec\Application";

            string strRet = string.Empty;
            try
            {
                RegistryKey regKey = Registry.ClassesRoot.OpenSubKey(mainKey);
                strRet = regKey.GetValue("").ToString();
            }
            catch
            {
                strRet = "";
            }
            return strRet;
        }

        /// <summary>
        /// 设置自定义浏览器为默认浏览器
        /// </summary>
        /// <param name="browserExePath"></param>
        /// <returns></returns>
        public static bool SetDefaultBrowser(string browserExePath)
        {
            string mainKey = @"http\shell\open\command";
            string nameKey = @"http\shell\open\ddeexec\Application";
            bool result = false;

            try
            {
                string value = string.Format("\"{0}\" \"%1\"", browserExePath);
                RegistryKey regKey = Registry.ClassesRoot.OpenSubKey(mainKey, true);
                regKey.SetValue("", value);
                regKey.Close();

                FileInfo fileInfo = new FileInfo(browserExePath);
                string fileName = fileInfo.Name.Replace(fileInfo.Extension, "");
                regKey = Registry.ClassesRoot.OpenSubKey(nameKey, true);
                regKey.SetValue("", fileName);
                regKey.Close();

                result = true;
            }
            catch (Exception ex)
            {

            }

            return result;
        }

        /// <summary>
        /// 恢复IE为默认浏览器
        /// </summary>
        /// <returns></returns>
        public static bool ResetIEDefaultBrowser()
        {
            string mainKey = @"http\shell\open\command";
            string nameKey = @"http\shell\open\ddeexec\Application";
            string IEPath = @"C:\Program Files\Internet Explorer\iexplore.exe";
            bool result = false;

            try
            {
                string value = string.Format("\"{0}\" -- \"%1\"", IEPath);
                RegistryKey regKey = Registry.ClassesRoot.OpenSubKey(mainKey, true);
                regKey.SetValue("", value);
                regKey.Close();

                regKey = Registry.ClassesRoot.OpenSubKey(nameKey, true);
                regKey.SetValue("", "IExplore");
                regKey.Close();

                result = true;
            }
            catch
            {
            }

            return result;
        }
        /// <summary>
        /// 定义IE版本的枚举
        /// </summary>
        private enum IeVersion
        {
            强制ie10,//10001 (0x2711) Internet Explorer 10。网页以IE 10的标准模式展现，页面!DOCTYPE无效 
            标准ie10,//10000 (0x02710) Internet Explorer 10。在IE 10标准模式中按照网页上!DOCTYPE指令来显示网页。Internet Explorer 10 默认值。
            强制ie9,//9999 (0x270F) Windows Internet Explorer 9. 强制IE9显示，忽略!DOCTYPE指令 
            标准ie9,//9000 (0x2328) Internet Explorer 9. Internet Explorer 9默认值，在IE9标准模式中按照网页上!DOCTYPE指令来显示网页。
            强制ie8,//8888 (0x22B8) Internet Explorer 8，强制IE8标准模式显示，忽略!DOCTYPE指令 
            标准ie8,//8000 (0x1F40) Internet Explorer 8默认设置，在IE8标准模式中按照网页上!DOCTYPE指令展示网页
            标准ie7//7000 (0x1B58) 使用WebBrowser Control控件的应用程序所使用的默认值，在IE7标准模式中按照网页上!DOCTYPE指令来展示网页
        }

        /// <summary>
        /// 设置WebBrowser的默认版本
        /// </summary>
        /// <param name="ver">IE版本</param>
        private void SetIE(IeVersion ver)
        {
            string productName = AppDomain.CurrentDomain.SetupInformation.ApplicationName;//获取程序名称

            object version;
            switch (ver)
            {
                case IeVersion.标准ie7:
                    version = 0x1B58;
                    break;
                case IeVersion.标准ie8:
                    version = 0x1F40;
                    break;
                case IeVersion.强制ie8:
                    version = 0x22B8;
                    break;
                case IeVersion.标准ie9:
                    version = 0x2328;
                    break;
                case IeVersion.强制ie9:
                    version = 0x270F;
                    break;
                case IeVersion.标准ie10:
                    version = 0x02710;
                    break;
                case IeVersion.强制ie10:
                    version = 0x2711;
                    break;
                default:
                    version = 0x1F40;
                    break;
            }

            RegistryKey key = Registry.CurrentUser;
            RegistryKey software =
                key.CreateSubKey(
                    @"Software\Microsoft\Internet Explorer\Main\FeatureControl\FEATURE_BROWSER_EMULATION\" + productName);
            if (software != null)
            {
                software.Close();
                //software.Dispose();
            }
            RegistryKey wwui =
                key.OpenSubKey(
                    @"Software\Microsoft\Internet Explorer\Main\FeatureControl\FEATURE_BROWSER_EMULATION", true);
            //该项必须已存在
            if (wwui != null) wwui.SetValue(productName, version, RegistryValueKind.DWord);
        }

        //显示代码
        #endregion

    }
}
