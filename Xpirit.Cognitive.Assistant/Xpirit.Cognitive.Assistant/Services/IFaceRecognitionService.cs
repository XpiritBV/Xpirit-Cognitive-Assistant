using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace Xpirit.Cognitive.Assistant.Services
{
    public interface IFaceRecognitionService
    {
        Task<List<string>> FindPersonsInImage(Stream image);
    }
}