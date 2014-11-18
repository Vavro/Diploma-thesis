using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Raven.Client.Indexes;

namespace DragqnLD.Core.Indexes
{

    public class Documents_CountByCollection : AbstractIndexCreationTask<object, Documents_CountByCollection.ReduceResult>
    {
        public class ReduceResult
        {
            public string Name { get; set; }

            public int Count { get; set; }
        }

        public Documents_CountByCollection()
        {
            Map = docs => from doc in docs
                          select new ReduceResult()
                          {
                              Name = MetadataFor(doc).Value<string>("Raven-Entity-Name"),
                              Count = 1
                          };

            Reduce = results => from result in results
                                group result by result.Name
                                    into g
                                    select new ReduceResult()
                                    {
                                        Name = g.Key,
                                        Count = g.Sum(x => x.Count)
                                    };
        }
    }
}
