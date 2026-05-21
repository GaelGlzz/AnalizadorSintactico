using System;
using System.Collections.Generic;
using System.Linq;

namespace AnalizadorLexico
{
    public class Token
    {
        public int Estado { get; set; }
        public string Categoria { get; set; }
        public string Lexema { get; set; }
        public bool EsError { get; set; }

        public override string ToString()
        {
            return $"Token(Estado: {Estado}, Categoría: {Categoria}, Lexema: '{Lexema}'))";
        }
    }

    public class Lexer
    {
        // Palabras reservadas y sus categorías
        private static readonly Dictionary<string, string> RESERVED_WORDS = new Dictionary<string, string>()
        {
            { "desde", "DESDE" },
            { "entonces", "ENTONCES" },
            { "escribir", "ESCRIBIR" },
            { "fin", "FIN" },
            { "funcion", "FUNCION" },
            { "hacer", "HACER" },
            { "hasta", "HASTA" },
            { "inicio", "INICIO" },
            { "ir", "IR" },
            { "leer", "LEER" },
            { "limp", "LIMP" },
            { "mientras", "MIENTRAS" },
            { "mostrar", "MOSTRAR" },
            { "no", "NO" },
            { "nuevo", "NUEVO" },
            { "o", "O" },
            { "para", "PARA" },
            { "repetir", "REPETIR" },
            { "retornar", "RETORNAR" },
            { "romper", "ROMPER" },
            { "si", "SI" },
            { "sino", "SINO" },
            { "var", "VAR" },
            { "y", "Y" }
        };

        /// <summary>
        /// Analiza una cadena de entrada y retorna un token
        /// </summary>
        public Token Analyze(string input)
        {
            if (string.IsNullOrEmpty(input))
                return new Token { Estado = 0, Categoria = "ERROR", Lexema = "", EsError = true };

            string trimmedInput = input.Trim();
            
            // Primero verificar si es una palabra reservada (deben estar en MAYÚSCULAS)
            string lowerInput = trimmedInput.ToLower();
            if (RESERVED_WORDS.ContainsKey(lowerInput))
            {
                // Verificar que esté completamente en mayúsculas
                if (trimmedInput == trimmedInput.ToUpper())
                {
                    // Retornar con el estado correcto según la palabra
                    int estado = GetEstadoPalabraReservada(lowerInput);
                    return new Token
                    {
                        Estado = estado,
                        Categoria = RESERVED_WORDS[lowerInput],
                        Lexema = trimmedInput,
                        EsError = false
                    };
                }
                else
                {
                    // Si es una palabra reservada pero no está en mayúsculas, es error
                    return new Token
                    {
                        Estado = 218,
                        Categoria = "ERROR: Palabra reservada no válida",
                        Lexema = trimmedInput,
                        EsError = true
                    };
                }
            }

            // Verificar si parece ser un identificador (comienza con 'p')
            if (trimmedInput.Length >= 1 && trimmedInput[0] == 'p')
            {
                if (IsIdentifier(trimmedInput))
                {
                    return new Token
                    {
                        Estado = 153,
                        Categoria = "IDENT",
                        Lexema = trimmedInput,
                        EsError = false
                    };
                }
                else
                {
                    // Es un identificador inválido
                    return new Token
                    {
                        Estado = 214,
                        Categoria = "ERROR: Identificador no válido",
                        Lexema = trimmedInput,
                        EsError = true
                    };
                }
            }

            // Verificar si es un comentario (comienza con #)
            if (IsComment(trimmedInput))
            {
                return new Token
                {
                    Estado = 139,
                    Categoria = "COMEN",
                    Lexema = trimmedInput,
                    EsError = false
                };
            }

            // Verificar si es un número con exponente (ANTES que número real)
            if (IsExponential(trimmedInput))
            {
                return new Token
                {
                    Estado = 137,
                    Categoria = "CN CON EXPONENTE",
                    Lexema = trimmedInput,
                    EsError = false
                };
            }

            // Verificar si es un número real
            if (IsReal(trimmedInput))
            {
                return new Token
                {
                    Estado = 133,
                    Categoria = "CN REALES",
                    Lexema = trimmedInput,
                    EsError = false
                };
            }

            // Verificar si parece ser un número (contiene dígitos)
            if (ContainsDigits(trimmedInput))
            {
                // Podría ser un número entero o inválido
                if (IsInteger(trimmedInput))
                {
                    return new Token
                    {
                        Estado = 129,
                        Categoria = "CN ENTEROS",
                        Lexema = trimmedInput,
                        EsError = false
                    };
                }
                else
                {
                    // Es un número inválido
                    if (trimmedInput.Contains(".") || trimmedInput.Contains(","))
                    {
                        return new Token
                        {
                            Estado = 216,
                            Categoria = "ERROR: Constante numérica real no válida",
                            Lexema = trimmedInput,
                            EsError = true
                        };
                    }
                    else if (trimmedInput.Contains("e") || trimmedInput.Contains("E"))
                    {
                        return new Token
                        {
                            Estado = 217,
                            Categoria = "ERROR: Constante numérica exponencial no válida",
                            Lexema = trimmedInput,
                            EsError = true
                        };
                    }
                    else
                    {
                        return new Token
                        {
                            Estado = 215,
                            Categoria = "ERROR: Constante numérica entera no válida",
                            Lexema = trimmedInput,
                            EsError = true
                        };
                    }
                }
            }

            // Verificar si es una cadena
            if (IsString(trimmedInput))
            {
                return new Token
                {
                    Estado = 142,
                    Categoria = "CAD",
                    Lexema = trimmedInput,
                    EsError = false
                };
            }

            // Verificar si es un operador o símbolo
            Token operatorToken = AnalyzeOperator(trimmedInput);
            if (operatorToken != null)
                return operatorToken;

            // No reconocido
            return new Token
            {
                Estado = 223,
                Categoria = "ERROR: Carácter especial no válido",
                Lexema = trimmedInput,
                EsError = true
            };
        }

        private int GetEstadoPalabraReservada(string palabra)
        {
            switch (palabra)
            {
                case "desde": return 7;
                case "entonces": return 16;
                case "escribir": return 24;
                case "fin": return 28;
                case "funcion": return 35;
                case "hacer": return 41;
                case "hasta": return 45;
                case "inicio": return 52;
                case "ir": return 54;
                case "leer": return 59;
                case "limp": return 63;
                case "mientras": return 72;
                case "mostrar": return 79;
                case "no": return 82;
                case "nuevo": return 87;
                case "o": return 89;
                case "para": return 94;
                case "repetir": return 102;
                case "retornar": return 109;
                case "romper": return 115;
                case "si": return 118;
                case "sino": return 121;
                case "var": return 125;
                case "y": return 127;
                default: return 1;
            }
        }

        private bool ContainsDigits(string input)
        {
            return input.Any(char.IsDigit);
        }

        private bool IsIdentifier(string input)
        {
            if (string.IsNullOrEmpty(input))
                return false;

            // Debe comenzar con 'p' minúscula
            if (input[0] != 'p')
                return false;

            // Debe tener al menos 2 caracteres (p + una letra mayúscula)
            if (input.Length < 2)
                return false;

            // El segundo carácter debe ser una letra mayúscula
            if (!char.IsUpper(input[1]))
                return false;

            // El resto puede ser solo letras, dígitos o guion bajo
            for (int i = 2; i < input.Length; i++)
            {
                char c = input[i];
                if (!char.IsLetterOrDigit(c) && c != '_')
                    return false;
            }

            return true;
        }

        private bool IsInteger(string input)
        {
            if (string.IsNullOrEmpty(input))
                return false;

            if (input[0] == '-' || input[0] == '+')
            {
                if (input.Length == 1)
                    return false;
                return int.TryParse(input, out _);
            }

            return int.TryParse(input, out _);
        }

        private bool IsReal(string input)
        {
            if (string.IsNullOrEmpty(input))
                return false;

            // Debe contener un punto decimal
            if (!input.Contains("."))
                return false;

            return double.TryParse(input.Replace(",", "."), System.Globalization.NumberStyles.Float,
                System.Globalization.CultureInfo.InvariantCulture, out _);
        }

        private bool IsExponential(string input)
        {
            if (string.IsNullOrEmpty(input))
                return false;

            // Debe contener E o e
            if (!input.Contains("E") && !input.Contains("e"))
                return false;

            // Validar que sea un número exponencial válido
            // Formato: [dígitos][.dígitos][e|E][+|-][dígitos]
            return double.TryParse(input, System.Globalization.NumberStyles.Float |
                System.Globalization.NumberStyles.AllowExponent,
                System.Globalization.CultureInfo.InvariantCulture, out double result) && 
                double.IsInfinity(result) == false;
        }

        private bool IsComment(string input)
        {
            if (string.IsNullOrEmpty(input))
                return false;

            // Un comentario comienza con #
            return input.StartsWith("#");
        }

        private bool IsString(string input)
        {
            if (string.IsNullOrEmpty(input))
                return false;

            // Debe estar entre comillas
            if ((input.StartsWith("\"") && input.EndsWith("\"")) ||
                (input.StartsWith("'") && input.EndsWith("'")))
            {
                return input.Length >= 2;
            }

            return false;
        }

        private Token AnalyzeOperator(string input)
        {
            switch (input)
            {
                // Operadores aritméticos
                case "+":
                    return new Token { Estado = 144, Categoria = "OA1", Lexema = input, EsError = false };
                case "-":
                    return new Token { Estado = 146, Categoria = "OA2", Lexema = input, EsError = false };
                case "*":
                    return new Token { Estado = 148, Categoria = "OA3", Lexema = input, EsError = false };
                case "/":
                    return new Token { Estado = 150, Categoria = "OA4", Lexema = input, EsError = false };

                // Operadores relacionales
                case "<":
                    return new Token { Estado = 155, Categoria = "OR3", Lexema = input, EsError = false };
                case ">":
                    return new Token { Estado = 161, Categoria = "OR1", Lexema = input, EsError = false };
                case "<=":
                    return new Token { Estado = 157, Categoria = "OR4", Lexema = input, EsError = false };
                case ">=":
                    return new Token { Estado = 163, Categoria = "OR2", Lexema = input, EsError = false };
                case "<>":
                    return new Token { Estado = 159, Categoria = "OR5", Lexema = input, EsError = false };
                case "==":
                    return new Token { Estado = 167, Categoria = "OR6", Lexema = input, EsError = false };

                // Operador de asignación
                case "=":
                    return new Token { Estado = 165, Categoria = "ASI", Lexema = input, EsError = false };

                // Caracteres especiales
                case "¡":
                    return new Token { Estado = 169, Categoria = "CE1", Lexema = input, EsError = false };
                case "@":
                    return new Token { Estado = 171, Categoria = "CE2", Lexema = input, EsError = false };
                case "$":
                    return new Token { Estado = 173, Categoria = "CE3", Lexema = input, EsError = false };
                case "%":
                    return new Token { Estado = 175, Categoria = "CE4", Lexema = input, EsError = false };
                case "^":
                    return new Token { Estado = 177, Categoria = "CE5", Lexema = input, EsError = false };
                case "&":
                    return new Token { Estado = 179, Categoria = "CE6", Lexema = input, EsError = false };
                case "(":
                    return new Token { Estado = 181, Categoria = "CE7", Lexema = input, EsError = false };
                case ")":
                    return new Token { Estado = 183, Categoria = "CE8", Lexema = input, EsError = false };
                case "{":
                    return new Token { Estado = 185, Categoria = "CE9", Lexema = input, EsError = false };
                case "}":
                    return new Token { Estado = 187, Categoria = "CE10", Lexema = input, EsError = false };
                case ":":
                    return new Token { Estado = 189, Categoria = "CE11", Lexema = input, EsError = false };
                case "\"":
                    return new Token { Estado = 191, Categoria = "CE12", Lexema = input, EsError = false };
                case ";":
                    return new Token { Estado = 193, Categoria = "DEL", Lexema = input, EsError = false };
                case "?":
                    return new Token { Estado = 195, Categoria = "CE14", Lexema = input, EsError = false };
                case "\\":
                    return new Token { Estado = 197, Categoria = "CE15", Lexema = input, EsError = false };
                case ",":
                    return new Token { Estado = 199, Categoria = "CE16", Lexema = input, EsError = false };
                case ".":
                    return new Token { Estado = 201, Categoria = "CE17", Lexema = input, EsError = false };
                case "~":
                    return new Token { Estado = 203, Categoria = "CE18", Lexema = input, EsError = false };
                case "[":
                    return new Token { Estado = 205, Categoria = "CE19", Lexema = input, EsError = false };
                case "]":
                    return new Token { Estado = 207, Categoria = "CE20", Lexema = input, EsError = false };
                case "_":
                    return new Token { Estado = 209, Categoria = "CE21", Lexema = input, EsError = false };
                case "¿":
                    return new Token { Estado = 211, Categoria = "CE22", Lexema = input, EsError = false };
                case "!":
                    return new Token { Estado = 213, Categoria = "CE23", Lexema = input, EsError = false };
                case "#":
                    return new Token { Estado = 138, Categoria = "COM", Lexema = input, EsError = false };

                default:
                    return null;
            }
        }
    }
}
