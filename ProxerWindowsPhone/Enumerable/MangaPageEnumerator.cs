using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Threading.Tasks;
using System.Threading.Tasks;
using Akavache;
using Azuria.Media;
using Proxer.Utility;
using Splat;

namespace Proxer.Enumerable
{
    public class MangaPageEnumerator : IEnumerator<IBitmap>
    {
        private readonly Manga.Chapter.Page[] _pages;
        private int _currentIndex = -1;

        public MangaPageEnumerator(IEnumerable<Manga.Chapter.Page> chapter)
        {
            this._pages = chapter.ToArray();
        }

        #region Properties

        /// <inheritdoc />
        public IBitmap Current { get; private set; }

        /// <inheritdoc />
        object IEnumerator.Current => this.Current;

        #endregion

        #region Methods

        /// <inheritdoc />
        public void Dispose()
        {
            this.Current.Dispose();
            Task.Factory.StartNew(async () => { await BlobCache.UserAccount.Flush().ToTask().ConfigureAwait(false); })
                .WaitTaskFactory();
        }

        /// <inheritdoc />
        public bool MoveNext()
        {
            this._currentIndex++;
            if (this._currentIndex >= this._pages.Length) return false;

            string lImageUrl = this._pages[this._currentIndex].Image.AbsoluteUri;
            Task.Factory.StartNew(async () =>
            {
                this.Current = await BlobCache.UserAccount.LoadImageFromUrl(lImageUrl,
                        absoluteExpiration: DateTimeOffset.Now.AddDays(1)).ToTask()
                    .ConfigureAwait(false);
            }).WaitTaskFactory();

            return this.Current != null;
        }

        /// <inheritdoc />
        public void Reset()
        {
            this._currentIndex = -1;
        }

        #endregion
    }
}