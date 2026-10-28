namespace AnalizadorLexico
{
    partial class CodigoIntermedio
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
            this.tabIntermedio = new System.Windows.Forms.TabControl();
            this.tabCodigoP = new System.Windows.Forms.TabPage();
            this.label1 = new System.Windows.Forms.Label();
            this.rtCodigoP = new System.Windows.Forms.RichTextBox();
            this.tabTriplos = new System.Windows.Forms.TabPage();
            this.rtTresDireccionesTriplos = new System.Windows.Forms.RichTextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.lblTablaTriplos = new System.Windows.Forms.Label();
            this.dgTriplos = new System.Windows.Forms.DataGridView();
            this.IndiceTriplos = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.opTriplos = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.arg1Triplos = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.arg2Triplos = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.tabCuadruplos = new System.Windows.Forms.TabPage();
            this.lblTresDireccionesCuadruplos = new System.Windows.Forms.Label();
            this.rtTresDireccionesCuadruplos = new System.Windows.Forms.RichTextBox();
            this.lblTablaCuadruplos = new System.Windows.Forms.Label();
            this.dgCuadruplos = new System.Windows.Forms.DataGridView();
            this.IndiceCuadruplos = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.OpCuadruplos = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.arg1Cuadruplos = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.arg2Cuadruplos = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.resultadoCuadruplos = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.tabPrefija = new System.Windows.Forms.TabPage();
            this.lblPrefijaResultado = new System.Windows.Forms.Label();
            this.rtResultadoPrefija = new System.Windows.Forms.RichTextBox();
            this.lblNotacionPrefija = new System.Windows.Forms.Label();
            this.dgPrefija = new System.Windows.Forms.DataGridView();
            this.tabPostfija = new System.Windows.Forms.TabPage();
            this.label3 = new System.Windows.Forms.Label();
            this.rtResultadoPostfija = new System.Windows.Forms.RichTextBox();
            this.lblTablaPostfija = new System.Windows.Forms.Label();
            this.dgPostfija = new System.Windows.Forms.DataGridView();
            this.SimboloPostfija = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.AccionPostfija = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.PilaPostfija = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.CalculosPostfija = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.LeidoPrefija = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.AccionPrefija = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.PilaPrefija = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.CalculosPrefija = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.tabIntermedio.SuspendLayout();
            this.tabCodigoP.SuspendLayout();
            this.tabTriplos.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgTriplos)).BeginInit();
            this.tabCuadruplos.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgCuadruplos)).BeginInit();
            this.tabPrefija.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgPrefija)).BeginInit();
            this.tabPostfija.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgPostfija)).BeginInit();
            this.SuspendLayout();
            // 
            // tabIntermedio
            // 
            this.tabIntermedio.Controls.Add(this.tabCodigoP);
            this.tabIntermedio.Controls.Add(this.tabTriplos);
            this.tabIntermedio.Controls.Add(this.tabCuadruplos);
            this.tabIntermedio.Controls.Add(this.tabPrefija);
            this.tabIntermedio.Controls.Add(this.tabPostfija);
            this.tabIntermedio.Location = new System.Drawing.Point(12, 12);
            this.tabIntermedio.Name = "tabIntermedio";
            this.tabIntermedio.SelectedIndex = 0;
            this.tabIntermedio.Size = new System.Drawing.Size(524, 413);
            this.tabIntermedio.TabIndex = 0;
            // 
            // tabCodigoP
            // 
            this.tabCodigoP.Controls.Add(this.label1);
            this.tabCodigoP.Controls.Add(this.rtCodigoP);
            this.tabCodigoP.Location = new System.Drawing.Point(4, 22);
            this.tabCodigoP.Name = "tabCodigoP";
            this.tabCodigoP.Padding = new System.Windows.Forms.Padding(3);
            this.tabCodigoP.Size = new System.Drawing.Size(516, 387);
            this.tabCodigoP.TabIndex = 0;
            this.tabCodigoP.Text = "Código P";
            this.tabCodigoP.UseVisualStyleBackColor = true;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(6, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(98, 13);
            this.label1.TabIndex = 2;
            this.label1.Text = "Código P generado";
            // 
            // rtCodigoP
            // 
            this.rtCodigoP.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(210)))), ((int)(((byte)(225)))), ((int)(((byte)(235)))));
            this.rtCodigoP.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.rtCodigoP.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(50)))), ((int)(((byte)(70)))));
            this.rtCodigoP.Location = new System.Drawing.Point(6, 29);
            this.rtCodigoP.Name = "rtCodigoP";
            this.rtCodigoP.ReadOnly = true;
            this.rtCodigoP.Size = new System.Drawing.Size(504, 352);
            this.rtCodigoP.TabIndex = 1;
            this.rtCodigoP.Text = "";
            // 
            // tabTriplos
            // 
            this.tabTriplos.Controls.Add(this.rtTresDireccionesTriplos);
            this.tabTriplos.Controls.Add(this.label2);
            this.tabTriplos.Controls.Add(this.lblTablaTriplos);
            this.tabTriplos.Controls.Add(this.dgTriplos);
            this.tabTriplos.Location = new System.Drawing.Point(4, 22);
            this.tabTriplos.Name = "tabTriplos";
            this.tabTriplos.Padding = new System.Windows.Forms.Padding(3);
            this.tabTriplos.Size = new System.Drawing.Size(516, 387);
            this.tabTriplos.TabIndex = 1;
            this.tabTriplos.Text = "Triplos";
            this.tabTriplos.UseVisualStyleBackColor = true;
            // 
            // rtTresDireccionesTriplos
            // 
            this.rtTresDireccionesTriplos.Location = new System.Drawing.Point(9, 283);
            this.rtTresDireccionesTriplos.Name = "rtTresDireccionesTriplos";
            this.rtTresDireccionesTriplos.Size = new System.Drawing.Size(491, 98);
            this.rtTresDireccionesTriplos.TabIndex = 3;
            this.rtTresDireccionesTriplos.Text = "";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(6, 267);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(180, 13);
            this.label2.TabIndex = 2;
            this.label2.Text = "Código de tres direcciones generado";
            // 
            // lblTablaTriplos
            // 
            this.lblTablaTriplos.AutoSize = true;
            this.lblTablaTriplos.Location = new System.Drawing.Point(6, 16);
            this.lblTablaTriplos.Name = "lblTablaTriplos";
            this.lblTablaTriplos.Size = new System.Drawing.Size(79, 13);
            this.lblTablaTriplos.TabIndex = 1;
            this.lblTablaTriplos.Text = "Tabla de triplos";
            // 
            // dgTriplos
            // 
            this.dgTriplos.AllowUserToDeleteRows = false;
            this.dgTriplos.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgTriplos.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.IndiceTriplos,
            this.opTriplos,
            this.arg1Triplos,
            this.arg2Triplos});
            this.dgTriplos.Location = new System.Drawing.Point(6, 32);
            this.dgTriplos.Name = "dgTriplos";
            this.dgTriplos.ReadOnly = true;
            this.dgTriplos.Size = new System.Drawing.Size(504, 232);
            this.dgTriplos.TabIndex = 0;
            // 
            // IndiceTriplos
            // 
            this.IndiceTriplos.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.IndiceTriplos.HeaderText = "Índice";
            this.IndiceTriplos.Name = "IndiceTriplos";
            this.IndiceTriplos.ReadOnly = true;
            this.IndiceTriplos.Width = 61;
            // 
            // opTriplos
            // 
            this.opTriplos.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.opTriplos.HeaderText = "op";
            this.opTriplos.Name = "opTriplos";
            this.opTriplos.ReadOnly = true;
            // 
            // arg1Triplos
            // 
            this.arg1Triplos.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.arg1Triplos.HeaderText = "arg1";
            this.arg1Triplos.Name = "arg1Triplos";
            this.arg1Triplos.ReadOnly = true;
            // 
            // arg2Triplos
            // 
            this.arg2Triplos.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.arg2Triplos.HeaderText = "arg2";
            this.arg2Triplos.Name = "arg2Triplos";
            this.arg2Triplos.ReadOnly = true;
            // 
            // tabCuadruplos
            // 
            this.tabCuadruplos.Controls.Add(this.lblTresDireccionesCuadruplos);
            this.tabCuadruplos.Controls.Add(this.rtTresDireccionesCuadruplos);
            this.tabCuadruplos.Controls.Add(this.lblTablaCuadruplos);
            this.tabCuadruplos.Controls.Add(this.dgCuadruplos);
            this.tabCuadruplos.Location = new System.Drawing.Point(4, 22);
            this.tabCuadruplos.Name = "tabCuadruplos";
            this.tabCuadruplos.Size = new System.Drawing.Size(516, 387);
            this.tabCuadruplos.TabIndex = 2;
            this.tabCuadruplos.Text = "Cuádruplos";
            this.tabCuadruplos.UseVisualStyleBackColor = true;
            // 
            // lblTresDireccionesCuadruplos
            // 
            this.lblTresDireccionesCuadruplos.AutoSize = true;
            this.lblTresDireccionesCuadruplos.Location = new System.Drawing.Point(3, 287);
            this.lblTresDireccionesCuadruplos.Name = "lblTresDireccionesCuadruplos";
            this.lblTresDireccionesCuadruplos.Size = new System.Drawing.Size(180, 13);
            this.lblTresDireccionesCuadruplos.TabIndex = 4;
            this.lblTresDireccionesCuadruplos.Text = "Código de tres direcciones generado";
            // 
            // rtTresDireccionesCuadruplos
            // 
            this.rtTresDireccionesCuadruplos.Location = new System.Drawing.Point(6, 303);
            this.rtTresDireccionesCuadruplos.Name = "rtTresDireccionesCuadruplos";
            this.rtTresDireccionesCuadruplos.Size = new System.Drawing.Size(504, 81);
            this.rtTresDireccionesCuadruplos.TabIndex = 3;
            this.rtTresDireccionesCuadruplos.Text = "";
            // 
            // lblTablaCuadruplos
            // 
            this.lblTablaCuadruplos.AutoSize = true;
            this.lblTablaCuadruplos.Location = new System.Drawing.Point(3, 9);
            this.lblTablaCuadruplos.Name = "lblTablaCuadruplos";
            this.lblTablaCuadruplos.Size = new System.Drawing.Size(104, 13);
            this.lblTablaCuadruplos.TabIndex = 2;
            this.lblTablaCuadruplos.Text = "Tabla de cuádruplos";
            // 
            // dgCuadruplos
            // 
            this.dgCuadruplos.AllowUserToDeleteRows = false;
            this.dgCuadruplos.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgCuadruplos.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.IndiceCuadruplos,
            this.OpCuadruplos,
            this.arg1Cuadruplos,
            this.arg2Cuadruplos,
            this.resultadoCuadruplos});
            this.dgCuadruplos.Location = new System.Drawing.Point(6, 25);
            this.dgCuadruplos.Name = "dgCuadruplos";
            this.dgCuadruplos.ReadOnly = true;
            this.dgCuadruplos.Size = new System.Drawing.Size(504, 259);
            this.dgCuadruplos.TabIndex = 1;
            // 
            // IndiceCuadruplos
            // 
            this.IndiceCuadruplos.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.IndiceCuadruplos.HeaderText = "Índice";
            this.IndiceCuadruplos.Name = "IndiceCuadruplos";
            this.IndiceCuadruplos.ReadOnly = true;
            this.IndiceCuadruplos.Width = 61;
            // 
            // OpCuadruplos
            // 
            this.OpCuadruplos.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.OpCuadruplos.HeaderText = "op";
            this.OpCuadruplos.Name = "OpCuadruplos";
            this.OpCuadruplos.ReadOnly = true;
            // 
            // arg1Cuadruplos
            // 
            this.arg1Cuadruplos.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.arg1Cuadruplos.HeaderText = "arg1";
            this.arg1Cuadruplos.Name = "arg1Cuadruplos";
            this.arg1Cuadruplos.ReadOnly = true;
            // 
            // arg2Cuadruplos
            // 
            this.arg2Cuadruplos.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.arg2Cuadruplos.HeaderText = "arg2";
            this.arg2Cuadruplos.Name = "arg2Cuadruplos";
            this.arg2Cuadruplos.ReadOnly = true;
            // 
            // resultadoCuadruplos
            // 
            this.resultadoCuadruplos.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.resultadoCuadruplos.HeaderText = "resultado";
            this.resultadoCuadruplos.Name = "resultadoCuadruplos";
            this.resultadoCuadruplos.ReadOnly = true;
            // 
            // tabPrefija
            // 
            this.tabPrefija.Controls.Add(this.lblPrefijaResultado);
            this.tabPrefija.Controls.Add(this.rtResultadoPrefija);
            this.tabPrefija.Controls.Add(this.lblNotacionPrefija);
            this.tabPrefija.Controls.Add(this.dgPrefija);
            this.tabPrefija.Location = new System.Drawing.Point(4, 22);
            this.tabPrefija.Name = "tabPrefija";
            this.tabPrefija.Size = new System.Drawing.Size(516, 387);
            this.tabPrefija.TabIndex = 4;
            this.tabPrefija.Text = "Notación Prefija";
            this.tabPrefija.UseVisualStyleBackColor = true;
            // 
            // lblPrefijaResultado
            // 
            this.lblPrefijaResultado.AutoSize = true;
            this.lblPrefijaResultado.Location = new System.Drawing.Point(3, 263);
            this.lblPrefijaResultado.Name = "lblPrefijaResultado";
            this.lblPrefijaResultado.Size = new System.Drawing.Size(86, 13);
            this.lblPrefijaResultado.TabIndex = 3;
            this.lblPrefijaResultado.Text = "Resultado prefija";
            // 
            // rtResultadoPrefija
            // 
            this.rtResultadoPrefija.Location = new System.Drawing.Point(6, 279);
            this.rtResultadoPrefija.Name = "rtResultadoPrefija";
            this.rtResultadoPrefija.Size = new System.Drawing.Size(507, 105);
            this.rtResultadoPrefija.TabIndex = 2;
            this.rtResultadoPrefija.Text = "";
            // 
            // lblNotacionPrefija
            // 
            this.lblNotacionPrefija.AutoSize = true;
            this.lblNotacionPrefija.Location = new System.Drawing.Point(3, 8);
            this.lblNotacionPrefija.Name = "lblNotacionPrefija";
            this.lblNotacionPrefija.Size = new System.Drawing.Size(135, 13);
            this.lblNotacionPrefija.TabIndex = 1;
            this.lblNotacionPrefija.Text = "Tabla de evaluación prefija";
            // 
            // dgPrefija
            // 
            this.dgPrefija.AllowUserToDeleteRows = false;
            this.dgPrefija.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgPrefija.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.LeidoPrefija,
            this.AccionPrefija,
            this.PilaPrefija,
            this.CalculosPrefija});
            this.dgPrefija.Location = new System.Drawing.Point(3, 24);
            this.dgPrefija.Name = "dgPrefija";
            this.dgPrefija.ReadOnly = true;
            this.dgPrefija.Size = new System.Drawing.Size(510, 236);
            this.dgPrefija.TabIndex = 0;
            // 
            // tabPostfija
            // 
            this.tabPostfija.Controls.Add(this.label3);
            this.tabPostfija.Controls.Add(this.rtResultadoPostfija);
            this.tabPostfija.Controls.Add(this.lblTablaPostfija);
            this.tabPostfija.Controls.Add(this.dgPostfija);
            this.tabPostfija.Location = new System.Drawing.Point(4, 22);
            this.tabPostfija.Name = "tabPostfija";
            this.tabPostfija.Size = new System.Drawing.Size(516, 387);
            this.tabPostfija.TabIndex = 3;
            this.tabPostfija.Text = "Notación Postfija";
            this.tabPostfija.UseVisualStyleBackColor = true;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(0, 268);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(91, 13);
            this.label3.TabIndex = 7;
            this.label3.Text = "Resultado postfija";
            // 
            // rtResultadoPostfija
            // 
            this.rtResultadoPostfija.Location = new System.Drawing.Point(6, 284);
            this.rtResultadoPostfija.Name = "rtResultadoPostfija";
            this.rtResultadoPostfija.Size = new System.Drawing.Size(507, 97);
            this.rtResultadoPostfija.TabIndex = 6;
            this.rtResultadoPostfija.Text = "";
            // 
            // lblTablaPostfija
            // 
            this.lblTablaPostfija.AutoSize = true;
            this.lblTablaPostfija.Location = new System.Drawing.Point(3, 5);
            this.lblTablaPostfija.Name = "lblTablaPostfija";
            this.lblTablaPostfija.Size = new System.Drawing.Size(140, 13);
            this.lblTablaPostfija.TabIndex = 5;
            this.lblTablaPostfija.Text = "Tabla de evaluación postfija";
            // 
            // dgPostfija
            // 
            this.dgPostfija.AllowUserToDeleteRows = false;
            this.dgPostfija.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgPostfija.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.SimboloPostfija,
            this.AccionPostfija,
            this.PilaPostfija,
            this.CalculosPostfija});
            this.dgPostfija.Location = new System.Drawing.Point(3, 21);
            this.dgPostfija.Name = "dgPostfija";
            this.dgPostfija.ReadOnly = true;
            this.dgPostfija.Size = new System.Drawing.Size(510, 244);
            this.dgPostfija.TabIndex = 4;
            // 
            // SimboloPostfija
            // 
            this.SimboloPostfija.HeaderText = "Símbolo";
            this.SimboloPostfija.Name = "SimboloPostfija";
            this.SimboloPostfija.ReadOnly = true;
            this.SimboloPostfija.Width = 71;
            // 
            // AccionPostfija
            // 
            this.AccionPostfija.HeaderText = "Acción";
            this.AccionPostfija.Name = "AccionPostfija";
            this.AccionPostfija.ReadOnly = true;
            this.AccionPostfija.Width = 65;
            // 
            // PilaPostfija
            // 
            this.PilaPostfija.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.PilaPostfija.HeaderText = "Pila";
            this.PilaPostfija.Name = "PilaPostfija";
            this.PilaPostfija.ReadOnly = true;
            // 
            // CalculosPostfija
            // 
            this.CalculosPostfija.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.CalculosPostfija.HeaderText = "Cálculos";
            this.CalculosPostfija.Name = "CalculosPostfija";
            this.CalculosPostfija.ReadOnly = true;
            // 
            // LeidoPrefija
            // 
            this.LeidoPrefija.HeaderText = "Símbolo";
            this.LeidoPrefija.Name = "LeidoPrefija";
            this.LeidoPrefija.ReadOnly = true;
            this.LeidoPrefija.Width = 71;
            // 
            // AccionPrefija
            // 
            this.AccionPrefija.HeaderText = "Acción";
            this.AccionPrefija.Name = "AccionPrefija";
            this.AccionPrefija.ReadOnly = true;
            this.AccionPrefija.Width = 65;
            // 
            // PilaPrefija
            // 
            this.PilaPrefija.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.PilaPrefija.HeaderText = "Pila";
            this.PilaPrefija.Name = "PilaPrefija";
            this.PilaPrefija.ReadOnly = true;
            // 
            // CalculosPrefija
            // 
            this.CalculosPrefija.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.CalculosPrefija.HeaderText = "Cálculos";
            this.CalculosPrefija.Name = "CalculosPrefija";
            this.CalculosPrefija.ReadOnly = true;
            // 
            // CodigoIntermedio
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(548, 430);
            this.Controls.Add(this.tabIntermedio);
            this.Name = "CodigoIntermedio";
            this.Text = "CodigoIntermedio";
            this.Load += new System.EventHandler(this.CodigoIntermedio_Load);
            this.tabIntermedio.ResumeLayout(false);
            this.tabCodigoP.ResumeLayout(false);
            this.tabCodigoP.PerformLayout();
            this.tabTriplos.ResumeLayout(false);
            this.tabTriplos.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgTriplos)).EndInit();
            this.tabCuadruplos.ResumeLayout(false);
            this.tabCuadruplos.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgCuadruplos)).EndInit();
            this.tabPrefija.ResumeLayout(false);
            this.tabPrefija.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgPrefija)).EndInit();
            this.tabPostfija.ResumeLayout(false);
            this.tabPostfija.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgPostfija)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TabControl tabIntermedio;
        private System.Windows.Forms.TabPage tabCodigoP;
        private System.Windows.Forms.TabPage tabTriplos;
        private System.Windows.Forms.TabPage tabCuadruplos;
        private System.Windows.Forms.TabPage tabPostfija;
        private System.Windows.Forms.TabPage tabPrefija;
        private System.Windows.Forms.RichTextBox rtCodigoP;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.DataGridView dgTriplos;
        private System.Windows.Forms.RichTextBox rtTresDireccionesTriplos;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label lblTablaTriplos;
        private System.Windows.Forms.DataGridViewTextBoxColumn IndiceTriplos;
        private System.Windows.Forms.DataGridViewTextBoxColumn opTriplos;
        private System.Windows.Forms.DataGridViewTextBoxColumn arg1Triplos;
        private System.Windows.Forms.DataGridViewTextBoxColumn arg2Triplos;
        private System.Windows.Forms.DataGridView dgCuadruplos;
        private System.Windows.Forms.Label lblTablaCuadruplos;
        private System.Windows.Forms.RichTextBox rtTresDireccionesCuadruplos;
        private System.Windows.Forms.Label lblTresDireccionesCuadruplos;
        private System.Windows.Forms.DataGridView dgPrefija;
        private System.Windows.Forms.Label lblPrefijaResultado;
        private System.Windows.Forms.RichTextBox rtResultadoPrefija;
        private System.Windows.Forms.Label lblNotacionPrefija;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.RichTextBox rtResultadoPostfija;
        private System.Windows.Forms.Label lblTablaPostfija;
        private System.Windows.Forms.DataGridView dgPostfija;
        private System.Windows.Forms.DataGridViewTextBoxColumn IndiceCuadruplos;
        private System.Windows.Forms.DataGridViewTextBoxColumn OpCuadruplos;
        private System.Windows.Forms.DataGridViewTextBoxColumn arg1Cuadruplos;
        private System.Windows.Forms.DataGridViewTextBoxColumn arg2Cuadruplos;
        private System.Windows.Forms.DataGridViewTextBoxColumn resultadoCuadruplos;
        private System.Windows.Forms.DataGridViewTextBoxColumn SimboloPostfija;
        private System.Windows.Forms.DataGridViewTextBoxColumn AccionPostfija;
        private System.Windows.Forms.DataGridViewTextBoxColumn PilaPostfija;
        private System.Windows.Forms.DataGridViewTextBoxColumn CalculosPostfija;
        private System.Windows.Forms.DataGridViewTextBoxColumn LeidoPrefija;
        private System.Windows.Forms.DataGridViewTextBoxColumn AccionPrefija;
        private System.Windows.Forms.DataGridViewTextBoxColumn PilaPrefija;
        private System.Windows.Forms.DataGridViewTextBoxColumn CalculosPrefija;
    }
}