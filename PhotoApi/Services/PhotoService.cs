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
        private List<PhotoList> _photoRepository = JsonSerializer.Deserialize<IEnumerable<PhotoList>>(BackendConfig.IMG_PATH).ToList();
        public ILogger<PhotoService> Logger { get; set; }
        public PhotoService(ILogger<PhotoService> logger)
        {
            Logger = logger;
        }
        public IEnumerable<string> GetPhotos(int num, string age, string keywords, string gender)
        {
            var result = _photoRepository.First(x => (x.Age.Contains(age) && age != string.Empty) ||
                                                (x.Gender.Contains(gender) && gender != string.Empty) ||
                                                (x.Name.Contains(keywords) && keywords != string.Empty)) ?? _photoRepository[RandomNumberGenerator.GetInt32(0, _photoRepository.Count)];
            var photoList = new List<string>();
            for (var i = 1; i <= num; i++)
            {
                var l = result.Links[RandomNumberGenerator.GetInt32(0, result.Links.Count)];
                if (l is not null)
                {
                    photoList.Add(l);
                }
            }
            return photoList;
        }
        public void LoadPhoto(List<PhotoList> list)
        {
            _photoRepository = list;
            File.Delete(BackendConfig.IMG_PATH);
            File.Create(BackendConfig.IMG_PATH).Close();
        }
    }
}