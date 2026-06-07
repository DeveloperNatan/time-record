⌚ Ponto Fácil Backend

API REST para registro e gestão de ponto de funcionários, desenvolvida com ASP.NET Core e focada em uma abordagem moderna de execução em contêiner, automação de pipeline e publicação em cloud.
Visão geral

O backend do Ponto Fácil foi construído em .NET 9 com ASP.NET Core Web API, utilizando C# e Entity Framework Core para expor endpoints de cadastro de funcionários, registro de marcações de ponto e consulta de histórico.

A aplicação utiliza um banco de dados PostgreSQL externo hospedado no Neon, o que desacopla a camada de persistência do contêiner da aplicação e aproxima o projeto de uma arquitetura mais próxima de ambiente real de produção.
Tecnologias principais

    .NET 9 / ASP.NET Core Web API

    C#

    Entity Framework Core

    PostgreSQL externo (Neon)

    Docker

    GitHub Actions

    Azure para hospedagem do contêiner

Funcionalidades

    CRUD de funcionários

    Registro de marcações de ponto

    Consulta de histórico de marcações por funcionário

    Integração com o frontend Ponto Fácil em Next.js

    Exposição de documentação interativa da API via Swagger/OpenAPI.

Swagger e consumo da API

A API disponibiliza documentação interativa via Swagger, permitindo visualizar endpoints, contratos de entrada e saída e testar chamadas HTTP diretamente pela interface.

URL publicada:

    [Swagger da aplicação](https://timerecord-dev-b5cwhmadgrguhvb6.brazilsouth-01.azurewebsites.net/swagger/index.html)

Como boa prática, Swagger em produção deve ser avaliado com cuidado e, em cenários mais rígidos, protegido ou limitado por ambiente, autenticação ou regra de acesso.
Docker

Um dos principais destaques técnicos do projeto é a execução do backend em contêiner Docker, o que padroniza o ambiente, reduz diferenças entre máquina local e cloud e facilita build, distribuição e deploy.

O uso de Docker neste projeto traz benefícios práticos:

    empacotamento consistente da aplicação e suas dependências;

    facilidade para publicar versões da imagem;

    integração direta com pipeline automatizado;

    caminho mais simples para deploy em serviços gerenciados do Azure.

GitHub Actions

O projeto utiliza GitHub Actions para automação de etapas do ciclo de entrega. Workflows do GitHub Actions permitem reagir a eventos como pull_request e push, executando testes, build e publicação de artefatos automaticamente.

No cenário atual do projeto, o pipeline foi estruturado para publicar a imagem Docker em repositório de imagens, usando actions reutilizáveis para checkout do código, autenticação no registry e build/push da imagem.

Esse fluxo normalmente envolve ações como:

    actions/checkout para baixar o código no runner;

    docker/login-action para autenticação segura no registry;

    docker/build-push-action para buildar e publicar a imagem Docker.

Essa automação reduz trabalho manual, melhora repetibilidade do processo e torna o projeto mais alinhado a práticas reais de CI/CD.
Azure

O deploy do backend é feito no Azure com uma abordagem baseada em contêiner, o que facilita a publicação de novas versões da aplicação e torna o processo compatível com pipelines automatizados.

No Azure App Service para contêineres, é possível configurar CI/CD para imagens customizadas e integrar a aplicação com um fluxo de publicação vindo do GitHub Actions.

Em cenários com atualização contínua de imagem, o Azure App Service pode ser configurado para detectar atualizações no registry e reiniciar a aplicação para fazer novo pull da imagem publicada.

Esse modelo é interessante porque separa claramente responsabilidades:

    o GitHub Actions gera e publica a imagem;

    o registry armazena a versão da aplicação;

    o Azure executa o contêiner em ambiente gerenciado.

Banco de dados externo

O projeto utiliza PostgreSQL externo no Neon, o que é uma decisão importante do ponto de vista arquitetural. Em vez de acoplar o banco ao mesmo contêiner da API, a aplicação consome um serviço de dados dedicado, o que é mais aderente a boas práticas de produção.

Esse desenho reforça conceitos importantes para quem avalia o projeto:

    separação entre aplicação e persistência;

    maior flexibilidade de deploy;

    possibilidade de escalar ou trocar a camada de banco de forma independente;

    alinhamento com arquitetura cloud-native.

A documentação interativa da API está disponível no Swagger publicado em produção, o que facilita testes, inspeção de contratos e validação rápida dos recursos disponíveis.

Link de acesso:

    https://timerecord-dev-b5cwhmadgrguhvb6.brazilsouth-01.azurewebsites.net/swagger/index.html
