using MauiAppTempoAgora.Models;
using Newtonsoft.Json.Linq;
using System.Net;

namespace MauiAppTempoAgora.Services
{
    public class DataService
    {
        public static async Task<Tempo?> GetPrevisao(string cidade)
        {
            string chave = "a67cc15b7a82f79dc176c7279a0b9f9e";

            string url = $"https://api.openweathermap.org/data/2.5/weather?" +
                         $"q={cidade}&units=metric&appid={chave}&lang=pt_br";

            using (HttpClient client = new HttpClient())
            {
                HttpResponseMessage resp;

                try
                {
                    resp = await client.GetAsync(url);
                }
                catch (HttpRequestException)
                {
                    throw new Exception("Sem conexão com a internet. Verifique sua conexão e tente novamente.");
                }

                // Cidade não encontrada
                if (resp.StatusCode == HttpStatusCode.NotFound)
                {
                    throw new Exception("Cidade não encontrada. Verifique o nome informado.");
                }

                // Outros erros da API
                if (!resp.IsSuccessStatusCode)
                {
                    throw new Exception($"Erro ao consultar o clima. Código: {(int)resp.StatusCode}");
                }

                string json = await resp.Content.ReadAsStringAsync();

                var rascunho = JObject.Parse(json);

                Tempo t = new Tempo()
                {
                    lat = (double)rascunho["coord"]["lat"],
                    lon = (double)rascunho["coord"]["lon"],

                    sunrise = (int)(long)rascunho["sys"]["sunrise"],
                    sunset = (int)(long)rascunho["sys"]["sunset"],

                    temp_max = (double)rascunho["main"]["temp_max"],
                    temp_min = (double)rascunho["main"]["temp_min"],

                    // Novos dados
                    description = (string)rascunho["weather"][0]["description"],
                    speed = (double)rascunho["wind"]["speed"],
                    visibility = (int)rascunho["visibility"]
                };

                return t;
            }
        }
    }
}
