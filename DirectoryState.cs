namespace WorkWithFiles
{
    internal class DirectoryState
    {
        public string Path { get; set; }
        public int SelectedIndex { get; set; }
        public int ScrollOffset { get; set; }
        public string DirSize { get; set; }
        public bool SizeCalculated { get; set; }

        public DirectoryState(string path, int selectedIndex, int scrollOffset)
        {
            Path = path;
            SelectedIndex = selectedIndex;
            ScrollOffset = scrollOffset;
            DirSize = "";
            SizeCalculated = false;
        }
    }
}