using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using Filme5.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Filme5.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AvaliacoesController : ControllerBase
    {
        private readonly IMongoCollection<Avaliacao> _avaliacoesCollection;

        public AvaliacoesController(IMongoCollection<Avaliacao> avaliacoesCollection)
        {
            _avaliacoesCollection = avaliacoesCollection;
        }

        // Método Create para adicionar uma nova avaliação
        [HttpPost]
        public async Task<IActionResult> CreateReview([FromBody] Avaliacao avaliacao)
        {
            await _avaliacoesCollection.InsertOneAsync(avaliacao);
            return CreatedAtAction(nameof(GetReviewById), new { id = avaliacao.Id }, avaliacao);
        }

        // READ - Busca uma avaliação específica pelo Id
        [HttpGet("{id:length(24)}")]
        public async Task<ActionResult<Avaliacao>> GetReviewById(string id)
        {
            var avaliacao = await _avaliacoesCollection.Find(a => a.Id == id).FirstOrDefaultAsync();
            if (avaliacao == null)
            {
                return NotFound("Avaliação não encontrada.");
            }
            return Ok(avaliacao);
        }

        // READ ALL - Lista todas as avaliações
        [HttpGet]
        public async Task<ActionResult<List<Avaliacao>>> GetAllReviews()
        {
            var avaliacoes = await _avaliacoesCollection.Find(a => true).ToListAsync();
            return Ok(avaliacoes);
        }

        // READ - Busca todas as avaliações por Id do Usuário
        [HttpGet("usuario/{idUsuario}")]
        public async Task<ActionResult<List<Avaliacao>>> GetReviewsByUserId(string idUsuario)
        {
            var avaliacoes = await _avaliacoesCollection.Find(a => a.IdUsuario == idUsuario).ToListAsync();
            if (avaliacoes.Count == 0)
            {
                return NotFound("Nenhuma avaliação encontrada para o usuário especificado.");
            }
            return Ok(avaliacoes);
        }

        // READ - Busca todas as avaliações por Id do Filme
        [HttpGet("filme/{idTmdb}")]
        public async Task<ActionResult<List<Avaliacao>>> GetReviewsByFilmId(int idTmdb)
        {
            var avaliacoes = await _avaliacoesCollection.Find(a => a.IdTmdb == idTmdb).ToListAsync();
            if (avaliacoes.Count == 0)
            {
                return NotFound("Nenhuma avaliação encontrada para o filme especificado.");
            }
            return Ok(avaliacoes);
        }

        // UPDATE - Atualiza uma avaliação existente
        [HttpPut("{id:length(24)}")]
        public async Task<IActionResult> UpdateReview(string id, [FromBody] Avaliacao updatedReview)
        {
            updatedReview.Id = id; // Certifique-se de que o Id da avaliação seja mantido
            var result = await _avaliacoesCollection.ReplaceOneAsync(a => a.Id == id, updatedReview);
            if (result.MatchedCount == 0)
            {
                return NotFound("Avaliação não encontrada.");
            }
            return NoContent();
        }

        // DELETE - Remove uma avaliação pelo Id
        [HttpDelete("{id:length(24)}")]
        public async Task<IActionResult> DeleteReview(string id)
        {
            var result = await _avaliacoesCollection.DeleteOneAsync(a => a.Id == id);
            if (result.DeletedCount == 0)
            {
                return NotFound("Avaliação não encontrada.");
            }
            return NoContent();
        }

        // LIKE - Incrementa o campo Likes de uma avaliação
        [HttpPost("{id:length(24)}/like")]
        public async Task<IActionResult> LikeReview(string id)
        {
            var update = Builders<Avaliacao>.Update.Inc(a => a.Likes, 1);
            var result = await _avaliacoesCollection.UpdateOneAsync(a => a.Id == id, update);
            if (result.MatchedCount == 0)
            {
                return NotFound("Avaliação não encontrada.");
            }
            return NoContent();
        }

        // DISLIKE - Incrementa o campo Dislikes de uma avaliação
        [HttpPost("{id:length(24)}/dislike")]
        public async Task<IActionResult> DislikeReview(string id)
        {
            var update = Builders<Avaliacao>.Update.Inc(a => a.Dislikes, 1);
            var result = await _avaliacoesCollection.UpdateOneAsync(a => a.Id == id, update);
            if (result.MatchedCount == 0)
            {
                return NotFound("Avaliação não encontrada.");
            }
            return NoContent();
        }
    }
}
