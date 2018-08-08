using ISR_System;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
 
using System.Windows.Forms;

namespace WindowsFormsApplication1
{
    public partial class WBlogin : Form
    {
        private WbBlockNewUrl MyWebBrower;
        private Form viewForm;
  
         string linl = "";
        public WBlogin()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            InitialWebbroswer();
        }

        #region MyWebBrower
        public void InitialWebbroswer()
        {
            try
            {

                MyWebBrower = new WbBlockNewUrl();
                //不显示弹出错误继续运行框（HP方可）
                MyWebBrower.ScriptErrorsSuppressed = true;
                MyWebBrower.BeforeNewWindow += new EventHandler<WebBrowserExtendedNavigatingEventArgs>(MyWebBrower_BeforeNewWindow);
                MyWebBrower.DocumentCompleted += new WebBrowserDocumentCompletedEventHandler(AnalysisWebInfo);

                MyWebBrower.Dock = DockStyle.Fill;

                //显示用的窗体

                viewForm = new Form();
                //viewForm.Icon=
                viewForm.ClientSize = new System.Drawing.Size(800, 600);
                viewForm.StartPosition = FormStartPosition.CenterScreen;
                viewForm.Controls.Clear();
                viewForm.Controls.Add(MyWebBrower);
                viewForm.FormClosing += new FormClosingEventHandler(viewForm_FormClosing);
                //显示窗体

                viewForm.Show();

                MyWebBrower.Url = new Uri("https://app.singlewindow.cn/cas/login?service=http%3A%2F%2Fwww.singlewindow.cn%2Fsinglewindow%2Flogin.jspx&logoutFlag=1&_swCardF=1");

            }
            catch (Exception ex)
            {
                throw ex;
            }

        }


        void MyWebBrower_BeforeNewWindow(object sender, WebBrowserExtendedNavigatingEventArgs e)
        {
            #region 在原有窗口导航出新页
            e.Cancel = true;//http://pro.wwpack-crest.hp.com/wwpak.online/regResults.aspx
            MyWebBrower.Navigate(e.Url);
            #endregion
        }
        protected void AnalysisWebInfo(object sender, WebBrowserDocumentCompletedEventArgs e)
        {

            WbBlockNewUrl myDoc = sender as WbBlockNewUrl;
                                          //  https://app.singlewindow.cn/cas/login?service=http:%2F%2Fwww.singlewindow.cn%2Fsinglewindow%2Flogin.jspx&logoutFlag=1&_swCardF=1
            if (myDoc.Url.ToString().IndexOf("https://app.singlewindow.cn/cas/login?service=http") >= 0)
            {


                HtmlElement KEYTX = null;
                HtmlElement submit = null;
                HtmlElementCollection a = myDoc.Document.GetElementsByTagName("Input");
                int aaa = 0;
                foreach (HtmlElement item in a)
                {
                    if (item.OuterHtml.IndexOf("password") > 0)
                    {
                        KEYTX = item;
                   
                    }
                    if (item.OuterHtml.IndexOf("loginbutton") > 0)
                    {
                        submit = item;
                        break;
                    }
                }
                if (KEYTX != null && KEYTX != null)
                {
                 
                    KEYTX.SetAttribute("Value", "12345678");
                }
                if (submit != null && submit != null)
                {
                    MessageBox.Show("登录成功！");
                    submit.InvokeMember("Click");
                   
                }


                //  submit.InvokeMember("Click");
            }
        }
        private void viewForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            //if (toolStripStatusLabel1.Text != " Search Finished  !")
            {
                if (MessageBox.Show("正在进行，是否中止?", "Sign Out", MessageBoxButtons.OKCancel) == DialogResult.OK)
                {
                    if (MyWebBrower != null)
                    {
                        if (MyWebBrower.IsBusy)
                        {
                            MyWebBrower.Stop();
                        }
                        MyWebBrower.Dispose();
                        MyWebBrower = null;
                    }
                }
                else
                {
                    e.Cancel = true;
                }
            }
        }

        #endregion
    }
}
