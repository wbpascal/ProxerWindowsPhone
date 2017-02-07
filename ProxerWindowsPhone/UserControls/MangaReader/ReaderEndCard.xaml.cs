using System;
using Windows.Graphics.Display;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.Effects;
using Microsoft.Graphics.Canvas.Numerics;
using Microsoft.Graphics.Canvas.UI;
using Microsoft.Graphics.Canvas.UI.Xaml;
using Proxer.ViewModels.Media;
using ReactiveUI;

namespace Proxer.UserControls.MangaReader
{
    public sealed partial class ReaderEndCard : UserControl, IReaderPage
    {
        public static readonly DependencyProperty UriSourceProperty = DependencyProperty.Register(
            nameof(UriSource), typeof(string), typeof(ReaderEndCard), new PropertyMetadata(default(string)));

        public static readonly DependencyProperty ViewModelProperty = DependencyProperty.Register(
            nameof(ViewModel), typeof(ReaderEndCardViewModel), typeof(ReaderEndCard),
            new PropertyMetadata(default(ReaderEndCardViewModel)));

        private CanvasBitmap _canvasBitmap;

        public ReaderEndCard()
        {
            this.InitializeComponent();
            this.WhenAnyValue(card => card.UriSource).Subscribe(this.OnBackgroundUriChanged);
            this.WhenAnyValue(card => card.ViewModel).Subscribe(model => this.DataContext = model);
        }

        #region Properties

        public string UriSource
        {
            get { return (string) this.GetValue(UriSourceProperty); }
            set { this.SetValue(UriSourceProperty, value); }
        }

        public ReaderEndCardViewModel ViewModel
        {
            get { return (ReaderEndCardViewModel) this.GetValue(ViewModelProperty); }
            set { this.SetValue(ViewModelProperty, value); }
        }

        #endregion

        #region Methods

        private async void CanvasOnCreateResources(CanvasControl sender, CanvasCreateResourcesEventArgs args)
        {
            this._canvasBitmap = await CanvasBitmap.LoadAsync(sender.Device, new Uri(this.UriSource));
            double lCanvasHeight = this._canvasBitmap.Size.Height * (this.ActualWidth / this._canvasBitmap.Size.Width);
            this.Height = Math.Max(lCanvasHeight, this.ActualHeight);
            this.BackgroundPresenter.Height = lCanvasHeight;
            sender.Invalidate();
        }

        private void CanvasOnDraw(CanvasControl sender, CanvasDrawEventArgs args)
        {
            if (this._canvasBitmap == null) return;

            using (CanvasDrawingSession session = args.DrawingSession)
            {
                session.Units = CanvasUnits.Pixels;

                double displayScaling = DisplayInformation.GetForCurrentView().LogicalDpi / 96.0;
                double pixelWidth = sender.ActualWidth * displayScaling;
                double scalefactor = pixelWidth / this._canvasBitmap.Size.Width;

                ScaleEffect scaleEffect = new ScaleEffect
                {
                    Source = this._canvasBitmap,
                    Scale = new Vector2
                    {
                        X = (float) scalefactor,
                        Y = (float) scalefactor
                    }
                };
                GaussianBlurEffect blurEffect = new GaussianBlurEffect
                {
                    Source = scaleEffect,
                    BlurAmount = 4.0f
                };

                session.DrawImage(blurEffect, 0.0f, 0.0f);
            }
        }

        private void OnBackgroundUriChanged(string uri)
        {
            if (string.IsNullOrEmpty(uri)) return;

            this._canvasBitmap = null;
            CanvasControl lCanvasControl = new CanvasControl();
            lCanvasControl.Draw += this.CanvasOnDraw;
            lCanvasControl.CreateResources += this.CanvasOnCreateResources;
            this.BackgroundPresenter.Content = lCanvasControl;
        }

        #endregion
    }
}