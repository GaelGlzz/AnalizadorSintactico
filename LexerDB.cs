using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace AnalizadorLexico
{
    public class LexerDB
    {
        private DataTable _matriz;
        private readonly string _connectionString = "Server=localhost;Database=LyA;User Id=sa;Password=17012005;";
        private string _initErrorMessage = string.Empty;
        private readonly string _logFile = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "matriz_init_error.log");

        public LexerDB()
        {
            InicializarMatriz();
        }

        /// Intenta abrir la conexión y reporta el resultado
        public bool TestConnection(out string message)
        {
            try
            {
                using (var conn = new SqlConnection(_connectionString))
                {
                    conn.Open();
                    message = "Conexión exitosa";
                    return true;
                }
            }
            catch (Exception ex)
            {
                message = ex.Message;
                return false;
            }
        }

        /// Indica si la matriz fue cargada correctamente
        public bool IsMatrizLoaded()
        {
            return _matriz != null && _matriz.Rows.Count > 0;
        }

        /// Mensaje de error relacionado con la inicialización (si existe)
        public string GetInitError()
        {
            return _initErrorMessage;
        }

        /// Rutina 1 - Divide una línea en lexemas preservando cadenas entre comillas
        public List<string> LeerCadena(string lineaCompleta)
        {
            var result = new List<string>();
            if (lineaCompleta == null)
                return result;

            string linea = lineaCompleta.Trim();
            if (string.IsNullOrEmpty(linea))
                return result;

            // ── Detectar comentarios: si la línea comienza con #, retornar toda como comentario
            if (linea.StartsWith("#"))
            {
                result.Add(linea);
                return result;
            }

            bool inDouble = false;
            bool inSingle = false;
            var token = new System.Text.StringBuilder();

            for (int i = 0; i < linea.Length; i++)
            {
                char c = linea[i];

                // ── Manejo de cadenas entre comillas dobles ──────────────
                if (c == '"' && !inSingle)
                {
                    token.Append(c);
                    inDouble = !inDouble;
                    if (!inDouble && token.Length > 0)
                    {
                        result.Add(token.ToString());
                        token.Clear();
                    }
                    continue;
                }

                // ── Manejo de cadenas entre comillas simples ─────────────
                if (c == '\'' && !inDouble)
                {
                    token.Append(c);
                    inSingle = !inSingle;
                    if (!inSingle && token.Length > 0)
                    {
                        result.Add(token.ToString());
                        token.Clear();
                    }
                    continue;
                }

                // ── Dentro de comillas: acumular sin procesar ────────────
                if (inDouble || inSingle)
                {
                    token.Append(c);
                    continue;
                }

                // ── Espacio: cierra el token actual ──────────────────────
                if (char.IsWhiteSpace(c))
                {
                    if (token.Length > 0)
                    {
                        result.Add(token.ToString());
                        token.Clear();
                    }
                    continue;
                }

                // ── Punto decimal dentro de número: 15.42 ───────────────
                // Condición: el token ya tiene dígitos Y lo que sigue también es dígito
                if (c == '.'
                    && token.Length > 0
                    && char.IsDigit(token[token.Length - 1])
                    && i + 1 < linea.Length
                    && char.IsDigit(linea[i + 1]))
                {
                    token.Append(c);
                    continue;
                }

                // ── e/E exponencial: convertir siempre a E mayúscula ────────
                if ((c == 'e' || c == 'E')
                    && token.Length > 0
                    && char.IsDigit(token[token.Length - 1])
                    && i + 1 < linea.Length
                    && (char.IsDigit(linea[i + 1]) || linea[i + 1] == '+' || linea[i + 1] == '-'))
                {
                    token.Append('E');   // ← siempre mayúscula para que coincida con columna E1
                    continue;
                }

                // ── Signo +/- después de e/E: 1.5e+10 o 1.5e-3 ─────────
                // Condición: el token termina en 'e' o 'E' Y lo que sigue es dígito
                if ((c == '+' || c == '-')
                    && token.Length > 0
                    && (token[token.Length - 1] == 'e' || token[token.Length - 1] == 'E')
                    && i + 1 < linea.Length
                    && char.IsDigit(linea[i + 1]))
                {
                    token.Append(c);
                    continue;
                }

                token.Append(c);
            }

            if (token.Length > 0)
                result.Add(token.ToString());

            return result;
        }


        /// Rutina 2 - Carga la matriz desde la base de datos (una sola vez)
        public void InicializarMatriz()
        {
            _initErrorMessage = string.Empty;
            try
            {
                using (var conn = new SqlConnection(_connectionString))
                {
                    conn.Open();
                    using (var da = new SqlDataAdapter("SELECT * FROM MatrizTransicion", conn))
                    {
                        var dt = new DataTable();
                        da.Fill(dt);
                        _matriz = dt;
                    }
                }
            }
            catch (Exception ex)
            {
                _initErrorMessage = ex.Message;
                try
                {
                    File.WriteAllText(_logFile, DateTime.Now.ToString("s") + " - " + ex.ToString());
                }
                catch
                {
                    // ignore logging errors
                }
                MessageBox.Show($"Error al inicializar la matriz de transición:\n{ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                _matriz = null;
            }
        }


        /// Rutina 3 - Recorre la matriz para un lexema dado
        /// Utiliza autómata finito: fase 1 inicial (primer estado), transiciones por cada carácter,
        /// termina cuando se alcanza un estado de aceptación (columna CAT con categoría válida)
        public Token RecorrerMatriz(string lexema)
        {
            var token = new Token { Lexema = lexema, Estado = -1, Categoria = string.Empty, EsError = true };

            if (_matriz == null)
            {
                token.Categoria = "Matriz no inicializada";
                return token;
            }

            if (string.IsNullOrEmpty(lexema))
            {
                token.Categoria = "Lexema vacío";
                return token;
            }

            // ── Detectar comentarios directamente (comienzan con #) ──────────────
            if (lexema.StartsWith("#"))
            {
                token.Estado = 139;
                token.Categoria = "COM";
                token.EsError = false;
                return token;
            }

            int estadoActual = 1;

            // ── Helper: buscar fila por Fase ─────────────────────────────
            DataRow BuscarFila(int fase)
            {
                foreach (DataRow r in _matriz.Rows)
                    if (int.TryParse(r["Fase"].ToString(), out int f) && f == fase)
                        return r;
                return null;
            }

            // ── Helper: mapear carácter a nombre de columna ──────────────
            string CharAColumna(char c)
            {
                if (char.IsLower(c)) return c.ToString();
                if (char.IsUpper(c)) return c + "1";
                if (char.IsDigit(c)) return "_" + c;
                switch (c)
                {
                    case '+': return "OA1";
                    case '-': return "OA2";
                    case '*': return "OA3";
                    case '/': return "OA4";
                    case '<': return "OR3";
                    case '>': return "OR1";
                    case '=': return "ASI";
                    case '¡': return "CE1";
                    case '@': return "CE2";
                    case '$': return "CE3";
                    case '%': return "CE4";
                    case '^': return "CE5";
                    case '&': return "CE6";
                    case '(': return "CE7";
                    case ')': return "CE8";
                    case '{': return "CE9";
                    case '}': return "CE10";
                    case ':': return "CE11";
                    case '"': return "CE12";
                    case ';': return "CE13";  
                    case '?': return "CE14";
                    case '\\': return "CE15";
                    case ',': return "CE16";
                    case '.': return "CE17";
                    case '~': return "CE18";
                    case '[': return "CE19";
                    case ']': return "CE20";
                    case '_': return "CE21";
                    case '¿': return "CE22";
                    case '!': return "CE23";
                    case '\'': return "CAD";
                    case '#': return "COM";

                    default: return string.Empty;
                }
            }

            // ── Fase 1: recorrer cada carácter del lexema ────────────────
            for (int i = 0; i < lexema.Length; i++)
            {
                char car = lexema[i];
                string col = CharAColumna(car);

                if (string.IsNullOrEmpty(col) || !_matriz.Columns.Contains(col))
                {
                    token.Estado = estadoActual;
                    token.Categoria = $"Carácter '{car}' no reconocido";
                    token.EsError = true;
                    return token;
                }

                DataRow fila = BuscarFila(estadoActual);
                if (fila == null)
                {
                    token.Estado = estadoActual;
                    token.Categoria = $"Fase {estadoActual} no existe en la matriz";
                    token.EsError = true;
                    return token;
                }

                string celda = fila[col]?.ToString() ?? string.Empty;

                // Celda vacía o "Error" → token de error usando CAT del estado actual
                if (string.IsNullOrEmpty(celda) || celda.Equals("Error", StringComparison.OrdinalIgnoreCase))
                {
                    string catError = fila["CAT"]?.ToString().Trim() ?? string.Empty;
                    token.Estado = estadoActual;
                    token.Categoria = string.IsNullOrEmpty(catError)
                                      ? $"Transición inválida: Fase {estadoActual} + '{car}'"
                                      : catError;
                    token.EsError = true;
                    return token;
                }

                // La celda debe ser un número de fase destino
                if (!int.TryParse(celda, out int estadoDestino))
                {
                    token.Estado = estadoActual;
                    token.Categoria = $"Valor inesperado en matriz: '{celda}'";
                    token.EsError = true;
                    return token;
                }

                estadoActual = estadoDestino;
            }

            // ── Fase 2: todos los caracteres consumidos → seguir FDC ─────
            // La matriz usa un paso extra de FDC al terminar el lexema.
            // El estado actual tiene FDC = número → ese número es el estado
            // de aceptación real que tiene FDC = "ACEPTA".

            DataRow filaFinal = BuscarFila(estadoActual);
            if (filaFinal == null)
            {
                token.Estado = estadoActual;
                token.Categoria = $"Fase final {estadoActual} no existe";
                token.EsError = true;
                return token;
            }

            string fdcValor = filaFinal["FDC"]?.ToString().Trim() ?? string.Empty;

            // Caso A: el propio estado ya es de aceptación (FDC = "ACEPTA")
            if (fdcValor.Equals("ACEPTA", StringComparison.OrdinalIgnoreCase))
            {
                string cat = filaFinal["CAT"]?.ToString().Trim() ?? string.Empty;
                token.Estado = estadoActual;
                token.Categoria = cat;
                token.EsError = false;
                return token;
            }

            // Caso B: FDC es un número → saltar al estado de aceptación real
            // Caso B: FDC es un número → seguir la cadena hasta ACEPTA o Error
            if (int.TryParse(fdcValor, out int estadoSiguiente))
            {
                // Seguir saltando por FDC hasta encontrar ACEPTA o Error (máx 5 saltos)
                int saltos = 0;
                while (saltos < 5)
                {
                    DataRow filaSiguiente = BuscarFila(estadoSiguiente);
                    if (filaSiguiente == null)
                    {
                        token.Estado = estadoSiguiente;
                        token.Categoria = $"Fase {estadoSiguiente} no existe";
                        token.EsError = true;
                        return token;
                    }

                    string fdcSig = filaSiguiente["FDC"]?.ToString().Trim() ?? string.Empty;
                    string catSig = filaSiguiente["CAT"]?.ToString().Trim() ?? string.Empty;

                    // Llegamos a aceptación
                    if (fdcSig.Equals("ACEPTA", StringComparison.OrdinalIgnoreCase))
                    {
                        token.Estado = estadoSiguiente;
                        token.Categoria = catSig;
                        token.EsError = false;
                        return token;
                    }

                    // Llegamos a error
                    if (fdcSig.Equals("Error", StringComparison.OrdinalIgnoreCase))
                    {
                        token.Estado = estadoSiguiente;
                        token.Categoria = string.IsNullOrEmpty(catSig)
                                          ? "Token no reconocido"
                                          : catSig;
                        token.EsError = true;
                        return token;
                    }

                    // FDC vacío — no hay más cadena
                    if (string.IsNullOrEmpty(fdcSig))
                    {
                        token.Estado = estadoSiguiente;
                        token.Categoria = string.IsNullOrEmpty(catSig)
                                          ? "Token no reconocido"
                                          : catSig;
                        token.EsError = true;
                        return token;
                    }

                    // Seguir al siguiente eslabón de la cadena
                    if (!int.TryParse(fdcSig, out estadoSiguiente))
                    {
                        token.Estado = estadoSiguiente;
                        token.Categoria = $"FDC inesperado: '{fdcSig}'";
                        token.EsError = true;
                        return token;
                    }

                    saltos++;
                }

                token.Estado = estadoSiguiente;
                token.Categoria = "Cadena FDC demasiado larga";
                token.EsError = true;
                return token;
            }

            // Caso C: FDC es "Error" o valor no reconocido
            if (fdcValor.Equals("Error", StringComparison.OrdinalIgnoreCase))
            {
                string catErr = filaFinal["CAT"]?.ToString().Trim() ?? string.Empty;
                token.Estado = estadoActual;
                token.Categoria = string.IsNullOrEmpty(catErr)
                                  ? "Token no reconocido"
                                  : catErr;
                token.EsError = true;
                return token;
            }

            // Caso D: FDC vacío y no es aceptación → no reconocido
            token.Estado = estadoActual;
            token.Categoria = "Token no reconocido";
            token.EsError = true;
            return token;
        }

        /// Rutina 4 - Actualiza tabla de símbolos en memoria
        public void ActualizarTablaSimbolos(Token token, List<Simbolo> tablaSimbolos)
        {
            if (token == null || tablaSimbolos == null)
                return;

            if (token.Categoria == "IDENT" && token.EsError == false)
            {
                foreach (var s in tablaSimbolos)
                {
                    if (s.Nombre == token.Lexema)
                    {
                        //MessageBox.Show("Identificador duplicado", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }
                }

                var nuevo = new Simbolo
                {
                    NumID = tablaSimbolos.Count + 1,
                    Nombre = token.Lexema,
                    Tipo = string.Empty,
                    Valor = string.Empty
                };

                tablaSimbolos.Add(nuevo);
            }
        }

        /// Rutina 5 - Recupera un error: agrega fila a dgvErrores y marca token en rtbTokens
        public void RecuperarError(Token token, int numeroLinea, DataGridView dgvErrores, RichTextBox rtbTokens)
        {
            if (token == null || dgvErrores == null || rtbTokens == null)
                return;

            if (token.EsError)
            {
                // Asegurar que el grid tenga al menos 3 columnas
                while (dgvErrores.Columns.Count < 3)
                {
                    dgvErrores.Columns.Add(new DataGridViewTextBoxColumn 
                    { 
                        Name = "Column" + (dgvErrores.Columns.Count + 1),
                        HeaderText = dgvErrores.Columns.Count == 1 ? "Lexema" : "Error",
                        ReadOnly = true 
                    });
                }

                // Agregar error a la grid
                int rowIndex = dgvErrores.Rows.Add();
                dgvErrores.Rows[rowIndex].Cells[0].Value = numeroLinea;
                dgvErrores.Rows[rowIndex].Cells[1].Value = token.Lexema;
                dgvErrores.Rows[rowIndex].Cells[2].Value = token.Categoria;
                dgvErrores.Rows[rowIndex].DefaultCellStyle.ForeColor = System.Drawing.Color.Red;

                // marcar token en rtbTokens agregando el lexema en rojo
                rtbTokens.SelectionStart = rtbTokens.TextLength;
                rtbTokens.SelectionColor = System.Drawing.Color.Red;
                rtbTokens.AppendText($" [{token.Lexema}] ");
                rtbTokens.SelectionColor = rtbTokens.ForeColor;
            }
        }

        /// Mapea un carácter a la columna correspondiente en la matriz
        private string GetColumnForChar(char c)
        {
            // letras minúsculas
            if (char.IsLetter(c) && char.IsLower(c))
                return c.ToString();

            // letras mayúsculas - en la matriz usan A1..Z1
            if (char.IsLetter(c) && char.IsUpper(c))
                return c + "1";

            // dígitos 0-9
            if (char.IsDigit(c))
                return "_" + c;

            // operadores y caracteres especiales (adaptados a Lexer.cs)
            switch (c)
            {
                // Operadores aritméticos
                case '+': return "OA1";
                case '-': return "OA2";
                case '*': return "OA3";
                case '/': return "OA4";

                // Operadores relacionales
                case '<': return "OR3";
                case '>': return "OR1";
                case '=': return "ASI";

                // Caracteres especiales (CE) - Adaptados a Lexer.cs
                case '¡': return "CE1";
                case '@': return "CE2";
                case '$': return "CE3";
                case '%': return "CE4";
                case '^': return "CE5";
                case '&': return "CE6";
                case '(': return "CE7";
                case ')': return "CE8";
                case '{': return "CE9";
                case '}': return "CE10";
                case ':': return "CE11";
                case '"': return "CE12";
                case ';': return "DEL";  // Delimitador (como en Lexer.cs)
                case '?': return "CE14";
                case '\\': return "CE15";
                case ',': return "CE16";
                case '.': return "CE17";
                case '~': return "CE18";
                case '[': return "CE19";
                case ']': return "CE20";
                case '_': return "CE21";
                case '¿': return "CE22";
                case '!': return "CE23";   

                // Cadena (comillas)
                case '\'': return "CADE";

                default: return string.Empty;
            }
        }
    }
}
