using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Xpirit.Cognitive.Assistant.Repository.Model
{
    public class Group
    {
        public Guid GroupId;
        public string Name;
        public IEnumerable<Person> Persons;
    }
}
