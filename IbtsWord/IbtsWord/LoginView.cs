using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Net;

namespace IbtsWord
{
    public partial class LoginView : Form
    {
        #region 自定义函数

        private void Login()
        {
            PostMessage pm = new PostMessage();
            //pm.getCapchta();
            bool result = pm.login(tb_loginCode.Text, tb_password.Text, tb_captcha.Text);
            if (result)
            {
                MainView mv = new MainView();
                mv.Show();
                this.Hide();
            }
        }

        #endregion
        public LoginView()
        {
            InitializeComponent();
        }

        private void btn_login_Click(object sender, EventArgs e)
        {
            this.Login();
        }

        private void LoginView_Load(object sender, EventArgs e)
        {
            PostMessage pm = new PostMessage();
            using (var sr = pm.getCapchta())
            { 
                pictureBox1.Image = new Bitmap(sr);

                if (sr != null)
                {
                    sr.Close();
                    sr.Dispose();
                }
            }
                      
        }  

        private void LoginView_KeyUp(object sender, KeyEventArgs e)
        { 
            if (e.KeyCode == Keys.Enter)
            {
                this.Login();
            }
        }
    }
}
