using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Xpirit.Cognitive.Assistant.Repository.Model;

namespace Xpirit.Cognitive.Assistant.Services
{
    public interface IFaceRecognitionService
    {
        Task<List<Person>> FindPersonsInImage(Stream image);
    }
}