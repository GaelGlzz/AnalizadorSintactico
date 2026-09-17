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
                case "Entero": return "Tipo de dato: Entero";
                case "Real": return "Tipo de dato: Real";
                case "Cadena": return "Tipo de dato: Cadena";
                case "Booleano": return "Tipo de dato: Booleano";
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

        // Inferir el tipo resultante de una asignación a partir de los tokens de la expresión
        private string InferirTipoDeAsignacion(List<Token> tokensExpr, List<Simbolo> tablaSimbolos, int numeroLinea)
        {
            if (tokensExpr == null || tokensExpr.Count == 0)
            {
                dgvErrores.Rows.Add(numeroLinea, "Expresión vacía o incompleta", "Semántico - Error en expresión");
                dgvErrores.Rows[dgvErrores.Rows.Count - 1].DefaultCellStyle.ForeColor = Color.DarkOrange;
                return "Error";
            }

            bool tieneReal = false;
            bool tieneEntero = false;

            var tiposIdentificadores = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

            foreach (var t in tokensExpr)
            {
                if (t == null)
                    continue;

                if (t.Categoria == "CAD")
                    return "Cadena"; // cadena domina

                if (t.Categoria == "CN REALES" || t.Categoria == "CN CON EXPONENTE")
                    tieneReal = true;
                else if (t.Categoria == "CN ENTEROS")
                    tieneEntero = true;
                else if (t.Categoria == "IDENT")
                {
                    var sym = tablaSimbolos?.FirstOrDefault(s => s.Nombre == t.Lexema);
                    if (sym == null || string.IsNullOrEmpty(sym.Tipo))
                    {
                        dgvErrores.Rows.Add(numeroLinea, $"Tipo desconocido para el identificador '{t.Lexema}'", "Semántico - Tipo desconocido");
                        dgvErrores.Rows[dgvErrores.Rows.Count - 1].DefaultCellStyle.ForeColor = Color.DarkOrange;
                        return "Error";
                    }
                    tiposIdentificadores.Add(sym.Tipo);
                }
            }

            // Si hay identificadores, validar coherencia entre ellos y con constantes
            if (tiposIdentificadores.Count > 0)
            {
                // Si todos los identificadores tienen el mismo tipo, devolverlo (con promoción a reales si hay reales)
                if (tiposIdentificadores.Count == 1)
                {
                    var tipoId = tiposIdentificadores.First();
                    if (tieneReal)
                        return "Real";
                    if (tieneEntero)
                        return tipoId.IndexOf("Real", StringComparison.OrdinalIgnoreCase) >= 0 ? "Real" : tipoId;
                    return tipoId;
                }

                // Identificadores con tipos distintos -> ambigüedad
                return "Error";
            }

            // Sólo constantes numéricas
            if (tieneReal)
                return "Real";
            if (tieneEntero)
                return "Entero";

            return "Error";
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

                // 1) Tokenizar toda la línea primero
                var tokensLinea = new List<Token>();
                foreach (var lex in lexemas)
                {
                    Token t;
                    try { t = lexerDB.RecorrerMatriz(lex); }
                    catch (Exception ex) { t = new Token { Lexema = lex, Estado = -1, Categoria = "ERROR: Excepción durante análisis: " + ex.Message, EsError = true }; }
                    t.Lexema = lex;
                    tokensLinea.Add(t);
                }

                // 2) Registrar identificadores válidos en la tabla de símbolos ANTES de inferir tipos
                //    (para que existan cuando busquemos IDENT = ... )
                foreach (var token in tokensLinea)
                {
                    if (!token.EsError)
                        lexerDB.ActualizarTablaSimbolos(token, tablaSimbolos);
                }

                // 3) Detectar asignaciones (IDENT = EXPRESION ;) e inferir el tipo de TODA la expresión aritmética
                for (int k = 0; k < tokensLinea.Count - 1; k++)
                {
                    if (!tokensLinea[k].EsError && tokensLinea[k].Categoria == "IDENT" &&
                        !tokensLinea[k + 1].EsError && tokensLinea[k + 1].Categoria == "ASI")
                    {
                        string nombreVar = tokensLinea[k].Lexema;

                        var tokensExpr = new List<Token>();
                        int j = k + 2;
                        while (j < tokensLinea.Count && tokensLinea[j].Categoria != "DEL") // DEL = ";"
                        {
                            if (!tokensLinea[j].EsError)
                                tokensExpr.Add(tokensLinea[j]);
                            j++;
                        }

                        var simboloDestino = tablaSimbolos.FirstOrDefault(s => s.Nombre == nombreVar);
                        string tipoResultado = InferirTipoConPilaSemantica(tokensExpr, tablaSimbolos, numeroLinea);

                        if (simboloDestino != null && tipoResultado != "Error")
                            simboloDestino.Tipo = tipoResultado;

                        k = j;
                    }
                }

                // 4) Detectar LEER <ident> ; -> se asume Entero por defecto
                for (int k = 0; k < tokensLinea.Count - 1; k++)
                {
                    if (!tokensLinea[k].EsError && tokensLinea[k].Categoria == "LEER" &&
                        !tokensLinea[k + 1].EsError && tokensLinea[k + 1].Categoria == "IDENT")
                    {
                        var simbolo = tablaSimbolos.FirstOrDefault(s => s.Nombre == tokensLinea[k + 1].Lexema);
                        if (simbolo != null && string.IsNullOrEmpty(simbolo.Tipo))
                            simbolo.Tipo = "Entero";
                    }
                }

                // 5) Detectar condiciones dentro de SI ( ... ) y validar que el resultado sea Booleano
                for (int k = 0; k < tokensLinea.Count - 1; k++)
                {
                    if (!tokensLinea[k].EsError && tokensLinea[k].Categoria == "SI" &&
                        !tokensLinea[k + 1].EsError && tokensLinea[k + 1].Categoria == "CE7") // (
                    {
                        var tokensCondicion = new List<Token>();
                        int j = k + 2;
                        int nivelParentesis = 1;

                        while (j < tokensLinea.Count && nivelParentesis > 0)
                        {
                            if (tokensLinea[j].Categoria == "CE7") nivelParentesis++;
                            else if (tokensLinea[j].Categoria == "CE8") nivelParentesis--;

                            if (nivelParentesis > 0 && !tokensLinea[j].EsError)
                                tokensCondicion.Add(tokensLinea[j]);

                            j++;
                        }

                        string tipoCondicion = InferirTipoCondicion(tokensCondicion, tablaSimbolos, numeroLinea);

                        if (tipoCondicion != "Booleano" && tipoCondicion != "Error")
                        {
                            string mensaje = $"La condición de SI debe ser Booleana, se obtuvo '{tipoCondicion}'";
                            dgvErrores.Rows.Add(numeroLinea, mensaje, "Semántico - Condición no booleana");
                            dgvErrores.Rows[dgvErrores.Rows.Count - 1].DefaultCellStyle.ForeColor = Color.DarkOrange;
                        }
                    }
                }

                // 6) Imprimir tokens en rtbTokens y registrar errores léxicos
                rtbTokens.SelectionColor = Color.Blue;
                rtbTokens.AppendText($"{numeroLinea}  ");
                rtbTokens.SelectionColor = Color.Black;

                foreach (var token in tokensLinea)
                {
                    if (token.EsError)
                    {
                        string tipoError = ClasificarTipoError(token.Categoria);
                        dgvErrores.Rows.Add(numeroLinea, token.Categoria, tipoError);
                        dgvErrores.Rows[dgvErrores.Rows.Count - 1].DefaultCellStyle.ForeColor = Color.Red;

                        rtbTokens.SelectionStart = rtbTokens.TextLength;
                        rtbTokens.SelectionColor = Color.Red;
                        rtbTokens.AppendText($"Error {token.Lexema} ");
                    }
                    else
                    {
                        tokensLexico.Add(token);

                        var simboloActual = tablaSimbolos.FirstOrDefault(s => s.Nombre == token.Lexema);
                        string etiquetaMostrar = token.Categoria;

                        if (token.Categoria == "IDENT" && simboloActual != null)
                        {
                            etiquetaMostrar = $"IDENT{simboloActual.NumID}";
                            rtbTokens.SelectionColor = Color.Orange;
                        }
                        else
                        {
                            rtbTokens.SelectionColor = Color.Black;
                        }

                        rtbTokens.SelectionStart = rtbTokens.TextLength;
                        rtbTokens.AppendText($" {etiquetaMostrar} ");
                        rtbTokens.SelectionColor = rtbTokens.ForeColor;
                    }
                }

                rtbTokens.AppendText("\n");
            }

            // 7) Llenar dgvSimbolos: Número | Nombre | Tipo
            foreach (var s in tablaSimbolos)
            {
                int idx = dgvSimbolos.Rows.Add();
                dgvSimbolos.Rows[idx].Cells[0].Value = s.NumID;
                dgvSimbolos.Rows[idx].Cells[1].Value = s.Nombre;
                dgvSimbolos.Rows[idx].Cells[2].Value = string.IsNullOrEmpty(s.Tipo) ? "Sin determinar" : s.Tipo;
                dgvSimbolos.Rows[idx].DefaultCellStyle.ForeColor = Color.Red;
            }

            rtbFuente.ReadOnly = true;
        }

        // Determina el "tipo de dato" a partir de la categoría léxica del valor asignado
        private string InferirTipoDato(string categoria)
        {
            switch (categoria)
            {
                case "CN ENTEROS": return "Entero";
                case "CN REALES": return "Real";
                case "CN CON EXPONENTE": return "Real (Exponencial)";
                case "CAD": return "Cadena";
                case "IDENT": return "Referencia"; // asignación de otra variable
                default: return "Desconocido";
            }
        }

        // Tipo de un operando individual (IDENT, CNU o CAD)
        private string TipoDeOperando(Token token, List<Simbolo> tablaSimbolos)
        {
            switch (token.Categoria)
            {
                case "CN ENTEROS": return "Entero";
                case "CN REALES": return "Real";
                case "CN CON EXPONENTE": return "Real";
                case "CAD": return "Cadena";
                case "IDENT":
                    var s = tablaSimbolos.FirstOrDefault(x => x.Nombre == token.Lexema);
                    return (s != null && !string.IsNullOrEmpty(s.Tipo) && s.Tipo != "Sin determinar")
                        ? s.Tipo : "Desconocido";
                default:
                    return "Desconocido";
            }
        }

        // Combina dos tipos según el operador; marca esError si son incompatibles
        // Soporta: aritméticos (+ - * /), relacionales (> >= < <= <> ==) y lógicos (y, o)
        private string CombinarTipos(string tipoIzq, string operador, string tipoDer, out bool esError)
        {
            esError = false;
            if (tipoIzq == "Desconocido" || tipoDer == "Desconocido") return "Desconocido";

            bool esNumIzq = tipoIzq == "Entero" || tipoIzq == "Real";
            bool esNumDer = tipoDer == "Entero" || tipoDer == "Real";

            // --- Operadores aritméticos: + - * / ---
            if (operador == "+" || operador == "-" || operador == "*" || operador == "/")
            {
                if (esNumIzq && esNumDer)
                    return (tipoIzq == "Real" || tipoDer == "Real") ? "Real" : "Entero";

                if (tipoIzq == "Cadena" && tipoDer == "Cadena" && operador == "+")
                    return "Cadena"; // concatenación

                esError = true;
                return "Error";
            }

            // --- Operadores relacionales: > >= < <= <> == ---
            if (operador == ">" || operador == ">=" || operador == "<" ||
                operador == "<=" || operador == "<>" || operador == "==")
            {
                // Solo se pueden comparar tipos compatibles entre sí (numérico-numérico, o cadena-cadena)
                bool comparablesNum = esNumIzq && esNumDer;
                bool comparablesCad = tipoIzq == "Cadena" && tipoDer == "Cadena";

                if (comparablesNum || comparablesCad)
                    return "Booleano"; // toda comparación produce un booleano

                esError = true;
                return "Error";
            }

            // --- Operadores lógicos: Y (AND), O (OR) ---
            var opLower = operador.ToLower();
            if (opLower == "y" || opLower == "o" || opLower == "and" || opLower == "or")
            {
                if (tipoIzq == "Booleano" && tipoDer == "Booleano")
                    return "Booleano";

                esError = true;
                return "Error";
            }

            esError = true;
            return "Error";
        }


        // Infiere el tipo de una condición completa: comparaciones + operadores lógicos + NO
        private string InferirTipoCondicion(List<Token> tokensExpr, List<Simbolo> tablaSimbolos, int numeroLinea)
        {
            if (tokensExpr.Count == 0) return "Desconocido";

            int idx = 0;
            string resultado = ParseOperandoCondicion(tokensExpr, ref idx, tablaSimbolos, numeroLinea);

            while (idx < tokensExpr.Count)
            {
                Token opToken = tokensExpr[idx];
                if (opToken.Categoria != "Y" && opToken.Categoria != "O")
                    break; // token inesperado, se detiene aquí

                idx++;
                string siguiente = ParseOperandoCondicion(tokensExpr, ref idx, tablaSimbolos, numeroLinea);

                bool esError;
                resultado = CombinarTipos(resultado, opToken.Lexema.ToLower(), siguiente, out esError);

                if (esError)
                {
                    string mensaje = $"Operador lógico '{opToken.Lexema}' requiere operandos booleanos";
                    dgvErrores.Rows.Add(numeroLinea, mensaje, "Semántico - Tipos incompatibles");
                    dgvErrores.Rows[dgvErrores.Rows.Count - 1].DefaultCellStyle.ForeColor = Color.DarkOrange;
                    return "Error";
                }
            }

            return resultado;
        }

        // Procesa un operando de condición: puede venir precedido de NO, o ser una comparación simple
        private string ParseOperandoCondicion(List<Token> tokens, ref int idx, List<Simbolo> tablaSimbolos, int numeroLinea)
        {
            bool negado = false;
            if (idx < tokens.Count && tokens[idx].Categoria == "NO")
            {
                negado = true;
                idx++;
            }

            if (idx >= tokens.Count)
            {
                Error_TipoCondicionVacia(numeroLinea);
                return "Error";
            }

            // Primer operando de la comparación
            string tipoIzq = TipoDeOperando(tokens[idx], tablaSimbolos);
            idx++;

            string resultado;

            // ¿Sigue un operador relacional?
            if (idx < tokens.Count && EsOperadorRelacional(tokens[idx].Categoria))
            {
                string opRelacional = tokens[idx].Lexema;
                idx++;

                if (idx >= tokens.Count)
                {
                    resultado = "Error";
                }
                else
                {
                    string tipoDer = TipoDeOperando(tokens[idx], tablaSimbolos);
                    idx++;

                    bool esError;
                    resultado = CombinarTipos(tipoIzq, opRelacional, tipoDer, out esError);

                    if (esError)
                    {
                        string mensaje = $"Comparación no válida: '{tipoIzq}' {opRelacional} '{tipoDer}'";
                        dgvErrores.Rows.Add(numeroLinea, mensaje, "Semántico - Tipos incompatibles");
                        dgvErrores.Rows[dgvErrores.Rows.Count - 1].DefaultCellStyle.ForeColor = Color.DarkOrange;
                        resultado = "Error";
                    }
                }
            }
            else
            {
                // No hay operador relacional: el operando ya debería ser booleano (ej: variable booleana sola)
                resultado = tipoIzq;
            }

            return negado ? (resultado == "Booleano" ? "Booleano" : "Error") : resultado;
        }


        // ============================================================
        // ANÁLISIS DE TIPOS MEDIANTE PILA SEMÁNTICA
        // ============================================================
        // Reemplaza la evaluación manual anterior (una función para aritmética,
        // otra para condiciones) por UN SOLO evaluador de dos pilas que respeta
        // la jerarquía real de operadores del lenguaje, de mayor a menor prioridad:
        //
        //   1) ( )                 agrupación
        //   2) NO                  negación lógica (unario)
        //   3) *  /                multiplicativos
        //   4) +  -                aditivos
        //   5) >  >=  <  <=  <>  ==  relacionales
        //   6) Y                   AND
        //   7) O                   OR  (la de menor prioridad)
        //
        // Funciona como el analizador ascendente descrito en la teoría de la
        // pila semántica: cada operando (IDENT/CNU/CAD) empuja su TIPO a
        // "pilaTipos"; cada operador se compara contra el tope de
        // "pilaOperadores" y, si el de la pila tiene igual o mayor prioridad,
        // se "reduce" (se aplica) antes de apilar el nuevo operador — igual que
        // se reduce una producción al alcanzar un símbolo de menor precedencia.

        // Detecta los tipos que existen en el lenguaje: Entero, Real, Cadena, Booleano.
        // Un IDENT toma el tipo ya inferido y guardado en la tabla de símbolos.
        // (Ya definido arriba como TipoDeOperando — se reutiliza aquí sin cambios.)

        // Precedencia de cada operador. A mayor número, mayor prioridad (se reduce primero).
        private int PrecedenciaOperador(string categoria)
        {
            switch (categoria)
            {
                case "NO": return 5;                                     // negación (unario)
                case "OA3": case "OA4": return 4;                        // * /
                case "OA1": case "OA2": return 3;                        // + -
                case "OR1":
                case "OR2":
                case "OR3":
                case "OR4":
                case "OR5":
                case "OR6": return 2;            // > >= < <= <> ==
                case "Y": return 1;                                      // AND
                case "O": return 0;                                      // OR (la más baja)
                default: return -1;
            }
        }

        // Indica si la categoría del token corresponde a un operador soportado
        private bool EsOperadorValido(string categoria)
        {
            return categoria == "OA1" || categoria == "OA2" || categoria == "OA3" || categoria == "OA4" ||
                   EsOperadorRelacional(categoria) ||
                   categoria == "Y" || categoria == "O" || categoria == "NO";
        }

        private bool EsOperadorRelacional(string categoria)
        {
            return categoria == "OR1" || categoria == "OR2" || categoria == "OR3" ||
                   categoria == "OR4" || categoria == "OR5" || categoria == "OR6";
        }

        // Traduce la categoría del lexer al símbolo textual que CombinarTipos espera
        private string ObtenerSimboloOperador(string categoria)
        {
            switch (categoria)
            {
                case "OA1": return "+";
                case "OA2": return "-";
                case "OA3": return "*";
                case "OA4": return "/";
                case "OR1": return ">";
                case "OR2": return ">=";
                case "OR3": return "<";
                case "OR4": return "<=";
                case "OR5": return "<>";
                case "OR6": return "==";
                case "Y": return "y";
                case "O": return "o";
                default: return categoria;
            }
        }

        // Saca un operador de "pilaOperadores" y lo aplica sobre "pilaTipos".
        // NO es unario (un solo operando); el resto son binarios (dos operandos).
        private bool AplicarOperador(string operador, Stack<string> pilaTipos, int numeroLinea)
        {
            if (operador == "NO")
            {
                if (pilaTipos.Count < 1)
                {
                    Error_TipoCondicionVacia(numeroLinea);
                    pilaTipos.Push("Error");
                    return false;
                }

                string operando = pilaTipos.Pop();
                if (operando == "Booleano")
                {
                    pilaTipos.Push("Booleano");
                    return true;
                }

                string mensajeNo = $"El operador 'NO' requiere un operando Booleano, se obtuvo '{operando}'";
                dgvErrores.Rows.Add(numeroLinea, mensajeNo, "Semántico - Tipos incompatibles");
                dgvErrores.Rows[dgvErrores.Rows.Count - 1].DefaultCellStyle.ForeColor = Color.DarkOrange;
                pilaTipos.Push("Error");
                return false;
            }

            // Operadores binarios: +, -, *, /, relacionales, Y, O
            if (pilaTipos.Count < 2)
            {
                Error_TipoCondicionVacia(numeroLinea);
                pilaTipos.Push("Error");
                return false;
            }

            string tipoDer = pilaTipos.Pop();
            string tipoIzq = pilaTipos.Pop();
            string simbolo = ObtenerSimboloOperador(operador);

            bool esError;
            string resultado = CombinarTipos(tipoIzq, simbolo, tipoDer, out esError);

            if (esError)
            {
                string mensaje = $"Operación no válida: '{tipoIzq}' {simbolo} '{tipoDer}'";
                dgvErrores.Rows.Add(numeroLinea, mensaje, "Semántico - Tipos incompatibles");
                dgvErrores.Rows[dgvErrores.Rows.Count - 1].DefaultCellStyle.ForeColor = Color.DarkOrange;
                pilaTipos.Push("Error");
                return false;
            }

            pilaTipos.Push(resultado);
            return true;
        }

        private void Error_TipoCondicionVacia(int numeroLinea)
        {
            dgvErrores.Rows.Add(numeroLinea, "Expresión vacía o mal formada", "Semántico - Expresión inválida");
            dgvErrores.Rows[dgvErrores.Rows.Count - 1].DefaultCellStyle.ForeColor = Color.DarkOrange;
        }

        // Punto de entrada único: analiza CUALQUIER expresión (aritmética, relacional,
        // lógica, con paréntesis anidados) usando la pila semántica y la jerarquía
        // de operadores. Sustituye a las asignaciones (IDENT = EXPR ;) y a las
        // condiciones (SI (...), MIENTRAS ... HACER).
        private string InferirTipoConPilaSemantica(List<Token> tokensExpr, List<Simbolo> tablaSimbolos, int numeroLinea)
        {
            if (tokensExpr.Count == 0) return "Desconocido";

            Stack<string> pilaTipos = new Stack<string>();
            Stack<string> pilaOperadores = new Stack<string>();

            foreach (var token in tokensExpr)
            {
                if (token.Categoria == "CE7") // "(" -> abrir grupo
                {
                    pilaOperadores.Push("(");
                }
                else if (token.Categoria == "CE8") // ")" -> reducir hasta el "(" correspondiente
                {
                    while (pilaOperadores.Count > 0 && pilaOperadores.Peek() != "(")
                    {
                        string op = pilaOperadores.Pop();
                        if (!AplicarOperador(op, pilaTipos, numeroLinea))
                            return "Error";
                    }
                    if (pilaOperadores.Count > 0)
                        pilaOperadores.Pop(); // descartar el "("
                }
                else if (EsOperadorValido(token.Categoria))
                {
                    // Reduce todo lo que en la pila tenga igual o mayor prioridad
                    // antes de apilar el operador nuevo (jerarquía de operaciones)
                    while (pilaOperadores.Count > 0 && pilaOperadores.Peek() != "(" &&
                           PrecedenciaOperador(pilaOperadores.Peek()) >= PrecedenciaOperador(token.Categoria))
                    {
                        string op = pilaOperadores.Pop();
                        if (!AplicarOperador(op, pilaTipos, numeroLinea))
                            return "Error";
                    }
                    pilaOperadores.Push(token.Categoria);
                }
                else
                {
                    // Operando: IDENT, CN ENTEROS/REALES/CON EXPONENTE, o CAD
                    pilaTipos.Push(TipoDeOperando(token, tablaSimbolos));
                }
            }

            // Reducir los operadores restantes en la pila
            while (pilaOperadores.Count > 0)
            {
                string op = pilaOperadores.Pop();
                if (op == "(") continue; // paréntesis sin cerrar (el sintáctico ya lo reporta aparte)
                if (!AplicarOperador(op, pilaTipos, numeroLinea))
                    return "Error";
            }

            if (pilaTipos.Count == 0) return "Desconocido";
            return pilaTipos.Pop();
        }

        // Clasifica el error léxico en un "tipo" legible
        private string ClasificarTipoError(string categoriaError)
        {
            if (categoriaError.Contains("no reconocido")) return "Léxico - Carácter no válido";
            if (categoriaError.Contains("Transición inválida")) return "Léxico - Transición inválida";
            if (categoriaError.Contains("Fase")) return "Léxico - Estado inválido";
            if (categoriaError.Contains("Identificador")) return "Léxico - Identificador inválido";
            if (categoriaError.Contains("numérica")) return "Léxico - Constante numérica inválida";
            if (categoriaError.Contains("Excepción")) return "Léxico - Error interno";
            return "Léxico - Error no clasificado";
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

        // Añadir este método dentro de la clase Form1 (por ejemplo al final de Form1.cs)
        private string InferirTipoDeAsignacion(string lexema)
        {
            if (string.IsNullOrEmpty(lexema))
                return "DESCONOCIDO";

            // Enteros
            if (Regex.IsMatch(lexema, @"^[+-]?\d+$"))
                return "Entero";

            // Reales (con o sin exponente)
            if (Regex.IsMatch(lexema, @"^[+-]?(\d+\.\d*|\d*\.\d+)([eE][+-]?\d+)?$"))
                return "Real";

            // Cadena entre comillas
            if ((lexema.StartsWith("\"") && lexema.EndsWith("\"")) || (lexema.StartsWith("'") && lexema.EndsWith("'")))
                return "Cadena";

            // Booleanos literales (si aplica)
            if (lexema.Equals("true", StringComparison.OrdinalIgnoreCase) || lexema.Equals("false", StringComparison.OrdinalIgnoreCase))
                return "Booleano";

            // Identificador por defecto
            return "IDENT";
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