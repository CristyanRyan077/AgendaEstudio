using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Imaging;

namespace AgendaWPF.Helpers
{
    public class SalvarImagem
    {
        public static string? SalvarImagemDoClipboardEmPastaApp()
        {
            if (!Clipboard.ContainsImage())
                return null;

            var img = Clipboard.GetImage();
            if (img == null)
                return null;

            // Pasta fixa da aplicação (pode ajustar)
            var baseFolder = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                "AgendaStudio",
                "LembretesImagens");

            Directory.CreateDirectory(baseFolder);

            var fileName = $"lembrete_{DateTime.Now:yyyyMMdd_HHmmss}_{Guid.NewGuid():N}.png";
            var fullPath = Path.Combine(baseFolder, fileName);

            using (var fs = new FileStream(fullPath, FileMode.Create, FileAccess.Write))
            {
                var encoder = new PngBitmapEncoder();
                encoder.Frames.Add(BitmapFrame.Create(img));
                encoder.Save(fs);
            }

            return fullPath;
        }
    }
}
