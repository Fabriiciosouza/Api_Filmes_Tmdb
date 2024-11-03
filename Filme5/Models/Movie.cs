using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;

namespace Filme5.Models
{
    public class Movie
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; } // ID gerado pelo MongoDB

        [BsonElement("IdTmdb")]
        public int IdTmdb { get; set; } // ID do TMDb

        [BsonElement("Titulo")]
        public string Titulo { get; set; } = string.Empty; // Título do filme

        [BsonElement("Sinopse")]
        public string Sinopse { get; set; } = string.Empty; // Sinopse

        [BsonElement("DataLancamento")]
        public DateTime DataLancamento { get; set; } // Data de lançamento

        [BsonElement("Duracao")]
        public int Duracao { get; set; } // Duração em minutos

        [BsonElement("MediaVotos")]
        public double MediaVotos { get; set; } // Média dos votos

        [BsonElement("ContagemVotos")]
        public int ContagemVotos { get; set; } // Contagem de votos

        [BsonElement("CaminhoPoster")]
        public string CaminhoPoster { get; set; } = string.Empty; // Caminho para o poster

        [BsonElement("CaminhoBackdrop")]
        public string CaminhoBackdrop { get; set; } = string.Empty; // Caminho para a imagem de fundo

        [BsonElement("Generos")]
        public List<string> Generos { get; set; } = new List<string>(); // Gêneros

        [BsonElement("Trailers")]
        public List<Trailer> Trailers { get; set; } = new List<Trailer>(); // Lista de trailers

        [BsonElement("Elenco")]
        public List<Ator> Elenco { get; set; } = new List<Ator>(); // Lista de atores

        [BsonElement("ComissaoTecnica")]
        public List<CrewMember> ComissaoTecnica { get; set; } = new List<CrewMember>(); // Lista de equipe técnica

        [BsonElement("Orcamento")]
        public long Orcamento { get; set; } // Orçamento do filme

        [BsonElement("Receita")]
        public long Receita { get; set; } // Receita do filme

        [BsonElement("Providers")]
        public List<Provider> Providers { get; set; } = new List<Provider>(); // Plataformas de onde assistir

        [BsonElement("PalavrasChave")]
        public List<string> PalavrasChave { get; set; } = new List<string>(); // Palavras-chave do filme
    }

    public class Trailer
    {
        [BsonElement("Id")]
        public string Id { get; set; } = string.Empty; // ID do trailer

        [BsonElement("Nome")]
        public string Nome { get; set; } = string.Empty; // Nome do trailer

        [BsonElement("Tipo")]
        public string Tipo { get; set; } = string.Empty; // Tipo do vídeo (ex: Trailer, Teaser)

        [BsonElement("Site")]
        public string Site { get; set; } = string.Empty; // Site de origem (ex: YouTube)

        [BsonElement("Chave")]
        public string Chave { get; set; } = string.Empty; // Chave para acessar o vídeo no site (ex: ID do YouTube)

        [BsonElement("LinkYouTube")]
        public string LinkYouTube { get; internal set; } = string.Empty;
    }

    public class Ator
    {
        [BsonElement("Id")]
        public int Id { get; set; } // ID do ator no TMDb

        [BsonElement("Nome")]
        public string Nome { get; set; } = string.Empty; // Nome do ator

        [BsonElement("Personagem")]
        public string Personagem { get; set; } = string.Empty; // Nome do personagem interpretado

        [BsonElement("FotoCaminho")]
        public string FotoCaminho { get; set; } = string.Empty; // Caminho para a foto do ator
    }

    public class CrewMember
    {
        [BsonElement("Id")]
        public int Id { get; set; } // ID do membro da equipe no TMDb

        [BsonElement("Nome")]
        public string Nome { get; set; } = string.Empty; // Nome do membro da equipe

        [BsonElement("Cargo")]
        public string Cargo { get; set; } = string.Empty; // Função na equipe (ex: Diretor, Roteirista)

        [BsonElement("Departamento")]
        public string Departamento { get; set; } = string.Empty; // Departamento (ex: Direção, Roteiro)
    }
    public class Provider
    {
        [BsonElement("Nome")]
        public string Nome { get; set; } = string.Empty; // Nome do provedor (ex: Netflix, Amazon Prime)

        [BsonElement("LogoCaminho")]
        public string LogoCaminho { get; set; } = string.Empty; // Caminho do logo do provedor
    }
}

