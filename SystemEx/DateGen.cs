using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Woof.SystemEx
{
    public class DateGen : IEnumerable<DateTime> {

        public DateGen(DateTime since, DateTime until) {
            Since = since.Date;
            Until = until.Date;
        }

        public IEnumerator<DateTime> GetEnumerator() {
            var d = Since;
            while (d < Until) {
                yield return d.Date;
                d =  d.AddDays(1);
            }
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        private readonly DateTime Since;
        private readonly DateTime Until;

    }

}