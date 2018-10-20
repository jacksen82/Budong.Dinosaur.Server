<%@ WebHandler Language="C#" Class="Test" %>

using System;
using System.Web;

public class Test : IHttpHandler {

    public void ProcessRequest(HttpContext context)
    {
            context.Response.Write(new Random().Next(0,4));
    }

    public bool IsReusable {
        get {
            return false;
        }
    }

}