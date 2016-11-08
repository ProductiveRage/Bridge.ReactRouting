using System;
using System.Linq;
using Microsoft.Owin;
using Owin;

[assembly: OwinStartup(typeof(ExampleHost.Startup))]

namespace ExampleHost
{
	public class Startup
	{
		public void Configuration(IAppBuilder app)
		{
			app.Run(context =>
			{
				return context.Response.WriteAsync(StripOffsetWhitespace(@"
					<!DOCTYPE html>
					<html lang=""en"" xmlns=""http://www.w3.org/1999/xhtml"">
					<head>
						<meta charset=""utf-8"" />
						<title>Host for interactive testing of ProductiveRage.ReactRouting</title>
						<link rel=""stylesheet"" href=""/Styles.css"">
					</head>
					<body>
						<div id=""main""></div>
						<script src=""https://cdnjs.cloudflare.com/ajax/libs/react/15.3.0/react.js""></script>
						<script src=""https://cdnjs.cloudflare.com/ajax/libs/react/15.3.0/react-dom.js""></script>
						<script src=""/Generated/Example.js""></script>
					</body>
					</html>
				"));
			});
		}

		private static string StripOffsetWhitespace(string value)
		{
			if (value == null)
				throw new ArgumentNullException("value");

			var lines = value.Split(new[] { Environment.NewLine }, StringSplitOptions.None);
			var firstLineWithOffset = lines.FirstOrDefault(line => line != "");
			if (firstLineWithOffset == null)
				return value;

			var offset = new string(firstLineWithOffset.TakeWhile(c => (c != '\r') && (c != '\n') && char.IsWhiteSpace(c)).ToArray());
			if (offset == "")
				return value;

			return
				string.Join(
					Environment.NewLine,
					lines.Select(line => line.StartsWith(offset) ? line.Substring(offset.Length) : line)
				)
				.Trim();
		}
	}
}
