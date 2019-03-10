using ImportAnything.Services.Interfaces;
using System.IO;

namespace ImportAnything.Services.Transformers
{
    public class FolderToFiles : Transformer<DirectoryInfo,FileInfo[]>
    {
        private bool _recurse;
        private string _pattern;

        public FolderToFiles(string pattern, bool recurse)
        {
            _recurse = recurse;
            _pattern = pattern;
        }

        protected override  FileInfo[] Transform(DirectoryInfo source)
        {
            return source.GetFiles(_pattern, _recurse ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly);
        }      
    }
}
