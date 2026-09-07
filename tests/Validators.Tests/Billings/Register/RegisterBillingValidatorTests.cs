using BarberBoss.Application.UseCases.Billings;
using BarberBoss.Communication.Enums;
using BarberBoss.Exception;
using CommonTestUtilities.Requests;
using Shouldly;
using Xunit;

namespace Validators.Tests.Billings.Register;

public class RegisterBillingValidatorTests
{
    [Fact]
    public void Success()
    {
        // Arrange (config the instances that we need to execute our test)
        var validator = new WriteBillingValidator();

        var request = RequestRegisterBillingJsonBuilder.Build();

        // Act (execute the method that we want to test)
        var result = validator.Validate(request);

        // Assert (verify that the result is what we expected)
        result.IsValid.ShouldBeTrue();
    }

    [Fact]
    public void Error_Date_Future()
    {
        // Arrange
        var validator = new WriteBillingValidator();
        var request = RequestRegisterBillingJsonBuilder.Build();
        request.Date = DateTime.Now.AddDays(1); // Set the date to a future date

        // Act
        var result = validator.Validate(request);

        // Assert
        result.IsValid.ShouldBeFalse();
        result.Errors.ShouldSatisfyAllConditions(
            errors => errors.Count.ShouldBe(1),
            errors => errors.ShouldContain(
                error => error.ErrorMessage.Equals(ResourceErrorMessages.BILLING_DATE_CANNOT_BE_FUTURE)
            )
        );
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData(null)]
    public void Error_BarberName_Empty(string barberName)
    {
        // Arrange
        var validator = new WriteBillingValidator();
        var request = RequestRegisterBillingJsonBuilder.Build();
        request.BarberName = barberName; // Set the BarberName to an empty string,


        // Act
        var result = validator.Validate(request);

        // Assert
        result.IsValid.ShouldBeFalse();
        result.Errors.ShouldSatisfyAllConditions(
            errors => errors.Count.ShouldBe(1),
            errors => errors.ShouldContain(
                error => error.ErrorMessage.Equals(ResourceErrorMessages.BARBER_NAME_REQUIRED)
            )
        );
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData(null)]
    public void Error_ClientName_Empty(string clientName)
    {
        // Arrange
        var validator = new WriteBillingValidator();
        var request = RequestRegisterBillingJsonBuilder.Build();
        request.ClientName = clientName; // Set the ClientName to an empty string,


        // Act
        var result = validator.Validate(request);

        // Assert
        result.IsValid.ShouldBeFalse();
        result.Errors.ShouldSatisfyAllConditions(
            errors => errors.Count.ShouldBe(1),
            errors => errors.ShouldContain(
                error => error.ErrorMessage.Equals(ResourceErrorMessages.CLIENT_NAME_REQUIRED)
            )
        );
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData(null)]
    public void Error_ServiceName_Empty(string serviceName)
    {
        // Arrange
        var validator = new WriteBillingValidator();
        var request = RequestRegisterBillingJsonBuilder.Build();
        request.ServiceName = serviceName; // Set the ServiceName to an empty string,


        // Act
        var result = validator.Validate(request);

        // Assert
        result.IsValid.ShouldBeFalse();
        result.Errors.ShouldSatisfyAllConditions(
            errors => errors.Count.ShouldBe(1),
            errors => errors.ShouldContain(
                error => error.ErrorMessage.Equals(ResourceErrorMessages.SERVICE_NAME_REQUIRED)
            )
        );
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(-2)]
    [InlineData(-7)]
    public void Error_Amount_Invalid(decimal amount)
    {
        // Arrange
        var validator = new WriteBillingValidator();
        var request = RequestRegisterBillingJsonBuilder.Build();
        request.Amount = amount; // Set the Amount to an invalid value,


        // Act
        var result = validator.Validate(request);

        // Assert
        result.IsValid.ShouldBeFalse();
        result.Errors.ShouldSatisfyAllConditions(
            errors => errors.Count.ShouldBe(1),
            errors => errors.ShouldContain(
                error => error.ErrorMessage.Equals(ResourceErrorMessages.AMOUNT_MUST_BE_GREATER_THAN_ZERO)
            )
        );
    }

    [Fact]
    public void Error_PaymentMethod_Invalid()
    {
        // Arrange
        var validator = new WriteBillingValidator();
        var request = RequestRegisterBillingJsonBuilder.Build();
        request.PaymentMethod = (PaymentMethod)999; // Set the PaymentMethod to an invalid value (assuming 999 is not a valid enum value)

        // Act
        var result = validator.Validate(request);

        // Assert
        result.IsValid.ShouldBeFalse();
        result.Errors.ShouldSatisfyAllConditions(
            errors => errors.Count.ShouldBe(1),
            errors => errors.ShouldContain(
                error => error.ErrorMessage.Equals(ResourceErrorMessages.INVALID_PAYMENT_METHOD)
            )
        );
    }

    [Fact]
    public void Error_Status_Invalid()
    {
        // Arrange
        var validator = new WriteBillingValidator();
        var request = RequestRegisterBillingJsonBuilder.Build();
        request.Status = (BillingStatus)999; // Set the Status to an invalid value (assuming 999 is not a valid enum value)

        // Act
        var result = validator.Validate(request);

        // Assert
        result.IsValid.ShouldBeFalse();
        result.Errors.ShouldSatisfyAllConditions(
            errors => errors.Count.ShouldBe(1),
            errors => errors.ShouldContain(
                error => error.ErrorMessage.Equals(ResourceErrorMessages.INVALID_STATUS)
            )
        );
    }
}