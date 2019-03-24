using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VKBot.VkontakteBot.Models
{
    public class RegisterResponse : IRegisterResponse
    {
        public RegisterResponseData response;
        public bool Success { get; set; }
    }
    public class RegisterResponseData
    {
        public string ts;
        public string new_ts;
        public string server;
        public string key;
        public string upload_url;
        public int album_id;
        public int user_id;
    }

    public class PhotoUploadResponse
    {
        public string server;
        public string photo;
        public string hash;
    }

    public class PhotoSaveResponse
    {
        public PhotoSaveData[] response;
    }

    public class PhotoSaveData
    {
        public string id;
        public int album_id;
        public int owner_id;
        public object[] sizes;
        public string text;
        public int date;
        public string access_key;
    }

    public class UpdateMessageDataAction
    {
        public string type { get; set; }
        public int member_id { get; set; }
    }

    public class UpdateMessageData
    {
        public int date { get; set; }
        public int from_id { get; set; }
        public int id { get; set; }
        public int @out { get; set; }
        public int peer_id { get; set; }
        public string text { get; set; }
        public int conversation_message_id { get; set; }
        public UpdateMessageDataAction action { get; set; }
        public List<object> fwd_messages { get; set; }
        public bool important { get; set; }
        public int random_id { get; set; }
        public List<object> attachments { get; set; }
        public bool is_hidden { get; set; }
    }

    public class UpdateMessage : IIncomingMessage
    {
        public string type { get; set; }
        public UpdateMessageData @object { get; set; }
        public int group_id { get; set; }

        public string peer_id => @object.peer_id.ToString();
        public string MessageType => type;
        public string from_id => @object.from_id.ToString();

        public string text => @object.text;
        public List<dynamic> attachments => @object.attachments;
    }

    public class UpdateResponse : IUpdatesResponse
    {
        public string ts { get; set; }
        public List<UpdateMessage> updates { get; set; }
        public string new_ts;
        public int failed;

        public IEnumerable<IIncomingMessage> Updates => updates;
    }


}
