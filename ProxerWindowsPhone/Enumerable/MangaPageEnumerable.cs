using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Azuria.Media;
using Splat;

namespace Proxer.Enumerable
{
    public class MangaPageEnumerable : IEnumerable<IBitmap>
    {
        private readonly IEnumerable<Manga.Chapter.Page> _pages;

        public MangaPageEnumerable(IEnumerable<Manga.Chapter.Page> pages)
        {
            this._pages = pages;
        }

        #region Properties

        public int Length => this._pages.Count();

        #endregion

        #region Methods

        /// <inheritdoc />
        public IEnumerator<IBitmap> GetEnumerator()
        {
            return new MangaPageEnumerator(this._pages);
        }

        /// <inheritdoc />
        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        #endregion
    }
}