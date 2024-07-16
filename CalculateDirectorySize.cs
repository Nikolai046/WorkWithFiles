namespace WorkWithFiles
{
    class CalculateDirectorySize
    {
        public long DirSize { get; private set; } = 0;

        public CalculateDirectorySize(string currentPath)
        {
            DirectoryInfo dirInfo = new DirectoryInfo(currentPath);
            GetDirectorySize(dirInfo);
        }

        private void GetDirectorySize(DirectoryInfo dirInfo)
        {
            // Получаем размер всех файлов в директории
            FileInfo[] files = dirInfo.GetFiles();
            foreach (FileInfo file in files)
            {
                DirSize += file.Length;
            }

            // Рекурсивно получаем размер всех поддиректорий
            DirectoryInfo[] dirs = dirInfo.GetDirectories();
            foreach (DirectoryInfo dir in dirs)
            {
                GetDirectorySize(dir);
            }
        }
    }
}