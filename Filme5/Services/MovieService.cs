using MongoDB.Driver;
using Microsoft.Extensions.Options;
using System.Threading.Tasks;
using System.Collections.Generic;
using Filme5.Settings;
using Filme5.Models;

namespace Filme5.Services
{
    public class MovieService
    {
        private readonly IMongoCollection<Movie> _FilmesPopularesCollection;
        private readonly IMongoCollection<Movie> _FilmesEmCartazCollection;
        private readonly IMongoCollection<Movie> _FilmesLancamentosCollection;
        private readonly IMongoCollection<Movie> _FilmesMelhoresAvaliadosCollection;
        private readonly IMongoCollection<Movie> _FilmesTrendingDiaCollection;
        private readonly IMongoCollection<Movie> _FilmesTrendingSemanaCollection;

        public MovieService(IOptions<MongoDbSettings> mongoSettings)
        {
            var client = new MongoClient(mongoSettings.Value.ConnectionString);
            var database = client.GetDatabase(mongoSettings.Value.DatabaseName);

            _FilmesPopularesCollection = database.GetCollection<Movie>(mongoSettings.Value.PopularesCollection);
            _FilmesEmCartazCollection = database.GetCollection<Movie>(mongoSettings.Value.EmCartazCollection);
            _FilmesLancamentosCollection = database.GetCollection<Movie>(mongoSettings.Value.LancamentosCollection);
            _FilmesMelhoresAvaliadosCollection = database.GetCollection<Movie>(mongoSettings.Value.MelhoresAvaliadosCollection);
            _FilmesTrendingDiaCollection = database.GetCollection<Movie>(mongoSettings.Value.TrendingDiaCollection);
            _FilmesTrendingSemanaCollection = database.GetCollection<Movie>(mongoSettings.Value.TrendingSemanaCollection);
        }

        public async Task<List<Movie>> GetMoviesAsync() =>
            await _FilmesPopularesCollection.Find(movie => true).ToListAsync();

        public async Task CreateMovieAsync(Movie movie, string category)
        {
            IMongoCollection<Movie> collection;

            // Define a coleção com base na categoria
            switch (category)
            {
                case "Populares":
                    collection = _FilmesPopularesCollection;
                    break;
                case "EmCartaz":
                    collection = _FilmesEmCartazCollection;
                    break;
                case "Lancamentos":
                    collection = _FilmesLancamentosCollection;
                    break;
                case "MelhoresAvaliados":
                    collection = _FilmesMelhoresAvaliadosCollection;
                    break;
                case "TrendingDia":
                    collection = _FilmesTrendingDiaCollection;
                    break;
                case "TrendingSemana":
                    collection = _FilmesTrendingSemanaCollection;
                    break;
                default:
                    throw new ArgumentException("Categoria inválida");
            }
            await collection.InsertOneAsync(movie);
        }

        // Método para buscar um filme pelo ID do MongoDB em qualquer coleção
        public async Task<Movie> GetAnyMovieByIdAsync(string id)
        {
            var collections = new List<IMongoCollection<Movie>>
            {
                _FilmesPopularesCollection,
                _FilmesEmCartazCollection,
                _FilmesLancamentosCollection,
                _FilmesMelhoresAvaliadosCollection,
                _FilmesTrendingDiaCollection,
                _FilmesTrendingSemanaCollection
            };

            foreach (var collection in collections)
            {
                var movie = await collection.Find(m => m.Id == id).FirstOrDefaultAsync();
                if (movie != null)
                {
                    return movie; // Retorna o primeiro filme encontrado
                }
            }

            return null; // Retorna null se o filme não foi encontrado em nenhuma coleção
        }

        // Método para buscar um filme pelo ID do TMDb em qualquer coleção
        public async Task<Movie> GetAnyMovieByTmdbIdAsync(int idTmdb)
        {
            var collections = new List<IMongoCollection<Movie>>
            {
                  _FilmesPopularesCollection,
                _FilmesEmCartazCollection,
                _FilmesLancamentosCollection,
                _FilmesMelhoresAvaliadosCollection,
                _FilmesTrendingDiaCollection,
                _FilmesTrendingSemanaCollection
            };

            foreach (var collection in collections)
            {
                var movie = await collection.Find(m => m.IdTmdb == idTmdb).FirstOrDefaultAsync();
                if (movie != null)
                {
                    return movie; // Retorna o primeiro filme encontrado
                }
            }

            return null; // Retorna null se o filme não foi encontrado em nenhuma coleção
        }

        public IMongoCollection<Movie> GetPopularesCollection() => _FilmesPopularesCollection;
        public IMongoCollection<Movie> GetEmCartazCollection() => _FilmesEmCartazCollection;
        public IMongoCollection<Movie> GetLancamentosCollection() => _FilmesLancamentosCollection;
        public IMongoCollection<Movie> GetMelhoresAvaliadosCollection() => _FilmesMelhoresAvaliadosCollection;
        public IMongoCollection<Movie> GetTrendingDiaCollection() => _FilmesTrendingDiaCollection;
        public IMongoCollection<Movie> GetTrendingSemanaCollection() => _FilmesTrendingSemanaCollection;

    }
}

