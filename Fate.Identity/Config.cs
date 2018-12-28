using IdentityServer4;
using IdentityServer4.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Fate.Identity
{
    public class Config
    {
        public static IEnumerable<ApiResource> GetApiResources()
        {
            return new List<ApiResource>(){
                new ApiResource("Fate_Admin","Fate Admin"),
                new ApiResource("Fate_GateWay","User Service"),
                new ApiResource("Fate_ContactAPI","Fate ContactAPI"),
                new ApiResource("Fate_UserAPI","Fate UserAPI"),
                new ApiResource("Fate_ProjectAPI","Fate ProjectAPI"),
                new ApiResource("Fate_RecommendAPI","Fate RecommendAPI")

            };
        }

        public static IEnumerable<Client> GetClients()
        {
            return new List<Client>{
                new Client{
                    ClientId="client",
                    // 没有交互性用户，使用 clientid/secret 实现认证。
                    AllowedGrantTypes=GrantTypes.ClientCredentials,
                    AllowAccessTokensViaBrowser=true,
                     // 用于认证的密码
                    ClientSecrets={new Secret("secret".Sha256())},
                    //AccessTokenType=AccessTokenType.Jwt,
                    AllowedScopes={
                        "Fate_UserAPI"
                    }
                },
                new Client{
                    ClientId="pwdclient",
                    AllowedGrantTypes=GrantTypes.ResourceOwnerPassword,
                    ClientSecrets={new Secret("secret".Sha256())},
                    RequireClientSecret=false,//是否需要secret
                    AllowedScopes={
                        IdentityServerConstants.StandardScopes.Profile,
                        IdentityServerConstants.StandardScopes.OpenId,
                        IdentityServerConstants.StandardScopes.OfflineAccess,
                        "Fate_UserAPI"
                    },
                },
                new Client{
                    ClientId="mvc2",
                    ClientName="Mvc Client",
                    ClientUri="http://localhost:5001",

                    LogoUri="http://static.freepik.com/free-photo/location-address_318-27954.jpg",
                    AllowedGrantTypes=GrantTypes.Implicit,
                    ClientSecrets={new Secret("secret".Sha256())},//授权密钥 必须一致
                    RequireConsent=true, //是否显示用户授权 且Controller名字必须为Consent
                    //RequireClientSecret=false,//是否需要secret
                    RedirectUris={"http://localhost:5001/signin-oidc" },
                    PostLogoutRedirectUris={ "http://localhost:5001/signout-callback-oidc"},
                    AllowedScopes={
                        IdentityServerConstants.StandardScopes.Profile,
                        IdentityServerConstants.StandardScopes.OpenId,
                        IdentityServerConstants.StandardScopes.OfflineAccess,
                        "Fate_UserAPI"

                    },
                },
                new Client{
                    ClientId="mvc",
                    ClientName="Mvc Client",
                    //ClientUri="http://localhost:5001",
                    RequireClientSecret=false,
                    //LogoUri="http://static.freepik.com/free-photo/location-address_318-27954.jpg",
                    AllowedGrantTypes=new List<string>{ "sms_auth_code"},//混合模式
                    ClientSecrets={new Secret("secret".Sha256())},//授权密钥 必须一致
                    //RequireConsent=true, //是否显示用户授权 且Controller名字默认为Consent
                    //RequireClientSecret=false,//是否需要secret
                    //RedirectUris={"http://localhost:5001/signin-oidc" },
                    //PostLogoutRedirectUris={ "http://localhost:5001/signout-callback-oidc"},
                    AllowOfflineAccess=true,
                    AlwaysIncludeUserClaimsInIdToken=true,//显示Claims（从ID Token直接传过去(客户端不需要处理)，混合模式需要）
                    // 客户端有权访问的范围（Scopes）
                    AllowedScopes={
                        IdentityServerConstants.StandardScopes.Profile,
                        IdentityServerConstants.StandardScopes.OpenId,
                        IdentityServerConstants.StandardScopes.OfflineAccess,
                        "Fate_GateWay",
                        "Fate_ContactAPI",
                        "Fate_UserAPI",
                        "Fate_ProjectAPI",
                        "Fate_RecommendAPI"
                    },
                },
                new Client{
                    ClientId="testadmin",
                    ClientName="Mvc Client",
                    //ClientUri="http://localhost:5001",
                    //RequireClientSecret=false,
                    //LogoUri="http://static.freepik.com/free-photo/location-address_318-27954.jpg",
                    AllowedGrantTypes=GrantTypes.Implicit,
                    ClientSecrets={new Secret("secret".Sha256())},//授权密钥 必须一致
                    RequireConsent=false, //是否显示用户授权 且Controller名字默认为Consent
                    //RequireClientSecret=false,//是否需要secret
                    AccessTokenType=AccessTokenType.Reference,//指定访问令牌是一个参考令牌还是自包含的JWT（JSON Web Token) 令牌(默认是Jwt).
                    RedirectUris={"http://localhost:8008/signin-oidc" },
                    AllowAccessTokensViaBrowser=true,//指定客户端是否可以通过浏览器请求访问令牌。（简单模式需要）
                    PostLogoutRedirectUris={ "http://localhost:8008/signout-callback-oidc"},
                    AllowOfflineAccess=true,
                    AlwaysIncludeUserClaimsInIdToken=true,//显示Claims（从ID Token直接传过去(客户端不需要处理)，混合模式需要）
                    // 客户端有权访问的范围（Scopes）
                    AllowedScopes={
                        IdentityServerConstants.StandardScopes.Profile,
                        IdentityServerConstants.StandardScopes.OpenId,
                        IdentityServerConstants.StandardScopes.OfflineAccess,
                        "Fate_Admin"
                    },
                },
                new Client{
                    ClientId="testadmin2",
                    ClientName="Mvc Client",
                    //ClientUri="http://localhost:5001",
                    //RequireClientSecret=false,
                    LogoUri="http://static.freepik.com/free-photo/location-address_318-27954.jpg",
                    AllowedGrantTypes=GrantTypes.HybridAndClientCredentials,
                    ClientSecrets={new Secret("secret".Sha256())},//授权密钥 必须一致
                    RequireConsent=false, //是否显示用户授权 且Controller名字默认为Consent
                    //RequireClientSecret=true,
                    //AccessTokenType=AccessTokenType.Reference,
                    RedirectUris={"http://localhost:8008/signin-oidc" },
                    //AllowAccessTokensViaBrowser=true, 混合模式不需要了
                    PostLogoutRedirectUris={ "http://localhost:8008/signout-callback-oidc"},
                    AllowOfflineAccess=true,//混合模式需要为true
                    AlwaysIncludeUserClaimsInIdToken=true,//显示Claims（从ID Token直接传过去(客户端不需要处理)，混合模式需要）
                    // 客户端有权访问的范围（Scopes）
                    AllowedScopes={
                        IdentityServerConstants.StandardScopes.Profile,
                        IdentityServerConstants.StandardScopes.OpenId,
                        IdentityServerConstants.StandardScopes.OfflineAccess,
                        "Fate_Admin"
                    },
                },
                new Client{
                    ClientId="testclient2",
                    ClientName="Mvc Client",
                    //ClientUri="http://localhost:5001",
                    //RequireClientSecret=false,
                    LogoUri="http://static.freepik.com/free-photo/location-address_318-27954.jpg",
                    AllowedGrantTypes=GrantTypes.HybridAndClientCredentials,
                    ClientSecrets={new Secret("secret".Sha256())},//授权密钥 必须一致
                    RequireConsent=false, //是否显示用户授权 且Controller名字默认为Consent
                    //RequireClientSecret=true,
                    //AccessTokenType=AccessTokenType.Reference,
                    RedirectUris={"http://localhost:8006/signin-oidc" },
                    //AllowAccessTokensViaBrowser=true, 混合模式不需要了
                    PostLogoutRedirectUris={ "http://localhost:8006/signout-callback-oidc"},
                    //AllowOfflineAccess=true,
                    AlwaysIncludeUserClaimsInIdToken=true,//显示Claims（从ID Token直接传过去(客户端不需要处理)，混合模式需要）
                    // 客户端有权访问的范围（Scopes）
                    AllowedScopes={
                        IdentityServerConstants.StandardScopes.Profile,
                        IdentityServerConstants.StandardScopes.OpenId,
                        IdentityServerConstants.StandardScopes.OfflineAccess,
                        "Fate_Admin"
                    },
                },
                new Client{
                    ClientId="testclient",
                    ClientName="Mvc Client",
                    //ClientUri="http://localhost:5001",
                    //RequireClientSecret=false,
                    LogoUri="http://static.freepik.com/free-photo/location-address_318-27954.jpg",
                    AllowedGrantTypes=GrantTypes.Implicit,
                    ClientSecrets={new Secret("secret".Sha256())},//授权密钥 必须一致
                    RequireConsent=false, //是否显示用户授权 且Controller名字默认为Consent
                    //RequireClientSecret=true,
                    AccessTokenType=AccessTokenType.Reference,
                    RedirectUris={"http://localhost:8006/signin-oidc" },
                    AllowAccessTokensViaBrowser=true, //混合模式不需要了
                    PostLogoutRedirectUris={ "http://localhost:8006/signout-callback-oidc"},
                    //AllowOfflineAccess=true,
                    AlwaysIncludeUserClaimsInIdToken=true,//显示Claims（从ID Token直接传过去(客户端不需要处理)，混合模式需要）
                    // 客户端有权访问的范围（Scopes）
                    AllowedScopes={
                        IdentityServerConstants.StandardScopes.Profile,
                        IdentityServerConstants.StandardScopes.OpenId,
                        IdentityServerConstants.StandardScopes.OfflineAccess,
                        "Fate_Admin"
                    }
                   
                }

            };
        }

        public static IEnumerable<IdentityResource> GetIdentityResource()
        {
            return new List<IdentityResource> {
                new IdentityResources.OpenId(),
                new IdentityResources.Profile(),
                //new IdentityResources.Email()
            };
        }

    }
}
