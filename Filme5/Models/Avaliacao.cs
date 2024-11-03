using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Filme5.Models
{
    public class Avaliacao
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; } // ID gerado pelo MongoDB

        [BsonElement("IdTmdb")]
        public int IdTmdb { get; set; } // ID do filme no TMDb

        [BsonElement("IdUsuario")]
        public string IdUsuario { get; set; } = string.Empty; // ID do usuário no MongoDB

        [BsonElement("Comentario")]
        public string Comentario { get; set; } = string.Empty; // Comentário da avaliação

        [BsonElement("Likes")]
        public int Likes { get; set; } // Contagem de likes

        [BsonElement("Dislikes")]
        public int Dislikes { get; set; } // Contagem de dislikes

        [BsonElement("Spoiler")] // Nome do campo no MongoDB
        public bool Spoiler { get; set; } // Atributo booleano
    }
}

