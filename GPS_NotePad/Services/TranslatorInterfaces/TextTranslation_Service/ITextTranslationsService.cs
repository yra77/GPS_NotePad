
using System.Threading.Tasks;

namespace GPS_NotePad.Services.TranslatorInterfaces.TextTranslation_Service
{
    public interface ITextTranslationsService
    {
        Task<string> TranslateTextAsync(string text, string languageFrom, string languageTo);
    }
}
