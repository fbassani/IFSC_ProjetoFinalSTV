using System;
using System.Diagnostics;
namespace Visao.Reconhecimento {
    [DebuggerDisplay("Valor: {_valorConfigurado}, Tolerância: {_tolerancia}")]
    public struct CorTolerancia {
        private readonly byte _valorConfigurado;
        private readonly byte _tolerancia;

        public CorTolerancia(byte valorConfigurado, byte toleranciaConfigurada) {
            _valorConfigurado = valorConfigurado;

            if (valorConfigurado < toleranciaConfigurada) {
                _tolerancia = valorConfigurado;
            }

            if (valorConfigurado + toleranciaConfigurada > Byte.MaxValue) {
                _tolerancia = (byte) (Byte.MaxValue - valorConfigurado);
            } else {
                _tolerancia = toleranciaConfigurada;
            }
        }

        public byte NoExtremo(int valor) {
            var min = _valorConfigurado - _tolerancia;
            var max = _valorConfigurado + _tolerancia;

            min = min < 0 ? 0 : min;
            max = max > Byte.MaxValue ? Byte.MaxValue : max;
            
            if (valor >= min && valor <= max) {
                return Byte.MaxValue;
            }
            return Byte.MinValue;
        }
    }
}