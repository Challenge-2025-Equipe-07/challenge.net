# 🐾 Veti — Sistema de Acompanhamento Clínico de Pets

API RESTful desenvolvida em ASP.NET Core para gerenciamento de consultas, exames e tratamentos veterinários. O sistema permite que tutores acompanhem o histórico clínico de seus pets, enquanto veterinários cadastram consultas, exames e tratamentos com medicamentos associados.

**Novidades da Sprint 2:** a aplicação agora conta com camadas de monitoramento e observabilidade (Health Checks, logging estruturado e tracing/métricas), além de testes automatizados (unitários e de integração) cobrindo as regras de negócio e os endpoints da API.

---

## 🛠️ Tecnologias Utilizadas

- **ASP.NET Core** — Framework para construção da API
- **Entity Framework Core** — ORM para mapeamento objeto-relacional
- **Oracle Database** — Banco de dados relacional
- **Scalar** — Documentação interativa da API
- **C#** — Linguagem de programação
- **Serilog** — Logging estruturado
- **OpenTelemetry** — Tracing distribuído e métricas de desempenho
- **xUnit** — Framework de testes automatizados
- **Moq** — Criação de mocks para testes unitários

---

## 📦 Pacotes NuGet

| Pacote | Versão |
|--------|--------|
| Microsoft.AspNetCore.OpenApi | 10.0.8 |
| Microsoft.EntityFrameworkCore | 10.0.8 |
| Microsoft.EntityFrameworkCore.Design | 10.0.8 |
| Microsoft.EntityFrameworkCore.Tools | 10.0.8 |
| Microsoft.OpenApi | 2.7.5 |
| Oracle.EntityFrameworkCore | 10.23.26200 |
| Scalar.AspNetCore | 2.14.14 |
| Serilog.AspNetCore | 10.0.0 |
| Serilog.Sinks.Console | 6.1.1 |
| Serilog.Sinks.File | 7.0.0 |
| OpenTelemetry.Extensions.Hosting | 1.18.0 |
| OpenTelemetry.Instrumentation.AspNetCore | 1.18.0 |
| OpenTelemetry.Exporter.Console | 1.18.0 |



---

## 🗄️ Estrutura do Banco de Dados

```
TB_TUTOR
TB_VETERINARIO
TB_PET (FK → TB_TUTOR)
TB_CONSULTA (FK → TB_PET, TB_VETERINARIO)
TB_EXAME (FK → TB_CONSULTA)
TB_MEDICAMENTO
TB_EXAME_MEDICAMENTO (FK → TB_EXAME, TB_MEDICAMENTO)
TB_TRATAMENTO (FK → TB_PET)
TB_TRATAMENTO_MEDICAMENTO (FK → TB_TRATAMENTO, TB_MEDICAMENTO)
```

---

## ⚙️ Instalação e Execução

### Pré-requisitos

- Visual Studio 2022 ou superior
- .NET 10 SDK
- Acesso ao Oracle Database (FIAP)

### Passo a passo

**1. Clone o repositório**
```bash
git clone https://github.com/Challenge-2025-Equipe-07/challenge.net.git
cd VetiWebApplication
```

**2. Configure a connection string**

Abra o arquivo `appsettings.json` e preencha com suas credenciais Oracle:
```json
{
  "ConnectionStrings": {
    "OracleConnection": "User Id=SEU_USUARIO;Password=SUA_SENHA;Data Source=oracle.fiap.com.br:1521/ORCL;"
  }
}
```

**3. Restaure os pacotes NuGet**

No terminal, na pasta da solução:
```powershell
dotnet restore
```

**4. Crie as tabelas no banco**

No Package Manager Console, com o projeto 
`VetiWebApplication` selecionado:
```powershell
Add-Migration InitialCreate
Update-Database
```

**5. Execute o projeto**

Pressione `F5` no Visual Studio ou execute:
```bash
dotnet run --project VetiWebApplication
```

**6. Acesse a documentação**

```
https://localhost:{porta}/scalar/v1
```
---

## 📊 Monitoramento e Observabilidade

### Health Check

A API expõe um endpoint de verificação de saúde que testa tanto a disponibilidade da aplicação quanto a conectividade real com o banco Oracle: 

GET /health


**Respostas possíveis:**
- `Healthy` (200 OK) — API funcionando e conexão com o Oracle bem-sucedida.
- `Unhealthy` (503 Service Unavailable) — a API está de pé, mas não conseguiu se conectar ao Oracle (ex: banco fora do ar, credenciais inválidas, rede indisponível).

A verificação é feita por uma implementação customizada (`OracleHealthCheck`, em `HealthChecks/`), usando apenas o framework nativo `Microsoft.Extensions.Diagnostics.HealthChecks`. Ela tenta abrir uma conexão real com o banco (`Database.CanConnectAsync()`) a cada chamada ao endpoint.

**Como monitorar manualmente:**
```bash
curl -i https://localhost:{porta}/health
```
A flag `-i` mostra o código de status HTTP retornado, junto com o corpo da resposta.

### Logging Estruturado (Serilog)

Todos os eventos da aplicação são registrados de forma estruturada, com os seguintes níveis: `Information`, `Warning` e `Error`.

- **Console**: os logs aparecem em tempo real no terminal ao rodar a aplicação.
- **Arquivo**: os logs também são salvos na pasta `Logs/`, na raiz do projeto, em arquivos diários (ex: `Logs/log-20260910.txt`).
- **Correlação de requisições**: cada requisição HTTP é logada com método, rota, status code e tempo de resposta, através do middleware `UseSerilogRequestLogging()`, permitindo rastrear o ciclo completo de uma chamada.

Exemplo de log de uma requisição:

[16:30:42 INF] Request starting HTTP/2 GET https://localhost:7232/api/tutor - null null
[16:30:44 INF] Executed DbCommand (97ms) [...] SELECT "t"."Id", "t"."DsCpf", ... FROM "TB_TUTOR" "t"
[16:30:45 INF] HTTP GET /api/tutor responded 200 in 2194.0829 ms

### Tracing e Métricas (OpenTelemetry)

A aplicação usa OpenTelemetry para:
- **Tracing**: rastrear cada requisição HTTP recebida, com duração e status code, exibido no console como blocos `Activity`.
- **Métricas**: medir tempo de resposta (histograma por rota) e contagem de requisições, permitindo calcular taxa de erros (proporção de status 4xx/5xx sobre o total). Exibido no console como blocos `Metric Name`.

Por padrão, tracing e métricas são exportados para o console (`AddConsoleExporter()`), útil para desenvolvimento e demonstração. Em um ambiente de produção, esses dados seriam enviados para uma ferramenta especializada (ex: Grafana, Jaeger, Application Insights).

---

## 🧪 Testes Automatizados

O projeto conta com dois projetos de teste, organizados por camada:

- **`VetiWebApplication.UnitTests`** — testa a camada de regra de negócio (Services), usando xUnit + Moq. As dependências (repositórios) são mockadas, sem tocar em banco de dados real.
- **`VetiWebApplication.IntegrationTests`** — testa os endpoints da API de ponta a ponta, usando `WebApplicationFactory`. Os repositórios Oracle são substituídos por versões em memória (via `CustomWebApplicationFactory`), evitando dependência de conexão real com o banco durante os testes.

Todos os testes seguem o padrão **AAA** (Arrange, Act, Assert) e a nomenclatura `MetodoTestado_Cenario_ResultadoEsperado`.

Os testes de integração usam uma **Collection Fixture** (`[Collection("Veti API")]`), permitindo que classes de teste de diferentes Controllers compartilhem a mesma instância da API e os mesmos dados em memória durante a execução — por exemplo, um Pet criado nos testes de `PetsController` pode ser referenciado pelos testes de `ConsultasController`.

### Como executar os testes

**Rodar todos os testes da solução:**
```bash
dotnet test
```

**Rodar apenas os testes unitários:**
```bash
dotnet test VetiWebApplication.UnitTests
```

**Rodar apenas os testes de integração:**
```bash
dotnet test VetiWebApplication.IntegrationTests
```

**Rodar com detalhes de cada teste:**
```bash
dotnet test --logger "console;verbosity=detailed"
```

Alternativamente, pelo Visual Studio: **Teste → Executar Todos os Testes** (ou pelo painel Gerenciador de Testes).

---

## 📋 Documentação das Rotas

### 👤 Tutor — `/api/tutor`

| Método | Rota | Descrição |
|--------|------|-----------|
| GET | `/api/tutor` | Lista todos os tutores |
| GET | `/api/tutor/{id}` | Busca tutor por ID com seus pets |
| GET | `/api/tutor/cpf/{cpf}` | Busca tutor por CPF |
| POST | `/api/tutor` | Cadastra novo tutor |
| PUT | `/api/tutor/{id}` | Atualiza dados do tutor |
| DELETE | `/api/tutor/{id}` | Remove tutor |

**Exemplo POST /api/tutor:**
```json
{
  "nmTutor": "Laura Lopes",
  "dsCpf": "12345678901",
  "dsEmail": "laura@email.com",
  "dsTelefone": "11999999999"
}
```

---

### 🐶 Pet — `/api/pet`

| Método | Rota | Descrição |
|--------|------|-----------|
| GET | `/api/pet` | Lista todos os pets |
| GET | `/api/pet/{id}` | Busca pet por ID com tutor |
| GET | `/api/pet/tutor/{tutorId}` | Lista pets de um tutor |
| GET | `/api/pet/especie/{especie}` | Filtra pets por espécie |
| POST | `/api/pet` | Cadastra novo pet |
| PUT | `/api/pet/{id}` | Atualiza dados do pet |
| DELETE | `/api/pet/{id}` | Remove pet |

**Exemplo POST /api/pet:**
```json
{
  "nmPet": "Sauro",
  "dsEspecie": "Gato",
  "dsRaca": "SRD",
  "nrIdade": 6,
  "stCastrado": 1,
  "tutorId": 1
}
```
> stCastrado: 1 = castrado, 0 = não castrado

---

### 🩺 Veterinário — `/api/veterinario`

| Método | Rota | Descrição |
|--------|------|-----------|
| GET | `/api/veterinario` | Lista todos os veterinários |
| GET | `/api/veterinario/{id}` | Busca veterinário por ID |
| POST | `/api/veterinario` | Cadastra novo veterinário |
| PUT | `/api/veterinario/{id}` | Atualiza dados do veterinário |
| DELETE | `/api/veterinario/{id}` | Remove veterinário |

**Exemplo POST /api/veterinario:**
```json
{
  "dsEmail": "carlos@veti.com",
  "dsPassword": "senha123"
}
```

---

### 📅 Consulta — `/api/consulta`

| Método | Rota | Descrição |
|--------|------|-----------|
| GET | `/api/consulta` | Lista todas as consultas |
| GET | `/api/consulta/{id}` | Busca consulta por ID |
| GET | `/api/consulta/pet/{petId}` | Lista consultas de um pet |
| POST | `/api/consulta` | Registra nova consulta |
| PUT | `/api/consulta/{id}` | Atualiza consulta |
| DELETE | `/api/consulta/{id}` | Remove consulta |

**Exemplo POST /api/consulta:**
```json
{
  "dtConsulta": "2026-05-24",
  "tpEvento": "Consulta de rotina",
  "notificar": 1,
  "petId": 1,
  "veterinarioId": 1
}
```
> tpEvento: Consulta de rotina, Consulta de Emergência, Retorno, Medicação
> notificar: 1 = sim, 0 = não

---

### 🔬 Exame — `/api/exames`

| Método | Rota | Descrição |
|--------|------|-----------|
| GET | `/api/exames` | Lista todos os exames |
| GET | `/api/exames/{id}` | Busca exame por ID |
| GET | `/api/exames/tutor/{tutorId}` | Lista exames de um tutor |
| GET | `/api/exames/pet/{petId}` | Lista exames de um pet |
| GET | `/api/exames/consulta/{consultaId}` | Lista exames de uma consulta |
| POST | `/api/exames` | Cadastra novo exame |
| PUT | `/api/exames/{id}` | Atualiza exame |
| DELETE | `/api/exames/{id}` | Remove exame |

**Exemplo POST /api/exames:**
```json
{
  "dsDocumento": "hemograma.pdf",
  "dtRealizacao": "2026-05-24",
  "dsDiagnostico": "Anemia leve",
  "consultaId": 1
}
```

---

### 💊 Medicamento — `/api/medicamento`

| Método | Rota | Descrição |
|--------|------|-----------|
| GET | `/api/medicamento` | Lista todos os medicamentos |
| GET | `/api/medicamento/{id}` | Busca medicamento por ID |
| POST | `/api/medicamento` | Cadastra novo medicamento |
| PUT | `/api/medicamento/{id}` | Atualiza medicamento |
| DELETE | `/api/medicamento/{id}` | Remove medicamento |

**Exemplo POST /api/medicamento:**
```json
{
  "nmMedicamento": "Amoxicilina",
  "dsDosagem": "500mg",
  "dsFrequencia": "A cada 8 horas"
}
```

---

### 🔗 Exame-Medicamento — `/api/exame-medicamento`

| Método | Rota | Descrição |
|--------|------|-----------|
| GET | `/api/exame-medicamento` | Lista todas as relações exame-medicamento |
| GET | `/api/exame-medicamento/exame/{exameId}` | Lista medicamentos de um exame |
| GET | `/api/exame-medicamento/medicamento/{medicamentoId}` | Lista exames de um medicamento |
| POST | `/api/exame-medicamento` | Vincula medicamento a um exame |
| DELETE | `/api/exame-medicamento/{exameId}/{medicamentoId}` | Remove vínculo |

**Exemplo POST /api/exame-medicamento:**
```json
{
  "exameId": 1,
  "medicamentoId": 1,
  "qtMedicamento": 2
}
```

---

### 💉 Tratamento — `/api/tratamento`

| Método | Rota | Descrição |
|--------|------|-----------|
| GET | `/api/tratamento` | Lista todos os tratamentos |
| GET | `/api/tratamento/{id}` | Busca tratamento por ID com medicamentos |
| GET | `/api/tratamento/pet/{petId}` | Lista tratamentos de um pet |
| POST | `/api/tratamento` | Cadastra novo tratamento |
| POST | `/api/tratamento/{tratamentoId}/medicamento` | Vincula medicamento ao tratamento |
| PUT | `/api/tratamento/{id}` | Atualiza tratamento |
| DELETE | `/api/tratamento/{id}` | Remove tratamento |

**Exemplo POST /api/tratamento:**
```json
{
  "dsDiagnostico": "Infecção bacteriana",
  "dtInicio": "2026-05-24",
  "dtRetornoPrevisto": "2026-06-07",
  "dsObservacao": "Manter em repouso",
  "petId": 1
}
```

**Exemplo POST /api/tratamento/{tratamentoId}/medicamento:**
```json
{
  "medicamentoId": 1,
  "qtMedicamento": 2,
  "dsInstrucao": "Administrar após refeição"
}
```

---

## 🗂️ Estrutura do Projeto

```
VetiWebApplication/
├── Controllers/
│   ├── TutoresController.cs
│   ├── VeterinariosController.cs
│   ├── PetsController.cs
│   ├── ConsultasController.cs
│   ├── ExamesController.cs
│   ├── MedicamentosController.cs
│   ├── ExameMedicamentosController.cs
│   └── TratamentosController.cs
├── Services/
│ ├── TutorService.cs
│ ├── VeterinarioService.cs
│ ├── PetService.cs
│ ├── ConsultaService.cs
│ ├── ExameService.cs
│ ├── MedicamentoService.cs
│ ├── ExameMedicamentoService.cs
│ └── TratamentoService.cs
├── Interfaces/
│ ├── ITutorRepository.cs
│ ├── IVeterinarioRepository.cs
│ ├── IPetRepository.cs
│ ├── IConsultaRepository.cs
│ ├── IExameRepository.cs
│ ├── IMedicamentoRepository.cs
│ ├── IExameMedicamentoRepository.cs
│ ├── ITratamentoRepository.cs
│ └── ITratamentoMedicamentoRepository.cs
├── Repositories/
│ ├── TutorRepository.cs
│ ├── VeterinarioRepository.cs
│ ├── PetRepository.cs
│ ├── ConsultaRepository.cs
│ ├── ExameRepository.cs
│ ├── MedicamentoRepository.cs
│ ├── ExameMedicamentoRepository.cs
│ ├── TratamentoRepository.cs
│ ├── TratamentoMedicamentoRepository.cs
│ └── EmMemoria/
│ ├── TutorRepositoryEmMemoria.cs
│ ├── VeterinarioRepositoryEmMemoria.cs
│ ├── PetRepositoryEmMemoria.cs
│ ├── ConsultaRepositoryEmMemoria.cs
│ ├── ExameRepositoryEmMemoria.cs
│ ├── MedicamentoRepositoryEmMemoria.cs
│ ├── ExameMedicamentoRepositoryEmMemoria.cs
│ ├── TratamentoRepositoryEmMemoria.cs
│ └── TratamentoMedicamentoRepositoryEmMemoria.cs
├── HealthChecks/
│ └── OracleHealthCheck.cs
├── Data/
│   └── AppDbContext.cs
├── Models/
│   ├── Tutor.cs
│   ├── Veterinario.cs
│   ├── Pet.cs
│   ├── Consulta.cs
│   ├── Exame.cs
│   ├── Medicamento.cs
│   ├── ExameMedicamento.cs
│   ├── Tratamento.cs
│   ├── TratamentoMedicamento.cs
│   └── Requests/
│   ├── TutorRequest.cs
│   ├── VeterinarioRequest.cs
│   ├── PetRequest.cs
│   ├── ConsultaRequest.cs
│   ├── ExameRequest.cs
│   ├── MedicamentoRequest.cs
│   ├── TratamentoRequest.cs
│   ├── TratamentoMedicamentoRequest.cs
│   └── ExameMedicamentoRequest.cs
├── Migrations/
├── Logs/ (gerado em tempo de execução pelo Serilog)
├── appsettings.json
└── Program.cs

VetiWebApplication.IntegrationTests/
├── CustomWebApplicationFactory.cs
├── VetiApiCollection.cs
├── TutoresControllerTests.cs
├── PetsControllerTests.cs
├── VeterinariosControllerTests.cs
├── ConsultasControllerTests.cs
├── ExamesControllerTests.cs
├── MedicamentosControllerTests.cs
├── ExameMedicamentosControllerTests.cs
└── TratamentosControllerTests.cs

VetiWebApplication.UnitTests/
├── TutorServiceTests.cs
├── PetServiceTests.cs
├── VeterinarioServiceTests.cs
├── ConsultaServiceTests.cs
├── ExameServiceTests.cs
├── MedicamentoServiceTests.cs
├── ExameMedicamentoServiceTests.cs
└── TratamentoServiceTests.cs
```

---
