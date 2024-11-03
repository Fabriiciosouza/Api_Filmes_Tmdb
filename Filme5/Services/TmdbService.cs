using Filme5.Models;
using Filme5.Settings; // Adicione esta linha para usar MongoDbSettings
using Microsoft.Extensions.Options; // Adicione para usar IOptions
using MongoDB.Driver;
using Newtonsoft.Json.Linq;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Filme5.Services
{
    public class TmdbService
    {
        private readonly string _apiKey;
        private readonly MovieService _movieService;

        // Coleções do MongoDB
        private readonly IMongoCollection<Movie> _FilmesPopularesCollection;
        private readonly IMongoCollection<Movie> _FilmesEmCartazCollection;
        private readonly IMongoCollection<Movie> _FilmesLancamentosCollection;
        private readonly IMongoCollection<Movie> _FilmesMelhoresAvaliadosCollection;
        private readonly IMongoCollection<Movie> _FilmesTrendingDiaCollection;
        private readonly IMongoCollection<Movie> _FilmesTrendingSemanaCollection;
        public TmdbService(IOptions<MongoDbSettings> mongoDbSettings, IConfiguration configuration, MovieService movieService)
        {
            _apiKey = configuration["TmdbApi:ApiKey"]; // Agora está pegando a API Key da configuração
            _movieService = movieService;

            var settings = mongoDbSettings.Value;

            // Verifica se a string de conexão é nula ou vazia
            if (string.IsNullOrWhiteSpace(settings.ConnectionString))
            {
                throw new ArgumentNullException(nameof(settings.ConnectionString), "A string de conexão do MongoDB não pode ser nula ou vazia.");
            }

            var mongoClient = new MongoClient(settings.ConnectionString);
            var database = mongoClient.GetDatabase(settings.DatabaseName);

            _FilmesPopularesCollection = database.GetCollection<Movie>(settings.PopularesCollection);
            _FilmesEmCartazCollection = database.GetCollection<Movie>(settings.EmCartazCollection);
            _FilmesLancamentosCollection = database.GetCollection<Movie>(settings.LancamentosCollection);
            _FilmesMelhoresAvaliadosCollection = database.GetCollection<Movie>(settings.MelhoresAvaliadosCollection);
            _FilmesTrendingDiaCollection = database.GetCollection<Movie>(settings.TrendingDiaCollection);
            _FilmesTrendingSemanaCollection = database.GetCollection<Movie>(settings.TrendingSemanaCollection);
        }

        public async Task<List<Movie>> GetMoviesFromTmdbAsync(string category)
        {
            string url = category switch
            {
                "Populares" => "https://api.themoviedb.org/3/movie/popular",
                "Lancamentos" => "https://api.themoviedb.org/3/movie/now_playing",
                "EmCartaz" => "https://api.themoviedb.org/3/movie/now_playing",
                "MelhoresAvaliados" => "https://api.themoviedb.org/3/movie/top_rated",
                "TrendingDia" => "https://api.themoviedb.org/3/trending/movie/day",
                "TrendingSemana" => "https://api.themoviedb.org/3/trending/movie/week",
                _ => throw new ArgumentException("Categoria inválida")
            };

            var client = new RestClient(url);
            var request = new RestRequest();
            request.AddParameter("api_key", _apiKey);
            request.AddParameter("language", "pt-BR"); // Configurar o idioma para português
            var response = await client.GetAsync(request);

            var filmes = new List<Movie>();

            if (response.IsSuccessful)
            {
                var json = JObject.Parse(response.Content);
                var results = json["results"].ToList();

                foreach (var result in results)
                {
                    var filme = new Movie
                    {
                        IdTmdb = (int)result["id"],
                        Titulo = result["title"].ToString(),
                        Sinopse = result["overview"].ToString(),
                        DataLancamento = DateTime.Parse(result["release_date"].ToString()),
                        Duracao = result["runtime"]?.ToObject<int>() ?? 0,
                        MediaVotos = result["vote_average"].ToObject<double>(),
                        ContagemVotos = result["vote_count"].ToObject<int>(),
                        CaminhoPoster = result["poster_path"].ToString(),
                        CaminhoBackdrop = result["backdrop_path"]?.ToString() ?? string.Empty,
                        Generos = result["genre_ids"].ToObject<List<int>>().ConvertAll(g => g.ToString())
                    };

                    var trailersResponse = await GetTrailersAsync(filme.IdTmdb);
                    filme.Trailers = trailersResponse;

                    var elencoResponse = await GetElencoAsync(filme.IdTmdb);
                    filme.Elenco = elencoResponse;

                    await GetBudgetAndRevenueAsync(filme);
filme.Providers = await GetWhereToWatchAsync(filme.IdTmdb);
filme.PalavrasChave = await GetKeywordsAsync(filme.IdTmdb);

                    filmes.Add(filme);

                    // Salvar filme na coleção correspondente
                    await SaveMovieToCollectionAsync(filme, category);
                }
            }

            return filmes;
        }
        private async Task GetBudgetAndRevenueAsync(Movie filme)
        {
            var client = new RestClient($"https://api.themoviedb.org/3/movie/{filme.IdTmdb}");
            var request = new RestRequest();
            request.AddParameter("api_key", _apiKey);
            var response = await client.GetAsync(request);

            if (response.IsSuccessful)
            {
                var json = JObject.Parse(response.Content);
                filme.Orcamento = json["budget"]?.ToObject<long>() ?? 0;
                filme.Receita = json["revenue"]?.ToObject<long>() ?? 0;
            }
        }
        private async Task<List<Provider>> GetWhereToWatchAsync(int idTmdb)
        {
            var client = new RestClient($"https://api.themoviedb.org/3/movie/{idTmdb}/watch/providers");
            var request = new RestRequest();
            request.AddParameter("api_key", _apiKey);
            var response = await client.GetAsync(request);

            var providers = new List<Provider>();

            if (response.IsSuccessful)
            {
                var json = JObject.Parse(response.Content);
                var results = json["results"]?["BR"]?["flatrate"] ?? new JArray();

                foreach (var provider in results)
                {
                    providers.Add(new Provider
                    {
                        Nome = provider["provider_name"]?.ToString(),
                        LogoCaminho = provider["logo_path"]?.ToString()
                    });
                }
            }

            return providers;
        }
        private async Task<List<string>> GetKeywordsAsync(int idTmdb)
        {
            var client = new RestClient($"https://api.themoviedb.org/3/movie/{idTmdb}/keywords");
            var request = new RestRequest();
            request.AddParameter("api_key", _apiKey);
            var response = await client.GetAsync(request);

            var keywords = new List<string>();

            if (response.IsSuccessful)
            {
                var json = JObject.Parse(response.Content);
                var results = json["keywords"]?.ToList();

                if (results != null)
                {
                    foreach (var keyword in results)
                    {
                        keywords.Add(keyword["name"].ToString());
                    }
                }
            }

            return keywords;
        }


        private async Task<List<Trailer>> GetTrailersAsync(int idTmdb)
        {
            var client = new RestClient($"https://api.themoviedb.org/3/movie/{idTmdb}/videos");
            var request = new RestRequest();
            request.AddParameter("api_key", _apiKey);
            var response = await client.GetAsync(request);

            var trailers = new List<Trailer>();

            if (response.IsSuccessful)
            {
                var json = JObject.Parse(response.Content);
                var results = json["results"].ToList();

                foreach (var result in results)
                {
                    var trailer = new Trailer
                    {
                        Id = result["id"].ToString(),
                        Nome = result["name"].ToString(),
                        Tipo = result["type"].ToString(),
                        Site = result["site"].ToString(),
                        Chave = result["key"].ToString(),
                        LinkYouTube = result["site"].ToString() == "YouTube" ? $"https://www.youtube.com/watch?v={result["key"]}" : string.Empty
                    };
                    trailers.Add(trailer);
                }
            }

            return trailers;
        }

        private async Task<List<Ator>> GetElencoAsync(int idTmdb)
        {
            var client = new RestClient($"https://api.themoviedb.org/3/movie/{idTmdb}/credits");
            var request = new RestRequest();
            request.AddParameter("api_key", _apiKey);
            var response = await client.GetAsync(request);

            var elenco = new List<Ator>();

            if (response.IsSuccessful)
            {
                var json = JObject.Parse(response.Content);
                var cast = json["cast"].ToList();

                foreach (var member in cast)
                {
                    var ator = new Ator
                    {
                        Id = (int)member["id"],
                        Nome = member["name"].ToString(),
                        Personagem = member["character"].ToString(),
                        FotoCaminho = member["profile_path"]?.ToString() ?? string.Empty
                    };
                    elenco.Add(ator);
                }
            }

            return elenco;
        }

        private async Task SaveMovieToCollectionAsync(Movie filme, string category)
        {
            switch (category)
            {
                case "Populares":
                    await _FilmesPopularesCollection.InsertOneAsync(filme);
                    break;
                case "Lancamentos":
                    await _FilmesLancamentosCollection.InsertOneAsync(filme);
                    break;
                case "EmCartaz":
                    await _FilmesEmCartazCollection.InsertOneAsync(filme);
                    break;
                case "MelhoresAvaliados":
                    await _FilmesMelhoresAvaliadosCollection.InsertOneAsync(filme);
                    break;
                case "TrendingDia":
                    await _FilmesTrendingDiaCollection.InsertOneAsync(filme);
                    break;
                case "TrendingSemana":
                    await _FilmesTrendingSemanaCollection.InsertOneAsync(filme);
                    break;
                default:
                    throw new ArgumentException("Categoria inválida");
            }
        }
    }
}
