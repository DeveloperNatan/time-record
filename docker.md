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