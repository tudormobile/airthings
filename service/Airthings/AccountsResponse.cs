namespace Tudormobile.Airthings;

public class AccountsResponse : ApiResponse
{
    public List<AccountResponse> Accounts { get; set; } = [];
}
