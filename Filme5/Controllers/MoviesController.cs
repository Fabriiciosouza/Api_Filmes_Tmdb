using Microsoft.AspNetCore.Mvc;
using Filme5.Services; 
using Filme5.Models; 
using System.Collections.Generic;
using System.Threading.Tasks;
using MongoDB.Driver;
using Filme5.Settings;
using Microsoft.Extensions.Options;
using MongoDB.Bson;

namespace Filme5.Controllers 
{
    [ApiController]
    [Route("api/[controller]")]
    public class MoviesController : ControllerBase
    {
        private readonly MovieService _movieService;
        private readonly TmdbService _tmdbService;

        public MoviesController(MovieService movieService, TmdbService tmdbService)
        {
            _movieService = movieService;
            _tmdbService = tmdbService;
        }

        // Método para obter todos os filmes salvos
        [HttpGet]
        public async Task<ActionResult<List<Movie>>> GetMovies()
        {
            var movies = await _movieService.GetMoviesAsync();
            if (movies == null || movies.Count == 0)
            {
                return NotFound("Nenhum filme encontrado.");
            }
            return Ok(movies);
        }


        // Método para buscar e salvar filmes da TMDb
        [HttpPost("fetch-from-tmdb")]
        public async Task<IActionResult> FetchAndSaveMovies([FromQuery] string category)
        {
            if (string.IsNullOrEmpty(category))
            {
                return BadRequest("A categoria não pode ser vazia.");
            }

            var movies = await _tmdbService.GetMoviesFromTmdbAsync(category);
            if (movies == null || movies.Count == 0)
            {
                return NotFound("Nenhum filme encontrado na TMDb para a categoria especificada.");
            }

            // Salva cada filme na categoria correspondente
            foreach (var movie in movies)
            {
                await _movieService.CreateMovieAsync(movie, category); // Passando a categoria
            }
            return Ok("Filmes salvos com sucesso.");
        }
        // Método para obter filmes por coleção
        [HttpGet("{collectionName}")]
        public async Task<IActionResult> GetMoviesByCollection(string collectionName)
        {
            IMongoCollection<Movie> collection;

            switch (collectionName)
            {
                case "Populares":
                    collection = _movieService.GetPopularesCollection();
                    break;
                case "EmCartaz":
                    collection = _movieService.GetEmCartazCollection(); 
                    break;
                case "Lancamentos":
                    collection = _movieService.GetLancamentosCollection(); 
                    break;
                case "MelhoresAvaliados":
                    collection = _movieService.GetMelhoresAvaliadosCollection(); 
                    break;
                case "TrendingDia":
                    collection = _movieService.GetTrendingDiaCollection();
                    break;
                case "TrendingSemana":
                    collection = _movieService.GetTrendingSemanaCollection(); 
                    break;
                default:
                    return BadRequest("Categoria inválida");
            }

            var movies = await collection.Find(_ => true).ToListAsync(); // Retorna todos os filmes da coleção específica
            return Ok(movies);
        }

        // Método para obter filme pelo ID do MongoDB em qualquer coleção
        [HttpGet("id/{id:length(24)}")]
        public async Task<IActionResult> GetAnyMovieById(string id)
        {
            var movie = await _movieService.GetAnyMovieByIdAsync(id);
            if (movie == null)
            {
                return NotFound("Filme não encontrado.");
            }
            return Ok(movie);
        }

        // Método para obter filme pelo ID do TMDb em qualquer coleção
        [HttpGet("tmdb/{idTmdb:int}")]
        public async Task<IActionResult> GetAnyMovieByTmdbId(int idTmdb)
        {
            var movie = await _movieService.GetAnyMovieByTmdbIdAsync(idTmdb);
            if (movie == null)
            {
                return NotFound("Filme não encontrado.");
            }
            return Ok(movie);
        }


    }


}
 