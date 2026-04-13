using ThePredictions.Contracts.Account;

namespace ThePredictions.Tests.Builders.Account;

public class UpdateUserDetailsRequestBuilder
{
    private string _firstName = "John";
    private string _lastName = "Smith";
    private string? _phoneNumber = "07123456789";

    public UpdateUserDetailsRequestBuilder WithFirstName(string firstName)
    {
        _firstName = firstName;
        return this;
    }

    public UpdateUserDetailsRequestBuilder WithLastName(string lastName)
    {
        _lastName = lastName;
        return this;
    }

    public UpdateUserDetailsRequestBuilder WithPhoneNumber(string? phoneNumber)
    {
        _phoneNumber = phoneNumber;
        return this;
    }

    public UpdateUserDetailsRequest Build() => new()
    {
        FirstName = _firstName,
        LastName = _lastName,
        PhoneNumber = _phoneNumber
    };
}
