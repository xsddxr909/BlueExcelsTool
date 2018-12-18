namespace OPenExcel
{
    partial class Form1
    {
        /// <summary>
        /// 必需的设计器变量。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 清理所有正在使用的资源。
        /// </summary>
        /// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows 窗体设计器生成的代码

        /// <summary>
        /// 设计器支持所需的方法 - 不要
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.DataOutPutPathBtn = new System.Windows.Forms.Button();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.button2 = new System.Windows.Forms.Button();
            this.DataOutPutPathLab = new System.Windows.Forms.TextBox();
            this.progressBar1 = new System.Windows.Forms.ProgressBar();
            this.LogtextBox = new System.Windows.Forms.TextBox();
            this.button1 = new System.Windows.Forms.Button();
            this.button3 = new System.Windows.Forms.Button();
            this.DataOutPutLuaPathLab = new System.Windows.Forms.TextBox();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPage1.SuspendLayout();
            this.tabControl1.SuspendLayout();
            this.SuspendLayout();
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.DataOutPutLuaPathLab);
            this.tabPage1.Controls.Add(this.LogtextBox);
            this.tabPage1.Controls.Add(this.DataOutPutPathLab);
            this.tabPage1.Controls.Add(this.textBox1);
            this.tabPage1.Controls.Add(this.button3);
            this.tabPage1.Controls.Add(this.button1);
            this.tabPage1.Controls.Add(this.progressBar1);
            this.tabPage1.Controls.Add(this.button2);
            this.tabPage1.Controls.Add(this.DataOutPutPathBtn);
            this.tabPage1.Location = new System.Drawing.Point(4, 22);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(698, 476);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "tabPage1";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // DataOutPutPathBtn
            // 
            this.DataOutPutPathBtn.Location = new System.Drawing.Point(7, 42);
            this.DataOutPutPathBtn.Name = "DataOutPutPathBtn";
            this.DataOutPutPathBtn.Size = new System.Drawing.Size(99, 23);
            this.DataOutPutPathBtn.TabIndex = 12;
            this.DataOutPutPathBtn.Text = "Ts输出目录";
            this.DataOutPutPathBtn.UseVisualStyleBackColor = true;
            this.DataOutPutPathBtn.Click += new System.EventHandler(this.DataOutPutPathBtn_Click);
            // 
            // textBox1
            // 
            this.textBox1.Location = new System.Drawing.Point(7, 203);
            this.textBox1.Multiline = true;
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(337, 202);
            this.textBox1.TabIndex = 6;
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(6, 424);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(76, 38);
            this.button2.TabIndex = 4;
            this.button2.Text = "生成";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // DataOutPutPathLab
            // 
            this.DataOutPutPathLab.Location = new System.Drawing.Point(117, 42);
            this.DataOutPutPathLab.Name = "DataOutPutPathLab";
            this.DataOutPutPathLab.ReadOnly = true;
            this.DataOutPutPathLab.Size = new System.Drawing.Size(567, 21);
            this.DataOutPutPathLab.TabIndex = 13;
            this.DataOutPutPathLab.Text = "DATA生成路径1";
            // 
            // progressBar1
            // 
            this.progressBar1.Location = new System.Drawing.Point(88, 424);
            this.progressBar1.Name = "progressBar1";
            this.progressBar1.Size = new System.Drawing.Size(596, 38);
            this.progressBar1.TabIndex = 5;
            // 
            // LogtextBox
            // 
            this.LogtextBox.Location = new System.Drawing.Point(347, 203);
            this.LogtextBox.Multiline = true;
            this.LogtextBox.Name = "LogtextBox";
            this.LogtextBox.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.LogtextBox.Size = new System.Drawing.Size(337, 202);
            this.LogtextBox.TabIndex = 9;
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(7, 6);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(98, 23);
            this.button1.TabIndex = 0;
            this.button1.Text = "选择";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // button3
            // 
            this.button3.Location = new System.Drawing.Point(7, 81);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(98, 23);
            this.button3.TabIndex = 21;
            this.button3.Text = "Lua输出目录";
            this.button3.UseVisualStyleBackColor = true;
            this.button3.Click += new System.EventHandler(this.button3_Click);
            // 
            // DataOutPutLuaPathLab
            // 
            this.DataOutPutLuaPathLab.Location = new System.Drawing.Point(117, 81);
            this.DataOutPutLuaPathLab.Name = "DataOutPutLuaPathLab";
            this.DataOutPutLuaPathLab.ReadOnly = true;
            this.DataOutPutLuaPathLab.Size = new System.Drawing.Size(567, 21);
            this.DataOutPutLuaPathLab.TabIndex = 22;
            this.DataOutPutLuaPathLab.Text = "Lua生成路径(Lua/GameData下)";
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabPage1);
            this.tabControl1.Location = new System.Drawing.Point(12, 12);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(706, 502);
            this.tabControl1.TabIndex = 15;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(730, 527);
            this.Controls.Add(this.tabControl1);
            this.Name = "Form1";
            this.Text = "数据序列化";
            this.tabPage1.ResumeLayout(false);
            this.tabPage1.PerformLayout();
            this.tabControl1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.TextBox DataOutPutLuaPathLab;
        private System.Windows.Forms.TextBox LogtextBox;
        private System.Windows.Forms.TextBox DataOutPutPathLab;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.Button button3;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.ProgressBar progressBar1;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Button DataOutPutPathBtn;
        private System.Windows.Forms.TabControl tabControl1;
    }
}

