# 🐾 VitalPet Tech API

## 📖 Descrição do Projeto

A VitalPet Tech API é um sistema back-end desenvolvido em C# com .NET e Entity Framework Core. O projeto tem como objetivo gerenciar a saúde e o monitoramento de pets. Ele permite o cadastro de tutores e seus animais, o registro de histórico médico (eventos clínicos como vacinas e consultas) e a recepção de alertas gerados por sensores IoT (monitoramento de batimento cardíaco, temperatura e localização do animal).

Além das funcionalidades de cadastro e consulta, a API conta com uma camada de **Monitoramento e Observabilidade** (Health Checks, logging estruturado com Serilog e distributed tracing com OpenTelemetry) e uma suíte de **testes automatizados** (unitários e de integração, seguindo o padrão AAA) cobrindo Tutores, Pets, Eventos Clínicos e Alertas.

Projeto desenvolvido por: Gabriel Müller RM561995 Jonas Santos RM563234 Pedro Ferreira RM565824

## 🛠️ Instruções de Instalação e Execução

### Pré-requisitos

* [.NET SDK](https://dotnet.microsoft.com/download) (Versão 8.0 ou superior)
* SQL Server (ou outro banco de dados relacional compatível configurado)
* Uma IDE de sua preferência (Visual Studio, VS Code, Rider)

### Passo a passo para rodar o projeto

1. Clone o repositório: git clone https://github.com/glMuller/VitalPet_Tech.git
2. Configure o Banco de Dados: Abra o arquivo `appsettings.json` e configure a sua string de conexão na chave `ConnectionStrings:ConexaoBanco` (RM561995 - 200107 apontando para o seu servidor local.
3. Crie o Banco de Dados (Migrations): Abra o terminal na raiz do projeto e execute o comando: `dotnet ef database update`
4. Execute a aplicação: No terminal, execute: `dotnet run`
5. Acesse a documentação interativa: Com a aplicação rodando, abra o navegador e acesse o Swagger para testar os endpoints: `http://localhost:porta/swagger`

## 📌 Documentação das Rotas

Abaixo estão listados os principais endpoints da API.

### 👤 Tutores (/api/Tutor)

* `GET /api/Tutor`: Retorna todos os tutores cadastrados e seus pets.
* `GET /api/Tutor/{id}`: Retorna os dados de um tutor específico e seus pets.
* `POST /api/Tutor`: Cadastra um novo tutor.
* `PUT /api/Tutor/{id}`: Atualiza os dados de um tutor existente.
* `DELETE /api/Tutor/{id}`: Remove um tutor do sistema.

### 🐾 Pets (/api/Pet)

* `GET /api/Pet`: Retorna todos os pets e os dados de seus respectivos tutores.
* `GET /api/Pet/{id}`: Retorna um pet específico.
* `GET /api/Pet/especie/{especie}`: Retorna todos os pets filtrados por uma espécie específica.
* `GET /api/Pet/tutor/{tutorId}`: Retorna todos os pets pertencentes a um tutor específico.
* `POST /api/Pet`: Cadastra um novo pet.
* `PUT /api/Pet/{id}`: Atualiza os dados de um pet.
* `DELETE /api/Pet/{id}`: Remove um pet.

### 🏥 Eventos Clínicos (/api/EventoClinico)

* `GET /api/EventoClinico`: Retorna todos os registros clínicos.
* `GET /api/EventoClinico/{id}`: Retorna um evento clínico específico.
* `GET /api/EventoClinico/pet/{petId}`: Retorna o histórico médico de um pet, ordenado do mais recente para o mais antigo.
* `POST /api/EventoClinico`: Adiciona um novo evento clínico.
* `PUT /api/EventoClinico/{id}`: Atualiza os dados de um evento.
* `DELETE /api/EventoClinico/{id}`: Remove um evento clínico.

### 🚨 Alertas de Sensores (/api/Alerta)

* `GET /api/Alerta`: Retorna todo o histórico de alertas emitidos pelos sensores.
* `GET /api/Alerta/{id}`: Retorna os detalhes de um alerta específico.
* `GET /api/Alerta/pendentes`: Retorna apenas os alertas que não foram resolvidos.
* `POST /api/Alerta`: Registra um novo alerta no sistema.
* `PUT /api/Alerta/{id}`: Atualiza um alerta (marca como resolvido).
* `DELETE /api/Alerta/{id}`: Remove um alerta do histórico.

## 📊 Monitoramento e Observabilidade

### Health Check

A API expõe um endpoint de verificação de saúde que valida a conectividade com o banco de dados Oracle:


**Exemplo de resposta (aplicação saudável):**

```json
{
  "status": "Healthy",
  "totalDuration": "00:00:00.7050745",
  "entries": {
    "oracle-database": {
      "data": {},
      "duration": "00:00:00.6985029",
      "status": "Healthy",
      "tags": ["db", "oracle"]
    }
  }
}
```

Se o banco Oracle estiver indisponível, o campo `status` retorna `Unhealthy` e o endpoint responde com HTTP 503 (Service Unavailable).

**Como monitorar a aplicação:**
- Acesse `https://localhost:{porta}/health` no navegador, Postman ou via `curl` para um diagnóstico rápido do estado da API e do banco de dados.
- O endpoint pode ser integrado a ferramentas de orquestração (Kubernetes liveness/readiness probes, Docker healthcheck, load balancers) para decidir automaticamente se a instância deve receber tráfego.
- **Logs estruturados** (via Serilog) são gravados em tempo real no console e também persistidos em arquivo, em `Logs/log-{data}.txt`, com rotação diária. Cada requisição HTTP é registrada automaticamente com método, rota, status de resposta e tempo de execução.
- **Tracing e métricas** (via OpenTelemetry) rastreiam o ciclo de vida das requisições entre as camadas da aplicação e expõem tempo de resposta e taxa de erros, exportados para o console durante o desenvolvimento.

## ✅ Como Executar os Testes

O projeto possui dois projetos de teste, organizados por responsabilidade:

* **`VitalPetTech.Tests.Unit`**: testes unitários de entidades (validação de dados) e controllers (lógica de negócio, isolada via banco InMemory e mocking com Moq), cobrindo Tutor, Pet, EventoClinico e Alerta.
* **`VitalPetTech.Tests.Integration`**: testes de integração que sobem a API completa em memória (via `WebApplicationFactory`) e validam o fluxo HTTP real, incluindo respostas de sucesso e de erro.

### Rodando todos os testes

Na raiz da solução:

```bash
dotnet test
```

### Rodando apenas um projeto de teste

```bash
dotnet test VitalPetTech.Tests.Unit
dotnet test VitalPetTech.Tests.Integration
```

### Rodando um teste específico

```bash
dotnet test --filter "NomeDoMetodoDeTeste"
```

### Convenções adotadas

* Nomenclatura dos testes: `MetodoTestado_Cenario_ResultadoEsperado`
* Todos os testes seguem o padrão **AAA** (Arrange, Act, Assert)
* Testes de integração compartilham uma instância da API via **Collection Fixture** (`CustomWebApplicationFactory`), evitando subir a aplicação repetidamente a cada teste
* O banco de dados Oracle é substituído por **EF Core InMemory** durante os testes, isolando a suíte de testes de uma infraestrutura externa real