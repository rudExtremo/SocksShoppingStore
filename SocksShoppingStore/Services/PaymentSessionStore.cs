using System.Collections.Concurrent;
using SocksShoppingStore.Models;

namespace SocksShoppingStore.Services
{
    public class PaymentSessionStore
    {
        public enum SessionStatus { Created, Completed }

        private readonly ConcurrentDictionary<string, (Order Order, SessionStatus Status)> _store = new();

        public void SaveDraft(string sessionId, Order order)
        {
            _store[sessionId] = (Clone(order), SessionStatus.Created);
        }

        public bool TryGet(string sessionId, out Order? order, out SessionStatus status)
        {
            if (_store.TryGetValue(sessionId, out var value))
            {
                order = value.Order;
                status = value.Status;
                return true;
            }
            order = null;
            status = SessionStatus.Created;
            return false;
        }

        public void MarkCompleted(string sessionId)
        {
            if (_store.TryGetValue(sessionId, out var value))
            {
                _store[sessionId] = (value.Order, SessionStatus.Completed);
            }
        }

        private static Order Clone(Order src)
        {
            return new Order
            {
                CustomerName = src.CustomerName,
                Email = src.Email,
                AddressLine1 = src.AddressLine1,
                AddressLine2 = src.AddressLine2,
                City = src.City,
                PostalCode = src.PostalCode,
                Country = src.Country,
                Items = src.Items.Select(i => new OrderItem
                {
                    ProductId = i.ProductId,
                    Name = i.Name,
                    UnitPrice = i.UnitPrice,
                    Quantity = i.Quantity
                }).ToList()
            };
        }
    }
}

