using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Options;
using Microsoft.JSInterop;
using Moq;
using Xunit;

namespace BlazorJavascriptIsolationExtensions.Tests;

public class JSObjectReferenceTests
{
    [Fact]
    public async void InvokeAsync__WithoutCancellationToken__ShouldCallJS()
    {
        // Arrange
        var mockSet = new MockSet();
        mockSet.OptionsMock.Setup(m => m.Value).Returns(new JavaScriptReferencesOptions().MapDefault((assembly, component, isExternalAssembly) => "component.js"));
        mockSet.AssemblyResolverMock.Setup(m => m.IsExternalAssembly<TestComponent>()).Returns(false);
        mockSet.JsMock.Setup(m => m.InvokeAsync<IJSObjectReference>(
                "import",
                new object[] { "component.js" }
            ))
            .Returns(ValueTask.FromResult(mockSet.JsObjectReferenceMock.Object));
        mockSet.JsObjectReferenceMock.Setup(m => m.InvokeAsync<string>(
                "test",
                new object[] { "arg1", "arg2" }
            ))
            .Returns(ValueTask.FromResult("OK"));

        var jsReference = mockSet.Create();

        // Act
        var result = await jsReference.InvokeAsync<string>("test", "arg1", "arg2");

        // Assert
        Assert.Equal("OK", result);
        mockSet.VerifyAll();
    }

    [Fact]
    public async void InvokeAsync__WithCancellationToken__ShouldCallJS()
    {
        // Arrange
        var cancellationToken = new CancellationToken();
        var mockSet = new MockSet();
        mockSet.OptionsMock.Setup(m => m.Value).Returns(new JavaScriptReferencesOptions().MapDefault((assembly, component, isExternalAssembly) => "component.js"));
        mockSet.AssemblyResolverMock.Setup(m => m.IsExternalAssembly<TestComponent>()).Returns(false);
        mockSet.JsMock.Setup(m => m.InvokeAsync<IJSObjectReference>(
                "import",
                new object[] { "component.js" }
            ))
            .Returns(ValueTask.FromResult(mockSet.JsObjectReferenceMock.Object));
        mockSet.JsObjectReferenceMock.Setup(m => m.InvokeAsync<string>(
                "test",
                cancellationToken,
                new object[] { "arg1", "arg2" }
            ))
            .Returns(ValueTask.FromResult("OK"));

        var jsReference = mockSet.Create();

        // Act
        var result = await jsReference.InvokeAsync<string>("test", cancellationToken, "arg1", "arg2");

        // Assert
        Assert.Equal("OK", result);
        mockSet.VerifyAll();
    }

    [Fact]
    public async void ExportsTestInvoke__WithoutCancellationToken__ShouldCallJS()
    {
        // Arrange
        var mockSet = new MockSet();
        mockSet.OptionsMock.Setup(m => m.Value).Returns(new JavaScriptReferencesOptions().MapDefault((assembly, component, isExternalAssembly) => "component.js"));
        mockSet.AssemblyResolverMock.Setup(m => m.IsExternalAssembly<TestComponent>()).Returns(false);
        mockSet.JsMock.Setup(m => m.InvokeAsync<IJSObjectReference>(
                "import",
                new object[] { "component.js" }
            ))
            .Returns(ValueTask.FromResult(mockSet.JsObjectReferenceMock.Object));
        mockSet.JsObjectReferenceMock.Setup(m => m.InvokeAsync<string>(
                "test",
                new object[] { "arg1", "arg2" }
            ))
            .Returns(ValueTask.FromResult("OK"));

        var jsReference = mockSet.Create();

        // Act
        var result = await jsReference.Exports.test<string>("arg1", "arg2");

        // Assert
        Assert.Equal("OK", result);
        mockSet.VerifyAll();
    }

    private class MockSet
    {
        public Mock<IJSRuntime> JsMock { get; } = new(MockBehavior.Strict);

        public Mock<IJSObjectReference> JsObjectReferenceMock { get; } = new(MockBehavior.Strict);

        public Mock<IOptions<JavaScriptReferencesOptions>> OptionsMock { get; } = new(MockBehavior.Strict);

        public Mock<IAssemblyNameResolver> AssemblyResolverMock { get; } = new(MockBehavior.Strict);

        public JSObjectReference<TestComponent> Create() => new(JsMock.Object, OptionsMock.Object, AssemblyResolverMock.Object);

        public void VerifyAll()
        {
            JsMock.VerifyAll();
            JsObjectReferenceMock.VerifyAll();
            OptionsMock.VerifyAll();
            AssemblyResolverMock.VerifyAll();
        }
    }

    private class TestComponent : ComponentBase
    {
    }
}