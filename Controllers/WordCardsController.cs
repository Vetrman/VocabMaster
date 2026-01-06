using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;
using System.Text.Json.Serialization;
using VocabMaster.Data;
using VocabMaster.Models;

namespace VocabMaster.Controllers
{
    [Authorize]
    public class WordCardsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _environment;

        public WordCardsController(ApplicationDbContext context, IWebHostEnvironment environment)
        {
            _context = context;
            _environment = environment;
        }

        // GET: WordCards
        public async Task<IActionResult> Index()
        {
            var wordCards = await _context.WordCards.ToListAsync();
            return View(wordCards);
        }

        // GET: WordCards/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: WordCards/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(WordCard wordCard)
        {
            if (ModelState.IsValid)
            {
                // Обработка загрузки картинки
                if (wordCard.ImageFile != null && wordCard.ImageFile.Length > 0)
                {
                    // Создаем папку uploads если нет
                    var uploadsFolder = Path.Combine(_environment.WebRootPath, "uploads");
                    if (!Directory.Exists(uploadsFolder))
                        Directory.CreateDirectory(uploadsFolder);

                    // Генерируем уникальное имя файла
                    var uniqueFileName = Guid.NewGuid().ToString() + "_" + wordCard.ImageFile.FileName;
                    var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                    // Сохраняем файл
                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await wordCard.ImageFile.CopyToAsync(stream);
                    }

                    // Сохраняем путь к файлу
                    wordCard.ImagePath = "/uploads/" + uniqueFileName;
                }

                wordCard.CreatedDate = DateTime.UtcNow;
                _context.Add(wordCard);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(wordCard);
        }

        // GET: WordCards/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var wordCard = await _context.WordCards
                .FirstOrDefaultAsync(m => m.Id == id);

            if (wordCard == null)
            {
                return NotFound();
            }

            return View(wordCard);
        }

        // POST: WordCards/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var wordCard = await _context.WordCards.FindAsync(id);
            if (wordCard != null)
            {
                // Удаляем файл картинки если есть
                if (!string.IsNullOrEmpty(wordCard.ImagePath))
                {
                    var filePath = Path.Combine(_environment.WebRootPath, wordCard.ImagePath.TrimStart('/'));
                    if (System.IO.File.Exists(filePath))
                    {
                        System.IO.File.Delete(filePath);
                    }
                }

                _context.WordCards.Remove(wordCard);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }

        // API для перевода
        [HttpPost]
        public async Task<IActionResult> Translate([FromBody] TranslateRequest request)
        {
            try
            {
                string translatedText = request.Text;

                // Пробуем первый переводчик
                translatedText = await TryTranslateLibre(request.Text, request.FromLang, request.ToLang);

                // Если не сработало, пробуем второй
                if (translatedText == request.Text)
                {
                    translatedText = await TryMyMemory(request.Text, request.FromLang, request.ToLang);
                }

                // Если оба не сработали - возвращаем исходный текст
                return Ok(translatedText);
            }
            catch (Exception ex)
            {
                // Если ошибка - возвращаем исходный текст
                Console.WriteLine($"Translation error: {ex.Message}");
                return Ok(request.Text);
            }
        }

        private async Task<string> TryTranslateLibre(string text, string fromLang, string toLang)
        {
            try
            {
                using var httpClient = new HttpClient();
                httpClient.Timeout = TimeSpan.FromSeconds(5);

                // Пробуем несколько эндпоинтов LibreTranslate
                var endpoints = new[]
                {
                    "https://libretranslate.com/translate",
                    "https://libretranslate.de/translate",
                    "https://translate.argosopentech.com/translate"
                };

                foreach (var endpoint in endpoints)
                {
                    try
                    {
                        var translateRequest = new
                        {
                            q = text,
                            source = fromLang,
                            target = toLang,
                            format = "text"
                        };

                        var response = await httpClient.PostAsJsonAsync(endpoint, translateRequest);

                        if (response.IsSuccessStatusCode)
                        {
                            var result = await response.Content.ReadFromJsonAsync<TranslationResponse>();
                            if (!string.IsNullOrEmpty(result?.TranslatedText))
                                return result.TranslatedText;
                        }
                    }
                    catch
                    {
                        continue; // Пробуем следующий endpoint
                    }
                }
            }
            catch
            {
                // Игнорируем ошибку
            }

            return text;
        }

        private async Task<string> TryMyMemory(string text, string fromLang, string toLang)
        {
            try
            {
                using var httpClient = new HttpClient();
                httpClient.Timeout = TimeSpan.FromSeconds(5);

                var url = $"https://api.mymemory.translated.net/get?q={Uri.EscapeDataString(text)}&langpair={fromLang}|{toLang}";
                var response = await httpClient.GetAsync(url);

                if (response.IsSuccessStatusCode)
                {
                    var result = await response.Content.ReadFromJsonAsync<MyMemoryResponse>();
                    if (!string.IsNullOrEmpty(result?.ResponseData?.TranslatedText))
                        return result.ResponseData.TranslatedText;
                }
            }
            catch
            {
                // Игнорируем ошибку
            }

            return text;
        }

        // Модели для перевода
        public class TranslateRequest
        {
            public string Text { get; set; } = "";
            public string FromLang { get; set; } = "en";
            public string ToLang { get; set; } = "ru";
        }

        public class TranslationResponse
        {
            [JsonPropertyName("translatedText")]
            public string? TranslatedText { get; set; }
        }

        public class MyMemoryResponse
        {
            [JsonPropertyName("responseData")]
            public ResponseData? ResponseData { get; set; }
        }

        public class ResponseData
        {
            [JsonPropertyName("translatedText")]
            public string? TranslatedText { get; set; }
        }
    }
}