using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace AgendaWPF.Converters
{
    public class BoolToOpacityConverter : IValueConverter
    {
        public object Convert(object v, Type t, object p, CultureInfo c)
            => (v is bool b && b) ? 1.0 : 0.0;
        public object ConvertBack(object v, Type t, object p, CultureInfo c)
            => (v is double d && d > 0.5);
    }
}
