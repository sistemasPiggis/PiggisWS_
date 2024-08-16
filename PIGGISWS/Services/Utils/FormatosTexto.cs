using System.Globalization;
using System.Text;

namespace PIGGISWS.Services.Utils;

public class FormatosTexto
{

    /// <summary>
    /// quita carateres y formatea texto.
    /// </summary>
    /// <param name="text"></param>
    /// <returns></returns>
    public static string RemoveDiacritics(string text)
    {
        var normalizedString = text.Normalize(NormalizationForm.FormD);
        var stringBuilder = new StringBuilder();

        foreach (var c in normalizedString)
        {
            var unicodeCategory = CharUnicodeInfo.GetUnicodeCategory(c);
            if (unicodeCategory != UnicodeCategory.NonSpacingMark)
            {
                stringBuilder.Append(c);
            }
        }

        return stringBuilder.ToString().Normalize(NormalizationForm.FormC);
    }


    public static string DatosNoEncontrados = "Datos No Encontrados En la Base de Datos";
    public static string DatosEncontrados = "Datos Encontrados Exitosamente";
}
