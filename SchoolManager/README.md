# SchoolManager

Projeto desenvolvido em ASP.NET Core MVC no âmbito da UFCD 5417.

## Tema
Gestão Escolar.

## Tecnologias utilizadas
- ASP.NET Core MVC
- .NET 8
- Visual Studio 2022
- Git
- GitHub
- Entity Framework Core
- SQL Server
- ASP.NET Core Identity
- Bootstrap
- Repository Pattern

## Objetivo
Desenvolver uma aplicação Web para gestão escolar, permitindo gerir alunos, cursos, professores, disciplinas, inscrições e planos curriculares.

## Funcionalidades

- Registo e autenticação de utilizadores
- Login e logout
- Recuperação e alteração de palavra-passe
- Edição de perfil
- Autorização por roles
- Gestão de cursos
- Gestão de alunos
- Gestão de professores
- Gestão de disciplinas
- Gestão de inscrições
- Gestão do plano curricular
- Dashboard com estatísticas
- Pesquisa e filtros
- Validação de dados e conflitos

## Perfis

A aplicação possui os seguintes perfis:

- Admin
- Secretaria
- Professor
- Utilizador

O administrador possui acesso à área de administração.

## Entidades principais

- Course
- Student
- Teacher
- Discipline
- Enrollment
- CourseDiscipline
- User

## Relacionamentos

A aplicação possui relacionamentos entre as várias entidades.

Exemplos:

- Um curso pode ter vários alunos.
- Um professor pode ter várias disciplinas.
- Um aluno pode ter várias inscrições.
- Uma disciplina pode ter várias inscrições.
- Cursos e disciplinas possuem uma relação muitos-para-muitos através de CourseDiscipline.
- Alunos e disciplinas possuem uma relação muitos-para-muitos através de Enrollment.

## Arquitetura

Foi utilizado o Repository Pattern para separar o acesso aos dados da lógica dos controllers.

Fluxo simplificado:

Controller  
↓  
Repository  
↓  
DataContext  
↓  
SQL Server

## Base de dados

A aplicação utiliza SQL Server com Entity Framework Core.

As migrations podem ser aplicadas através do Package Manager Console:

```powershell
Update-Database


## Utilizador administrador

Durante a inicialização da aplicação é criado um utilizador administrador.

Email:

```text
admin@schoolmanager.pt

Password:

```text
Admin123!
```

## Executar o projeto
1. Abrir a solução no Visual Studio 2022.
2. Confirmar a connection string no appsettings.json.
3. Executar Update-Database, se necessário.
4. Fazer Build da solução.
5. Executar o projeto.

## Dashboard
O dashboard apresenta informação resumida da aplicação, incluindo:
- Número de alunos
- Número de cursos
- Número de professores
- Número de disciplinas
- Número de inscrições
- Inscrições mais recentes

## Validações
Foram implementadas validações para evitar situações como:
- Cursos com códigos repetidos
- Disciplinas com códigos repetidos
- Alunos com números ou emails repetidos
- Professores com números de funcionário ou emails repetidos
- Inscrição duplicada do mesmo aluno na mesma disciplina
- Associação duplicada de uma disciplina ao mesmo curso
- Eliminação de entidades que possuem dados relacionados

## Autor
Projeto desenvolvido no âmbito da formação de desenvolvimento de aplicações web com ASP.NET Core MVC.