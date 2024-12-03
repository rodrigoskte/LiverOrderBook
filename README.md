# LiveOrderBook - Rodrigo Castagnaro

<details>
 <summary>Documentação do projeto LiveOrderBook</summary>

 #### Especialização em Arquitetura de Sistemas .NET com Azure: Live Order Book

# 0. Metadados

**Nome do Projeto:** LiveOrderBook

**Desenvolvedores do Projeto:**

| Profissional                        | Data          |  
| --------------------------------    | ------------- | 
| Rodrigo Castagnaro                  | 01-12-2024    |

**Tecnologias Utilizadas:**

| Tecnologia                               | Propósito                                                      |  
| -----------------------------------      | -------------------------------------------------------------- | 
| .NET 7                                   | API, Class Library, Blazor                                     |
| Microsoft SQL Server                     | Banco de Dados                                                 |
| xUnit, Bogus e NSubstitute               | Testes unitários/integrados                                    |
| Visual Studio, Rider e VS Code           | Desenvolvimento                                                |
| Azure Devops                             | Versionamento                                                  |

# 1. Desafio

O projeto consiste em desenvolver um projeto para seguinte demanda: Escreva um micro-serviço em netcore (net6 ou net7) utilizando as melhores práticas arquiteturais para 
esse tipo de aplicação e considerando que vai rodar em container Linux na cloud (AKS/EKS).

**Requisitos:**

- Requisitos Funcionais:
    - Conectar com o websocket de Order Book público da Bitstamp: WebSocket API v2 - Bitstamp
	  (olhar para a especificação Live Order Book e para o mecanismo de subscrição pública em 
	  Subscriptions > Public channels). Não é necessário criação de tokens ou qualquer outro 
	  cadastro. 
	- Manter a ingestão e tratamento dos dados de 2 instrumentos: BTC/USD e ETH/USD, persistindo os 
	  dados em alguma base NoSQL ou SQL para consultas.
	- A cada 5 segundos, printar na tela:
		- Maior e menor preço de cada ativo naquele momento.
		- Média de preço de cada ativo naquele momento
		- Média de preço de cada ativo acumulada nos últimos 5 segundos
		- Média de quantidade acumulada de cada ativo	  
	- Expor uma API de simulação de melhor preço:
		-	Deve ser informado a operação (compra ou venda), o instrumento e a quantidade 
			-	Calcular o melhor preço para a quantidade total, ou seja: 
		- 	Se foi solicitado uma compra de 100 BTCs: 
			- 	Ordenar todos os itens da coleção de “asks” do JSON contido na última atualização recebida em ordem crescente de preço. 
			- 	Calcular o valor correspondente a 100 BTCs varrendo essa coleção e multiplicando quantidade x valor até chegar a 100 BTC. Nesse caso 
				serão necessários, provavelmente, vários itens da coleção para cumprir a quantidade de 100 BTC dado que os itens isoladamente tendem a 
				ser de pequena quantidade. 
			-	Caso a quantidade seja atendida já com o primeiro item da coleção, somente retornar o preço de quantidade desejada x valor. 
		-	Se for solicitado uma venda, fazer a mesma operação, mas na coleção de “bids” ordenados em ordem decrescente de preço.
		-	O payload de retorno deve conter: 
			- Um ID único que represente essa cotação 
			- A coleção usada no cálculo (somente os itens que foram efetivamente utilizados) 
			- A quantidade solicitada 
			- O tipo de operação solicitado (compra/venda) 
			- O resultado do cálculo o Gravar a memória de cálculo no banco de dados 
			- A API de simulação não pode interferir na ingestão de dados sendo realizada 
			
- Requisitos Não Funcionais:
	- Conhecimento em CosmosDB 
	- Experiencia com Testes não functionais
	- Experiencia com Sistema escalável horizontalmente
	- Experiencia com Baixa latencia

# 2. Solução

Primeiramente, definido que iria usar um banco SQL, assim, um para gerenciamento para persistir os valores no banco.
Por fim, foi escolhido a abordagem via container, Docker.

## 2.1. Arquitetura Proposta

Para concretizar as ideias, foi utilizado a abordagem de DDD, de acordo com a seguinte arquitetura:
![](./res/arquiteturav1.png "Diagrama da Aplicação")

**Figura 1:** Arquitetura do LiveOrderBook

De acordo com a Figura 1, a arquitetura do LiveOrderBook é descrita pelos itens a seguir:

1. Criar as imagens [O Dockerfile para criação da imagem se encontra neste repositório](DockerfileApi).

2. Execução do docker-compose [O docker-compose para criação da imagem se encontra neste repositório](docker-compose).

3. Criação do banco e das tabelas 

4. O resultado da API é verificado pelo usuário via Swagger ou Postman.

5. O resultado do webapp é verificado pelo usuário via Navegador.

Por fim, apresentamos as entidades criadas, a partir do Migrations e Entity, para persistir as informações de consultas e de usuários. 

![](./res/Entidades.png "As entidades de LiveOrderBook")

**Figura 2:** As entidades criadas

## 2.2. Explicação dos Recursos

A seguir, definimos a função de cada recurso em nossa solução:

- BD SQL: **db-liveorderbook** - o BD em si, contendo as tabelas *LiveOrderBook*.

## 2.3. Código Desenvolvido

Para elucidar o código desenvolvido, fornecemos as informações a seguir, de cada pasta deste repositório.

Observação: na raiz deste repositório temos os Dockerfiles e a Solution, contendo: um projeto de API, Application, Domain, Infrastructure e BlazorWASM.

**Projeto API (LiveOrderBook.Presentation.API):**

- Contém os Controllers.

- Os endpoints fornecem as funcionalidades para CRUD da aplicação.

- A API é documentada com o Swagger.

**Projeto BlazorWebApp (LiveOrderBook.Presentation.BlazorWebApp):**

- Contém as telas de interface com usuário.

**Pasta Infraestrutura: (LiveOrderBook.Infrastructure)**

- Contém as configurações de BD.

- Mapeamento das tabelas de BD.

- Repository para consultas de BD.
  
- Possui os migrations das entidades para os BDs.

**Pasta Domain: (LiveOrderBook.Domain)**

- Contém as definições das tabelas de BD.

- Interfaces utilizada pelo sistema.  

**Pasta Application: (LiveOrderBook.Application)**

- Contém as constantes do sistema.

- Models/DTOs.

- Validadores. 

**Pasta res:** recursos usados por este documento.

**Outras pastas:** armazenam informações de configurações das IDEs utilizadas.

## 2.4. Dockefile Criado

Foi criado dois arquivos Dockerfile, necessário para que nossa aplicação rode por meio de um container.

[Dockerfile Api criado](DockerfileApi).
[Dockerfile Blazor WASM criado](DockerfileBlazor).

## 2.5. Docker Compose
Foi criado um arquivo docker-compose, necessário para que se execute a aplição completa.

[Docker-Compose criado](docker-compose).

## 2.6 Comandos utilizados
Comandos utilizado para desenvolvimento e publicação

**Criação das imagens**
- docker build -t liveorderbookblazor -f DockerfileBlazor .
- docker build -t liveorderbookapi -f DockerfileApi .

**Docker Compose**
- docker-compose down -v
- docker-compose up --build

**Migrations**
- dotnet ef migrations add InitialCreateDBSql --project LiveOrderBook.Infrastructure --context LiveOrderBook.Infrastructure.Context.LiveOrderBookApiDbContext
- dotnet ef database update --project LiveOrderBook.Infrastructure --startup-project LiveOrderBook.Presentation.API --context LiveOrderBook.Infrastructure.Context.LiveOrderBookApiDbContext

**Criação da imagem do SQL**
- docker run -v ~/docker --name sqlserver -e "ACCEPT_EULA=Y" -e SA_PASSWORD="QBk88ka(6>" -p 1433:1433 -d mcr.microsoft.com/mssql/server

**Dicas**
- Se for usar container separados sem compose:
- ConnectionString = "DefaultSqlConnection_dev": "Server=host.docker.internal;
- docker build -t liveorderbookapiv1 -f DockerfileApi .
- docker run -d --name liveorderbookapiv1 -p 5001:8080 -d liveorderbookapiv1

# 3. Conclusão

Este repositório apresenta uma solução para o websocket de Order Book público da Bitstamp, usando .NET com Entity, Blazor e tecnologia de containers.

# 4. Referências

1. [ASP.NET Core Blazor](https://learn.microsoft.com/en-us/aspnet/core/blazor/?view=aspnetcore-7.0)

2. [Docker](https://docs.docker.com/compose/intro/features-uses/)

3. [Explore  MudBlazor] (https://mudblazor.com/docs/overview)

 </details>