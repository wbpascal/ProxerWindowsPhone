using System;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.Xaml.Navigation;

namespace Proxer
{
    /// <summary>
    ///     Stellt das anwendungsspezifische Verhalten bereit, um die Standardanwendungsklasse zu ergänzen.
    /// </summary>
    public sealed partial class App : Application
    {
        private TransitionCollection _transitions;

        /// <summary>
        ///     Initialisiert das Singletonanwendungsobjekt. Dies ist die erste Zeile von erstelltem Code
        ///     und daher das logische Äquivalent von main() bzw. WinMain().
        /// </summary>
        public App()
        {
            this.InitializeComponent();
            this.Suspending += this.OnSuspending;
        }

        #region

        /// <summary>
        ///     Wird aufgerufen, wenn die Anwendung durch den Endbenutzer normal gestartet wird. Weitere Einstiegspunkte
        ///     werden verwendet, wenn die Anwendung zum Öffnen einer bestimmten Datei, zum Anzeigen
        ///     von Suchergebnissen usw. gestartet wird.
        /// </summary>
        /// <param name="e">Details über Startanforderung und -prozess.</param>
        protected override void OnLaunched(LaunchActivatedEventArgs e)
        {
            Frame rootFrame = Window.Current.Content as Frame;

            // App-Initialisierung nicht wiederholen, wenn das Fenster bereits Inhalte enthält.
            // Nur sicherstellen, dass das Fenster aktiv ist.
            if (rootFrame == null)
            {
                // Frame erstellen, der als Navigationskontext fungiert und zum Parameter der ersten Seite navigieren
                rootFrame = new Frame {CacheSize = 1};

                // TODO: diesen Wert auf eine Cachegröße ändern, die für Ihre Anwendung geeignet ist

                if (e.PreviousExecutionState == ApplicationExecutionState.Terminated)
                {
                    // TODO: Zustand von zuvor angehaltener Anwendung laden
                }

                // Den Frame im aktuellen Fenster platzieren
                Window.Current.Content = rootFrame;
            }

            if (rootFrame.Content == null)
            {
                // Entfernt die Drehkreuznavigation für den Start.
                if (rootFrame.ContentTransitions != null)
                {
                    this._transitions = new TransitionCollection();
                    foreach (Transition c in rootFrame.ContentTransitions)
                    {
                        this._transitions.Add(c);
                    }
                }

                rootFrame.ContentTransitions = null;
                rootFrame.Navigated += this.RootFrame_FirstNavigated;

                // Wenn der Navigationsstapel nicht wiederhergestellt wird, zur ersten Seite navigieren
                // und die neue Seite konfigurieren, indem die erforderlichen Informationen als Navigationsparameter
                // übergeben werden
                if (!rootFrame.Navigate(typeof(MainPage), e.Arguments))
                {
                    throw new Exception("Failed to create initial page");
                }
            }

            // Sicherstellen, dass das aktuelle Fenster aktiv ist
            Window.Current.Activate();
        }

        /// <summary>
        ///     Wird aufgerufen, wenn die Ausführung der Anwendung angehalten wird.  Der Anwendungszustand wird gespeichert,
        ///     ohne zu wissen, ob die Anwendung beendet oder fortgesetzt wird und die Speicherinhalte dabei
        ///     unbeschädigt bleiben.
        /// </summary>
        /// <param name="sender">Die Quelle der Anhalteanforderung.</param>
        /// <param name="e">Details zur Anhalteanforderung.</param>
        private void OnSuspending(object sender, SuspendingEventArgs e)
        {
            SuspendingDeferral deferral = e.SuspendingOperation.GetDeferral();

            // TODO: Anwendungszustand speichern und alle Hintergrundaktivitäten beenden
            deferral.Complete();
        }

        /// <summary>
        ///     Stellt die Inhaltsübergänge nach dem Start der App wieder her.
        /// </summary>
        /// <param name="sender">Das Objekt, an das der Handler angefügt wird.</param>
        /// <param name="e">Details zum Navigationsereignis.</param>
        private void RootFrame_FirstNavigated(object sender, NavigationEventArgs e)
        {
            Frame rootFrame = sender as Frame;
            if (rootFrame == null) return;
            rootFrame.ContentTransitions = this._transitions ??
                                           new TransitionCollection {new NavigationThemeTransition()};
            rootFrame.Navigated -= this.RootFrame_FirstNavigated;
        }

        #endregion
    }
}