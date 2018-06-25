using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace System.CommandLine.API
{

    public class OptionCollection : IEnumerable<Option>
    {
        private readonly List<Option> _options = new List<Option>();

        internal OptionCollection()
        { }

        public static OptionCollection Create(params Option[] options)
        {
            var collection = new OptionCollection();
            if (options != null)
            {
                foreach (var option in options)
                {
                    collection.Add<Option>(option);
                }
            }
            return collection;
        }


        public Option this[string idOrName]
            => _options.FirstOrDefault(x => x.Id == idOrName)
               ?? _options.FirstOrDefault(x => x.Name == idOrName)
               ?? _options.FirstOrDefault(x => x.Aliases.Contains(idOrName));

        internal TNewOption Add<TNewOption>(TNewOption option)
            where TNewOption : Option
        {
            _options.Add(option);
            return option;
        }

        internal OptionCollection Add(params Option[] options)
        {
            if (options != null)
            {
                foreach (var option in options)
                {
                    Add<Option>(option);
                }
            }
            return this;
        }

        public IEnumerator<Option> GetEnumerator()
            => _options.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator()
            => GetEnumerator();
    }
}
