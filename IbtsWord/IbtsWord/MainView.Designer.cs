namespace IbtsWord
{
    partial class MainView
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.btn_getTaskList = new System.Windows.Forms.Button();
            this.dvTaskList = new System.Windows.Forms.DataGridView();
            this.taskId = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.taskNo = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.agentCode = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.operateDate = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.stageName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.stateName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.button1 = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.dvTaskList)).BeginInit();
            this.SuspendLayout();
            // 
            // btn_getTaskList
            // 
            this.btn_getTaskList.Location = new System.Drawing.Point(603, 6);
            this.btn_getTaskList.Margin = new System.Windows.Forms.Padding(2);
            this.btn_getTaskList.Name = "btn_getTaskList";
            this.btn_getTaskList.Size = new System.Drawing.Size(120, 23);
            this.btn_getTaskList.TabIndex = 2;
            this.btn_getTaskList.Text = "Get Task List";
            this.btn_getTaskList.UseVisualStyleBackColor = true;
            this.btn_getTaskList.Click += new System.EventHandler(this.btn_getTaskList_Click);
            // 
            // dvTaskList
            // 
            this.dvTaskList.AllowUserToAddRows = false;
            this.dvTaskList.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dvTaskList.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dvTaskList.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.taskId,
            this.taskNo,
            this.agentCode,
            this.operateDate,
            this.stageName,
            this.stateName});
            this.dvTaskList.Location = new System.Drawing.Point(11, 35);
            this.dvTaskList.Margin = new System.Windows.Forms.Padding(2);
            this.dvTaskList.Name = "dvTaskList";
            this.dvTaskList.RowTemplate.Height = 27;
            this.dvTaskList.Size = new System.Drawing.Size(712, 264);
            this.dvTaskList.TabIndex = 3;
            this.dvTaskList.CellContentClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dvTaskList_CellContentClick);
            // 
            // taskId
            // 
            this.taskId.HeaderText = "Task Id";
            this.taskId.Name = "taskId";
            // 
            // taskNo
            // 
            this.taskNo.HeaderText = "Task No";
            this.taskNo.Name = "taskNo";
            // 
            // agentCode
            // 
            this.agentCode.HeaderText = "IIN";
            this.agentCode.Name = "agentCode";
            // 
            // operateDate
            // 
            this.operateDate.HeaderText = "Operate Date";
            this.operateDate.Name = "operateDate";
            // 
            // stageName
            // 
            this.stageName.HeaderText = "Progession";
            this.stageName.Name = "stageName";
            // 
            // stateName
            // 
            this.stateName.HeaderText = "State";
            this.stateName.Name = "stateName";
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(479, 6);
            this.button1.Margin = new System.Windows.Forms.Padding(2);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(120, 23);
            this.button1.TabIndex = 4;
            this.button1.Text = "Read Word";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(355, 6);
            this.button2.Margin = new System.Windows.Forms.Padding(2);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(120, 23);
            this.button2.TabIndex = 5;
            this.button2.Text = "Read Word";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // MainView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(741, 310);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.dvTaskList);
            this.Controls.Add(this.btn_getTaskList);
            this.Margin = new System.Windows.Forms.Padding(2);
            this.MaximumSize = new System.Drawing.Size(757, 349);
            this.MinimumSize = new System.Drawing.Size(757, 349);
            this.Name = "MainView";
            this.Text = "Task List";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.MainView_FormClosed);
            ((System.ComponentModel.ISupportInitialize)(this.dvTaskList)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button btn_getTaskList;
        private System.Windows.Forms.DataGridView dvTaskList;
        private System.Windows.Forms.DataGridViewTextBoxColumn taskId;
        private System.Windows.Forms.DataGridViewTextBoxColumn taskNo;
        private System.Windows.Forms.DataGridViewTextBoxColumn agentCode;
        private System.Windows.Forms.DataGridViewTextBoxColumn operateDate;
        private System.Windows.Forms.DataGridViewTextBoxColumn stageName;
        private System.Windows.Forms.DataGridViewTextBoxColumn stateName;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button button2;
    }
}