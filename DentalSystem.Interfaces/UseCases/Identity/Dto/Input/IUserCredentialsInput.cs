namespace DentalSystem.Interfaces.UseCases.Identity.Dto.Input
{
    public interface IUserCredentialsInput
    {
        string UserName { get; set; }

        string Password { get; set; }
    }
}