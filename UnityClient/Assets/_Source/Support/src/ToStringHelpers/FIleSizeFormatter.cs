namespace Support.ToStringHelpers
{
    public static class FIleSizeFormatter
    {
        private static readonly string[] sSizes = { "B", "KB", "MB", "GB", "TB" };

        public static string ToPrettyFileSize(long fileSize)
        {
            double len = fileSize;
            int order = 0;
            while (len >= 1024 && order < sSizes.Length - 1)
            {
                order++;
                len = len / 1024;
            }
            return string.Format("{0:0.##} {1}", len, sSizes[order]);
        }
    }
}
