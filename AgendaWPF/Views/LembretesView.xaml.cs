using AgendaWPF.Helpers;
using AgendaWPF.ViewModels;
using DocumentFormat.OpenXml.Office2013.Drawing.ChartStyle;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace AgendaWPF.Views
{
    /// <summary>
    /// Interação lógica para LembretesView.xam
    /// </summary>
    public partial class LembretesView : UserControl
    {
        private readonly LembretesViewModel _vm;
        public LembretesView(LembretesViewModel vm)
        {
            InitializeComponent();

            _vm = vm;
            DataContext = _vm;
            DropZone.PreviewDragOver += DropZone_PreviewDragOver;
            DropZone.Drop += DropZone_Drop;
            DropZone.PreviewKeyDown += DropZone_PreviewKeyDown;
            DropZone.MouseDown += (_, __) => DropZone.Focus();
            Loaded += async (_, _) => await _vm.CarregarAsync();
        }
        private void DropZone_PreviewDragOver(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
                e.Effects = DragDropEffects.Copy;
            else
                e.Effects = DragDropEffects.None;

            e.Handled = true;
        }

        private void DropZone_Drop(object sender, DragEventArgs e)
        {
            if (DataContext is not LembretesViewModel vm ||
                vm.LembreteEmEdicao is null)
                return;

            // 1) Se for arquivo, usa o arquivo
            if (e.Data.GetData(DataFormats.FileDrop) is string[] files &&
                files.Length > 0 &&
                File.Exists(files[0]))
            {
                var file = files[0];

                // aqui você pode escolher:
                // a) usar o path direto
                // b) copiar pra pasta da aplicação (eu recomendo b)
                var baseFolder = System.IO.Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                    "AgendaStudio",
                    "LembretesImagens");
                Directory.CreateDirectory(baseFolder);

                var dest = System.IO.Path.Combine(baseFolder, System.IO.Path.GetFileName(file));
                File.Copy(file, dest, overwrite: true);

                vm.LembreteEmEdicao.CaminhoImagem = dest;
                return;
            }

            // 2) Se não for arquivo, mas tiver bitmap, tenta salvar como clipboard
            if (e.Data.GetDataPresent(DataFormats.Bitmap))
            {
                // reaproveita o helper do clipboard
                var path = SalvarImagem.SalvarImagemDoClipboardEmPastaApp();
                if (path != null)
                {
                    vm.LembreteEmEdicao.CaminhoImagem = path;
                }
            }
        }

        private void DropZone_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.V && Keyboard.Modifiers == ModifierKeys.Control)
            {
                if (DataContext is not LembretesViewModel vm || vm.LembreteEmEdicao is null)
                    return;

                var path = SalvarImagem.SalvarImagemDoClipboardEmPastaApp();
                if (path != null)
                {
                    vm.LembreteEmEdicao.CaminhoImagem = path;
                }

                e.Handled = true;
            }
        }
        private void DropZone_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DropZone.Focus(); // agora Ctrl+V vem pra cá
            Keyboard.Focus(DropZone);
        }
    }
}
