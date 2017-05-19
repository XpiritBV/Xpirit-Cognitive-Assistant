using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xpirit.Cognitive.Assistant.Repository.Interfaces;
using Xpirit.Cognitive.Assistant.Repository.Model;
using Xpirit.Cognitive.Repository.Internals;

namespace Xpirit.Cognitive.Assistant.Repository.Implementation
{
    public class PersonDataRepository : IPersonData
    {
        private const string TABLENAME = "persons";
        CloudTableClient _tableClient;
        CloudTable _table;
        public PersonDataRepository(string storageConnectionString)
        {
            CloudStorageAccount account = CloudStorageAccount.Parse(storageConnectionString);
            CloudTableClient tableClient  = account.CreateCloudTableClient();
            _table = tableClient.GetTableReference(TABLENAME);
            _table.CreateIfNotExists();
            _tableClient = _table.ServiceClient;
        }

        public async Task AddPerson(string firstName, string lastName, Guid id, Guid groupId, string groupName)
        {
            var person = new PersonEntity(id, groupId) {
                FirstName = firstName,
                LastName = lastName,
                GroupName = groupName,
            };

            TableOperation insertOrMergeOperation = TableOperation.InsertOrMerge(person);
            TableResult result = await _table.ExecuteAsync(insertOrMergeOperation);
        }

        public async Task<Person> FindPerson(Guid id, Guid groupId)
        {
            TableOperation retrieveOperation = TableOperation.Retrieve<PersonEntity>(groupId.ToString(), id.ToString());
            TableResult result = await _table.ExecuteAsync(retrieveOperation);
            PersonEntity person = result.Result as PersonEntity;

            return new Person()
            {
                FirstName = person.FirstName,
                LastName = person.LastName,
                Id = new Guid(person.RowKey)
            };
        }

        public Task<Group> GetGroup(Guid groupId)
        {
            throw new NotImplementedException();
        }

        public Task<Group> GetGroup(string name)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<Guid>> GetGroupGuids()
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<string>> GetGroupNames()
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<Group>> GetGroups()
        {
            throw new NotImplementedException();
        }

        public Task RemoveGroup(Guid groupId)
        {
            throw new NotImplementedException();
        }

        public Task RemovePerson(Guid id, Guid groupId)
        {
            throw new NotImplementedException();
        }
    }
}
