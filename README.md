# *Q框架SDK开发包


```C#
        /// <summary>
        /// 登录.
        /// </summary>
        /// <param name="userId">账号.</param>
	/// <param name="password">密码.</param>
	public static void LoginIn(string userId, string password)

        /// <summary>
        /// 发送好友消息.
        /// </summary>
        /// <param name="thisQQ">自己的QQ号.</param>
        /// <param name="sendQQ">好友QQ号.</param>
        /// <param name="szMsg">发送的内容.</param>
        /// <returns>FriendWithdrawInfo.用于撤回消息</returns>
        public static API.FriendWithdrawInfo SendPrivateMsg(long thisQQ,long sendQQ,string szMsg )
       
        /// <summary>
        /// 发送好友图片.
        /// </summary>
        /// <param name="thisQQ">自己的QQ号.</param>
        /// <param name="sendQQ">好友QQ号.</param>
        /// <param name="PicData">图片的字节内容.</param>
        public static void SendPrivatePicMsg(long thisQQ, long sendQQ, byte[] PicData)
        
        /// <summary>
        /// 发送好友语音.
        /// </summary>
        /// <param name="thisQQ">自己的QQ号.</param>
        /// <param name="sendQQ">好友QQ号.</param>
        /// <param name="AudioData">amr格式语音字节内容.</param>
        public static void SendPrivateAudioMsg(long thisQQ, long sendQQ, byte[] AudioData)
       
        /// <summary>
        /// 发送好友XML消息.
        /// </summary>
        /// <param name="thisQQ">自己的QQ号.</param>
        /// <param name="sendQQ">好友QQ号.</param>
        /// <param name="xmlMsg">xml文本内容.</param>
        public static void SendPrivateXmlMsg(long thisQQ, long sendQQ, string xmlMsg)
       
        /// <summary>
        /// 撤回消息.
        /// </summary>
        /// <param name="sendQQ">QQ号.</param>
        /// <param name="MsgReqId">消息ID.</param>
        /// <param name="MsgRandomId">识别ID.</param>
        /// <param name="MsgTimeStamp">发送时间.</param>
        public static void PrivateMsgWithdraw(long sendQQ, long MsgReqId, long MsgTimeStamp, long MsgRandomId=0)
        
        /// <summary>
        /// 获取昵称.
        /// </summary>
        /// <param name="AnyQQ">要获取的QQ号.</param>
        /// <returns>返回QQ资料泛型集合</returns>
        public static List<string> GetQQNick(long AnyQQ)
        
        /// <summary>
        /// 获取好友列表.
        /// </summary>
        /// <param name="thisQQ">自己的QQ号.</param>
        public static List<string> GetFriendList(long thisQQ)
       
        /// <summary>
        /// 发送群消息.
        /// </summary>
        /// <param name="thisQQ">自己的QQ号.</param>
        /// <param name="groupId">群号.</param>
        /// <param name="szMsg">消息文本内容.</param>
        /// <param name="sendQQ">要@对方加入对方QQ号.</param>
        public static API.GroupWithdrawInfo SendGroupMsg(long thisQQ, long groupId, string szMsg, long sendQQ = 0)
       
        /// <summary>
        /// 发送群图片.
        /// </summary>
        /// <param name="thisQQ">自己的QQ号.</param>
        /// <param name="groupId">群号.</param>
        /// <param name="PicData">图片字节内容.</param>
        /// <param name="sendQQ">要@对方加入对方QQ号.</param>
        public static void SendGroupPicMsg(long thisQQ,  long groupId, byte[] PicData, long sendQQ= 0 )
        
        /// <summary>
        /// 发送群语音.
        /// </summary>
        /// <param name="thisQQ">自己的QQ号.</param>
        /// <param name="groupId">群号.</param>
        /// <param name="AudioData">amr格式语音字节内容.</param>
        /// <param name="sendQQ">要@对方加入对方QQ号.</param>
        public static void SendGroupAudioMsg(long thisQQ, long groupId, byte[] AudioData, long sendQQ = 0)
       
        /// <summary>
        /// 撤回群消息.
        /// </summary>
        /// <param name="groupId">群号.</param>
        /// <param name="MsgReqId">消息ID.</param>
        /// <param name="MsgRandomId">辨认ID.</param>
        public static void GroupMsgWithdraw(long groupId, long MsgReqId, long MsgRandomId)
       
        /// <summary>
        /// 发送群XML消息.
        /// </summary>
        /// <param name="thisQQ">自己的QQ号.</param>
        /// <param name="groupId">群号.</param>
        /// <param name="xmlMsg">xml文本内容.</param>
        /// <param name="sendQQ">要@对方加入对方QQ号.</param>
        public static void SendGroupXmlMsg(long thisQQ,  long groupId, string xmlMsg, long sendQQ = 0)
       
        /// <summary>
        /// 获取群列表.
        /// </summary>
        /// <param name="thisQQ">我的QQ号.</param>
        /// <returns>返回群号集合</returns>
        public static List<string> GetGroupList(long thisQQ)
        
        /// <summary>
        /// 获取群成员员列表.
        /// </summary>
        /// <param name="groupId">群号.</param>
        /// <returns>返回QQ泛型集合</returns>
        public static List<string> GetGroupMemberList(long groupId)
        
        /// <summary>
        /// 获取群管理员列表.
        /// </summary>
        /// <param name="groupId">群号.</param>
        /// <returns>返回QQ泛型集合</returns>
        public static List<string> GetGroupAdminList(long groupId)
        
        /// <summary>
        /// 关闭全员禁言.
        /// </summary>
        /// <param name="groupId">群号.</param>
        public static void ShutDownGroup(long groupId)
       
        /// <summary>
        /// 开启全员禁言.
        /// </summary>
        /// <param name="groupId">群号.</param>
        public static void ShutUpGroup(long groupId)
        
        /// <summary>
        /// 禁言群成员.
        /// </summary>
        /// <param name="groupId">群号.</param>
        /// <param name="HisId">对方QQ号.</param>
        /// <param name="times">禁言时长.</param>
        public static void ShutUpMember(long GroupId, long HisId, long times)
        
        /// <summary>
        /// 删除群成员.
        /// </summary>
        /// <param name="groupId">群号.</param>
        /// <param name="HisId">对方QQ号.</param>
        /// <param name="IsRefuseNext">是否再接受申请.</param>
        public static void RemoveGroupMember(long GroupId, long HisId, bool IsRefuseNext)
        
        /// <summary>
        /// 解散群.
        /// </summary>
        /// <param name="groupId">群号.</param>
        public static void DisbandGroup(long thisQQ, long GroupId)
        
        /// <summary>
        /// 退群.
        /// </summary>
        /// <param name="groupId">群号.</param>
        /// <param name="thisQQ">QQ号.</param>
        public static void LeavingGroup(long thisQQ, long GroupId)
       
        /// <summary>
        /// 提取PKey.
        /// </summary>
        /// <param name="domain">域名："weiyun.com" "tenpay.com" "openmobile.qq.com" "qun.qq.com"  "game.qq.com"   "mail.qq.com"  "qzone.com"  "qzone.qq.com" "vip.qq.com" "gamecenter.qq.com" ...</param>
        public static String GetPKey(string domain)
       
        /// <summary>
        /// 提取SKey.
        /// </summary>
        public static String GetSKey()
       
```
