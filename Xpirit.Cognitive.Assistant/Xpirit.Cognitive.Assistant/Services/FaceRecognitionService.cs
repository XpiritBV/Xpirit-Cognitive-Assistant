using Microsoft.ProjectOxford.Emotion;
using Microsoft.ProjectOxford.Face;
using Microsoft.ProjectOxford.Vision;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xpirit.Cognitive.Assistant.Repository.Implementation;
using Xpirit.Cognitive.Assistant.Repository.Model;

namespace Xpirit.Cognitive.Assistant.Services
{
    public class FaceRecognitionService : IFaceRecognitionService
    {
        private EmotionServiceClient _emotionClient = null;
        private FaceServiceClient _faceClient = null;
        private VisionServiceClient _visionClient = null;

        public FaceRecognitionService()
        {
            _faceClient = new FaceServiceClient(ApiKeys.FACE_API_KEY, "https://westeurope.api.cognitive.microsoft.com/face/v1.0");
            _emotionClient = new EmotionServiceClient(ApiKeys.EMOTION_API_KEY, "https://westus.api.cognitive.microsoft.com/emotion/v1.0");
            _visionClient = new VisionServiceClient(ApiKeys.VISION_API_KEY, "https://westeurope.api.cognitive.microsoft.com/vision/v1.0");
        }

        public async Task<List<Person>> FindPersonsInImage(Stream image)
        {
            var attrs = new List<FaceAttributeType> { FaceAttributeType.Age,
                FaceAttributeType.Gender, FaceAttributeType.HeadPose };
            var faces = await _faceClient.DetectAsync(image, returnFaceAttributes: attrs);

            var persons = await _faceClient.IdentifyAsync("1b1c4d55-49f8-4f25-a939-c045dee9e879", faces.Select(f => f.FaceId).ToArray());

            List<Person> personList = new List<Person>();
            foreach (var person in persons)
            {
                PersonDataRepository rep = new PersonDataRepository(ApiKeys.STORAGEKEY);
                if (person.Candidates != null && person.Candidates.Count() > 0)
                {
                    var result = await rep.FindPerson(person.Candidates[0].PersonId, new Guid("1b1c4d55-49f8-4f25-a939-c045dee9e879"));
                    personList.Add(result);
                }
            }

            return personList;
        }
    }
}
