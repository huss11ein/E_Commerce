using Xunit;

namespace E_Commerce.Tests
{
    public class BasicTests
    {
        [Fact]
        public void SimpleAssertionTest()
        {
            // Arrange
            int a = 2;
            int b = 3;
            
            // Act
            int result = a + b;
            
            // Assert
            Assert.Equal(5, result);
        }
        
        [Theory]
        [InlineData(1, 2, 3)]
        [InlineData(5, 5, 10)]
        [InlineData(0, 0, 0)]
        public void ParameterizedTest(int x, int y, int expected)
        {
            // Act
            int result = x + y;
            
            // Assert
            Assert.Equal(expected, result);
        }
    }
} 
