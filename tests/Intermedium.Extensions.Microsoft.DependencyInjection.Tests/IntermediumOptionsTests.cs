using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Intermedium.Extensions.Microsoft.DependencyInjection.Tests
{
    [TestClass]
    public class IntermediumOptionsTests
    {
        [TestMethod]
        public void Scan_SameAssemblyMultipleTimes_ScansOnlyUniqueAssemblies()
        {
            var type = typeof(IntermediumOptionsTests);
            var options = new IntermediumOptions();

            options.Scan(type, type, type);

            options.ScanTargets.Should().HaveCount(1);
        }
    }
}
