## 变量列表
### 食用方法
在执行命令的文本中插入``%变量名%`` 即可 
>例子：  
``现在是%DateTime%``→``现在是2022/1/1 20:00:00``
### 日期变量
| 变量名 | 说明 | 示例 |  
| ---- | ---- | ---- |
| DateTime  | 当前时间 | ``2022/1/1 20:00:00``| 
| DateTime-Year  | 年（1-9999） | ``2022`` | 
| DateTime-Month  | 月（1-12） | ``1`` | 
| DateTime-Day  | 日（1-31） | ``1`` | 
| DateTime-Hour  | 小时（0-23） | ``20`` | 
| DateTime-Minute  | 分钟（0-59） | ``0`` | 
| DateTime-Second  | 秒（0-59） | ``0`` | 

### 消息变量（私聊）
| 变量名 | 说明 | 示例 |  
| ---- | ---- | ---- |
| QQ-Age  | 年龄 | ``1`` | 
| QQ-ID  | 发送者 QQ 号 | ``114514`` | 
| QQ-Nickname  | 昵称（在资料中设置） | ``我是昵称`` | 
| QQ-Sex  | 性别 | ``未知`` ``男`` ``女`` |

若非消息触发的命令使用以上字段可能**返回空值或保留原文**
### 消息变量（群聊）
| 变量名 | 说明 | 示例 |  
| ---- | ---- | ---- |
| QQ-Age  | 年龄 | ``1`` | 
| QQ-ID  | 发送者 QQ 号 | ``114514`` | 
| QQ-Sex  | 性别 | ``未知`` ``男`` ``女`` | 
| QQ-Area  | 地区 | *未知* | 
| QQ-Card  | 群名片 | ``我是群名片`` | 
| QQ-Level  | 成员等级 | *未知* | 
| QQ-Title  | 专属头衔 | ``我是专属头衔`` | 
| QQ-Nickname  | 成员昵称（在资料中设置） | ``我是昵称`` | 
| QQ-Role  | 角色 | ``群主`` ``管理员`` ``成员`` | 
| QQ-ShownName  | 显示名称（群名片若空则为昵称） | ``我是群名片``或``我是昵称`` | 

>参考：https://docs.go-cqhttp.org/event/#%E7%BE%A4%E6%B6%88%E6%81%AF  
需要注意的是， 各字段是尽最大努力提供的， 也就是说， **不保证每个字段都一定存在**， **也不保证存在的字段都是完全正确的** ( 缓存可能过期 ) 。尤其对于匿名消息， 此字段**不具有参考价值**。

若非群聊消息触发的命令使用以上字段可能**返回空值或保留原文**