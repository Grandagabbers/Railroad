namespace RailRoadSimulator
{
	partial class MainForm
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
			this.railRoadMap = new System.Windows.Forms.PictureBox();
			((System.ComponentModel.ISupportInitialize)(this.railRoadMap)).BeginInit();
			this.SuspendLayout();
			// 
			// railRoadMap
			// 
			this.railRoadMap.Location = new System.Drawing.Point(12, 12);
			this.railRoadMap.Name = "railRoadMap";
			this.railRoadMap.Size = new System.Drawing.Size(562, 547);
			this.railRoadMap.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
			this.railRoadMap.TabIndex = 0;
			this.railRoadMap.TabStop = false;
			this.railRoadMap.Click += new System.EventHandler(this.railRoadMap_Click);
			// 
			// MainForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(586, 565);
			this.Controls.Add(this.railRoadMap);
			this.Name = "MainForm";
			this.Text = "Form1";
			this.Load += new System.EventHandler(this.Form1_Load);
			((System.ComponentModel.ISupportInitialize)(this.railRoadMap)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.PictureBox railRoadMap;
	}
}

