public class LineItemDto
{
    public string Title { get; set; }
    public string? VariantTitle { get; set; }
    public int Quantity { get; set; }
    public string? ImageUrl { get; set; }  // 🔥 Görsel burada taşınıyor
}
