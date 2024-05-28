using AutoMapper;
using Dtos.Pagination;
using EntityFramework;
using Microsoft.EntityFrameworkCore;

public class OrderService
{
    private AppDBContext _appDbContext;
    private readonly IMapper _mapper;

    public OrderService(AppDBContext appDbContext, IMapper mapper)
    {
        _appDbContext = appDbContext;
        _mapper = mapper;
    }

    // public async Task<PaginationResult<OrderModel>> GetAllOrdersService(int pageNumber, int pageSize)
    // {
    //     var totalCount = _appDbContext.Orders.Count();
    //     var totalPages = (int)Math.Ceiling((decimal)totalCount / pageSize);
    //     var order = _appDbContext.Orders.Include(o=>o.Products).AsQueryable();
    //     var items = await order.Skip((pageNumber - 1) * pageSize)
    //         .Take(pageSize).Select(order => _mapper.Map<OrderModel>(order)).ToListAsync();

    //     return new PaginationResult<OrderModel>
    //     {
    //         Items = items,
    //         TotalCount = totalCount,
    //         PageNumber = pageNumber,
    //         PageSize = pageSize,
    //     };
    // }
    public async Task<IEnumerable<Order>> GetAllOrdersService()
    {
        return await  _appDbContext.Orders.Include(o=>o.Products).ToListAsync();
    }

    public async Task<List<Order>> GetMyOrders(Guid userId)
    {
        return await _appDbContext.Orders.Include(o => o.Products).Where(o => o.UserId == userId).ToListAsync();
    }

    public async Task<Order?> GetOrderById(Guid orderId)
    {
        return await _appDbContext.Orders.Include(o => o.Products).FirstOrDefaultAsync(o => o.OrderId == orderId);
    }

    public async Task<Guid> CreateOrderService(Guid userId, PaymentMethod paymentMethod)
    {
        // Create record
        var order = new Order
        {
            OrderId = Guid.NewGuid(),
            UserId = userId,
            Status = OrderStatus.Pending,
            Payment = paymentMethod,
            Amount = 0,
        };

        // Add the record to the context
        await _appDbContext.Orders.AddAsync(order);
        // Save to database
        await _appDbContext.SaveChangesAsync();

        return order.OrderId;
    }

    public async Task AddProductToOrder(Guid orderId, Guid productId)
    {
        var order = await _appDbContext.Orders.Include(o => o.Products).FirstOrDefaultAsync(o => o.OrderId == orderId);
        var product = await _appDbContext.Products.FindAsync(productId);

        if (order != null && product != null)
        {
            if (product.Quantity == 0)
            {
                throw new InvalidOperationException("This product is unavailable");
            }

            order.Products.Add(product);
            product.Quantity--;
            order.Amount = (double) product.Price;
            await _appDbContext.SaveChangesAsync();
        }
        else
        {
            throw new InvalidOperationException("This Product has already added to the Order");
        }
    }

    public async Task<bool> UpdateOrderService(Guid orderId, OrderModel updateOrder)
    {
        var existingOrder = _appDbContext.Orders.FirstOrDefault(o => o.OrderId == orderId);
        if (existingOrder != null)
        {
            existingOrder.Status = updateOrder.Status;
            existingOrder.Payment = updateOrder.Payment;
            existingOrder.Amount = updateOrder.Amount;

            // Add the record to the context
            _appDbContext.Orders.Update(existingOrder);
            // Save to database
            await _appDbContext.SaveChangesAsync();
            return true;
        }
        return false;
    }

    public async Task<bool> UpdateOrderService(Guid userId, Guid orderId, OrderModel updateOrder)
    {
        var existingOrder = _appDbContext.Orders.FirstOrDefault(o => o.OrderId == orderId && o.UserId == userId);
        if (existingOrder != null)
        {
            existingOrder.Payment = updateOrder.Payment;

            // Add the record to the context
            _appDbContext.Orders.Update(existingOrder);
            // Save to database
            await _appDbContext.SaveChangesAsync();
            return true;
        }
        return false;
    }

    public async Task<bool> DeleteOrderService(Guid userId, Guid orderId)
    {
        var orderToRemove = _appDbContext.Orders.FirstOrDefault(order => order.OrderId == orderId && order.UserId == userId);
        if (orderToRemove != null)
        {
            // Use context to remove
            _appDbContext.Orders.Remove(orderToRemove);
            // Save to database
            await _appDbContext.SaveChangesAsync();
            return true;
        }
        return false;
    }

    public async Task<bool> DeleteOrderService(Guid orderId)
    {
        var orderToRemove = _appDbContext.Orders.FirstOrDefault(order => order.OrderId == orderId);
        if (orderToRemove != null)
        {
            // Use context to remove
            _appDbContext.Orders.Remove(orderToRemove);
            // Save to database
            await _appDbContext.SaveChangesAsync();
            return true;
        }
        return false;
    }
}
