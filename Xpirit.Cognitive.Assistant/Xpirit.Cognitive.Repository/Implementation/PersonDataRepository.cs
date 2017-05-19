using Microsoft.WindowsAzure.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xpirit.Cognitive.Assistant.Repository.Interfaces;
using Xpirit.Cognitive.Assistant.Repository.Model;

namespace Xpirit.Cognitive.Assistant.Repository.Implementation
{
    public class PersonDataRepository : IPersonData
    {
        CloudStorageAccount _account;
        public PersonDataRepository(string storageConnectionString)
        {
            _account = CloudStorageAccount.Parse(storageConnectionString);
        }

        public void AddPerson(string firstName, string lastName, Guid id, Guid groupId)
        {
            throw new NotImplementedException();
        }

        public Person FindPerson(Guid id)
        {
            throw new NotImplementedException();
        }

        public Group GetGroup(Guid groupId)
        {
            throw new NotImplementedException();
        }

        public Group GetGroup(string name)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<Guid> GetGroupGuids()
        {
            throw new NotImplementedException();
        }

        public IEnumerable<string> GetGroupNames()
        {
            throw new NotImplementedException();
        }

        public IEnumerable<Group> GetGroups()
        {
            throw new NotImplementedException();
        }

        public void RemovePerson(Guid id, Guid groupId)
        {
            throw new NotImplementedException();
        }
    }
}
