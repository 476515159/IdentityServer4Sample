using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Fate.Identity.ViewModels
{
    public class LoginViewModel
    {
        public string UserName { get; set; }
        /// <summary>
        /// 密码
        /// </summary>
        //[Required]
        public string Password { get; set; }
        /// <summary>
        /// 界面上的选择框  选择是否记住登录
        /// </summary>
        public bool RememberLogin { get; set; }
        /// <summary>
        /// 回调授权验证地址 这个地址与Redirect地址不一样
        /// 登录成功后会转到 ReturnUrl  然后验证授权登录后 获取到客户端的信息 然后根据Client配置中的RedirectUrl转到对应的系统
        /// </summary>
        public string ReturnUrl { get; set; }

        public string ClientName { get; set; }
        public string ClientUrl { get; set; }
        public string ClientLogoUrl { get; set; }
    }
}
