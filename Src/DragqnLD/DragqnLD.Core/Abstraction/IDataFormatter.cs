﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DragqnLD.Core.Abstraction
{
    interface IDataFormatter
    {
        void Format(TextReader input, TextWriter output, string rootObjectId);
    }
}