# 📚 Livraria API - Projeto Final

API REST desenvolvida em **.NET 8** para gestão de uma Livraria Online, integrada com autenticação **JWT**, resiliência e cache com **Polly**, e simulação de serviços externos via **Mountebank** (Imposter).

## 🏗️ Arquitetura do Projeto

- **Backend:** ASP.NET Core Web API (.NET 8)
- **Segurança:** JWT (JSON Web Tokens)
- **Resiliência e Cache:** Polly (Polly Cache In-Memory, Retry Policy e Circuit Breaker)
- **Mocks Externos:** Mountebank (Porta 4545) para simulação de Inventário e Pagamentos
- **Documentação:** Swagger / OpenAPI

---

## 📁 Estrutura do Repositório

```text
api-dario-PI0924-projeto-final/
├── api/
│   ├── ApiAnaForteProjetoFinal/
│   │   ├── Controllers/       # Endpoints REST (Auth, Books)
│   │   ├── Models/            # Entidades e DTOs
│   │   ├── Services/          # Lógica de negócio e chamadas HTTP
│   │   ├── Cache/             # Implementação da Polly Cache
│   │   ├── Resilience/        # Políticas de Retry e Circuit Breaker da Polly
│   │   ├── Program.cs         # Configuração de serviços e middlewares
│   │   └── Dockerfile         # Ficheiro de containerização
├── imposter/
│   └── mountebank.json        # Configuração das respostas simuladas do Mountebank
├── tests/
│   └── requests.http          # Coleção de testes para Postman/VS Code REST Client
└── README.md
```

---

## 🚀 Como Executar o Projeto Passo a Passo

### 1️⃣ Arrancar o Mountebank (Imposter)
1. Abre a linha de comandos (`cmd`) na pasta onde o Mountebank está instalado.
2. Arranca o imposter apontando para o ficheiro de configuração:
   ```bash
   mb --configfile "C:\Users\Anapa\OneDrive - ATEC - Academia de Formação\zOutros\Ambiente de Trabalho\ProjAPI\imposter\mountebank.json"
   ```
3. O Mountebank ficará ativo a responder na porta `4545`.

### 2️⃣ Executar a API em .NET 8
1. Abre a solução no **Visual Studio 2022**.
2. Pressiona **`F5`** para compilar e iniciar.
3. O navegador abrirá automaticamente na página do **Swagger**: `https://localhost:7207/swagger`.

---

## 🧪 Roteiro de Testes

### 🔓 1. Autenticação JWT
1. No Swagger, acede ao endpoint `POST /api/Auth/login`.
2. Envia o corpo com o utilizador `dario` e palavra-passe `password123`.
3. Copia o `token` devolvido.
4. Clica no botão **Authorize** no topo do Swagger e insere: `Bearer <teu_token>` (sem aspas).

### ⚡ 2. Polly Cache In-Memory
1. Executa o endpoint `GET /api/Books/1`.
2. Verifica na consola da aplicação que a primeira leitura foi realizada na fonte de dados.
3. Repete o pedido nos 30 segundos seguintes e observa a mensagem no terminal: `[Polly Cache] LIVRO ID 1 RETORNADO DA CACHE LOCAL (RÁPIDO)!`.

### 🛡️ 3. Resiliência com Polly (Retry e Circuit Breaker)
1. Com o Mountebank ligado, executa `GET /api/Books/1/stock-externo`. O resultado será `200 OK` com dados de stock.
2. Fecha a janela do Mountebank para simular falha externa.
3. Executa a compra de um livro em `POST /api/Books/comprar`.
4. Observa a consola da API:
   - A **Política de Retry** efetuará 3 tentativas automáticas.
   - Em caso de falhas consecutivas, a **Política de Circuit Breaker** abrirá o disjuntor durante 15 segundos para proteger o sistema.