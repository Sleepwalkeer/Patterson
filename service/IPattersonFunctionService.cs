using Patterson.model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Patterson.service
{
    internal interface IPattersonFunctionService
    {
        void Execute(Sample sampl);
    }
}
