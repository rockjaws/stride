namespace client.Application.Interfaces;

public interface IUser
{
    int Id { get; }
    string FirstName { get; }
    string LastName { get; }
    string WorkMail { get; }
}
