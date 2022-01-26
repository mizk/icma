using System.Collections.Generic;

namespace icma
{
    public class ListRowModel : Model
    {
        public List<Media> Cells { get; private set; } = new();
        public override void Clone(object value)
        {

        }
    }
}
