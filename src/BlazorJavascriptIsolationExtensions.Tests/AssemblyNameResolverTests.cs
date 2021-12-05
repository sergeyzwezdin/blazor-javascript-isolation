using System.Reflection;
using Microsoft.AspNetCore.Components;
using Moq;
using Xunit;

namespace BlazorJavascriptIsolationExtensions.Tests;

public class AssemblyNameResolverTests
{
    [Fact]
    public void GetEntryAssembly__ShouldReturnAssemlbyName()
    {
        // Arrange
        var resolver = new AssemblyNameResolver();

        // Act
        var result = resolver.GetEntryAssembly();

        // Assert
        Assert.Equal(Assembly.GetEntryAssembly().FullName!, result);
    }

    [Fact]
    public void GetComponentAssembly__ShouldReturnComponentName()
    {
        // Arrange
        var resolver = new AssemblyNameResolver();

        // Act
        var result = resolver.GetComponentAssembly<TestComponent>();

        // Assert
        Assert.Equal(typeof(TestComponent).Assembly.FullName!, result);
    }

    [Fact]
    public void IsExternalAssembly__WithSameAssemblies__ShouldReturnTrue()
    {
        // Arrange
        var mockSet = new MockSet();
        mockSet.AssemblyNameResolverMock.Setup(r => r.GetEntryAssembly()).Returns("Assembly1");
        mockSet.AssemblyNameResolverMock.Setup(r => r.GetComponentAssembly<TestComponent>()).Returns("Assembly2");

        var resolver = mockSet.Create();

        // Act
        var result = resolver.IsExternalAssembly<TestComponent>();

        // Assert
        Assert.True(result);
        mockSet.VerifyAll();
    }

    [Fact]
    public void IsExternalAssembly__WithDifferentAssemblies__ShouldReturnFalse()
    {
        // Arrange
        var mockSet = new MockSet();
        mockSet.AssemblyNameResolverMock.Setup(r => r.GetEntryAssembly()).Returns("Assembly1");
        mockSet.AssemblyNameResolverMock.Setup(r => r.GetComponentAssembly<TestComponent>()).Returns("Assembly1");

        var resolver = mockSet.Create();

        // Act
        var result = resolver.IsExternalAssembly<TestComponent>();

        // Assert
        Assert.False(result);
        mockSet.VerifyAll();
    }

    private class MockSet
    {
        public Mock<AssemblyNameResolver> AssemblyNameResolverMock { get; } = new(MockBehavior.Strict);

        public AssemblyNameResolver Create() => AssemblyNameResolverMock.Object;

        public void VerifyAll()
        {
            AssemblyNameResolverMock.VerifyAll();
        }
    }

    private class TestComponent : ComponentBase
    {
    }
}