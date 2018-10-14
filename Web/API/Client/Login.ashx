<%@ WebHandler Language="C#" Class="Login" %>

using System.Web;
using Budong.Common.Utils;

public class Login : IHttpHandler {

    public void ProcessRequest(HttpContext context)
    {
        //  格式化参数
        string code = context.Request.Params["code"];

        //  定义返回结果
        Hash result = ClientService.Login(code);

        //  返回结果
        context.Response.Write(result.ToJSON());
    }

    public bool IsReusable
    {
        get
        {
            return false;
        }
    }
}