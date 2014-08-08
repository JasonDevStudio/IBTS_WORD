using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Collections;
using System.IO;
using System.Runtime.Serialization.Json;

namespace IbtsWord
{
    public partial class MainView : Form
    { 

        public MainView()
        {
            InitializeComponent();
        }

        private void btn_getTaskList_Click(object sender, EventArgs e)
        {
            PostMessage pm = new PostMessage();
            string result = string.Empty;

            StreamReader strRead = new StreamReader(AppDomain.CurrentDomain.BaseDirectory + "json.txt");
            result = strRead.ReadToEnd();
                
            //pm.getTaskList();

            System.Data.DataTable table = new DataTable("ParentTable");
            // Declare variables for DataColumn and DataRow objects.
            DataColumn column;
            DataRow row;           
             

            table.Columns.AddRange(new DataColumn[] { 
                                         new DataColumn("taskId", typeof(string)),
                                         new DataColumn("taskNo", typeof(string)),
                                         new DataColumn("agentCode", typeof(string)),
                                         new DataColumn("operateDate", typeof(string)),
                                         new DataColumn("stageName", typeof(string)),
                                         new DataColumn("stateName", typeof(string))});
            //table.Columns.Add("taskNo", typeof(string));

            Hashtable hs = (Hashtable)JsonUtil.getObject(result);
            // Create three sets of DataRow objects, 
            // five rows each, and add to DataTable.

            Hashtable hs1 = (Hashtable)hs["pagingDTO"];
            foreach (Hashtable hs2 in (ArrayList)hs1["resultList"])
            {
                row = table.NewRow();
                row["taskId"] = hs2["taskId"];
                row["taskNo"] = hs2["taskNo"];
                row["agentCode"] = ((Hashtable)hs2["agentDTO"])["agentCode"];
                row["operateDate"] = hs2["operateDate"];
                row["stageName"] = hs2["stageName"];
                row["stateName"] = hs2["stateName"];

                table.Rows.Add(row);
            }


            dvTaskList.Columns.Clear();

            //取消dataGridView1按默认方式显示
            dvTaskList.AutoGenerateColumns = false;
            
            dvTaskList.DataSource = table;

            DataGridViewTextBoxColumn column1 = new DataGridViewTextBoxColumn();
            column1.DataPropertyName = "taskId";//SQL语句得到的列名，可从集合中获得
            column1.HeaderText = "Task Id";//列头显示的汉字dtcTimeStamp.Width = 110;
            dvTaskList.Columns.Add(column1);//最后一定要添加进去

            DataGridViewTextBoxColumn column2 = new DataGridViewTextBoxColumn();
            column2.DataPropertyName = "taskNo";//SQL语句得到的列名，可从集合中获得
            column2.HeaderText = "Task No";//列头显示的汉字dtcTimeStamp.Width = 110;
            dvTaskList.Columns.Add(column2);//最后一定要添加进去

            DataGridViewTextBoxColumn column3 = new DataGridViewTextBoxColumn();
            column3.DataPropertyName = "agentCode";//SQL语句得到的列名，可从集合中获得
            column3.HeaderText = "IIN";//列头显示的汉字dtcTimeStamp.Width = 110;
            dvTaskList.Columns.Add(column3);//最后一定要添加进去

            DataGridViewTextBoxColumn column4 = new DataGridViewTextBoxColumn();
            column4.DataPropertyName = "stageName";//SQL语句得到的列名，可从集合中获得
            column4.HeaderText = "progession";//列头显示的汉字dtcTimeStamp.Width = 110;
            dvTaskList.Columns.Add(column4);//最后一定要添加进去

            DataGridViewTextBoxColumn column5 = new DataGridViewTextBoxColumn();
            column5.DataPropertyName = "stateName";//SQL语句得到的列名，可从集合中获得
            column5.HeaderText = "state";//列头显示的汉字dtcTimeStamp.Width = 110;
            dvTaskList.Columns.Add(column5);//最后一定要添加进去

            DataGridViewButtonColumn column6 = new DataGridViewButtonColumn();
            //设定列的名字 
            column6.Name = "Button";
            //在所有按钮上表示"点击阅览" 
            column6.UseColumnTextForButtonValue = true;
            column6.Text = "Operation";
            dvTaskList.Columns.Add(column6);//最后一定要添加进去

        }

        private void dvTaskList_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            DataGridView dgv = (DataGridView)sender;//这行语句也可以不要，如果已经创建了dgv，详见航道系统的代码。 
            //如果是"Button"列，按钮被点击 
            if (dgv.Columns[e.ColumnIndex].Name == "Button")//此处索引列可以使name、也可以使headertext，看具体的设置。      
            { 
                MessageBox.Show(e.RowIndex.ToString() +              "行的按钮被点击了。");      
            }
        }

        private void MainView_FormClosed(object sender, FormClosedEventArgs e)
        {
            Application.Exit();
        }
    }
}
