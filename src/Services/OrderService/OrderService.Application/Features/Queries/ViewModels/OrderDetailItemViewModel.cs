#nullable disable

namespace OrderService.Application.Features.Queries.ViewModels
{
    public class OrderDetailItemViewModel
    {
        public string Productname { get; init; }
        public int Units { get; init; }
        public double Unitprice { get; init; }
        public string Pictureurl { get; init; }
    }
}

