
// BusTrack administrative password-reset utility.
// Requires the .NET 10 SDK. Run manually; this file is not part of the API startup.
// Windows/Linux: dotnet run ./BusTrack.Script/AdminPasswordReset.cs

using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;

const string GenericFailure =
    "A operação não foi concluída. Confira a API e os dados informados.";

Console.WriteLine("BusTrack — redefinição administrativa de senha");
Console.WriteLine(
    "Este utilitário solicita autorização à API; não acessa o MongoDB diretamente.\n");

var baseUrl = ReadRequired(
    "URL base da API (ex.: https://localhost:7000 ou https://bustrack-8yuy.onrender.com)")
    .TrimEnd('/');

if (!Uri.TryCreate(baseUrl, UriKind.Absolute, out var baseUri) ||
    !(baseUri.Scheme == Uri.UriSchemeHttps ||
      (baseUri.Scheme == Uri.UriSchemeHttp && baseUri.IsLoopback)))
{
    Console.Error.WriteLine(
        "Use uma URL HTTPS. HTTP só é permitido para localhost.");
    return 1;
}

var adminEmail = ReadRequired("E-mail do administrador");
var adminPassword = ReadSecret("Senha atual do administrador");
var targetEmail = ReadRequired(
    "E-mail da conta que terá a senha redefinida");
var newPassword = ReadSecret("Nova senha da conta");
var confirmPassword = ReadSecret("Confirme a nova senha");

if (!string.Equals(newPassword, confirmPassword, StringComparison.Ordinal))
{
    Console.Error.WriteLine("As senhas informadas não coincidem.");
    return 1;
}

Console.Write(
    "\nConfirma a redefinição para " + targetEmail +
    "? Digite SIM para continuar: ");

if (!string.Equals(Console.ReadLine()?.Trim(), "SIM",
        StringComparison.Ordinal))
{
    Console.WriteLine("Operação cancelada.");
    return 0;
}

using var http = new HttpClient
{
    BaseAddress = baseUri,
    Timeout = TimeSpan.FromSeconds(30)
};

var browserId = Guid.NewGuid().ToString("D");
http.DefaultRequestHeaders.Add("X-Browser-Id", browserId);

try
{
    using var loginResponse = await http.PostAsJsonAsync(
        "/AuthenticationControllerAPI/login",
        new
        {
            email = adminEmail,
            password = adminPassword
        });

    if (!loginResponse.IsSuccessStatusCode)
    {
        Console.Error.WriteLine(
            "Não foi possível autenticar o administrador. " +
            "Verifique as credenciais e o endereço da API.");
        return 1;
    }

    using var loginJson = await JsonDocument.ParseAsync(
        await loginResponse.Content.ReadAsStreamAsync());

    if (!loginJson.RootElement.TryGetProperty(
            "accessToken", out var tokenElement) ||
        string.IsNullOrWhiteSpace(tokenElement.GetString()))
    {
        Console.Error.WriteLine(GenericFailure);
        return 1;
    }

    http.DefaultRequestHeaders.Authorization =
        new AuthenticationHeaderValue(
            "Bearer", tokenElement.GetString());

    using var resetResponse = await http.PostAsJsonAsync(
        "/AuthenticationControllerAPI/admin-reset-password",
        new
        {
            adminCurrentPassword = adminPassword,
            targetEmail,
            newPassword,
            confirmPassword
        });

    if (!resetResponse.IsSuccessStatusCode)
    {
        Console.Error.WriteLine(
            $"A API recusou a operação (HTTP {(int)resetResponse.StatusCode}). " +
            GenericFailure);
        return 1;
    }

    using var resetJson = await JsonDocument.ParseAsync(
        await resetResponse.Content.ReadAsStreamAsync());

    if (!resetJson.RootElement.TryGetProperty(
            "success", out var success) ||
        success.ValueKind != JsonValueKind.True)
    {
        Console.Error.WriteLine(GenericFailure);
        return 1;
    }

    Console.WriteLine(
        "Senha redefinida com sucesso. " +
        "As sessões da conta foram encerradas.");

    return 0;
}
catch (HttpRequestException)
{
    Console.Error.WriteLine(
        "Não foi possível conectar à API. " +
        "Verifique a URL, a rede e o certificado HTTPS.");
    return 1;
}
catch (TaskCanceledException)
{
    Console.Error.WriteLine(
        "A API não respondeu dentro do tempo limite.");
    return 1;
}
catch (JsonException)
{
    Console.Error.WriteLine(GenericFailure);
    return 1;
}

static string ReadRequired(string prompt)
{
    while (true)
    {
        Console.Write(prompt + ": ");
        var value = Console.ReadLine()?.Trim();

        if (!string.IsNullOrWhiteSpace(value))
            return value;

        Console.WriteLine("O campo é obrigatório.");
    }
}

static string ReadSecret(string prompt)
{
    Console.Write(prompt + ": ");
    var value = new System.Text.StringBuilder();

    while (true)
    {
        var key = Console.ReadKey(intercept: true);

        if (key.Key == ConsoleKey.Enter)
        {
            Console.WriteLine();
            return value.ToString();
        }

        if (key.Key == ConsoleKey.Backspace)
        {
            if (value.Length > 0)
                value.Length--;

            continue;
        }

        if (!char.IsControl(key.KeyChar))
            value.Append(key.KeyChar);
    }
}
