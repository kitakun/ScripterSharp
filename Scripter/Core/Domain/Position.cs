using System;

namespace Scripter.Core.Domain
{
    /// <summary>
    /// Ref struct для хранения позиции в тексте - оптимизированная версия
    /// </summary>
    public ref struct TextPosition
    {
        public int Position;
        public int Line;
        public int Column;

        public TextPosition(int position, int line, int column)
        {
            Position = position;
            Line = line;
            Column = column;
        }

        public void Advance()
        {
            Position++;
            Column++;
        }

        public void AdvanceLine()
        {
            Position++;
            Line++;
            Column = 1;
        }

        public readonly bool IsValid(int maxLength) => Position < maxLength;
    }
}
