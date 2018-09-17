namespace KaTalkEspresso
{
    partial class Form1
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
            this.components = new System.ComponentModel.Container();
            this.step01 = new System.Windows.Forms.Label();
            this.step02 = new System.Windows.Forms.Label();
            this.step03 = new System.Windows.Forms.Label();
            this.step00 = new System.Windows.Forms.Label();
            this.waitForKatalk = new System.Windows.Forms.Timer(this.components);
            this.waitForExit = new System.Windows.Forms.Timer(this.components);
            this.btnShowLog = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // step01
            // 
            this.step01.AutoSize = true;
            this.step01.Location = new System.Drawing.Point(12, 87);
            this.step01.Name = "step01";
            this.step01.Size = new System.Drawing.Size(244, 50);
            this.step01.TabIndex = 0;
            this.step01.Text = "1. Launching KakaoTalk\r\n카카오톡 실행중";
            // 
            // step02
            // 
            this.step02.AutoSize = true;
            this.step02.Location = new System.Drawing.Point(12, 169);
            this.step02.Name = "step02";
            this.step02.Size = new System.Drawing.Size(300, 50);
            this.step02.TabIndex = 0;
            this.step02.Text = "2. Hiding ads from KakaoTalk.\r\n카카오톡에서 광고 숨기는 중";
            // 
            // step03
            // 
            this.step03.AutoSize = true;
            this.step03.Location = new System.Drawing.Point(12, 252);
            this.step03.Name = "step03";
            this.step03.Size = new System.Drawing.Size(490, 50);
            this.step03.TabIndex = 0;
            this.step03.Text = "3. Finished. This window will close in few seconds\r\n완료. 수 초 내로 종료됩니다.";
            // 
            // step00
            // 
            this.step00.AutoSize = true;
            this.step00.Location = new System.Drawing.Point(12, 9);
            this.step00.Name = "step00";
            this.step00.Size = new System.Drawing.Size(241, 50);
            this.step00.TabIndex = 0;
            this.step00.Text = "0. Searching KakaoTalk\r\n카카오톡 찾는중";
            // 
            // waitForKatalk
            // 
            this.waitForKatalk.Interval = 1000;
            this.waitForKatalk.Tick += new System.EventHandler(this.waitForKatalk_Tick);
            // 
            // waitForExit
            // 
            this.waitForExit.Interval = 1000;
            this.waitForExit.Tick += new System.EventHandler(this.waitForExit_Tick);
            // 
            // btnShowLog
            // 
            this.btnShowLog.Location = new System.Drawing.Point(17, 320);
            this.btnShowLog.Name = "btnShowLog";
            this.btnShowLog.Size = new System.Drawing.Size(540, 48);
            this.btnShowLog.TabIndex = 1;
            this.btnShowLog.Text = "show logs, 로그 보기";
            this.btnShowLog.UseVisualStyleBackColor = true;
            this.btnShowLog.Click += new System.EventHandler(this.btnShowLog_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(12F, 25F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(569, 380);
            this.Controls.Add(this.btnShowLog);
            this.Controls.Add(this.step03);
            this.Controls.Add(this.step02);
            this.Controls.Add(this.step00);
            this.Controls.Add(this.step01);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.Name = "Form1";
            this.Text = "KatalkEspresso";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Form1_FormClosing);
            this.Load += new System.EventHandler(this.Form1_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label step01;
        private System.Windows.Forms.Label step02;
        private System.Windows.Forms.Label step03;
        private System.Windows.Forms.Label step00;
        private System.Windows.Forms.Timer waitForKatalk;
        private System.Windows.Forms.Timer waitForExit;
        private System.Windows.Forms.Button btnShowLog;
    }
}

