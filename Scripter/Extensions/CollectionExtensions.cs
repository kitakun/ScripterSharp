using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Scripter.Extensions
{
    /// <summary>
    /// Расширения для коллекций с оптимизациями
    /// </summary>
    public static class CollectionExtensions
    {
        /// <summary>
        /// Безопасное получение элемента по индексу
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T SafeGet<T>(this IList<T> list, int index) where T : class
        {
            return index >= 0 && index < list.Count ? list[index] : null;
        }

        /// <summary>
        /// Проверяет, является ли индекс валидным
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsValidIndex<T>(this IList<T> list, int index)
        {
            return index >= 0 && index < list.Count;
        }

        /// <summary>
        /// Добавляет элемент в список, если он не null
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void AddIfNotNull<T>(this IList<T> list, T item) where T : class
        {
            if (item != null)
            {
                list.Add(item);
            }
        }

        /// <summary>
        /// Получает последний элемент списка
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T LastOrDefault<T>(this IList<T> list) where T : class
        {
            return list.Count > 0 ? list[list.Count - 1] : null;
        }
    }
}
