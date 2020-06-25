namespace ChartApp
{
    partial class Main
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
            System.Windows.Forms.DataVisualization.Charting.ChartArea chartArea1 = new System.Windows.Forms.DataVisualization.Charting.ChartArea();
            System.Windows.Forms.DataVisualization.Charting.Legend legend1 = new System.Windows.Forms.DataVisualization.Charting.Legend();
            System.Windows.Forms.DataVisualization.Charting.Series series1 = new System.Windows.Forms.DataVisualization.Charting.Series();
            this.sysChart = new System.Windows.Forms.DataVisualization.Charting.Chart();
            this.btn_Cpu = new System.Windows.Forms.Button();
            this.btn_Memory = new System.Windows.Forms.Button();
            this.btn_Disk = new System.Windows.Forms.Button();
            this.btn_PauseResume = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize) (this.sysChart)).BeginInit();
            this.SuspendLayout();
            // 
            // sysChart
            // 
            chartArea1.Name = "ChartArea1";
            this.sysChart.ChartAreas.Add(chartArea1);
            this.sysChart.Dock = System.Windows.Forms.DockStyle.Fill;
            legend1.Name = "Legend1";
            this.sysChart.Legends.Add(legend1);
            this.sysChart.Location = new System.Drawing.Point(0, 0);
            this.sysChart.Name = "sysChart";
            series1.ChartArea = "ChartArea1";
            series1.Legend = "Legend1";
            series1.Name = "Series1";
            this.sysChart.Series.Add(series1);
            this.sysChart.Size = new System.Drawing.Size(684, 446);
            this.sysChart.TabIndex = 0;
            this.sysChart.Text = "sysChart";
            // 
            // btn_Cpu
            // 
            this.btn_Cpu.Location = new System.Drawing.Point(565, 271);
            this.btn_Cpu.Name = "btn_Cpu";
            this.btn_Cpu.Size = new System.Drawing.Size(101, 23);
            this.btn_Cpu.TabIndex = 1;
            this.btn_Cpu.Text = "CPU (ON)";
            this.btn_Cpu.UseVisualStyleBackColor = true;
            this.btn_Cpu.Click += new System.EventHandler(this.btn_Cpu_Click);
            // 
            // btn_Memory
            // 
            this.btn_Memory.Location = new System.Drawing.Point(565, 309);
            this.btn_Memory.Name = "btn_Memory";
            this.btn_Memory.Size = new System.Drawing.Size(103, 23);
            this.btn_Memory.TabIndex = 2;
            this.btn_Memory.Text = "MEMORY (OFF)";
            this.btn_Memory.UseVisualStyleBackColor = true;
            this.btn_Memory.Click += new System.EventHandler(this.btn_Memory_Click);
            // 
            // btn_Disk
            // 
            this.btn_Disk.Location = new System.Drawing.Point(565, 351);
            this.btn_Disk.Name = "btn_Disk";
            this.btn_Disk.Size = new System.Drawing.Size(101, 23);
            this.btn_Disk.TabIndex = 3;
            this.btn_Disk.Text = "DISK (OFF)";
            this.btn_Disk.UseVisualStyleBackColor = true;
            this.btn_Disk.Click += new System.EventHandler(this.btn_Disk_Click);
            // 
            // btn_PauseResume
            // 
            this.btn_PauseResume.Location = new System.Drawing.Point(565, 185);
            this.btn_PauseResume.Name = "btn_PauseResume";
            this.btn_PauseResume.Size = new System.Drawing.Size(101, 55);
            this.btn_PauseResume.TabIndex = 4;
            this.btn_PauseResume.Text = "Pause ||";
            this.btn_PauseResume.UseVisualStyleBackColor = true;
            this.btn_PauseResume.Click += new System.EventHandler(this.btn_PauseResume_Click);
            // 
            // Main
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(684, 446);
            this.Controls.Add(this.btn_PauseResume);
            this.Controls.Add(this.btn_Disk);
            this.Controls.Add(this.btn_Memory);
            this.Controls.Add(this.btn_Cpu);
            this.Controls.Add(this.sysChart);
            this.Name = "Main";
            this.Text = "System Metrics";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Main_FormClosing);
            this.Load += new System.EventHandler(this.Main_Load);
            ((System.ComponentModel.ISupportInitialize) (this.sysChart)).EndInit();
            this.ResumeLayout(false);
        }

        private System.Windows.Forms.Button btn_Cpu;
        private System.Windows.Forms.Button btn_Disk;
        private System.Windows.Forms.Button btn_Memory;
        private System.Windows.Forms.Button btn_PauseResume;
        private System.Windows.Forms.DataVisualization.Charting.Chart sysChart;

        #endregion
    }
}

