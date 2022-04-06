using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VKBot.VkBotLogic.Models
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
        public PhotoSize[] sizes;
        public string text;
        public int date;
        public string access_key;
    }

    public class PhotoSize
    {
        public string type;
        public string url;
        public int width;
        public int height;
    }

    public class UpdateMessage : IIncomingMessage
    {
        public int group_id { get; set; }
        public string type { get; set; }
        public string event_id { get; set; }
        public string v { get; set; }
        public UpdateMessageObject @object { get; set; }
        //
        bool hasMessage => @object != null && @object.message != null;
        //
        public string payload => hasMessage ? @object.message.payload : null;

        public string peer_id => hasMessage ? @object.message.peer_id.ToString() : null;
        public string MessageType => type;
        public string from_id => hasMessage? @object.message.from_id.ToString() : null;
        public string date => hasMessage ? @object.message.date.ToString() : null;//DateTime.Now.ToString();

        public string text => hasMessage ? @object.message.text : null;
        public List<Attachment> attachments => hasMessage ? @object.message.attachments : new List<Attachment>();
    }

    public class UpdateMessageObject
    {
        public UpdateMessageData message;
        public UpdateMessageClientInfo client_info;
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
        public List<Attachment> attachments { get; set; }
        public bool is_hidden { get; set; }
        public string payload { get; set; }

    }

    public class UpdateMessageClientInfo
    {
        List<object> button_actions { get; set; }
        public bool keyboard { get; set; }
        public bool inline_keyboard { get; set; }
        public bool carousel { get;set; }
        public int lang_id { get; set; }
    }

    public class UpdateMessageDataAction
    {
        public string type { get; set; }
        public int member_id { get; set; }
    }

    public class Attachment {
        public string type { get; set; }
        public PhotoSaveData photo { get; set; }
    }

    public class UpdateResponse : IUpdatesResponse
    {
        public string ts { get; set; }
        public List<UpdateMessage> updates { get; set; }
        public string new_ts;
        public int failed;

        public IEnumerable<IIncomingMessage> Updates => updates;
    }

    public class ConvertOnlineResponse
    {
        public string id { get; set; }
        public string token { get; set; }
        public string type { get; set; }
        public ConvertOnlineStatus status { get; set; }
        public List<object> errors { get; set; }

        public List<ConvertOnlineOutput> output { get; set; }

    }

    public class ConvertOnlineStatus
    {
        public string code { get; set; }
        public string info { get; set; }
    }

    public class ConvertOnlineOutput
    {
        public string id { get; set; }
        public object source { get; set; }
        public string uri { get; set; }
        public int size { get; set; }
        public string status { get; set; }
        public string content_type { get; set; }
        public int downloads_counter { get; set; }
        public string checksum { get; set; }
        public string created_at { get; set; }
    }


}
