API de Filmes com ASP.NET Core e TMDb
🎥 Sobre o Projeto

A API de Filmes é uma aplicação desenvolvida para consumir dados da API TMDb e fornecer informações completas sobre filmes. Este projeto foi criado para facilitar o acesso a detalhes como sinopse, elenco, trailers, orçamento, e onde assistir, armazenando os dados em um banco de dados MongoDB.

📝 Descrição Geral

A API permite que os usuários consultem informações detalhadas sobre os filmes, incluindo categorias como lançamentos, melhores avaliados e em cartaz. Com um CRUD de avaliações, a aplicação possibilita que os usuários interajam com filmes, deixando comentários, curtidas e descurtidas. O objetivo é fornecer uma experiência completa de informações de filmes para aplicativos e sites que desejam integrar esses dados de maneira eficiente.

⚙️ Funcionalidades Principais

Informações Detalhadas de Filmes: A API armazena dados no MongoDB, incluindo sinopse, elenco, trailers, orçamento e receita de cada filme. Esses dados são acessíveis e podem ser filtrados por ID para consultas específicas.

Categorias de Filmes: Organiza os filmes em diversas coleções, como "Em Cartaz", "Lançamentos", "Melhores Avaliados" e "Tendências", permitindo acesso rápido a categorias populares de forma dinâmica.

CRUD de Avaliações: Gerencia comentários, curtidas e descurtidas para cada filme. As avaliações são armazenadas no MongoDB e podem ser acessadas por ID de filme ou usuário, permitindo uma interação personalizada.

Integração Completa com TMDb: A API busca e exibe dados diretamente do TMDb, garantindo que todas as informações de filmes estejam atualizadas.
🚀 Tecnologias Utilizadas

ASP.NET Core: Backend da API.
MongoDB: Armazenamento dos dados de filmes e avaliações.
RestSharp: Consumo da API TMDb para busca de informações.

Desenvolvido por Fabricio Souza
