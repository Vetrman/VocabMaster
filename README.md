# VocabMaster

A vocabulary learning app. Built for myself, might be useful for others.

## What it does?

- Translates words/sentences (LibreTranslate API)
- Saves translations as word cards
- Hover over any word in translation to see its individual translation
- Text-to-speech pronunciation (when it works)
- Image upload for word cards
- Basic user authentication

## Tech Stack

**Backend:**
- ASP.NET Core 8 MVC
- Entity Framework Core
- PostgreSQL

**Frontend:**
- Bootstrap 5
- Vanilla JavaScript
- HTML/CSS

**External Services:**
- LibreTranslate (translation)
- MyMemory (fallback translation)
- SpeechSynthesis API (text-to-speech)

## What's broken?

### 1. Translation
- Sometimes returns garbage (especially for simple words like "are")
- Slow response times
- No caching system

### 2. Pronunciation
- Speaks in wrong language
- Tries to pronounce English words with Russian voice
- Sometimes doesn't work at all

### 3. Text parsing
- Words get merged in long sentences
- Punctuation handling is buggy

### 4. Example sentences
- Shows Russian examples for English words
- Examples are generated, not from real sources

## What needs a proper API?

### Current translation APIs suck:
- **LibreTranslate**: Free but unstable, returns nonsense sometimes
- **MyMemory**: Backup but not much better

### Need a proper translation API:
1. **Google Translate API** - Best quality but paid
2. **DeepL API** - Excellent for European languages
3. **Microsoft Translator** - Decent free tier

### Also need:
1. **Example sentences API** - Tatoeba or similar for real examples
2. **Pronunciation API** - Forvo or similar for native pronunciations
3. **Dictionary API** - For word definitions and usage

## What needs to be fixed/added?

### High priority (needs fixing now):
1. Find a stable, free translation API
2. Fix English pronunciation
3. Fix sentence word splitting
4. Add proper error handling for API failures

### Medium priority (should be added):
5. Add spaced repetition (Leitner system)
6. Add learning statistics
7. Export/import word cards
8. Mobile responsive design

### Low priority (nice to have):
9. Group cards by topics
10. Vocabulary tests/quizzes
11. Multi-device sync
12. Dark mode

## How to run?

1. Install .NET 8 and PostgreSQL
2. Clone the repo
3. Create VocabMasterDB database
4. Edit appsettings.json (add your DB credentials)
5. `dotnet ef database update`
6. `dotnet run`

## Found a bug?

Create an Issue. Especially if:
- Translation returns garbage
- Pronunciation is in wrong language
- Words are merged together

Better yet - fix it and submit a PR. Code isn't perfect but it works.

## Security Note

This application is for **educational/personal use only**. While basic security measures are implemented (ASP.NET Core Identity, CSRF protection, parameterized queries), several areas need improvement for production use: file upload validation, input sanitization, and proper HTTPS configuration. Users should not store sensitive information and should deploy with appropriate security measures.

## License

MIT
