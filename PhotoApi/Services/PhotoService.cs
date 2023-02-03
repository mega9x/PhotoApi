using ConstStr;
using Models;
using System.Security.Cryptography;
using LiteDB;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace PhotoApi.Services
{
    public class PhotoService
    {

        private readonly string _collectionName = "photos";
        private readonly ILiteCollection<PhotoCategoryBucket> _photoRepository;
        public ILogger<PhotoService> Logger { get; set; }
        public PhotoService(ILogger<PhotoService> logger)
        {
            Logger = logger;
            _photoRepository =
                new LiteDatabase(BackendConfigPath.DatabasePath).GetCollection<PhotoCategoryBucket>(_collectionName);
        }
        /// <summary>
        /// 根据关键词获取图片
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public IEnumerable<string> GetPhotos(PhotoRequest request)
        {
            var all = _photoRepository.Query();
            var query = all.Where(x => x.Age.Contains(request.Age) && x.Gender.Contains(request.Gender) &&
                                                                        (x.Name.Contains(request.Name) || request.Name == string.Empty));
            if (all.Count() <= 0 || query.Count() <= 0) return new List<string>();
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
            var all = _photoRepository.Query().ToList();
            var allPhoto = all.SelectMany(x => x.Links).ToArray();
            var photoLinksList = new List<string>();
            if (allPhoto.Length == 0) return photoLinksList;
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
            var queryed = _photoRepository.Query().Where(x =>
                x.Age == photoCategoryBucket.Age && x.Gender == photoCategoryBucket.Gender &&
                x.Name == photoCategoryBucket.Name);
            if (queryed.Count() <= 0)
            {
                _photoRepository.Insert(photoCategoryBucket);
                return;
            }

            var sameBucket = queryed.First();
            var newLinks = photoCategoryBucket.Links;
            newLinks.AddRange(sameBucket.Links);
            sameBucket.Links = newLinks.Distinct().ToList();
            _photoRepository.Update(sameBucket);
        }
        /// <summary>
        /// 随机抽取样本
        /// </summary>
        /// <param name="num"></param>
        /// <returns></returns>
        public IEnumerable<PhotoCategoryBucket> BuildNewSamples(int num)
        {
            var allList = _photoRepository.Query();
            var list = new List<PhotoCategoryBucket>();
            if (!allList.ToArray().Any()) return new List<PhotoCategoryBucket>();
            foreach (var photo in allList.ToArray())
            {
                if (photo.Links.Count <= 0) continue;
                var singleList = new PhotoCategoryBucket()
                {
                    Age = photo.Age,
                    Gender = photo.Gender,
                    Id = photo.Id,
                    Links = new(),
                    Name = photo.Name
                };
                var requestNum = num > photo.Links.Count ? num : photo.Links.Count;
                singleList.Links.Clear();
                for (var i = 1; i <= requestNum; i++)
                {
                    var index = RandomNumberGenerator.GetInt32(0, photo.Links.Count);
                    Console.WriteLine(photo.Links.Count);
                    Console.WriteLine(index);
                    singleList.Links.Add(photo.Links[index]);
                }
                list.Add(singleList);
            }
            return list;
        }
    }
}