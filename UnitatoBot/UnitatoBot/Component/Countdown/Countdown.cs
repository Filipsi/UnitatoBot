using System;
using System.Timers;

namespace UnitatoBot.Component.Countdown {

    public partial class Countdown {

        private readonly Timer  _timer;
        private TimeSpan        _length;
        private double          _elapsedSeconds;
        private double          _lastStage;

        public double Stage => Math.Round(12 / 100F * (_elapsedSeconds / _length.TotalSeconds * 100F));
        public TimeSpan Remining => TimeSpan.FromSeconds(_length.TotalSeconds - _elapsedSeconds);

        public event EventHandler<StateChangedEventArgs> OnStateChanged;

        public Countdown(TimeSpan length) {
            _elapsedSeconds = 0;
            _length = length;
            _timer = new Timer(1000);
            _timer.Elapsed += OnTimerElapsed;
            _timer.Start();
        }

        private void OnTimerElapsed(object sender, ElapsedEventArgs e) {
            if(_elapsedSeconds < _length.TotalSeconds) {
                _elapsedSeconds++;

                double stage = Stage;
                if(Math.Abs(stage - _lastStage) > 1) {
                    _lastStage = stage;
                    OnStateChanged(this, new StateChangedEventArgs(this));
                }
            } else {
                _timer.Stop();
                _timer.Dispose();
                OnStateChanged(this, new StateChangedEventArgs(this));
            }
        }

    }

}
