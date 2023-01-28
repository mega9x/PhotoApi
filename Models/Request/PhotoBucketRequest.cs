using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.Request
{
    public class PhotoBucketRequest
    {
        public PhotoCategoryBucket Bucket { get; set; }
        public bool IsContinue { get; set; }
    }
}
