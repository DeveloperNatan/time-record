 EF CORE commands

//Add new migration//
dotnet ef migrations add "name migration"

//Apply migrations//
dotnet ef database update

//Verify migrations//
dotnet ef migrations list


.NET commands
//Restore dependencies//
dotnet restore

//Build//
dotnet build



//Docker//
Buildar + rodar 
docker compose up --build
(se quiser em background junto: docker compose up -d --build)​

Só rodar 
docker compose up
(em background: docker compose up -d)

Só buildar 
docker compose build

//sem compose
docker build -t name:tag .(diretorio atual)

docker run --name timerecord-container -p 5000:8080 timerecord:dev

docker run -d --name timerecord-container -p 5000:8080 timerecord:dev


docker ps                       # containers rodando
docker logs timerecord-container
docker stop timerecord-container
docker rm timerecord-container

- proximos passos segunda feira 
Autenticação com banco (primeiro passo)

No login:

    Você busca o usuário por email no banco.

    Verifica a senha digitada contra PasswordHash (bcrypt/argon2; não compare string pura).

    Se estiver ok, gera JWT com id e roles.​

Exemplo do core (no service), a ideia é essa: Verify("senha", user.PasswordHash) e só então gerar token.​
Segredos e chaves (não vão no banco do usuário)

A “chave do JWT” continua sendo da aplicação/ambiente, não do usuário, e não precisa estar no banco; ela fica em config/variável de ambiente/KeyVault.​
O banco guarda usuários (email, password hash, roles), não a chave de assinatura do token.​
Dica específica pro teu model Roles

No EF, string[] Roles com PostgreSQL até pode funcionar (array), mas pra começar mais fácil costuma ser:

    Uma tabela UserRoles (N:N) ou

    Um campo string (ex.: "developer,admin") só pra protótipo.


