using System    ;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xpirit.Cognitive.Assistant.Repository.Model;

namespace Xpirit.Cognitive.Assistant.Repository.Interfaces
{
    public interface IPersonData
    {
        Task AddPerson(string firstName, string lastName, Guid id, Guid groupId, string groupName);
        Task<Group> GetGroup(Guid groupId);
        Task<Group> GetGroup(string name);
        Task<IEnumerable<Group>> GetGroups();
        Task<IEnumerable<Guid>> GetGroupGuids();
        Task<IEnumerable<string>> GetGroupNames();
        Task RemovePerson(Guid id, Guid groupId);
    }
}
