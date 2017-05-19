using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Xpirit.Cognitive.FaceApi
{
    public class Name
    {
        public string FirstName {get; set;}
        public string LastName { get; set; }

        public string FullName {
            get
            {
                return $"{FirstName} {LastName}";
            }
            set
            {
                char[] splitChar = { ' ' };
                string[] splitted = value.Split(splitChar, 2);
                FirstName = splitted.ElementAtOrDefault(0)?.ToString();
                LastName = splitted.ElementAtOrDefault(1)?.ToString();
            }
        }

        public Name(string fullName)
        {
            this.FullName = fullName;
        }

    }
}
