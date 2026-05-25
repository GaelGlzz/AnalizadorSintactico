namespace AnalizadorLexico
{
    partial class FormInfo
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormInfo));
            this.lblDedicatoria = new System.Windows.Forms.Label();
            this.lblIntegrantes = new System.Windows.Forms.Label();
            this.picSnoopy = new System.Windows.Forms.PictureBox();
            this.lblDisclaimer = new System.Windows.Forms.Label();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.picSnoopy)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // lblDedicatoria
            // 
            this.lblDedicatoria.AutoSize = true;
            this.lblDedicatoria.Font = new System.Drawing.Font("Arial Narrow", 12F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Italic))), System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblDedicatoria.Location = new System.Drawing.Point(9, 7);
            this.lblDedicatoria.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lblDedicatoria.Name = "lblDedicatoria";
            this.lblDedicatoria.Size = new System.Drawing.Size(324, 40);
            this.lblDedicatoria.TabIndex = 0;
            this.lblDedicatoria.Text = "Este programa fue desarrollado por las siguientes \r\npersonas con mucho esfuerzo y" +
    " dedicación.";
            // 
            // lblIntegrantes
            // 
            this.lblIntegrantes.AutoSize = true;
            this.lblIntegrantes.Font = new System.Drawing.Font("Microsoft Sans Serif", 10.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblIntegrantes.Location = new System.Drawing.Point(9, 54);
            this.lblIntegrantes.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lblIntegrantes.Name = "lblIntegrantes";
            this.lblIntegrantes.Size = new System.Drawing.Size(327, 51);
            this.lblIntegrantes.TabIndex = 1;
            this.lblIntegrantes.Text = "#23100169 Hernández Navarro Karely Guadalupe\r\n#23100211 Segovia Espinoza Valeria\r" +
    "\n#23100676 González Maldonado Julio Gael ";
            // 
            // picSnoopy
            // 
            this.picSnoopy.Image = ((System.Drawing.Image)(resources.GetObject("picSnoopy.Image")));
            this.picSnoopy.Location = new System.Drawing.Point(338, 10);
            this.picSnoopy.Margin = new System.Windows.Forms.Padding(2);
            this.picSnoopy.Name = "picSnoopy";
            this.picSnoopy.Size = new System.Drawing.Size(112, 122);
            this.picSnoopy.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.picSnoopy.TabIndex = 2;
            this.picSnoopy.TabStop = false;
            // 
            // lblDisclaimer
            // 
            this.lblDisclaimer.AutoSize = true;
            this.lblDisclaimer.Font = new System.Drawing.Font("Microsoft Sans Serif", 7.8F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblDisclaimer.Location = new System.Drawing.Point(247, 170);
            this.lblDisclaimer.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lblDisclaimer.Name = "lblDisclaimer";
            this.lblDisclaimer.Size = new System.Drawing.Size(208, 13);
            this.lblDisclaimer.TabIndex = 3;
            this.lblDisclaimer.Text = "Copyright KaViGex© Derechos reservados";
            // 
            // pictureBox1
            // 
            this.pictureBox1.Image = ((System.Drawing.Image)(resources.GetObject("pictureBox1.Image")));
            this.pictureBox1.Location = new System.Drawing.Point(9, 115);
            this.pictureBox1.Margin = new System.Windows.Forms.Padding(2);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(75, 66);
            this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pictureBox1.TabIndex = 4;
            this.pictureBox1.TabStop = false;
            // 
            // FormInfo
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(460, 190);
            this.Controls.Add(this.pictureBox1);
            this.Controls.Add(this.lblDisclaimer);
            this.Controls.Add(this.picSnoopy);
            this.Controls.Add(this.lblIntegrantes);
            this.Controls.Add(this.lblDedicatoria);
            this.Margin = new System.Windows.Forms.Padding(2);
            this.Name = "FormInfo";
            this.Text = "Información del programa";
            ((System.ComponentModel.ISupportInitialize)(this.picSnoopy)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lblDedicatoria;
        private System.Windows.Forms.Label lblIntegrantes;
        private System.Windows.Forms.PictureBox picSnoopy;
        private System.Windows.Forms.Label lblDisclaimer;
        private System.Windows.Forms.PictureBox pictureBox1;
    }
}