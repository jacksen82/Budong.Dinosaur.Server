<%@ WebHandler Language="C#" Class="Answer" %>

using System.Web;
using Budong.Common.Utils;

public class Answer : IHttpHandler {

    public void ProcessRequest(HttpContext context)
    {
        //  格式化参数
        int dinosaurId = Parse.ToInt(context.Request.Params["dinosaurId"]);
        int _result  = Parse.ToInt(context.Request.Params["result"]);
        string session3rd = context.Request.Params["session3rd"];

        //  定义返回结果
        Hash result = ClientService.Token(session3rd);

        if (result.ToInt("code") == 0)
        {
            result = GameService.Answer(result.ToHash("data"), dinosaurId, _result);
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