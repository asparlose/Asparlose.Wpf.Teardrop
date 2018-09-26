using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Asparlose.Wpf
{
    public class FileDropEventArgs : EventArgs
    {
        public ReadOnlyCollection<string> Files { get; }

        public FileDropEventArgs(IEnumerable<string> files)
        {
            Files = new ReadOnlyCollection<string>(files.ToArray());
        }
    }
}
