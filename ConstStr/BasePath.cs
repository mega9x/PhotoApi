using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConstStr
{
    public class BasePath
    {
        public static string ExePath =>
            Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location)!;
    }
}
