using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;

var url = "https://api.openai.com/v1/completions";
var pic_url = "https://api.openai.com/v1/images/generations";
var token = Environment.GetEnvironmentVariable("OPEN_AI_KEY");


var option = Environment.GetCommandLineArgs()[1];
var prompt = "";
var how_do_i = string.Join(' ', Environment.GetCommandLineArgs());

switch(option)
{
    case "limerick":
        var limerick = string.Join(' ', how_do_i.Split(' ')[2..]);
        prompt = $"Write a limerick about {limerick}";
    break;
    case "dirty-limerick":
        limerick = string.Join(' ', how_do_i.Split(' ')[2..]);
        prompt = $"Write a dirty limerick about {limerick}";
    break;
    case "code":
        var lang = how_do_i.Split(' ')[2];
        var text = string.Join(' ', how_do_i.Split(' ')[3..]);
        prompt = $"Convert this text to {lang} code: {text}";
    break;
    case "pic":
        prompt = string.Join(' ', how_do_i.Split(' ')[1..]);
        var success = await fetch_pic(prompt);
        Environment.ExitCode = (success ? 0 : -1);
        return;
    default:
        prompt = string.Join(' ', how_do_i.Split(' ')[1..]);
    break;
}

var json = JsonSerializer.Serialize(new {
    model = "text-davinci-003",
    prompt = prompt,
    temperature = 1,
    max_tokens = 400,
    top_p = 1, // default is 1
    //frequency_penalty = 0.2, // default is 0
    presence_penalty = 0 // default is 0
});

var request = new HttpRequestMessage(HttpMethod.Post, url);
request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
request.Content = new StringContent(json, Encoding.UTF8);
request.Content.Headers.ContentType = new MediaTypeHeaderValue("application/json");

using var httpClient = new HttpClient();
httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
var response = await httpClient.SendAsync(request);
response.EnsureSuccessStatusCode();

var body = await response.Content.ReadAsStringAsync();
var h = JsonSerializer.Deserialize<JsonNode>(body);

Console.WriteLine(h?["choices"]?.AsArray()?.First()?["text"]);


async Task<bool> fetch_pic(string prompt)
{
    var json = JsonSerializer.Serialize(new {
        prompt = prompt,
        n = 1,
        size = "1024x1024"
    });

    var request = new HttpRequestMessage(HttpMethod.Post, pic_url);
    request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
    request.Content = new StringContent(json, Encoding.UTF8);
    request.Content.Headers.ContentType = new MediaTypeHeaderValue("application/json");

    using var httpClient = new HttpClient();
    httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
    var response = await httpClient.SendAsync(request);
    if(!response.IsSuccessStatusCode)
    {
        Console.WriteLine("-----------ERROR----------------------");
        Console.WriteLine(await response.Content.ReadAsStringAsync());
        Console.WriteLine("--------------------------------------");
        return false;
    }

    response.EnsureSuccessStatusCode();

    var body = await response.Content.ReadAsStringAsync();
    var h = JsonSerializer.Deserialize<JsonNode>(body);
    var result = h?["data"]?.AsArray()?.First()?["url"];

    Environment.SetEnvironmentVariable("OAI_PIC_URL", result?.ToString());

    Console.WriteLine(result);

    return true;
}