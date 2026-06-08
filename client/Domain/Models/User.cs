// =============================================================================
// Author: Oliver
// =============================================================================

using client.Application.Interfaces;

namespace client.Domain.Models;

public class User : IUser
{
    public int Id { get; }
    public string FirstName { get; }
    public string LastName { get; }
    public string WorkMail { get; }

    public string FullName => $"{FirstName} {LastName}";

    // Author: Oliver
    public User(int id, string firstName, string lastName, string workMail)
    {
        Id = id;
        FirstName = firstName;
        LastName = lastName;
        WorkMail = workMail;
    }
}
