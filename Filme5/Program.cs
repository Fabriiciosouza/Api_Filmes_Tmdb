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
            // Cria um novo construtor de aplicativo web para configurar os servi�os e o pipeline de requisi��es
            var builder = WebApplication.CreateBuilder(args);

            // Adiciona a configura��o do MongoDbSettings a partir do arquivo de configura��o (appsettings.json)
            // Isso permite acesso �s configura��es do MongoDB (como string de conex�o e nome do banco de dados)
            builder.Services.Configure<MongoDbSettings>(
                builder.Configuration.GetSection("MongoDbSettings"));

            // Registra uma inst�ncia �nica de IMongoClient, que � usada para se conectar ao MongoDB
            // Aqui, a string de conex�o � extra�da das configura��es do MongoDbSettings
            builder.Services.AddSingleton<IMongoClient, MongoClient>(sp =>
            {
                var settings = sp.GetRequiredService<IOptions<MongoDbSettings>>().Value;
                return new MongoClient(settings.ConnectionString);
            });

            // Registra uma cole��o do MongoDB para avalia��es
            // Isso facilita o acesso direto � cole��o de Avalia��es no banco de dados MongoDB
            builder.Services.AddSingleton(sp =>
            {
                var settings = sp.GetRequiredService<IOptions<MongoDbSettings>>().Value;
                var client = sp.GetRequiredService<IMongoClient>();
                var database = client.GetDatabase(settings.DatabaseName);
                return database.GetCollection<Avaliacao>(settings.AvaliacoesCollection);
            });

            // Registra os servi�os customizados da aplica��o (MovieService e TmdbService) como singletons
            // Isso significa que haver� uma �nica inst�ncia desses servi�os durante toda a execu��o da aplica��o
            builder.Services.AddSingleton<MovieService>();
            builder.Services.AddSingleton<TmdbService>();

            // Configura para que o app use o arquivo appsettings.json para carregar as configura��es
            builder.Configuration.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

            // Adiciona o suporte para controladores de API
            // Isso permite que a aplica��o utilize endpoints baseados em controladores
            builder.Services.AddControllers();

            // Configura o Swagger para documenta��o de API
            // Swagger � uma ferramenta que facilita a visualiza��o e teste dos endpoints da API
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            // Cria o aplicativo final com todas as configura��es definidas acima
            var app = builder.Build();

            // Configura o pipeline de requisi��es HTTP
            // Verifica se o ambiente � de desenvolvimento; caso positivo, habilita o Swagger para visualiza��o
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            // For�a o redirecionamento de HTTP para HTTPS para aumentar a seguran�a
            app.UseHttpsRedirection();

            // Habilita a autoriza��o para a aplica��o (caso tenha endpoints protegidos)
            app.UseAuthorization();

            // Define o roteamento dos controladores; mapeia as rotas automaticamente para os controladores da API
            app.MapControllers();

            // Inicia a aplica��o e come�a a escutar as requisi��es
            app.Run();
        }
    }
}
