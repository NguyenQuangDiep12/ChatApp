# He thong Chatapp da nen tang

## Gioi thieu du an:
-- He thong chatapp da nen tang phuc vu cho muc dich hoc tap

## cac cong nghe su dung 
*Backend* .NET 8, EF Core, PostgreSQL.
*Frontend* ReactJs(Vite), TailwindCss.
*Database* PostgreSQL 14

## Hướng dẫn cài đặt

### 1. Clone dự án

```bash
git clone <repo-url>
cd ChatApp
```

### 2. Cấu hình PostgreSQL

Đảm bảo bạn đã cài đặt PostgreSQL và tạo database `ChatApp`.

Mở file `appsettings.json` trong project `ChatApp.API` và cấu hình:

```json
"ConnectionStrings": {
  "DefaultConnection": "Host=localhost;Port=5432;Database=ChatApp;Username=***;Password=***"
}
```

---

### Lưu ý

* `Host`: địa chỉ server (mặc định: `localhost`)
* `Port`: cổng PostgreSQL (mặc định: `5432`)
* `Database`: tên database (cần tạo trước)
* `Username` / `Password`: tài khoản PostgreSQL của bạn

---

### Tạo database (nếu chưa có)

Bạn có thể tạo nhanh bằng lệnh:

```bash
createdb ChatApp
```

hoặc dùng pgAdmin để tạo thủ công.

---

### Test kết nối (khuyến khích)

```bash
psql -U postgres -d ChatApp
```

Nếu vào được shell PostgreSQL là OK.

---

### 3. Tạo và áp dụng Migration

Nếu project **chưa có migration**, chạy:

```bash
dotnet ef migrations add InitialCreate \
--project ChatApp.Repository \
--startup-project ChatApp.API
```

Sau đó:

```bash
dotnet ef database update \
--project ChatApp.Repository \
--startup-project ChatApp.API
```

> Nếu project đã có sẵn migration, chỉ cần chạy `database update`.

---

### 4. Chạy ứng dụng

```bash
dotnet run --project ChatApp.API
```


## Kien truc cua du an
-- Kien truc duoc xay dung tren Kien truc NLayer Architecture duoc lay cam hung tu Project `https://github.com/BerkayKulak/NLayerArchitecture`
-- gom 4 lop: API, CORE, REPOSITORY, SERVICE

*DATA ACCESS / REPOSITORY LAYER* 
- Overview: DAL (Data Access Control) la noi chiu trach nhiem ket noi voi co so du lieu 
- ![img](NLayerArchitecture.Repository.png)

*CORE LAYER*
- Overview: Phát triển logic nghiệp vụ với sự trừu tượng hóa. Giao diện điều khiển các yêu cầu nghiệp vụ với việc triển khai đơn giản. 
- Dự án Core là trung tâm của thiết kế Kiến trúc Sạch, và tất cả các dự án phụ thuộc khác đều phải hướng về nó.
- ![img](NLayerArchitecture.Repository.png)

*BUSINESS / SERVICE LOGIC LAYER*
- Overview: Lớp này cần xử lý tất cả logic chuyên biệt của ứng dụng, nhờ đó toàn bộ logic được tập trung tại một vị trí để dễ dàng quản lý. 
- Các phương thức CRUD nguyên tử của lớp truy cập dữ liệu có thể được sử dụng để tạo ra các kịch bản nghiệp vụ có ý nghĩa, và lớp logic nghiệp vụ này thường được thêm vào dưới dạng các dịch vụ.
- ![img](NLayerArchitecture.Service.png)

*API LAYER*
- Overview: Lớp API (thư viện phần mềm) không gì khác hơn là một trung gian tổng hợp tất cả các dịch vụ mà bạn cung cấp. 
- Giao diện người dùng đồ họa cung cấp sự tương tác cho người dùng, và đằng sau đó, API xử lý các hành động ở chế độ trừu tượng.
- ![img](NLayerArchitecture.API.png)

## Design Entity - Relationship Model
- Mo hinh moi quan he giua cac thuc the duoc the hien nhu sau:
- https://drive.google.com/file/d/1DIQRr0dgIuNAqvLhSiCYgwC156nN-hMb/view?usp=sharing


## Design Business Flow API


## Conclude
Du an dang trong qua trinh phat trien va hoan thien cac tinh nang 


