#pragma warning disable 1591

using System.Collections.Generic;

namespace DragqnLD.WebApi.Areas.RouteDebugger.Models
{
    public class RouteInfo
    {
        public string RouteTemplate { get; set; }

        public KeyValuePair<string, string>[] Defaults { get; set; }

        public KeyValuePair<string, string>[] Constraints { get; set; }

        public KeyValuePair<string, string>[] DataTokens { get; set; }

        public string Handler { get; set; }

        public bool Picked { get; set; }
    }
}
