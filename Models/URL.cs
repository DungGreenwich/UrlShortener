using System.ComponentModel.DataAnnotations;

namespace URLShortener.Models
{
	public class URL
	{
		public URL()
		{
			Id = Guid.NewGuid();
			OriginalUrl = "";
        }

		public Guid Id { get; set; }

        [Required]
        [Url(ErrorMessage = "Not an URL")]
		[MaxLength(Constants.MaxLengths.OriginalURL, ErrorMessage = "URL cannot exceed 2048 characters")]
		public string OriginalUrl { get; set; }

		public string ShortCode { get; set; } = string.Empty;
	}
}
