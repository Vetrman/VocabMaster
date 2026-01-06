// Перевод при наведении на слова
document.addEventListener('DOMContentLoaded', function () {
    // Вешаем обработчики на все слова
    document.querySelectorAll('.translatable-word').forEach(element => {
        element.addEventListener('mouseenter', async function () {
            const word = this.textContent;
            const fromLang = this.dataset.from || 'en';
            const toLang = this.dataset.to || 'ru';

            // Показываем подсказку
            const tooltip = document.createElement('div');
            tooltip.className = 'translation-tooltip';
            tooltip.textContent = 'Translating...';

            this.appendChild(tooltip);

            try {
                const response = await fetch('/WordCards/Translate', {
                    method: 'POST',
                    headers: { 'Content-Type': 'application/json' },
                    body: JSON.stringify({
                        text: word,
                        fromLang: fromLang,
                        toLang: toLang
                    })
                });

                if (response.ok) {
                    const translation = await response.text();
                    tooltip.textContent = translation;
                } else {
                    tooltip.textContent = 'Translation failed';
                }
            } catch {
                tooltip.textContent = 'Error';
            }
        });

        element.addEventListener('mouseleave', function () {
            const tooltip = this.querySelector('.translation-tooltip');
            if (tooltip) {
                tooltip.remove();
            }
        });
    });
});