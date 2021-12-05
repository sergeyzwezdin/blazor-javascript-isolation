using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Moq;
using Xunit;

namespace BlazorJavascriptIsolationExtensions.Tests;

public class JSRuntimeExtensionsTests
{
    [Fact]
    public async void ImportComponentScriptAsync__WithDefaultPathFormatter__ShouldReturnJSModule()
    {
        // Arrange
        var mockSet = new MockSet();
        mockSet.JSRuntimeMock.Setup(m => m.InvokeAsync<IJSObjectReference>(
                "import",
                new object[] { "component.js" }
            ))
            .Returns(ValueTask.FromResult(mockSet.JsObjectReferenceMock.Object));
        mockSet.PathFormatterResolverMock.Setup(m => m.ResolvePathFormatter<TestComponent>())
            .Returns((string assemblyName, string componentName, bool isExternalAssembly) => "component.js");
        mockSet.AssemblyNameResolverMock.Setup(m => m.IsExternalAssembly<TestComponent>())
            .Returns(false);

        // Act
        var result = await mockSet.JSRuntimeMock.Object
            .ImportComponentScriptAsync<TestComponent>(
                mockSet.PathFormatterResolverMock.Object,
                mockSet.AssemblyNameResolverMock.Object
            );

        // Assert
        Assert.Equal(mockSet.JsObjectReferenceMock.Object, result);
        mockSet.VerifyAll();
    }

    [Fact]
    public async void ImportComponentScriptAsync__WithoutDefaultPathFormatter__ShouldThrowException()
    {
        // Arrange
        var mockSet = new MockSet();
        mockSet.PathFormatterResolverMock.Setup(m => m.ResolvePathFormatter<TestComponent>())
            .Returns(null as PathFormatter);

        // Act
        var result = await Assert.ThrowsAsync<InvalidOperationException>(
            async () =>
            {
                await mockSet.JSRuntimeMock.Object
                    .ImportComponentScriptAsync<TestComponent>(
                        mockSet.PathFormatterResolverMock.Object,
                        mockSet.AssemblyNameResolverMock.Object
                    );
            });

        // Assert
        Assert.Equal("Unable to resolve javascript file path since there is no matching path formatter and the default formatter is not defined.", result.Message);
        mockSet.VerifyAll();
    }

    private class MockSet
    {
        public Mock<IJSRuntime> JSRuntimeMock { get; } = new(MockBehavior.Strict);
        public Mock<IJSObjectReference> JsObjectReferenceMock { get; } = new(MockBehavior.Strict);
        public Mock<IPathFormatterResolver> PathFormatterResolverMock { get; } = new(MockBehavior.Strict);
        public Mock<IAssemblyNameResolver> AssemblyNameResolverMock { get; } = new(MockBehavior.Strict);

        public void VerifyAll()
        {
            JSRuntimeMock.VerifyAll();
            JsObjectReferenceMock.VerifyAll();
            PathFormatterResolverMock.VerifyAll();
            AssemblyNameResolverMock.VerifyAll();
        }
    }

    private class TestComponent : ComponentBase
    {
    }
}