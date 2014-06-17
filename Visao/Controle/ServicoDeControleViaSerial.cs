using System;
using System.Diagnostics;
using System.IO.Ports;
using System.Reflection;
using Visao.Properties;
using Visao.Reconhecimento;

namespace Visao.Controle {
    public class ServicoDeControleViaSerial : IServicoDeControle {

        private readonly IServicoDeVisao _servicoDeVisao;
        private const byte MsgInicioParadaMotor = 0x0A;
        private const byte MsgDescarte = 0x0D;
        private const byte MsgCamera = 0x0F;

        private SerialPort _serialPort;
        private DateTime _ultimoAcionamento;

        public ServicoDeControleViaSerial(IServicoDeVisao servicoDeVisao) {
            _servicoDeVisao = servicoDeVisao;
            _servicoDeVisao.ImagemProcessada += ServicoDeVisaoOnImagemProcessada;
        }

        private void ServicoDeVisaoOnImagemProcessada(bool aprovado) {
            if (!aprovado) {
                AcioneValvulaDescarte();
            }
        }

        public void Conecte(string endereco) {
            if (_serialPort != null) {
                return;
            }
            try {
                Trace.WriteLine("Conectando na porta " + endereco);
                _serialPort = new SerialPort(endereco);
                _serialPort.DataReceived += SerialPortDataReceived;
                _serialPort.Open();
                Trace.WriteLine("Conectado");
            } catch (Exception e) {
                Dispose();
                Trace.WriteLine("[ERRO] " + e.Message);
            }
        }

        void SerialPortDataReceived(object sender, SerialDataReceivedEventArgs e) {
            var serialPort = (SerialPort)sender;    
            if (e.EventType == SerialData.Chars) {
                var b = serialPort.ReadByte();
                Trace.WriteLine("[RX] " + b.ToString("X2"));
                if (b == MsgCamera) {
                    ProcesseImagem();
                }
            }
        }

        public void InicieMotor() {
            Trace.WriteLine("[TX] (ativar motor) " + MsgInicioParadaMotor.ToString("X2"));
            TenteExecutar(() => _serialPort.BaseStream.WriteByte(MsgInicioParadaMotor));
        }
            
        public void PareMotor() {
            Trace.WriteLine("[TX] (parar motor) " + MsgInicioParadaMotor.ToString("X2"));
            TenteExecutar(() => _serialPort.BaseStream.WriteByte(MsgInicioParadaMotor));
        }

        public void AcioneValvulaDescarte() {
            if (DateTime.Now - _ultimoAcionamento > TimeSpan.FromMilliseconds(Settings.Default.MinimoEntreAcionamentos)) {
                Trace.WriteLine("[TX] (descartar produto) " + MsgDescarte.ToString("X2"));
                TenteExecutar(() => _serialPort.BaseStream.WriteByte(MsgDescarte));
                _ultimoAcionamento = DateTime.Now;
            }
        }

        public void ProcesseImagem() {
            Trace.WriteLine("Capturar imagem");
            _servicoDeVisao.ProcesseImagem();
        }

        public void Dispose() {
            if (_serialPort == null) {
                return;
            }
            try {
                PareMotor();
                _serialPort.DataReceived -= SerialPortDataReceived;
                _serialPort.Dispose();

                _servicoDeVisao.ImagemProcessada -= ServicoDeVisaoOnImagemProcessada;
                _servicoDeVisao.Dispose();
            } catch {}
        }

        private static void TenteExecutar(Action acao) {
            try {
                acao();
            } catch (Exception e) {
                Trace.WriteLine(String.Format("[ERRO] Em {0} - {1} ", acao.GetMethodInfo().Name, e.Message));
            }    
        }
    }
}