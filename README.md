# AthenasAcademy - Certificate

## 1. Entendimento Geral do Projeto

O projeto **AthenasAcademy.Certificate** é uma solução completa para gerenciamento de certificados acadêmicos da fictícia *Athenas Academy*. Ele oferece uma API RESTful que atua como ponto de entrada para a criação e recuperação de certificados, Arquitetura orientada a eventos com integração em serviços de mensageria, armazenamento local com a possibilidade de expansão para a nuvem, além de testes automatizados para garantir a qualidade do software.

## 2. Tecnologias


### 2.1 Docker

Docker é utilizado para containerizar os diferentes serviços da aplicação, facilitando o desenvolvimento, a distribuição e a execução em diversos ambientes.

#### 2.1.1 Postgres

Postgres é o banco de dados utilizado pelo projeto para armazenar os dados de certificados e para armazenar processos dos eventos de forma relacional.

#### 2.1.2 MinIO

MinIO é um serviço de armazenamento de objetos compatível com S3 da AWS, utilizado para armazenar arquivos de certificados e outros documentos.

#### 2.1.3 RabbitMQ

RabbitMQ é uma plataforma de mensageria utilizada para comunicação assíncrona entre os serviços do projeto.

## 3. Wkhtmltopdf

Wkhtmltopdf é uma ferramenta de linha de comando utilizada para converter HTML em PDF, essencial para a geração de certificados em formato PDF.

### 3.1 Wkhtmltopdf - Linux

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

O projeto **AthenasAcademy.Certificate.Core** contém a lógica de negócios e os serviços principais utilizados pela aplicação.

### 4.2 EventBus

O projeto **AthenasAcademy.Certificate.EventBus** gerencia a comunicação entre os serviços através do RabbitMQ.

### 4.3 API

O projeto **AthenasAcademy.Certificate.API** expõe os endpoints RESTful para interação com os serviços de certificados.

### 4.4 Handling

O projeto **AthenasAcademy.Certificate.Handling** é responsável pelo tratamento de exceções e gerenciamento de erros da aplicação.

### 4.5 Domain

O projeto **AthenasAcademy.Certificate.Domain** contém as entidades e modelos de dados utilizados pela aplicação.

### 4.6 Test

O projeto **AthenasAcademy.Certificate.Test** contém os testes automatizados para garantir a qualidade e o correto funcionamento da aplicação.

## 5. Configuração

### 5.1 Docker Compose

O Docker Compose é utilizado para orquestrar os diferentes serviços em containers. Sua configuração está disponível no arquivo `./deploy/docker-compose.yml`.

### 5.2 MinIO


```yaml
  minio:
    image: minio/minio
    container_name: file-manager
    environment:
      MINIO_ROOT_USER: <YOUR_USERNAME>
      MINIO_ROOT_PASSWORD: <YOUR_PASSWORD>
      MINIO_HTTP_TRACE: "off"
    command: server /data --console-address ":9001"
    volumes:
      - minio:/data
    ports:
      - "9000:9000" # Client
      - "9001:9001" # UI
```

#### Detalhes da Configuração
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

```yaml
  postgres:
    image: postgres:latest
    container_name: db-certificate
    environment:
      POSTGRES_DB: dbcertificate
      POSTGRES_USER: <YOUR_USERNAME>
      POSTGRES_PASSWORD: <YOUR_PASSWORD>
    volumes:
      - postgres:/data
      - ./certificate-create.sql:/docker-entrypoint-initdb.d/certificate-create.sql
    ports:
      - "5433:5432" # Client
```

#### Detalhes da Configuração
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

```yaml
  rabbitmq:
    image: rabbitmq:3-management
    container_name: event-bus
    environment:
      RABBITMQ_DEFAULT_USER: <YOUR_USERNAME>
      RABBITMQ_DEFAULT_PASS: <YOUR_PASSWORD>
      RABBITMQ_LOAD_DEFINITIONS: /etc/rabbitmq/definitions.json
    volumes:
      - rabbitmq:/data
      - ./definitions.json:/etc/rabbitmq/definitions.json
      - ./rabbitmq.conf:/etc/rabbitmq/rabbitmq.conf
    command: 
      ["sh", "-c", "rabbitmq-plugins enable rabbitmq_management rabbitmq_shovel rabbitmq_shovel_management && rabbitmq-server"]
    ports:
      - "5672:5672" # Client
      - "15672:15672" # UI
```

#### Detalhes da Configuração
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
