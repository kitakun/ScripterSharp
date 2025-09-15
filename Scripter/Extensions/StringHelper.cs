using System;
using System.Runtime.CompilerServices;

namespace Scripter.Extensions
{
    /// <summary>
    /// Вспомогательные методы для работы со строками с оптимизациями
    /// </summary>
    public static class StringHelper
    {
        /// <summary>
        /// Проверяет, является ли символ буквой или цифрой
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsAlphaNumeric(char c)
        {
            return char.IsLetterOrDigit(c) || c == '_';
        }

        /// <summary>
        /// Проверяет, является ли символ буквой или подчеркиванием
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsAlphaOrUnderscore(char c)
        {
            return char.IsLetter(c) || c == '_';
        }

        /// <summary>
        /// Проверяет, является ли символ пробелом или табуляцией
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsWhitespace(char c)
        {
            return c == ' ' || c == '\t' || c == '\r';
        }

        /// <summary>
        /// Проверяет, является ли символ оператором
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsOperator(char c)
        {
            return c == '+' || c == '-' || c == '*' || c == '/' || c == '%' ||
                   c == '=' || c == '!' || c == '<' || c == '>' || c == '&' || c == '|';
        }

        /// <summary>
        /// Проверяет, является ли символ разделителем
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsDelimiter(char c)
        {
            return c == '(' || c == ')' || c == '{' || c == '}' || 
                   c == '[' || c == ']' || c == ';' || c == ',' || c == '.';
        }
    }
}
