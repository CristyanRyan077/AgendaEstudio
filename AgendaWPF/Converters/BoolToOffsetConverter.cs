using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace AgendaWPF.Converters
{
    public class BoolToOffsetConverter : IValueConverter
    {
        public double Fechado { get; set; } = 600; // fora da tela
        public double Aberto { get; set; } = 0;
        public object Convert(object v, Type t, object p, CultureInfo c)
            => (v is bool b && b) ? Aberto : Fechado;
        public object ConvertBack(object v, Type t, object p, CultureInfo c)
            => (v is double d) ? Math.Abs(d - Aberto) < 0.5 : false;
    }
}
