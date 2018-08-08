using BLL;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
//using System.Threading.Tasks;
using System.Windows.Forms;

namespace CITS
{
    public partial class frmlogin : Form
    {
        public frmlogin()
        {
            InitializeComponent();
        }

   

        private void btlogin_Click(object sender, EventArgs e)
        {  
            #region Noway
            DateTime oldDate = DateTime.Now;
            DateTime dt3;
            string endday = DateTime.Now.ToString("yyyy/MM/dd");
            dt3 = Convert.ToDateTime(endday);
            DateTime dt2;
            dt2 = Convert.ToDateTime("2018/08/08");

            TimeSpan ts = dt2 - dt3;
            int timeTotal = ts.Days;
            if (timeTotal < 0)
            {
                MessageBox.Show("运行期已到，请将剩余费用付清 !");
                return;
            }
            // MessageBox.Show("当前为测试系统 !");

            #endregion

            if (txkey.Text =="")
            {
                MessageBox.Show("请写入密钥,然后重试");
                return;
            }
            clsAllnew BusinessHelp = new clsAllnew();
            BusinessHelp.ReadWeb_Report(txkey.Text);
            MessageBox.Show("login OK !", "Right", MessageBoxButtons.OK, MessageBoxIcon.Information);

        
        }
    }
}
