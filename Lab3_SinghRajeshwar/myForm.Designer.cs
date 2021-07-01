
namespace Lab3_SinghRajeshwar
{
    partial class myForm
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
            this.timerClick = new System.Windows.Forms.Timer(this.components);
            this.SuspendLayout();
            // 
            // timerClick
            // 
            this.timerClick.Interval = 25;
            // 
            // myForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(904, 468);
            this.KeyPreview = true;
            this.Name = "myForm";
            this.Text = "Pointy Pixel Size : 50";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Timer timerClick;
    }
}

