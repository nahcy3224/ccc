using Newtonsoft.Json;
using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;

namespace CHOY.App_Code.Common
{
  public class JsonResponse
  {
    private HttpResponseMessage response;

    public JsonResponse()
    {
      this.response = new HttpResponseMessage();
    }

    public JsonResponse(Object message, HttpStatusCode statusCode = HttpStatusCode.OK) : this()
    {
      this.Set(message, statusCode);
    }

    public void Set(Object message, HttpStatusCode StatusCode = HttpStatusCode.OK)
    {
      this.response.StatusCode = StatusCode;
      this.response.Content = new StringContent(JsonConvert.SerializeObject(message));
      this.response.Content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
    }

    public HttpResponseMessage Get()
    {
      return this.response;
    }

  }
}