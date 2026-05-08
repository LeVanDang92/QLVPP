# QLVPP
Quản lý văn phòng phẩm net core + angular 21

## Mục tiêu refactor

- Giữ ý tưởng tốt từ source cũ: CQRS, MediatR, EF Core, Dapper, JWT, Identity, Result pattern, Audit.
- Sửa dependency sai: `Application` không reference `Persistence/Infrastructure`.
- Chuẩn hóa Vertical Slice: mỗi use case nằm riêng một folder.
- Command dùng EF Core transaction; Query có thể dùng EF `AsNoTracking` hoặc Dapper.
- Audit tự động ghi `CreatedAt`, `CreatedBy`, `ModifiedAt`, `ModifiedBy`.
- Có AuditLog table để track thêm thay đổi dữ liệu.
- Có Identity + JWT + Refresh Token mẫu.
- Có Authorization policy theo permission.

## Cấu trúc

```text
src/
  OfficeSuppliesTemplate.Domain
  OfficeSuppliesTemplate.Application
  OfficeSuppliesTemplate.Infrastructure
  OfficeSuppliesTemplate.Api

tests/
  OfficeSuppliesTemplate.ArchitectureTests
```

## Dependency rule

```text
Api -> Application -> Domain
Api -> Infrastructure -> Application + Domain
Domain -> không phụ thuộc project nào
```

`Application` chỉ khai báo interface. `Infrastructure` mới implement EF Core, Identity, JWT, Dapper.

## Chạy project

Cập nhật connection string trong:

```text
src/OfficeSuppliesTemplate.Api/appsettings.Development.json
```

Sau đó chạy:

```bash
dotnet restore
dotnet build
dotnet ef migrations add InitialCreate -p src/OfficeSuppliesTemplate.Infrastructure -s src/OfficeSuppliesTemplate.Api
dotnet ef database update -p src/OfficeSuppliesTemplate.Infrastructure -s src/OfficeSuppliesTemplate.Api
dotnet run --project src/OfficeSuppliesTemplate.Api
```

Swagger:

```text
https://localhost:5001/swagger
```

## Flow chuẩn

```text
Controller
  -> MediatR
  -> ValidationBehavior
  -> TransactionBehavior nếu là ICommand
  -> Handler
  -> IApplicationDbContext / Repository / QueryService
  -> EF Core / Dapper trong Infrastructure
```

## Quy ước thêm feature mới

Ví dụ thêm feature `Categories/CreateCategory`:

```text
Application/Features/Categories/CreateCategory
  CreateCategoryCommand.cs
  CreateCategoryCommandValidator.cs
  CreateCategoryCommandHandler.cs

Api/Controllers/CategoriesController.cs
```

Handler không gọi trực tiếp DbContext cụ thể. Chỉ dùng `IApplicationDbContext` hoặc abstraction trong Application.
----------------------------
## Register
Client gửi email/password
  ↓
AuthController
  ↓
RegisterCommand
  ↓
RegisterHandler
  ↓
UserManager.CreateAsync
  ↓
Tạo user trong AspNetUsers

## Login
Client gửi email/password
  ↓
LoginCommand
  ↓
LoginHandler
  ↓
Check password
  ↓
Generate JWT
  ↓
Generate RefreshToken
  ↓
Trả token về client

## Gọi API cần đăng nhập
Client gửi request kèm header:

Authorization: Bearer {access_token}

  ↓
API kiểm tra token
  ↓
Nếu token hợp lệ thì cho qua
  ↓
Nếu có policy thì kiểm tra permission

## Code feature mới thì làm theo checklist
1. Tạo Entity trong Domain nếu có bảng mới.
2. Thêm DbSet vào IApplicationDbContext.
3. Thêm DbSet vào ApplicationDbContext.
4. Tạo EntityConfiguration.
5. Tạo folder Feature theo Vertical Slice.
6. Tạo Command/Query.
7. Tạo Validator.
8. Tạo Handler.
9. Tạo Controller endpoint.
10. Add migration.
11. Update database.
12. Test Swagger.


## Đăng ký User
Client gửi userName, email, password
 ↓
RegisterCommand
 ↓
RegisterCommandHandler
 ↓
IIdentityService.RegisterAsync
 ↓
IdentityService.RegisterAsync
 ↓
UserManager.CreateAsync
 ↓
Lưu user vào AspNetUsers
 ↓
Tạo role User nếu chưa có
 ↓
Gán user vào role User

## Tạo tocken
Client gửi userName, email, password
 ↓
RegisterCommand
 ↓
RegisterCommandHandler
 ↓
IIdentityService.RegisterAsync
 ↓
IdentityService.RegisterAsync
 ↓
UserManager.CreateAsync
 ↓
Lưu user vào AspNetUsers
 ↓
Tạo role User nếu chưa có
 ↓
Gán user vào role User

---
Lấy roles của user
 ↓
Lấy permissions của user
 ↓
Tạo access token chứa userId, username, roles, permissions
 ↓
Tạo refresh token random
 ↓
Lưu refresh token vào database
 ↓
Trả TokenResponse cho client


## Flow refresh token
Client gửi refreshToken cũ
 ↓
Tìm refreshToken trong DB
 ↓
Nếu không có hoặc hết hạn thì trả lỗi
 ↓
Nếu hợp lệ thì revoke token cũ
 ↓
Tạo access token mới
 ↓
Tạo refresh token mới
 ↓
Trả token mới về client

## Flow lấy permission
User
 ↓
Lấy danh sách role của user
 ↓
Từ role name lấy role id
 ↓
Từ role id tìm RolePermissions
 ↓
Lấy Permission.Code
 ↓
Trả danh sách quyền

## Flow gọi API cần quyền
API nhận request
 ↓
JWT middleware kiểm tra token
 ↓
Nếu token sai: 401 Unauthorized
 ↓
Nếu token đúng: tạo ClaimsPrincipal
 ↓
Authorization kiểm tra policy Products.Write
 ↓
PermissionAuthorizationHandler kiểm tra claim permission = products.write
 ↓
Nếu có quyền: cho vào action
 ↓
Nếu thiếu quyền: 403 Forbidden

## Sơ đồ tổng thể Identity
Client
  |
  | POST /auth/login
  v
AuthController
  |
  v
LoginCommand
  |
  v
LoginCommandHandler
  |
  v
IIdentityService
  |
  v
IdentityService
  |
  |-- UserManager kiểm tra user/password
  |-- RoleManager lấy role
  |-- DbContext lấy permission
  |-- JwtTokenService tạo access token
  |-- DbContext lưu refresh token
  |
  v
TokenResponse
  |
  v
Client lưu accessToken + refreshToken