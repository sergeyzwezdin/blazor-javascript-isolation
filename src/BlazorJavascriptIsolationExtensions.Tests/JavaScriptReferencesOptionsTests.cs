using Microsoft.AspNetCore.Components;
using Xunit;

namespace BlazorJavascriptIsolationExtensions.Tests;

public class JavaScriptReferencesOptionsTests
{
    [Fact]
    public void ResolvePathFormatter__WithDefaultFormatter__ShouldReturnDefaultPath()
    {
        // Arrange
        var options = new JavaScriptReferencesOptions();

        // Act
        var pathFormatter = options.ResolvePathFormatter<TestComponent>();
        var result = pathFormatter(typeof(TestComponent).Assembly.GetName().Name!, nameof(TestComponent), false);
        var resultForExternalAssembly =
            pathFormatter(typeof(TestComponent).Assembly.GetName().Name!, nameof(TestComponent), true);

        // Assert
        Assert.Equal("./TestComponent.razor.js", result);
        Assert.Equal($"./_content/{typeof(TestComponent).Assembly.GetName().Name!}/TestComponent.razor.js", resultForExternalAssembly);
    }

    [Fact]
    public void ResolvePathFormatter__WithCustomDefaultFormatter__ShouldReturnPath()
    {
        // Arrange
        var options = new JavaScriptReferencesOptions()
            .MapDefault((assembly, component, isExternalAssembly) => $"./custom/{assembly}/{component}.razor.js");

        // Act
        var pathFormatter = options.ResolvePathFormatter<TestComponent>();
        var result = pathFormatter(typeof(TestComponent).Assembly.GetName().Name!, nameof(TestComponent), false);
        var resultForExternalAssembly =
            pathFormatter(typeof(TestComponent).Assembly.GetName().Name!, nameof(TestComponent), true);

        // Assert
        Assert.Equal($"./custom/{typeof(TestComponent).Assembly.GetName().Name!}/TestComponent.razor.js", result);
    }

    [Fact]
    public void ResolvePathFormatter__WithAssemblyMapping__ShouldReturnPath()
    {
        // Arrange
        var options = new JavaScriptReferencesOptions()
            .MapAssembly(typeof(TestComponent).Assembly.FullName!, (assembly, component, isExternalAssembly) => $"./custom/{assembly}/{component}.razor.js");

        // Act
        var pathFormatter = options.ResolvePathFormatter<TestComponent>();
        var result = pathFormatter(typeof(TestComponent).Assembly.GetName().Name!, nameof(TestComponent), false);

        // Assert
        Assert.Equal($"./custom/{typeof(TestComponent).Assembly.GetName().Name}/{nameof(TestComponent)}.razor.js", result);
    }

    [Fact]
    public void ResolvePathFormatter__WithNamespaceMapping__ShouldReturnPath()
    {
        // Arrange
        var options = new JavaScriptReferencesOptions()
            .MapNamespace(typeof(TestComponent).Namespace!, (assembly, component, isExternalAssembly) => $"./custom/{assembly}/{component}.razor.js");

        // Act
        var pathFormatter = options.ResolvePathFormatter<TestComponent>();
        var result = pathFormatter(typeof(TestComponent).Assembly.GetName().Name!, nameof(TestComponent), false);

        // Assert
        Assert.Equal($"./custom/{typeof(TestComponent).Assembly.GetName().Name}/{nameof(TestComponent)}.razor.js", result);
    }

    [Fact]
    public void ResolvePathFormatter__WithComponentMapping__ShouldReturnPath()
    {
        // Arrange
        var options = new JavaScriptReferencesOptions()
            .MapComponent<TestComponent>((assembly, component, isExternalAssembly) => $"./custom/{assembly}/{component}.razor.js");

        // Act
        var pathFormatter = options.ResolvePathFormatter<TestComponent>();
        var result = pathFormatter(typeof(TestComponent).Assembly.GetName().Name!, nameof(TestComponent), false);

        // Assert
        Assert.Equal($"./custom/{typeof(TestComponent).Assembly.GetName().Name}/{nameof(TestComponent)}.razor.js", result);
    }

    private class TestComponent : ComponentBase
    {
    }
}