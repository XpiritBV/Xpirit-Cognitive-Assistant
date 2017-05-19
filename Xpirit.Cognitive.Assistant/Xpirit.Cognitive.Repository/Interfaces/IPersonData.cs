using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xpirit.Cognitive.Assistant.Repository.Model;

namespace Xpirit.Cognitive.Assistant.Repository.Interfaces
{
    public interface IPersonData
    {
        void AddPerson(string    firstName, string lastName, Guid id, Guid groupId);
        Person FindPerson(Guid id);
        Group GetGroup(Guid groupId);
        Group GetGroup(string name);    
        IEnumerable<Group> GetGroups();
        IEnumerable<Guid> GetGroupGuids();
        IEnumerable<string> GetGroupNames();
        void RemovePerson(Guid id, Guid groupId);
    }
}
