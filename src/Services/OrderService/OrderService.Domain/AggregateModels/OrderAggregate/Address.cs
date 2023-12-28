#nullable disable

namespace OrderService.Domain.AggregateModels.OrderAggregate
{
    //ValueObject yerine record tüm işlemlerimizi yapıyor.
	public record Address
	{
        public Address(string street, string city, string state, string country, string zipCode)
        {
            Street = street;
            City = city;
            State = state;
            Country = country;
            ZipCode = zipCode;
        }
        public Address()
        {

        }
        public String Street { get; private set; }
		public String City { get; private set; }
		public String State { get; private set; }
		public String Country { get; private set; }
		public String ZipCode { get; private set; }

	}
}

