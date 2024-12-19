using System.Collections.Generic;
using CodeChallenge.Models;

namespace CodeChallenge.Repositories
{
    public interface ICompensationRepository
    {
        List<Compensation> GetCompensations(string id);
    }
}
