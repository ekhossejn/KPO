using System;
using System.Diagnostics;

namespace big_hw_1.commands
{
	public class TimeMeasureDecorator : ICommand
	{
        private readonly ICommand _command;
        private readonly Action<string, TimeSpan> _logCallback;
        
        public TimeMeasureDecorator(ICommand command, Action<string, TimeSpan> logCallback) {
            _command = command;
            _logCallback = logCallback;
        }

        public void Execute() {
            var stopwatch = Stopwatch.StartNew();
            _command.Execute();
            stopwatch.Stop();
            _logCallback(_command.GetType().Name, stopwatch.Elapsed);
        }
    }
}

