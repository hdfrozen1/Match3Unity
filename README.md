# Triple Match Game - Project Documentation & Changelog

Dự án game Triple Match (Match-3 kiểu mới) phát triển trên Unity. Người chơi chọn các vật phẩm từ bàn chơi (Board) đưa vào khay (Tray). Khi đủ 3 vật phẩm cùng loại trong khay, chúng sẽ biến mất.

## 🚀 Các Tính Năng Đã Thực Hiện

### 1. Cơ chế Đổ Bàn Chơi (Board Fill Logic)
- **Đảm bảo đa dạng:** Chỉnh sửa hàm `Fill()` để ưu tiên xuất hiện ít nhất một bộ 3 cho mỗi loại vật phẩm có trong `NormalItem.eNormalType`.
- **Logic Bộ 3:** Vật phẩm luôn được thêm vào theo nhóm 3 viên để đảm bảo bàn chơi luôn có khả năng hoàn thành (không bị dư lẻ).
- **Xáo trộn (Shuffle):** Sau khi tạo đủ các bộ 3, danh sách vật phẩm được xáo trộn ngẫu nhiên trước khi gán vào các ô (`Cell`).

### 2. Quản lý Khay (Tray Manager)
- **Hệ thống Slot:** Quản lý các ô trong khay, tự động tính toán vị trí cho vật phẩm bay vào.
- **Tự động sắp xếp:** Khi một bộ 3 biến mất hoặc có vật phẩm mới thêm vào, các vật phẩm còn lại tự động dồn sang trái.
- **Check Match:** Tự động kiểm tra và xóa bộ 3 khi người chơi chọn đủ.

### 3. Chế độ Chơi (Level Modes)
- **Timer Mode (Chế độ thời gian):** Thua cuộc khi hết giờ.
- **Moves Mode (Chế độ lượt đi):** Thua cuộc khi hết số lượt click cho phép.
- **Cơ chế đặc biệt cho Timer Mode:** Cho phép người chơi click vào vật phẩm trong Tray để trả ngược lại Board.

### 4. Cơ chế Trả Vật Phẩm (Return to Board)
- **Ghi nhớ vị trí:** Mỗi vật phẩm khi được click sẽ lưu lại `OriginalCell` (ô gốc).
- **Hành động ngược:** Khi click vào vật phẩm trong khay (trong chế độ Timer), vật phẩm sẽ bay ngược về đúng vị trí cũ trên bàn chơi thay vì một ô ngẫu nhiên.
- **Tối ưu hóa Click:** Sử dụng `GetItemFromHit` để nhận diện việc click vào ô nền của khay hoặc trực tiếp vật phẩm.

### 5. Hệ thống Auto-Play (Debug Tools)
- **Auto Win:** Coroutine tự động tìm các nhóm 3 vật phẩm cùng loại và click lần lượt để thắng game.
- **Auto Lose:** Coroutine tự động click vật phẩm bất kỳ cho đến khi đầy khay để test trạng thái thua cuộc.
- **Wait Mechanism:** Sử dụng `WaitUntil` để đợi các hiệu ứng animation kết thúc trước khi thực hiện bước tiếp theo, tránh xung đột logic.

---

## 🛠 Chi Tiết Thay Đổi Kỹ Thuật (Technical Changelog)

### `Board.cs`
- Thêm `GetAllOccupiedCells()`: Lấy danh sách các ô đang có vật phẩm.
- Thêm `GetFirstEmptyCell()`: Tìm ô trống đầu tiên trên bàn.
- Cập nhật `Fill()`: Logic ưu tiên chọn đủ các loại Enum trước khi lấp đầy bằng random.

### `BoardController.cs`
- Thêm logic xử lý click trong `Update()` cho cả Board và Tray.
- Thêm `HandleReturnToBoard(Item item)`: Điều khiển vật phẩm bay từ Tray về Board.
- Thêm vùng `#region AUTO PLAY` chứa các hàm giả lập người chơi.

### `TrayManager.cs`
- Thêm script `TrayManager.cs` và các hàm liên quan như 
- Thêm `RemoveItem(Item item)`: Xóa vật phẩm khỏi danh sách khay và cập nhật lại vị trí.
- Thêm `GetItemFromHit(GameObject hitObject)`: Xác định Item dựa trên Raycast vào Slot Index.
- .... vv

### `Item.cs` & `NormalItem.cs`
- Thêm thuộc tính `OriginalCell`: Lưu trữ tham chiếu ô gốc để phục vụ tính năng trả đồ.

### `UIMainManager.cs` 
- Thêm 2 field `btnAutoWin` và `btnAutoLose`: tham chiếu đến 2 nút autowin và autolose

### `GameManager.cs` &  `UIMainManager.cs`
- Thêm `GAME_WIN` vào `eStateGame`: xử lý hiện ui khi thắng

### `UIPanelGameWin.cs` 
- Thêm `UIPanelGameWin.cs` 
