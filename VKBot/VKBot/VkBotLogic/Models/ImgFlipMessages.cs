using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace VKBot.VkBotLogic.Models
{
    //public class CaptionImageRequest
    //{
    //    public string template_id;
    //    public string username;
    //    public string password;
    //    public string text0;
    //    public string text1;
    //    public List<CaptionImageBoxesRequest> boxes;
    //}
    //public class CaptionImageBoxesRequest
    //{
    //    public string text;
    //    public string x;
    //    public string y;
    //    public string width;
    //    public string height;
    //    public string color;
    //    public string outline_color;
    //}

    public class CaptionImageResponse
    {
        public bool success;
        public string error_message;
        public CaptionImageData data;
    }

    public class CaptionImageData
    {
        public string url;
        public string page_url;
    }

    public class GetMemesResponse
    {
        public bool success;
        public string error_message;
        public GetMemesData data;
    }
    public class GetMemesData
    {
        public List<GetMemesMemes> memes;
    }
    public class GetMemesMemes
    {
        public string id;
        public string name;
        public string url;
        public string width;
        public string height;
        public string box_count;
    }
}
