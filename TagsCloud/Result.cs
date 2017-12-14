using System;
using System.Collections.Generic;
using System.Linq;

namespace TagsCloud
{
	public class None
	{
		
	}

	public class Result<T>
	{
		public T Value { get; }
		public string Error { get; }

		public bool IsSuccess => Error == null;

		public Result(string error, T value = default(T))
		{
			Value = value;
			Error = error;
		}

		public Result<TOutput> Then<TOutput>(Func<Result<T>, TOutput> continuation)
		{
			return !IsSuccess 
				? Result.Fail<TOutput>(Error) 
				: Result.Of(() => continuation(this));
		}
		
		public Result<IEnumerable<T>> ToResultOfList(IEnumerable<Result<T>> resultsList)
		{
			var results = resultsList.ToList();

			return results.Any(r => !r.IsSuccess) 
				? Result.Fail<IEnumerable<T>>("One of elements has exception") 
				: Result.Ok(results.Select(r => r.Value));
		}
	}

	public static class Result
	{
		public static Result<T> Ok<T>(T value)
		{
			return new Result<T>(null, value);
		}

		public static Result<T> Fail<T>(string message)
		{
			return new Result<T>(message);
		}

		public static Result<TOutput> Select<TInput, TOutput>(
			this Result<TInput> input,
			Func<TInput, Result<TOutput>> continuation)
		{
			if (!input.IsSuccess)
				return Fail<TOutput>(input.Error);
			return continuation(input.Value);
		}

		public static Result<T> Of<T>(Func<T> f, string error = null)
		{
			try
			{
				return Ok(f());
			}
			catch (Exception e)
			{
				return Fail<T>(error ?? e.Message);
			}
		}

		public static Result<Dictionary<TOutputKey, TOutputValue>> ToDictionary
			<TInputKey, TInputValue, TOutputKey, TOutputValue>(
			this Result<Dictionary<TInputKey, TInputValue>> origin,
			Func<KeyValuePair<TInputKey, TInputValue>, Result<TOutputKey>> keySelector,
			Func<KeyValuePair<TInputKey, TInputValue>, Result<TOutputValue>> valueSelector)
		{
			if (!origin.IsSuccess)
				return Fail<Dictionary<TOutputKey, TOutputValue>>(origin.Error);

			var tuples = origin.Value
				.Select(p => (keySelector(p), valueSelector(p)))
				.ToList();

			return tuples.Any(t => !t.Item1.IsSuccess || !t.Item2.IsSuccess) 
				? Fail<Dictionary<TOutputKey, TOutputValue>>("One of elements has exception") 
				: Ok(tuples.ToDictionary(t => t.Item1.Value, t => t.Item2.Value));
		}

//		public static Result<IEnumerable<TOutput>> Select<TInput, TOutput>(
//			this Result<IEnumerable<TInput>> original,
//			Func<TInput, TOutput> transform)
//		{
//			if (!original.IsSuccess)
//				return Fail<IEnumerable<TOutput>>(original.Error);
//
//			return Ok(original.Value
//				.Select(transform)
//			);
//		}
	}
}
