using System;
using Xunit;

namespace E_Commerce.Tests;

public class UnitTest1
{
    [Fact]
    public void Test1()
    {
        // Arrange
        int a = 2;
        int b = 3;
        
        // Act
        int result = a + b;
        
        // Assert
        Assert.Equal(5, result);
    }
}
