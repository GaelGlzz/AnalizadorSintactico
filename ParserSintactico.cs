using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

namespace AnalizadorLexico
{
    public class ParserSintactico
    {
        #region Clases y Estructuras Internas

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

        #endregion

        #region Constructor

        /// <param name="tokensDelLexer">Lista de tokens del analizador lexico</param>
        /// <param name="dgvErrores">DataGridView para mostrar errores sintacticos</param>
        /// <param name="rtSintaxis">RichTextBox para mostrar la sintaxis detectada</param>
        public ParserSintactico(List<TokenSintactico> tokensDelLexer, DataGridView dgvErrores, RichTextBox rtSintaxis)
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
            // Usar la última línea válida para EOF en lugar de 0
            int ultimaLinea = tokens.Count > 0 ? tokens[tokens.Count - 1].Linea : 1;
            return new TokenSintactico { Tipo = "EOF", Valor = "", Linea = ultimaLinea };
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

            // Palabras reservadas
            switch (tipoCodigo)
            {
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

        /// <param name="tipoCodigo">Código del token (ej: "PR5", "CE13")</param>
        /// <param name="valor">Valor del token (para IDENT, CNU, CAD)</param>
        /// <returns>Representación del token para rtSintaxis</returns>
        private string ObtenerRepresentacionSintaxis(string tipoCodigo, string valor = "")
        {
            if (string.IsNullOrEmpty(tipoCodigo))
                return tipoCodigo;

            // Palabras reservadas en minúsculas
            switch (tipoCodigo)
            {
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
                // Tipos de datos - usar el valor si está disponible
                case "IDENT": return !string.IsNullOrEmpty(valor) ? valor : "IDENT";
                case "CNU": return !string.IsNullOrEmpty(valor) ? valor : "CNU";
                case "CAD": return !string.IsNullOrEmpty(valor) ? valor : "CAD";
                case "EOF": return "fin de archivo";
                default: return tipoCodigo;
            }
        }

        // Valida el token y registra error en dgvErroresSintacticos si no coincide
        // tipoEsperado: tipo esperado del token
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

                // Mostrar el valor del token si es diferente del nombre (para identificadores, keywords, etc.)
                string mostrarEncontrado = nombreEncontrado;
                if (!string.IsNullOrEmpty(valorEncontrado) && valorEncontrado != nombreEncontrado)
                {
                    mostrarEncontrado = valorEncontrado;
                }

                string mensaje = $"Se esperaba '{nombreEsperado}', encontrado '{mostrarEncontrado}'";
                Error(mensaje);
                return false;
            }
        }

        // Agrega una fila al DataGridView de errores
        private void Error(string mensaje)
        {
            string errorCompleto = mensaje;
            if (dgvErroresSintacticos != null && dgvErroresSintacticos.InvokeRequired)
            {
                dgvErroresSintacticos.Invoke(new Action(() =>
                {
                    dgvErroresSintacticos.Rows.Add(TokenActual().Linea, errorCompleto);
                }));
            }
            else if (dgvErroresSintacticos != null)
            {
                dgvErroresSintacticos.Rows.Add(TokenActual().Linea, errorCompleto);
            }
        }

        // Sincroniza el parser avanzando tokens hasta encontrar CE13 (;) o PR2 (FIN)
        // Limita el consumo a 10 tokens para evitar consumir toda la entrada
        private void Sincronizar()
        {
            int tokensConsumidos = 0;
            while (!EsEOF() && TipoActual() != "CE13" && TipoActual() != "PR2")
            {
                NextToken();
                tokensConsumidos++;
                if (tokensConsumidos >= 10)
                {
                    break;
                }
            }

            // NO consumir el token de sincronización para permitir continuar
        }

        // Escribe en rtSintaxis con indentacion y número de línea
        private void EscribirSintaxis(string texto)
        {
            // Usar contador interno para números de línea que empiece por 1
            lineaSintaxis++;

            // Formatear número de línea con ancho fijo de 3 caracteres, alineado a la derecha
            string numeroLinea = lineaSintaxis.ToString().PadLeft(3);
            string textoConLinea = $"{numeroLinea}  {texto}";

            if (rtSintaxis != null && rtSintaxis.InvokeRequired)
            {
                rtSintaxis.Invoke(new Action(() =>
                {
                    rtSintaxis.AppendText(new string(' ', indentacion * 2) + textoConLinea + "\n");
                }));
            }
            else if (rtSintaxis != null)
            {
                rtSintaxis.AppendText(new string(' ', indentacion * 2) + textoConLinea + "\n");
            }
        }

        #endregion

        #region Metodo Publico Parse

        // Metodo publico que inicia el analisis sintactico
        // Limpia los controles, llama a ParseS() y reinicia contadores
        public void Parse()
        {
            // Limpiar rtSintaxis
            if (rtSintaxis != null)
            {
                rtSintaxis.Clear();
                {
                    rtSintaxis.Clear();
                }
            }

            // Reiniciar puntero y contador de líneas
            puntero = 0;
            indentacion = 0;
            lineaSintaxis = 0;

            // Iniciar parseo
            ParseS();

            // No verificar tokens restantes para evitar errores falsos
            // if (!EsEOF())
            // {
            //     Error("Se esperada fin del programa, pero quedan tokens por procesar");
            // }
        }

        #endregion

        #region Producciones Principales - Gramática Oficial KaViGex

        // S → PR1 L_INSTR PR2 CE13
        // Programa: INICIO lista_instrucciones FIN ;
        private void ParseS()
        {
            if (!Match("PR1")) return; // INICIO

            EscribirSintaxis("inicio");

            ParseL_INSTR();

            if (!Match("PR2")) return; // FIN
            if (!Match("CE13")) return; // ;

            EscribirSintaxis("fin");
        }

        // L_INSTR → INSTR L_INSTR | ε
        // Lista de instrucciones
        private void ParseL_INSTR()
        {
            while (!EsEOF() && TipoActual() != "EOF")
            {
                // Detenerse si encontramos el FIN del programa (no de bloques internos)
                // El FIN del programa es el que está al mismo nivel de indentación que INICIO
                if (TipoActual() == "PR2" && indentacion == 0)
                    break;

                // Si encontramos un ; al nivel actual, saltarlo y continuar
                if (TipoActual() == "CE13" && indentacion == 0)
                {
                    NextToken();
                    continue;
                }

                // Si estamos en un token que no puede iniciar una instrucción, saltarlo
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
                    // Si la excepción es por FIN en bloque interno, salir del bucle para que el parser del bloque lo maneje
                    if (TipoActual() == "PR2" && indentacion > 0)
                    {
                        break;
                    }
                    Sincronizar();
                    // Después de sincronizar, consumir el ; si está presente
                    if (TipoActual() == "CE13")
                    {
                        NextToken();
                    }
                    // Si después de sincronizar estamos en EOF o FIN, salir del bucle
                    if (EsEOF() || (TipoActual() == "PR2" && indentacion == 0))
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
                   tipo == "PR2" ||  // FIN (para que no sea saltado en bloques internos)
                   tipo == "PR3" ||  // LEER
                   tipo == "PR4" ||  // MOSTRAR
                   tipo == "PR5" ||  // SI
                   tipo == "PR8" ||  // DESDE
                   tipo == "PR10" || // REPETIR
                   tipo == "PR11" || // ROMPER
                   tipo == "PR12" || // PARA
                   tipo == "PR14" || // MIENTRAS
                   tipo == "PR15" || // LIMP
                   tipo == "PR16" || // IR
                   tipo == "PR17" || // ESCRIBIR
                   tipo == "PR18" || // FUNCION
                   tipo == "PR19" || // RETORNAR
                   tipo == "PR20" || // VAR
                   tipo == "PR21";   // NUEVO
        }

        // INSTR → ASIG | LEER | MOSTRAR | SI | DESDE | PARA | MIENTRAS
        //       | REPETIR | ROMPER | LIMP | IR | ESCRIBIR | FUNCION
        //       | RETORNAR | VAR | NUEVO
        // Despachador de instrucciones
        private void ParseINSTR()
        {
            if (EsEOF())
                return;

            // Solo retornar si es FIN al nivel del programa (indentacion == 0)
            // Los FIN de bloques internos son manejados por sus respectivos parsers
            if (TipoActual() == "PR2" && indentacion == 0)
                return;

            // Si es FIN en un bloque interno, lanzar excepción para que ParseL_INSTR salga del bucle
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
                    case "PR6":
                        // ENTONCES - no es una instruccion independiente
                        Error($"Se esperaba instruccion, se encontro ENTONCES");
                        Sincronizar();
                        break;
                    case "PR7":
                        // SINO - no es una instruccion independiente
                        Error($"Se esperaba instruccion, se encontro SINO");
                        Sincronizar();
                        break;
                    case "PR8":
                        ParseDESDE();
                        break;
                    case "PR9":
                        // HASTA - no es una instruccion independiente
                        Error($"Se esperaba instruccion, se encontro HASTA");
                        Sincronizar();
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
                    case "PR13":
                        // HACER - no es una instruccion independiente
                        Error($"Se esperaba instruccion, se encontro HACER");
                        Sincronizar();
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
                        string nombreTipo = ObtenerNombreToken(tipo);
                        Error($"Se esperaba instruccion, se encontro {nombreTipo}");
                        Sincronizar();
                        break;
                }
            }
            catch
            {
                // Si hay un error al parsear la instrucción, sincronizar y continuar
                Sincronizar();
                if (TipoActual() == "CE13")
                {
                    NextToken();
                }
            }
        }

        #endregion

        #region Instrucciones Simples - Gramática Oficial KaViGex

        // ASIG → IDENT ASI EXP CE13
        // Asignación: variable = expresión ;
        private void ParseASIG()
        {
            string varName = TokenActual().Valor;

            if (!Match("IDENT")) return;

            if (!Match("ASI")) return; // =

            // Consumir la expresión completa hasta ;
            bool encontroPuntoComa = false;
            while (!EsEOF() && TipoActual() != "CE13")
            {
                NextToken();
            }

            // Validar que haya ; al final
            if (!EsEOF() && TipoActual() == "CE13")
            {
                encontroPuntoComa = true;
                NextToken();
            }

            if (!encontroPuntoComa)
            {
                Error($"Se esperaba ';' después de la expresión en asignación de {varName}");
            }

            EscribirSintaxis($"asignación {varName} = <expresión>");
        }

        // LEER → PR3 ARG L_ARG2_LEER CE13
        // L_ARG2_LEER → CE16 ARG L_ARG2_LEER | ε
        // ARG → IDENT
        // Lectura de variables
        private void ParseLEER()
        {
            if (!Match("PR3")) return; // LEER

            // Consumir argumentos hasta ;
            while (!EsEOF() && TipoActual() != "CE13")
            {
                NextToken();
            }

            // Intentar consumir el ; si está presente
            if (TipoActual() == "CE13")
            {
                NextToken();
            }

            EscribirSintaxis("leer <identificador>");
        }

        // MOSTRAR → PR4 ARG2 L_ARG2_MOSTRAR CE13
        // L_ARG2_MOSTRAR → CE16 ARG2 L_ARG2_MOSTRAR | ε
        // ARG2 → IDENT | CNU | CAD | EXP
        // Mostrar argumentos
        private void ParseMOSTRAR()
        {
            if (!Match("PR4")) return; // MOSTRAR

            // Consumir argumentos hasta ;
            while (!EsEOF() && TipoActual() != "CE13")
            {
                NextToken();
            }

            // Intentar consumir el ; si está presente
            if (TipoActual() == "CE13")
            {
                NextToken();
            }

            EscribirSintaxis("mostrar <cadena>");
        }

        // ROMPER → PR11 CE13
        // Romper ciclo
        private void ParseROMPER()
        {
            if (!Match("PR11")) return; // ROMPER
            if (!Match("CE13")) return; // ;

            EscribirSintaxis("romper");
        }

        // LIMP → PR15 CE13
        // Limpiar pantalla
        private void ParseLIMP()
        {
            if (!Match("PR15")) return; // LIMP
            if (!Match("CE13")) return; // ;

            EscribirSintaxis("limpiar");
        }

        // VAR → PR20 IDENT L_VAR CE13
        // L_VAR → CE16 IDENT L_VAR | ε
        // Declaración de variables
        private void ParseVAR()
        {
            if (!Match("PR20")) return; // VAR

            // Consumir identificadores hasta ;
            bool encontroPuntoComa = false;
            while (!EsEOF() && TipoActual() != "CE13")
            {
                NextToken();
            }

            // Validar que haya ; al final
            if (!EsEOF() && TipoActual() == "CE13")
            {
                encontroPuntoComa = true;
                NextToken();
            }

            if (!encontroPuntoComa)
            {
                Error("Se esperaba ';' después de la declaración de variable");
            }

            EscribirSintaxis("declaración var <identificador>");
        }

        #endregion

        #region Parseadores Auxiliares - Gramática Oficial KaViGex

        // ARG → IDENT
        private string ParseARG()
        {
            if (TipoActual() == "IDENT")
            {
                string valor = TokenActual().Valor;
                NextToken();
                return ObtenerRepresentacionSintaxis("IDENT", valor);
            }
            Error("Se esperaba IDENT");
            return "";
        }

        // ARG2 → IDENT | CNU | CAD | EXP
        private string ParseARG2()
        {
            string tipo = TipoActual();

            if (tipo == "IDENT")
            {
                string valor = TokenActual().Valor;
                NextToken();
                return ObtenerRepresentacionSintaxis(tipo, valor);
            }
            else if (tipo == "CNU" || tipo == "CAD")
            {
                string valor = TokenActual().Valor;
                NextToken();
                return ObtenerRepresentacionSintaxis(tipo, valor);
            }
            else
            {
                return ParseEXP();
            }
        }

        // ARG3 → IDENT | CNU | EXP
        private string ParseARG3()
        {
            string tipo = TipoActual();

            if (tipo == "IDENT")
            {
                string valor = TokenActual().Valor;
                NextToken();
                return ObtenerRepresentacionSintaxis(tipo, valor);
            }
            else if (tipo == "CNU")
            {
                string valor = TokenActual().Valor;
                NextToken();
                return ObtenerRepresentacionSintaxis(tipo, valor);
            }
            else
            {
                return ParseEXP();
            }
        }

        // L_ARG2_LEER → CE16 ARG L_ARG2_LEER | ε
        private void ParseL_ARG2_LEER(StringBuilder linea)
        {
            while (TipoActual() == "CE16") // ,
            {
                NextToken();
                linea.Append(", ");
                if (TipoActual() == "IDENT")
                {
                    linea.Append(TokenActual().Valor);
                    NextToken();
                }
            }
        }

        // L_ARG2_MOSTRAR → CE16 ARG2 L_ARG2_MOSTRAR | ε
        private void ParseL_ARG2_MOSTRAR(StringBuilder linea)
        {
            while (TipoActual() == "CE16") // ,
            {
                NextToken();
                linea.Append(", ");
                if (TipoActual() == "IDENT" || TipoActual() == "CNU" || TipoActual() == "CAD")
                {
                    string tipo = TipoActual();
                    string valor = TokenActual().Valor;
                    linea.Append(ObtenerRepresentacionSintaxis(tipo, valor));
                    NextToken();
                }
            }
        }

        // L_VAR → CE16 IDENT L_VAR | ε
        private void ParseL_VAR(StringBuilder linea)
        {
            while (TipoActual() == "CE16") // ,
            {
                NextToken();
                linea.Append(", ");
                if (TipoActual() == "IDENT")
                {
                    linea.Append(TokenActual().Valor);
                    NextToken();
                }
            }
        }

        #endregion

        #region Expresiones Aritméticas - Gramática Oficial KaViGex

        // EXP → TERM EXP_TAIL
        private string ParseEXP()
        {
            StringBuilder exp = new StringBuilder();

            exp.Append(ParseTERM());
            exp.Append(ParseEXP_TAIL());

            return exp.ToString().Trim();
        }

        // EXP_TAIL → OA1 TERM EXP_TAIL | OA2 TERM EXP_TAIL | ε
        private string ParseEXP_TAIL()
        {
            StringBuilder tail = new StringBuilder();

            while (TipoActual() == "OA1" || TipoActual() == "OA2") // + o -
            {
                string tipo = TipoActual();
                NextToken();
                tail.Append(" " + ObtenerRepresentacionSintaxis(tipo) + " ");
                tail.Append(ParseTERM());
            }

            return tail.ToString();
        }

        // TERM → FACTOR TERM_TAIL
        private string ParseTERM()
        {
            StringBuilder term = new StringBuilder();

            term.Append(ParseFACTOR());
            term.Append(ParseTERM_TAIL());

            return term.ToString();
        }

        // TERM_TAIL → OA3 FACTOR TERM_TAIL | OA4 FACTOR TERM_TAIL | ε
        private string ParseTERM_TAIL()
        {
            StringBuilder tail = new StringBuilder();

            while (TipoActual() == "OA3" || TipoActual() == "OA4") // * o /
            {
                string tipo = TipoActual();
                NextToken();
                tail.Append(" " + ObtenerRepresentacionSintaxis(tipo) + " ");
                tail.Append(ParseFACTOR());
            }

            return tail.ToString();
        }

        // FACTOR → CE7 EXP CE8 | IDENT | CNU | CAD | PR21 IDENT CE7 L_ARG2_MOSTRAR CE8
        private string ParseFACTOR()
        {
            if (TipoActual() == "CE7") // (
            {
                NextToken();
                string exp = ParseEXP();
                if (!Match("CE8")) return ""; // )
                return "(" + exp + ")";
            }
            else if (TipoActual() == "IDENT")
            {
                string valor = TokenActual().Valor;
                NextToken();
                return ObtenerRepresentacionSintaxis("IDENT", valor);
            }
            else if (TipoActual() == "CNU")
            {
                string valor = TokenActual().Valor;
                NextToken();
                return ObtenerRepresentacionSintaxis("CNU", valor);
            }
            else if (TipoActual() == "CAD")
            {
                string valor = TokenActual().Valor;
                NextToken();
                return ObtenerRepresentacionSintaxis("CAD", valor);
            }
            else if (TipoActual() == "PR21") // NUEVO
            {
                NextToken();
                string className = TokenActual().Valor;
                if (!Match("IDENT")) return "";
                if (!Match("CE7")) return ""; // (

                StringBuilder args = new StringBuilder();
                args.Append(ParseARG2());
                ParseL_ARG2_MOSTRAR(args);

                if (!Match("CE8")) return ""; // )

                return $"nuevo {className}({args})";
            }
            else
            {
                Error("Se esperaba FACTOR (IDENT, CNU, CAD, o expresion entre parentesis)");
                return "";
            }
        }

        #region Condiciones Lógicas - Gramática Oficial KaViGex

        private string ParseCONDICION()
        {
            StringBuilder cond = new StringBuilder();

            cond.Append(ParseCOND_T());
            cond.Append(ParseCONDICION_TAIL());

            return cond.ToString().Trim();
        }

        private string ParseCONDICION_TAIL()
        {
            StringBuilder tail = new StringBuilder();

            while (TipoActual() == "OPL1" || TipoActual() == "OPL2") // Y/O
            {
                string tipo = TipoActual();
                NextToken();
                tail.Append(" " + ObtenerRepresentacionSintaxis(tipo) + " ");
                tail.Append(ParseCOND_T());
            }

            return tail.ToString();
        }

        private string ParseCOND_T()
        {
            if (TipoActual() == "OPL3") // NO
            {
                NextToken();
                return "no " + ParseCOND_F();
            }
            else
            {
                return ParseCOND_F();
            }
        }

        private string ParseCOND_F()
        {
            if (TipoActual() == "CE7") // (
            {
                NextToken();
                string cond = ParseCONDICION();
                if (!Match("CE8")) return ""; // )
                return "(" + cond + ")";
            }
            else
            {
                return ParseCOMP();
            }
        }

        private string ParseCOMP()
        {
            StringBuilder comp = new StringBuilder();

            comp.Append(ParseVALOR());

            if (TipoActual() == "OR1" || TipoActual() == "OR2" || TipoActual() == "OR3"
                || TipoActual() == "OR4" || TipoActual() == "OR5" || TipoActual() == "OR6")
            {
                string tipo = TipoActual();
                NextToken();
                comp.Append(" " + ObtenerRepresentacionSintaxis(tipo) + " ");
                comp.Append(ParseVALOR());
            }
            else
            {
                Error("Se esperaba operador relacional (>, >=, <, <=, <>, ==)");
            }

            return comp.ToString();
        }

        private string ParseVALOR()
        {
            if (TipoActual() == "IDENT")
            {
                string valor = TokenActual().Valor;
                NextToken();
                return ObtenerRepresentacionSintaxis("IDENT", valor);
            }
            else if (TipoActual() == "CNU")
            {
                string valor = TokenActual().Valor;
                NextToken();
                return ObtenerRepresentacionSintaxis("CNU", valor);
            }
            else if (TipoActual() == "CAD")
            {
                string valor = TokenActual().Valor;
                NextToken();
                return ObtenerRepresentacionSintaxis("CAD", valor);
            }
            else
            {
                Error("Se esperaba valor (IDENT, CNU o CAD)");
                return "";
            }
        }

        #endregion

        #region Instrucciones de Control - Gramática Oficial KaViGex

        private void ParseSI()
        {
            if (!Match("PR5")) return; // SI

            // Validar que la condición tenga paréntesis de apertura
            if (TipoActual() != "CE7") // (
            {
                Error("Se esperaba '(' después de SI para la condición");
            }

            // Consumir condición hasta ENTONCES, verificando paréntesis de cierre
            bool encontroParentesisCierre = false;
            while (!EsEOF() && TipoActual() != "PR6")
            {
                if (TipoActual() == "CE8") // )
                {
                    encontroParentesisCierre = true;
                }
                NextToken();
            }

            // Validar que se haya encontrado paréntesis de cierre
            if (!encontroParentesisCierre)
            {
                Error("Se esperaba ')' al final de la condición en SI");
            }

            EscribirSintaxis("si <condición> entonces");

            if (!Match("PR6")) return; // ENTONCES

            indentacion++;
            try
            {
                ParseL_INSTR();
            }
            catch
            {
                // Continuar incluso si hay errores en las instrucciones internas
            }
            indentacion--;

            // SINO_OPC
            if (TipoActual() == "PR7") // SINO
            {
                EscribirSintaxis("sino");
                if (!Match("PR7")) return;

                indentacion++;
                try
                {
                    ParseL_INSTR();
                }
                catch
                {
                    // Continuar incluso si hay errores en las instrucciones internas
                }
                indentacion--;
            }

            // Intentar match de FIN (consumir si está presente)
            if (TipoActual() == "PR2")
            {
                NextToken(); // Consumir FIN
                // Intentar consumir el ; si está presente
                if (!EsEOF() && TipoActual() == "CE13")
                {
                    NextToken(); // Consumir ;
                }
                EscribirSintaxis("fin si");
            }
        }

        private void ParseSINO_OPC()
        {
            if (TipoActual() == "PR7") // SINO
            {
                NextToken();
                ParseL_INSTR();
            }
        }

        private void ParseDESDE()
        {
            if (!Match("PR8")) return; // DESDE

            // Consumir hasta HACER
            while (!EsEOF() && TipoActual() != "PR13")
            {
                NextToken();
            }

            if (!Match("PR13")) return; // HACER

            EscribirSintaxis("desde <parámetros> hacer");

            indentacion++;
            ParseL_INSTR();
            indentacion--;

            if (!Match("PR2")) return; // FIN
            if (!Match("CE13")) return; // ;

            EscribirSintaxis("fin desde");
        }

        private void ParsePARA()
        {
            if (!Match("PR12")) return; // PARA

            // Consumir hasta HACER
            while (!EsEOF() && TipoActual() != "PR13")
            {
                NextToken();
            }

            if (!Match("PR13")) return; // HACER

            EscribirSintaxis("para <parametros> hacer");

            indentacion++;
            EscribirSintaxis("<instrucciones>");
            ParseL_INSTR();
            indentacion--;

            if (!Match("PR2")) return; // FIN
            if (!Match("CE13")) return; // ;

            EscribirSintaxis("fin");
        }

        private void ParseMIENTRAS()
        {
            if (!Match("PR14")) return; // MIENTRAS

            // Consumir condición hasta HACER
            while (!EsEOF() && TipoActual() != "PR13")
            {
                NextToken();
            }

            if (!Match("PR13")) return; // HACER

            EscribirSintaxis("mientras <condición> hacer");

            indentacion++;
            ParseL_INSTR();
            indentacion--;

            if (!Match("PR2")) return; // FIN
            if (!Match("CE13")) return; // ;

            EscribirSintaxis("fin mientras");
        }

        private void ParseREPETIR()
        {
            if (!Match("PR10")) return; // REPETIR

            EscribirSintaxis("repetir");

            indentacion++;
            try
            {
                ParseL_INSTR();
            }
            catch
            {
                // Continuar incluso si hay errores en las instrucciones internas
            }
            indentacion--;

            // Intentar match de MIENTRAS solo si no estamos en EOF
            if (!EsEOF())
            {
                if (TipoActual() == "PR14")
                {
                    Match("PR14"); // MIENTRAS

                    // Consumir condición hasta ;
                    while (!EsEOF() && TipoActual() != "CE13")
                    {
                        NextToken();
                    }

                    if (!EsEOF() && TipoActual() == "CE13")
                    {
                        Match("CE13"); // ;
                    }

                    EscribirSintaxis("hasta <condición>");
                }
            }
        }

        #endregion

        #region Instrucciones Restantes - Gramática Oficial KaViGex

        private void ParseIR()
        {
            if (!Match("PR16")) return; // IR

            // Consumir argumentos hasta ;
            while (!EsEOF() && TipoActual() != "CE13")
            {
                NextToken();
            }

            if (!Match("CE13")) return; // ;

            EscribirSintaxis("ir <argumentos>");
        }

        private string ParseARG5()
        {
            if (TipoActual() == "IDENT")
            {
                string valor = TokenActual().Valor;
                NextToken();
                return valor;
            }
            else if (TipoActual() == "CNU")
            {
                string valor = TokenActual().Valor;
                NextToken();
                return valor;
            }
            else
            {
                Error("Se esperaba IDENT o CNU");
                return "";
            }
        }

        private void ParseESCRIBIR()
        {
            if (!Match("PR17")) return; // ESCRIBIR

            // Consumir argumentos hasta ;
            while (!EsEOF() && TipoActual() != "CE13")
            {
                NextToken();
            }

            if (!Match("CE13")) return; // ;

            EscribirSintaxis("escribir <argumentos>");
        }

        private void ParseFUNCION()
        {
            if (!Match("PR18")) return; // FUNCION

            string funcName = ParseARG();

            // Consumir parámetros hasta {
            while (!EsEOF() && TipoActual() != "CE9")
            {
                NextToken();
            }

            EscribirSintaxis($"función {funcName} <parámetros>");

            // ARG7 - cuerpo de la funcion
            if (TipoActual() == "CE9") // {
            {
                NextToken();
                indentacion++;
                ParseL_INSTR();
                indentacion--;
                if (!Match("CE10")) return; // }
            }

            if (!Match("CE13")) return; // ;

            EscribirSintaxis("fin función");
        }

        private void ParseL_PARAMS(StringBuilder parameters)
        {
            if (TipoActual() == "IDENT")
            {
                parameters.Append(ParseARG());
                ParseL_PARAMS_R(parameters);
            }
        }

        private void ParseL_PARAMS_R(StringBuilder parameters)
        {
            while (TipoActual() == "CE16") // ,
            {
                NextToken();
                parameters.Append(", ");
                parameters.Append(ParseARG());
            }
        }

        private void ParseRETORNAR()
        {
            if (!Match("PR19")) return; // RETORNAR

            // Consumir argumento opcional hasta ;
            while (!EsEOF() && TipoActual() != "CE13")
            {
                NextToken();
            }

            if (!Match("CE13")) return; // ;

            EscribirSintaxis("retornar <valor>");
        }

        private void ParseNUEVO()
        {
            if (!Match("PR21")) return; // NUEVO

            string className = ParseARG();

            // Consumir argumentos hasta ;
            while (!EsEOF() && TipoActual() != "CE13")
            {
                NextToken();
            }

            if (!Match("CE13")) return; // ;

            EscribirSintaxis($"nuevo {className} <argumentos>");
        }

        #endregion
    }
}
#endregion