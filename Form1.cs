using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Globalization;
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

            label4.Click += label4_Click;

            label5.Click += label5_Click;
            label5.MouseEnter += label5_MouseEnter;
            label5.MouseLeave += label5_MouseLeave;

            rtbFuente.KeyDown += RtbFuente_KeyDown;

            rtbFuente.ReadOnly = false;

            if (!lexerDB.IsMatrizLoaded())
            {
                string msg = "No se pudo cargar la matriz de transición desde la base de datos.";
                var initErr = lexerDB.GetInitError();

                if (!string.IsNullOrEmpty(initErr))
                    msg += "\nDetalle: " + initErr;

                MessageBox.Show(
                    msg,
                    "Error BD",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);

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

                default:
                    return categoria;
            }
        }

        // ============================================================
        // INFERENCIA DE TIPOS
        // ============================================================

        private string InferirTipoDeAsignacion(
            List<Token> tokensExpr,
            List<Simbolo> tablaSimbolos,
            int numeroLinea)
        {
            if (tokensExpr == null || tokensExpr.Count == 0)
            {
                dgvErrores.Rows.Add(
                    numeroLinea,
                    "Expresión vacía o incompleta",
                    "Semántico - Error en expresión");

                dgvErrores.Rows[dgvErrores.Rows.Count - 1]
                    .DefaultCellStyle.ForeColor = Color.DarkOrange;

                return "Error";
            }

            bool tieneReal = false;
            bool tieneEntero = false;

            var tiposIdentificadores =
                new HashSet<string>(
                    StringComparer.OrdinalIgnoreCase);

            foreach (var t in tokensExpr)
            {
                if (t == null)
                    continue;

                if (t.Categoria == "CAD")
                    return "Cadena";

                if (t.Categoria == "CN REALES" ||
                    t.Categoria == "CN CON EXPONENTE")
                {
                    tieneReal = true;
                }
                else if (t.Categoria == "CN ENTEROS")
                {
                    tieneEntero = true;
                }
                else if (t.Categoria == "IDENT")
                {
                    var sym = tablaSimbolos.FirstOrDefault(
                        s => s.Nombre.Equals(
                            t.Lexema,
                            StringComparison.OrdinalIgnoreCase));

                    if (sym == null ||
                        string.IsNullOrEmpty(sym.Tipo) ||
                        sym.Tipo == "Sin determinar")
                    {
                        dgvErrores.Rows.Add(
                            numeroLinea,
                            $"Tipo desconocido para el identificador '{t.Lexema}'",
                            "Semántico - Tipo desconocido");

                        dgvErrores.Rows[dgvErrores.Rows.Count - 1]
                            .DefaultCellStyle.ForeColor = Color.DarkOrange;

                        return "Error";
                    }

                    tiposIdentificadores.Add(sym.Tipo);
                }
            }

            if (tiposIdentificadores.Count > 0)
            {
                if (tiposIdentificadores.Count == 1)
                {
                    var tipoId = tiposIdentificadores.First();

                    if (tieneReal)
                        return "Real";

                    if (tieneEntero)
                        return tipoId.IndexOf(
                            "Real",
                            StringComparison.OrdinalIgnoreCase) >= 0
                            ? "Real"
                            : tipoId;

                    return tipoId;
                }

                return "Error";
            }

            if (tieneReal)
                return "Real";

            if (tieneEntero)
                return "Entero";

            return "Error";
        }

        private string InferirTipoDato(string categoria)
        {
            switch (categoria)
            {
                case "CN ENTEROS":
                    return "Entero";

                case "CN REALES":
                    return "Real";

                case "CN CON EXPONENTE":
                    return "Real";

                case "CAD":
                    return "Cadena";

                case "IDENT":
                    return "Referencia";

                default:
                    return "Desconocido";
            }
        }

        private string TipoDeOperando(
            Token token,
            List<Simbolo> tablaSimbolos)
        {
            switch (token.Categoria)
            {
                case "CN ENTEROS":
                    return "Entero";

                case "CN REALES":
                    return "Real";

                case "CN CON EXPONENTE":
                    return "Real";

                case "CAD":
                    return "Cadena";

                case "IDENT":
                    var s = tablaSimbolos.FirstOrDefault(
                        x => x.Nombre.Equals(
                            token.Lexema,
                            StringComparison.OrdinalIgnoreCase));

                    return (
                        s != null &&
                        !string.IsNullOrEmpty(s.Tipo) &&
                        s.Tipo != "Sin determinar")
                        ? s.Tipo
                        : "Desconocido";

                default:
                    return "Desconocido";
            }
        }

        // ============================================================
        // COMBINACIÓN DE TIPOS
        // ============================================================

        private string CombinarTipos(
            string tipoIzq,
            string operador,
            string tipoDer,
            out bool esError)
        {
            esError = false;

            if (tipoIzq == "Desconocido" ||
                tipoDer == "Desconocido")
            {
                return "Desconocido";
            }

            bool esNumIzq =
                tipoIzq == "Entero" ||
                tipoIzq == "Real";

            bool esNumDer =
                tipoDer == "Entero" ||
                tipoDer == "Real";

            if (operador == "+" ||
                operador == "-" ||
                operador == "*" ||
                operador == "/")
            {
                if (esNumIzq && esNumDer)
                {
                    return (
                        tipoIzq == "Real" ||
                        tipoDer == "Real")
                        ? "Real"
                        : "Entero";
                }

                if (operador == "+" &&
                    (tipoIzq == "Cadena" || tipoDer == "Cadena"))
                {
                    return "Cadena";
                }

                esError = true;
                return "Error";
            }

            if (operador == ">" ||
                operador == ">=" ||
                operador == "<" ||
                operador == "<=" ||
                operador == "<>" ||
                operador == "==")
            {
                bool comparablesNum =
                    esNumIzq && esNumDer;

                bool comparablesCad =
                    tipoIzq == "Cadena" &&
                    tipoDer == "Cadena";

                if (comparablesNum || comparablesCad)
                    return "Booleano";

                esError = true;
                return "Error";
            }

            string opLower = operador.ToLower();

            if (opLower == "y" ||
                opLower == "o" ||
                opLower == "and" ||
                opLower == "or")
            {
                if (tipoIzq == "Booleano" &&
                    tipoDer == "Booleano")
                {
                    return "Booleano";
                }

                esError = true;
                return "Error";
            }

            esError = true;
            return "Error";
        }

        // ============================================================
        // CONDICIONES
        // ============================================================

        private string InferirTipoCondicion(
            List<Token> tokensExpr,
            List<Simbolo> tablaSimbolos,
            int numeroLinea)
        {
            if (tokensExpr.Count == 0)
                return "Desconocido";

            int idx = 0;

            string resultado = ParseOperandoCondicion(
                tokensExpr,
                ref idx,
                tablaSimbolos,
                numeroLinea);

            while (idx < tokensExpr.Count)
            {
                Token opToken = tokensExpr[idx];

                if (opToken.Categoria != "Y" &&
                    opToken.Categoria != "O")
                {
                    break;
                }

                idx++;

                string siguiente = ParseOperandoCondicion(
                    tokensExpr,
                    ref idx,
                    tablaSimbolos,
                    numeroLinea);

                bool esError;

                resultado = CombinarTipos(
                    resultado,
                    opToken.Lexema.ToLower(),
                    siguiente,
                    out esError);

                if (esError)
                {
                    string mensaje =
                        $"Operador lógico '{opToken.Lexema}' requiere operandos booleanos";

                    dgvErrores.Rows.Add(
                        numeroLinea,
                        mensaje,
                        "Semántico - Tipos incompatibles");

                    dgvErrores.Rows[dgvErrores.Rows.Count - 1]
                        .DefaultCellStyle.ForeColor = Color.DarkOrange;

                    return "Error";
                }
            }

            return resultado;
        }

        private string ParseOperandoCondicion(
            List<Token> tokens,
            ref int idx,
            List<Simbolo> tablaSimbolos,
            int numeroLinea)
        {
            bool negado = false;

            if (idx < tokens.Count &&
                tokens[idx].Categoria == "NO")
            {
                negado = true;
                idx++;
            }

            if (idx >= tokens.Count)
            {
                Error_TipoCondicionVacia(numeroLinea);
                return "Error";
            }

            string tipoIzq =
                TipoDeOperando(
                    tokens[idx],
                    tablaSimbolos);

            idx++;

            string resultado;

            if (idx < tokens.Count &&
                EsOperadorRelacional(
                    tokens[idx].Categoria))
            {
                string opRelacional =
                    tokens[idx].Lexema;

                idx++;

                if (idx >= tokens.Count)
                {
                    resultado = "Error";
                }
                else
                {
                    string tipoDer =
                        TipoDeOperando(
                            tokens[idx],
                            tablaSimbolos);

                    idx++;

                    bool esError;

                    resultado = CombinarTipos(
                        tipoIzq,
                        opRelacional,
                        tipoDer,
                        out esError);

                    if (esError)
                    {
                        string mensaje =
                            $"Comparación no válida: '{tipoIzq}' {opRelacional} '{tipoDer}'";

                        dgvErrores.Rows.Add(
                            numeroLinea,
                            mensaje,
                            "Semántico - Tipos incompatibles");

                        dgvErrores.Rows[dgvErrores.Rows.Count - 1]
                            .DefaultCellStyle.ForeColor = Color.DarkOrange;

                        resultado = "Error";
                    }
                }
            }
            else
            {
                resultado = tipoIzq;
            }

            return negado
                ? (resultado == "Booleano"
                    ? "Booleano"
                    : "Error")
                : resultado;
        }

        private bool EsOperadorRelacional(string categoria)
        {
            return categoria == "OR1" ||
                   categoria == "OR2" ||
                   categoria == "OR3" ||
                   categoria == "OR4" ||
                   categoria == "OR5" ||
                   categoria == "OR6";
        }

        // ============================================================
        // PILA SEMÁNTICA
        // ============================================================

        private int PrecedenciaOperador(string categoria)
        {
            switch (categoria)
            {
                case "NO":
                    return 5;

                case "OA3":
                case "OA4":
                    return 4;

                case "OA1":
                case "OA2":
                    return 3;

                case "OR1":
                case "OR2":
                case "OR3":
                case "OR4":
                case "OR5":
                case "OR6":
                    return 2;

                case "Y":
                    return 1;

                case "O":
                    return 0;

                default:
                    return -1;
            }
        }

        private bool EsOperadorValido(string categoria)
        {
            return categoria == "OA1" ||
                   categoria == "OA2" ||
                   categoria == "OA3" ||
                   categoria == "OA4" ||
                   EsOperadorRelacional(categoria) ||
                   categoria == "Y" ||
                   categoria == "O" ||
                   categoria == "NO";
        }

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

        private bool AplicarOperador(
            string operador,
            Stack<string> pilaTipos,
            int numeroLinea)
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

                string mensajeNo =
                    $"El operador 'NO' requiere un operando Booleano, se obtuvo '{operando}'";

                dgvErrores.Rows.Add(
                    numeroLinea,
                    mensajeNo,
                    "Semántico - Tipos incompatibles");

                dgvErrores.Rows[dgvErrores.Rows.Count - 1]
                    .DefaultCellStyle.ForeColor = Color.DarkOrange;

                pilaTipos.Push("Error");
                return false;
            }

            if (pilaTipos.Count < 2)
            {
                Error_TipoCondicionVacia(numeroLinea);
                pilaTipos.Push("Error");
                return false;
            }

            string tipoDer = pilaTipos.Pop();
            string tipoIzq = pilaTipos.Pop();

            string simbolo =
                ObtenerSimboloOperador(operador);

            bool esError;

            string resultado =
                CombinarTipos(
                    tipoIzq,
                    simbolo,
                    tipoDer,
                    out esError);

            if (esError)
            {
                string mensaje =
                    $"Operación no válida: '{tipoIzq}' {simbolo} '{tipoDer}'";

                dgvErrores.Rows.Add(
                    numeroLinea,
                    mensaje,
                    "Semántico - Tipos incompatibles");

                dgvErrores.Rows[dgvErrores.Rows.Count - 1]
                    .DefaultCellStyle.ForeColor = Color.DarkOrange;

                pilaTipos.Push("Error");
                return false;
            }

            pilaTipos.Push(resultado);
            return true;
        }

        private void Error_TipoCondicionVacia(int numeroLinea)
        {
            dgvErrores.Rows.Add(
                numeroLinea,
                "Expresión vacía o mal formada",
                "Semántico - Expresión inválida");

            dgvErrores.Rows[dgvErrores.Rows.Count - 1]
                .DefaultCellStyle.ForeColor = Color.DarkOrange;
        }

        private string InferirTipoConPilaSemantica(
            List<Token> tokensExpr,
            List<Simbolo> tablaSimbolos,
            int numeroLinea)
        {
            if (tokensExpr.Count == 0)
                return "Desconocido";

            Stack<string> pilaTipos =
                new Stack<string>();

            Stack<string> pilaOperadores =
                new Stack<string>();

            foreach (var token in tokensExpr)
            {
                if (token.Categoria == "CE7")
                {
                    pilaOperadores.Push("(");
                }
                else if (token.Categoria == "CE8")
                {
                    while (pilaOperadores.Count > 0 &&
                           pilaOperadores.Peek() != "(")
                    {
                        string op =
                            pilaOperadores.Pop();

                        if (!AplicarOperador(
                            op,
                            pilaTipos,
                            numeroLinea))
                        {
                            return "Error";
                        }
                    }

                    if (pilaOperadores.Count > 0)
                        pilaOperadores.Pop();
                }
                else if (EsOperadorValido(token.Categoria))
                {
                    while (pilaOperadores.Count > 0 &&
                           pilaOperadores.Peek() != "(" &&
                           PrecedenciaOperador(
                               pilaOperadores.Peek()) >=
                           PrecedenciaOperador(
                               token.Categoria))
                    {
                        string op =
                            pilaOperadores.Pop();

                        if (!AplicarOperador(
                            op,
                            pilaTipos,
                            numeroLinea))
                        {
                            return "Error";
                        }
                    }

                    pilaOperadores.Push(
                        token.Categoria);
                }
                else
                {
                    pilaTipos.Push(
                        TipoDeOperando(
                            token,
                            tablaSimbolos));
                }
            }

            while (pilaOperadores.Count > 0)
            {
                string op =
                    pilaOperadores.Pop();

                if (op == "(")
                    continue;

                if (!AplicarOperador(
                    op,
                    pilaTipos,
                    numeroLinea))
                {
                    return "Error";
                }
            }

            if (pilaTipos.Count == 0)
                return "Desconocido";

            return pilaTipos.Pop();
        }

        // ============================================================
        // EVALUACIÓN REAL DE VALORES
        // ============================================================

        private object ObtenerValorOperando(
            Token token,
            List<Simbolo> simbolos)
        {
            if (token == null)
                return null;

            switch (token.Categoria)
            {
                case "IDENT":
                    var simbolo =
                        simbolos.FirstOrDefault(
                            s => s.Nombre.Equals(
                                token.Lexema,
                                StringComparison.OrdinalIgnoreCase));

                    if (simbolo == null ||
                        simbolo.Valor == null ||
                        string.IsNullOrWhiteSpace(
                            simbolo.Valor.ToString()))
                    {
                        return null;
                    }

                    return ConvertirValorTexto(
                        simbolo.Valor.ToString(),
                        simbolo.Tipo);

                case "CN ENTEROS":
                    if (int.TryParse(
                        token.Lexema,
                        NumberStyles.Integer,
                        CultureInfo.InvariantCulture,
                        out int entero))
                    {
                        return entero;
                    }
                    break;

                case "CN REALES":
                case "CN CON EXPONENTE":
                    if (double.TryParse(
                        token.Lexema,
                        NumberStyles.Float,
                        CultureInfo.InvariantCulture,
                        out double real))
                    {
                        return real;
                    }
                    break;

                case "CAD":
                    string cadena = token.Lexema;

                    if (cadena.Length >= 2 &&
                        ((cadena.StartsWith("'") &&
                          cadena.EndsWith("'")) ||
                         (cadena.StartsWith("\"") &&
                          cadena.EndsWith("\""))))
                    {
                        cadena =
                            cadena.Substring(
                                1,
                                cadena.Length - 2);
                    }

                    return cadena;
            }

            return null;
        }

        private object ConvertirValorTexto(
            string valor,
            string tipo)
        {
            if (string.IsNullOrWhiteSpace(valor))
                return null;

            valor = valor.Trim();

            if (tipo == "Booleano")
            {
                if (bool.TryParse(
                    valor,
                    out bool booleano))
                {
                    return booleano;
                }
            }

            if (tipo == "Entero")
            {
                if (int.TryParse(
                    valor,
                    NumberStyles.Integer,
                    CultureInfo.InvariantCulture,
                    out int entero))
                {
                    return entero;
                }
            }

            if (tipo == "Real")
            {
                if (double.TryParse(
                    valor,
                    NumberStyles.Float,
                    CultureInfo.InvariantCulture,
                    out double real))
                {
                    return real;
                }
            }

            if (tipo == "Cadena")
            {
                if (valor.Length >= 2 &&
                    ((valor.StartsWith("'") &&
                      valor.EndsWith("'")) ||
                     (valor.StartsWith("\"") &&
                      valor.EndsWith("\""))))
                {
                    return valor.Substring(
                        1,
                        valor.Length - 2);
                }

                return valor;
            }

            return valor;
        }

        private bool EsExpresionBooleana(
            List<Token> tokensExpr)
        {
            if (tokensExpr == null)
                return false;

            foreach (var token in tokensExpr)
            {
                if (token == null)
                    continue;

                if (EsOperadorRelacional(
                        token.Categoria) ||
                    token.Categoria == "Y" ||
                    token.Categoria == "O" ||
                    token.Categoria == "NO")
                {
                    return true;
                }
            }

            return false;
        }

        private bool EvaluarExpresionBooleana(
            List<Token> tokensExpr,
            List<Simbolo> simbolos,
            int numeroLinea)
        {
            try
            {
                int indice = 0;

                return EvaluarOperacionO(
                    tokensExpr,
                    ref indice,
                    simbolos,
                    numeroLinea);
            }
            catch
            {
                return false;
            }
        }

        private bool EvaluarOperacionO(
            List<Token> tokens,
            ref int indice,
            List<Simbolo> simbolos,
            int numeroLinea)
        {
            bool resultado =
                EvaluarOperacionY(
                    tokens,
                    ref indice,
                    simbolos,
                    numeroLinea);

            while (indice < tokens.Count &&
                   tokens[indice].Categoria == "O")
            {
                indice++;

                bool derecha =
                    EvaluarOperacionY(
                        tokens,
                        ref indice,
                        simbolos,
                        numeroLinea);

                resultado =
                    resultado || derecha;
            }

            return resultado;
        }

        private bool EvaluarOperacionY(
            List<Token> tokens,
            ref int indice,
            List<Simbolo> simbolos,
            int numeroLinea)
        {
            bool resultado =
                EvaluarOperacionNo(
                    tokens,
                    ref indice,
                    simbolos,
                    numeroLinea);

            while (indice < tokens.Count &&
                   tokens[indice].Categoria == "Y")
            {
                indice++;

                bool derecha =
                    EvaluarOperacionNo(
                        tokens,
                        ref indice,
                        simbolos,
                        numeroLinea);

                resultado =
                    resultado && derecha;
            }

            return resultado;
        }

        private bool EvaluarOperacionNo(
            List<Token> tokens,
            ref int indice,
            List<Simbolo> simbolos,
            int numeroLinea)
        {
            if (indice < tokens.Count &&
                tokens[indice].Categoria == "NO")
            {
                indice++;

                return !EvaluarOperacionNo(
                    tokens,
                    ref indice,
                    simbolos,
                    numeroLinea);
            }

            return EvaluarFactorBooleano(
                tokens,
                ref indice,
                simbolos,
                numeroLinea);
        }

        private bool EvaluarFactorBooleano(
            List<Token> tokens,
            ref int indice,
            List<Simbolo> simbolos,
            int numeroLinea)
        {
            if (indice >= tokens.Count)
                return false;

            if (tokens[indice].Categoria == "CE7")
            {
                indice++;

                bool resultado =
                    EvaluarOperacionO(
                        tokens,
                        ref indice,
                        simbolos,
                        numeroLinea);

                if (indice < tokens.Count &&
                    tokens[indice].Categoria == "CE8")
                {
                    indice++;
                }

                return resultado;
            }

            object izquierda =
                ObtenerValorOperando(
                    tokens[indice],
                    simbolos);

            indice++;

            if (indice >= tokens.Count ||
                !EsOperadorRelacional(
                    tokens[indice].Categoria))
            {
                if (izquierda is bool)
                    return (bool)izquierda;

                return false;
            }

            string operador =
                tokens[indice].Categoria;

            indice++;

            if (indice >= tokens.Count)
                return false;

            object derecha =
                ObtenerValorOperando(
                    tokens[indice],
                    simbolos);

            indice++;

            return CompararValores(
                izquierda,
                derecha,
                operador);
        }

        private bool CompararValores(
            object izquierda,
            object derecha,
            string operador)
        {
            if (izquierda == null ||
                derecha == null)
            {
                return false;
            }

            bool izquierdaNumerica =
                izquierda is int ||
                izquierda is double ||
                izquierda is float ||
                izquierda is decimal;

            bool derechaNumerica =
                derecha is int ||
                derecha is double ||
                derecha is float ||
                derecha is decimal;

            if (izquierdaNumerica &&
                derechaNumerica)
            {
                double a =
                    Convert.ToDouble(
                        izquierda,
                        CultureInfo.InvariantCulture);

                double b =
                    Convert.ToDouble(
                        derecha,
                        CultureInfo.InvariantCulture);

                switch (operador)
                {
                    case "OR1": return a > b;
                    case "OR2": return a >= b;
                    case "OR3": return a < b;
                    case "OR4": return a <= b;
                    case "OR5": return a != b;
                    case "OR6": return a == b;
                }
            }

            if (izquierda is string &&
                derecha is string)
            {
                int comparacion =
                    string.Compare(
                        izquierda.ToString(),
                        derecha.ToString(),
                        StringComparison.Ordinal);

                switch (operador)
                {
                    case "OR1": return comparacion > 0;
                    case "OR2": return comparacion >= 0;
                    case "OR3": return comparacion < 0;
                    case "OR4": return comparacion <= 0;
                    case "OR5": return comparacion != 0;
                    case "OR6": return comparacion == 0;
                }
            }

            if (izquierda is bool &&
                derecha is bool)
            {
                bool a = (bool)izquierda;
                bool b = (bool)derecha;

                if (operador == "OR6")
                    return a == b;

                if (operador == "OR5")
                    return a != b;
            }

            return false;
        }

        // ============================================================
        // EVALUACIÓN ARITMÉTICA
        // ============================================================

        private object EvaluarExpresionAritmetica(
            List<Token> tokensExpr,
            List<Simbolo> simbolos)
        {
            try
            {
                int indice = 0;

                return EvaluarSumaResta(
                    tokensExpr,
                    ref indice,
                    simbolos);
            }
            catch
            {
                return null;
            }
        }

        private object EvaluarSumaResta(
            List<Token> tokens,
            ref int indice,
            List<Simbolo> simbolos)
        {
            object resultado =
                EvaluarMultiplicacionDivision(
                    tokens,
                    ref indice,
                    simbolos);

            while (indice < tokens.Count &&
                  (tokens[indice].Categoria == "OA1" ||
                   tokens[indice].Categoria == "OA2"))
            {
                string operador =
                    tokens[indice].Categoria;

                indice++;

                object derecha =
                    EvaluarMultiplicacionDivision(
                        tokens,
                        ref indice,
                        simbolos);

                resultado =
                    AplicarOperacionAritmetica(
                        resultado,
                        derecha,
                        operador);
            }

            return resultado;
        }

        private object EvaluarMultiplicacionDivision(
            List<Token> tokens,
            ref int indice,
            List<Simbolo> simbolos)
        {
            object resultado =
                EvaluarFactorAritmetico(
                    tokens,
                    ref indice,
                    simbolos);

            while (indice < tokens.Count &&
                  (tokens[indice].Categoria == "OA3" ||
                   tokens[indice].Categoria == "OA4"))
            {
                string operador =
                    tokens[indice].Categoria;

                indice++;

                object derecha =
                    EvaluarFactorAritmetico(
                        tokens,
                        ref indice,
                        simbolos);

                resultado =
                    AplicarOperacionAritmetica(
                        resultado,
                        derecha,
                        operador);
            }

            return resultado;
        }

        private object EvaluarFactorAritmetico(
            List<Token> tokens,
            ref int indice,
            List<Simbolo> simbolos)
        {
            if (indice >= tokens.Count)
                return null;

            if (tokens[indice].Categoria == "CE7")
            {
                indice++;

                object resultado =
                    EvaluarSumaResta(
                        tokens,
                        ref indice,
                        simbolos);

                if (indice < tokens.Count &&
                    tokens[indice].Categoria == "CE8")
                {
                    indice++;
                }

                return resultado;
            }

            Token token = tokens[indice];
            indice++;

            return ObtenerValorOperando(
                token,
                simbolos);
        }

        private object AplicarOperacionAritmetica(
            object izquierda,
            object derecha,
            string operador)
        {
            if (izquierda == null ||
                derecha == null)
            {
                return null;
            }

            if (operador == "OA1" &&
                (izquierda is string || derecha is string))
            {
                return FormatearValorConcatenacion(izquierda) +
                       FormatearValorConcatenacion(derecha);
            }

            bool ambosEnteros =
                izquierda is int &&
                derecha is int;

            double a =
                Convert.ToDouble(
                    izquierda,
                    CultureInfo.InvariantCulture);

            double b =
                Convert.ToDouble(
                    derecha,
                    CultureInfo.InvariantCulture);

            switch (operador)
            {
                case "OA1":
                    return ambosEnteros
                        ? (object)((int)a + (int)b)
                        : a + b;

                case "OA2":
                    return ambosEnteros
                        ? (object)((int)a - (int)b)
                        : a - b;

                case "OA3":
                    return ambosEnteros
                        ? (object)((int)a * (int)b)
                        : a * b;

                case "OA4":
                    if (b == 0)
                        return null;

                    return ambosEnteros
                        ? (object)((int)a / (int)b)
                        : a / b;
            }

            return null;
        }

        private string FormatearValorConcatenacion(object valor)
        {
            if (valor == null)
                return "";

            if (valor is double)
                return ((double)valor).ToString(
                    CultureInfo.InvariantCulture);

            if (valor is float)
                return ((float)valor).ToString(
                    CultureInfo.InvariantCulture);

            if (valor is decimal)
                return ((decimal)valor).ToString(
                    CultureInfo.InvariantCulture);

            return valor.ToString();
        }

        private string ObtenerTipoValorSemantico(
            object valor)
        {
            if (valor == null)
                return "";

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

            return "";
        }

        private string ValorParaTabla(object valor)
        {
            if (valor == null)
                return "";

            if (valor is bool)
                return ((bool)valor)
                    ? "true"
                    : "false";

            if (valor is double)
                return ((double)valor).ToString(
                    CultureInfo.InvariantCulture);

            if (valor is float)
                return ((float)valor).ToString(
                    CultureInfo.InvariantCulture);

            return valor.ToString();
        }

        // ============================================================
        // EJECUCIÓN LÉXICA
        // ============================================================

        private void EjecutarLexico()
        {
            rtbTokens.Clear();
            dgvErrores.Rows.Clear();
            dgvSimbolos.Rows.Clear();
            tablaSimbolos.Clear();
            tokensLexico.Clear();

            string[] lines =
                RemoveLineNumbers(rtbFuente.Text)
                .Split(
                    new[] { '\r', '\n' },
                    StringSplitOptions.RemoveEmptyEntries);

            for (int i = 0; i < lines.Length; i++)
            {
                int numeroLinea = i + 1;

                var lexemas =
                    lexerDB.LeerCadena(lines[i]);

                var tokensLinea =
                    new List<Token>();

                foreach (var lex in lexemas)
                {
                    Token t;

                    try
                    {
                        t = lexerDB.RecorrerMatriz(lex);
                    }
                    catch (Exception ex)
                    {
                        t = new Token
                        {
                            Lexema = lex,
                            Estado = -1,
                            Categoria =
                                "ERROR: Excepción durante análisis: "
                                + ex.Message,
                            EsError = true
                        };
                    }

                    t.Lexema = lex;
                    tokensLinea.Add(t);
                }

                // Registrar símbolos
                foreach (var token in tokensLinea)
                {
                    if (!token.EsError)
                    {
                        lexerDB.ActualizarTablaSimbolos(
                            token,
                            tablaSimbolos);
                    }
                }

                // ====================================================
                // ASIGNACIONES
                // ====================================================

                for (int k = 0;
                     k < tokensLinea.Count - 1;
                     k++)
                {
                    if (!tokensLinea[k].EsError &&
                        tokensLinea[k].Categoria == "IDENT" &&
                        !tokensLinea[k + 1].EsError &&
                        tokensLinea[k + 1].Categoria == "ASI")
                    {
                        string nombreVar =
                            tokensLinea[k].Lexema;

                        var tokensExpr =
                            new List<Token>();

                        int j = k + 2;

                        while (j < tokensLinea.Count &&
                               tokensLinea[j].Categoria != "DEL")
                        {
                            if (!tokensLinea[j].EsError)
                                tokensExpr.Add(
                                    tokensLinea[j]);

                            j++;
                        }

                        var simboloDestino =
                            tablaSimbolos.FirstOrDefault(
                                s => s.Nombre.Equals(
                                    nombreVar,
                                    StringComparison.OrdinalIgnoreCase));

                        // Primero inferir el tipo
                        string tipoResultado =
                            InferirTipoConPilaSemantica(
                                tokensExpr,
                                tablaSimbolos,
                                numeroLinea);

                        if (simboloDestino != null &&
                            tipoResultado != "Error" &&
                            tipoResultado != "Desconocido")
                        {
                            simboloDestino.Tipo =
                                tipoResultado;
                        }

                        // ====================================================
                        // AQUÍ ESTÁ LA CORRECCIÓN IMPORTANTE:
                        // YA NO GUARDAMOS "pEdad > 18".
                        // EVALUAMOS LA EXPRESIÓN.
                        // ====================================================

                        if (simboloDestino != null)
                        {
                            try
                            {
                                object valorEvaluado;

                                if (EsExpresionBooleana(
                                    tokensExpr))
                                {
                                    valorEvaluado =
                                        EvaluarExpresionBooleana(
                                            tokensExpr,
                                            tablaSimbolos,
                                            numeroLinea);

                                    simboloDestino.Tipo =
                                        "Booleano";
                                }
                                else
                                {
                                    valorEvaluado =
                                        EvaluarExpresionAritmetica(
                                            tokensExpr,
                                            tablaSimbolos);
                                }

                                if (valorEvaluado != null)
                                {
                                    simboloDestino.Valor =
                                        ValorParaTabla(
                                            valorEvaluado);

                                    string tipoEvaluado =
                                        ObtenerTipoValorSemantico(
                                            valorEvaluado);

                                    if (!string.IsNullOrEmpty(
                                        tipoEvaluado))
                                    {
                                        simboloDestino.Tipo =
                                            tipoEvaluado;
                                    }
                                }
                            }
                            catch
                            {
                                // No detener el análisis
                            }
                        }

                        k = j;
                    }
                }

                // ====================================================
                // LEER
                // ====================================================

                for (int k = 0;
                     k < tokensLinea.Count - 1;
                     k++)
                {
                    if (!tokensLinea[k].EsError &&
                        tokensLinea[k].Categoria == "LEER" &&
                        !tokensLinea[k + 1].EsError &&
                        tokensLinea[k + 1].Categoria == "IDENT")
                    {
                        var simbolo =
                            tablaSimbolos.FirstOrDefault(
                                s => s.Nombre.Equals(
                                    tokensLinea[k + 1].Lexema,
                                    StringComparison.OrdinalIgnoreCase));

                        if (simbolo != null &&
                            (string.IsNullOrEmpty(simbolo.Tipo) ||
                             simbolo.Tipo == "Sin determinar"))
                        {
                            simbolo.Tipo = "Entero";
                        }
                    }
                }

                // ====================================================
                // CONDICIONES DE SI
                // ====================================================

                for (int k = 0;
                     k < tokensLinea.Count - 1;
                     k++)
                {
                    if (!tokensLinea[k].EsError &&
                        tokensLinea[k].Categoria == "SI" &&
                        !tokensLinea[k + 1].EsError &&
                        tokensLinea[k + 1].Categoria == "CE7")
                    {
                        var tokensCondicion =
                            new List<Token>();

                        int j = k + 2;
                        int nivelParentesis = 1;

                        while (j < tokensLinea.Count &&
                               nivelParentesis > 0)
                        {
                            if (tokensLinea[j].Categoria == "CE7")
                                nivelParentesis++;

                            else if (tokensLinea[j].Categoria == "CE8")
                                nivelParentesis--;

                            if (nivelParentesis > 0 &&
                                !tokensLinea[j].EsError)
                            {
                                tokensCondicion.Add(
                                    tokensLinea[j]);
                            }

                            j++;
                        }

                        string tipoCondicion =
                            InferirTipoConPilaSemantica(
                                tokensCondicion,
                                tablaSimbolos,
                                numeroLinea);

                        if (tipoCondicion != "Booleano" &&
                            tipoCondicion != "Error")
                        {
                            string mensaje =
                                $"La condición de SI debe ser Booleana, se obtuvo '{tipoCondicion}'";

                            dgvErrores.Rows.Add(
                                numeroLinea,
                                mensaje,
                                "Semántico - Condición no booleana");

                            dgvErrores.Rows[dgvErrores.Rows.Count - 1]
                                .DefaultCellStyle.ForeColor =
                                Color.DarkOrange;
                        }
                    }
                }

                // ====================================================
                // TOKENS
                // ====================================================

                rtbTokens.SelectionColor = Color.Blue;
                rtbTokens.AppendText(
                    $"{numeroLinea}  ");
                rtbTokens.SelectionColor = Color.Black;

                foreach (var token in tokensLinea)
                {
                    if (token.EsError)
                    {
                        string tipoError =
                            ClasificarTipoError(
                                token.Categoria);

                        dgvErrores.Rows.Add(
                            numeroLinea,
                            token.Categoria,
                            tipoError);

                        dgvErrores.Rows[
                            dgvErrores.Rows.Count - 1]
                            .DefaultCellStyle.ForeColor =
                            Color.Red;

                        rtbTokens.SelectionStart =
                            rtbTokens.TextLength;

                        rtbTokens.SelectionColor =
                            Color.Red;

                        rtbTokens.AppendText(
                            $"Error {token.Lexema} ");
                    }
                    else
                    {
                        tokensLexico.Add(token);

                        var simboloActual =
                            tablaSimbolos.FirstOrDefault(
                                s => s.Nombre.Equals(
                                    token.Lexema,
                                    StringComparison.OrdinalIgnoreCase));

                        string etiquetaMostrar =
                            token.Categoria;

                        if (token.Categoria == "IDENT" &&
                            simboloActual != null)
                        {
                            etiquetaMostrar =
                                $"IDENT{simboloActual.NumID}";

                            rtbTokens.SelectionColor =
                                Color.Orange;
                        }
                        else
                        {
                            rtbTokens.SelectionColor =
                                Color.Black;
                        }

                        rtbTokens.SelectionStart =
                            rtbTokens.TextLength;

                        rtbTokens.AppendText(
                            $" {etiquetaMostrar} ");

                        rtbTokens.SelectionColor =
                            rtbTokens.ForeColor;
                    }
                }

                rtbTokens.AppendText("\n");
            }

            // ====================================================
            // TABLA DE SÍMBOLOS
            // ====================================================

            foreach (var s in tablaSimbolos)
            {
                int idx =
                    dgvSimbolos.Rows.Add();

                dgvSimbolos.Rows[idx].Cells[0].Value =
                    s.NumID;

                dgvSimbolos.Rows[idx].Cells[1].Value =
                    s.Nombre;

                dgvSimbolos.Rows[idx].Cells[2].Value =
                    string.IsNullOrEmpty(s.Tipo)
                        ? "Sin determinar"
                        : s.Tipo;

                if (dgvSimbolos.Columns.Count > 3)
                {
                    dgvSimbolos.Rows[idx].Cells[3].Value =
                        s.Valor ?? string.Empty;
                }

                dgvSimbolos.Rows[idx]
                    .DefaultCellStyle.ForeColor =
                    Color.Red;
            }

            rtbFuente.ReadOnly = true;
        }

        // ============================================================
        // RESTO DEL FORM
        // ============================================================

        private string ClasificarTipoError(
            string categoriaError)
        {
            if (categoriaError.Contains(
                "no reconocido"))
                return "Léxico - Carácter no válido";

            if (categoriaError.Contains(
                "Transición inválida"))
                return "Léxico - Transición inválida";

            if (categoriaError.Contains("Fase"))
                return "Léxico - Estado inválido";

            if (categoriaError.Contains(
                "Identificador"))
                return "Léxico - Identificador inválido";

            if (categoriaError.Contains(
                "numérica"))
                return "Léxico - Constante numérica inválida";

            if (categoriaError.Contains(
                "Excepción"))
                return "Léxico - Error interno";

            return "Léxico - Error no clasificado";
        }

        private string AddLineNumbers(string text)
        {
            var lines =
                text.Split(
                    new[] { "\r\n", "\r", "\n" },
                    StringSplitOptions.None);

            var sb = new StringBuilder();

            for (int i = 0; i < lines.Length; i++)
            {
                if (i < lines.Length - 1)
                    sb.AppendLine(lines[i]);
                else
                    sb.Append(lines[i]);
            }

            return sb.ToString();
        }

        private string RemoveLineNumbers(string text)
        {
            var lines =
                text.Split(
                    new[] { '\r', '\n' },
                    StringSplitOptions.None);

            var sb = new StringBuilder();

            var regex =
                new Regex("^\\s*\\d+\\t?");

            for (int i = 0; i < lines.Length; i++)
            {
                string clean =
                    regex.Replace(lines[i], "");

                sb.AppendLine(clean);
            }

            return sb.ToString();
        }

        private void RtbFuente_KeyDown(
            object sender,
            KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Tab)
            {
                e.SuppressKeyPress = true;
                rtbFuente.SelectedText = "     ";
            }
        }

        private void Form1_Load(
            object sender,
            EventArgs e)
        {
            string msg;

            bool ok =
                lexerDB.TestConnection(
                    out msg);

            if (ok)
            {
                MessageBox.Show(
                    "Conexión exitosa a la base de datos.",
                    "Prueba BD",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
            }
            else
            {
                MessageBox.Show(
                    "Error de conexión a la base de datos:\n"
                    + msg,
                    "Prueba BD",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        private void btnCargar_Click(
            object sender,
            EventArgs e)
        {
            using (OpenFileDialog ofd =
                   new OpenFileDialog())
            {
                ofd.Filter =
                    "Text Files|*.txt";

                if (ofd.ShowDialog() ==
                    DialogResult.OK)
                {
                    string content =
                        File.ReadAllText(
                            ofd.FileName,
                            Encoding.UTF8);

                    rtbFuente.Text =
                        AddLineNumbers(content);

                    rtbFuente.ReadOnly = true;

                    rtbTokens.Clear();
                    dgvErrores.Rows.Clear();
                    dgvSimbolos.Rows.Clear();
                    tablaSimbolos.Clear();
                }
            }
        }

        private void btnEditar_Click(
            object sender,
            EventArgs e)
        {
            rtbFuente.ReadOnly = false;

            rtbTokens.Clear();
            dgvErrores.Rows.Clear();
            dgvSimbolos.Rows.Clear();
            tablaSimbolos.Clear();

            dgvErroresSintacticos.Rows.Clear();
            rtSintaxis.Clear();
        }

        private void btnGuardar_Click(
            object sender,
            EventArgs e)
        {
            using (SaveFileDialog sfd =
                   new SaveFileDialog())
            {
                sfd.Filter =
                    "Text Files|*.txt";

                if (sfd.ShowDialog() ==
                    DialogResult.OK)
                {
                    string text =
                        RemoveLineNumbers(
                            rtbFuente.Text);

                    File.WriteAllText(
                        sfd.FileName,
                        text,
                        Encoding.UTF8);
                }
            }
        }

        private void btnGuardarA_Click(
            object sender,
            EventArgs e)
        {
            using (SaveFileDialog sfd =
                   new SaveFileDialog())
            {
                sfd.Filter =
                    "Text Files|*.txt";

                sfd.FileName = "tokens.txt";

                if (sfd.ShowDialog() ==
                    DialogResult.OK)
                {
                    File.WriteAllText(
                        sfd.FileName,
                        rtbTokens.Text,
                        Encoding.UTF8);
                }
            }
        }

        private void label4_Click(
            object sender,
            EventArgs e)
        {
            if (!lexerDB.IsMatrizLoaded())
            {
                MessageBox.Show(
                    "La matriz de transición no está disponible. Verifica la conexión a la base de datos.",
                    "Error BD",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);

                return;
            }

            EjecutarLexico();

            if (dgvErrores.Rows.Count > 0)
            {
                btnGuardarA.Enabled = false;

                dgvErroresSintacticos.Rows.Clear();
                rtSintaxis.Clear();

                MessageBox.Show(
                    "Se encontraron errores léxicos. No se puede ejecutar el análisis sintáctico.",
                    "Errores Léxicos",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
            }
            else
            {
                btnGuardarA.Enabled = true;

                bool hayErroresSintacticos =
                    EjecutarSintactico();

                if (hayErroresSintacticos)
                {
                    MessageBox.Show(
                        "Se encontraron errores en la sintaxis.",
                        "Errores Sintácticos",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning);
                }
                else
                {
                    MessageBox.Show(
                        "No se encontraron errores en la sintaxis.",
                        "Análisis Sintáctico",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information);
                }
            }
        }

        private bool EjecutarSintactico()
        {
            if (string.IsNullOrEmpty(
                    rtbTokens.Text) ||
                rtbTokens.Text.Trim() == "")
            {
                EjecutarLexico();
            }

            List<ParserSintactico.TokenSintactico>
                tokensSintacticos =
                new List<ParserSintactico.TokenSintactico>();

            string[] lines =
                RemoveLineNumbers(
                    rtbFuente.Text)
                .Split(
                    new[] { '\r', '\n' },
                    StringSplitOptions.RemoveEmptyEntries);

            int tokenIndex = 0;

            for (int lineaActual = 0;
                 lineaActual < lines.Length;
                 lineaActual++)
            {
                var lexemas =
                    lexerDB.LeerCadena(
                        lines[lineaActual]);

                int numeroLinea =
                    lineaActual + 1;

                foreach (var lex in lexemas)
                {
                    if (tokenIndex <
                        tokensLexico.Count)
                    {
                        Token token =
                            tokensLexico[tokenIndex];

                        if (!token.EsError)
                        {
                            string tipo =
                                DeterminarTipoTokenDesdeCategoria(
                                    token.Categoria);

                            tokensSintacticos.Add(
                                new ParserSintactico.TokenSintactico
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

            ParserSintactico parser =
                new ParserSintactico(
                    tokensSintacticos,
                    dgvErroresSintacticos,
                    rtSintaxis);

            parser.Parse();

            return dgvErroresSintacticos.Rows.Count > 0;
        }

        private string DeterminarTipoTokenDesdeCategoria(
            string categoria)
        {
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

                case "Y": return "OPL1";
                case "O": return "OPL2";
                case "NO": return "OPL3";

                case "DEL": return "CE13";

                case "CE7": return "CE7";
                case "CE8": return "CE8";
                case "CE9": return "CE9";
                case "CE10": return "CE10";
                case "CE11": return "CE11";
                case "CE12": return "CE12";
                case "CE14": return "CE14";
                case "CE15": return "CE15";
                case "CE16": return "CE16";
                case "CE17": return "CE17";
                case "CE18": return "CE18";
                case "CE19": return "CE19";
                case "CE20": return "CE20";
                case "CE21": return "CE21";
                case "CE22": return "CE22";
                case "CE23": return "CE23";

                case "CN ENTEROS":
                case "CN REALES":
                case "CN CON EXPONENTE":
                    return "CNU";

                case "CAD":
                    return "CAD";

                case "COM":
                    return "COMEN";

                default:
                    return categoria;
            }
        }

        private string DeterminarTipoToken(
            string tokenStr)
        {
            if (tokenStr.StartsWith("IDENT"))
                return "IDENT";

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

            if (tokenStr == "CE7") return "CE7";
            if (tokenStr == "CE8") return "CE8";
            if (tokenStr == "CE9") return "CE9";
            if (tokenStr == "CE10") return "CE10";
            if (tokenStr == "CE13") return "CE13";
            if (tokenStr == "CE16") return "CE16";

            if (tokenStr.StartsWith("CN"))
                return "CNU";

            if (tokenStr == "CAD")
                return "CAD";

            return tokenStr;
        }

        // ============================================================
        // EVENTOS
        // ============================================================

        private void btnGuardar_Click_1(
            object sender,
            EventArgs e)
        {
            using (SaveFileDialog sfd =
                   new SaveFileDialog())
            {
                sfd.Filter =
                    "Text Files|*.txt";

                if (sfd.ShowDialog() ==
                    DialogResult.OK)
                {
                    File.WriteAllText(
                        sfd.FileName,
                        rtbFuente.Text,
                        Encoding.UTF8);
                }
            }
        }

        private void label4_MouseEnter(
            object sender,
            EventArgs e)
        {
            label4.Cursor =
                Cursors.Hand;

            label4.BackColor =
                Color.FromArgb(
                    187,
                    222,
                    251);
        }

        private void label4_MouseLeave(
            object sender,
            EventArgs e)
        {
            label4.BackColor =
                Color.FromArgb(
                    25,
                    70,
                    130);
        }

        private void label5_Click(
            object sender,
            EventArgs e)
        {
            if (!lexerDB.IsMatrizLoaded())
            {
                MessageBox.Show(
                    "La matriz de transición no está disponible. Verifica la conexión a la base de datos.",
                    "Error BD",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);

                return;
            }

            EjecutarSintactico();
        }

        private void label5_MouseEnter(
            object sender,
            EventArgs e)
        {
            label5.Cursor =
                Cursors.Hand;

            label5.BackColor =
                Color.FromArgb(
                    187,
                    222,
                    251);
        }

        private void label5_MouseLeave(
            object sender,
            EventArgs e)
        {
            label5.BackColor =
                Color.FromArgb(
                    25,
                    70,
                    130);
        }

        private void btnInfo_Click(
            object sender,
            EventArgs e)
        {
            FormInfo infoForm =
                new FormInfo();

            infoForm.Show();
        }

        public void UpdateLineNumbers()
        {
            Point pt =
                new Point(0, 0);

            int firstIndex =
                rtbFuente.GetCharIndexFromPosition(
                    pt);

            int firstLine =
                rtbFuente.GetLineFromCharIndex(
                    firstIndex);

            pt.X =
                rtbFuente.ClientRectangle.Width;

            pt.Y =
                rtbFuente.ClientRectangle.Height;

            int lastIndex =
                rtbFuente.GetCharIndexFromPosition(
                    pt);

            int lastLine =
                rtbFuente.GetLineFromCharIndex(
                    lastIndex);

            txtNumerosLinea.SelectionAlignment =
                HorizontalAlignment.Center;

            txtNumerosLinea.Text = "";

            for (int i = firstLine;
                 i <= lastLine + 1;
                 i++)
            {
                txtNumerosLinea.Text +=
                    (i + 1) + "\n";
            }
        }

        private void rtbFuente_VScroll(object sender,EventArgs e)
        {
            UpdateLineNumbers();
        }

        private void rtbFuente_TextChanged_1(object sender,EventArgs e)
        {
            UpdateLineNumbers();
        }

        private void rtbFuente_VScroll_1(object sender,EventArgs e)
        {
            UpdateLineNumbers();
        }

        private void lblTitulo_Click(object sender,EventArgs e)
        {
        }

        private void lblTitulo_Click_1(object sender,EventArgs e)
        {
        }

        private void label13_Click(object sender,EventArgs e)
        {
        }

        private void label4_Click_1(object sender,EventArgs e)
        {
        }

        private void dgvErrores_CellContentClick(object sender,DataGridViewCellEventArgs e)
        {
        }

        private void dgvSimbolos_CellContentClick(object sender,DataGridViewCellEventArgs e)
        {
        }

        private void dgvErroresSintacticos_CellContentClick(object sender,DataGridViewCellEventArgs e)
        {
        }

        private void rtSintaxis_TextChanged(object sender,EventArgs e)
        {
        }
    }
}