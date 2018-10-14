<%@ WebHandler Language="C#" Class="SetUserInfo" %>

using System.Web;
using Budong.Common.Utils;

public class SetUserInfo : IHttpHandler {

    public void ProcessRequest(HttpContext context)
    {
        //  格式化参数
        int gender = Parse.ToInt(context.Request.Params["gender"]);
        string nick = context.Request.Params["nick"];
        string avatarUrl = context.Request.Params["avatarUrl"];
        string session3rd = context.Request.Params["session3rd"];

        //  定义返回结果
        Hash result = ClientService.Token(session3rd);

        if (result.ToInt("code") == 0)
        {
            result = ClientService.SetUserInfo(result.ToHash("data"), nick, gender, avatarUrl);
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