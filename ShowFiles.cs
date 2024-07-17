namespace WorkWithFiles
{
    class ShowFiles
    {
        protected FileInfo[] files;
        protected string[,] showFiles;
        private const int MaxFileNameLength = 30;
        private const int ColumnWidth = MaxFileNameLength + 2;
        private const int FilesPerPage = 60;
        protected readonly string dirSize;
        protected long selectedDirSize = 0;
        protected int countselected = 0;
        protected bool isSelected = false;

        public ShowFiles(string currentDir, string dirSize)
        {
            files = new DirectoryInfo(currentDir).GetFiles();
            this.dirSize = dirSize;
            showFiles = new string[files.Length, 2];
            for (int i = 0; i < files.Length; i++)
            {
                showFiles[i, 0] = files[i].Name;
                showFiles[i, 1] = "false";
            }
            Console.Clear();
            DisplayFiles();
        }

        private void SelectFiles()
        {
            for (int i = 0; i < files.Length; i++)
            {
                showFiles[i, 0] = files[i].Name;
                if (DateTime.Now - files[i].LastAccessTime > TimeSpan.FromMinutes(30)) // если файл не использовался более 30 минут
                {
                    showFiles[i, 1] = "true";
                    countselected++;
                    selectedDirSize += files[i].Length;
                }
                else
                {
                    showFiles[i, 1] = "false";
                }
            }

            isSelected = true;
        }

        private void UnSelectFiles()
        {
            for (int i = 0; i < files.Length; i++)
            {
                showFiles[i, 1] = "false";
            }
            countselected = 0;
            selectedDirSize = 0;
            isSelected = false;
        }

        private void DisplayFiles()
        {
            int pageCount = (showFiles.GetLength(0) + FilesPerPage - 1) / FilesPerPage;
            int currentPage = 0;

            while (currentPage < pageCount)
            {
                Console.Clear();
                DisplayPage(showFiles, currentPage);
                currentPage = GetNextPage(currentPage, pageCount);
            }
            isSelected = false;
        }

        private void DisplayPage(string[,] showFiles, int page)
        {
            int startIndex = page * FilesPerPage;
            int endIndex = Math.Min(startIndex + FilesPerPage, showFiles.GetLength(0));
            int currentPageLines = (int)Math.Ceiling((double)(endIndex - startIndex) / 3);
            const int maxPageLines = 24;

            for (int i = startIndex; i < endIndex; i += 3)
            {
                string col1 = GetFormattedFileName(showFiles, i);
                string col2 = GetFormattedFileName(showFiles, i + 1);
                string col3 = GetFormattedFileName(showFiles, i + 2);

                Console.WriteLine($"{col1}{col2}{col3}");
            }
            if (currentPageLines < maxPageLines)
            {
                for (int i = 0; i < maxPageLines - currentPageLines; i++)
                {
                    Console.WriteLine();
                }
            }

            Console.WriteLine($"Директория содерит {showFiles.GetLength(0)} файлов. {dirSize}");
            if (isSelected)
            {
                Console.WriteLine(
                    $"Выбрано: {countselected} файлов объемом {selectedDirSize} байт"
                );
            }
            else
            {
                Console.WriteLine();
            }
            Console.WriteLine(
                $"Страница {page + 1} из {(showFiles.GetLength(0) + FilesPerPage - 1) / FilesPerPage}"
            );
            Console.WriteLine("Используйте стрелки для навигации, Backspace для возврата");

            Console.WriteLine(
                "\u001b[47m\u001b[30mF10 - выделить/снять выделение, файлы которые не использовались более 30 минут\u001b[0m"
            );
        }

        private static string GetFormattedFileName(string[,] showFiles, int index)
        {
            if (index < showFiles.GetLength(0))
            {
                string fileName = showFiles[index, 0];
                bool isSelected = showFiles[index, 1] == "true";

                if (fileName.Length > MaxFileNameLength)
                {
                    fileName = $"{fileName.AsSpan(0, MaxFileNameLength - 1)}~";
                }

                string formattedName = fileName.PadRight(ColumnWidth);

                if (isSelected)
                {
                    return $"\u001b[47m\u001b[30m{formattedName}\u001b[0m"; // Белый фон, черный текст
                }
                else
                {
                    return formattedName;
                }
            }
            return new string(' ', ColumnWidth);
        }

        private int GetNextPage(int currentPage, int pageCount)
        {
            while (true)
            {
                ConsoleKeyInfo key = Console.ReadKey(true);
                switch (key.Key)
                {
                    case ConsoleKey.DownArrow:
                        return (currentPage + 1) % pageCount;

                    case ConsoleKey.UpArrow:
                        return (currentPage == 0) ? pageCount - 1 : (currentPage - 1) % pageCount;

                    case ConsoleKey.Backspace:
                        return pageCount;

                    case ConsoleKey.F10:
                        if (!isSelected)
                        {
                            SelectFiles();
                            return currentPage;
                        }
                        if (isSelected)
                        {
                            UnSelectFiles();
                            return currentPage;
                        }
                        return currentPage;
                }
            }
        }
    }
}
