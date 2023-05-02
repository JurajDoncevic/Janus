using Xunit;

namespace Janus.Base.Tests
{
    public class AggregationTests
    {
        [Fact]
        public void FoldOnSameTypesTest()
        {
            const string testCsv = "This;is;some;rediculous;Csv;line";

            var csvSplit = testCsv.Split(';').ToList();

            string finalCsv = csvSplit.Skip(1).Fold(csvSplit.First(), (item, seed) => seed + ";" + item);

            Assert.NotNull(finalCsv);
            Assert.NotEmpty(finalCsv);
            Assert.Equal(testCsv, finalCsv);
        }

        [Fact]
        public void FoldOnDifferentTypesTest()
        {
            const string testString = "12345";

            List<int> testInts = new List<int> { 1, 2, 3, 4, 5 };

            string finalCsv = testInts.Skip(1).Fold(testInts.First().ToString(), (item, seed) => seed + item.ToString());

            Assert.NotNull(finalCsv);
            Assert.NotEmpty(finalCsv);
            Assert.Equal(testString, finalCsv);
        }

        [Fact]
        public void FoldiTest()
        {
            List<int> testInts = new List<int> { 1, 2, 3, 4, 5 };

            string expectedResultString = "1122334455";

            string resultString = testInts.Foldi("", (idx, item, seed) => seed + (idx + 1).ToString() + item.ToString());

            Assert.NotNull(resultString);
            Assert.NotEmpty(resultString);
            Assert.Equal(resultString, expectedResultString);
        }
    }
}
