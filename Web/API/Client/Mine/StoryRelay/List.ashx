<%@ WebHandler Language="C#" Class="List" %>

using System.Web;
using Budong.Common.Utils;

public class List : IHttpHandler {

    public void ProcessRequest(HttpContext context)
    {
        //  格式化参数
        int pageId = Parse.ToInt(context.Request.Params["pageId"]);
        int pageSize = Parse.ToInt(context.Request.Params["pageSize"], 10);
        string session3rd = context.Request.Params["session3rd"];

        //  定义返回结果
        Hash result = ClientService.Token(session3rd);

        if (result.ToInt("code") == 0)
        {
            result = ClientStoryRelayService.List(result.ToHash("data"), pageId, pageSize);
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