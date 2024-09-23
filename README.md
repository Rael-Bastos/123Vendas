# 123Vendas - Sistema de Vendas

Este projeto é uma API de vendas para o sistema **123Vendas**. Ele utiliza **.NET 8**, **Entity Framework Core** para acesso a dados e **SEQ** para gerenciamento de logs.

## Requisitos

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- [SQL Server](https://www.microsoft.com/en-us/sql-server/sql-server-downloads)
- [Docker](https://www.docker.com/) (opcional, para execução do SEQ via container)
- [SEQ](https://datalust.co/seq) (para visualização de logs)

## Configuração do SEQ

Para visualizar os logs gerados pela API, o **SEQ** precisa estar configurado e rodando. Você pode configurar o SEQ localmente ou em um ambiente Docker.

### Executando SEQ via Docker

Se você tiver o Docker instalado, pode executar o SEQ com o seguinte comando:

```bash
docker run --name seq -d --restart unless-stopped -e ACCEPT_EULA=Y -p 5341:80 datalust/seq
