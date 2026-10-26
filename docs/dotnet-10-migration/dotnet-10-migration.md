# Migração do .NET 7 para o .NET 10

## Status

**Concluída.**

A solução BusTrack foi atualizada de .NET 7 para .NET 10. Esta documentação registra a migração como histórico técnico; os trabalhos descritos como futuros abaixo não representam pendências da V1.

## Objetivo

Atualizar a solução para .NET 10, mantendo as funcionalidades da aplicação e adequando código, dependências e testes à nova versão da plataforma.

## Versão anterior

```text
.NET 7
```

## Versão atual

```text
.NET 10
```

## Principais alterações

Os projetos .NET foram atualizados de `net7.0` para `net10.0`.

As dependências foram revisadas durante a migração. Entre as atualizações relevantes estão o MongoDB Driver e o AutoMapper.

Versões registradas durante a migração:

- MongoDB Driver: `3.11.1`;
- AutoMapper: `16.2.0`.

Dependências que deixaram de ser necessárias foram removidas.

## Ajustes no AutoMapper

A atualização do AutoMapper exigiu adequações no código e nos testes para compatibilidade com a versão utilizada.

## Testes

O projeto `BusTrack.Tests` também foi atualizado para .NET 10.

A solução possui testes unitários, de integração, performance e usabilidade.

## Nullable Reference Types

A migração também exigiu revisão de contratos relacionados a referências potencialmente nulas. Os ajustes necessários foram incorporados ao processo de evolução da aplicação.

## Plataforma atual

A solução utiliza:

```text
C#
.NET 10
ASP.NET Core
MongoDB
Angular
TypeScript
```

## Resultado

A migração principal foi concluída. A documentação não considera mais a atualização de .NET como uma tarefa pendente.

A eventual revisão futura de warnings, APIs obsoletas ou melhorias de qualidade de código deve ser tratada como manutenção técnica independente da migração.

## Histórico

### 2026-09-11

- Atualização dos projetos de .NET 7 para .NET 10.
- Atualização das principais dependências.
- Atualização do MongoDB Driver para 3.11.1.
- Atualização do AutoMapper para 16.2.0.
- Remoção de dependências que não eram mais necessárias.
- Ajustes nos testes para compatibilidade com as dependências atualizadas.

### 2026-09-16

- Revisão da compilação após a migração.
- Correção de incompatibilidades relacionadas à nulabilidade.
- Correção de incompatibilidade encontrada em teste unitário relacionado ao serviço de ônibus.
- Compilação finalizada sem erros.
