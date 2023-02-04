using ConstStr;
using Models;
using System.Security.Cryptography;
using LiteDB;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Jpeg;
using SixLabors.ImageSharp.Processing;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace PhotoApi.Services
{
    public class PhotoService
    {
        private bool cacheMaked = true;
        private readonly string _photoBucketCollectionName = "photos";
        private readonly string _localPhotoCollection = "local_photos";
        private readonly ILiteCollection<PhotoCategoryBucket> _photoRepository;
        private readonly ILiteCollection<KeyValuePair<string, string>> _localPhotoRepository;
        public System.Timers.Timer Timer { get; }
        public ILogger<PhotoService> Logger { get; set; }
        public PhotoService(ILogger<PhotoService> logger)
        {
            var db = new LiteDatabase(BackendConfigPath.DatabasePath);
            Logger = logger;
            _photoRepository = db.GetCollection<PhotoCategoryBucket>(_photoBucketCollectionName);
            _localPhotoRepository = db.GetCollection<KeyValuePair<string, string>>(_localPhotoCollection);
            Timer = new System.Timers.Timer(60000);
            Timer.Elapsed += async (source, e) =>
            {
                if ((DateTime.Now.Hour == 00 || DateTime.Now.Hour == 24) && cacheMaked) await MakeCache();
            };
            Timer.AutoReset = true;
        }
        /// <summary>
        /// 根据关键词获取图片
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public async Task<IEnumerable<string>> GetPhotos(PhotoRequest request)
        {
            var all = _photoRepository.Query();
            var query = all.Where(x => x.Age.Contains(request.Age) && x.Gender.Contains(request.Gender) &&
                                                                        (x.Name.Contains(request.Name) || request.Name == string.Empty));
            if (all.Count() <= 0 || query.Count() <= 0) return new List<string>();
            var bucket = query.Count() <= 0 ? all.ToArray()[RandomNumberGenerator.GetInt32(0, all.Count())] : query.First();
            var photoList = new List<string>();
            for (var i = 1; i <= request.Num; i++)
            {
                var l = bucket.Links[RandomNumberGenerator.GetInt32(0, bucket.Links.Count)];
                photoList.Add(l);
            }
            return await HandleHrefs(photoList);
        }

        public async Task<IEnumerable<string>> GetPhotoGroup(PhotoRequest request)
        {
            var all = _photoRepository.Query();
            var query = all.Where(x => x.Age.Contains(request.Age) && x.Gender.Contains(request.Gender) &&
                                       (x.Name.Contains(request.Name) || request.Name == string.Empty));
            if (all.Count() <= 0 || query.Count() <= 0) return new List<string>();
            var bucket = query.Count() <= 0 ? all.ToArray()[RandomNumberGenerator.GetInt32(0, all.Count())] : query.First();
            var index = RandomNumberGenerator.GetInt32(0, bucket.Links.Count - request.Num);
            if (request.Num > bucket.Links.Count)
            {
                index = 0;
            }
            return await HandleHrefs(bucket.Links.GetRange(index, request.Num));
        }
        /// <summary>
        /// 随机获取图片，不管类别
        /// </summary>
        /// <param name="num"></param>
        /// <returns></returns>
        public async Task<IEnumerable<string>> GetPhotosRandomly(int num)
        {
            var all = _photoRepository.Query().ToList();
            var allPhoto = all.SelectMany(x => x.Links).ToArray();
            var photoLinksList = new List<string>();
            if (allPhoto.Length == 0) return photoLinksList;
            for (var i = 1; i < num; i++)
            {
                photoLinksList.Add(allPhoto[RandomNumberGenerator.GetInt32(0, allPhoto.Length)]);
            }
            return await HandleHrefs(photoLinksList);
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

        public async Task MakeCache()
        {
            cacheMaked = false;
            var all = _photoRepository.Query().ToList();
            var allPhoto = all.SelectMany(x => x.Links).ToArray();
            await HandleHrefs(allPhoto);
            cacheMaked = true;
        }
        private async Task<IEnumerable<string>> HandleHrefs(IEnumerable<string> hrefs)
        {
            var result = new List<string> ();
            foreach (var href in hrefs)
            {
                try
                {
                    result.Add(await HandlerHref(href));
                }
                catch
                {
                    // ignore
                }
            }
            return result;
        }
        private async Task<string> HandlerHref(string href)
        {
            var localPhotos = _localPhotoRepository.Query().ToArray();
            try
            {
                var found = localPhotos.First(x => x.Key == href);
                return found.Value;
            }
            catch (Exception e)
            {
                // ignore
            }

            var client = new HttpClient();
            var stream = await client.GetStreamAsync(href);
            using var image = await Image.LoadAsync(stream);
            image.Mutate(x => x.Resize(image.Width + 200, image.Height + 200));
            // var Stream = new MemoryStream();
            // await image.SaveAsJpegAsync(imageStream);
            // using var jpg = await Image.LoadAsync(imageStream);
            var b64string = image.ToBase64String(JpegFormat.Instance);
            _localPhotoRepository.Insert( new KeyValuePair<string, string>(href, b64string));
            return b64string;
        }
    }
}