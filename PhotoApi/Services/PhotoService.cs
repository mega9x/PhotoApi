using ConstStr;
using Models;
using System.Security.Cryptography;
using LiteDB;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace PhotoApi.Services
{
    public class PhotoService
    {
        private LiteDatabase _photoRepository = new(BackendConfigPath.DatabasePath);
        private readonly string _collectionName = "photos";
        public ILogger<PhotoService> Logger { get; set; }
        public PhotoService(ILogger<PhotoService> logger)
        {
            Logger = logger;
        }
        /// <summary>
        /// 根据关键词获取图片
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public IEnumerable<string> GetPhotos(PhotoRequest request)
        {
            var result = _photoRepository.GetCollection<PhotoCategoryBucket>(_collectionName);
            var all = result.Query();
            var query = all.Where(x => (x.Age.Contains(request.Age) && request.Age != string.Empty) &&
                                       (x.Gender.Contains(request.Gender) && request.Gender != string.Empty) &&
                                       (x.Name.Contains(request.Name) && request.Name != string.Empty));
            PhotoCategoryBucket bucket;
            bucket = query.Count() <= 0 ? all.ToArray()[RandomNumberGenerator.GetInt32(0, all.Count())] : query.First();
            var photoList = new List<string>();
            for (var i = 1; i <= request.Num; i++)
            {
                var l = bucket.Links[RandomNumberGenerator.GetInt32(0, bucket.Links.Count)];
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
            var result = _photoRepository.GetCollection<PhotoCategoryBucket>(_collectionName);
            var all = result.Query().ToList();
            var allPhoto = all.SelectMany(x => x.Links).ToArray();
            var photoLinksList = new List<string>();
            for (var i = 1; i < num; i++)
            {
                photoLinksList.Add(allPhoto[RandomNumberGenerator.GetInt32(0, allPhoto.Length)]);
            }
            return photoLinksList;
        }
        /// <summary>
        /// 按桶更新
        /// </summary>
        /// <param name="photoCategoryBucket"></param>
        public void AddBucketOneByOne(PhotoCategoryBucket photoCategoryBucket)
        {
            var result = _photoRepository.GetCollection<PhotoCategoryBucket>(_collectionName);
            var queryed = result.Query().Where(x =>
                x.Age == photoCategoryBucket.Age && x.Gender == photoCategoryBucket.Gender &&
                x.Name == photoCategoryBucket.Name);
            if (queryed.Count() <= 0)
            {
                result.Insert(photoCategoryBucket);
                return;
            }

            var sameBucket = queryed.First();
            var newLinks = photoCategoryBucket.Links;
            newLinks.AddRange(sameBucket.Links);
            sameBucket.Links = newLinks.Distinct().ToList();
            result.Update(sameBucket);
        }
        /// <summary>
        /// 随机抽取样本
        /// </summary>
        /// <param name="num"></param>
        /// <returns></returns>
        public IEnumerable<PhotoCategoryBucket> BuildNewSamples(int num)
        {
            var result = _photoRepository.GetCollection<PhotoCategoryBucket>(_collectionName).Query().ToList();
            var list = new List<PhotoCategoryBucket>();
            if (!result.Any()) return new List<PhotoCategoryBucket>();
            foreach (var photo in result)
            {
                if (photo.Clone() is not PhotoCategoryBucket singleList) continue;
                var requestNum = num > photo.Links.Count ? num : photo.Links.Count;
                singleList.Links.Clear();
                for (var i = 1; i <= requestNum; i++)
                {
                    singleList.Links.Add(photo.Links[RandomNumberGenerator.GetInt32(0, photo.Links.Count)]);
                }
                list.Add(singleList);
            }
            return list;
        }
    }
}