using Filme5.Settings;
using Filme5.Services;
using MongoDB.Driver;
using Microsoft.Extensions.Options;
using Filme5.Models;

namespace Filme5
{
    public class Program
    {
        public static void Main(string[] args)
        {
            // Cria um novo construtor de aplicativo web para configurar os serviços e o pipeline de requisições
            var builder = WebApplication.CreateBuilder(args);

            // Adiciona a configuração do MongoDbSettings a partir do arquivo de configuração (appsettings.json)
            // Isso permite acesso às configurações do MongoDB (como string de conexão e nome do banco de dados)
            builder.Services.Configure<MongoDbSettings>(
                builder.Configuration.GetSection("MongoDbSettings"));

            // Registra uma instância única de IMongoClient, que é usada para se conectar ao MongoDB
            // Aqui, a string de conexão é extraída das configurações do MongoDbSettings
            builder.Services.AddSingleton<IMongoClient, MongoClient>(sp =>
            {
                var settings = sp.GetRequiredService<IOptions<MongoDbSettings>>().Value;
                return new MongoClient(settings.ConnectionString);
            });

            // Registra uma coleção do MongoDB para avaliações
            // Isso facilita o acesso direto à coleção de Avaliações no banco de dados MongoDB
            builder.Services.AddSingleton(sp =>
            {
                var settings = sp.GetRequiredService<IOptions<MongoDbSettings>>().Value;
                var client = sp.GetRequiredService<IMongoClient>();
                var database = client.GetDatabase(settings.DatabaseName);
                return database.GetCollection<Avaliacao>(settings.AvaliacoesCollection);
            });

            // Registra os serviços customizados da aplicação (MovieService e TmdbService) como singletons
            // Isso significa que haverá uma única instância desses serviços durante toda a execução da aplicação
            builder.Services.AddSingleton<MovieService>();
            builder.Services.AddSingleton<TmdbService>();

            // Configura para que o app use o arquivo appsettings.json para carregar as configurações
            builder.Configuration.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

            // Adiciona o suporte para controladores de API
            // Isso permite que a aplicação utilize endpoints baseados em controladores
            builder.Services.AddControllers();

            // Configura o Swagger para documentação de API
            // Swagger é uma ferramenta que facilita a visualização e teste dos endpoints da API
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            // Cria o aplicativo final com todas as configurações definidas acima
            var app = builder.Build();

            // Configura o pipeline de requisições HTTP
            // Verifica se o ambiente é de desenvolvimento; caso positivo, habilita o Swagger para visualização
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            // Força o redirecionamento de HTTP para HTTPS para aumentar a segurança
            app.UseHttpsRedirection();

            // Habilita a autorização para a aplicação (caso tenha endpoints protegidos)
            app.UseAuthorization();

            // Define o roteamento dos controladores; mapeia as rotas automaticamente para os controladores da API
            app.MapControllers();

            // Inicia a aplicação e começa a escutar as requisições
            app.Run();
        }
    }
}
