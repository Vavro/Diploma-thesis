using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Lucene.Net.Analysis;
using Raven.Database.Indexing;

namespace DragqnLD.Analyzers
{
    public class ReverseLowerCaseKeywordAnalyzer : LowerCaseKeywordAnalyzer
    {
        public override TokenStream TokenStream(string fieldName, TextReader reader)
        {
            var stream = base.TokenStream(fieldName, reader);
            var reverseStringFilterTokenStream = new ReverseStringFilter(stream);
            return reverseStringFilterTokenStream;
        }
    }
}
