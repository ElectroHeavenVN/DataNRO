# DataNRO
Xem dữ liệu vật phẩm, quái, NPC, map,... của game [**Ngọc Rồng Online**](http://ngocrongonline.com/) và một số máy chủ lậu khác. Dữ liệu được tự động cập nhật mỗi ngày bằng [GitHub Actions](https://github.com/features/actions).
<br>Dữ liệu của các server lậu được thêm vào không được cập nhật tự động.

## Link
- Web xem data chính thức: [DataNRO](https://electroheavenvn.github.io/DataNRO/)
- Web của [chủ shop ShopWibu (Cường)](https://shopwibu.net/)

## API
API được tạo ra với mục đích dùng miễn phí, vui lòng không sử dụng API cho mục đích kiếm tiền, lừa đảo hay các mục đích xấu khác.

**Định dạng API dữ liệu:** `Nhà phát hành`/`Server`/`Loại dữ liệu`
<br>**Định dạng API ảnh:** `Nhà phát hành`/Icons/`ID`.png
- Ảnh NPC: `Nhà phát hành`/NCPs/`ID template NPC`.png
- Ảnh quái: `Nhà phát hành`/Monsters/`ID template quái`.png
- Ảnh map: `Nhà phát hành`/Maps/`ID map`.png
### Nhà phát hành / Server
- `/TeaMobi`: Data Ngọc Rồng Online
  + `/Server`*: Server 1-7 và 11-13
  + `/Server8910`: Server gộp (server 8, 9, 10)
  + `/Super`*: Server Super 1 và Super 2
  + `/Universe1`: Server Quốc tế (Universe 1)
  + `/Naga`: Server Indonesia (Naga)
- `/HSNR`: Data Hồi sinh Ngọc Rồng
  + `/Server`*: Server 1-4
- `/BlueFake`: Data NRO Blue
  + `/Server1`*: Server Blue 1
- `/ILoveNRO`: Data Tôi yêu Ngọc Rồng
  + `/Server1`*: Server 1
### Loại dữ liệu
- `ItemOptionTemplates.json`: Dữ liệu loại thuộc tính vật phẩm
```json
  {
    "id": 93,
    "type": 0,
    "name": "HSD # ngày"
  }
```
- `ItemTemplates.json`: Dữ liệu vật phẩm, chứa tên, mô tả, hành tinh, sức mạnh yêu cầu,...
```json
  {
    "isUpToUp": false,
    "id": 26,
    "type": 2,
    "gender": 2,
    "level": 3,
    "strRequire": 50000,
    "iconID": 410,
    "part": -1,
    "name": "Găng sắt",
    "description": "Giúp tăng sức đánh"
  }
```
- `LastUpdated`: Thời gian mới nhất dữ liệu được cập nhật
```
2024-08-07T09:42:37.0621765Z
```
- `Maps.json`: Dữ liệu map
```json
  {
    "id": 155,
    "name": "Nhà tù Hồi sinh ngọc rồng"
  }
```
- `MobTemplates.json`: Dữ liệu quái
```json
  {
    "mobTemplateId": 80,
    "rangeMove": 0,
    "speed": 1,
    "type": 0,
    "dartType": 25,
    "hp": 2000000000,
    "name": "Mộc Nhân Super"
  }
```
- `NClasses.json`: Dữ liệu class nhân vật
```json
{
    "classId": 0,
    "name": "Trái đất",
    "skillTemplates": [
        {
        "id": 0,
        "maxPoint": 7,
        "manaUseType": 0,
        "type": 1,
        "iconId": 539,
        "name": "Chiêu đấm Dragon",
        "description": "Tấn công cận chiến",
        "damInfo": "Tăng sức đánh: #%",
        "skills": [
          {
            "point": 1,
            "maxFight": 1,
            "manaUse": 1,
            "skillId": 0,
            "dx": 32,
            "dy": 18,
            "damage": 100,
            "price": 0,
            "coolDown": 500,
            "powRequire": 1000,
            "moreInfo": "tại ông nội ngay lúc đầu"
          },
          ...
        ]
      },
      ...
    ]
}
```
- `NpcTemplates.json`: Dữ liệu NPC
```json
  {
    "npcTemplateId": 70,
    "headId": 1968,
    "bodyId": 1969,
    "legId": 1970,
    "name": "Mafia Internet <D2F>",
    "menu": [
      ["Nói chuyện"]
    ]
  }
```
- `Parts.json`: Dữ liệu part nhân vật và NPC
```json
  {
    "type": 2,
    "pi": [
      {
        "id": 11826,
        "dx": 3,
        "dy": 2
      },
      ...
    ]
  }
```

## Lưu ý
- **Không phải tất cả ảnh đều có sẵn.** API chỉ chứa những ảnh quan trọng (ảnh vật phẩm, kỹ năng, ảnh NPC) để hiển thị trên trang web.
- **Một số ảnh có thể không tồn tại trên API, kém chất lượng hoặc khác với ứng dụng khách** do máy chủ không trả về ảnh, trả về ảnh rỗng hoặc kém chất lượng, hoặc ảnh chỉ có sẵn trên ứng dụng khách.
- **Dữ liệu của một số server TeaMobi có thể bị chậm** do TeaMobi chặn địa chỉ IP nước ngoài truy cập game.
- Mã nguồn của phần lấy dữ liệu từ các server lậu là __độc quyền__ và __không có trong kho mã nguồn này__.
- Các tệp nén trong thư mục `SampleData` và trong log của các workflow đều được mã hoá mật khẩu. Nếu bạn muốn lấy dữ liệu cho mục đích riêng, vui lòng liên hệ với tôi.

# Giấy phép
Mã nguồn của dự án này, bao gồm module `DataNRO.TeaMobi`, được phát hành dưới giấy phép [GNU AGPLv3](https://www.gnu.org/licenses/agpl-3.0.en.html).

# Credit
- [Mirco Bauer](https://github.com/meebey), tác giả dự án [StarkSoftProxy](https://github.com/meebey/starksoftproxy)
- [James Newton-King](https://github.com/JamesNK), tác giả dự án [Newtonsoft.Json](https://github.com/JamesNK/Newtonsoft.Json)
- ~~Lê Trường Giang (VNPAY), code UI cho web~~ (giao diện cũ)
