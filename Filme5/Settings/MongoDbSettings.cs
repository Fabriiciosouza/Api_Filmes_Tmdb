using System;

namespace Filme5.Settings
{
    public class MongoDbSettings
    {
        // ConnectionString armazena a string de conexão com o servidor MongoDB.
        // É utilizada para estabelecer a conexão com o banco de dados.
        public string ConnectionString { get; set; } = string.Empty;

        // DatabaseName define o nome do banco de dados que será usado na aplicação.
        // Todas as operações de armazenamento e recuperação de dados ocorrerão nesse banco.
        public string DatabaseName { get; set; } = string.Empty;

        // PopularesCollection especifica o nome da coleção onde os filmes populares são armazenados.
        // Coleções em MongoDB são como tabelas em bancos relacionais.
        public string PopularesCollection { get; set; } = "FilmesPopulares";

        // EmCartazCollection define a coleção onde são armazenados os filmes em cartaz atualmente.
        public string EmCartazCollection { get; set; } = "FilmesEmCartaz";

        // LancamentosCollection representa a coleção que guarda os filmes recentemente lançados.
        public string LancamentosCollection { get; set; } = "FilmesLancamentos";

        // MelhoresAvaliadosCollection armazena o nome da coleção de filmes que possuem as melhores avaliações.
        public string MelhoresAvaliadosCollection { get; set; } = "FilmesMelhoresAvaliados";

        // TrendingDiaCollection define a coleção onde são armazenados filmes populares no dia.
        public string TrendingDiaCollection { get; set; } = "FilmesTrendingDia";

        // TrendingSemanaCollection é a coleção para filmes populares durante a semana.
        public string TrendingSemanaCollection { get; set; } = "FilmesTrendingSemana";

        // AvaliacoesCollection define o nome da coleção onde as avaliações dos usuários são armazenadas.
        // Essa coleção guarda feedbacks sobre os filmes, como comentários, likes e dislikes.
        public string AvaliacoesCollection { get; internal set; } = "Avaliacoes";
    }
}
