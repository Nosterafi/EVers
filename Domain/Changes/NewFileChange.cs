using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain
{
    public class NewFileChange : IChange
    {
        private readonly string _path;

        public void Apply(DirectoryInfo repoDirectory)
        {
            try { File.Create(Path.Combine(repoDirectory.FullName, _path)); }
            catch (Exception ex)
            {
                throw new IOException($"Failed to create file {_path}", ex);
            }
        }

        public void Cansel(DirectoryInfo repoDirectory)
        {
            try { File.Create(Path.Combine(repoDirectory.FullName, _path)); }
            catch (Exception ex)
            {
                throw new IOException($"Failed to create file {_path}", ex);
            }
        }
    }
}
