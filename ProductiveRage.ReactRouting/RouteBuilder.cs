using System;
using System.Linq;
using Bridge;
using ProductiveRage.Immutable;

namespace ProductiveRage.ReactRouting
{
	public sealed class RouteBuilder
	{
		public static RouteBuilder Empty { get; } = new RouteBuilder(NonNullList<NonBlankTrimmedString>.Empty);

		private readonly NonNullList<NonBlankTrimmedString> _segments;
		private RouteBuilder(NonNullList<NonBlankTrimmedString> segments)
		{
			if (segments == null)
				throw new ArgumentNullException("segments");

			_segments = segments;
		}

		public RouteBuilder Fixed(NonBlankTrimmedString segment)
		{
			if (segment == null)
				throw new ArgumentNullException("segment");

			return new RouteBuilder(_segments.Add(segment));
		}

		public RouteBuilder Fixed(NonNullList<NonBlankTrimmedString> segments)
		{
			if (segments == null)
				throw new ArgumentNullException("segments");

			var newRouteBuilder = this;
			foreach (var segment in segments)
				newRouteBuilder = newRouteBuilder.Fixed(segment);
			return newRouteBuilder;
		}

		[IgnoreGeneric]
		public IBuildRoutesWithVariablesToMatch<TValues> Variable<TValues, TVariable>(
			Func<TVariable, TValues> valueExtender,
			Func<NonBlankTrimmedString, Optional<TVariable>> parser)
		{
			if (valueExtender == null)
				throw new ArgumentNullException("valueExtender");
			if (parser == null)
				throw new ArgumentNullException("parser");

			IBuildRoutesWithVariablesToMatch<object> routeBuilder = new BuilderWithExtractedValues<object>();
			foreach (var segment in _segments)
				routeBuilder = routeBuilder.Fixed(segment);
			return routeBuilder.Variable((noMatchedValues, value) => valueExtender(value), parser);
		}

		public IMatchRoutes ToRoute(Action<Optional<QueryString>> ifMatched)
		{
			if (ifMatched == null)
				throw new ArgumentNullException("ifMatched");
			return new StaticRouteDetails(_segments, ifMatched);
		}

		[IgnoreGeneric]
		public interface IBuildRoutesWithVariablesToMatch<TValues>
		{
			[IgnoreGeneric]
			IBuildRoutesWithVariablesToMatch<TValues> Fixed(NonBlankTrimmedString segment);
			[IgnoreGeneric]
			IMatchRoutes ToRoute(Action<TValues, Optional<QueryString>> ifMatched);
			[IgnoreGeneric]
			IBuildRoutesWithVariablesToMatch<TValuesExpanded> Variable<TValuesExpanded, TVariable>(
				Func<TValues, TVariable, TValuesExpanded> valueExtender,
				Func<NonBlankTrimmedString, Optional<TVariable>> parser);
		}

		[IgnoreGeneric]
		private sealed class BuilderWithExtractedValues<TValues> : IBuildRoutesWithVariablesToMatch<TValues>
		{
			private readonly NonNullList<IMatchSegments> _segmentMatchers;
			private readonly Optional<Func<NonNullList<object>, TValues>> _extractedValueBuilder;
			private BuilderWithExtractedValues(NonNullList<IMatchSegments> segmentMatchers, Optional<Func<NonNullList<object>, TValues>> extractedValueBuilder)
			{
				if (segmentMatchers == null)
					throw new ArgumentNullException("segmentMatchers");

				_segmentMatchers = segmentMatchers;
				_extractedValueBuilder = extractedValueBuilder;
			}
			public BuilderWithExtractedValues() : this(NonNullList<IMatchSegments>.Empty, null) { }

			[IgnoreGeneric]
			public IBuildRoutesWithVariablesToMatch<TValues> Fixed(NonBlankTrimmedString segment)
			{
				if (segment == null)
					throw new ArgumentNullException("segment");

				return new BuilderWithExtractedValues<TValues>(
					_segmentMatchers.Add(new FixedSegmentMatcher<TValues>(segment)),
					_extractedValueBuilder
				);
			}

			[IgnoreGeneric]
			public IBuildRoutesWithVariablesToMatch<TValuesExpanded> Variable<TValuesExpanded, TVariable>(
				Func<TValues, TVariable, TValuesExpanded> valueExtender,
				Func<NonBlankTrimmedString, Optional<TVariable>> parser)
			{
				if (valueExtender == null)
					throw new ArgumentNullException("valueExtender");
				if (parser == null)
					throw new ArgumentNullException("parser");

				Func<NonNullList<object>, TValuesExpanded> extractedValueBuilderExpanded = valuesExtractedFromMatchedVariables =>
				{
					// Every time that a variable URL segment is added to the route, we need to update the "value extractor" so that we can handle this additional variable segment
					// - The first time that a variable segment is added, there will not be any value extractor from previous variable segments and so the new value extractor will
					//   simply be the provided Func
					// - If no more variable segments were added then, if this route builder matched a route then that simple Func would be executed against the single variable
					//   segment, providing a "matched values" reference
					// - If subsequent variable URL segments must be matched then the value extractors are applied one at a time, pass the resulting value from the last into the
					//   next as an accumulator (so the first value extractor is given the first url segment and then the resulting value is given to the second value extractor,
					//   along with the second url segment to provide the second stage value - which would then be passed to the third value extractor, along with the third
					//   segment, if a third variable segment was specified)
					//   > The first value extractor only has to take a url segment and produce an "extracted value" while each subsequent value extractor has to take the previous
					//     value AND a url segment and produce a new value from those two pieces of information
					TValues previousValue;
					if (valuesExtractedFromMatchedVariables.Count == 1)
					{
						// Historically, when using anonymous types (which is how I expected the interim values to be represented), Bridge required that the classes and methods
						// specify [IgnoreGeneric] since it didn't generate classes for anonymous types and so would emit "Anonymous Type" as the type parameter, which meant that
						// the JavaScript would be invalid. However, this meant that "previousValue" could not be set to default(TValues) since "TValues" would not be known at
						// runtime and so "previousValue" would be set to null if it had no value, which required that "TValues" be a reference type (otherwise the C# compiler
						// would report an error: "Cannot convert null to type parameter 'TValues' because it could be a non-nullable value type. Consider using 'default(TValues)'
						// instead". A different workaround is to set "previousValue" to null using Script.Write, which means that there is no need for a "TValues : class" type
						// constraint. The fact that a value type may be set to null will have no impact because it only gets set to null if it won't be used.
						previousValue = Script.Write<TValues>("null");
					}
					else
					{
						// When accessing the "Value" property on an Optional, where that property value is a function, we can't execute it directly due to the way that Bridge
						// generates the JavaScript - so we need to copy the function reference (to "valueBuilderForPreviousSegments", here) and then execute that as a function
						var previousSegments = valuesExtractedFromMatchedVariables.Take((int)valuesExtractedFromMatchedVariables.Count - 1);
						previousValue = _extractedValueBuilder.Value(previousSegments.ToNonNullList());
					}

					// If this lambda is executed then we know that the last segment that was matched was a variable which was parsed using the current parser (since that is
					// the point at which this lambda would be useful). However, valuesExtractedFromMatchedVariables is only a NonNullList<object> and each value does not have any
					// additional type information - so we need to perform a cast from object to TVariable (which, again, we are confident is safe since the only way that
					// this code path should be executed is if the last parsed value was parsed using the parser in this scope). We can't use a regular cast since TVariable
					// is not available at runtime (due to the use of [IgnoreGeneric]) but, since we know that the value is already of the correct type and that there is no
					// cast logic that must be performed (there is no change that an implicit cast operator will be executed, for example, since the type is already correct),
					// we can use Script.Write<T> to step around C#'s compile-time type checking even without having access to TVariable at runtime.
					var finalValueExtractedFromMatchedVariables = valuesExtractedFromMatchedVariables.Last();
					return valueExtender(previousValue, Script.Write<TVariable>("finalValueExtractedFromMatchedVariables"));
				};

				return new BuilderWithExtractedValues<TValuesExpanded>(
					_segmentMatchers.Add(new VariableStringSegmentMatcher<TValues, TVariable, TValuesExpanded>(parser)),
					extractedValueBuilderExpanded
				);
			}

			[IgnoreGeneric]	
			public IMatchRoutes ToRoute(Action<TValues, Optional<QueryString>> ifMatched)
			{
				if (ifMatched == null)
					throw new ArgumentNullException("ifMatched");
				return new VariableRouteDetails(_segmentMatchers, _extractedValueBuilder, ifMatched);
			}

			[IgnoreGeneric]
			private sealed class VariableRouteDetails : IMatchRoutes
			{
				private readonly NonNullList<IMatchSegments> _segmentMatchers;
				private readonly Optional<Func<NonNullList<object>, TValues>> _extractedValueBuilder;
				private readonly Action<TValues, Optional<QueryString>> _ifMatched;
				public VariableRouteDetails(NonNullList<IMatchSegments> segmentMatchers, Optional<Func<NonNullList<object>, TValues>> extractedValueBuilder, Action<TValues, Optional<QueryString>> ifMatched)
				{
					if (segmentMatchers == null)
						throw new ArgumentNullException("segmentMatchers");
					if (ifMatched == null)
						throw new ArgumentNullException("ifMatched");

					_segmentMatchers = segmentMatchers;
					_extractedValueBuilder = extractedValueBuilder;
					_ifMatched = ifMatched;
				}

				public bool ExecuteCallbackIfUrlMatches(UrlDetails url)
				{
					if (url == null)
						throw new ArgumentNullException("url");

					if (url.Segments.Count != _segmentMatchers.Count)
						return false;

					var valuesExtractedFromMatchedVariables = NonNullList<object>.Empty;
					foreach (var segmentAndMatcher in url.Segments.Zip(_segmentMatchers, (segment, matcher) => new { Segment = segment, Matcher = matcher }))
					{
						var matchResult = segmentAndMatcher.Matcher.Match(segmentAndMatcher.Segment);
						if (!matchResult.IsDefined)
							return false;

						if (matchResult.Value.ValueExtractedFromVariableSegment.IsDefined)
							valuesExtractedFromMatchedVariables = valuesExtractedFromMatchedVariables.Add(matchResult.Value.ValueExtractedFromVariableSegment.Value);
					}
					if (!valuesExtractedFromMatchedVariables.Any())
					{
						// The VariableRouteDetails class should only be used if there is at least one variable url segment to match, otherwise the StaticRouteDetails would be
						// more sensible. Since both VariableRouteDetails and StaticRouteDetails are private and may only be instantiated by the RouteBuilder, there is no way
						// that we should find ourselves here with no variable segments to process.
						throw new Exception("This shouldn't happen because.. ");
					}

					// Note: The number of segments that the extractedValueBuilder expects should be consistent with the number of matchedVariables (this is always the case for
					// VariableRouteDetails instances created by the RouteBuilder which means that it should always be the general case since VariableRouteDetails is a private
					// class and may not be instantiated by anything other than the RouteBuilder)
					TValues extractedValue;
					if (_extractedValueBuilder.IsDefined)
						extractedValue = _extractedValueBuilder.Value(valuesExtractedFromMatchedVariables);
					else
					{
						// If there's no _extractedValueBuilder then no-one cares about what's matched here, so we can set extractedValue to null (we have to use Script.Write
						// because we can't be sure that TValues is not a value type and we can't set it to default(TValues) because we don't have access to the TValues type
						// at runtime because this class is decorated with [IgnoreGeneric]
						extractedValue = Script.Write<TValues>("null");
					}
					_ifMatched(extractedValue, url.QueryString);
					return true;
				}

				public IMatchRoutes MakeRelativeTo(NonNullList<NonBlankTrimmedString> parentSegments)
				{
					if (parentSegments == null)
						throw new ArgumentNullException("parentSegments");

					if (!parentSegments.Any())
						return this;

					var newSegmentMatchers = _segmentMatchers;
					foreach (var segment in parentSegments.Reverse())
						newSegmentMatchers = newSegmentMatchers.Insert(new FixedSegmentMatcher<TValues>(segment));
					return new VariableRouteDetails(newSegmentMatchers, _extractedValueBuilder, _ifMatched);
				}

				public override string ToString()
				{
					return "/" + string.Join("/", _segmentMatchers);
				}
			}
		}

		private interface IMatchSegments
		{
			Optional<SegmentMatchResult> Match(NonBlankTrimmedString segment);
		}

		private sealed class SegmentMatchResult : IAmImmutable
		{
			public SegmentMatchResult(Optional<object> valueExtractedFromVariableSegment)
			{
				this.CtorSet(_ => _.ValueExtractedFromVariableSegment, valueExtractedFromVariableSegment);
			}
			public Optional<object> ValueExtractedFromVariableSegment { get; }
		}

		[IgnoreGeneric]
		private sealed class FixedSegmentMatcher<TValues> : IMatchSegments
		{
			private readonly NonBlankTrimmedString _segment;
			public FixedSegmentMatcher(NonBlankTrimmedString segment)
			{
				if (segment == null)
					throw new ArgumentNullException("segment");
				_segment = segment;
			}

			[IgnoreGeneric]
			public Optional<SegmentMatchResult> Match(NonBlankTrimmedString segment)
			{
				if (segment == null)
					throw new ArgumentNullException("segment");
				return segment.Value.Equals(_segment.Value, StringComparison.OrdinalIgnoreCase)
					? new SegmentMatchResult(valueExtractedFromVariableSegment: null)
					: null;
			}

			public override string ToString()
			{
				return _segment.Value;
			}
		}

		[IgnoreGeneric]
		private sealed class VariableStringSegmentMatcher<TValues, TVariable, TValuesExpanded> : IMatchSegments
		{
			private readonly Func<NonBlankTrimmedString, Optional<TVariable>> _parser;
			public VariableStringSegmentMatcher(Func<NonBlankTrimmedString, Optional<TVariable>> parser)
			{
				if (parser == null)
					throw new ArgumentNullException("parser");
				_parser = parser;
			}

			[IgnoreGeneric]
			public Optional<SegmentMatchResult> Match(NonBlankTrimmedString segment)
			{
				if (segment == null)
					throw new ArgumentNullException("segment");

				var parsedValue = _parser(segment);
				return parsedValue.IsDefined
					? new SegmentMatchResult(valueExtractedFromVariableSegment: parsedValue.Value)
					: null;
			}
			public override string ToString()
			{
				return "{}";
			}
		}

		private sealed class StaticRouteDetails : IMatchRoutes
		{
			private readonly NonNullList<NonBlankTrimmedString> _segments;
			private readonly Action<Optional<QueryString>> _ifMatched;
			public StaticRouteDetails(NonNullList<NonBlankTrimmedString> segments, Action<Optional<QueryString>> ifMatched)
			{
				if (segments == null)
					throw new ArgumentNullException("segments");
				if (ifMatched == null)
					throw new ArgumentNullException("ifMatched");

				_segments = segments;
				_ifMatched = ifMatched;
			}

			public bool ExecuteCallbackIfUrlMatches(UrlDetails url)
			{
				if (url == null)
					throw new ArgumentNullException("url");

				if (url.Segments.Count != _segments.Count)
					return false;

				if (url.Segments.Zip(_segments, (x, y) => x.Value.Equals(y.Value, StringComparison.OrdinalIgnoreCase)).Any(isMatch => !isMatch))
					return false;

				_ifMatched(url.QueryString);
				return true;
			}

			public IMatchRoutes MakeRelativeTo(NonNullList<NonBlankTrimmedString> parentSegments)
			{
				if (parentSegments == null)
					throw new ArgumentNullException("parentSegments");

				if (!parentSegments.Any())
					return this;

				var newSegments = _segments;
				foreach (var segment in parentSegments.Reverse())
					newSegments = newSegments.Insert(segment);
				return new StaticRouteDetails(newSegments, _ifMatched);
			}

			public override string ToString()
			{
				return "/" + string.Join("/", _segments);
			}
		}
	}
}
