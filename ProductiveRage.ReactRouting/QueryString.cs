using System;
using System.Linq;
using Bridge.Html5;
using ProductiveRage.Immutable;

namespace ProductiveRage.ReactRouting
{
	public sealed class QueryString
	{
		public static QueryString Parse(string queryStringWithoutQuestionMark)
		{
			if (queryStringWithoutQuestionMark == null)
				throw new ArgumentNullException("queryStringWithoutQuestionMark");
			if (queryStringWithoutQuestionMark.StartsWith("?"))
			{
				// This is primarily done so that there's no confusion about whether it's correct or not to include a string with the leading
				// question mark AND so that the ToString method doesn't have to worry about whether or not to include one
				throw new ArgumentException("The queryStringWithoutQuestionMark must not start with a '?' character");
			}

			return new QueryString(queryStringWithoutQuestionMark.Split('&')
				.Select(entry =>
				{
					// In the QueryString "?x=1&y=&z", the "x" Segment will have a string value of "1", the "y" Segment will have a string
					// value of "" and the "z" segment will have no string value (all three cases need to be supported, which is why the
					// Segment's Value type is Optional<string>, so missing, blank and non-blank must all be acceptable values)
					var elements = entry.Split('=', 2);
					if (elements.Length == 1)
						return new Segment(Global.DecodeURIComponent(entry), null);
					return new Segment(Global.DecodeURIComponent(elements[0]), Global.DecodeURIComponent(elements[1]));
				})
				.ToSet()
			);
		}

		private readonly NonNullList<Segment> _segments;
		public QueryString(NonNullList<Segment> segments)
		{
			if (segments == null)
				throw new ArgumentNullException("segments");

			_segments = segments;
		}

		public new NonNullList<Optional<string>> this[string name]
		{
			get
			{
				if (name == null)
					throw new ArgumentNullException("name");

				return _segments
					.Where(segment => segment.Name.Equals(name, StringComparison.OrdinalIgnoreCase))
					.Select(segment => segment.Value)
					.ToSet();
			}
		}

		public QueryString Add(string name, Optional<string> value)
		{
			if (name == null)
				throw new ArgumentNullException("name");

			return new QueryString(_segments.Add(new Segment(name, value)));
		}

		public QueryString RemoveIfPresent(string name)
		{
			if (name == null)
				throw new ArgumentNullException("name");

			var updatedSegments = _segments.Remove(segment => segment.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
			return (updatedSegments == _segments) ? this : new QueryString(updatedSegments);
		}

		public override string ToString()
		{
			return string.Join("&", _segments);
		}

		public sealed class Segment : IAmImmutable
		{
			public Segment(string name, Optional<string> value)
			{
				this.CtorSet(_ => _.Name, name);
				this.CtorSet(_ => _.Value, value);
			}

			public string Name { get; private set; }
			public Optional<string> Value { get; private set; }

			public override string ToString()
			{
				var nameValuePair = Global.EncodeURIComponent(Name);
				if (Value.IsDefined)
					nameValuePair += "=" + Global.EncodeURIComponent(Value.Value);
				return nameValuePair;
			}
		}
	}
}
