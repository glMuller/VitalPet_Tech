# 🐾 VitalPet Tech API

## 📖 Descrição do Projeto
A **VitalPet Tech API** é um sistema back-end desenvolvido em C# com .NET e Entity Framework Core. O projeto tem como objetivo gerenciar a saúde e o monitoramento de pets. Ele permite o cadastro de tutores e seus animais, o registro de histórico médico (eventos clínicos como vacinas e consultas) e a recepção de alertas gerados por sensores IoT (monitoramento de batimento cardíaco, temperatura e localização do animal).

---

## 🛠️ Instruções de Instalação e Execução

### Pré-requisitos
* [.NET SDK](https://dotnet.microsoft.com/download) (Versão 8.0 ou superior)
* SQL Server (ou outro banco de dados relacional compatível configurado)
* Uma IDE de sua preferência (Visual Studio, VS Code, Rider)

### Passo a passo para rodar o projeto

1. **Clone o repositório:**
   git clone https://github.com/glMuller/VitalPet-Tech.git

2. **Configure o Banco de Dados:**
   Abra o arquivo `appsettings.json` e configure a sua string de conexão na chave `ConnectionStrings:ConexaoBanco` (RM561995 - 200107 apontando para o seu servidor local.

3. **Crie o Banco de Dados (Migrations):**
   Abra o terminal na raiz do projeto e execute o comando:
   `dotnet ef database update`

4. **Execute a aplicação:**
   No terminal, execute:
   `dotnet run`

5. **Acesse a documentação interativa:**
   Com a aplicação rodando, abra o navegador e acesse o Swagger para testar os endpoints: `http://localhost:porta/swagger`

---

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
* `GET /api/Alerta/pendentes`: Retorna **apenas** os alertas que não foram resolvidos.
* `POST /api/Alerta`: Registra um novo alerta no sistema.
* `PUT /api/Alerta/{id}`: Atualiza um alerta (marca como resolvido).
* `DELETE /api/Alerta/{id}`: Remove um alerta do histórico.
