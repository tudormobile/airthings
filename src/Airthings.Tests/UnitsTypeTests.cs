namespace Airthings.Tests;

[TestClass]
public class UnitsTypeTests
{
    [TestMethod]
    public void DefaultValue_MustBeMetric()
    {
        var expected = UnitsType.Metric;
        var actual = (UnitsType)0;
        Assert.AreEqual(expected, actual, "Default value must be 'metric'");
    }
}
