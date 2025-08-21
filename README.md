# Order Management System (PoC)

Este repositório contém uma **prova de conceito** de um sistema de **gestão de pedidos (Order Management System)**, desenvolvido em **.NET 8**.  
A solução foi estruturada para demonstrar boas práticas de arquitetura, mensageria assíncrona e frontend moderno.

## Setup

### Pré-requisitos
- [.NET 8 SDK](https://dotnet.microsoft.com/en-us/download/dotnet/8.0)
- [Docker + Docker Compose](https://www.docker.com/)
- [Node.js 20+](https://nodejs.org/) (para rodar o frontend localmente, caso queira)

### Clonar o repositório
```bash
git clone https://github.com/felipespinelli/poc-tmb.git
cd poc-tmb
````

### Configuração de variáveis

Acesse a pasta Docker, na raiz do repositório, crie e edite um arquivo **`.env`**, conforme necessário:

```env
# caminho onde os dados do Postgres serão salvos
POSTGRES_DATA=C:\\Data\\postgresql  

# porta onde a API ficará disponível
API_PORT=5000

# porta onde o Worker ficará disponível
WORKER_PORT=6000

# caminho completo para o arquivo ./Docker/asb-emulator.config.json
CONFIG_PATH=C:\\Documentos\\desenvolvimento\\github\\FelipeSpinelli\\poc-tmb\\Docker\\asb-emulator.config.json

# Manter como "Y" para funcionamento do emulador do Azure Service Bus
ACCEPT_EULA="Y"                                                      

# Define uma senha de sua preferência 
MSSQL_SA_PASSWORD=""

# porta onde o frontend (React) ficará disponível
WEBAPP_PORT=9000
```
---

## Execução

### Subir toda a stack (API + Worker + Postgres + WebApp)

Na raiz do projeto, execute:

```bash
cd ./Docker
docker-compose up -d --build
```

Isso vai levantar:

* **Postgres** → `localhost:5432`
* **Azure Service Bus** → Emulador oficial da Microsoft
* **SQL Server** → Utilizado pelo emulador do Azure Service Bus
* **API (.NET 8 Minimal API)** → `http://localhost:5000`
* **Worker (.NET 8 BackgroundService)** → processa mensagens do Azure Service Bus
* **WebApp (React + Tailwind)** → `http://localhost:5173`

Para parar os serviços:

```bash
docker-compose down
```
---

## Detalhes Técnicos

### Estrutura da Solução

A solução foi organizada em múltiplos projetos para manter o baixo acoplamento e a alta coesão:

* **Tmb.OrderManagementSystem.Api**

  * Implementa uma **Minimal API** em .NET 8.
  * Exposição dos endpoints REST para criar, listar e consultar pedidos.

* **Tmb.OrderManagementSystem.Worker**

  * Serviço em segundo plano (BackgroundService).
  * Consome mensagens da fila do **Azure Service Bus** e processa mensagens de forma assíncrona.

* **Tmb.OrderManagementSystem.Core**
  Estrutura em **três camadas lógicas**:

  * **Application** → orquestra os fluxos de negócio e contém abstrações que desacoplam a infraestrutura.
  * **Domain** → entidade `Order` (Pedido) e suas regras de negócio.
  * **Infra** → implementa acesso a banco (Postgres via EF Core) e mensageria (Azure Service Bus).

### Infraestrutura

* **PostgreSQL**: banco relacional usado para persistência de pedidos.
* **Entity Framework Core**: ORM para mapeamento objeto-relacional.
* **Azure Service Bus**: mensageria assíncrona para processar mudanças de status de pedidos.

### Frontend

* **React 18 + Vite + TypeScript**
* **Tailwind CSS** para estilização rápida e responsiva.
* Funcionalidades:

  * Criar novos pedidos
  * Listar pedidos com paginação
  * Visualizar detalhes de pedidos
  * Integração direta com a API REST

---

## 📌 Endpoints da API

* `POST /orders` → Cria um pedido
* `GET /orders` → Lista pedidos (com paginação: `?pageNumber={pageNumber}&pageSize={pageSize}`)
* `GET /orders/{id}` → Retorna detalhes de um pedido específico

---

## ✅ Critérios Implementados

* Arquitetura em camadas (Application, Domain, Infra)
* API Minimal em .NET 8
* Worker assíncrono com Azure Service Bus
* Persistência com PostgreSQL + EF Core
* Frontend em React + Tailwind conectado à API
* Docker Compose para execução completa do ambiente

---

## 🚀 Próximos Passos (Possíveis Melhorias)

* Implementar SignalR para notificações em tempo real quando o status do pedido mudar.
* Testes automatizados (unitários e de integração).
