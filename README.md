# API de Embalagens - Loja do Seu Manoel

Esta é uma API para ajudar o Seu Manoel a organizar as embalagens dos pedidos da sua loja de jogos. Ela decide quais caixas usar para cada pedido.

## O que faz

* Recebe pedidos (com produtos e suas dimensões) via JSON.
* Calcula como encaixar os produtos nas caixas disponíveis.
* Tenta usar o mínimo de caixas.
* Retorna um JSON com as caixas e os produtos dentro de cada uma.
* Registra as operações em um banco SQL Server.

## Tecnologias

* .NET 6
* EF Core
* SQL Server 2017 (via Docker)
* Docker / Docker Compose
* Swagger (para testes)

## Para Rodar

**Você vai precisar de:**
* Docker Desktop instalado e rodando.

**Passos:**
1.  Abra um terminal na pasta raiz deste projeto (onde está o `docker-compose.yml`).
2.  Execute:
    ```bash
    docker-compose up --build
    ```
    *(Na primeira vez, o SQL Server pode levar um ou dois minutos para iniciar por completo).*
3.  API disponível em: `http://localhost:8080`
4.  Swagger para testes: `http://localhost:8080/swagger`

## Testando a API

1.  Acesse o Swagger: `http://localhost:8080/swagger`
2.  Procure o endpoint `POST /api/packing/process`.
3.  Clique em "Try it out".
4.  No "Request body", cole um JSON de exemplo:
    ```json
    [
      {
        "id_pedido": "pedido_XPTO_001",
        "produtos": [
          {
            "altura": 10,
            "largura": 15,
            "comprimento": 20
          },
          {
            "altura": 25,
            "largura": 30,
            "comprimento": 70
          }
        ]
      }
    ]
    ```
5.  Clique em "Execute". Você deve receber uma resposta `200 OK`.

## Banco de Dados

A API usa um SQL Server (rodando no Docker) para registrar informações sobre os processamentos. O banco se chama `LojaDoManoelDB_Docker` e a tabela é `PackingJobLogs`.

Se quiser conectar direto no banco (com SSMS ou Azure Data Studio):
* **Servidor:** `localhost,1433` (ou `tcp:localhost,1433`)
* **Auth:** `Autenticação do SQL Server`
* **Login:** `sa`
* **Senha:** `SenhaF0rte!!123` (definida no `docker-compose.yml`)

*(Nota: Conectar ao SQL Server no Docker a partir de ferramentas no Windows às vezes requer atenção ao firewall para a porta 1433. A API se conecta ao banco normalmente dentro da rede Docker).*

## Para Parar Tudo

1.  No terminal onde o `docker-compose up` está rodando, pressione `Ctrl + C`.
2.  Para remover os containers, use:
    ```bash
    docker-compose down
    ```
    *(Os dados do banco serão perdidos ao rodar `docker-compose down`, pois estamos rodando o SQL Server sem volume persistente nesta configuração).*