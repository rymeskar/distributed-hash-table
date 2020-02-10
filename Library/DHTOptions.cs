using System;
using System.Collections.Generic;
using System.Text;

namespace Library
{
    public class DHTOptions
    {
        public TimeSpan LivenessCaching { get; set; } = TimeSpan.FromSeconds(11);

        public TimeSpan TranslationCaching { get; set; } = TimeSpan.FromMinutes(1);

        public TimeSpan InMemoryCaching { get; set; } = TimeSpan.FromMinutes(1);

        public TimeSpan ConsistencyPeriod { get; set; } = TimeSpan.FromSeconds(3);

    }
}
