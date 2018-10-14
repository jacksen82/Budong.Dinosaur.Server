<%@ WebHandler Language="C#" Class="Token" %>

using System.Web;
using Budong.Common.Utils;

public class Token : IHttpHandler {

    public void ProcessRequest(HttpContext context)
    {
        //  格式化参数
        string session3rd = context.Request.Params["session3rd"];

        //  定义返回结果
        Hash result = ClientService.Token(session3rd);

        //  记录日志
        ClientLogService.Append(session3rd);

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