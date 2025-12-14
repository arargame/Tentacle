namespace ConsoleAppForTesting
{
    internal class Program
    {
        static async Task Main()
        {
            try
            {
                using var client = new HttpClient();

                var url = "http://localhost:5132/api/request/gethello"; // HTTP portu
                var response = await client.GetAsync(url);

                response.EnsureSuccessStatusCode();

                var content = await response.Content.ReadAsStringAsync();
                Console.WriteLine("Response:");
                Console.WriteLine(content);
            }
            catch (HttpRequestException ex)
            {
                Console.WriteLine("HTTP Hatası: " + ex.Message);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Genel Hata: " + ex.Message);
            }

            Console.WriteLine("İşlem tamamlandı.");
        }
    }

    }
