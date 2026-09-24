# Migração do .NET 7 para o .NET 10

## Objetivo

Atualizar a solução Bus Track da versão .NET 7 para o .NET 10, mantendo as funcionalidades existentes e adequando as dependências e os testes à nova versão do framework.

A atualização foi realizada durante a refatoração do projeto em setembro de 2026.

## Versão anterior

A solução utilizava:

```text
.NET 7
```

## Versão atual

Os projetos .NET da solução foram atualizados para:

```text
.NET 10
```

A atualização abrangeu os projetos principais da solução e o projeto separado de testes.

## Principais alterações

### Atualização do Target Framework

Os projetos foram atualizados de:

```xml
<TargetFramework>net7.0</TargetFramework>
```

para:

```xml
<TargetFramework>net10.0</TargetFramework>
```

## Atualização das dependências

Durante a migração, as principais dependências utilizadas pela aplicação também foram revisadas e atualizadas.

Entre elas:

* MongoDB Driver;
* AutoMapper;
* dependências relacionadas ao ASP.NET Core;
* dependências utilizadas pelos testes.

O MongoDB Driver foi atualizado para a versão:

```text
3.11.1
```

O AutoMapper foi atualizado para a versão:

```text
16.2.0
```

## Remoção de dependências

Dependências que não eram mais necessárias após a atualização para o .NET 10 foram removidas.

A remoção teve como objetivo manter os projetos com apenas as dependências necessárias para seu funcionamento atual.

## Ajustes no AutoMapper

A atualização do AutoMapper exigiu ajustes no código e nos testes para adequação à versão atual da biblioteca.

Os testes que dependiam do comportamento anterior foram revisados para funcionar com a nova versão.

## Ajustes nos testes

O projeto `BusTrack.Tests` também foi atualizado para utilizar o .NET 10.

Foram realizados ajustes na infraestrutura e nos testes para manter a compatibilidade com as versões atualizadas das dependências.

Os testes incluem:

* testes unitários;
* testes de integração;
* testes de performance;
* testes de usabilidade.

## Nullable Reference Types

Durante a atualização também foram revisados problemas relacionados ao recurso de Nullable Reference Types do C#.

A compilação passou a identificar situações em que referências potencialmente nulas não estavam sendo tratadas de maneira explícita.

Esses avisos estão sendo tratados separadamente após a conclusão da migração principal.

## Compatibilidade do projeto

Após as alterações, a solução passou a utilizar:

```text
C#
.NET 10
ASP.NET Core
MongoDB
Angular
TypeScript
```

Os projetos .NET foram compilados com sucesso.

O comando utilizado para verificar a compilação foi:

```powershell
dotnet build
```

O resultado atual é:

```text
Build succeeded
```

A solução ainda apresenta alguns warnings de compilação que serão tratados posteriormente.

Esses warnings não impedem a compilação da solução.

## Verificação da compilação

A compilação foi validada executando:

```powershell
dotnet clean
dotnet build
```

O `dotnet clean` é utilizado para remover os artefatos de compilação anteriores.

Em seguida, o `dotnet build` restaura as dependências e recompila os projetos.

No estado atual:

```text
BusTrack.Frontend.Server  → sucesso
BusTrack                   → sucesso
BusTrack.Tests             → sucesso
```

Não existem erros de compilação.

Existem warnings pendentes relacionados principalmente a:

* Nullable Reference Types;
* métodos de testes;
* APIs obsoletas utilizadas pela infraestrutura de testes.

## Próximos ajustes

A migração para o .NET 10 foi concluída do ponto de vista da compilação.

Os próximos trabalhos são:

1. revisar os warnings restantes;
2. corrigir os problemas de nulabilidade;
3. revisar a infraestrutura dos testes;
4. substituir APIs obsoletas quando necessário;
5. executar novamente a suíte de testes;
6. confirmar que a aplicação continua funcionando após as correções.

O objetivo da próxima etapa é deixar a solução com:

```text
0 erros
0 warnings
```

sem remover funcionalidades apenas para eliminar mensagens do compilador.

## Histórico

### 2026-09-11

* Atualização dos projetos de .NET 7 para .NET 10.
* Atualização das principais dependências.
* Atualização do MongoDB Driver para 3.11.1.
* Atualização do AutoMapper para 16.2.0.
* Remoção de dependências que não eram mais necessárias.
* Ajustes nos testes para compatibilidade com o AutoMapper atualizado.

### 2026-09-16

* Revisão da compilação após a migração.
* Correção de incompatibilidades relacionadas à nulabilidade em interfaces e serviços.
* Correção de incompatibilidade encontrada em teste unitário relacionado ao serviço de ônibus.
* Compilação finalizada sem erros.
* Warnings restantes separados para tratamento posterior.
