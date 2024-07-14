using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WorkWithFiles
{
    public class ShowFiles
    {
        protected FileInfo[] files;
        private const int MaxFileNameLength = 30;
        private const int ColumnWidth = MaxFileNameLength + 2;
        private const int FilesPerPage = 90;
        public ShowFiles(string currentDir)
        {
            files = new DirectoryInfo(currentDir).GetFiles();
            Console.Clear();
            DisplayFiles();

        }

        public void DisplayFiles()
        {
            int pageCount = (files.Length + FilesPerPage - 1) / FilesPerPage;
            int currentPage = 0;

            while (currentPage < pageCount)
            {
                Console.Clear();
                DisplayPage(files, currentPage);
                currentPage = GetNextPage(currentPage, pageCount);
            }
        }

        public void DisplayPage(FileInfo[] files, int page)
        {
            int startIndex = page * FilesPerPage;
            int endIndex = Math.Min(startIndex + FilesPerPage, files.Length);

            for (int i = startIndex; i < endIndex; i += 3)
            {
                string col1 = GetFormattedFileName(files, i);
                string col2 = GetFormattedFileName(files, i + 1);
                string col3 = GetFormattedFileName(files, i + 2);

                Console.WriteLine($"{col1}{col2}{col3}");
            }
            Console.WriteLine($"\nДиректория содерит {files.Length} файлов");
            Console.WriteLine($"\nСтраница {page + 1} из {(files.Length + FilesPerPage - 1) / FilesPerPage}");
            Console.WriteLine("Нажмите Enter для следующей страницы, 'q' для выхода");
        }

        private string GetFormattedFileName(FileInfo[] files, int index)
        {
            if (index < files.Length)
            {
                string fileName = files[index].Name;
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
                var key = Console.ReadKey(true);
                if (key.Key == ConsoleKey.Enter)
                {
                    return (currentPage + 1) % pageCount;
                }
                else if (key.KeyChar == 'q' || key.KeyChar == 'Q')
                {
                    return pageCount;
                }
            }
        }


    }
}
