using System;

namespace Library
{
    public class Result
    {
        public string Value { get; }

        public string MemorySource { get; set; }

        public Result(string value, string memorySource)
        {
            Value = value ?? throw new ArgumentNullException(nameof(value));
            MemorySource = memorySource ?? throw new ArgumentNullException(nameof(memorySource));
        }
    }
}
