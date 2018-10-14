﻿<%@ WebHandler Language="C#" Class="Relay" %>

using System.Web;
using Budong.Common.Utils;

public class Relay : IHttpHandler {

    public void ProcessRequest(HttpContext context)
    {
        //  格式化参数
        int storyId = Parse.ToInt(context.Request.Params["storyId"]);
        int parentStoryRelayId = Parse.ToInt(context.Request.Params["parentStoryRelayId"]);
        string content = context.Request.Params["content"];
        string session3rd = context.Request.Params["session3rd"];

        //  定义返回结果
        Hash result = ClientService.Token(session3rd);

        if (result.ToInt("code") == 0)
        {
            result = StoryRelayService.Create(result.ToHash("data"), storyId, parentStoryRelayId, content);
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