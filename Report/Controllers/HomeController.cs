﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;

namespace Report.Controllers
{
	public class HomeController : Controller
	{
		public ActionResult Index()
		{
			return View();
		}

		/// <summary>
		/// 크롤링 Ajax 함수
		/// </summary>
		/// <param name="strUrl"></param>
		/// <param name="strType"></param>
		/// <returns></returns>
		public string PrintResultAjax(string strUrl, string strType)
		{
			string strHtml = string.Empty;
			HttpWebRequest request = (HttpWebRequest)WebRequest.Create(strUrl);
			using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
			{
				HttpStatusCode statusCode = response.StatusCode;
				if (statusCode == HttpStatusCode.OK)
				{
					using (Stream stream = response.GetResponseStream())
					{
						StreamReader reader = new StreamReader(stream, Encoding.UTF8, true);
						strHtml = reader.ReadToEnd().Replace(Environment.NewLine, string.Empty);
					}
				}
			}

			//html 태그 제외, 텍스트만 출력
			if (strType.Equals("onlyText"))
			{
				strHtml = Regex.Replace(strHtml, @"<[^>]*>", string.Empty).Replace(Environment.NewLine, string.Empty).Replace("\t", string.Empty);
			}

			//영어만 출력 후 대문자,소문자 순서로 오더바이
			var arrEng = Regex.Replace(strHtml, @"[^a-zA-Z]", string.Empty).Select(z=>z.ToString()).OrderBy(n => n, StringComparer.OrdinalIgnoreCase).ThenBy(x => x, StringComparer.Ordinal).ToArray();

			//숫자만 출력
			var arrNum = Regex.Replace(strHtml, @"[^0-9]", string.Empty).Select(n => n.ToString()).OrderBy(n => n).ToArray();

			int arrEngLen = 0;
			int arrNumLen = 0;
			int count = 0;

			string result = string.Empty;

			arrEngLen = arrEng.Length;
			arrNumLen = arrNum.Length;

			count = arrEngLen > arrNumLen ? arrEngLen : arrNumLen;

			//영어,숫자 교차로 출력
			for (int i = 0; i < count; i++)
			{
				if (arrEngLen > i)
				{
					result += arrEng[i];
				}

				if (arrNumLen > i)
				{
					result += arrNum[i];
				}
			}
			
			return result;
		}
	}
}