namespace WorkWithFiles
{
    internal class Program
    {
        private const int MaxDisplayLines = 20;
        private const int MaxHistorySize = 2;
        private static DirectoryState[] history = new DirectoryState[MaxHistorySize];
        private static int historyIndex = -1;

        private static void Main(string[] args)
        {
            string currentPath = $"{Directory.GetCurrentDirectory()}";

            DirectoryDrawer drawer = new DirectoryDrawer(MaxDisplayLines);
            DirectoryState currentState = new(currentPath, 0, 0);
            bool needSizeCalculated = true;
            int selectedIndex = 0;
            int previosIndex = 0;
            int scrollOffset = 0;
            string dirSize = "";
            bool backward = false;

            while (true)
            {
                string[] directories;
                try
                {
                    directories = Directory.GetDirectories(currentPath);
                }
                catch (Exception e)
                {
                    Console.WriteLine($"Ошибка: {e.Message}");

                    if (historyIndex >= 0)
                    {
                        historyIndex--;
                        currentPath = Directory.GetParent(currentPath)?.FullName ?? currentPath;
                    }
                    else
                    {
                        currentPath = Directory.GetParent(currentPath)?.FullName ?? currentPath;
                    }
                    Console.ReadKey();

                    continue;
                }

                bool needRedraw = true;
                while (true)
                {
                    if (needSizeCalculated)
                    {
                        try
                        {
                            dirSize = ($"Общий размер: {new CalculateDirectorySize(currentPath).DirSize} байт");
                            needSizeCalculated = false;
                        }
                        catch (Exception ex)
                        {
                            dirSize = "Размер невозможно посчитать, т.к. нет доступа к одному из элементов";
                            needSizeCalculated = false;
                            if (!backward) selectedIndex = previosIndex;
                        }
                    }
                    if (needRedraw)
                    {
                        drawer.DrawInterface(currentPath, directories, selectedIndex, scrollOffset, dirSize);
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
                                if (selectedIndex >= scrollOffset + MaxDisplayLines)
                                    scrollOffset = selectedIndex - MaxDisplayLines + 1;
                                needRedraw = true;
                            }
                            break;

                        case ConsoleKey.Enter:
                            if (directories.Length > 0)
                            {
                                string newPath = directories[selectedIndex];
                                if (historyIndex >= MaxHistorySize)
                                {
                                    Array.Copy(history, 1, history, 0, MaxHistorySize - 1);
                                    historyIndex = MaxHistorySize - 1;
                                }
                                if (historyIndex < 0) { selectedIndex = 0; historyIndex++; }
                                if (historyIndex >= 0)
                                {
                                    history[historyIndex] = new DirectoryState(currentPath, selectedIndex, scrollOffset);
                                    
                                }
                                if (historyIndex < 0)
                                {
                                    historyIndex++;
                                    history[0] = new DirectoryState(currentPath, selectedIndex, scrollOffset);
                                }
                                currentPath = newPath;
                                previosIndex = selectedIndex;
                                selectedIndex = 0;
                                scrollOffset = 0;
                                needSizeCalculated = true;
                                historyIndex++;

                                goto Exit;
                            }
                            break;

                        case ConsoleKey.Backspace:
                           if (historyIndex >= 0)
                            {
                                historyIndex--;
                                if (historyIndex >= 0)
                                {
                                    DirectoryState prevState = history[historyIndex];
                                    currentPath = prevState.Path;
                                    selectedIndex = prevState.SelectedIndex;
                                    scrollOffset = prevState.ScrollOffset;
                                }
                                else
                                {
                                    currentPath = Directory.GetParent(currentPath)?.FullName ?? currentPath;
                                    selectedIndex = 0;
                                    scrollOffset = 0;
                                }
                                needSizeCalculated = true;
                                backward = true;
                                goto Exit;
                            }
                            else
                            {
                                currentPath = Directory.GetParent(currentPath)?.FullName ?? currentPath;
                                selectedIndex = 0;
                                scrollOffset = 0;
                            }
                            needSizeCalculated = true;
                            goto Exit;

                        case ConsoleKey.Escape:
                            return;

                        case ConsoleKey.F1:
                            ShowFiles showFiles = new ShowFiles(currentPath, dirSize);
                            goto Exit;
                    }
                }

            //
            Exit:
                continue;
            }
        }
    }
}