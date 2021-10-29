using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bogdanov.ConsoleWorkLib
{
    /// <summary>
    /// Класс для вывода примеров по разным дизайнерским решениям,
    /// например, цвета шрифтов.
    /// </summary>
    public static class DesignExamples
    {
        /// <summary>
        /// Выводит примеры цвета шрифтов с их названиями.
        /// </summary>
        public static void FontColors()
        {
            Action<ConsoleColor> ShowColor = (color) =>
            {
                Console.ForegroundColor = color;
                Console.WriteLine($"{color}");
            };

            ShowColor(ConsoleColor.Red);
            ShowColor(ConsoleColor.Green);
            ShowColor(ConsoleColor.Blue);
            ShowColor(ConsoleColor.Magenta);
            ShowColor(ConsoleColor.Yellow);
            ShowColor(ConsoleColor.Cyan);
            ShowColor(ConsoleColor.Black);
            ShowColor(ConsoleColor.DarkBlue);
            ShowColor(ConsoleColor.DarkCyan);
            ShowColor(ConsoleColor.DarkGray);
            ShowColor(ConsoleColor.DarkGreen);
            ShowColor(ConsoleColor.DarkMagenta);
            ShowColor(ConsoleColor.DarkRed);
            ShowColor(ConsoleColor.DarkYellow);
            ShowColor(ConsoleColor.Gray);
            ShowColor(ConsoleColor.White);

            Console.ResetColor();
        }
    }
}
