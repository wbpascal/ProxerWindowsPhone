using System;

namespace Proxer
{
    public class DoublePressHandler
    {
        private readonly TimeSpan _interval;
        private DateTime _lastPressed = DateTime.MinValue;

        public DoublePressHandler(TimeSpan interval)
        {
            this._interval = interval;
        }

        #region

        protected virtual void OnPressedOnce()
        {
            this.PressedOnce?.Invoke(this, EventArgs.Empty);
        }

        protected virtual void OnPressedTwice()
        {
            this.PressedTwice?.Invoke(this, EventArgs.Empty);
        }

        public void Pressed()
        {
            if (DateTime.Now.Subtract(this._lastPressed) < this._interval) this.OnPressedTwice();
            else this.OnPressedOnce();

            this._lastPressed = DateTime.Now;
        }

        public event EventHandler PressedOnce;

        public event EventHandler PressedTwice;

        #endregion
    }
}