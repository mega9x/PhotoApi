using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;

namespace Utils.PhotoUtils
{
    public static class PhotoResizer
    {
        public static Image MakeBigger(this Image photo, int size)
        {
            photo.Mutate(x => x.Resize(photo.Width + size, photo.Height + size));
            return photo;
        }
    }
}
