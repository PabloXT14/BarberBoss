using BarberBoss.Application.UseCases.Billings;
using BarberBoss.Communication.Enums;
using BarberBoss.Exception;
using CommonTestUtilities.Requests;
using Shouldly;
using Xunit;

namespace Validators.Tests.Billings.GetAll;

public class GetAllBillingsValidatorTests
{
    [Fact]
    public void Success()
    {
        // Arrange (config the instances that we need to execute our test)
        var validator = new GetAllBillingsValidator();
        var request = RequestGetAllBillingsJsonBuilder.Build();

        // Act (execute the method that we want to test)
        var result = validator.Validate(request);

        // Assert (verify that the result is what we expected)
        result.IsValid.ShouldBeTrue();
    }


    // PAGINATION TESTS
    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void Error_Page_Invalid(int page)
    {
        // Arrange
        var validator = new GetAllBillingsValidator();
        var request = RequestGetAllBillingsJsonBuilder.Build();
        request.Page = page;

        // Act
        var result = validator.Validate(request);

        // Assert
        result.IsValid.ShouldBeFalse();
        result.Errors.ShouldSatisfyAllConditions(
            errors => errors.Count.ShouldBe(1),
            errors => errors.ShouldContain(
                error => error.ErrorMessage.Equals(ResourceErrorMessages.PAGE_MUST_BE_GREATER_THAN_ZERO)
            )
        );
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void Error_PageSize_Invalid(int pageSize)
    {
        // Arrange
        var validator = new GetAllBillingsValidator();
        var request = RequestGetAllBillingsJsonBuilder.Build();
        request.PageSize = pageSize;

        // Act
        var result = validator.Validate(request);

        // Assert
        result.IsValid.ShouldBeFalse();
        result.Errors.ShouldSatisfyAllConditions(
            errors => errors.Count.ShouldBe(1),
            errors => errors.ShouldContain(
                error => error.ErrorMessage.Equals(ResourceErrorMessages.PAGE_SIZE_MUST_BE_GREATER_THAN_ZERO)
            )
        );
    }


    // DATE TESTS

    [Fact]
    public void Error_EndDate_Required_When_StartDate_Is_Provided()
    {
        // Arrange
        var validator = new GetAllBillingsValidator();
        var request = RequestGetAllBillingsJsonBuilder.Build();
        request.EndDate = null;

        // Act
        var result = validator.Validate(request);

        // Assert
        result.IsValid.ShouldBeFalse();
        result.Errors.ShouldSatisfyAllConditions(
            errors => errors.Count.ShouldBe(1),
            errors => errors.ShouldContain(
                error => error.ErrorMessage.Equals(ResourceErrorMessages.END_DATE_IS_REQUIRED_WHEN_START_DATE_IS_PROVIDED)
            )
        );
    }

    [Fact]
    public void Error_StartDate_Required_When_EndDate_Is_Provided()
    {
        // Arrange
        var validator = new GetAllBillingsValidator();
        var request = RequestGetAllBillingsJsonBuilder.Build();
        request.StartDate = null;

        // Act
        var result = validator.Validate(request);

        // Assert
        result.IsValid.ShouldBeFalse();
        result.Errors.ShouldSatisfyAllConditions(
            errors => errors.Count.ShouldBe(1),
            errors => errors.ShouldContain(
                error => error.ErrorMessage.Equals(ResourceErrorMessages.START_DATE_IS_REQUIRED_WHEN_END_DATE_IS_PROVIDED)
            )
        );
    }

    [Fact]
    public void Error_EndDate_Must_Be_Greater_Than_Or_Equal_To_StartDate()
    {
        // Arrange
        var validator = new GetAllBillingsValidator();
        var request = RequestGetAllBillingsJsonBuilder.Build();
        request.StartDate = new DateOnly(2024, 1, 1);
        request.EndDate = new DateOnly(2023, 1, 1);

        // Act
        var result = validator.Validate(request);

        // Assert
        result.IsValid.ShouldBeFalse();
        result.Errors.ShouldSatisfyAllConditions(
            errors => errors.Count.ShouldBe(1),
            errors => errors.ShouldContain(
                error => error.ErrorMessage.Equals(ResourceErrorMessages.END_DATE_MUST_BE_GREATER_THAN_OR_EQUAL_TO_START_DATE)
            )
        );
    }

    // AMOUNT TESTS
    [Fact]
    public void MaxAmount_Is_Required_When_MinAmount_Is_Provided()
    {
        // Arrange
        var validator = new GetAllBillingsValidator();
        var request = RequestGetAllBillingsJsonBuilder.Build();
        request.MaxAmount = null;

        // Act
        var result = validator.Validate(request);

        // Assert
        result.IsValid.ShouldBeFalse();
        result.Errors.ShouldSatisfyAllConditions(
            errors => errors.Count.ShouldBe(1),
            errors => errors.ShouldContain(
                error => error.ErrorMessage.Equals(ResourceErrorMessages.MAX_AMOUNT_IS_REQUIRED_WHEN_MIN_AMOUNT_IS_PROVIDED)
            )
        );
    }

    [Fact]
    public void MinAmount_Is_Required_When_MaxAmount_Is_Provided()
    {
        // Arrange
        var validator = new GetAllBillingsValidator();
        var request = RequestGetAllBillingsJsonBuilder.Build();
        request.MinAmount = null;

        // Act
        var result = validator.Validate(request);

        // Assert
        result.IsValid.ShouldBeFalse();
        result.Errors.ShouldSatisfyAllConditions(
            errors => errors.Count.ShouldBe(1),
            errors => errors.ShouldContain(
                error => error.ErrorMessage.Equals(ResourceErrorMessages.MIN_AMOUNT_IS_REQUIRED_WHEN_MAX_AMOUNT_IS_PROVIDED)
            )
        );
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(-100)]
    public void MinAmount_Must_Be_Greater_Than_Or_Equal_To_Zero(decimal minAmount)
    {
        // Arrange
        var validator = new GetAllBillingsValidator();
        var request = RequestGetAllBillingsJsonBuilder.Build();
        request.MinAmount = minAmount;

        // Act
        var result = validator.Validate(request);

        // Assert
        result.IsValid.ShouldBeFalse();
        result.Errors.ShouldSatisfyAllConditions(
            errors => errors.Count.ShouldBe(1),
            errors => errors.ShouldContain(
                error => error.ErrorMessage.Equals(ResourceErrorMessages.MIN_AMOUNT_MUST_BE_GREATER_THAN_OR_EQUAL_TO_ZERO)
            )
        );
    }

    [Fact]
    public void MaxAmount_Must_Be_Greater_Than_Or_Equal_To_MinAmount()
    {
        // Arrange
        var validator = new GetAllBillingsValidator();
        var request = RequestGetAllBillingsJsonBuilder.Build();
        request.MinAmount = 100; // Set MinAmount to a specific value
        request.MaxAmount = request.MinAmount - 1; // Set MaxAmount to be less than MinAmount

        // Act
        var result = validator.Validate(request);

        // Assert
        result.IsValid.ShouldBeFalse();
        result.Errors.ShouldSatisfyAllConditions(
            errors => errors.Count.ShouldBe(1),
            errors => errors.ShouldContain(
                error => error.ErrorMessage.Equals(ResourceErrorMessages.MAX_AMOUNT_MUST_BE_GREATER_THAN_OR_EQUAL_TO_MIN_AMOUNT)
            )
        );
    }


    // ENUM TESTS

    [Fact]
    public void Error_Status_Invalid()
    {
        // Arrange
        var validator = new GetAllBillingsValidator();
        var request = RequestGetAllBillingsJsonBuilder.Build();
        request.Status = (BillingStatus)999; // Invalid enum value

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

    [Fact]
    public void Error_PaymentMethod_Invalid()
    {
        // Arrange
        var validator = new GetAllBillingsValidator();
        var request = RequestGetAllBillingsJsonBuilder.Build();
        request.PaymentMethod = (PaymentMethod)999; // Invalid enum value

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
}