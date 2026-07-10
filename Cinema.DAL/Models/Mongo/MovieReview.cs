using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;

namespace Cinema.DAL.Models.Mongo
{
    public class MovieReview
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }

        [BsonElement("maPhim")]
        public int MaPhim { get; set; }

        [BsonElement("userAccount")]
        public string UserAccount { get; set; } = null!;

        [BsonElement("rating")]
        public int Rating { get; set; } // 1 to 5 stars

        [BsonElement("comment")]
        public string Comment { get; set; } = string.Empty;

        [BsonElement("createdAt")]
        [BsonDateTimeOptions(Kind = DateTimeKind.Local)]
        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
}
