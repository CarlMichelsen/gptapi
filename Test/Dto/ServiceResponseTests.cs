using Domain.Dto;

namespace Test.Dto;

public class ServiceResponseTests
{
    [Fact]
    public void ServiceResponse_WithData_AndError_NotOk()
    {
        // Arrange
        // Act
        var res = new ServiceResponse<object>
        {
            Data = Guid.NewGuid(),
            Errors = new List<string>
            {
                "This is an error even though there is data",
            },
        };

        // Assert
        Assert.False(res.Ok);
    }

    [Fact]
    public void ServiceResponse_WithData_AndNoError_Ok()
    {
        // Arrange
        // Act
        var res = new ServiceResponse<object>
        {
            Data = Guid.NewGuid(),
        };

        // Assert
        Assert.True(res.Ok);
    }

    [Fact]
    public void ServiceResponse_WithNoData_AndNoError_NotOk()
    {
        // Arrange
        // Act
        var res = new ServiceResponse<object>
        {
            Data = default!,
        };

        // Assert
        Assert.False(res.Ok);
    }
}
