<%@ WebHandler Language="C#" Class="Share" %>

using System.Web;
using Budong.Common.Utils;

public class Share : IHttpHandler {

    public void ProcessRequest(HttpContext context)
    {
        //  格式化参数
        string shareFrom = context.Request.Params["shareFrom"];
        string session3rd = context.Request.Params["session3rd"];

        //  定义返回结果
        Hash result = ClientService.Token(session3rd);

        if (result.ToInt("code") == 0)
        {
            result = ClientShareService.Share(result.ToHash("data"), shareFrom);
        }

        //  记录日志
        ClientLogService.Append(session3rd);

        //  返回结果
        context.Response.Write(result.ToJSON());
    }

    public bool IsReusable {
        get {
            return false;
        }
    }

}