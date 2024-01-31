namespace lostborn_backend.Models
{
    public class Products
    {
        public int ID { get; set; }
        public string Title { get; set; }
        public string Weight { get; set; }
        public string Description { get; set; }
        public string Price { get; set; }
        public string ImageUrl { get; set; }
        public string ProdStatus { get; set; }
        public string ProdFrag { get; set; }
        public string DescriptionLong { get; set; }
        public string InStock { get; set; }
        public string AverageRating { get; set; }
        public string ImageUrlDetail { get; set; }

        public Products(int ID, string Title, string Weight, string Description, string Price, string ImageUrl,
                        string ProdStatus, string ProdFrag, string DescriptionLong,
                        string InStock, string AverageRating, string ImageUrlDetail)
        {
            this.ID = ID;
            this.Title = Title;
            this.Weight = Weight;
            this.Description = Description;
            this.Price = Price;
            this.ImageUrl = ImageUrl;
            this.ProdStatus = ProdStatus;
            this.ProdFrag = ProdFrag;
            this.DescriptionLong = DescriptionLong;
            this.InStock = InStock;
            this.AverageRating = AverageRating;
            this.ImageUrlDetail = ImageUrlDetail;
        }
    }
}


