using System.Collections.Generic;

namespace Dekofar.HyperConnect.Application.Common.Models
{
    /// <summary>
    /// Genel bir sayfalama sonucu modeli.
    /// İstenen veri listesi ile toplam kayıt sayısı gibi bilgileri tutar.
    /// </summary>
    /// <typeparam name="T">Listelemede dönecek veri tipi</typeparam>
    public class PagedResult<T>
    {
        /// <summary>
        /// Sayfadaki elemanların listesi
        /// </summary>
        public List<T> Items { get; set; } = new();

        /// <summary>
        /// Toplam kayıt sayısı
        /// </summary>
        public int TotalCount { get; set; }

        /// <summary>
        /// Sayfa başına gösterilecek kayıt sayısı
        /// </summary>
        public int PageSize { get; set; }

        /// <summary>
        /// Mevcut sayfa numarası
        /// </summary>
        public int CurrentPage { get; set; }

        /// <summary>
        /// Toplam sayfa sayısı
        /// </summary>
        public int TotalPages { get; set; }
    }
}
