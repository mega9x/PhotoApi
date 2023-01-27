using System.Collections;
using System.Security.Cryptography;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using ConstStr;
using Models;

namespace PhotoApi.Services
{
    public class PhotoService
    {
        private List<PhotoList> _photoRepository;
        public ILogger<PhotoService> Logger { get; set; }
        public PhotoService(ILogger<PhotoService> logger)
        {
            if(!File.Exists(BackendConfig.IMG_PATH)) File.Create(BackendConfig.IMG_PATH).Close();
            var allTextOfPhotoLists = File.ReadAllText(BackendConfig.IMG_PATH);
            try
            {
                _photoRepository = (JsonSerializer.Deserialize<IEnumerable<PhotoList>>(allTextOfPhotoLists) ??
                                    Array.Empty<PhotoList>()).ToList();
            }
            catch
            {
                // ignore
                _photoRepository = new List<PhotoList>();
            }
            Logger = logger;
        }
        public IEnumerable<string> GetPhotos(PhotoRequest request)w
        {
            var result = _photoRepository.First(x => (x.Age.Contains(request.Age) && request.Age != string.Empty) ||
                                                (x.Gender.Contains(request.Gender) && request.Gender != string.Empty) ||
                                                (x.Name.Contains(request.Name) && request.Name != string.Empty)) ?? _photoRepository[RandomNumberGenerator.GetInt32(0, _photoRepository.Count)];
            var photoList = new List<string>();
            for (var i = 1; i <= request.Num; i++)
            {
                var l = result.Links[RandomNumberGenerator.GetInt32(0, result.Links.Count)];
                photoList.Add(l);
            }
            return photoList;
        }
        public void LoadPhoto(List<PhotoList> list)
        {
            
            File.WriteAllText(BackendConfig.ARCHIVE_FILE.Replace("date",DateTime.Today.ToString($"yyyy-M-d dddd {RandomNumberGenerator.GetInt32(0, 9999999)}")), JsonSerializer.Serialize(_photoRepository));
            _photoRepository = list;
            File.Delete(BackendConfig.IMG_PATH);
            File.WriteAllText(BackendConfig.IMG_PATH,JsonSerializer.Serialize(_photoRepository));
        }

        public List<PhotoList> BuildNewSamples()
        {
            var list = new List<PhotoList>();
            foreach (var photo in _photoRepository)
            {
                photo.Links[RandomNumberGenerator.GetInt32(0, photo.Links.Count)]
            }
        }
    }
}