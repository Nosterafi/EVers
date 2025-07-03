using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain
{
    public interface IChange
    {
        void Apply(DirectoryInfo repoDirectory);

        void Cansel(DirectoryInfo repoDirectory);
    }
}
