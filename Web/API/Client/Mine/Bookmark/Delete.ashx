<%@ WebHandler Language="C#" Class="Delete" %>

using System.Web;
using Budong.Common.Utils;

public class Delete : IHttpHandler {

    public void ProcessRequest(HttpContext context)
    {
        //  格式化参数
        int storyId = Parse.ToInt(context.Request.Params["storyId"]);
        int storyRelayId = Parse.ToInt(context.Request.Params["storyRelayId"]);
        string session3rd = context.Request.Params["session3rd"];

        //  定义返回结果
        Hash result = ClientService.Token(session3rd);

        if (result.ToInt("code") == 0)
        {
            result = ClientBookmarkService.Delete(result.ToHash("data"), storyId, storyRelayId);
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