using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage;

namespace Xpirit.Cognitive.Repository.Internals
{
    public class PersonEntity : TableEntity
    {
        public PersonEntity() { }
        public PersonEntity(Guid id, Guid groupId)
        {
            PartitionKey = groupId.ToString();

            RowKey = id.ToString();
        }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string GroupName { get; set; }
 
    }
}
