# AthenasAcademy - Certificate

Projeto desenvolvido em c# com .net core 8, para atestar as abilidades dessas tecnoligias e conceitos de SOLID, Clean Code, técnicas de escalabilidade, performance de código e arquitetura de software.

O Projeto consiste é uma solução completa para gerenciamento de certificados acadêmicos da fictícia *Athenas Academy*. Ele oferece uma API RESTful que atua como ponto de entrada para a criação e recuperação de certificados, Arquitetura orientada a eventos com integração em serviços de mensageria, armazenamento local com a possibilidade de expansão para a nuvem, além de testes automatizados para garantir a qualidade do software.

## 1. Tecnologias

### 1.1 Docker

Docker é utilizado para containerizar os diferentes serviços da aplicação, facilitando o desenvolvimento, a distribuição e a execução em diversos ambientes.

#### 1.1.1 Postgres

Postgres é o banco de dados utilizado pelo projeto para armazenar os dados de certificados e para armazenar processos dos eventos de forma relacional.

#### 1.1.2 MinIO

MinIO é um serviço de armazenamento de objetos compatível com S3 da AWS, utilizado para armazenar arquivos de certificados e outros documentos.

#### 1.1.3 RabbitMQ

RabbitMQ é uma plataforma de mensageria utilizada para comunicação assíncrona entre os serviços do projeto.

## 2. Wkhtmltopdf

Wkhtmltopdf é uma ferramenta de linha de comando utilizada para converter HTML em PDF, essencial para a geração de certificados em formato PDF.

### 2.1 Wkhtmltopdf - Linux

Para instalar o Wkhtmltopdf no Linux, siga os passos abaixo:

```bash
sudo apt-get update
sudo apt-get install -y wkhtmltopdf
```

### 3.2 Wkhtmltopdf - Windows

Para instalar o Wkhtmltopdf no Windows, siga os passos abaixo:

1. Baixe o instalador em [Wkhtmltopdf Downloads](https://wkhtmltopdf.org/downloads.html).
2. Execute o instalador e siga as instruções na tela.

## 4. Projetos

### 4.1 Core

O projeto **AthenasAcademy.Certificate.Core** contém a lógica de negócios e os serviços principais utilizados pela aplicação, bem como os repositórios para acesso aos dados externos.

### 4.2 EventBus

O projeto **AthenasAcademy.Certificate.EventBus** é um componente abstrato para o aplicação da arquitetura orientada a eventos.
Nesse projeto o boker escolhido foi o RabbitMQ.

### 4.3 API

O projeto **AthenasAcademy.Certificate.API** expõe os endpoints RESTful para interação com os serviços de certificados. 
O mesmo já possui uma pré implementação para o uso de autenticação e autorização por meio de JWT.

### 4.4 Handling

O projeto **AthenasAcademy.Certificate.Handling** é responsável por orquestrar os eventos disponíveis no broker e manusealos da melhor forma possíve, utilizando gerenciadores de inscrição, programação paralela e trabalhos em segundo plano.

### 4.5 Domain

O projeto **AthenasAcademy.Certificate.Domain** contém as entidades e modelos de dados utilizados no dominio da aplicação, como por exemplo, contratos de requisição e resposta.

### 4.6 Test

O projeto **AthenasAcademy.Certificate.Test** contém os testes automatizados para garantir a qualidade e o correto funcionamento da aplicação.

## 5. Configuração

### 5.1 Docker Compose

O Docker Compose é utilizado para orquestrar os diferentes serviços em containers. Sua configuração está disponível no arquivo `./deploy/docker-compose.yml`.

### 5.2 MinIO

- **Imagem**: `minio/minio`
- **Nome do Contêiner**: `file-manager`
- **Variáveis de Ambiente**:
  - `MINIO_ROOT_USER`: Nome de usuário do MinIO.
  - `MINIO_ROOT_PASSWORD`: Senha do usuário do MinIO.
  - `MINIO_HTTP_TRACE`: Desativado para não registrar os rastros HTTP.
- **Comando**: Inicia o servidor MinIO com o endereço da console na porta `9001`.
- **Volumes**:
  - `minio:/data`: Volume para persistência de dados.
- **Portas**:
  - `9000:9000`: Porta para acesso ao cliente.
  - `9001:9001`: Porta para acesso à interface de usuário (console).

### 5.3 Postgres

- **Imagem**: `postgres:latest`
- **Nome do Contêiner**: `db-certificate`
- **Variáveis de Ambiente**:
  - `POSTGRES_DB`: Nome do banco de dados a ser criado.
  - `POSTGRES_USER`: Nome de usuário do PostgreSQL.
  - `POSTGRES_PASSWORD`: Senha do usuário do PostgreSQL.
- **Volumes**:
  - `postgres:/data`: Volume para persistência de dados.
  - `./certificate-create.sql:/docker-entrypoint-initdb.d/certificate-create.sql`: Script SQL para inicialização do banco de dados.
- **Portas**:
  - `5433:5432`: Porta para acesso ao cliente.

### 5.4 RabbitMQ

- **Imagem**: `rabbitmq:3-management`
- **Nome do Contêiner**: `event-bus`
- **Variáveis de Ambiente**:
  - `RABBITMQ_DEFAULT_USER`: Nome de usuário padrão do RabbitMQ.
  - `RABBITMQ_DEFAULT_PASS`: Senha do usuário padrão do RabbitMQ.
  - `RABBITMQ_LOAD_DEFINITIONS`: Caminho para o arquivo de definições.
- **Volumes**:
  - `rabbitmq:/data`: Volume para persistência de dados.
  - `./definitions.json:/etc/rabbitmq/definitions.json`: Arquivo de definições do RabbitMQ.
  - `./rabbitmq.conf:/etc/rabbitmq/rabbitmq.conf`: Arquivo de configuração do RabbitMQ.
- **Comando**: Habilita os plugins de gerenciamento, shovel e shovel management e inicia o servidor RabbitMQ.
- **Portas**:
  - `5672:5672`: Porta para comunicação entre clientes.
  - `15672:15672`: Porta para acesso à interface de gerenciamento.

### 5.5 Wkhtmltopdf - Linux

1. **Atualização e Instalação de Dependências:**

   ```bash
   apt-get update && apt-get install -y libgdiplus libc6-dev wkhtmltopdf xvfb
   ```

   - `libgdiplus`: Biblioteca para compatibilidade com GDI+ no Linux.
   - `libc6-dev`: Pacote de desenvolvimento do glibc, necessário para compilação.
   - `wkhtmltopdf`: Ferramenta de linha de comando para converter HTML em PDF.
   - `xvfb`: X Virtual Framebuffer, utilizado para simular uma tela gráfica.

2. **Configuração do Script de Execução:**

   ```bash
   printf '#!/bin/bash\nxvfb-run -a --server-args="-screen 0, 1024x768x24" /usr/bin/wkhtmltopdf -q $*' > /usr/bin/wkhtmltopdf.sh
   ```

   - Cria um script `wkhtmltopdf.sh` que executa `wkhtmltopdf` dentro do `xvfb-run`, garantindo que o processo tenha acesso a uma "tela" virtual.

3. **Permissões e Links Simbólicos:**

   ```bash
   chmod a+x /usr/bin/wkhtmltopdf.sh
   ln -s /usr/bin/wkhtmltopdf.sh /usr/local/bin/wkhtmltopdf
   ln -s /usr/lib/libgdiplus.so /usr/lib/gdiplus.dll
   ```

   - Define permissões de execução para o script `wkhtmltopdf.sh`.
   - Cria um link simbólico `/usr/local/bin/wkhtmltopdf` para o script, permitindo que seja acessível globalmente.
   - Cria um link simbólico `/usr/lib/gdiplus.dll` apontando para `/usr/lib/libgdiplus.so` para compatibilidade com algumas dependências.


## 6. Initialization

To start the project, follow these steps:

1. **Start the Docker Containers:**

   Ensure your Docker containers are up and running. Use the following command to start the containers defined in your Docker Compose file:

   ```bash
   docker-compose up -d
   ```

2. **Run the API Project:**

   Navigate to the API project directory and run the project using the following commands:

   ```bash
   cd ./AthenasAcademy.Certificate.API
   dotnet run
   ```

3. **Run the Handling Project:**

   Open a new terminal window, navigate to the Handling project directory, and run the project:

   ```bash
   cd path/to/AthenasAcademy.Certificate.Handling
   dotnet run
   ```

4. **Generate a Certificate:**

   Make a request to the `/api/v1/certificate/generate` endpoint with the following JSON payload:

   **Request:**

   ```json
   {
     "student": {
       "name": "John Doe",
       "registration": "123456",
       "document": {
         "type": "ID",
         "number": "987654321"
       }
     },
     "course": {
       "course": "Computer Science",
       "workload": 200,
       "disciplines": [
         {
           "discipline": "Algorithms",
           "workload": 50,
           "utilization": 90,
           "conclusion": "2024-07-17T01:26:08.149Z"
         },
         {
           "discipline": "Data Structures",
           "workload": 50,
           "utilization": 85,
           "conclusion": "2024-07-17T01:26:08.149Z"
         }
       ]
     },
     "utilization": 87,
     "conclusion": "2024-07-17T01:26:08.149Z"
   }
   ```

5. **Response:**

   The response will be in the following format:

   **Response:**

   ```json
   {
     "student": "John Doe",
     "course": "Computer Science",
     "workload": 200,
     "utilization": 87,
     "conclusion": "2024-07-17T01:27:20.537Z",
     "files": [
       {
         "name": "sign_hash.pdf",
         "download": "https://example.com/download/sign_hash.pdf",
         "type": ".pdf",
         "size": 123456
       }
     ]
   }
   ```

By following these steps, you will have your API and handling services running, allowing you to generate and retrieve academic certificates through the specified endpoint.

## 8. Parameters

The `Parameters` section in the secrets and configurations YAML file defines various settings that are essential for the application's operation. These settings include configurations for storage paths, template files, event handling, and driver locations. Each parameter should be appropriately set up to ensure the application functions correctly.

### Parameters Configuration

```yaml
Parameters:
  BucketName: certificateshare
  BucketPathPdf: pdf
  BucketPathQR: qrcode
  BucketKeyStamp: template/stamp.png
  BucketKeyLogo: template/logo.png
  BucketKeyTemplate: template/certificate_template.html
  EventMaxAttemps: 10
  EventMaxCallbacks: 10
  DriverDir: /usr/local/bin
```

### Parameters Explanation

- **BucketName**: The base bucket configuration. This bucket should be created before using the application.
- **BucketPathPdf**: The base path configuration for saving PDF files. This path should be created before using the application.
- **BucketPathQR**: The base path configuration for saving QR code files. This path should be created before using the application.
- **BucketKeyStamp**: Path to the certificate stamp. The file is located in the `./assets` folder of the project.
- **BucketKeyLogo**: Path to the certificate logo. The file is located in the `./assets` folder of the project.
- **BucketKeyTemplate**: Path to the certificate template. The file is located in the `./assets` folder of the project.
- **EventMaxAttemps**: Number of attempts the handling process will make to process an event.
- **EventMaxCallbacks**: Number of parallel processes to handle a subscriber.
- **DriverDir**: Location of the wkhtmltopdf driver if running on Linux.

### Parameters Binding

To bind these settings in your application, define a record in C#:

```csharp
public record Parameters
{
    public string BucketName { get; set; }
    public string BucketPathPdf { get; set; }
    public string BucketPathQR { get; set; }
    public string BucketKeyStamp { get; set; }
    public string BucketKeyLogo { get; set; }
    public string BucketKeyTemplate { get; set; }
    
    public int EventMaxAttemps { get; set; }
    public int EventMaxCallbacks { get; set; }
    
    public string DriverDir { get; set; }
}
```

### Implementation of use

```csharp
public static IServiceCollection ConfigureParameters(this IServiceCollection services, IConfiguration configuration)
{
    services.Configure<Parameters>(configuration.GetSection("Parameters"));
    return services;
}
```

### Usage Example

To use these parameters within your application, you can bind them in your `Startup` or `Program` class:

```csharp
builder.Services.ConfigureParameters(builder.Configuration);
```

This configuration ensures that all necessary parameters are loaded and available for your application to use, providing the flexibility and security needed to manage sensitive settings efficiently.