using OSM.Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace OSM.Domain.Entities.Products
{
    public sealed class Product : Entity<Guid>, IAuditableEntity, ISoftDelete
    {
        private Product() {}

       private Product(string name, string code, int stockQuantity)
        {
            Id = Guid.NewGuid();
            Name = name;
            Code = code;
            StockQuantity = stockQuantity;
        }

        public string Name { get; private set; } = string.Empty;
        public string Code { get; private set; } = string.Empty;
        public int StockQuantity { get; private set; }
        public bool IsActive { get; private set; }

        public DateTimeOffset CreatedAt { get ; set; }
        public string? CreatedBy { get; set; }
        public DateTimeOffset? ModifiedAt { get; set; }
        public string? ModifiedBy { get; set; }
        public bool IsDeleted { get; set; }
        public DateTimeOffset? DeletedAt { get; set; }
        public string? DeletedBy { get; set; }

        public static Product Create(string name, string code, int stockQuantity)
        {
            if (string.IsNullOrWhiteSpace(name)) throw new ArgumentException("Product name is required.", nameof(name));
            if (string.IsNullOrWhiteSpace(code)) throw new ArgumentException("Product code is required.", nameof(code));
            if (stockQuantity < 0) throw new ArgumentOutOfRangeException(nameof(stockQuantity), "Stock quantity must be greater than or equal to zero.");

            var product = new Product(name.Trim(), code.Trim().ToUpperInvariant(), stockQuantity);

            // thông báo rằng trong hệ thống vừa có một product mới được tạo ra, để các phần khác có thể phản ứng nếu cần (ví dụ: gửi email thông báo, cập nhật cache, v.v.)
            product.RaiseDomainEvent(new ProductCreatedDomainEvent(product.Id));
            return product;
        }

        public void Update(string name, int stockQuantity, bool isActive)
        {
            if (string.IsNullOrWhiteSpace(name)) throw new ArgumentException("Product name is required.", nameof(name));
            if (stockQuantity < 0) throw new ArgumentOutOfRangeException(nameof(stockQuantity));

            Name = name.Trim();
            StockQuantity = stockQuantity;
            IsActive = isActive;
            ModifiedAt = DateTime.UtcNow;
        }

        public void Delete()
        {
            IsDeleted = true;
            IsActive = false;
            DeletedAt = DateTime.UtcNow;
        }

        public void Deactivate()
        {
            IsActive = false;
            ModifiedAt = DateTime.UtcNow;
        }

        public void Activate()
        {
            IsActive = true;
            ModifiedAt = DateTime.UtcNow;
        }
    }
}
