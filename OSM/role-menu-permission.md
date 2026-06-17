# Role Menu Permission

## 1. Mục đích

Màn hình **Role Menu Permission** dùng để cấu hình menu và quyền thao tác cho từng role trong hệ thống.

Mỗi role có thể được phân quyền sử dụng từng menu với 3 quyền chính:

| Quyền  | Ý nghĩa                          |
| ------ | -------------------------------- |
| Read   | Được nhìn thấy / truy cập menu   |
| Write  | Được thêm mới / cập nhật dữ liệu |
| Delete | Được xóa dữ liệu                 |

Dữ liệu được lưu vào bảng:

```text
RoleMenuPermission
```

---

## 2. Các bảng liên quan

### Menus

Dùng để lấy danh sách menu hiển thị trên grid.

Các field quan trọng:

```text
MenuId
MenuName
MenuGroup
ParentMenuId
DisplayOrder
```

Ý nghĩa:

| Field        | Ý nghĩa                             |
| ------------ | ----------------------------------- |
| MenuId       | Khóa định danh menu                 |
| MenuName     | Tên menu hiển thị                   |
| MenuGroup    | Nhóm menu                           |
| ParentMenuId | Menu cha, dùng để dựng cây phân cấp |
| DisplayOrder | Thứ tự hiển thị                     |

---

### Permissions

Dùng để định nghĩa các quyền chuẩn.

Cần có tối thiểu 3 quyền:

```text
read
write
delete
```

---

### RoleMenuPermission

Lưu mapping giữa role, menu và permission.

Dữ liệu lưu theo dạng nhiều dòng:

```text
RoleId + MenuId + PermissionId
```

Ví dụ role `admin` có quyền read/write menu `menuSetup`:

```text
RoleId    MenuId      PermissionId
admin     menuSetup   read
admin     menuSetup   write
```

---

## 3. Business rules

### Rule 1: Chọn Write thì tự chọn Read

Nếu user tick quyền `Write` thì hệ thống tự động tick `Read`.

Lý do: muốn ghi dữ liệu thì bắt buộc phải được đọc / truy cập menu trước.

```text
Write = true
=> Read = true
=> Selected = true
```

---

### Rule 2: Chọn Delete thì tự chọn Read

Nếu user tick quyền `Delete` thì hệ thống tự động tick `Read`.

```text
Delete = true
=> Read = true
=> Selected = true
```

---

### Rule 3: Bỏ Read thì bỏ toàn bộ quyền

Nếu user bỏ tick `Read`, hệ thống tự động bỏ:

```text
Selected
Write
Delete
```

Kết quả:

```text
Read = false
Write = false
Delete = false
Selected = false
```

---

### Rule 4: Nếu Read = false thì không lưu database

Dù user có tick chọn menu nhưng `Read = false`, menu đó sẽ không được lưu vào bảng `RoleMenuPermission`.

Chỉ những menu có ít nhất quyền `Read` mới được lưu.

---

### Rule 5: Chọn menu con thì menu cha tự được chọn Read

Nếu user chọn menu con, hệ thống sẽ tự động chọn các menu cha phía trên với quyền `Read`.

Ví dụ:

```text
Settings
  Menu Setup
```

Nếu tick `Menu Setup`, hệ thống tự động set:

```text
Settings.Read = true
Settings.Selected = true
```

---

### Rule 6: Bỏ hết menu con thì menu cha cũng bị bỏ chọn

Nếu tất cả menu con của một menu cha đều bị bỏ chọn, menu cha cũng tự động bị bỏ chọn.

Ví dụ:

```text
Settings
  Menu Setup
  User Setup
```

Nếu bỏ chọn cả `Menu Setup` và `User Setup`, hệ thống tự động bỏ chọn `Settings`.

---

## 4. Luồng xử lý màn hình

### Bước 1: User chọn role

User chọn role trong select box.

```text
Select role
```

Client gọi API:

```http
GET /api/v1/RoleMenuPermissions/{roleId}
```

API trả về:

```json
{
  "roleId": "admin",
  "items": [
    {
      "menuId": "settings",
      "menuName": "Settings",
      "menuGroup": "SETTINGS",
      "parentMenuId": null,
      "displayOrder": 1,
      "level": 0,
      "isSelected": true,
      "canRead": true,
      "canWrite": false,
      "canDelete": false
    },
    {
      "menuId": "menuSetup",
      "menuName": "Menu Setup",
      "menuGroup": "SETTINGS",
      "parentMenuId": "settings",
      "displayOrder": 2,
      "level": 1,
      "isSelected": true,
      "canRead": true,
      "canWrite": true,
      "canDelete": false
    }
  ]
}
```

---

### Bước 2: Client hiển thị grid

Client dùng `app-ag-grid` để hiển thị danh sách menu.

Menu được hiển thị dạng phân cấp:

```text
Settings
  ↳ Menu Setup
  ↳ User Setup
Report
  ↳ Sale Report
  ↳ Inventory Report
```

Menu cha luôn nằm phía trên menu con.

Trong cùng một cấp, dữ liệu được order theo:

```text
DisplayOrder
```

---

### Bước 3: User tick quyền

User có thể tick:

```text
Selected
Read
Write
Delete
```

Mỗi lần cell thay đổi, client chạy logic:

```text
applyPermissionRules()
syncParentMenus()
```

Để đảm bảo các rule phân quyền luôn đúng.

---

### Bước 4: User bấm Update

Khi user bấm nút `Update` trên `app-setup-page-layout`, hệ thống hiển thị confirm.

Ví dụ:

```text
Bạn có chắc muốn cập nhật phân quyền menu cho role "Admin" không?
```

Nếu user chọn Cancel, hệ thống không gọi API.

Nếu user chọn OK, client gọi API:

```http
PUT /api/v1/RoleMenuPermissions/{roleId}
```

Request body:

```json
{
  "items": [
    {
      "menuId": "settings",
      "isSelected": true,
      "canRead": true,
      "canWrite": false,
      "canDelete": false
    },
    {
      "menuId": "menuSetup",
      "isSelected": true,
      "canRead": true,
      "canWrite": true,
      "canDelete": false
    }
  ]
}
```

---

### Bước 5: Server normalize lại dữ liệu

Dù client đã xử lý rule, server vẫn normalize lại lần nữa để đảm bảo an toàn.

Server xử lý:

```text
Write/Delete => Read
Read = false => không lưu
Child selected => Parent Read
All children unchecked => Parent unchecked
```

Sau đó server xóa permission cũ của role:

```sql
DELETE FROM RoleMenuPermission
WHERE RoleId = @RoleId
```

Rồi insert lại permission mới.

---

## 5. API backend

### GET role menu permissions

```http
GET /api/v1/RoleMenuPermissions/{roleId}
```

Dùng để lấy danh sách menu và quyền hiện tại của role.

Response:

```json
{
  "roleId": "admin",
  "items": [
    {
      "menuId": "menuSetup",
      "menuName": "Menu Setup",
      "menuGroup": "SETTINGS",
      "parentMenuId": "settings",
      "displayOrder": 1,
      "level": 1,
      "isSelected": true,
      "canRead": true,
      "canWrite": true,
      "canDelete": false
    }
  ]
}
```

---

### PUT role menu permissions

```http
PUT /api/v1/RoleMenuPermissions/{roleId}
```

Dùng để cập nhật toàn bộ menu permission của role.

Request:

```json
{
  "items": [
    {
      "menuId": "menuSetup",
      "isSelected": true,
      "canRead": true,
      "canWrite": true,
      "canDelete": false
    }
  ]
}
```

Response success:

```json
{
  "roleId": "admin",
  "updatedCount": 2
}
```

---

## 6. Các file backend chính

```text
OSM.API
└── Controllers
    └── BaseSetup
        └── RoleMenuPermissionsController.cs
```

Controller nhận HTTP request và gọi MediatR.

---

```text
OSM.Application
└── Features
    └── BaseSetup
        └── RoleMenuPermissions
            ├── GetRoleMenuPermissions
            │   ├── GetRoleMenuPermissionsQuery.cs
            │   └── GetRoleMenuPermissionsQueryHandler.cs
            ├── UpdateRoleMenuPermissions
            │   ├── UpdateRoleMenuPermissionsCommand.cs
            │   └── UpdateRoleMenuPermissionsCommandHandler.cs
            └── RoleMenuPermissionResponse.cs
```

Application layer chứa query, command, response DTO và logic xử lý chính.

---

## 7. Transaction

Project đã có `TransactionBehavior`.

Vì vậy command update permission không nên tự mở transaction nếu toàn bộ thao tác DB đi qua cùng transaction behavior.

Luồng update gồm:

```text
DELETE RoleMenuPermission
INSERT RoleMenuPermission
```

Hai thao tác này phải nằm trong transaction để tránh trường hợp:

```text
Delete thành công
Insert lỗi
=> mất toàn bộ permission của role
```

Nếu dùng Dapper, cần đảm bảo `DapperHelper` dùng được transaction hiện tại của EF `DbContext`.

---

## 8. Các file client chính

```text
src/app/features/Base_setup/pages/menurole/
├── menurole.ts
├── menurole.html
└── menurole.scss
```

Màn hình chính cho Role Menu Permission.

---

```text
src/app/features/Base_setup/services/
└── role-menu-permission.service.ts
```

Service gọi API backend.

---

```text
src/app/features/Base_setup/shared/models/
└── RoleMenuPermission.ts
```

Model TypeScript dùng cho màn hình.

---

```text
src/app/shared/components/ag-grid-wrapper/
├── ag-grid-wrapper.component.ts
├── ag-grid-wrapper.component.html
└── ag-grid-wrapper.component.scss
```

Wrapper dùng chung cho AG Grid.

Màn hình này dùng `app-ag-grid` để hiển thị và edit permission.

---

## 9. Model phía client

```ts
export interface RoleMenuPermissionRow {
  menuId: string;
  menuName: string;
  menuGroup?: string | null;
  parentMenuId?: string | null;
  displayOrder: number;
  level: number;

  isSelected: boolean;
  canRead: boolean;
  canWrite: boolean;
  canDelete: boolean;
}
```

---

## 10. Logic quan trọng phía client

### Khi cell thay đổi

AG Grid phát event:

```ts
cellValueChanged
```

Màn hình xử lý:

```ts
onCellValueChanged(event)
```

Sau đó gọi:

```ts
applyPermissionRules(rows, changedRow, changedField);
```

Để xử lý các rule:

```text
Write/Delete => Read
Read false => clear Write/Delete
Selected false => clear all
```

Sau đó gọi:

```ts
syncParentMenus(rows);
```

Để xử lý quan hệ cha con:

```text
Child selected => Parent read
All children unchecked => Parent unchecked
```

---

## 11. Reset grid

Nút `Reset` dùng để đưa grid về trạng thái ban đầu sau khi chọn role hoặc sau lần save gần nhất.

Khi user bấm reset:

```text
rowData = originalRows
hasChanged = false
```

Không gọi API.

---

## 12. Update grid

Nút `Update` dùng để lưu quyền menu của role hiện tại.

Trước khi lưu:

```text
- Kiểm tra đã chọn role chưa
- Kiểm tra có thay đổi chưa
- Hiển thị confirm
```

Sau khi lưu thành công:

```text
- Hiển thị thông báo thành công
- Cập nhật originalRows = rowData hiện tại
- hasChanged = false
```

---

## 13. Những lỗi thường gặp

### Không hiển thị dữ liệu khi chọn role

Kiểm tra API:

```http
GET /api/v1/RoleMenuPermissions/{roleId}
```

Kiểm tra bảng:

```text
Menus
Permissions
RoleMenuPermission
```

---

### Tick Write/Delete nhưng Read không tự tick

Kiểm tra event:

```html
(cellValueChanged)="onCellValueChanged($event)"
```

Và kiểm tra `app-ag-grid` đã expose output:

```ts
@Output() cellValueChanged = new EventEmitter<CellValueChangedEvent<any>>();
```

---

### Menu cha không tự chọn khi chọn menu con

Kiểm tra dữ liệu menu có đúng `ParentMenuId` không.

Ví dụ:

```text
Parent:
MenuId = settings

Child:
ParentMenuId = settings
```

Nếu `ParentMenuId` sai hoặc null thì client không thể dựng quan hệ cha con.

---

### Bấm update nhưng database không lưu

Kiểm tra:

```text
canRead = true
```

Nếu `canRead = false`, server sẽ không lưu dòng đó.

Kiểm tra bảng `Permissions` có đủ:

```text
read
write
delete
```

---

### Duplicate permission

Server nên distinct dữ liệu trước khi insert theo key:

```text
RoleId + MenuId + PermissionId
```

Để tránh insert trùng.

---

## 14. Tóm tắt flow

```text
User chọn Role
        ↓
Client gọi GET RoleMenuPermissions
        ↓
Server lấy Menus + RoleMenuPermission
        ↓
Client hiển thị grid dạng cây
        ↓
User tick Selected / Read / Write / Delete
        ↓
Client tự đồng bộ quyền và menu cha con
        ↓
User bấm Update
        ↓
Client confirm
        ↓
Client gọi PUT RoleMenuPermissions
        ↓
Server normalize rule lần nữa
        ↓
Server DELETE permission cũ
        ↓
Server INSERT permission mới
        ↓
Client báo thành công
```

---

## 15. Nguyên tắc maintain

Khi sửa màn hình này, cần nhớ:

```text
Client xử lý để UX mượt.
Server xử lý lại để đảm bảo dữ liệu đúng.
Database chỉ lưu menu có quyền Read trở lên.
Write/Delete không bao giờ tồn tại nếu không có Read.
Menu con có quyền thì menu cha phải có Read.
```
