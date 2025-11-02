using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace AgendaWPF.Converters
{
    public class MesAnoPtBrConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is DateTime date)
            {
                // 1. Define a cultura brasileira
                var cultureInfo = new CultureInfo("pt-BR");

                // 2. Gera a string no formato desejado (ex: "janeiro 2023")
                string formattedDate = date.ToString("MMMM yyyy", cultureInfo);

                // 3. Aplica o Title Case usando o TextInfo da cultura
                return cultureInfo.TextInfo.ToTitleCase(formattedDate); // Saída: "Janeiro 2023"
            }
            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
