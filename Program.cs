using System;
using System.IO;
using System.Collections.Generic;

namespace WorkWithFiles
{

    class Program
    {
        const int maxDisplayLines = 20;

        static void Main(string[] args)
        {
            List<DirectoryState> history = new List<DirectoryState> { new DirectoryState(Directory.GetCurrentDirectory(), 0, 0) };
            int historyIndex = 0;

            while (true)
            {
                DirectoryState currentState = history[historyIndex];
                string currentPath = currentState.Path;
                int selectedIndex = currentState.SelectedIndex;
                int scrollOffset = currentState.ScrollOffset;

                if (!currentState.SizeCalculated)
                {
                    try
                    {

                        currentState.DirSize = ($"Размер: {new CalculateDirectorySize(currentPath).DirSize} байт");
                        currentState.SizeCalculated = true;
                    }
                    catch (Exception ex)
                    {
                        currentState.DirSize = "Размер невозможно посчитать, т.к. нет доступа к одной из директорий";
                    }
                }


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

                        DirDrawInterface(currentPath, directories, selectedIndex, scrollOffset, currentState.DirSize);
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
                                goto Exit;
                            }
                            break;

                        case ConsoleKey.Backspace:
                            if (historyIndex > 0)
                            {
                                historyIndex--;
                                goto Exit;
                            }
                            else if (historyIndex == 0 && currentState.Path != Directory.GetDirectoryRoot(currentState.Path))
                            {
                                if (Directory.GetParent(currentState.Path) != null)
                                {
                                    currentState.Path = Directory.GetParent(currentState.Path).FullName;
                                    currentState.SelectedIndex = 0;
                                    currentState.ScrollOffset = 0;
                                    goto Exit;
                                }
                            }
                            break;

                        case ConsoleKey.Escape:
                            return;

                        case ConsoleKey.F1:
                            ShowFiles showFiles = new ShowFiles(currentState.Path);
                            goto Exit;
                    }

                    currentState.SelectedIndex = selectedIndex;
                    currentState.ScrollOffset = scrollOffset;
                }

            Exit:
                continue;
            }
        }

        static void DirDrawInterface(string currentPath, string[] directories, int selectedIndex, int scrollOffset, string dirsize)
        {

            int startIndex = Math.Max(0, Math.Min(scrollOffset, directories.Length - maxDisplayLines));
            int endIndex = Math.Min(startIndex + maxDisplayLines, directories.Length);
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

            if (maxDisplayLines > endIndex)

                for (int i = 0; i < maxDisplayLines - endIndex + 1; i++)
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