using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace AnalizadorLexico
{
    public partial class Form1 : Form
    {
        private Lexer lexer;
        private LexerDB lexerDB;
        private List<Simbolo> tablaSimbolos = new List<Simbolo>();
        private List<Token> tokensLexico = new List<Token>();

        public Form1()
        {
            InitializeComponent();
            lexer = new Lexer();
            lexerDB = new LexerDB();

            // Asociar eventos a los botones (si no están asignados en designer)
            /*btnCargar.Click += btnCargar_Click;
            btnEditar.Click += btnEditar_Click;
            btnGuardar.Click += btnGuardar_Click;
            btnGuardarA.Click += btnGuardarA_Click;*/

            // Crear botón léxico en runtime (label4 actúa como indicador, creamos click)
            label4.Click += label4_Click;

            // Crear botón sintáctico en runtime (label5 actúa como indicador, creamos click)
            label5.Click += label5_Click;
            label5.MouseEnter += label5_MouseEnter;
            label5.MouseLeave += label5_MouseLeave;

            // Manejar Tab en rtbFuente para insertar 5 espacios
            rtbFuente.KeyDown += RtbFuente_KeyDown;

            // Inicialmente rtbFuente editable
            rtbFuente.ReadOnly = false;

            // Verificar que la matriz esté cargada
            if (!lexerDB.IsMatrizLoaded())
            {
                string msg = "No se pudo cargar la matriz de transición desde la base de datos.";
                var initErr = lexerDB.GetInitError();
                if (!string.IsNullOrEmpty(initErr))
                    msg += "\nDetalle: " + initErr;

                MessageBox.Show(msg, "Error BD", MessageBoxButtons.OK, MessageBoxIcon.Error);

                // Desactivar la acción de Léxico
                label4.Enabled = false;
            }
        }

        private string ObtenerDefinicion(string categoria)
        {
            switch (categoria)
            {
                case "DESDE": return "Palabra reservada para inicios de bucles";
                case "ENTONCES": return "Palabra reservada para condicionales";
                case "ESCRIBIR": return "Función para escribir/mostrar";
                case "FIN": return "Palabra reservada para fin de bloque";
                case "FUNCION": return "Declaración de función";
                case "HACER": return "Palabra reservada para bucles";
                case "HASTA": return "Límite superior en bucles";
                case "INICIO": return "Inicio de programa o bloque";
                case "IR": return "Salto incondicional";
                case "LEER": return "Lectura de entrada";
                case "LIMP": return "Limpiar pantalla";
                case "MIENTRAS": return "Bucle condicional";
                case "MOSTRAR": return "Salida de datos";
                case "NO": return "Operador lógico de negación";
                case "NUEVO": return "Creación de instancia";
                case "O": return "Operador lógico OR";
                case "PARA": return "Bucle iterativo";
                case "REPETIR": return "Bucle con repetición";
                case "RETORNAR": return "Retorno de función";
                case "ROMPER": return "Salida de bucle";
                case "SI": return "Condicional simple";
                case "SINO": return "Rama alternativa de condicional";
                case "VAR": return "Declaración de variable";
                case "Y": return "Operador lógico AND";
                case "CN ENTEROS": return "Constante numérica entera";
                case "CN REALES": return "Constante numérica real";
                case "CN CON EXPONENTE": return "Constante numérica con exponente";
                case "COMEN": return "Comentario";
                case "CAD": return "Cadena de caracteres";
                case "OA1": return "Operador aritmético: + (suma)";
                case "OA2": return "Operador aritmético: - (resta)";
                case "OA3": return "Operador aritmético: * (multiplicación)";
                case "OA4": return "Operador aritmético: / (división)";
                case "OR1": return "Operador relacional: > (mayor que)";
                case "OR2": return "Operador relacional: >= (mayor o igual que)";
                case "OR3": return "Operador relacional: < (menor que)";
                case "OR4": return "Operador relacional: <= (menor o igual que)";
                case "OR5": return "Operador relacional: <> (distinto de)";
                case "OR6": return "Operador relacional: == (igual que)";
                case "ASI": return "Operador de asignación: =";
                case "IDENT": return "Identificador (formato: pXxxx)";
                case "CE1": return "Carácter especial: ( (paréntesis abierto)";
                case "CE2": return "Carácter especial: ) (paréntesis cerrado)";
                case "CE3": return "Carácter especial: { (llave abierta)";
                case "CE4": return "Carácter especial: } (llave cerrada)";
                case "CE5": return "Carácter especial: [ (corchete abierto)";
                case "CE6": return "Carácter especial: ] (corchete cerrado)";
                case "CE7": return "Carácter especial: , (coma)";
                case "CE8": return "Carácter especial: ; (punto y coma)";
                case "CE9": return "Carácter especial: . (punto)";
                case "CE10": return "Carácter especial: : (dos puntos)";
                case "CE11": return "Carácter especial: ? (interrogación)";
                case "CE12": return "Carácter especial: ! (exclamación)";
                case "CE13": return "Carácter especial: & (ampersand)";
                case "CE14": return "Carácter especial: | (barra vertical)";
                case "CE15": return "Carácter especial: ~ (tilde)";
                case "CE16": return "Carácter especial: @ (arroba)";
                case "CE17": return "Carácter especial: # (numeral)";
                case "CE18": return "Carácter especial: $ (dólar)";
                case "CE19": return "Carácter especial: % (porcentaje)";
                case "CE20": return "Carácter especial: ^ (circunflejo)";
                case "CE21": return "Carácter especial: \\ (barra invertida)";
                case "CE22": return "Carácter especial: ` (acento grave)";
                case "CE23": return "Carácter especial: ! (Signo de exclamación)";
                default: return categoria;
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            string msg;
            bool ok = lexerDB.TestConnection(out msg);
            if (ok)
                MessageBox.Show("Conexión exitosa a la base de datos.", "Prueba BD", MessageBoxButtons.OK, MessageBoxIcon.Information);
            else
                MessageBox.Show("Error de conexión a la base de datos:\n" + msg, "Prueba BD", MessageBoxButtons.OK, MessageBoxIcon.Error);
        
        }

        // Cargar Programa
        private void btnCargar_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog ofd = new OpenFileDialog())
            {
                ofd.Filter = "Text Files|*.txt";
                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    string content = File.ReadAllText(ofd.FileName, Encoding.UTF8);
                    rtbFuente.Text = AddLineNumbers(content);
                    rtbFuente.ReadOnly = true; // bloquear después de cargar

                    // limpiar anteriores
                    rtbTokens.Clear();
                    dgvErrores.Rows.Clear();
                    dgvSimbolos.Rows.Clear();
                    tablaSimbolos.Clear();
                }
            }
        }

        // Editar Programa
        private void btnEditar_Click(object sender, EventArgs e)
        {
            rtbFuente.ReadOnly = false;
            rtbTokens.Clear();
            dgvErrores.Rows.Clear();
            dgvSimbolos.Rows.Clear();
            tablaSimbolos.Clear();
            dgvErroresSintacticos.Rows.Clear();
            rtSintaxis.Clear();
        }

        // Guardar Programa Fuente
        private void btnGuardar_Click(object sender, EventArgs e)
        {
            using (SaveFileDialog sfd = new SaveFileDialog())
            {
                sfd.Filter = "Text Files|*.txt";
                if (sfd.ShowDialog() == DialogResult.OK)
                {
                    // quitar numeración de líneas al guardar
                    string text = RemoveLineNumbers(rtbFuente.Text);
                    File.WriteAllText(sfd.FileName, text, Encoding.UTF8);
                }
            }
        }

        // Guardar Archivo Tokens
        private void btnGuardarA_Click(object sender, EventArgs e)
        {
            using (SaveFileDialog sfd = new SaveFileDialog())
            {
                sfd.Filter = "Text Files|*.txt";
                sfd.FileName = "tokens.txt";
                if (sfd.ShowDialog() == DialogResult.OK)
                {
                    File.WriteAllText(sfd.FileName, rtbTokens.Text, Encoding.UTF8);
                }
            }
        }

        // Click en label4 (ejecuta Léxico)
        private void label4_Click(object sender, EventArgs e)
        {
            if (!lexerDB.IsMatrizLoaded())
            {
                MessageBox.Show("La matriz de transición no está disponible. Verifica la conexión a la base de datos.", "Error BD", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            EjecutarLexico();

            // Verificar si hay errores léxicos
            if (dgvErrores.Rows.Count > 0)
            {
                // Hay errores léxicos: deshabilitar btnGuardarA, limpiar errores sintácticos y rtSintaxis
                btnGuardarA.Enabled = false;
                dgvErroresSintacticos.Rows.Clear();
                rtSintaxis.Clear();
                MessageBox.Show("Se encontraron errores léxicos. No se puede ejecutar el análisis sintáctico.", "Errores Léxicos", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            else
            {
                // No hay errores léxicos: ejecutar análisis sintáctico
                btnGuardarA.Enabled = true;
                bool hayErroresSintacticos = EjecutarSintactico();

                if (hayErroresSintacticos)
                {
                    MessageBox.Show("Se encontraron errores en la sintaxis.", "Errores Sintácticos", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
                else
                {
                    MessageBox.Show("No se encontraron errores en la sintaxis.", "Análisis Sintáctico", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
        }

        private void EjecutarLexico()
        {
            rtbTokens.Clear();
            dgvErrores.Rows.Clear();
            dgvSimbolos.Rows.Clear();
            tablaSimbolos.Clear();
            tokensLexico.Clear();

            // Limpiar y preparar dgvErrores: solo dos columnas (Linea, Error)
            //dgvErrores.Columns.Clear();
            //dgvErrores.Columns.Add("Linea", "Línea");
            //dgvErrores.Columns.Add("Error", "Error");
            //dgvErrores.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;

            // Obtener líneas sin números
            string[] lines = RemoveLineNumbers(rtbFuente.Text).Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);

            for (int i = 0; i < lines.Length; i++)
            {
                int numeroLinea = i + 1;
                var lexemas = lexerDB.LeerCadena(lines[i]);

                // escribir cabecera de línea en rtbTokens
                rtbTokens.SelectionColor = rtbTokens.ForeColor;
                rtbTokens.SelectionColor = Color.Blue;
                rtbTokens.AppendText($"{numeroLinea}  ");
                rtbTokens.SelectionColor = Color.Black;
                foreach (var lex in lexemas)
                {
                    Token token;
                    try
                    {
                        token = lexerDB.RecorrerMatriz(lex);
                    }
                    catch (Exception ex)
                    {
                        token = new Token { Lexema = lex, Estado = -1, Categoria = "ERROR: Excepción durante análisis: " + ex.Message, EsError = true };
                    }

                    // Almacenar el token con su número de línea
                    token.Lexema = lex; // Asegurar que el lexema se guarde correctamente
                    // Usamos una propiedad personalizada para guardar la línea
                    // Como Token no tiene propiedad de línea, la guardaremos en una lista paralela

                    if(token.EsError)
                    {
                        // Agregar a errores: solo Linea y Error (sin lexema en columna separada)
                        dgvErrores.Rows.Add(numeroLinea, token.Categoria);
                        int lastRow = dgvErrores.Rows.Count - 1;
                        dgvErrores.Rows[lastRow].DefaultCellStyle.ForeColor = Color.Red;

                        // En rtbTokens mostrar "Error" en rojo seguido del lexema
                        rtbTokens.SelectionStart = rtbTokens.TextLength;
                        rtbTokens.SelectionColor = Color.Red;
                        rtbTokens.AppendText($"Error ");
                        rtbTokens.SelectionColor = Color.Red;
                        rtbTokens.AppendText($"{token.Lexema} ");
                    }
                    else
                    {
                        // Guardar token válido para el análisis sintáctico
                        tokensLexico.Add(token);

                        // 1. Aseguramos que el símbolo esté en la tabla y obtenemos su referencia
                        lexerDB.ActualizarTablaSimbolos(token, tablaSimbolos);

                        // 2. Buscamos el símbolo en la lista para obtener el NumID asignado
                        var simboloActual = tablaSimbolos.FirstOrDefault(s => s.Nombre == token.Lexema);
                        string etiquetaMostrar = token.Categoria;

                        if (token.Categoria == "IDENT" && simboloActual != null)
                        {
                            // Concatenamos el número del identificador
                            etiquetaMostrar = $"IDENT{simboloActual.NumID}";
                            rtbTokens.SelectionColor = Color.Orange; // Identificadores en azul
                        }
                        else
                        {
                            rtbTokens.SelectionColor = Color.Black;

                        }

                        // 3. Imprimimos en el RichTextBox
                        rtbTokens.SelectionStart = rtbTokens.TextLength;

                        rtbTokens.AppendText($" {etiquetaMostrar} ");
                        rtbTokens.SelectionColor = rtbTokens.ForeColor;
                    }
                }

                rtbTokens.AppendText("\n");
            }

            // Llenar dgvSimbolos
            foreach (var s in tablaSimbolos)
            {
                int idx = dgvSimbolos.Rows.Add();
                dgvSimbolos.Rows[idx].Cells[0].Value = s.NumID;
                dgvSimbolos.Rows[idx].Cells[1].Value = s.Nombre;
                // marcar en rojo
                dgvSimbolos.Rows[idx].DefaultCellStyle.ForeColor = Color.Red;
            }

            // bloquear rtbFuente
            rtbFuente.ReadOnly = true;
        }

        private string AddLineNumbers(string text)
        {
            var lines = text.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.None);
            var sb = new StringBuilder();
            for (int i = 0; i < lines.Length; i++)
            {
                // agregar numeración seguida de un tabulador
                if (i < lines.Length - 1)
                    sb.AppendLine(/*(i + 1) + "\t" + */ lines[i]);
                else
                    sb.Append(/*(i + 1) + "\t" + */ lines[i]); // última línea sin salto adicional
            }
            return sb.ToString();
        }

        private string RemoveLineNumbers(string text)
        {
            // eliminar numeración del inicio de cada línea: "numero<tab>"
            var lines = text.Split(new[] { '\r', '\n' }, StringSplitOptions.None);
            var sb = new StringBuilder();
            var regex = new Regex("^\\s*\\d+\\t?");
            for (int i = 0; i < lines.Length; i++)
            {
                string clean = regex.Replace(lines[i], "");
                sb.AppendLine(clean);
            }
            return sb.ToString();
        }

        private void RtbFuente_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Tab)
            {
                e.SuppressKeyPress = true; // Evita que Tab mueva el foco
                rtbFuente.SelectedText = "     "; // Insertar 5 espacios
            }
        }

        private void lblTitulo_Click(object sender, EventArgs e)
        {

        }

        private void lblTitulo_Click_1(object sender, EventArgs e)
        {

        }

        private void label13_Click(object sender, EventArgs e)
        {

        }

        private void label4_Click_1(object sender, EventArgs e)
        {

        }

        private void btnInfo_Click(object sender, EventArgs e)
        {
            FormInfo infoForm = new FormInfo();
            infoForm.Show();
        }


        public void UpdateLineNumbers()
        {
            // Obtener la posición actual
            Point pt = new Point(0, 0);
            int firstIndex = rtbFuente.GetCharIndexFromPosition(pt);
            int firstLine = rtbFuente.GetLineFromCharIndex(firstIndex);

            // Definir el punto final visible
            pt.X = rtbFuente.ClientRectangle.Width;
            pt.Y = rtbFuente.ClientRectangle.Height;
            int lastIndex = rtbFuente.GetCharIndexFromPosition(pt);
            int lastLine = rtbFuente.GetLineFromCharIndex(lastIndex);

            txtNumerosLinea.SelectionAlignment = HorizontalAlignment.Center;
            txtNumerosLinea.Text = "";

            // Llenar el margen con los números correspondientes
            for (int i = firstLine; i <= lastLine + 1; i++)
            {
                txtNumerosLinea.Text += (i + 1) + "\n";
            }
        }

        // Eventos para actualizar al escribir o cambiar tamaño
        private void rtbFuente_VScroll(object sender, EventArgs e) => UpdateLineNumbers();

        private void rtbFuente_TextChanged_1(object sender, EventArgs e)
        {
            UpdateLineNumbers();
        }

        private void rtbFuente_VScroll_1(object sender, EventArgs e)
        {
            int line = rtbFuente.GetLineFromCharIndex(rtbFuente.GetCharIndexFromPosition(new Point(0, 0)));
            UpdateLineNumbers();
        }

        private void btnGuardar_Click_1(object sender, EventArgs e)
        {
            using (SaveFileDialog sfd = new SaveFileDialog())
            {
                sfd.Filter = "Text Files|*.txt";
                if (sfd.ShowDialog() == DialogResult.OK)
                {
                    File.WriteAllText(sfd.FileName, rtbFuente.Text, Encoding.UTF8);
                }
            }

        }

        private void label4_MouseEnter(object sender, EventArgs e)
        {
            //label4.ForeColor = Color.FromArgb(187, 222, 251);
            label4.Cursor = Cursors.Hand;

            label4.BackColor = Color.FromArgb(187, 222, 251);
        }

        private void label4_MouseLeave(object sender, EventArgs e)
        {
            //label4.ForeColor = Color.White;
            label4.BackColor = Color.FromArgb(25, 70, 130);
        }

        // Click en label5 (ejecuta Sintáctico)
        private void label5_Click(object sender, EventArgs e)
        {
            if (!lexerDB.IsMatrizLoaded())
            {
                MessageBox.Show("La matriz de transición no está disponible. Verifica la conexión a la base de datos.", "Error BD", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            EjecutarSintactico();
        }

        private bool EjecutarSintactico()
        {
            // Primero ejecutar el léxico si no se ha hecho
            if (string.IsNullOrEmpty(rtbTokens.Text) || rtbTokens.Text.Trim() == "")
            {
                EjecutarLexico();
            }

            // Convertir tokens del lexer a TokenSintactico usando los tokens almacenados
            List<ParserSintactico.TokenSintactico> tokensSintacticos = new List<ParserSintactico.TokenSintactico>();

            // Obtener líneas sin números para calcular números de línea
            string[] lines = RemoveLineNumbers(rtbFuente.Text).Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);

            int tokenIndex = 0;
            for (int lineaActual = 0; lineaActual < lines.Length; lineaActual++)
            {
                var lexemas = lexerDB.LeerCadena(lines[lineaActual]);
                int numeroLinea = lineaActual + 1;

                foreach (var lex in lexemas)
                {
                    if (tokenIndex < tokensLexico.Count)
                    {
                        Token token = tokensLexico[tokenIndex];
                        if (!token.EsError)
                        {
                            // Determinar el tipo de token sintáctico
                            string tipo = DeterminarTipoTokenDesdeCategoria(token.Categoria);
                            tokensSintacticos.Add(new ParserSintactico.TokenSintactico
                            {
                                Tipo = tipo,
                                Valor = token.Lexema,
                                Linea = numeroLinea
                            });
                        }
                        tokenIndex++;
                    }
                }
            }

            // Crear parser y ejecutar
            ParserSintactico parser = new ParserSintactico(tokensSintacticos, dgvErroresSintacticos, rtSintaxis);
            parser.Parse();

            // Devolver true si hay errores sintácticos, false si no
            return dgvErroresSintacticos.Rows.Count > 0;
        }

        private string DeterminarTipoTokenDesdeCategoria(string categoria)
        {
            // Mapeo de categorías del lexer a tipos sintácticos
            switch (categoria)
            {
                case "IDENT": return "IDENT";
                case "INICIO": return "PR1";
                case "FIN": return "PR2";
                case "LEER": return "PR3";
                case "MOSTRAR": return "PR4";
                case "SI": return "PR5";
                case "ENTONCES": return "PR6";
                case "SINO": return "PR7";
                case "DESDE": return "PR8";
                case "HASTA": return "PR9";
                case "REPETIR": return "PR10";
                case "ROMPER": return "PR11";
                case "PARA": return "PR12";
                case "HACER": return "PR13";
                case "MIENTRAS": return "PR14";
                case "LIMP": return "PR15";
                case "IR": return "PR16";
                case "ESCRIBIR": return "PR17";
                case "FUNCION": return "PR18";
                case "RETORNAR": return "PR19";
                case "VAR": return "PR20";
                case "NUEVO": return "PR21";
                case "ASI": return "ASI";
                case "OA1": return "OA1";
                case "OA2": return "OA2";
                case "OA3": return "OA3";
                case "OA4": return "OA4";
                case "OR1": return "OR1";
                case "OR2": return "OR2";
                case "OR3": return "OR3";
                case "OR4": return "OR4";
                case "OR5": return "OR5";
                case "OR6": return "OR6";
                case "Y": return "OPL1"; // operador lógico AND
                case "O": return "OPL2"; // operador lógico OR
                case "NO": return "OPL3"; // operador lógico NOT
                case "DEL": return "CE13"; // ;
                case "CE16": return "CE16"; // ,
                case "CE7": return "CE7"; // (
                case "CE8": return "CE8"; // )
                case "CE9": return "CE9"; // {
                case "CE10": return "CE10"; // }
                case "CE11": return "CE11"; // :
                case "CE12": return "CE12"; // "
                case "CE14": return "CE14"; // ?
                case "CE15": return "CE15"; // \
                case "CE17": return "CE17"; // .
                case "CE18": return "CE18"; // ~
                case "CE19": return "CE19"; // [
                case "CE20": return "CE20"; // ]
                case "CE21": return "CE21"; // _
                case "CE22": return "CE22"; // ¿
                case "CE23": return "CE23"; // !
                case "CN ENTEROS": return "CNU";
                case "CN REALES": return "CNU";
                case "CN CON EXPONENTE": return "CNU";
                case "CAD": return "CAD";
                case "COM": return "COMEN";
                default: return categoria;
            }
        }

        private string DeterminarTipoToken(string tokenStr)
        {
            // Mapeo de tokens léxicos a tipos sintácticos
            if (tokenStr.StartsWith("IDENT")) return "IDENT";
            if (tokenStr == "INICIO") return "PR1";
            if (tokenStr == "FIN") return "PR2";
            if (tokenStr == "LEER") return "PR3";
            if (tokenStr == "MOSTRAR") return "PR4";
            if (tokenStr == "SI") return "PR5";
            if (tokenStr == "ENTONCES") return "PR6";
            if (tokenStr == "SINO") return "PR7";
            if (tokenStr == "DESDE") return "PR8";
            if (tokenStr == "HASTA") return "PR9";
            if (tokenStr == "REPETIR") return "PR10";
            if (tokenStr == "ROMPER") return "PR11";
            if (tokenStr == "PARA") return "PR12";
            if (tokenStr == "HACER") return "PR13";
            if (tokenStr == "MIENTRAS") return "PR14";
            if (tokenStr == "LIMP") return "PR15";
            if (tokenStr == "IR") return "PR16";
            if (tokenStr == "ESCRIBIR") return "PR17";
            if (tokenStr == "FUNCION") return "PR18";
            if (tokenStr == "RETORNAR") return "PR19";
            if (tokenStr == "VAR") return "PR20";
            if (tokenStr == "NUEVO") return "PR21";
            if (tokenStr == "ASI") return "ASI";
            if (tokenStr == "OA1") return "OA1";
            if (tokenStr == "OA2") return "OA2";
            if (tokenStr == "OA3") return "OA3";
            if (tokenStr == "OA4") return "OA4";
            if (tokenStr == "OR1") return "OR1";
            if (tokenStr == "OR2") return "OR2";
            if (tokenStr == "OR3") return "OR3";
            if (tokenStr == "OR4") return "OR4";
            if (tokenStr == "OR5") return "OR5";
            if (tokenStr == "OR6") return "OR6";
            if (tokenStr == "CE7") return "CE7"; // (
            if (tokenStr == "CE8") return "CE8"; // )
            if (tokenStr == "CE9") return "CE9"; // {
            if (tokenStr == "CE10") return "CE10"; // }
            if (tokenStr == "CE13") return "CE13"; // ;
            if (tokenStr == "CE16") return "CE16"; // ,
            if (tokenStr.StartsWith("CN")) return "CNU";
            if (tokenStr == "CAD") return "CAD";

            // Por defecto, usar el mismo string como tipo
            return tokenStr;
        }

        private void label5_MouseEnter(object sender, EventArgs e)
        {
            label5.Cursor = Cursors.Hand;
            label5.BackColor = Color.FromArgb(187, 222, 251);
        }

        private void label5_MouseLeave(object sender, EventArgs e)
        {
            label5.BackColor = Color.FromArgb(25, 70, 130);
        }

        private void dgvErrores_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void dgvSimbolos_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void dgvErroresSintacticos_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void rtSintaxis_TextChanged(object sender, EventArgs e)
        {

        }
    }
}
