using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Janus.Base.Tests
{
    public class BindingTests
    {
        [Fact]
        public void BindingOverListTest()
        {
            List<List<int>> listOfLists = new List<List<int>>()
            {
                new List<int> {1, 2, 3, 4},
                new List<int> {5, 6},
                new List<int> {7 ,8, 9, 10, 11}
            };

            List<int> expectedList = Enumerable.Range(1, 11).ToList();

            var returnedList = listOfLists.Bind(x => x).ToList();

            Assert.Equal(expectedList, returnedList);
        }

        [Fact]
        public async void BindingOverTaskTest()
        {
            var result =
                await Task.Run(() => new List<int> { 1 })
                          .BindTask(_ => Task.Run(() => _.Append(2)))
                          .BindTask(_ => Task.Run(() => _.Append(3)))
                          .BindTask(_ => Task.Run(() => _.Append(4)));

            var expectedList = Enumerable.Range(1, 4).ToList();

            Assert.Equal(expectedList, result);
        }

        [Fact]
        public async void BindingOverTasksWithSleepTest()
        {
            Stopwatch watch = new Stopwatch();
            watch.Start();
            await Task.Run(() => new List<int> { 1 })
                      .BindTask(_ => Task.Run(() => { System.Threading.Thread.Sleep(50); return _.Append(2); }))
                      .BindTask(_ => Task.Run(() => { System.Threading.Thread.Sleep(50); return _.Append(3); }))
                      .BindTask(_ => Task.Run(() => { System.Threading.Thread.Sleep(50); return _.Append(4); }));

            watch.Stop();

            Assert.True(watch.ElapsedMilliseconds > 150);
        }
    }
}
