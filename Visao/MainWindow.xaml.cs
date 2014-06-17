using System;
using System.Diagnostics;
using System.Windows;
using Visao.Controle;
using Visao.Reconhecimento;
using Visao.Util;

namespace Visao {
    public partial class MainWindow : Window {
        private readonly IServicoDeControle _servicoDeControle;

        private MainWindowViewModel _viewModel;

        public MainWindow(IServicoDeControle servicoDeControle) {
            try {
                _servicoDeControle = servicoDeControle;
                _viewModel = new MainWindowViewModel(servicoDeControle);
                InitializeComponent();
                Trace.Listeners.Add(new TxtBoxTraceListener(LogTextBox));
                DataContext = _viewModel;
                Closing += (sender, args) => servicoDeControle.Dispose();
            } catch {}
        }

        public MainWindow() : this(new ServicoDeControleViaSerial(new ServicoDeVisao(new ReconhecimentoPorCor()))) {}

        protected override void OnClosed(EventArgs e) {
            try {
                _servicoDeControle.Dispose();
            } catch { }

        }
    }
}
