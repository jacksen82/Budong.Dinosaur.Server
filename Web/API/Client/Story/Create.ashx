<%@ WebHandler Language="C#" Class="Create" %>

using System.Web;
using Budong.Common.Utils;

public class Create : IHttpHandler {

    public void ProcessRequest(HttpContext context)
    {
        //  格式化参数
        string content = context.Request.Params["content"];
        string session3rd = context.Request.Params["session3rd"];

        //  定义返回结果
        Hash result = ClientService.Token(session3rd);

        if (result.ToInt("code") == 0)
        {
            result = StoryService.Create(result.ToHash("data"), content);
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