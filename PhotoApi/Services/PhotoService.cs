using ConstStr;
using Models;
using System.Security.Cryptography;
using System.Text.Json;

namespace PhotoApi.Services
{
    public class PhotoService
    {
        private List<PhotoCategoryBucket> _photoRepository = new();
        private List<PhotoCategoryBucket> _newPhotoRepository = new();
        public ILogger<PhotoService> Logger { get; set; }
        public PhotoService(ILogger<PhotoService> logger)
        {
            if (!File.Exists(BackendConfigPath.ImgPath)) File.Create(BackendConfigPath.ImgPath).Close();
            var allTextOfPhotoLists = File.ReadAllText(BackendConfigPath.ImgPath);
            try
            {
                _photoRepository = (JsonSerializer.Deserialize<IEnumerable<PhotoCategoryBucket>>(allTextOfPhotoLists) ??
                                    Array.Empty<PhotoCategoryBucket>()).ToList();
            }
            catch
            {
                // ignore
                _photoRepository = new List<PhotoCategoryBucket>();
            }
            Logger = logger;
        }
        /// <summary>
        /// 根据关键词获取图片
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public IEnumerable<string> GetPhotos(PhotoRequest request)
        {
            var result = _photoRepository.First(x => (x.Age.Contains(request.Age) && request.Age != string.Empty) &&
            (x.Gender.Contains(request.Gender) && request.Gender != string.Empty) &&
                                                (x.Name.Contains(request.Name) && request.Name != string.Empty)) ?? _photoRepository[RandomNumberGenerator.GetInt32(0, _photoRepository.Count)];
            var photoList = new List<string>();
            for (var i = 1; i <= request.Num; i++)
            {
                var l = result.Links[RandomNumberGenerator.GetInt32(0, result.Links.Count)];
                photoList.Add(l);
            }
            return photoList;
        }
        /// <summary>
        /// 随机获取图片，不管类别
        /// </summary>
        /// <param name="num"></param>
        /// <returns></returns>
        public IEnumerable<string> GetPhotosRandomly(int num)
        {
            var AllPhotos = _photoRepository.SelectMany(x => x.Links).ToArray();
            var photoLinksList = new List<string>();
            for (var i = 0; i < num; i++)
            {
                photoLinksList.Add(AllPhotos[RandomNumberGenerator.GetInt32(0, photoLinksList.Count)]);
            }
            return photoLinksList;
        }
        /// <summary>
        /// 更新图片库
        /// </summary>
        /// <param name="list"></param>
        public void UpdatePhotos(IEnumerable<PhotoCategoryBucket> list)
        {
            _newPhotoRepository = list.ToList();
            Save();
        }
        /// <summary>
        /// 按桶更新
        /// </summary>
        /// <param name="photoCategoryBucket"></param>
        public void AddBucketOneByOne(PhotoCategoryBucket photoCategoryBucket)
        {
            _newPhotoRepository.Add(photoCategoryBucket);
        }
        /// <summary>
        /// 保存
        /// </summary>
        public void Save()
        {
            File.WriteAllText(BackendConfigPath.ArchiveFile.Replace("date", DateTime.Today.ToString($"yyyy-M-d dddd {RandomNumberGenerator.GetInt32(0, 9999999)}")), JsonSerializer.Serialize(_photoRepository));
            _photoRepository = _newPhotoRepository.ToList();
            File.Delete(BackendConfigPath.ImgPath);
            File.WriteAllText(BackendConfigPath.ImgPath, JsonSerializer.Serialize(_photoRepository));
            _newPhotoRepository.Clear();
        }
        /// <summary>
        /// 随机抽取样本
        /// </summary>
        /// <param name="num"></param>
        /// <returns></returns>
        public IEnumerable<PhotoCategoryBucket> BuildNewSamples(int num)
        {
            var list = new List<PhotoCategoryBucket>();
            if (!_photoRepository.Any()) return new List<PhotoCategoryBucket>();
            foreach (var photo in _photoRepository)
            {
                if (photo.Clone() is not PhotoCategoryBucket singleList) continue;
                var requestNum = num > photo.Links.Count ? num : photo.Links.Count;
                singleList.Links.Clear();
                for (var i = 0; i <= requestNum; i++)
                {
                    singleList.Links.Add(photo.Links[RandomNumberGenerator.GetInt32(0, photo.Links.Count)]);
                }
                list.Add(singleList);
            }
            return list;
        }
    }
}