using FluentAssertions;
using Intermedium.Compatibility;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Intermedium.Tests
{
    [TestClass]
    public class TaskBridgeTests
    {
        [TestMethod]
        public void CompletedTask_ShouldBeSingleton()
        {
            var task = TaskBridge.CompletedTask;

            task.IsCompleted.Should().BeTrue();
            task.Should().BeSameAs(TaskBridge.CompletedTask);
        }
    }
}
