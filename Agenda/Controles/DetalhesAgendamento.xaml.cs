﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
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

namespace AgendaNovo.Controles
{
    /// <summary>
    /// Interação lógica para DetalhesAgendamento.xam
    /// </summary>
    public partial class DetalhesAgendamento : UserControl
    {
        public DetalhesAgendamento()
        {
            InitializeComponent();
        }
        private void Testar_Click(object sender, RoutedEventArgs e)
        {
            Debug.WriteLine("Clique no botão capturado!");
        }
    }
}
