# Runbook de Deploy: API ASP.NET Core em Docker para Azure App Service

## Visão geral

Este runbook descreve, em passos reproduzíveis, como:

- Construir uma imagem Docker de uma API ASP.NET Core.
- Publicar essa imagem no Docker Hub.
- Criar um Azure App Service para rodar essa imagem como contêiner.
- Configurar variáveis de ambiente e porta corretamente.
- Validar logs e comportamento da aplicação em produção.

O fluxo se baseia em um cenário real de API ASP.NET Core (.NET 9) chamada **TimeRecord**, mas é facilmente adaptável para outras APIs.

## 1. Pré-requisitos

- Repositório com uma API ASP.NET Core funcional.
- Docker instalado e configurado na máquina de desenvolvimento.
- Conta no Docker Hub com permissões para criar repositórios.
- Assinatura Azure com permissão para criar App Service.

## 2. Dockerfile padrão para API ASP.NET Core

Utilizar um Dockerfile multi-stage para publicar a API e gerar uma imagem otimizada:

```dockerfile
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /app

COPY *.csproj .
COPY *.sln .
RUN dotnet restore TimeRecord.sln

COPY . .
RUN dotnet publish TimeRecord.sln -c Release -o out

FROM mcr.microsoft.com/dotnet/aspnet:9.0
WORKDIR /app

EXPOSE 8080
ENV ASPNETCORE_URLS=http://+:8080

COPY --from=build /app/out .
ENTRYPOINT ["dotnet", "TimeRecord.dll"]
```

Pontos importantes:

- `EXPOSE 8080`: define a porta que o container vai expor.
- `ASPNETCORE_URLS=http://+:8080`: força o ASP.NET Core a ouvir em todas as interfaces na porta 8080, o que é compatível com Azure App Service.

## 3. Build e teste local da imagem

1. No diretório raiz do projeto (onde está o Dockerfile), construir a imagem:

```bash
docker build -t <usuario-docker-hub>/time-record:latest .
```

2. Testar localmente a imagem:

```bash
docker run -p 8080:8080 <usuario-docker-hub>/time-record:latest
```

3. Acessar `http://localhost:8080/swagger` (ou rota equivalente) para validar que a API está funcional.

Erros comuns observados:

- Escrever o nome/tags da imagem errado (por exemplo, `lastest` em vez de `latest`).
- Trocar o nome do repositório entre `time-record` e `timerecord`.

## 4. Publicar imagem no Docker Hub

1. Fazer login no Docker Hub via CLI:

```bash
docker login
```

2. Publicar a imagem:

```bash
docker push <usuario-docker-hub>/time-record:latest
```

3. Confirmar no painel do Docker Hub que o repositório e a tag `latest` foram criados.

Observação: o `docker build` cria a imagem apenas localmente; o `docker push` é que envia para o Docker Hub.

## 5. Criar Azure App Service para contêiner

### 5.1 Criar o Aplicativo Web

1. Acessar o portal Azure (`https://portal.azure.com`).
2. Navegar até **App Services** e clicar em **Criar**.
3. Na aba **Básico**:
    - Tipo de recurso: **Aplicativo Web**.
    - Publicar: **Contêiner do Docker**.
    - Sistema operacional: **Linux**.
    - Nome do aplicativo: por exemplo, `timerecord-dev`.
    - Plano de App Service: escolher um plano compatível (por exemplo, B1 para ambientes de teste).

### 5.2 Configurar o contêiner (Docker Hub)

Na aba **Contêiner**:

- Suporte a sidecar: manter padrão (ativado se não houver necessidade especial).
- Origem da imagem: **Outros registros de contêiner**.
- Tipo de acesso: **Público**.
- URL do servidor de registro: `https://index.docker.io`.
- Imagem e marca: `<usuario-docker-hub>/time-record:latest`.
- Porta: `8080`.
- Comando de inicialização: deixar em branco.

Concluir em **Revisar + criar** e criar o recurso.

Erros comuns:

- Informar a tag errada ou repositório diferente do que foi publicado.
- Esquecer de configurar a porta correta (8080) na aba de contêiner.

## 6. Configurar variáveis de ambiente no App Service

### 6.1 Conceito

No ambiente local, o `docker-compose` pode traduzir variáveis como `DB_CONNECTION` e `JWT_PRIVATE_KEY` para os nomes esperados pelo código. No Azure App Service, o `docker-compose` **não é utilizado**; as variáveis precisam ser definidas diretamente na seção de configurações do aplicativo.

### 6.2 Configurações do aplicativo (Application Settings)

No portal, dentro do App Service:

1. Navegar até **Configurações** → **Variáveis de ambiente** → aba **Configurações do aplicativo**.
2. Adicionar as seguintes chaves (nomes exatos que o código ASP.NET Core espera):

- `ASPNETCORE_ENVIRONMENT` = `Development` ou `Production`.
- `ConnectionStrings__POSTGRESQLCONNSTR_AppDbConnectionString` = string de conexão completa do banco PostgreSQL.
- `Jwt__PrivateKey` = chave secreta usada para gerar tokens JWT.
- `Jwt__Issuer` = `Timerecord` (ou valor equivalente usado no código).
- `Jwt__Audience` = `Timerecord`.
- `WEBSITES_PORT` = `8080`.

3. Salvar as configurações e permitir o restart do App Service.

Problemas que ocorreram e correções:

- **Erro de JWT** (`Cannot create a SymmetricSecurityKey, key length is zero`):
    - Causa: variável `Jwt__PrivateKey` não estava definida com o nome correto; havia apenas `JWT_PRIVATE_KEY` (nome do compose).
    - Correção: criar a configuração `Jwt__PrivateKey` com o valor da chave.

- **Connection string não encontrada**:
    - Causa: apenas `DB_CONNECTION` foi criada no App Service.
    - Correção: criar `ConnectionStrings__POSTGRESQLCONNSTR_AppDbConnectionString` com o valor da connection string.

## 7. Logs e diagnóstico

Para acompanhar o comportamento da aplicação em produção:

1. No App Service, abrir **Fluxo de log** (Log stream).
2. Verificar logs de runtime do contêiner (stdout/stderr), incluindo:
    - Erros de middleware de exceção.
    - Exceções de autenticação JWT.
    - Problemas de conexão com banco.

Esse recurso substitui o `docker logs` local e é fundamental para troubleshooting em produção.

## 8. Testes finais

1. Após configurar as variáveis e porta, acessar:

- `https://<nome-da-app>.azurewebsites.net/swagger`

2. Validar:

- Endpoints públicos respondendo.
- Endpoints protegidos por JWT retornando erro adequado quando não autenticados.
- Ausência de erros críticos nos logs no startup.

## 9. Boas práticas observadas

- Separar imagem (Dockerfile) de configuração de ambiente (App Service).
- Usar `ASPNETCORE_URLS=http://+:8080` para compatibilidade com App Service.
- Centralizar secrets (JWT, connection strings) em variáveis de ambiente, não no Dockerfile.
- Usar Docker Hub como registry para distribuição da mesma imagem entre ambientes (dev, teste, produção).

## 10. Próximos passos recomendados

- Criar pipeline de CI/CD com GitHub Actions para automatizar:
    - Build da imagem.
    - Push para Docker Hub.
    - Atualização do App Service.
- Configurar domínios personalizados e certificados HTTPS.
- Adicionar endpoint de health check e integração com ferramentas de observabilidade.