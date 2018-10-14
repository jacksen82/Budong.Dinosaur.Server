<%@ WebHandler Language="C#" Class="Handler" %>

using System;
using System.Web;

public class Handler : IHttpHandler {

    public void ProcessRequest(HttpContext context)
    {

    }

    public bool IsReusable {
        get {
            return false;
        }
    }

}