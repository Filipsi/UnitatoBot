using System.Collections.Generic;

namespace UnitatoBot.Util {

    public class WrappedEnumerable<T> : IEnumerable<T> {

        private IEnumerable<T> DataSource;

        public WrappedEnumerable(IEnumerable<T> dataSource) {
            DataSource = dataSource;
        }

        public IEnumerator<T> GetEnumerator() {
            return DataSource.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() {
            return ((System.Collections.IEnumerable)DataSource).GetEnumerator();
        }

    }

}
