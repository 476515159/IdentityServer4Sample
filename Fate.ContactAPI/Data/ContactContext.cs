using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MongoDB.Driver;
using Microsoft.Extensions.Options;
using Models.Users;

namespace Fate.ContactAPI.Data
{
    public class ContactContext
    {
        private readonly IMongoDatabase _dataBase;
        //private readonly IMongoCollection<ContactBook> _collections;
        private readonly AppSetting _appSetting;

        public ContactContext(IOptionsSnapshot<AppSetting> options)//实时更新
        {
            _appSetting = options.Value;
            var client = new MongoClient(_appSetting.MongoDbConnectionString);
            if (client != null)
            {
                _dataBase = client.GetDatabase(_appSetting.MongoDbContactDataBaseName);
            }

        }

        /// <summary>
        /// 检查名称是否被注册
        /// </summary>
        /// <param name="name"></param>
        private void CreateOrCheck(string name)
        {
            var list = _dataBase.ListCollections().ToList();
            List<string> collectionName = new List<string>();
            list.ForEach(c => {
                collectionName.Add(c["name"].AsString);
            });
            if (!collectionName.Contains(name))
            {
                _dataBase.CreateCollection(name);
            }


            #region 暂未测试
            //var list2 = _dataBase.ListCollectionNames().ToList();
            //if (!collectionName.Contains(name))
            //{
            //    _dataBase.CreateCollection(name);
            //}
            #endregion
        }

        public IMongoCollection<UserInfo> UserInfo => _dataBase.GetCollection<UserInfo>("UserInfo");

        /// <summary>
        /// 用户通讯录
        /// </summary>
        public IMongoCollection<ContactBook> ContactBooks => _dataBase.GetCollection<ContactBook>("ContactBooks");

        /// <summary>
        /// 通讯录详细列表
        /// </summary>
        public IMongoCollection<Contact> Contacts => _dataBase.GetCollection<Contact>("Contacts");

        /// <summary>
        /// 好友申请请求记录
        /// </summary>
        public IMongoCollection<ContactApplyRequest> ContactApplyRequests => _dataBase.GetCollection<ContactApplyRequest>("ContactApplyRequests");
    }
}
