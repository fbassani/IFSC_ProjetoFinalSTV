using System;
using System.Diagnostics;
using System.Windows.Controls;
namespace Visao.Util {

    public class TxtBoxTraceListener : TraceListener {
        readonly TextBox _textBox;

        public TxtBoxTraceListener(TextBox textBox) {
            _textBox = textBox;
        }

        public override void Write(string message) {
            _textBox.Dispatcher.Invoke(new Action<string>(AtualizeTexto), message);
        }
        public override void WriteLine(string message) {
            _textBox.Dispatcher.Invoke(new Action<string>(AtualizeTexto), String.Format("{0}\r\n", message));
        }

        private void AtualizeTexto(string msg) {
            _textBox.AppendText(msg);
        }
    }
}