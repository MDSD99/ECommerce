using System;
using AutoMapper;
using OrderService.Application.Dtos;
using OrderService.Application.Features.Commands.CreateOrder;
using OrderService.Application.Features.Queries.ViewModels;
using OrderService.Domain.AggregateModels.OrderAggregate;

namespace OrderService.Application.Mapping.OrderMapping
{
	public class OrderMappingProfile:Profile
	{
		public OrderMappingProfile()
		{
			CreateMap<Order, CreateOrderCommand>().ReverseMap();
            CreateMap<OrderItem, OrderItemDto>().ReverseMap();

			CreateMap<Order, OrderDetailViewModel>()
				.ForMember(s => s.City, y => y.MapFrom(z => z.Address.City))
				.ForMember(s => s.Country, y => y.MapFrom(z => z.Address.Country))
				.ForMember(s => s.Street, y => y.MapFrom(z => z.Address.Street))
				.ForMember(s => s.Zipcode, y => y.MapFrom(z => z.Address.ZipCode))
				.ForMember(s => s.Date, y => y.MapFrom(z => z.OrderDate))
				.ForMember(s => s.OrderNumber, y => y.MapFrom(z => z.Id.ToString()))
				.ForMember(s => s.Status, y => y.MapFrom(z => z.OrderStatus.Name))
				.ForMember(s => s.Total, y => y.MapFrom(z => z.OrderItems.Sum(i => i.Units * i.Units * i.UnitPrice)))
				.ReverseMap();

			CreateMap<OrderItem, OrderDetailItemViewModel>();
        }
	}
}

