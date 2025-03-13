using AutoMapper;
using BusinessObject.Models;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        // Ánh xạ từ AspNetUsersDTO sang AspNetUsers
        CreateMap<AspNetUsersDTO, AspNetUsers>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.UserName))
            .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email))
            .ForMember(dest => dest.FirstName, opt => opt.MapFrom(src => src.FirstName))
            .ForMember(dest => dest.LastName, opt => opt.MapFrom(src => src.LastName));
            // Bỏ qua các thuộc tính không có trong DTO
            //.ForAllOtherMembers(opt => opt.Ignore());

        CreateMap<AspNetUsers, AspNetUsersDTO>();
        CreateMap<AspNetRoles, AspNetRolesDTO>();
        CreateMap<Product, ProductDTO>();
        CreateMap<ProductDTO, Product>();  // Ánh xạ từ ProductDTO sang Product
        CreateMap<Category, CategoryDTO>();
        CreateMap<CategoryDTO, Category>();  // Ánh xạ từ CategoryDTO sang Category
        CreateMap<Order, OrderDTO>();
        CreateMap<OrderDTO, Order>();
        CreateMap<OrderDetail, OrderDetailDTO>();
        CreateMap<OrderDTO, Order>()
            .ForMember(dest => dest.OrderDetails, opt => opt.MapFrom(src => src.OrderDetails));
        CreateMap<Order, OrderDTO>()
            .ForMember(dest => dest.OrderDetails, opt => opt.Ignore()); // Giữ [JsonIgnore]

        CreateMap<OrderDetailDTO, OrderDetail>();
        CreateMap<OrderDetail, OrderDetailDTO>();

        CreateMap<OrderDTO, Order>()
    .ForMember(dest => dest.OrderId, opt => opt.MapFrom(src => src.OrderId))
    .ForMember(dest => dest.MemberId, opt => opt.MapFrom(src => src.MemberId))
    .ForMember(dest => dest.OrderDate, opt => opt.MapFrom(src => src.OrderDate))
    .ForMember(dest => dest.RequiredDate, opt => opt.MapFrom(src => src.RequiredDate))
    .ForMember(dest => dest.ShippedDate, opt => opt.MapFrom(src => src.ShippedDate))
    .ForMember(dest => dest.Freight, opt => opt.MapFrom(src => src.Freight))
    .ForMember(dest => dest.OrderDetails, opt => opt.Ignore()); // Bỏ qua nếu không cần

        CreateMap<Order, OrderDTO>();
        CreateMap<Microsoft.Graph.Models.User, BusinessObject.Models.AspNetUsers>()
             .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
           .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.UserPrincipalName)) // Ánh xạ UserPrincipalName từ Microsoft.Graph.Models.User
           .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Mail)) // Ánh xạ Mail từ Microsoft.Graph.Models.User
           .ForMember(dest => dest.FirstName, opt => opt.MapFrom(src => src.GivenName)) // Ánh xạ GivenName từ Microsoft.Graph.Models.User
           .ForMember(dest => dest.LastName, opt => opt.MapFrom(src => src.Surname));
    }
}
