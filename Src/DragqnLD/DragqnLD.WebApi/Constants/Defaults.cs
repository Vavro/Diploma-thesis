using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DragqnLD.WebApi.Constants
{
    /// <summary>
    /// Default values for some processes
    /// </summary>
    public class Defaults
    {
        /// <summary>
        /// if no encoding information in headers of incoming request, this encoding is assumed
        /// </summary>
        public const string DefaultRequestEncoding = "UTF-8";
    }
}