using Microsoft.ProjectOxford.Emotion;
using Microsoft.ProjectOxford.Face;
using Microsoft.ProjectOxford.Vision;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

        public async Task<List<string>> FindPersonsInImage(Stream image)
        {
            var attrs = new List<FaceAttributeType> { FaceAttributeType.Age,
                FaceAttributeType.Gender, FaceAttributeType.HeadPose };
            var faces = await _faceClient.DetectAsync(image, returnFaceAttributes: attrs);

            var persons = await _faceClient.IdentifyAsync("1b1c4d55-49f8-4f25-a939-c045dee9e879", faces.Select(f => f.FaceId).ToArray());

            List<string> personList = new List<string>();
            foreach (var person in persons)
            {
                Repository.Implementation.PersonDataRepository rep = new Repository.Implementation.PersonDataRepository(STORAGE_CONNECTIONSTRING);
                var result = await rep.FindPerson(person.Candidates[0], "1b1c4d55-49f8-4f25-a939-c045dee9e879");
                //person.Candidates[0].PersonId 
                //personList.Add(face.FaceId.ToString());
            }

            return personList;
        }
    }
}
