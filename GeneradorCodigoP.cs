using System;
using System.Collections.Generic;

namespace AnalizadorLexico
{
    public class GeneradorCodigoP
    {
        private readonly List<string> instrucciones = new List<string>();
        private int etiqueta;

        public string Generar(IList<Token> tokens)
        {
            instrucciones.Clear();
            etiqueta = 0;

            if (tokens == null || tokens.Count == 0)
                return string.Empty;

            EmitirBloque(tokens, 0, tokens.Count);

            while (instrucciones.Count > 0 &&
                   string.IsNullOrEmpty(instrucciones[instrucciones.Count - 1]))
                instrucciones.RemoveAt(instrucciones.Count - 1);

            instrucciones.Add(string.Empty);
            instrucciones.Add("stp");
            return string.Join(Environment.NewLine, instrucciones);
        }

        private int EmitirBloque(IList<Token> tokens, int inicio, int fin)
        {
            int indice = inicio;
            while (indice < fin)
            {
                Token token = tokens[indice];
                if (token == null || token.EsError)
                {
                    indice++;
                    continue;
                }

                if (EsCategoria(token, "INICIO", "PR1"))
                {
                    indice++;
                    continue;
                }

                if (EsCategoria(token, "FIN", "PR2"))
                {
                    indice++;
                    continue;
                }

                if (EsCategoria(token, "SI", "PR5"))
                {
                    indice = EmitirSi(tokens, indice, fin);
                    continue;
                }

                if (EsCategoria(token, "REPETIR", "PR10"))
                {
                    indice = EmitirRepetir(tokens, indice, fin);
                    continue;
                }

                if (EsCategoria(token, "MIENTRAS", "PR14"))
                {
                    indice = EmitirMientras(tokens, indice, fin);
                    continue;
                }

                if (EsCategoria(token, "VAR", "PR20"))
                {
                    indice = AvanzarHastaDelimitador(tokens, indice + 1);
                    continue;
                }

                if (EsCategoria(token, "LEER", "PR3"))
                {
                    Token variable = SiguienteToken(tokens, indice + 1);
                    if (variable != null && variable.Categoria == "IDENT")
                    {
                        instrucciones.Add("lda " + variable.Lexema);
                        instrucciones.Add("rdi");
                        instrucciones.Add("sto");
                    }
                    indice = AvanzarHastaDelimitador(tokens, indice + 1);
                    continue;
                }

                if (EsCategoria(token, "MOSTRAR", "PR4") ||
                    EsCategoria(token, "ESCRIBIR", "PR17"))
                {
                    List<Token> expresion = HastaDelimitador(tokens, indice + 1);
                    EmitirExpresion(expresion);
                    instrucciones.Add("wri");
                    instrucciones.Add(string.Empty);
                    indice = AvanzarHastaDelimitador(tokens, indice + 1);
                    continue;
                }

                if (token.Categoria == "IDENT" &&
                    EsCategoria(SiguienteToken(tokens, indice + 1), "ASI"))
                {
                    List<Token> expresion = HastaDelimitador(tokens, indice + 2);
                    instrucciones.Add("lda " + token.Lexema);
                    EmitirExpresion(expresion);
                    instrucciones.Add("sto");
                    instrucciones.Add(string.Empty);
                    indice = AvanzarHastaDelimitador(tokens, indice + 2);
                    continue;
                }

                indice++;
            }

            return indice;
        }

        private int EmitirSi(IList<Token> tokens, int inicio, int fin)
        {
            int condicionInicio = SiguienteIndice(tokens, inicio + 1, "CE7");
            if (condicionInicio < 0)
                return inicio + 1;

            int condicionFin = EncontrarCierreParentesis(tokens, condicionInicio, fin);
            if (condicionFin < 0)
                return inicio + 1;

            List<Token> condicion = Sublista(tokens, condicionInicio + 1, condicionFin);
            EmitirExpresion(condicion);

            int entonces = SiguienteIndice(tokens, condicionFin + 1, "PR6");
            if (entonces < 0)
                return condicionFin + 1;

            int finBloque = EncontrarFinBloque(tokens, entonces + 1, fin);
            int sino = EncontrarSino(tokens, entonces + 1, finBloque);
            string etiquetaFalso = NuevaEtiqueta();
            string etiquetaFin = sino >= 0 ? NuevaEtiqueta() : null;

            instrucciones.Add("fjp " + etiquetaFalso);
            EmitirBloque(tokens, entonces + 1, sino >= 0 ? sino : finBloque);

            if (sino >= 0)
            {
                instrucciones.Add("ujp " + etiquetaFin);
                instrucciones.Add(etiquetaFalso + ":");
                EmitirBloque(tokens, sino + 1, finBloque);
                instrucciones.Add(etiquetaFin + ":");
            }
            else
            {
                instrucciones.Add(etiquetaFalso + ":");
            }

            return finBloque < fin ? finBloque + 1 : finBloque;
        }

        private int EmitirRepetir(IList<Token> tokens, int inicio, int fin)
        {
            int finBloque = EncontrarFinBloque(tokens, inicio + 1, fin);
            if (finBloque < 0)
                return inicio + 1;

            int hasta = EncontrarCategoria(tokens, inicio + 1, finBloque, "PR9");
            string etiquetaInicio = NuevaEtiqueta();
            instrucciones.Add(etiquetaInicio + ":");

            if (hasta >= 0)
            {
                EmitirBloque(tokens, inicio + 1, hasta);
                List<Token> condicion = HastaDelimitador(tokens, hasta + 1);
                EmitirExpresion(condicion);
                instrucciones.Add("fjp " + etiquetaInicio);
                return AvanzarHastaDelimitador(tokens, hasta + 1);
            }

            EmitirBloque(tokens, inicio + 1, finBloque);
            return finBloque + 1;
        }

        private int EmitirMientras(IList<Token> tokens, int inicio, int fin)
        {
            int hacer = SiguienteIndice(tokens, inicio + 1, "PR13");
            if (hacer < 0)
                return inicio + 1;

            int finBloque = EncontrarFinBloque(tokens, hacer + 1, fin);
            if (finBloque < 0)
                return hacer + 1;

            List<Token> condicion = Sublista(tokens, inicio + 1, hacer);
            string etiquetaInicio = NuevaEtiqueta();
            string etiquetaFin = NuevaEtiqueta();

            instrucciones.Add(etiquetaInicio + ":");
            EmitirExpresion(condicion);
            instrucciones.Add("fjp " + etiquetaFin);
            EmitirBloque(tokens, hacer + 1, finBloque);
            instrucciones.Add("ujp " + etiquetaInicio);
            instrucciones.Add(etiquetaFin + ":");
            return finBloque + 1;
        }

        private void EmitirExpresion(IList<Token> expresion)
        {
            List<Token> postfija = ConvertirAPostfija(expresion);

            foreach (Token token in postfija)
            {
                string operacion = Operacion(token);
                if (operacion != null)
                    instrucciones.Add(operacion);
                else if (token.Categoria == "IDENT")
                    instrucciones.Add("lod " + token.Lexema);
                else
                    instrucciones.Add("ldc " + token.Lexema);
            }
        }

        private List<Token> ConvertirAPostfija(IList<Token> expresion)
        {
            var resultado = new List<Token>();
            var operadores = new Stack<Token>();

            foreach (Token token in expresion)
            {
                if (token == null || token.EsError)
                    continue;

                if (token.Categoria == "CE7")
                {
                    operadores.Push(token);
                    continue;
                }

                if (token.Categoria == "CE8")
                {
                    while (operadores.Count > 0 &&
                           operadores.Peek().Categoria != "CE7")
                        resultado.Add(operadores.Pop());

                    if (operadores.Count > 0)
                        operadores.Pop();
                    continue;
                }

                string operacion = Operacion(token);
                if (operacion == null)
                {
                    resultado.Add(token);
                }
                else
                {
                    while (operadores.Count > 0 &&
                           operadores.Peek().Categoria != "CE7" &&
                           Precedencia(operadores.Peek()) >= Precedencia(token))
                        resultado.Add(operadores.Pop());

                    operadores.Push(token);
                }
            }

            while (operadores.Count > 0)
                resultado.Add(operadores.Pop());

            return resultado;
        }

        private List<Token> HastaDelimitador(IList<Token> tokens, int inicio)
        {
            var resultado = new List<Token>();
            int parentesis = 0;

            for (int i = inicio; i < tokens.Count; i++)
            {
                Token token = tokens[i];
                if (token == null)
                    continue;

                if (token.Categoria == "CE7")
                    parentesis++;
                else if (token.Categoria == "CE8")
                    parentesis--;

                if (parentesis == 0 &&
                    (token.Categoria == "DEL" || token.Categoria == "CE13"))
                    break;

                resultado.Add(token);
            }

            return resultado;
        }

            private List<Token> Sublista(IList<Token> tokens, int inicio, int fin)
            {
                var resultado = new List<Token>();
                for (int i = inicio; i < fin && i < tokens.Count; i++)
                    resultado.Add(tokens[i]);
                return resultado;
            }

            private int EncontrarCategoria(IList<Token> tokens, int inicio, int fin, string categoria)
            {
                int profundidad = 0;
                for (int i = inicio; i < fin; i++)
                {
                    if (tokens[i].Categoria == "SI" ||
                        tokens[i].Categoria == "REPETIR" ||
                        tokens[i].Categoria == "MIENTRAS")
                        profundidad++;
                    else if (tokens[i].Categoria == "FIN" && profundidad > 0)
                        profundidad--;
                    else if (tokens[i].Categoria == categoria && profundidad == 0)
                        return i;
                }
                return -1;
            }

            private int EncontrarSino(IList<Token> tokens, int inicio, int fin)
            {
                return EncontrarCategoria(tokens, inicio, fin, "PR7");
            }

            private int EncontrarFinBloque(IList<Token> tokens, int inicio, int fin)
            {
                int profundidad = 0;
                for (int i = inicio; i < fin; i++)
                {
                    string categoria = tokens[i].Categoria;
                    if (categoria == "SI" || categoria == "REPETIR" ||
                        categoria == "MIENTRAS")
                        profundidad++;
                    else if (categoria == "FIN")
                    {
                        if (profundidad == 0)
                            return i;
                        profundidad--;
                    }
                }
                return -1;
            }

            private int SiguienteIndice(IList<Token> tokens, int inicio, string categoria)
            {
                for (int i = inicio; i < tokens.Count; i++)
                {
                    if (tokens[i].Categoria == categoria)
                        return i;
                }
                return -1;
            }

            private int EncontrarCierreParentesis(IList<Token> tokens, int inicio, int fin)
            {
                int profundidad = 0;
                for (int i = inicio; i < fin; i++)
                {
                    if (tokens[i].Categoria == "CE7")
                        profundidad++;
                    else if (tokens[i].Categoria == "CE8")
                    {
                        profundidad--;
                        if (profundidad == 0)
                            return i;
                    }
                }
                return -1;
            }

            private string NuevaEtiqueta()
            {
                etiqueta++;
                return "L" + etiqueta;
            }

            private int AvanzarHastaDelimitador(IList<Token> tokens, int inicio)
        {
            int indice = inicio;
            while (indice < tokens.Count &&
                   tokens[indice].Categoria != "DEL" &&
                   tokens[indice].Categoria != "CE13")
                indice++;
            return indice + 1;
        }

        private Token SiguienteToken(IList<Token> tokens, int indice)
        {
            return indice >= 0 && indice < tokens.Count ? tokens[indice] : null;
        }

        private bool EsCategoria(Token token, params string[] categorias)
        {
            if (token == null)
                return false;

            foreach (string categoria in categorias)
            {
                if (token.Categoria == categoria)
                    return true;
            }

            return false;
        }

        private string Operacion(Token token)
        {
            switch (token.Categoria)
            {
                case "OA1": return "adi";
                case "OA2": return "sbi";
                case "OA3": return "mli";
                case "OA4": return "dvi";
                case "OR1": return "gri";
                case "OR2": return "geq";
                case "OR3": return "les";
                case "OR4": return "leq";
                case "OR5": return "neq";
                case "OR6": return "eql";
                case "ASI": return "eql";
                case "OPL1": return "and";
                case "OPL2": return "orr";
                case "OPL3": return "not";
                default: return null;
            }
        }

        private int Precedencia(Token token)
        {
            switch (token.Categoria)
            {
                case "OPL1":
                case "OPL2": return 1;
                case "OR1":
                case "OR2":
                case "OR3":
                case "OR4":
                case "OR5":
                case "OR6": return 2;
                case "OA1":
                case "OA2": return 3;
                case "OA3":
                case "OA4": return 4;
                case "OPL3": return 5;
                default: return 0;
            }
        }
    }
}
