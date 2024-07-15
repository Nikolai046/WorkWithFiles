using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace WorkWithFiles
{
    public class ShowFiles
    {
        protected FileInfo[] files;
        protected string[,] showFiles;
        private const int MaxFileNameLength = 30;
        private const int ColumnWidth = MaxFileNameLength + 2;
        private const int FilesPerPage = 60;
        protected readonly string dirSize;
        protected long selectedDirSize = 0;
        protected int countselected=0;
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
                { showFiles[i, 1] = "true";
                    countselected++;
                    selectedDirSize += files[i].Length;
                 }
                else
                    showFiles[i, 1] = "false";
            }

        }
        public void DisplayFiles()
        {
            int pageCount = (showFiles.GetLength(0) + FilesPerPage - 1) / FilesPerPage;
            int currentPage = 0;

            while (currentPage < pageCount)
            {
                Console.Clear();
                DisplayPage(showFiles, currentPage);
                currentPage = GetNextPage(currentPage, pageCount);
                
                 }
        }

        public void DisplayPage(string[,] showFiles, int page)
        {
            int startIndex = page * FilesPerPage;
            int endIndex = Math.Min(startIndex + FilesPerPage, showFiles.GetLength(0));
            int currentPageLines = (int)Math.Ceiling((double)(endIndex - startIndex) / 3);
            int maxPageLines = 24;

            for (int i = startIndex; i < endIndex; i += 3)
            {
                string col1 = GetFormattedFileName(showFiles, i);
                string col2 = GetFormattedFileName(showFiles, i + 1);
                string col3 = GetFormattedFileName(showFiles, i + 2);

                Console.WriteLine($"{col1}{col2}{col3}");
            }
            if (currentPageLines < maxPageLines)
                for (int i = 0; i < maxPageLines - currentPageLines; i++)
                {
                    Console.WriteLine();
                }
            Console.WriteLine($"Директория содерит {showFiles.GetLength(0)} файлов. {dirSize}");
            Console.WriteLine($"\nСтраница {page + 1} из {(showFiles.GetLength(0) + FilesPerPage - 1) / FilesPerPage}");
            Console.WriteLine("Используйте стрелки для навигации, Backspace для возврата");
            Console.BackgroundColor = ConsoleColor.Gray;
            Console.ForegroundColor = ConsoleColor.Black;
            Console.WriteLine("Нажми F10 для выделения файлов, которые не использовались более 30 минут");
            Console.ResetColor();
        }

        private string GetFormattedFileName(string[,] showFiles, int index)
        {
            if (index < showFiles.GetLength(0))
            {
                string fileName = showFiles[index,0];
                if (fileName.Length > MaxFileNameLength)
                {
                    fileName = fileName.Substring(0, MaxFileNameLength - 1) + "~";

                }
                return fileName.PadRight(ColumnWidth);
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
                        return (currentPage==0)? pageCount-1 : (currentPage - 1) % pageCount;

                    case ConsoleKey.Backspace:
                        return pageCount;
                        /*
                    case ConsoleKey.F10:
                        код для вызова метода выбора файлов которые не использовались более 30 минут*/
                }

            }
        }


    }
}
