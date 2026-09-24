# Configuração geral

## Visão geral

A solução Bus Track é composta por diferentes projetos que trabalham em conjunto para formar a aplicação.

Para manter esses projetos alinhados, foram adotadas configurações comuns relacionadas à versão do .NET, organização da solução, imports automáticos e análise de referências nulas.

Essas configurações não representam funcionalidades diretamente visíveis para o usuário. Elas fazem parte da configuração técnica utilizada durante o desenvolvimento e compilação da aplicação.

---

## .NET 10

Os projetos .NET da solução foram atualizados de **.NET 7 para .NET 10**.

A atualização foi realizada para modernizar a base da aplicação e permitir que os projetos utilizem uma versão atual do ecossistema .NET.

A migração envolve não apenas alterar a versão indicada nos arquivos de projeto, mas também verificar a compatibilidade do código, das dependências e das configurações utilizadas pela solução.

A migração específica para .NET 10 possui documentação própria:

[Ver documentação da migração para .NET 10](../dotnet-10-migration.md)

---

## ImplicitUsings

Os projetos utilizam `ImplicitUsings`.

Essa configuração permite que determinados namespaces utilizados frequentemente pelo .NET sejam disponibilizados automaticamente durante a compilação.

Sem essa configuração, seria necessário declarar manualmente determinados `using` em diversos arquivos.

Isso reduz código repetitivo e deixa os arquivos mais focados nas dependências específicas de cada classe.

A configuração não elimina a possibilidade de declarar `using` manualmente quando um namespace específico é necessário.

---

## Nullable Reference Types

A solução utiliza Nullable Reference Types.

Esse recurso permite que o compilador diferencie referências que podem representar `null` daquelas que deveriam sempre possuir um valor.

Por exemplo, uma propriedade ou retorno declarado como:

```csharp
string?
```

indica que o valor pode ser `null`.

Enquanto:

```csharp
string
```

representa uma referência que, de acordo com o contrato do código, não deveria ser `null`.

Essa análise acontece durante a compilação e permite identificar possíveis problemas relacionados a referências nulas antes da execução da aplicação.

No Bus Track, essa configuração também aparece em retornos de métodos que podem não encontrar determinado registro.

Por exemplo:

```csharp
Task<DriverDTOAPI?>
```

indica que a operação pode retornar um motorista ou não encontrar nenhum registro correspondente.

Isso torna o contrato do método mais explícito para quem utiliza esse retorno.

---

## BusTrack.sln

A solução utiliza o arquivo:

```text
BusTrack.sln
```

O arquivo `.sln` funciona como o ponto de organização dos diferentes projetos que compõem a solução.

Ele permite que os projetos sejam tratados como partes de uma mesma solução de desenvolvimento.

Entre os projetos organizados pela solução estão:

* `BusTrack.API`;
* `BusTrack.DB`;
* `BusTrack.Program`;
* `BusTrack.Frontend`;
* `BusTrack.Tests`;
* `BusTrack.Updater`.

Essa organização facilita a compilação, execução e manutenção de uma aplicação que possui diferentes projetos relacionados.

---

## Configuração dos projetos

Cada projeto possui seu próprio arquivo de projeto e pode possuir configurações específicas relacionadas às suas responsabilidades.

A configuração geral da solução não significa que todos os projetos tenham exatamente as mesmas dependências.

Por exemplo, o projeto de testes possui dependências específicas para execução de testes, enquanto o backend possui dependências relacionadas à API e persistência.

A solução utiliza o arquivo `.sln` para reunir esses projetos, enquanto cada projeto mantém suas próprias configurações.

---

## Relação entre configuração e desenvolvimento

Essas configurações ajudam a manter um padrão comum durante o desenvolvimento.

A utilização de .NET 10 mantém os projetos na mesma geração da plataforma.

`ImplicitUsings` reduz código repetitivo.

Nullable Reference Types tornam os contratos relacionados a valores nulos mais explícitos.

O arquivo `BusTrack.sln` organiza os diferentes projetos como uma única solução.

Embora nenhuma dessas configurações represente diretamente uma funcionalidade apresentada ao usuário, elas influenciam a forma como o código é escrito, compilado, organizado e mantido.

---

## Resumo

A configuração geral do Bus Track estabelece uma base comum para os projetos da solução.

A migração para .NET 10 atualizou a plataforma utilizada pelos projetos.

`ImplicitUsings` reduz declarações repetitivas de namespaces.

Nullable Reference Types ajudam a identificar possíveis problemas relacionados a `null` durante a compilação.

O `BusTrack.sln` organiza os diferentes projetos que compõem a solução.

Essas configurações fazem parte da base técnica do projeto e complementam a arquitetura e as tecnologias utilizadas pela aplicação.
