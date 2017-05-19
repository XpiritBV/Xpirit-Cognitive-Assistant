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

            List<string> personList = new List<string>();
            foreach (var face in faces)
            {
                personList.Add(face.FaceId.ToString());
            }

            return personList;
        }
    }
}
