using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace AnalizadorLexico
{
    public partial class CodigoIntermedio : Form
    {
        public CodigoIntermedio()
        {
            InitializeComponent();
            ConfigurarComponentesLectura();
        }

        public void CargarCodigo(string codigoIntermedio, string codigoFuente = null)
        {
            if (string.IsNullOrWhiteSpace(codigoIntermedio))
            {
                codigoIntermedio = string.Empty;
            }

            rtCodigoP.Text = codigoIntermedio;
            var fuente = string.IsNullOrWhiteSpace(codigoFuente) ? codigoIntermedio : codigoFuente;
            var textoFuente = NormalizarFuente(fuente);

            var triplos = GenerarTriplosDesdeFuente(textoFuente);
            dgTriplos.Rows.Clear();
            foreach (var triple in triplos)
            {
                dgTriplos.Rows.Add(triple.Indice, triple.Op, triple.Arg1, triple.Arg2);
            }
            rtTresDireccionesTriplos.Text = string.Join(Environment.NewLine, triplos.Select(t => t.Representacion).ToArray());

            var cuadruplos = GenerarCuadruplosDesdeFuente(textoFuente);
            dgCuadruplos.Rows.Clear();
            foreach (var cuadruplo in cuadruplos)
            {
                dgCuadruplos.Rows.Add(cuadruplo.Indice, cuadruplo.Op, cuadruplo.Arg1, cuadruplo.Arg2, cuadruplo.Resultado);
            }
            rtTresDireccionesCuadruplos.Text = string.Join(Environment.NewLine, cuadruplos.Select(c => c.Representacion).ToArray());

            var prefija = GenerarNotacionPrefijaDesdeFuente(textoFuente);
            dgPrefija.Rows.Clear();
            foreach (var fila in prefija)
            {
                dgPrefija.Rows.Add(fila.Simbolo, fila.Accion, fila.Pila, fila.Calculos);
            }
            rtResultadoPrefija.Text = string.Join(Environment.NewLine,
                prefija
                    .Where(x => !string.IsNullOrWhiteSpace(x.Resultado))
                    .Select(x => x.Resultado)
                    .Distinct()
                    .ToArray());

            var postfija = GenerarNotacionPostfijaDesdeFuente(textoFuente);
            dgPostfija.Rows.Clear();
            foreach (var fila in postfija)
            {
                dgPostfija.Rows.Add(fila.Simbolo, fila.Accion, fila.Pila, fila.Calculos);
            }
            rtResultadoPostfija.Text = string.Join(Environment.NewLine,
                postfija
                    .Where(x => !string.IsNullOrWhiteSpace(x.Resultado))
                    .Select(x => x.Resultado)
                    .Distinct()
                    .ToArray());
        }

        private void ConfigurarComponentesLectura()
        {
            rtCodigoP.ReadOnly = true;
            rtTresDireccionesTriplos.ReadOnly = true;
            rtTresDireccionesCuadruplos.ReadOnly = true;
            rtResultadoPrefija.ReadOnly = true;
            rtResultadoPostfija.ReadOnly = true;
            dgTriplos.ReadOnly = true;
            dgCuadruplos.ReadOnly = true;
            dgPrefija.ReadOnly = true;
            dgPostfija.ReadOnly = true;
        }

        private static string NormalizarFuente(string fuente)
        {
            if (string.IsNullOrWhiteSpace(fuente))
                return string.Empty;

            return fuente.Replace("\r\n", "\n").Replace('\r', '\n');
        }

        private static List<AsignacionFila> ObtenerAsignaciones(string fuente)
        {
            var resultado = new List<AsignacionFila>();
            if (string.IsNullOrWhiteSpace(fuente))
                return resultado;

            foreach (var linea in fuente.Split('\n'))
            {
                var texto = linea.Trim();
                if (string.IsNullOrWhiteSpace(texto))
                    continue;

                var match = Regex.Match(texto, @"^([A-Za-z_][A-Za-z0-9_]*)\s*=\s*(.+)$", RegexOptions.IgnoreCase);
                if (!match.Success)
                    continue;

                resultado.Add(new AsignacionFila(match.Groups[1].Value, match.Groups[2].Value.Trim().TrimEnd(';').Trim()));
            }

            return resultado;
        }

        private static List<string> ObtenerMostrars(string fuente)
        {
            var resultado = new List<string>();
            if (string.IsNullOrWhiteSpace(fuente))
                return resultado;

            foreach (var linea in fuente.Split('\n'))
            {
                var texto = linea.Trim();
                if (string.IsNullOrWhiteSpace(texto))
                    continue;

                var match = Regex.Match(texto, @"^MOSTRAR\s+(.+)$", RegexOptions.IgnoreCase);
                if (match.Success)
                {
                    resultado.Add(match.Groups[1].Value.Trim().TrimEnd(';').Trim());
                }
            }

            return resultado;
        }

        private static List<string> ObtenerMostrarsFueraDeSi(string fuente)
        {
            var resultado = new List<string>();
            if (string.IsNullOrWhiteSpace(fuente))
                return resultado;

            var lineas = fuente.Split('\n');
            bool dentroSi = false;

            foreach (var linea in lineas)
            {
                var texto = linea.Trim();
                if (string.IsNullOrWhiteSpace(texto))
                    continue;

                if (Regex.IsMatch(texto, @"^SI\s*\(.*\)\s*ENTONCES\s*$", RegexOptions.IgnoreCase))
                {
                    dentroSi = true;
                    continue;
                }

                if (dentroSi && Regex.IsMatch(texto, @"^FIN\s*;?\s*$", RegexOptions.IgnoreCase))
                {
                    dentroSi = false;
                    continue;
                }

                if (dentroSi)
                    continue;

                var match = Regex.Match(texto, @"^MOSTRAR\s+(.+)$", RegexOptions.IgnoreCase);
                if (match.Success)
                    resultado.Add(match.Groups[1].Value.Trim().TrimEnd(';').Trim());
            }

            return resultado;
        }

        private static List<TripleFila> GenerarTriplosDesdeFuente(string fuente)
        {
            var resultado = new List<TripleFila>();
            var asignaciones = ObtenerAsignaciones(fuente);

            foreach (var asignacion in asignaciones)
            {
                var tokens = ObtenerTokensExpresion(asignacion.Expresion);
                if (tokens.Count == 0)
                    continue;

                if (tokens.Count == 1 && !EsOperadorBinario(tokens[0]))
                {
                    resultado.Add(new TripleFila(resultado.Count, "=", asignacion.Destino, tokens[0], string.Empty,
                        string.Format("{0} = {1}", asignacion.Destino, tokens[0])));
                    continue;
                }

                var postfix = ConvertirPostfija(tokens);
                var pila = new Stack<string>();
                var referencias = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);

                foreach (var token in postfix)
                {
                    if (EsOperadorBinario(token))
                    {
                        var b = pila.Pop();
                        var a = pila.Pop();

                        var arg1 = (a.StartsWith("t") && referencias.ContainsKey(a)) ? "(" + referencias[a] + ")" : a;
                        var arg2 = (b.StartsWith("t") && referencias.ContainsKey(b)) ? "(" + referencias[b] + ")" : b;

                        var indice = resultado.Count;
                        resultado.Add(new TripleFila(indice, token, arg1, arg2, string.Empty,
                            string.Format("({0}) = {1} {2} {3}", indice, arg1, token, arg2)));

                        var temporales = "t" + (referencias.Count + 1);
                        referencias[temporales] = indice;
                        pila.Push(temporales);
                    }
                    else
                    {
                        pila.Push(token);
                    }
                }

                if (pila.Count > 0)
                {
                    var valor = pila.Pop();
                    var argValor = (valor.StartsWith("t") && referencias.ContainsKey(valor)) ? "(" + referencias[valor] + ")" : valor;
                    if (!string.Equals(argValor, asignacion.Destino, StringComparison.OrdinalIgnoreCase))
                    {
                        resultado.Add(new TripleFila(resultado.Count, "=", asignacion.Destino, argValor, string.Empty,
                            string.Format("{0} = {1}", asignacion.Destino, argValor)));
                    }
                }
            }

            foreach (var bloque in ObtenerBloquesSi(fuente))
            {
                int saltoDestino = resultado.Count + bloque.Cuerpo.Count + 1;
                resultado.Add(new TripleFila(resultado.Count, "fjp", bloque.Condicion, "(" + saltoDestino + ")", string.Empty,
                    string.Format("if_false {0} goto ({1})", bloque.Condicion, saltoDestino)));

                foreach (var linea in bloque.Cuerpo)
                {
                    var match = Regex.Match(linea, @"^MOSTRAR\s+(.+?)\s*;?\s*$", RegexOptions.IgnoreCase);
                    if (!match.Success)
                        continue;

                    var variable = match.Groups[1].Value.Trim();
                    resultado.Add(new TripleFila(resultado.Count, "MOSTRAR", variable, string.Empty, string.Empty,
                        "MOSTRAR " + variable));
                }

                resultado.Add(new TripleFila(resultado.Count, "lab", "L1", string.Empty, string.Empty, "lab L1"));
            }

            foreach (var mostrar in ObtenerMostrarsFueraDeSi(fuente))
            {
                resultado.Add(new TripleFila(resultado.Count, "MOSTRAR", mostrar, string.Empty, string.Empty,
                    "MOSTRAR " + mostrar));
            }

            return resultado;
        }

        private static List<CuadruploFila> GenerarCuadruplosDesdeFuente(string fuente)
        {
            var resultado = new List<CuadruploFila>();
            var asignaciones = ObtenerAsignaciones(fuente);

            foreach (var asignacion in asignaciones)
            {
                var tokens = ObtenerTokensExpresion(asignacion.Expresion);
                if (tokens.Count == 0)
                    continue;

                if (tokens.Count == 1 && !EsOperadorBinario(tokens[0]))
                {
                    resultado.Add(new CuadruploFila(resultado.Count, "=", tokens[0], string.Empty, asignacion.Destino,
                        string.Format("{0} = {1}", asignacion.Destino, tokens[0])));
                    continue;
                }

                var postfix = ConvertirPostfija(tokens);
                var pila = new Stack<string>();
                var tempCounter = 1;

                foreach (var token in postfix)
                {
                    if (EsOperadorBinario(token))
                    {
                        var b = pila.Pop();
                        var a = pila.Pop();
                        var temp = "t" + tempCounter++;

                        var indice = resultado.Count;
                        resultado.Add(new CuadruploFila(indice, token, a, b, temp,
                            string.Format("{0} = {1} {2} {3}", temp, a, token, b)));

                        pila.Push(temp);
                    }
                    else
                    {
                        pila.Push(token);
                    }
                }

                if (pila.Count > 0)
                {
                    var valor = pila.Pop();
                    resultado.Add(new CuadruploFila(resultado.Count, "=", valor, string.Empty, asignacion.Destino,
                        string.Format("{0} = {1}", asignacion.Destino, valor)));
                }
            }

            foreach (var bloque in ObtenerBloquesSi(fuente))
            {
                resultado.Add(new CuadruploFila(resultado.Count, "fjp", bloque.Condicion, string.Empty, "L1",
                    string.Format("if_false {0} goto L1", bloque.Condicion)));

                foreach (var linea in bloque.Cuerpo)
                {
                    var match = Regex.Match(linea, @"^MOSTRAR\s+(.+?)\s*;?\s*$", RegexOptions.IgnoreCase);
                    if (!match.Success)
                        continue;

                    var variable = match.Groups[1].Value.Trim();
                    resultado.Add(new CuadruploFila(resultado.Count, "MOSTRAR", variable, string.Empty, string.Empty,
                        "MOSTRAR " + variable));
                }

                resultado.Add(new CuadruploFila(resultado.Count, "lab", "L1", string.Empty, string.Empty, "lab L1"));
            }

            foreach (var mostrar in ObtenerMostrarsFueraDeSi(fuente))
            {
                resultado.Add(new CuadruploFila(resultado.Count, "MOSTRAR", mostrar, string.Empty, string.Empty,
                    "MOSTRAR " + mostrar));
            }

            return resultado;
        }

        private static List<PrefijaFila> GenerarNotacionPrefijaDesdeFuente(string fuente)
        {
            var resultado = new List<PrefijaFila>();
            var asignaciones = ObtenerAsignaciones(fuente);

            foreach (var asignacion in asignaciones)
            {
                var tokens = ObtenerTokensExpresion(asignacion.Expresion);
                if (tokens.Count == 0)
                    continue;

                var prefijaBase = ConvertirPrefijaExpresion(tokens);
                var prefijaTokens = new List<string> { "=", asignacion.Destino };
                prefijaTokens.AddRange(prefijaBase.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries));

                var expresionCompleta = string.Join(" ", prefijaTokens);
                var pila = new Stack<string>();
                foreach (var token in prefijaTokens)
                {
                    pila.Push(token);
                    var pilaActual = new Stack<string>(pila.Reverse());
                    resultado.Add(new PrefijaFila(token, "PUSH " + token, FormatearPila(pilaActual), expresionCompleta, expresionCompleta));
                }
            }

            foreach (var bloque in ObtenerBloquesSi(fuente))
            {
                resultado.Add(new PrefijaFila(bloque.Condicion, "PUSH " + bloque.Condicion, "[" + bloque.Condicion + "]",
                    "Condición = " + bloque.Condicion, bloque.Condicion));
                resultado.Add(new PrefijaFila("FJP", "POP " + bloque.Condicion + ", EVAL condición, JUMP L1", "[]",
                    "FJP L1", "FJP L1"));

                foreach (var linea in bloque.Cuerpo)
                {
                    var match = Regex.Match(linea, @"^MOSTRAR\s+(.+?)\s*;?\s*$", RegexOptions.IgnoreCase);
                    if (!match.Success)
                        continue;

                    var variable = match.Groups[1].Value.Trim();
                    resultado.Add(new PrefijaFila(variable, "PUSH " + variable, "[" + variable + "]",
                        "MOSTRAR " + variable, "MOSTRAR " + variable));
                }

                resultado.Add(new PrefijaFila("LAB", "LABEL L1", "[]", "LAB L1", "LAB L1"));
            }

            foreach (var mostrar in ObtenerMostrarsFueraDeSi(fuente))
            {
                resultado.Add(new PrefijaFila("MOSTRAR", "PUSH " + mostrar, "[]", mostrar, "MOSTRAR " + mostrar));
            }

            return resultado;
        }

        private static List<PostfijaFila> GenerarNotacionPostfijaDesdeFuente(string fuente)
        {
            var resultado = new List<PostfijaFila>();
            var asignaciones = ObtenerAsignaciones(fuente);

            foreach (var asignacion in asignaciones)
            {
                var tokens = ObtenerTokensExpresion(asignacion.Expresion);
                if (tokens.Count == 0)
                    continue;

                var postfixBase = ConvertirPostfija(tokens);
                var postfixTokens = new List<string> { asignacion.Destino };
                postfixTokens.AddRange(postfixBase);
                postfixTokens.Add("=");

                var pila = new Stack<string>();
                var expresionCompleta = string.Join(" ", postfixTokens);
                foreach (var token in postfixTokens)
                {
                    var pilaActual = new Stack<string>(pila.Reverse());

                    if (EsOperadorBinario(token))
                    {
                        var b = pila.Pop();
                        var a = pila.Pop();
                        var res = a + " " + token + " " + b;
                        pila.Push(res);
                        resultado.Add(new PostfijaFila(token, "POP " + b + ", POP " + a + ", EVAL " + a + " " + token + " " + b + ", PUSH " + res,
                            FormatearPila(new Stack<string>(pila.Reverse())), expresionCompleta, expresionCompleta));
                    }
                    else if (token == "=")
                    {
                        var valor = pila.Pop();
                        var destino = pila.Pop();
                        resultado.Add(new PostfijaFila(token, "POP " + valor + ", POP " + destino + ", ASIGNAR " + destino + " = " + valor,
                            FormatearPila(new Stack<string>(pila.Reverse())), expresionCompleta, expresionCompleta));
                    }
                    else
                    {
                        pila.Push(token);
                        resultado.Add(new PostfijaFila(token, "PUSH " + token, FormatearPila(new Stack<string>(pila.Reverse())), expresionCompleta, expresionCompleta));
                    }
                }
            }

            foreach (var bloque in ObtenerBloquesSi(fuente))
            {
                resultado.Add(new PostfijaFila(bloque.Condicion, "PUSH " + bloque.Condicion, "[" + bloque.Condicion + "]",
                    "Condición = " + bloque.Condicion, bloque.Condicion));
                resultado.Add(new PostfijaFila("FJP", "POP " + bloque.Condicion + ", EVAL condición, JUMP L1", "[]",
                    "FJP L1", "FJP L1"));

                foreach (var linea in bloque.Cuerpo)
                {
                    var match = Regex.Match(linea, @"^MOSTRAR\s+(.+?)\s*;?\s*$", RegexOptions.IgnoreCase);
                    if (!match.Success)
                        continue;

                    var variable = match.Groups[1].Value.Trim();
                    resultado.Add(new PostfijaFila(variable, "PUSH " + variable, "[" + variable + "]",
                        "MOSTRAR " + variable, "MOSTRAR " + variable));
                }

                resultado.Add(new PostfijaFila("LAB", "LABEL L1", "[]", "LAB L1", "LAB L1"));
            }

            foreach (var mostrar in ObtenerMostrarsFueraDeSi(fuente))
            {
                resultado.Add(new PostfijaFila("MOSTRAR", "PUSH " + mostrar, "[]", mostrar, "MOSTRAR " + mostrar));
            }

            return resultado;
        }

        private static List<TrazasPilaFila> GenerarTrazaPila(IList<string> expresion, string tipo)
        {
            var resultado = new List<TrazasPilaFila>();
            var pila = new Stack<string>();
            var temporal = 1;

            foreach (var token in expresion)
            {
                if (EsOperadorBinario(token))
                {
                    var b = pila.Pop();
                    var a = pila.Pop();
                    var temp = "t" + temporal++;
                    var nuevaPila = new Stack<string>(pila.Reverse());
                    nuevaPila.Push(temp);

                    resultado.Add(new TrazasPilaFila(token, "POP " + b + ", POP " + a + ", EVAL " + a + " " + token + " " + b + ", PUSH " + temp,
                        FormatearPila(nuevaPila), temp + " = " + a + " " + token + " " + b, temp + " = " + a + " " + token + " " + b));

                    pila.Push(temp);
                }
                else
                {
                    pila.Push(token);
                    var pilaActual = new Stack<string>(pila.Reverse());
                    resultado.Add(new TrazasPilaFila(token, "PUSH " + token, FormatearPila(pilaActual), token, token));
                }
            }

            return resultado;
        }

        private static string FormatearPila(Stack<string> pila)
        {
            if (pila == null || pila.Count == 0)
                return "[]";

            return "[" + string.Join(", ", pila.Reverse().ToArray()) + "]";
        }

        private sealed class TrazasPilaFila
        {
            public TrazasPilaFila(string simbolo, string accion, string pila, string calculos, string resultado)
            {
                Simbolo = simbolo;
                Accion = accion;
                Pila = pila;
                Calculos = calculos;
                Resultado = resultado;
            }

            public string Simbolo { get; }
            public string Accion { get; }
            public string Pila { get; }
            public string Calculos { get; }
            public string Resultado { get; }
        }

        private static List<SIBlock> ObtenerBloquesSi(string fuente)
        {
            var resultado = new List<SIBlock>();
            if (string.IsNullOrWhiteSpace(fuente))
                return resultado;

            var lineas = fuente.Split('\n');
            for (int i = 0; i < lineas.Length; i++)
            {
                var linea = lineas[i].Trim();
                if (string.IsNullOrWhiteSpace(linea))
                    continue;

                var siMatch = Regex.Match(linea, @"^SI\s*\(\s*(.+?)\s*\)\s*ENTONCES\s*$", RegexOptions.IgnoreCase);
                if (!siMatch.Success)
                    continue;

                var condicion = siMatch.Groups[1].Value.Trim();
                var cuerpo = new List<string>();
                int j = i + 1;
                while (j < lineas.Length)
                {
                    var lineaCuerpo = lineas[j].Trim();
                    if (string.IsNullOrWhiteSpace(lineaCuerpo))
                    {
                        j++;
                        continue;
                    }

                    if (lineaCuerpo.Equals("FIN", StringComparison.OrdinalIgnoreCase) ||
                        lineaCuerpo.Equals("FIN ;", StringComparison.OrdinalIgnoreCase) ||
                        lineaCuerpo.Equals("FIN;", StringComparison.OrdinalIgnoreCase))
                    {
                        break;
                    }

                    cuerpo.Add(lineaCuerpo);
                    j++;
                }

                resultado.Add(new SIBlock(condicion, cuerpo));
                i = j;
            }

            return resultado;
        }

        private sealed class SIBlock
        {
            public SIBlock(string condicion, List<string> cuerpo)
            {
                Condicion = condicion;
                Cuerpo = cuerpo ?? new List<string>();
            }

            public string Condicion { get; }
            public List<string> Cuerpo { get; }
        }

        private static List<string> ObtenerSentencias(string fuente)
        {
            return fuente
                .Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries)
                .Select(s => s.Trim())
                .Where(s => !string.IsNullOrWhiteSpace(s))
                .ToList();
        }

        private static List<string> ObtenerTokensExpresion(string expresion)
        {
            var tokens = new List<string>();
            if (string.IsNullOrWhiteSpace(expresion))
                return tokens;

            var actual = new System.Text.StringBuilder();

            for (int i = 0; i < expresion.Length; i++)
            {
                char c = expresion[i];

                if (char.IsWhiteSpace(c))
                {
                    if (actual.Length > 0)
                    {
                        tokens.Add(actual.ToString());
                        actual.Clear();
                    }
                    continue;
                }

                if (char.IsDigit(c) || char.IsLetter(c) || c == '_' || c == '.')
                {
                    actual.Append(c);
                    while (i + 1 < expresion.Length &&
                           (char.IsDigit(expresion[i + 1]) || char.IsLetter(expresion[i + 1]) || expresion[i + 1] == '_' || expresion[i + 1] == '.'))
                    {
                        i++;
                        actual.Append(expresion[i]);
                    }

                    tokens.Add(actual.ToString());
                    actual.Clear();
                    continue;
                }

                if (c == '(' || c == ')' || c == '+' || c == '-' || c == '*' || c == '/')
                {
                    tokens.Add(c.ToString());
                }
            }

            return tokens;
        }

        private static bool EsOperadorBinario(string token)
        {
            return token == "+" || token == "-" || token == "*" || token == "/";
        }

        private static int ObtenerPrecedencia(string token)
        {
            switch (token)
            {
                case "+":
                case "-": return 1;
                case "*":
                case "/": return 2;
                default: return 0;
            }
        }

        private static List<string> ConvertirPostfija(IList<string> tokens)
        {
            var resultado = new List<string>();
            var pila = new Stack<string>();

            foreach (var token in tokens)
            {
                if (EsOperadorBinario(token))
                {
                    while (pila.Count > 0 && pila.Peek() != "(" && ObtenerPrecedencia(pila.Peek()) >= ObtenerPrecedencia(token))
                    {
                        resultado.Add(pila.Pop());
                    }
                    pila.Push(token);
                }
                else if (token == "(")
                {
                    pila.Push(token);
                }
                else if (token == ")")
                {
                    while (pila.Count > 0 && pila.Peek() != "(")
                    {
                        resultado.Add(pila.Pop());
                    }

                    if (pila.Count > 0)
                        pila.Pop();
                }
                else
                {
                    resultado.Add(token);
                }
            }

            while (pila.Count > 0)
            {
                resultado.Add(pila.Pop());
            }

            return resultado;
        }

        private static string ConvertirPrefijaExpresion(IList<string> tokens)
        {
            var postfix = ConvertirPostfija(tokens);
            var pila = new Stack<string>();

            foreach (var token in postfix)
            {
                if (EsOperadorBinario(token))
                {
                    var b = pila.Pop();
                    var a = pila.Pop();
                    pila.Push(token + " " + a + " " + b);
                }
                else
                {
                    pila.Push(token);
                }
            }

            return pila.Count > 0 ? pila.Pop() : string.Empty;
        }

        private sealed class AsignacionFila
        {
            public AsignacionFila(string destino, string expresion)
            {
                Destino = destino;
                Expresion = expresion;
            }

            public string Destino { get; }
            public string Expresion { get; }
        }

        private sealed class TripleFila
        {
            public TripleFila(int indice, string op, string arg1, string arg2, string temporal, string representacion)
            {
                Indice = indice;
                Op = op;
                Arg1 = arg1;
                Arg2 = arg2;
                Temporal = temporal;
                Representacion = representacion;
            }

            public int Indice { get; }
            public string Op { get; }
            public string Arg1 { get; }
            public string Arg2 { get; }
            public string Temporal { get; }
            public string Representacion { get; }
        }

        private sealed class CuadruploFila
        {
            public CuadruploFila(int indice, string op, string arg1, string arg2, string resultado, string representacion)
            {
                Indice = indice;
                Op = op;
                Arg1 = arg1;
                Arg2 = arg2;
                Resultado = resultado;
                Representacion = representacion;
            }

            public int Indice { get; }
            public string Op { get; }
            public string Arg1 { get; }
            public string Arg2 { get; }
            public string Resultado { get; }
            public string Representacion { get; }
        }

        private sealed class PrefijaFila
        {
            public PrefijaFila(string simbolo, string accion, string pila, string calculos, string resultado)
            {
                Simbolo = simbolo;
                Accion = accion;
                Pila = pila;
                Calculos = calculos;
                Resultado = resultado;
            }

            public string Simbolo { get; }
            public string Accion { get; }
            public string Pila { get; }
            public string Calculos { get; }
            public string Resultado { get; }
        }

        private sealed class PostfijaFila
        {
            public PostfijaFila(string simbolo, string accion, string pila, string calculos, string resultado)
            {
                Simbolo = simbolo;
                Accion = accion;
                Pila = pila;
                Calculos = calculos;
                Resultado = resultado;
            }

            public string Simbolo { get; }
            public string Accion { get; }
            public string Pila { get; }
            public string Calculos { get; }
            public string Resultado { get; }
        }

        private void CodigoIntermedio_Load(object sender, EventArgs e)
        {
        }
    }
}
