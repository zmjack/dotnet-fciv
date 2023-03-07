using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dotnet_fciv
{
    [Flags]
    public enum HashTypes
    {
        None = 0,
        MD5 = 1,
        SHA1 = 2,
        SHA256 = 4,
        SHA384 = 8,
        SHA512 = 16,
    }
}
