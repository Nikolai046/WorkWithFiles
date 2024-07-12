using System;
using System.IO;
using System.Collections.Generic;

class DirectoryState
{
    public string Path { get; set; }
    public int SelectedIndex { get; set; }
    public int ScrollOffset { get; set; }

    public DirectoryState(string path, int selectedIndex, int scrollOffset)
    {
        Path = path;
        SelectedIndex = selectedIndex;
        ScrollOffset = scrollOffset;
    }
}

class Program
{
    static void Main(string[] args)
    {
        List<DirectoryState> history = new List<DirectoryState> { new DirectoryState(@"C:\", 0, 0) };
        int historyIndex = 0;
        const int maxDisplayLines = 30;

        while (true)
        {
            DirectoryState currentState = history[historyIndex];
            string currentPath = currentState.Path;
            int selectedIndex = currentState.SelectedIndex;
            int scrollOffset = currentState.ScrollOffset;

            string[] directories;
            try
            {
                directories = Directory.GetDirectories(currentPath);
            }
            catch (UnauthorizedAccessException)
            {
                Console.WriteLine("Нет доступа к этой директории. Нажмите любую клавишу для возврата.");
                Console.ReadKey(true);
                if (historyIndex > 0) historyIndex--;
                continue;
            }
            catch (Exception e)
            {
                Console.WriteLine($"Ошибка: {e.Message}. Нажмите любую клавишу для возврата.");
                Console.ReadKey(true);
                if (historyIndex > 0) historyIndex--;
                continue;
            }

            bool needRedraw = true;

            while (true)
            {
                if (needRedraw)
                {
                    Console.Clear();
                    Console.WriteLine($"Текущая директория: {currentPath}");
                    Console.WriteLine("Используйте стрелки для навигации, Enter для входа в папку, Backspace для возврата, Esc для выхода");
                    Console.WriteLine();

                    int startIndex = Math.Max(0, Math.Min(scrollOffset, directories.Length - maxDisplayLines));
                    int endIndex = Math.Min(startIndex + maxDisplayLines, directories.Length);

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

                    if (startIndex > 0)
                        Console.WriteLine("↑ Прокрутите вверх для просмотра дополнительных директорий");
                    if (endIndex < directories.Length)
                        Console.WriteLine("↓ Прокрутите вниз для просмотра дополнительных директорий");

                    needRedraw = false;
                }

                ConsoleKeyInfo key = Console.ReadKey(true);

                switch (key.Key)
                {
                    case ConsoleKey.UpArrow:
                        if (selectedIndex > 0)
                        {
                            selectedIndex--;
                            if (selectedIndex < scrollOffset)
                                scrollOffset = selectedIndex;
                            needRedraw = true;
                        }
                        break;

                    case ConsoleKey.DownArrow:
                        if (selectedIndex < directories.Length - 1)
                        {
                            selectedIndex++;
                            if (selectedIndex >= scrollOffset + maxDisplayLines)
                                scrollOffset = selectedIndex - maxDisplayLines + 1;
                            needRedraw = true;
                        }
                        break;

                    case ConsoleKey.Enter:
                        if (directories.Length > 0)
                        {
                            string newPath = directories[selectedIndex];
                            historyIndex++;
                            if (historyIndex < history.Count)
                            {
                                history[historyIndex] = new DirectoryState(newPath, 0, 0);
                                history.RemoveRange(historyIndex + 1, history.Count - historyIndex - 1);
                            }
                            else
                            {
                                history.Add(new DirectoryState(newPath, 0, 0));
                            }
                            currentState.SelectedIndex = selectedIndex;
                            currentState.ScrollOffset = scrollOffset;
                            goto OuterLoop;
                        }
                        break;

                    case ConsoleKey.Backspace:
                        if (historyIndex > 0)
                        {
                            historyIndex--;
                            goto OuterLoop;
                        }
                        break;

                    case ConsoleKey.Escape:
                        return;
                }

                currentState.SelectedIndex = selectedIndex;
                currentState.ScrollOffset = scrollOffset;
            }

        OuterLoop:
            continue;
        }
    }
}