using Microsoft.ProjectOxford.Face;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

using ClientContract = Microsoft.ProjectOxford.Face.Contract;
using System.Diagnostics;
using Xpirit.Cognitive.Assistant.Repository.Interfaces;
using Xpirit.Cognitive.Assistant.Repository.Implementation;

namespace Xpirit.Cognitive.FaceApi
{
    public class FaceApiClient
    {
        private FaceServiceClient _client;
        private IPersonData _repository;

        public FaceApiClient(string subscriptionKey)
        {
            string apiEndPoint = ConfigurationManager.AppSettings["FaceApiUrl"];
            _client = new FaceServiceClient(subscriptionKey, apiEndPoint);
            _repository = new PersonDataRepository(ConfigurationManager.ConnectionStrings["StorageConnectionString"].ConnectionString);
        }

        /// <summary>
        /// Tests whether a group exists or not
        /// </summary>
        /// <param name="groupName"></param>
        /// <returns></returns>
        public async Task<bool> PersonGroupExistsAsync(Guid groupName)
        {
            bool groupExists = false;
            try
            {
                Debug.WriteLine($"Request: Group {groupName} will be used to build a person database. Checking whether the group exists.");
                await _client.GetPersonGroupAsync(groupName.ToString());
                groupExists = true;
                Debug.WriteLine($"Response: Group {groupName} exists.");
            }
            catch (FaceAPIException ex)
            {
                if (ex.ErrorCode != "PersonGroupNotFound")
                {
                    Debug.WriteLine($"Response: {ex.ErrorCode}. {ex.ErrorMessage}");
                    throw ex;
                }
                else
                {
                    Debug.WriteLine($"Response: Group {groupName} did not exist previously.");
                }
            }
            return groupExists;
        }

        public async Task DeletePersonGroupAsync(Guid groupName)
        {
            //Delete from FaceServiceClient
            await _client.DeletePersonGroupAsync(groupName.ToString());

            //Delete from repository - to be implemented
            await _repository.RemoveGroup(groupName);
        }

        /// <summary>
        /// Creates a group
        /// </summary>
        /// <param name="groupName">The name of the group</param>
        /// <returns></returns>
        public async Task CreatePersonGroupAsync(Guid groupName)
        {
            //Create in FaceServiceClient
            try
            {
                await _client.CreatePersonGroupAsync(groupName.ToString(), groupName.ToString());
                Debug.WriteLine($"Response: Success. Group \"{groupName}\" created");
            }
            catch (FaceAPIException ex)
            {
                Debug.WriteLine("Response: {0}. {1}", ex.ErrorCode, ex.ErrorMessage);
                throw new Exception($"Error creating group: \"{ex.ErrorCode}. {ex.ErrorMessage}\"");
            }
        }

        /// <summary>
        /// Creates a person in a group
        /// </summary>
        /// <param name="groupName">Guid identifying the group</param>
        /// <param name="personName">Name of the person (e.g. "Kees")</param>
        /// <returns>Guid identifying the person in the group</returns>
        public async Task<Guid> CreatePersonAsync(Guid groupName, string personName)
        {
            //Create person in FaceServiceClient: 
            Guid personId = (await _client.CreatePersonAsync(groupName.ToString(), personName)).PersonId;

            //Create person in repository
            Name n = new Name(personName);
            await _repository.AddPerson(n.FirstName, n.LastName, personId, groupName, "Xpirit");

            return personId;
        }

        /// <summary>
        /// Adds a detected face to a person in a group
        /// </summary>
        /// <param name="groupName"></param>
        /// <param name="personId"></param>
        /// <param name="fStream"></param>
        /// <param name="imgPath"></param>
        /// <returns></returns>
        public async Task<Tuple<string, ClientContract.AddPersistedFaceResult>> AddPersonFaceAsync(Guid groupName, Guid personId, FileStream fStream, string imgPath)
        {
            //Add face to faceserviceclient
            var persistFace = await _client.AddPersonFaceAsync(groupName.ToString(), personId, fStream, imgPath);
            var retval =  new Tuple<string, ClientContract.AddPersistedFaceResult>(imgPath, persistFace);

            //Add face to repository

            return retval;
        }

        /// <summary>
        /// Trains the Face API for the specified group. Should be called after adding faces.
        /// </summary>
        /// <param name="groupName">Guid of the group you wish to train</param>
        /// <returns></returns>
        public async Task TrainPersonGroupAsync(Guid groupName)
        {
            await _client.TrainPersonGroupAsync(groupName.ToString());
        }

        public async Task<ClientContract.TrainingStatus> GetPersonGroupTrainingStatusAsync(Guid groupName)
        {
            return await _client.GetPersonGroupTrainingStatusAsync(groupName.ToString());
        }
    }
}
