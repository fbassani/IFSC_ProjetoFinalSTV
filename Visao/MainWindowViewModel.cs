using System.IO.Ports;
using System.Windows.Input;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using Visao.Controle;

namespace Visao {
    public class MainWindowViewModel : ViewModelBase {
        private readonly IServicoDeControle _servicoDeControle;
        private string _portaSelecionada;
        private bool _iniciou;
        private bool _conectarHabilitado;
        private bool _iniciarHabilitado;
        private bool _acionarValvulaHabilitado;
        private bool _capturaManualHabilitado;

        public string PortaSelecionada {
            get { return _portaSelecionada; }
            set {
                _portaSelecionada = value;
                RaisePropertyChanged(() => PortaSelecionada);
                ConectarHabilitado = true;
            }
        }

        public bool ConectarHabilitado {
            get { return _conectarHabilitado; }
            set {
                _conectarHabilitado = value;
                RaisePropertyChanged(() => ConectarHabilitado);
            }
        }

        public bool IniciarHabilitado {
            get { return _iniciarHabilitado; }
            set {
                _iniciarHabilitado = value;
                RaisePropertyChanged(() => IniciarHabilitado);
            }
        }

        public bool AcionarValvulaHabilitado {
            get { return _acionarValvulaHabilitado; }
            set {
                _acionarValvulaHabilitado = value;
                RaisePropertyChanged(() => AcionarValvulaHabilitado);
            }
        }

        public bool CapturaManualHabilitado {
            get { return _capturaManualHabilitado; }
            set {
                _capturaManualHabilitado = value;
                RaisePropertyChanged(() => CapturaManualHabilitado);
            }
        }

        public string[] PortasDisponiveis { get; private set; }
        public ICommand ConectarCommand { get; private set; }
        public ICommand MotorCommand { get; private set; }
        public ICommand ValvulaCommand { get; private set; }
        public ICommand CapturaManualCommand { get; private set; }

        public MainWindowViewModel(IServicoDeControle servicoDeControle) {
            _servicoDeControle = servicoDeControle;

            PortasDisponiveis = SerialPort.GetPortNames();
            ConectarCommand = new RelayCommand(Conecte);
            MotorCommand = new RelayCommand(ControleMotor);
            ValvulaCommand = new RelayCommand(AcioneValvula);
            CapturaManualCommand = new RelayCommand(CaptureImagemManualmente);
#if DEBUG
            CapturaManualHabilitado = true;
#endif
        }

        private void CaptureImagemManualmente() {
           _servicoDeControle.ProcesseImagem();
        }

        private void AcioneValvula() {
            _servicoDeControle.AcioneValvulaDescarte();
        }

        private void ControleMotor() {
            if (!_iniciou) {
                _servicoDeControle.InicieMotor();
                _iniciou = true;
            } else {
                _servicoDeControle.PareMotor();
                _iniciou = false;
            }

            AcionarValvulaHabilitado = _iniciou;
        }

        private void Conecte() {
            _servicoDeControle.Conecte(PortaSelecionada);
            IniciarHabilitado = true;
            CapturaManualHabilitado = true;
        }
    }
}