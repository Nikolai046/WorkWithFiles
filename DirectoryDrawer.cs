using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WorkWithFiles
{
    class DirectoryDrawer
    {
        public int MaxDisplayLines { get; set; }

        public DirectoryDrawer(int maxDisplayLines)
        {
            MaxDisplayLines = maxDisplayLines;
        }

        public void DrawInterface(string currentPath, string[] directories, int selectedIndex, int scrollOffset, string dirsize)
        {
            int startIndex = Math.Max(0, Math.Min(scrollOffset, directories.Length - MaxDisplayLines));
            int endIndex = Math.Min(startIndex + MaxDisplayLines, directories.Length);
            Console.Clear();
            Console.WriteLine("Используйте стрелки для навигации, Enter для входа в папку, Backspace для возврата, Esc для выхода");
            Console.WriteLine();
            if (startIndex > 0) Console.WriteLine("^"); else Console.WriteLine();

            for (int i = startIndex; i < endIndex; i++)
            {
                if (i == selectedIndex)
                {
                    Console.BackgroundColor = ConsoleColor.Gray;
                    Console.ForegroundColor = ConsoleColor.Black;
                }

                Console.WriteLine(Path.GetFileName(directories[i]));

                Console.ResetColor();
            }

            if (MaxDisplayLines > endIndex)
                for (int i = 0; i < MaxDisplayLines - endIndex + 1; i++)
                {
                    Console.WriteLine();
                }

            if (endIndex < directories.Length) Console.WriteLine("v");
            Console.Write($"\nТекущая директория: ");
            Console.BackgroundColor = ConsoleColor.Gray;
            Console.ForegroundColor = ConsoleColor.Black;
            Console.WriteLine($"{currentPath}");
            Console.ResetColor();

            Console.WriteLine(dirsize);
            Console.BackgroundColor = ConsoleColor.Gray;
            Console.ForegroundColor = ConsoleColor.Black;
            Console.WriteLine("Нажми F1 для перехода к файлам");
            Console.ResetColor();
        }
    }
}
