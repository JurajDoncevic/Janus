using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Janus.Base.Tests
{
    public class MappingTests
    {
        [Fact]
        public void MapOverList()
        {
            var list = Enumerable
                        .Range(1, 10);

            var expectedList = list.Select(_ => _ + 1).ToList();

            var mappedList = list.Map(_ => _ + 1);

            Assert.Equal(expectedList, mappedList);
        }

        [Fact]
        public void MapiOverList()
        {
            var list = Enumerable
                        .Range(0, 10);

            var expectedList = list.Select(_ => _ * 2).ToList();

            var mappedList = list.Mapi((idx, element) => (int)idx + element);

            Assert.Equal(expectedList, mappedList);
        }

        [Fact]
        public async void MapOverTask()
        {
            var result = Task.Run(() => { System.Threading.Thread.Sleep(100); return 1; })
                           .Map(_ => _ + 1)
                           .Map(_ => _ + 1);

            Assert.Equal(3, await result);
        }
    }
}
