using System.Collections.Generic;
using CodeChallenge.Models;

namespace CodeChallenge.Repositories
{
    public interface ICompensationRepository
    {
        Compensation Add(Compensation compensation);
        List<Compensation> GetByEmployeeId(string id);
    }
}
