using System.Collections.Generic;

namespace icma
{
    public static class ListExtensions
    {
        public static Media At(this List<Media> list, int pos)
        {
            return list == null || pos < 0 || pos >= list.Count?null:list[pos];
        }

        public static string At(this List<string> list, int pos)
        {
            return list == null || pos < 0 || pos >= list.Count ? string.Empty : list[pos];
        }
    }
}
