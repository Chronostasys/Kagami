namespace PixivCS.Objects
{
    using System;
    using System.Collections.Generic;

    using System.Globalization;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Converters;

    public partial class Tops
    {
        [JsonProperty("error")]
        public bool Error { get; set; }

        [JsonProperty("message")]
        public string Message { get; set; }

        [JsonProperty("body", NullValueHandling = NullValueHandling.Ignore)]
        public Body Body { get; set; }
    }

    public partial class Body
    {
        [JsonProperty("tagTranslation")]
        public Dictionary<string, TagTranslation> TagTranslation { get; set; }

        [JsonProperty("thumbnails")]
        public Thumbnails Thumbnails { get; set; }

        [JsonProperty("illustSeries")]
        public List<object> IllustSeries { get; set; }

        [JsonProperty("requests")]
        public List<Request> Requests { get; set; }

        [JsonProperty("users")]
        public List<User> Users { get; set; }

        [JsonProperty("page")]
        public Page Page { get; set; }

        [JsonProperty("boothItems")]
        public List<BoothItem> BoothItems { get; set; }

        [JsonProperty("sketchLives")]
        public List<SketchLive> SketchLives { get; set; }

        [JsonProperty("zoneConfig")]
        public ZoneConfig ZoneConfig { get; set; }
    }

    public partial class BoothItem
    {
        [JsonProperty("id")]
        [JsonConverter(typeof(ParseStringConverter))]
        public long Id { get; set; }

        [JsonProperty("userId")]
        [JsonConverter(typeof(ParseStringConverter))]
        public long UserId { get; set; }

        [JsonProperty("title")]
        public string Title { get; set; }

        [JsonProperty("url")]
        public Uri Url { get; set; }

        [JsonProperty("imageUrl")]
        public Uri ImageUrl { get; set; }

        [JsonProperty("adult")]
        public bool Adult { get; set; }
    }

    public partial class Page
    {
        [JsonProperty("tags")]
        public List<Tag> Tags { get; set; }

        [JsonProperty("follow")]
        public List<object> Follow { get; set; }

        [JsonProperty("mypixiv")]
        public List<object> Mypixiv { get; set; }

        [JsonProperty("recommend")]
        public Recommend Recommend { get; set; }

        [JsonProperty("recommendByTag")]
        public List<RecommendByTag> RecommendByTag { get; set; }

        [JsonProperty("ranking")]
        public Ranking Ranking { get; set; }

        [JsonProperty("pixivision")]
        public List<Pixivision> Pixivision { get; set; }

        [JsonProperty("recommendUser")]
        public List<RecommendUser> RecommendUser { get; set; }

        [JsonProperty("contestOngoing")]
        public List<ContestOngoing> ContestOngoing { get; set; }

        [JsonProperty("contestResult")]
        public List<ContestResult> ContestResult { get; set; }

        [JsonProperty("editorRecommend")]
        public List<EditorRecommend> EditorRecommend { get; set; }

        [JsonProperty("boothFollowItemIds")]
        [JsonConverter(typeof(DecodeArrayConverter))]
        public List<long> BoothFollowItemIds { get; set; }

        [JsonProperty("sketchLiveFollowIds")]
        public List<object> SketchLiveFollowIds { get; set; }

        [JsonProperty("sketchLivePopularIds")]
        public List<string> SketchLivePopularIds { get; set; }

        [JsonProperty("myFavoriteTags")]
        public List<object> MyFavoriteTags { get; set; }

        [JsonProperty("newPost")]
        [JsonConverter(typeof(DecodeArrayConverter))]
        public List<long> NewPost { get; set; }

        [JsonProperty("trendingTags")]
        public List<TrendingTag> TrendingTags { get; set; }

        [JsonProperty("completeRequestIds")]
        [JsonConverter(typeof(DecodeArrayConverter))]
        public List<long> CompleteRequestIds { get; set; }

        [JsonProperty("userEventIds")]
        [JsonConverter(typeof(DecodeArrayConverter))]
        public List<long> UserEventIds { get; set; }
    }

    public partial class ContestOngoing
    {
        [JsonProperty("slug")]
        public string Slug { get; set; }

        [JsonProperty("type")]
        public RequestPostWorkTypeEnum Type { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("url")]
        public Uri Url { get; set; }

        [JsonProperty("iconUrl")]
        public Uri IconUrl { get; set; }

        [JsonProperty("workIds")]
        public List<long> WorkIds { get; set; }

        [JsonProperty("isNew")]
        public bool IsNew { get; set; }
    }

    public partial class ContestResult
    {
        [JsonProperty("slug")]
        public string Slug { get; set; }

        [JsonProperty("type")]
        public RequestPostWorkTypeEnum Type { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("url")]
        public Uri Url { get; set; }

        [JsonProperty("iconUrl")]
        public Uri IconUrl { get; set; }

        [JsonProperty("winnerWorkIds")]
        public List<long> WinnerWorkIds { get; set; }
    }

    public partial class EditorRecommend
    {
        [JsonProperty("illustId")]
        [JsonConverter(typeof(ParseStringConverter))]
        public long IllustId { get; set; }

        [JsonProperty("comment")]
        public string Comment { get; set; }
    }

    public partial class Pixivision
    {
        [JsonProperty("id")]
        [JsonConverter(typeof(ParseStringConverter))]
        public long Id { get; set; }

        [JsonProperty("title")]
        public string Title { get; set; }

        [JsonProperty("thumbnailUrl")]
        public Uri ThumbnailUrl { get; set; }

        [JsonProperty("url")]
        public Uri Url { get; set; }
    }

    public partial class Ranking
    {
        [JsonProperty("items")]
        public List<Item> Items { get; set; }

        [JsonProperty("date")]
        [JsonConverter(typeof(ParseStringConverter))]
        public long Date { get; set; }
    }

    public partial class Item
    {
        [JsonProperty("rank")]
        [JsonConverter(typeof(ParseStringConverter))]
        public long Rank { get; set; }

        [JsonProperty("id")]
        [JsonConverter(typeof(ParseStringConverter))]
        public long Id { get; set; }
    }

    public partial class Recommend
    {
        [JsonProperty("ids")]
        [JsonConverter(typeof(DecodeArrayConverter))]
        public List<long> Ids { get; set; }

        [JsonProperty("details")]
        public Dictionary<string, Detail> Details { get; set; }
    }

    public partial class Detail
    {
        [JsonProperty("methods")]
        public List<Method> Methods { get; set; }

        [JsonProperty("score")]
        public long Score { get; set; }

        [JsonProperty("seedIllustIds")]
        public List<object> SeedIllustIds { get; set; }
    }

    public partial class RecommendByTag
    {
        [JsonProperty("tag")]
        public string Tag { get; set; }

        [JsonProperty("ids")]
        [JsonConverter(typeof(DecodeArrayConverter))]
        public List<long> Ids { get; set; }

        [JsonProperty("details")]
        public Dictionary<string, Detail> Details { get; set; }
    }

    public partial class RecommendUser
    {
        [JsonProperty("id")]
        public long Id { get; set; }

        [JsonProperty("illustIds")]
        [JsonConverter(typeof(DecodeArrayConverter))]
        public List<long> IllustIds { get; set; }

        [JsonProperty("novelIds")]
        public List<object> NovelIds { get; set; }
    }

    public partial class Tag
    {
        [JsonProperty("tag")]
        public string TagTag { get; set; }

        [JsonProperty("ids")]
        public List<long> Ids { get; set; }
    }

    public partial class TrendingTag
    {
        [JsonProperty("tag")]
        public string Tag { get; set; }

        [JsonProperty("trendingRate")]
        public long TrendingRate { get; set; }

        [JsonProperty("ids")]
        public List<long> Ids { get; set; }
    }

    public partial class Request
    {
        [JsonProperty("requestId")]
        [JsonConverter(typeof(ParseStringConverter))]
        public long RequestId { get; set; }

        [JsonProperty("planId")]
        [JsonConverter(typeof(ParseStringConverter))]
        public long PlanId { get; set; }

        [JsonProperty("fanUserId")]
        [JsonConverter(typeof(ParseStringConverter))]
        public long? FanUserId { get; set; }

        [JsonProperty("creatorUserId")]
        [JsonConverter(typeof(ParseStringConverter))]
        public long CreatorUserId { get; set; }

        [JsonProperty("requestStatus")]
        public RequestStatus RequestStatus { get; set; }

        [JsonProperty("requestPostWorkType")]
        public RequestPostWorkTypeEnum RequestPostWorkType { get; set; }

        [JsonProperty("requestPrice")]
        public long RequestPrice { get; set; }

        [JsonProperty("requestProposal")]
        public RequestProposal RequestProposal { get; set; }

        [JsonProperty("requestTags")]
        public List<string> RequestTags { get; set; }

        [JsonProperty("requestAdultFlg")]
        public bool RequestAdultFlg { get; set; }

        [JsonProperty("requestAnonymousFlg")]
        public bool RequestAnonymousFlg { get; set; }

        [JsonProperty("requestRestrictFlg")]
        public bool RequestRestrictFlg { get; set; }

        [JsonProperty("requestAcceptCollaborateFlg")]
        public bool RequestAcceptCollaborateFlg { get; set; }

        [JsonProperty("requestResponseDeadlineDatetime")]
        public DateTimeOffset RequestResponseDeadlineDatetime { get; set; }

        [JsonProperty("requestPostDeadlineDatetime")]
        public DateTimeOffset RequestPostDeadlineDatetime { get; set; }

        [JsonProperty("role")]
        public Role Role { get; set; }

        [JsonProperty("collaborateStatus")]
        public CollaborateStatus CollaborateStatus { get; set; }

        [JsonProperty("postWork")]
        public PostWork PostWork { get; set; }
    }

    public partial class CollaborateStatus
    {
        [JsonProperty("collaborating")]
        public bool Collaborating { get; set; }

        [JsonProperty("collaborateAnonymousFlg")]
        public bool CollaborateAnonymousFlg { get; set; }

        [JsonProperty("collaboratedCnt")]
        public long CollaboratedCnt { get; set; }

        [JsonProperty("collaborateUserSamples")]
        public List<object> CollaborateUserSamples { get; set; }
    }

    public partial class PostWork
    {
        [JsonProperty("postWorkId")]
        [JsonConverter(typeof(ParseStringConverter))]
        public long PostWorkId { get; set; }

        [JsonProperty("postWorkType")]
        public RequestPostWorkTypeEnum PostWorkType { get; set; }

        [JsonProperty("work")]
        public Work Work { get; set; }
    }

    public partial class Work
    {
        [JsonProperty("isUnlisted")]
        public bool IsUnlisted { get; set; }

        [JsonProperty("secret")]
        public object Secret { get; set; }
    }

    public partial class RequestProposal
    {
        [JsonProperty("requestOriginalProposal")]
        public string RequestOriginalProposal { get; set; }

        [JsonProperty("requestOriginalProposalHtml")]
        public string RequestOriginalProposalHtml { get; set; }

        [JsonProperty("requestOriginalProposalLang")]
        public RequestProposalLang RequestOriginalProposalLang { get; set; }

        [JsonProperty("requestTranslationProposal")]
        public List<RequestTranslationProposal> RequestTranslationProposal { get; set; }
    }

    public partial class RequestTranslationProposal
    {
        [JsonProperty("requestProposal")]
        public string RequestProposal { get; set; }

        [JsonProperty("requestProposalHtml")]
        public string RequestProposalHtml { get; set; }

        [JsonProperty("requestProposalLang")]
        public RequestProposalLang RequestProposalLang { get; set; }
    }

    public partial class SketchLive
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("url")]
        public Uri Url { get; set; }

        [JsonProperty("thumbnailUrl")]
        public Uri ThumbnailUrl { get; set; }

        [JsonProperty("audienceCount")]
        public long AudienceCount { get; set; }

        [JsonProperty("isR18")]
        public bool IsR18 { get; set; }

        [JsonProperty("streamerIds")]
        public List<long> StreamerIds { get; set; }
    }

    public partial class TagTranslation
    {
        [JsonProperty("en", NullValueHandling = NullValueHandling.Ignore)]
        public string En { get; set; }

        [JsonProperty("ko", NullValueHandling = NullValueHandling.Ignore)]
        public string Ko { get; set; }

        [JsonProperty("zh", NullValueHandling = NullValueHandling.Ignore)]
        public string Zh { get; set; }

        [JsonProperty("zh_tw", NullValueHandling = NullValueHandling.Ignore)]
        public string ZhTw { get; set; }

        [JsonProperty("romaji")]
        public string Romaji { get; set; }
    }

    public partial class Thumbnails
    {
        [JsonProperty("illust")]
        public List<Illust> Illust { get; set; }

        [JsonProperty("novel")]
        public List<Novel> Novel { get; set; }

        [JsonProperty("novelSeries")]
        public List<object> NovelSeries { get; set; }

        [JsonProperty("novelDraft")]
        public List<object> NovelDraft { get; set; }
    }

    public partial class Illust
    {
        [JsonProperty("id")]
        [JsonConverter(typeof(ParseStringConverter))]
        public long Id { get; set; }

        [JsonProperty("title")]
        public string Title { get; set; }

        [JsonProperty("illustType")]
        public long IllustType { get; set; }

        [JsonProperty("xRestrict")]
        public long XRestrict { get; set; }

        [JsonProperty("restrict")]
        public long Restrict { get; set; }

        [JsonProperty("sl")]
        public long Sl { get; set; }

        [JsonProperty("url")]
        public Uri Url { get; set; }

        [JsonProperty("description")]
        public string Description { get; set; }

        [JsonProperty("tags")]
        public List<string> Tags { get; set; }

        [JsonProperty("userId")]
        [JsonConverter(typeof(ParseStringConverter))]
        public long UserId { get; set; }

        [JsonProperty("userName")]
        public string UserName { get; set; }

        [JsonProperty("width")]
        public long Width { get; set; }

        [JsonProperty("height")]
        public long Height { get; set; }

        [JsonProperty("pageCount")]
        public long PageCount { get; set; }

        [JsonProperty("isBookmarkable")]
        public bool IsBookmarkable { get; set; }

        [JsonProperty("bookmarkData")]
        public object BookmarkData { get; set; }

        [JsonProperty("alt")]
        public string Alt { get; set; }

        [JsonProperty("titleCaptionTranslation")]
        public TitleCaptionTranslation TitleCaptionTranslation { get; set; }

        [JsonProperty("createDate")]
        public DateTimeOffset CreateDate { get; set; }

        [JsonProperty("updateDate")]
        public DateTimeOffset UpdateDate { get; set; }

        [JsonProperty("isUnlisted")]
        public bool IsUnlisted { get; set; }

        [JsonProperty("isMasked")]
        public bool IsMasked { get; set; }

        [JsonProperty("urls")]
        public Urls Urls { get; set; }

        [JsonProperty("profileImageUrl")]
        public Uri ProfileImageUrl { get; set; }

        [JsonProperty("seriesId", NullValueHandling = NullValueHandling.Ignore)]
        [JsonConverter(typeof(ParseStringConverter))]
        public long? SeriesId { get; set; }

        [JsonProperty("seriesTitle", NullValueHandling = NullValueHandling.Ignore)]
        public string SeriesTitle { get; set; }

        [JsonProperty("imageResponseCount", NullValueHandling = NullValueHandling.Ignore)]
        public long? ImageResponseCount { get; set; }

        [JsonProperty("bookmarkCount", NullValueHandling = NullValueHandling.Ignore)]
        public long? BookmarkCount { get; set; }
    }

    public partial class TitleCaptionTranslation
    {
        [JsonProperty("workTitle")]
        public object WorkTitle { get; set; }

        [JsonProperty("workCaption")]
        public object WorkCaption { get; set; }
    }

    public partial class Urls
    {
        [JsonProperty("250x250")]
        public Uri The250X250 { get; set; }

        [JsonProperty("360x360")]
        public Uri The360X360 { get; set; }

        [JsonProperty("540x540")]
        public Uri The540X540 { get; set; }
    }

    public partial class Novel
    {
        [JsonProperty("id")]
        [JsonConverter(typeof(ParseStringConverter))]
        public long Id { get; set; }

        [JsonProperty("title")]
        public string Title { get; set; }

        [JsonProperty("xRestrict")]
        public long XRestrict { get; set; }

        [JsonProperty("restrict")]
        public long Restrict { get; set; }

        [JsonProperty("url")]
        public Uri Url { get; set; }

        [JsonProperty("tags")]
        public List<string> Tags { get; set; }

        [JsonProperty("userId")]
        [JsonConverter(typeof(ParseStringConverter))]
        public long UserId { get; set; }

        [JsonProperty("userName")]
        public string UserName { get; set; }

        [JsonProperty("profileImageUrl")]
        public Uri ProfileImageUrl { get; set; }

        [JsonProperty("textCount")]
        public long TextCount { get; set; }

        [JsonProperty("wordCount")]
        public long WordCount { get; set; }

        [JsonProperty("readingTime")]
        public long ReadingTime { get; set; }

        [JsonProperty("useWordCount")]
        public bool UseWordCount { get; set; }

        [JsonProperty("description")]
        public string Description { get; set; }

        [JsonProperty("isBookmarkable")]
        public bool IsBookmarkable { get; set; }

        [JsonProperty("bookmarkData")]
        public object BookmarkData { get; set; }

        [JsonProperty("bookmarkCount")]
        public long BookmarkCount { get; set; }

        [JsonProperty("isOriginal")]
        public bool IsOriginal { get; set; }

        [JsonProperty("marker")]
        public object Marker { get; set; }

        [JsonProperty("titleCaptionTranslation")]
        public TitleCaptionTranslation TitleCaptionTranslation { get; set; }

        [JsonProperty("createDate")]
        public DateTimeOffset CreateDate { get; set; }

        [JsonProperty("updateDate")]
        public DateTimeOffset UpdateDate { get; set; }

        [JsonProperty("isMasked")]
        public bool IsMasked { get; set; }

        [JsonProperty("isUnlisted")]
        public bool IsUnlisted { get; set; }

        [JsonProperty("seriesId", NullValueHandling = NullValueHandling.Ignore)]
        [JsonConverter(typeof(ParseStringConverter))]
        public long? SeriesId { get; set; }

        [JsonProperty("seriesTitle", NullValueHandling = NullValueHandling.Ignore)]
        public string SeriesTitle { get; set; }
    }

    public partial class User
    {
        [JsonProperty("partial")]
        public long Partial { get; set; }

        [JsonProperty("comment")]
        public string Comment { get; set; }

        [JsonProperty("followedBack")]
        public bool FollowedBack { get; set; }

        [JsonProperty("userId")]
        [JsonConverter(typeof(ParseStringConverter))]
        public long UserId { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("image")]
        public Uri Image { get; set; }

        [JsonProperty("imageBig")]
        public Uri ImageBig { get; set; }

        [JsonProperty("premium")]
        public bool Premium { get; set; }

        [JsonProperty("isFollowed")]
        public bool IsFollowed { get; set; }

        [JsonProperty("isMypixiv")]
        public bool IsMypixiv { get; set; }

        [JsonProperty("isBlocking")]
        public bool IsBlocking { get; set; }

        [JsonProperty("background")]
        public object Background { get; set; }

        [JsonProperty("acceptRequest")]
        public bool AcceptRequest { get; set; }
    }

    public partial class ZoneConfig
    {
        [JsonProperty("logo")]
        public Footer Logo { get; set; }

        [JsonProperty("header")]
        public Footer Header { get; set; }

        [JsonProperty("footer")]
        public Footer Footer { get; set; }

        [JsonProperty("topbranding_rectangle")]
        public Footer TopbrandingRectangle { get; set; }

        [JsonProperty("illusttop_appeal")]
        public Footer IllusttopAppeal { get; set; }
    }

    public partial class Footer
    {
        [JsonProperty("url")]
        public Uri Url { get; set; }
    }

    public enum RequestPostWorkTypeEnum { Illust, Novel };

    public enum Method { ByTag, R18Ranking };

    public enum RequestProposalLang { En, Ja };

    public enum RequestStatus { Complete };

    public enum Role { Others };

    internal static class Converter
    {
        public static readonly JsonSerializerSettings Settings = new JsonSerializerSettings
        {
            MetadataPropertyHandling = MetadataPropertyHandling.Ignore,
            DateParseHandling = DateParseHandling.None,
            Converters =
            {
                RequestPostWorkTypeEnumConverter.Singleton,
                MethodConverter.Singleton,
                RequestProposalLangConverter.Singleton,
                RequestStatusConverter.Singleton,
                RoleConverter.Singleton,
                new IsoDateTimeConverter { DateTimeStyles = DateTimeStyles.AssumeUniversal }
            },
        };
    }

    internal class ParseStringConverter : JsonConverter
    {
        public override bool CanConvert(Type t) => t == typeof(long) || t == typeof(long?);

        public override object ReadJson(JsonReader reader, Type t, object existingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.Null) return null;
            var value = serializer.Deserialize<string>(reader);
            long l;
            if (Int64.TryParse(value, out l))
            {
                return l;
            }
            throw new Exception("Cannot unmarshal type long");
        }

        public override void WriteJson(JsonWriter writer, object untypedValue, JsonSerializer serializer)
        {
            if (untypedValue == null)
            {
                serializer.Serialize(writer, null);
                return;
            }
            var value = (long)untypedValue;
            serializer.Serialize(writer, value.ToString());
            return;
        }

        public static readonly ParseStringConverter Singleton = new ParseStringConverter();
    }

    internal class DecodeArrayConverter : JsonConverter
    {
        public override bool CanConvert(Type t) => t == typeof(List<long>);

        public override object ReadJson(JsonReader reader, Type t, object existingValue, JsonSerializer serializer)
        {
            reader.Read();
            var value = new List<long>();
            while (reader.TokenType != JsonToken.EndArray)
            {
                var converter = ParseStringConverter.Singleton;
                var arrayItem = (long)converter.ReadJson(reader, typeof(long), null, serializer);
                value.Add(arrayItem);
                reader.Read();
            }
            return value;
        }

        public override void WriteJson(JsonWriter writer, object untypedValue, JsonSerializer serializer)
        {
            var value = (List<long>)untypedValue;
            writer.WriteStartArray();
            foreach (var arrayItem in value)
            {
                var converter = ParseStringConverter.Singleton;
                converter.WriteJson(writer, arrayItem, serializer);
            }
            writer.WriteEndArray();
            return;
        }

        public static readonly DecodeArrayConverter Singleton = new DecodeArrayConverter();
    }

    internal class RequestPostWorkTypeEnumConverter : JsonConverter
    {
        public override bool CanConvert(Type t) => t == typeof(RequestPostWorkTypeEnum) || t == typeof(RequestPostWorkTypeEnum?);

        public override object ReadJson(JsonReader reader, Type t, object existingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.Null) return null;
            var value = serializer.Deserialize<string>(reader);
            switch (value)
            {
                case "illust":
                    return RequestPostWorkTypeEnum.Illust;
                case "novel":
                    return RequestPostWorkTypeEnum.Novel;
            }
            throw new Exception("Cannot unmarshal type RequestPostWorkTypeEnum");
        }

        public override void WriteJson(JsonWriter writer, object untypedValue, JsonSerializer serializer)
        {
            if (untypedValue == null)
            {
                serializer.Serialize(writer, null);
                return;
            }
            var value = (RequestPostWorkTypeEnum)untypedValue;
            switch (value)
            {
                case RequestPostWorkTypeEnum.Illust:
                    serializer.Serialize(writer, "illust");
                    return;
                case RequestPostWorkTypeEnum.Novel:
                    serializer.Serialize(writer, "novel");
                    return;
            }
            throw new Exception("Cannot marshal type RequestPostWorkTypeEnum");
        }

        public static readonly RequestPostWorkTypeEnumConverter Singleton = new RequestPostWorkTypeEnumConverter();
    }

    internal class MethodConverter : JsonConverter
    {
        public override bool CanConvert(Type t) => t == typeof(Method) || t == typeof(Method?);

        public override object ReadJson(JsonReader reader, Type t, object existingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.Null) return null;
            var value = serializer.Deserialize<string>(reader);
            switch (value)
            {
                case "by_tag":
                    return Method.ByTag;
                case "r18_ranking":
                    return Method.R18Ranking;
            }
            throw new Exception("Cannot unmarshal type Method");
        }

        public override void WriteJson(JsonWriter writer, object untypedValue, JsonSerializer serializer)
        {
            if (untypedValue == null)
            {
                serializer.Serialize(writer, null);
                return;
            }
            var value = (Method)untypedValue;
            switch (value)
            {
                case Method.ByTag:
                    serializer.Serialize(writer, "by_tag");
                    return;
                case Method.R18Ranking:
                    serializer.Serialize(writer, "r18_ranking");
                    return;
            }
            throw new Exception("Cannot marshal type Method");
        }

        public static readonly MethodConverter Singleton = new MethodConverter();
    }

    internal class RequestProposalLangConverter : JsonConverter
    {
        public override bool CanConvert(Type t) => t == typeof(RequestProposalLang) || t == typeof(RequestProposalLang?);

        public override object ReadJson(JsonReader reader, Type t, object existingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.Null) return null;
            var value = serializer.Deserialize<string>(reader);
            switch (value)
            {
                case "en":
                    return RequestProposalLang.En;
                case "ja":
                    return RequestProposalLang.Ja;
            }
            throw new Exception("Cannot unmarshal type RequestProposalLang");
        }

        public override void WriteJson(JsonWriter writer, object untypedValue, JsonSerializer serializer)
        {
            if (untypedValue == null)
            {
                serializer.Serialize(writer, null);
                return;
            }
            var value = (RequestProposalLang)untypedValue;
            switch (value)
            {
                case RequestProposalLang.En:
                    serializer.Serialize(writer, "en");
                    return;
                case RequestProposalLang.Ja:
                    serializer.Serialize(writer, "ja");
                    return;
            }
            throw new Exception("Cannot marshal type RequestProposalLang");
        }

        public static readonly RequestProposalLangConverter Singleton = new RequestProposalLangConverter();
    }

    internal class RequestStatusConverter : JsonConverter
    {
        public override bool CanConvert(Type t) => t == typeof(RequestStatus) || t == typeof(RequestStatus?);

        public override object ReadJson(JsonReader reader, Type t, object existingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.Null) return null;
            var value = serializer.Deserialize<string>(reader);
            if (value == "complete")
            {
                return RequestStatus.Complete;
            }
            throw new Exception("Cannot unmarshal type RequestStatus");
        }

        public override void WriteJson(JsonWriter writer, object untypedValue, JsonSerializer serializer)
        {
            if (untypedValue == null)
            {
                serializer.Serialize(writer, null);
                return;
            }
            var value = (RequestStatus)untypedValue;
            if (value == RequestStatus.Complete)
            {
                serializer.Serialize(writer, "complete");
                return;
            }
            throw new Exception("Cannot marshal type RequestStatus");
        }

        public static readonly RequestStatusConverter Singleton = new RequestStatusConverter();
    }

    internal class RoleConverter : JsonConverter
    {
        public override bool CanConvert(Type t) => t == typeof(Role) || t == typeof(Role?);

        public override object ReadJson(JsonReader reader, Type t, object existingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.Null) return null;
            var value = serializer.Deserialize<string>(reader);
            if (value == "others")
            {
                return Role.Others;
            }
            throw new Exception("Cannot unmarshal type Role");
        }

        public override void WriteJson(JsonWriter writer, object untypedValue, JsonSerializer serializer)
        {
            if (untypedValue == null)
            {
                serializer.Serialize(writer, null);
                return;
            }
            var value = (Role)untypedValue;
            if (value == Role.Others)
            {
                serializer.Serialize(writer, "others");
                return;
            }
            throw new Exception("Cannot marshal type Role");
        }

        public static readonly RoleConverter Singleton = new RoleConverter();
    }
}
