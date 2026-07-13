# ⌚ Time Record Backend

API REST para gestão de ponto de funcionários, desenvolvida em **C# com .NET 9 e ASP.NET Core**.

O projeto implementa recursos para cadastro de funcionários, registro de marcações de ponto e consulta de histórico, utilizando **Entity Framework Core** e **PostgreSQL** para persistência dos dados. A aplicação também é containerizada com Docker e preparada para execução em ambientes Linux e pipelines de CI/CD.

## Visão geral

O **Time Record Backend** foi desenvolvido como uma Web API em **ASP.NET Core**, com foco na construção de serviços HTTP, organização de endpoints, persistência de dados e integração com aplicações cliente.

A API expõe operações para gerenciar funcionários e suas marcações de ponto, permitindo que o frontend desenvolvido em **Angular** consuma os dados por meio de endpoints REST documentados com Swagger/OpenAPI.

A camada de dados utiliza Entity Framework Core para comunicação com PostgreSQL, mantendo a string de conexão configurável por variáveis de ambiente. Isso evita dependências fixas no código e facilita a execução local, em contêineres e em cloud.

## Tecnologias

- **C#**
- **.NET 9**
- **ASP.NET Core Web API**
- **Entity Framework Core**
- **PostgreSQL**
- **Swagger / OpenAPI**
- **Angular**
- **Docker**
- **GitHub Actions**
- **Azure App Service**
- **Neon PostgreSQL**

## Funcionalidades

- CRUD de funcionários
- Registro de marcações de ponto
- Consulta de histórico de marcações por funcionário
- Persistência de dados com Entity Framework Core e PostgreSQL
- Documentação e testes de endpoints com Swagger/OpenAPI
- Integração com o frontend **Time Record**, desenvolvido em Angular

## Arquitetura e desenvolvimento

A aplicação foi estruturada para separar responsabilidades entre a camada de API, regras de negócio e persistência de dados.

O ASP.NET Core é responsável por receber requisições HTTP, validar dados e retornar respostas REST. O Entity Framework Core abstrai a comunicação com o banco PostgreSQL, permitindo trabalhar com entidades e consultas em C#.

A configuração da aplicação é baseada em variáveis de ambiente, especialmente para dados sensíveis e configurações dependentes do ambiente, como a connection string do banco de dados.

```text
Frontend (Angular)
        |
        v
ASP.NET Core Web API
        |
        v
Entity Framework Core
        |
        v
PostgreSQL (Neon)
```

## Swagger

A API disponibiliza documentação interativa com Swagger/OpenAPI para consulta de endpoints, contratos de entrada e saída e testes diretos via interface web.

Acesse a versão publicada:

[Swagger - Time Record Backend](https://timerecord-dev-b5cwhmadgrguhvb6.brazilsouth-01.azurewebsites.net/swagger/index.html)

> Em produção, a exposição do Swagger deve ser avaliada conforme os requisitos de segurança do ambiente, podendo ser limitada por autenticação, rede ou configuração de ambiente.

## Executando com Docker

O projeto utiliza Docker para padronizar o ambiente de execução da API.

A containerização garante que a aplicação seja executada com as mesmas dependências em desenvolvimento, integração contínua e produção, reduzindo diferenças entre sistemas operacionais e facilitando o deploy.

Exemplo de execução:

```bash
docker build -t time-record-backend .
docker run -p 8080:8080 \
  -e ConnectionStrings__DefaultConnection="SUA_CONNECTION_STRING" \
  time-record-backend
```

Após iniciar o contêiner, a documentação pode ser acessada em:

```text
http://localhost:8080/swagger
```

## Banco de dados

O backend utiliza PostgreSQL hospedado no Neon como serviço externo de persistência.

A separação entre a API e o banco de dados permite que cada componente seja configurado, atualizado e escalado de forma independente. A connection string é fornecida por variável de ambiente, sem expor credenciais no repositório.

Exemplo:

```bash
ConnectionStrings__DefaultConnection="Host=...;Database=...;Username=...;Password=..."
```

## CI/CD e deploy

O GitHub Actions automatiza etapas do ciclo de entrega, como build da aplicação, criação da imagem Docker e publicação no registry.

O deploy é realizado no Azure App Service com suporte a contêineres, permitindo publicar novas versões da API a partir de imagens geradas pelo pipeline.

```text
Push / Pull Request
        |
        v
GitHub Actions
        |
        v
Build .NET + Docker Image
        |
        v
Container Registry
        |
        v
Azure App Service
```

## Objetivo do projeto

Este projeto demonstra o desenvolvimento de uma API REST moderna com **C#, .NET e ASP.NET Core**, cobrindo:

- Criação de endpoints REST
- Modelagem e persistência de dados relacionais
- Uso de Entity Framework Core
- Configuração por ambiente
- Documentação de API com Swagger/OpenAPI
- Integração com frontend Angular
- Containerização com Docker
- Automação de build e deploy com GitHub Actions
- Publicação em ambiente cloud com Azure
