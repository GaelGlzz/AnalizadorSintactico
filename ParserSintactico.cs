using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

namespace AnalizadorLexico
{
    public class ParserSintactico
    {
        #region Clases y Estructuras Internas

        // Información de una variable
        public class VariableInfo
        {
            public string Nombre { get; set; }
            public string Tipo { get; set; }
            public object Valor { get; set; }

            public override string ToString()
            {
                return $"{Nombre} | {Tipo} | {Valor}";
            }
        }

        public class TokenSintactico
        {
            public string Tipo { get; set; }
            public string Valor { get; set; }
            public int Linea { get; set; }

            public override string ToString()
            {
                return $"{Tipo}({Valor})";
            }
        }

        #endregion

        #region Propiedades Privadas

        private List<TokenSintactico> tokens;
        private int puntero = 0;
        private DataGridView dgvErroresSintacticos;
        private RichTextBox rtSintaxis;
        private int indentacion = 0;
        private int lineaSintaxis = 0;
        private const string INDENT = "  ";

        // Tabla de variables
        private Dictionary<string, VariableInfo> variables =
            new Dictionary<string, VariableInfo>(StringComparer.OrdinalIgnoreCase);

        #endregion

        #region Constructor

        /// <param name="tokensDelLexer">Lista de tokens del analizador lexico</param>
        /// <param name="dgvErrores">DataGridView para mostrar errores sintacticos</param>
        /// <param name="rtSintaxis">RichTextBox para mostrar la sintaxis detectada</param>
        public ParserSintactico(
            List<TokenSintactico> tokensDelLexer,
            DataGridView dgvErrores,
            RichTextBox rtSintaxis)
        {
            tokens = tokensDelLexer ?? new List<TokenSintactico>();
            this.dgvErroresSintacticos = dgvErrores;
            this.rtSintaxis = rtSintaxis;
        }

        #endregion

        #region Metodos Auxiliares

        // Avanza al siguiente token
        private void NextToken()
        {
            puntero++;
        }

        // Obtiene el token actual sin avanzar
        private TokenSintactico TokenActual()
        {
            if (puntero < tokens.Count)
                return tokens[puntero];

            int ultimaLinea = tokens.Count > 0
                ? tokens[tokens.Count - 1].Linea
                : 1;

            return new TokenSintactico
            {
                Tipo = "EOF",
                Valor = "",
                Linea = ultimaLinea
            };
        }

        // Obtiene el tipo del token actual
        private string TipoActual()
        {
            return TokenActual().Tipo;
        }

        // Verifica si se ha llegado al final del archivo
        private bool EsEOF()
        {
            return puntero >= tokens.Count || TipoActual() == "EOF";
        }

        /// <param name="tipoCodigo">Código del token (ej: "PR1", "CE13")</param>
        /// <returns>Nombre legible del token</returns>
        private string ObtenerNombreToken(string tipoCodigo)
        {
            if (string.IsNullOrEmpty(tipoCodigo))
                return tipoCodigo;

            switch (tipoCodigo)
            {
                // Palabras reservadas
                case "PR1": return "INICIO";
                case "PR2": return "FIN";
                case "PR3": return "LEER";
                case "PR4": return "MOSTRAR";
                case "PR5": return "SI";
                case "PR6": return "ENTONCES";
                case "PR7": return "SINO";
                case "PR8": return "DESDE";
                case "PR9": return "HASTA";
                case "PR10": return "REPETIR";
                case "PR11": return "ROMPER";
                case "PR12": return "PARA";
                case "PR13": return "HACER";
                case "PR14": return "MIENTRAS";
                case "PR15": return "LIMP";
                case "PR16": return "IR";
                case "PR17": return "ESCRIBIR";
                case "PR18": return "FUNCION";
                case "PR19": return "RETORNAR";
                case "PR20": return "VAR";
                case "PR21": return "NUEVO";

                // Caracteres especiales
                case "CE13": return ";";
                case "CE16": return ",";
                case "CE7": return "(";
                case "CE8": return ")";
                case "CE9": return "{";
                case "CE10": return "}";
                case "CE11": return ":";
                case "CE12": return '"'.ToString();
                case "CE14": return "?";
                case "CE15": return "\\";
                case "CE17": return ".";
                case "CE18": return "~";
                case "CE19": return "[";
                case "CE20": return "]";
                case "CE21": return "_";
                case "CE22": return "¿";
                case "CE23": return "!";

                // Operadores
                case "ASI": return "=";
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
                case "OPL1": return "Y";
                case "OPL2": return "O";
                case "OPL3": return "NO";

                // Tipos de datos
                case "IDENT": return "IDENT";
                case "CNU": return "CNU";
                case "CAD": return "CAD";
                case "EOF": return "fin de archivo";

                default: return tipoCodigo;
            }
        }

        /// <param name="tipoCodigo">Código del token</param>
        /// <param name="valor">Valor del token</param>
        /// <returns>Representación del token para rtSintaxis</returns>
        private string ObtenerRepresentacionSintaxis(
            string tipoCodigo,
            string valor = "")
        {
            if (string.IsNullOrEmpty(tipoCodigo))
                return tipoCodigo;

            switch (tipoCodigo)
            {
                // Palabras reservadas
                case "PR1": return "inicio";
                case "PR2": return "fin";
                case "PR3": return "leer";
                case "PR4": return "mostrar";
                case "PR5": return "si";
                case "PR6": return "entonces";
                case "PR7": return "sino";
                case "PR8": return "desde";
                case "PR9": return "hasta";
                case "PR10": return "repetir";
                case "PR11": return "romper";
                case "PR12": return "para";
                case "PR13": return "hacer";
                case "PR14": return "mientras";
                case "PR15": return "limp";
                case "PR16": return "ir";
                case "PR17": return "escribir";
                case "PR18": return "funcion";
                case "PR19": return "retornar";
                case "PR20": return "var";
                case "PR21": return "nuevo";

                // Caracteres especiales
                case "CE13": return ";";
                case "CE16": return ",";
                case "CE7": return "(";
                case "CE8": return ")";
                case "CE9": return "{";
                case "CE10": return "}";
                case "CE11": return ":";
                case "CE12": return '"'.ToString();
                case "CE14": return "?";
                case "CE15": return "\\";
                case "CE17": return ".";
                case "CE18": return "~";
                case "CE19": return "[";
                case "CE20": return "]";
                case "CE21": return "_";
                case "CE22": return "¿";
                case "CE23": return "!";

                // Operadores
                case "ASI": return "=";
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
                case "OPL1": return "y";
                case "OPL2": return "o";
                case "OPL3": return "no";

                // Tipos de datos
                case "IDENT":
                    return !string.IsNullOrEmpty(valor) ? valor : "IDENT";

                case "CNU":
                    return !string.IsNullOrEmpty(valor) ? valor : "CNU";

                case "CAD":
                    return !string.IsNullOrEmpty(valor) ? valor : "CAD";

                case "EOF":
                    return "fin de archivo";

                default:
                    return tipoCodigo;
            }
        }

        // Valida el token y registra error
        private bool Match(string tipoEsperado)
        {
            if (TipoActual() == tipoEsperado)
            {
                NextToken();
                return true;
            }
            else
            {
                int linea = TokenActual().Linea;
                string tokenEncontrado = TipoActual();
                string valorEncontrado = TokenActual().Valor;

                string nombreEsperado = ObtenerNombreToken(tipoEsperado);
                string nombreEncontrado = ObtenerNombreToken(tokenEncontrado);

                string mostrarEncontrado = nombreEncontrado;

                if (!string.IsNullOrEmpty(valorEncontrado) &&
                    valorEncontrado != nombreEncontrado)
                {
                    mostrarEncontrado = valorEncontrado;
                }

                string mensaje =
                    $"Se esperaba '{nombreEsperado}', encontrado '{mostrarEncontrado}'";

                Error(mensaje);
                return false;
            }
        }

        // Agrega una fila al DataGridView de errores
        private void Error(string mensaje)
        {
            string errorCompleto = mensaje;

            if (dgvErroresSintacticos != null &&
                dgvErroresSintacticos.InvokeRequired)
            {
                dgvErroresSintacticos.Invoke(new Action(() =>
                {
                    dgvErroresSintacticos.Rows.Add(
                        TokenActual().Linea,
                        errorCompleto);
                }));
            }
            else if (dgvErroresSintacticos != null)
            {
                dgvErroresSintacticos.Rows.Add(
                    TokenActual().Linea,
                    errorCompleto);
            }
        }

        // Registra un error en una línea específica
        private void ErrorEnLinea(string mensaje, int linea)
        {
            string errorCompleto = mensaje;

            if (dgvErroresSintacticos != null &&
                dgvErroresSintacticos.InvokeRequired)
            {
                dgvErroresSintacticos.Invoke(new Action(() =>
                {
                    dgvErroresSintacticos.Rows.Add(
                        linea,
                        errorCompleto);
                }));
            }
            else if (dgvErroresSintacticos != null)
            {
                dgvErroresSintacticos.Rows.Add(
                    linea,
                    errorCompleto);
            }
        }

        // Sincroniza el parser
        private void Sincronizar()
        {
            int tokensConsumidos = 0;

            while (!EsEOF() &&
                   TipoActual() != "CE13" &&
                   TipoActual() != "PR2")
            {
                NextToken();
                tokensConsumidos++;

                if (tokensConsumidos >= 10)
                    break;
            }
        }

        // Escribe en rtSintaxis
        private void EscribirSintaxis(string texto)
        {
            lineaSintaxis++;

            string numeroLinea =
                lineaSintaxis.ToString().PadLeft(3);

            string textoConLinea =
                $"{numeroLinea}  {texto}";

            if (rtSintaxis != null &&
                rtSintaxis.InvokeRequired)
            {
                rtSintaxis.Invoke(new Action(() =>
                {
                    rtSintaxis.AppendText(
                        new string(' ', indentacion * 2) +
                        textoConLinea +
                        "\n");
                }));
            }
            else if (rtSintaxis != null)
            {
                rtSintaxis.AppendText(
                    new string(' ', indentacion * 2) +
                    textoConLinea +
                    "\n");
            }
        }

        #endregion

        #region Metodo Publico Parse

        // Metodo publico que inicia el analisis sintactico
        public void Parse()
        {
            if (rtSintaxis != null)
            {
                rtSintaxis.Clear();
            }

            // Reiniciar
            puntero = 0;
            indentacion = 0;
            lineaSintaxis = 0;

            // Reiniciar variables para cada análisis
            variables.Clear();

            // Iniciar parseo
            ParseS();
        }

        // Permite obtener las variables desde otra clase
        public Dictionary<string, VariableInfo> ObtenerVariables()
        {
            return variables;
        }

        #endregion

        #region Producciones Principales - Gramática Oficial KaViGex

        // S → PR1 L_INSTR PR2 CE13
        private void ParseS()
        {
            if (!Match("PR1")) return;

            EscribirSintaxis("inicio");

            ParseL_INSTR();

            if (!Match("PR2")) return;
            if (!Match("CE13")) return;

            EscribirSintaxis("fin");
        }

        // L_INSTR → INSTR L_INSTR | ε
        private void ParseL_INSTR()
        {
            while (!EsEOF() && TipoActual() != "EOF")
            {
                if (TipoActual() == "PR2" && indentacion == 0)
                    break;

                if (TipoActual() == "CE13" && indentacion == 0)
                {
                    NextToken();
                    continue;
                }

                string tipo = TipoActual();

                if (!EsInicioInstruccion(tipo))
                {
                    NextToken();
                    continue;
                }

                try
                {
                    indentacion++;

                    ParseINSTR();

                    indentacion--;
                }
                catch
                {
                    indentacion--;

                    if (TipoActual() == "PR2" && indentacion > 0)
                        break;

                    Sincronizar();

                    if (TipoActual() == "CE13")
                        NextToken();

                    if (EsEOF() ||
                        (TipoActual() == "PR2" && indentacion == 0))
                    {
                        break;
                    }
                }
            }
        }

        // Verifica si el token puede iniciar una instrucción
        private bool EsInicioInstruccion(string tipo)
        {
            return tipo == "IDENT" ||
                   tipo == "PR2" ||
                   tipo == "PR3" ||
                   tipo == "PR4" ||
                   tipo == "PR5" ||
                   tipo == "PR7" ||
                   tipo == "PR8" ||
                   tipo == "PR10" ||
                   tipo == "PR11" ||
                   tipo == "PR12" ||
                   tipo == "PR14" ||
                   tipo == "PR15" ||
                   tipo == "PR16" ||
                   tipo == "PR17" ||
                   tipo == "PR18" ||
                   tipo == "PR19" ||
                   tipo == "PR20" ||
                   tipo == "PR21";
        }

        // INSTR
        private void ParseINSTR()
        {
            if (EsEOF())
                return;

            if (TipoActual() == "PR2" && indentacion == 0)
                return;

            if (TipoActual() == "PR2" && indentacion > 0)
                throw new Exception("FIN en bloque interno");

            string tipo = TipoActual();

            try
            {
                switch (tipo)
                {
                    case "IDENT":
                        ParseASIG();
                        break;

                    case "PR3":
                        ParseLEER();
                        break;

                    case "PR4":
                        ParseMOSTRAR();
                        break;

                    case "PR5":
                        ParseSI();
                        break;

                    case "PR7":
                        ParseSINO_OPC();
                        break;

                    case "PR8":
                        ParseDESDE();
                        break;

                    case "PR10":
                        ParseREPETIR();
                        break;

                    case "PR11":
                        ParseROMPER();
                        break;

                    case "PR12":
                        ParsePARA();
                        break;

                    case "PR14":
                        ParseMIENTRAS();
                        break;

                    case "PR15":
                        ParseLIMP();
                        break;

                    case "PR16":
                        ParseIR();
                        break;

                    case "PR17":
                        ParseESCRIBIR();
                        break;

                    case "PR18":
                        ParseFUNCION();
                        break;

                    case "PR19":
                        ParseRETORNAR();
                        break;

                    case "PR20":
                        ParseVAR();
                        break;

                    case "PR21":
                        ParseNUEVO();
                        break;

                    default:
                        Error(
                            $"Se esperaba instruccion, se encontro " +
                            $"{ObtenerNombreToken(tipo)}");

                        Sincronizar();
                        break;
                }
            }
            catch
            {
                Sincronizar();

                if (TipoActual() == "CE13")
                    NextToken();
            }
        }

        #endregion

        #region Instrucciones Simples

        // ASIG → IDENT ASI EXP CE13
        // También permite:
        // IDENT ASI CONDICION CE13
        private void ParseASIG()
        {
            string varName = TokenActual().Valor;

            if (!Match("IDENT"))
                return;

            if (!Match("ASI"))
                return;

            int lineaAsignacion = TokenActual().Linea;

            if (TipoActual() == "CE13")
            {
                ErrorEnLinea(
                    $"Se esperaba expresión después de '=' en asignación de {varName}",
                    lineaAsignacion);

                NextToken();

                EscribirSintaxis(
                    $"asignación {varName} = <expresión>");

                return;
            }

            string exp;
            bool esBooleana = HayOperadorRelacional();

            if (esBooleana)
            {
                // La asignación contiene una condición
                exp = ParseCONDICION();

                bool resultado = EvaluarCondicionTexto(exp);

                variables[varName].Tipo = "Booleano";
                variables[varName].Valor = resultado;
            }
            else
            {
                // Expresión aritmética
                exp = ParseEXP();

                object valor = EvaluarExpresion(exp);

                variables[varName].Valor = valor;
                variables[varName].Tipo = ObtenerTipoValor(valor);
            }

            // Verificar ;
            if (!EsEOF() &&
                TipoActual() == "CE13" &&
                TokenActual().Linea == lineaAsignacion)
            {
                NextToken();
            }
            else
            {
                ErrorEnLinea(
                    $"Se esperaba ';' después de la expresión en asignación de {varName}",
                    lineaAsignacion);
            }

            EscribirSintaxis(
                $"asignación {varName} = " +
                (!string.IsNullOrEmpty(exp)
                    ? exp
                    : "<expresión>"));
        }

        // Determina si una asignación contiene un operador relacional
        private bool HayOperadorRelacional()
        {
            int posicion = puntero;
            int parentesis = 0;

            while (posicion < tokens.Count)
            {
                string tipo = tokens[posicion].Tipo;

                if (tipo == "CE13")
                    break;

                if (tipo == "CE7")
                    parentesis++;

                if (tipo == "CE8")
                    parentesis--;

                if (tipo == "OR1" ||
                    tipo == "OR2" ||
                    tipo == "OR3" ||
                    tipo == "OR4" ||
                    tipo == "OR5" ||
                    tipo == "OR6")
                {
                    return true;
                }

                posicion++;
            }

            return false;
        }

        // Evalúa una expresión aritmética sencilla
        private object EvaluarExpresion(string expresion)
        {
            if (string.IsNullOrWhiteSpace(expresion))
                return null;

            expresion = expresion.Trim();

            // Cadena
            if (expresion.StartsWith("'") &&
                expresion.EndsWith("'"))
            {
                return expresion.Substring(
                    1,
                    expresion.Length - 2);
            }

            // Variable
            if (variables.ContainsKey(expresion))
            {
                return variables[expresion].Valor;
            }

            // Entero
            if (int.TryParse(expresion, out int entero))
            {
                return entero;
            }

            // Real
            if (double.TryParse(
                expresion,
                System.Globalization.NumberStyles.Any,
                System.Globalization.CultureInfo.InvariantCulture,
                out double real))
            {
                return real;
            }

            // Si es una operación aritmética sencilla
            try
            {
                return EvaluarOperacionAritmetica(expresion);
            }
            catch
            {
                return expresion;
            }
        }

        // Evalúa operaciones + - * /
        private object EvaluarOperacionAritmetica(string expresion)
        {
            string[] partes = expresion.Split(
                new char[] { '+', '-', '*', '/' },
                StringSplitOptions.RemoveEmptyEntries);

            if (partes.Length <= 1)
                return expresion;

            List<double> valores = new List<double>();

            foreach (string parte in partes)
            {
                string elemento = parte.Trim();

                if (variables.ContainsKey(elemento))
                {
                    object valorVariable =
                        variables[elemento].Valor;

                    if (valorVariable is int)
                        valores.Add(Convert.ToDouble(valorVariable));
                    else if (valorVariable is double)
                        valores.Add((double)valorVariable);
                    else
                        return expresion;
                }
                else if (double.TryParse(
                    elemento,
                    System.Globalization.NumberStyles.Any,
                    System.Globalization.CultureInfo.InvariantCulture,
                    out double numero))
                {
                    valores.Add(numero);
                }
                else
                {
                    return expresion;
                }
            }

            bool tieneReal = false;

            foreach (string parte in partes)
            {
                if (parte.Contains("."))
                {
                    tieneReal = true;
                    break;
                }

                string nombre = parte.Trim();

                if (variables.ContainsKey(nombre) &&
                    variables[nombre].Tipo == "Real")
                {
                    tieneReal = true;
                    break;
                }
            }

            double resultado = valores[0];

            for (int i = 1; i < valores.Count; i++)
            {
                char operador = ObtenerOperador(
                    expresion,
                    i);

                switch (operador)
                {
                    case '+':
                        resultado += valores[i];
                        break;

                    case '-':
                        resultado -= valores[i];
                        break;

                    case '*':
                        resultado *= valores[i];
                        break;

                    case '/':
                        resultado /= valores[i];
                        break;
                }
            }

            if (!tieneReal &&
                resultado % 1 == 0)
            {
                return (int)resultado;
            }

            return resultado;
        }

        // Obtiene operadores de una expresión
        private char ObtenerOperador(
            string expresion,
            int indiceOperador)
        {
            int encontrados = 0;

            foreach (char c in expresion)
            {
                if (c == '+' ||
                    c == '-' ||
                    c == '*' ||
                    c == '/')
                {
                    encontrados++;

                    if (encontrados == indiceOperador)
                        return c;
                }
            }

            return '+';
        }

        // Obtiene el tipo según el valor
        private string ObtenerTipoValor(object valor)
        {
            if (valor == null)
                return "Sin determinar";

            if (valor is bool)
                return "Booleano";

            if (valor is int)
                return "Entero";

            if (valor is double ||
                valor is float ||
                valor is decimal)
                return "Real";

            if (valor is string)
                return "Cadena";

            return "Sin determinar";
        }

        // LEER
        private void ParseLEER()
        {
            if (!Match("PR3"))
                return;

            while (!EsEOF() &&
                   TipoActual() != "CE13")
            {
                NextToken();
            }

            if (TipoActual() == "CE13")
                NextToken();

            EscribirSintaxis("leer <identificador>");
        }

        // MOSTRAR
        private void ParseMOSTRAR()
        {
            if (!Match("PR4"))
                return;

            StringBuilder linea =
                new StringBuilder();

            if (TipoActual() == "CE13")
            {
                NextToken();

                EscribirSintaxis("mostrar <cadena>");
                return;
            }

            if (TipoActual() == "IDENT" ||
                TipoActual() == "CNU" ||
                TipoActual() == "CAD" ||
                TipoActual() == "CE7" ||
                TipoActual() == "PR21")
            {
                linea.Append(ParseARG2());
                ParseL_ARG2_MOSTRAR(linea);
            }
            else
            {
                Error(
                    "Se esperaba argumento para MOSTRAR " +
                    "(IDENT, CNU, CAD o expresión)");

                while (!EsEOF() &&
                       TipoActual() != "CE13")
                {
                    NextToken();
                }
            }

            if (TipoActual() == "CE13")
                NextToken();

            EscribirSintaxis(
                "mostrar " + linea.ToString());
        }

        // ROMPER
        private void ParseROMPER()
        {
            if (!Match("PR11"))
                return;

            if (!Match("CE13"))
                return;

            EscribirSintaxis("romper");
        }

        // LIMP
        private void ParseLIMP()
        {
            if (!Match("PR15"))
                return;

            if (!Match("CE13"))
                return;

            EscribirSintaxis("limpiar");
        }

        // VAR
        private void ParseVAR()
        {
            if (!Match("PR20"))
                return;

            bool encontroPuntoComa = false;

            int lineaDeclaracion =
                TokenActual().Linea;

            while (!EsEOF() &&
                   TipoActual() != "CE13" &&
                   TokenActual().Linea == lineaDeclaracion)
            {
                if (TipoActual() == "IDENT")
                {
                    string nombre =
                        TokenActual().Valor;

                    if (!variables.ContainsKey(nombre))
                    {
                        variables.Add(
                            nombre,
                            new VariableInfo
                            {
                                Nombre = nombre,
                                Tipo = "Sin determinar",
                                Valor = null
                            });
                    }

                    NextToken();
                }
                else if (TipoActual() == "CE16")
                {
                    NextToken();
                }
                else
                {
                    Error(
                        "Se esperaba un identificador " +
                        "en la declaración de variable");

                    NextToken();
                }
            }

            if (!EsEOF() &&
                TipoActual() == "CE13" &&
                TokenActual().Linea == lineaDeclaracion)
            {
                encontroPuntoComa = true;
                NextToken();
            }

            if (!encontroPuntoComa)
            {
                ErrorEnLinea(
                    "Se esperaba ';' después de la declaración de variable",
                    lineaDeclaracion);
            }

            EscribirSintaxis(
                "declaración var <identificador>");
        }

        #endregion

        #region Parseadores Auxiliares

        // ARG → IDENT
        private string ParseARG()
        {
            if (TipoActual() == "IDENT")
            {
                string valor =
                    TokenActual().Valor;

                NextToken();

                return ObtenerRepresentacionSintaxis(
                    "IDENT",
                    valor);
            }

            Error("Se esperaba IDENT");

            return "";
        }

        // ARG2
        private string ParseARG2()
        {
            string tipo = TipoActual();

            if (tipo == "IDENT")
            {
                string valor =
                    TokenActual().Valor;

                NextToken();

                return ObtenerRepresentacionSintaxis(
                    tipo,
                    valor);
            }
            else if (tipo == "CNU" ||
                     tipo == "CAD")
            {
                string valor =
                    TokenActual().Valor;

                NextToken();

                return ObtenerRepresentacionSintaxis(
                    tipo,
                    valor);
            }
            else
            {
                return ParseEXP();
            }
        }

        // ARG3
        private string ParseARG3()
        {
            string tipo = TipoActual();

            if (tipo == "IDENT")
            {
                string valor =
                    TokenActual().Valor;

                NextToken();

                return ObtenerRepresentacionSintaxis(
                    tipo,
                    valor);
            }
            else if (tipo == "CNU")
            {
                string valor =
                    TokenActual().Valor;

                NextToken();

                return ObtenerRepresentacionSintaxis(
                    tipo,
                    valor);
            }
            else
            {
                return ParseEXP();
            }
        }

        // L_ARG2_LEER
        private void ParseL_ARG2_LEER(
            StringBuilder linea)
        {
            while (TipoActual() == "CE16")
            {
                NextToken();

                linea.Append(", ");

                if (TipoActual() == "IDENT")
                {
                    linea.Append(
                        TokenActual().Valor);

                    NextToken();
                }
            }
        }

        // L_ARG2_MOSTRAR
        private void ParseL_ARG2_MOSTRAR(
            StringBuilder linea)
        {
            while (TipoActual() == "CE16")
            {
                NextToken();

                linea.Append(", ");

                if (TipoActual() == "IDENT" ||
                    TipoActual() == "CNU" ||
                    TipoActual() == "CAD")
                {
                    string tipo =
                        TipoActual();

                    string valor =
                        TokenActual().Valor;

                    linea.Append(
                        ObtenerRepresentacionSintaxis(
                            tipo,
                            valor));

                    NextToken();
                }
            }
        }

        // L_VAR
        private void ParseL_VAR(
            StringBuilder linea)
        {
            while (TipoActual() == "CE16")
            {
                NextToken();

                linea.Append(", ");

                if (TipoActual() == "IDENT")
                {
                    linea.Append(
                        TokenActual().Valor);

                    NextToken();
                }
            }
        }

        #endregion

        #region Expresiones Aritméticas

        // EXP → TERM EXP_TAIL
        private string ParseEXP()
        {
            StringBuilder exp =
                new StringBuilder();

            exp.Append(ParseTERM());
            exp.Append(ParseEXP_TAIL());

            return exp.ToString().Trim();
        }

        // EXP_TAIL
        private string ParseEXP_TAIL()
        {
            StringBuilder tail =
                new StringBuilder();

            while (TipoActual() == "OA1" ||
                   TipoActual() == "OA2")
            {
                string tipo =
                    TipoActual();

                NextToken();

                tail.Append(
                    " " +
                    ObtenerRepresentacionSintaxis(tipo) +
                    " ");

                tail.Append(ParseTERM());
            }

            return tail.ToString();
        }

        // TERM
        private string ParseTERM()
        {
            StringBuilder term =
                new StringBuilder();

            term.Append(ParseFACTOR());
            term.Append(ParseTERM_TAIL());

            return term.ToString();
        }

        // TERM_TAIL
        private string ParseTERM_TAIL()
        {
            StringBuilder tail =
                new StringBuilder();

            while (TipoActual() == "OA3" ||
                   TipoActual() == "OA4")
            {
                string tipo =
                    TipoActual();

                NextToken();

                tail.Append(
                    " " +
                    ObtenerRepresentacionSintaxis(tipo) +
                    " ");

                tail.Append(ParseFACTOR());
            }

            return tail.ToString();
        }

        // FACTOR
        private string ParseFACTOR()
        {
            if (TipoActual() == "CE7")
            {
                NextToken();

                string exp =
                    ParseEXP();

                if (!Match("CE8"))
                    return "";

                return "(" + exp + ")";
            }
            else if (TipoActual() == "IDENT")
            {
                string valor =
                    TokenActual().Valor;

                NextToken();

                return ObtenerRepresentacionSintaxis(
                    "IDENT",
                    valor);
            }
            else if (TipoActual() == "CNU")
            {
                string valor =
                    TokenActual().Valor;

                NextToken();

                return ObtenerRepresentacionSintaxis(
                    "CNU",
                    valor);
            }
            else if (TipoActual() == "CAD")
            {
                string valor =
                    TokenActual().Valor;

                NextToken();

                return ObtenerRepresentacionSintaxis(
                    "CAD",
                    valor);
            }
            else if (TipoActual() == "PR21")
            {
                NextToken();

                string className =
                    TokenActual().Valor;

                if (!Match("IDENT"))
                    return "";

                if (!Match("CE7"))
                    return "";

                StringBuilder args =
                    new StringBuilder();

                args.Append(ParseARG2());
                ParseL_ARG2_MOSTRAR(args);

                if (!Match("CE8"))
                    return "";

                return
                    $"nuevo {className}({args})";
            }
            else
            {
                Error(
                    "Se esperaba FACTOR " +
                    "(IDENT, CNU, CAD, o expresion entre parentesis)");

                return "";
            }
        }

        #endregion

        #region Condiciones Lógicas

        private string ParseCONDICION()
        {
            StringBuilder cond =
                new StringBuilder();

            cond.Append(ParseCOND_T());
            cond.Append(ParseCONDICION_TAIL());

            return cond.ToString().Trim();
        }

        private string ParseCONDICION_TAIL()
        {
            StringBuilder tail =
                new StringBuilder();

            while (TipoActual() == "OPL1" ||
                   TipoActual() == "OPL2")
            {
                string tipo =
                    TipoActual();

                NextToken();

                tail.Append(
                    " " +
                    ObtenerRepresentacionSintaxis(tipo) +
                    " ");

                tail.Append(ParseCOND_T());
            }

            return tail.ToString();
        }

        private string ParseCOND_T()
        {
            if (TipoActual() == "OPL3")
            {
                NextToken();

                return "no " + ParseCOND_F();
            }

            return ParseCOND_F();
        }

        private string ParseCOND_F()
        {
            if (TipoActual() == "CE7")
            {
                NextToken();

                string cond =
                    ParseCONDICION();

                if (!Match("CE8"))
                    return "";

                return "(" + cond + ")";
            }

            return ParseCOMP();
        }

        private string ParseCOMP()
        {
            StringBuilder comp =
                new StringBuilder();

            comp.Append(ParseVALOR());

            if (TipoActual() == "OR1" ||
                TipoActual() == "OR2" ||
                TipoActual() == "OR3" ||
                TipoActual() == "OR4" ||
                TipoActual() == "OR5" ||
                TipoActual() == "OR6")
            {
                string tipo =
                    TipoActual();

                NextToken();

                comp.Append(
                    " " +
                    ObtenerRepresentacionSintaxis(tipo) +
                    " ");

                string valorDerecha =
                    ParseVALOR();

                if (string.IsNullOrEmpty(valorDerecha))
                {
                    Error(
                        "Se esperaba un valor " +
                        "(IDENT, CNU o CAD) a la derecha " +
                        "del operador relacional");
                }

                comp.Append(valorDerecha);
            }
            else
            {
                if (TipoActual() == "CE8" ||
                    TipoActual() == "OPL1" ||
                    TipoActual() == "OPL2" ||
                    TipoActual() == "PR6" ||
                    TipoActual() == "CE13" ||
                    EsEOF())
                {
                    Error(
                        "Se esperaba operador relacional " +
                        "(>, >=, <, <=, <>, ==)");
                }
                else if (TipoActual() == "IDENT" ||
                         TipoActual() == "CNU" ||
                         TipoActual() == "CAD")
                {
                    Error(
                        "Se esperaba operador relacional " +
                        "(>, >=, <, <=, <>, ==) entre los valores");

                    comp.Append(" ");
                    comp.Append(ParseVALOR());
                }
                else
                {
                    Error(
                        "Se esperaba operador relacional " +
                        "(>, >=, <, <=, <>, ==)");
                }
            }

            return comp.ToString();
        }

        private string ParseVALOR()
        {
            if (TipoActual() == "IDENT")
            {
                string valor =
                    TokenActual().Valor;

                NextToken();

                return ObtenerRepresentacionSintaxis(
                    "IDENT",
                    valor);
            }
            else if (TipoActual() == "CNU")
            {
                string valor =
                    TokenActual().Valor;

                NextToken();

                return ObtenerRepresentacionSintaxis(
                    "CNU",
                    valor);
            }
            else if (TipoActual() == "CAD")
            {
                string valor =
                    TokenActual().Valor;

                NextToken();

                return ObtenerRepresentacionSintaxis(
                    "CAD",
                    valor);
            }
            else
            {
                Error(
                    "Se esperaba valor " +
                    "(IDENT, CNU o CAD)");

                return "";
            }
        }

        #endregion

        #region Evaluacion de Booleanos

        // Evalúa una condición ya convertida a texto
        private bool EvaluarCondicionTexto(string condicion)
        {
            if (string.IsNullOrWhiteSpace(condicion))
                return false;

            condicion = condicion.Trim();

            // NO
            if (condicion.StartsWith("no "))
            {
                return !EvaluarCondicionTexto(
                    condicion.Substring(3).Trim());
            }

            // Paréntesis exteriores
            if (condicion.StartsWith("(") &&
                condicion.EndsWith(")"))
            {
                string interior =
                    condicion.Substring(
                        1,
                        condicion.Length - 2);

                return EvaluarCondicionTexto(interior);
            }

            // Buscar Y
            int posicionY =
                BuscarOperadorLogico(condicion, " y ");

            if (posicionY >= 0)
            {
                string izquierda =
                    condicion.Substring(0, posicionY);

                string derecha =
                    condicion.Substring(
                        posicionY + 3);

                return
                    EvaluarCondicionTexto(izquierda) &&
                    EvaluarCondicionTexto(derecha);
            }

            // Buscar O
            int posicionO =
                BuscarOperadorLogico(condicion, " o ");

            if (posicionO >= 0)
            {
                string izquierda =
                    condicion.Substring(0, posicionO);

                string derecha =
                    condicion.Substring(
                        posicionO + 3);

                return
                    EvaluarCondicionTexto(izquierda) ||
                    EvaluarCondicionTexto(derecha);
            }

            // Comparación
            return EvaluarComparacion(condicion);
        }

        private int BuscarOperadorLogico(
            string texto,
            string operador)
        {
            int nivel = 0;

            for (int i = 0;
                 i <= texto.Length - operador.Length;
                 i++)
            {
                if (texto[i] == '(')
                    nivel++;

                if (texto[i] == ')')
                    nivel--;

                if (nivel == 0 &&
                    texto.Substring(
                        i,
                        operador.Length) == operador)
                {
                    return i;
                }
            }

            return -1;
        }

        // Evalúa una comparación
        private bool EvaluarComparacion(
            string condicion)
        {
            string[] operadores =
            {
                ">=",
                "<=",
                "<>",
                "==",
                ">",
                "<"
            };

            foreach (string operador in operadores)
            {
                int posicion =
                    condicion.IndexOf(operador);

                if (posicion >= 0)
                {
                    string izquierda =
                        condicion.Substring(
                            0,
                            posicion).Trim();

                    string derecha =
                        condicion.Substring(
                            posicion +
                            operador.Length).Trim();

                    object valorIzquierda =
                        ObtenerValor(izquierda);

                    object valorDerecha =
                        ObtenerValor(derecha);

                    if (valorIzquierda == null ||
                        valorDerecha == null)
                    {
                        return false;
                    }

                    // Comparaciones numéricas
                    if (EsNumero(valorIzquierda) &&
                        EsNumero(valorDerecha))
                    {
                        double izq =
                            Convert.ToDouble(valorIzquierda);

                        double der =
                            Convert.ToDouble(valorDerecha);

                        switch (operador)
                        {
                            case ">":
                                return izq > der;

                            case ">=":
                                return izq >= der;

                            case "<":
                                return izq < der;

                            case "<=":
                                return izq <= der;

                            case "==":
                                return izq == der;

                            case "<>":
                                return izq != der;
                        }
                    }

                    // Comparaciones de texto
                    string textoIzquierda =
                        valorIzquierda.ToString();

                    string textoDerecha =
                        valorDerecha.ToString();

                    switch (operador)
                    {
                        case "==":
                            return textoIzquierda ==
                                   textoDerecha;

                        case "<>":
                            return textoIzquierda !=
                                   textoDerecha;
                    }
                }
            }

            return false;
        }

        // Obtiene el valor real de una variable o literal
        private object ObtenerValor(string texto)
        {
            texto = texto.Trim();

            // Variable
            if (variables.ContainsKey(texto))
            {
                return variables[texto].Valor;
            }

            // Cadena
            if (texto.StartsWith("'") &&
                texto.EndsWith("'"))
            {
                return texto.Substring(
                    1,
                    texto.Length - 2);
            }

            // Entero
            if (int.TryParse(
                texto,
                out int entero))
            {
                return entero;
            }

            // Real
            if (double.TryParse(
                texto,
                System.Globalization.NumberStyles.Any,
                System.Globalization.CultureInfo.InvariantCulture,
                out double real))
            {
                return real;
            }

            return null;
        }

        private bool EsNumero(object valor)
        {
            return valor is int ||
                   valor is double ||
                   valor is float ||
                   valor is decimal;
        }

        #endregion

        #region Instrucciones de Control

        private void ParseSI()
        {
            if (!Match("PR5"))
                return;

            if (TipoActual() != "CE7")
            {
                Error(
                    "Se esperaba '(' después de SI " +
                    "para la condición");

                Sincronizar();
                return;
            }

            if (!Match("CE7"))
                return;

            string condicion =
                ParseCONDICION();

            if (TipoActual() != "CE8")
            {
                Error(
                    "Se esperaba ')' al final " +
                    "de la condición en SI");
            }

            if (!Match("CE8"))
                return;

            if (!Match("PR6"))
                return;

            EscribirSintaxis(
                $"si {condicion} entonces");

            indentacion++;

            try
            {
                ParseL_INSTR();
            }
            catch
            {
            }

            indentacion--;

            ParseSINO_OPC();

            if (TipoActual() == "PR2")
            {
                NextToken();

                if (!EsEOF() &&
                    TipoActual() == "CE13")
                {
                    NextToken();
                }

                EscribirSintaxis("fin si");
            }
        }

        private void ParseSINO_OPC()
        {
            if (TipoActual() == "PR7")
            {
                if (!Match("PR7"))
                    return;

                EscribirSintaxis("sino");

                indentacion++;

                try
                {
                    ParseL_INSTR();
                }
                catch
                {
                }

                indentacion--;
            }
        }

        private void ParseDESDE()
        {
            if (!Match("PR8"))
                return;

            while (!EsEOF() &&
                   TipoActual() != "PR13")
            {
                NextToken();
            }

            if (!Match("PR13"))
                return;

            EscribirSintaxis(
                "desde <parámetros> hacer");

            indentacion++;

            ParseL_INSTR();

            indentacion--;

            if (!Match("PR2"))
                return;

            if (!Match("CE13"))
                return;

            EscribirSintaxis("fin desde");
        }

        private void ParsePARA()
        {
            if (!Match("PR12"))
                return;

            while (!EsEOF() &&
                   TipoActual() != "PR13")
            {
                NextToken();
            }

            if (!Match("PR13"))
                return;

            EscribirSintaxis(
                "para <parametros> hacer");

            indentacion++;

            EscribirSintaxis(
                "<instrucciones>");

            ParseL_INSTR();

            indentacion--;

            if (!Match("PR2"))
                return;

            if (!Match("CE13"))
                return;

            EscribirSintaxis("fin");
        }

        private void ParseMIENTRAS()
        {
            if (!Match("PR14"))
                return;

            while (!EsEOF() &&
                   TipoActual() != "PR13")
            {
                NextToken();
            }

            if (!Match("PR13"))
                return;

            EscribirSintaxis(
                "mientras <condición> hacer");

            indentacion++;

            ParseL_INSTR();

            indentacion--;

            if (!Match("PR2"))
                return;

            if (!Match("CE13"))
                return;

            EscribirSintaxis("fin mientras");
        }

        private void ParseREPETIR()
        {
            if (!Match("PR10"))
                return;

            EscribirSintaxis("repetir");

            indentacion++;

            try
            {
                ParseL_INSTR();
            }
            catch
            {
            }

            indentacion--;

            if (!EsEOF())
            {
                if (TipoActual() == "PR14")
                {
                    Match("PR14");

                    while (!EsEOF() &&
                           TipoActual() != "CE13")
                    {
                        NextToken();
                    }

                    if (!EsEOF() &&
                        TipoActual() == "CE13")
                    {
                        Match("CE13");
                    }

                    EscribirSintaxis(
                        "hasta <condición>");
                }
            }
        }

        #endregion

        #region Instrucciones Restantes

        private void ParseIR()
        {
            if (!Match("PR16"))
                return;

            while (!EsEOF() &&
                   TipoActual() != "CE13")
            {
                NextToken();
            }

            if (!Match("CE13"))
                return;

            EscribirSintaxis(
                "ir <argumentos>");
        }

        private string ParseARG5()
        {
            if (TipoActual() == "IDENT")
            {
                string valor =
                    TokenActual().Valor;

                NextToken();

                return valor;
            }
            else if (TipoActual() == "CNU")
            {
                string valor =
                    TokenActual().Valor;

                NextToken();

                return valor;
            }

            Error("Se esperaba IDENT o CNU");

            return "";
        }

        private void ParseESCRIBIR()
        {
            if (!Match("PR17"))
                return;

            while (!EsEOF() &&
                   TipoActual() != "CE13")
            {
                NextToken();
            }

            if (!Match("CE13"))
                return;

            EscribirSintaxis(
                "escribir <argumentos>");
        }

        private void ParseFUNCION()
        {
            if (!Match("PR18"))
                return;

            string funcName =
                ParseARG();

            while (!EsEOF() &&
                   TipoActual() != "CE9")
            {
                NextToken();
            }

            EscribirSintaxis(
                $"función {funcName} <parámetros>");

            if (TipoActual() == "CE9")
            {
                NextToken();

                indentacion++;

                ParseL_INSTR();

                indentacion--;

                if (!Match("CE10"))
                    return;
            }

            if (!Match("CE13"))
                return;

            EscribirSintaxis(
                "fin función");
        }

        private void ParseL_PARAMS(
            StringBuilder parameters)
        {
            if (TipoActual() == "IDENT")
            {
                parameters.Append(
                    ParseARG());

                ParseL_PARAMS_R(
                    parameters);
            }
        }

        private void ParseL_PARAMS_R(
            StringBuilder parameters)
        {
            while (TipoActual() == "CE16")
            {
                NextToken();

                parameters.Append(", ");

                parameters.Append(
                    ParseARG());
            }
        }

        private void ParseRETORNAR()
        {
            if (!Match("PR19"))
                return;

            while (!EsEOF() &&
                   TipoActual() != "CE13")
            {
                NextToken();
            }

            if (!Match("CE13"))
                return;

            EscribirSintaxis(
                "retornar <valor>");
        }

        private void ParseNUEVO()
        {
            if (!Match("PR21"))
                return;

            string className =
                ParseARG();

            while (!EsEOF() &&
                   TipoActual() != "CE13")
            {
                NextToken();
            }

            if (!Match("CE13"))
                return;

            EscribirSintaxis(
                $"nuevo {className} <argumentos>");
        }

        #endregion
    }
}