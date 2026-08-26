using System;
using System.Collections.Generic;
using Entitas;

namespace Code.Infrastructure.EntityComponentSystem.Extensions
{
	public static class CollectionExtensions
	{
		/// <summary>
		/// Evaluates the first entity in the group using the provided evaluator function.
		/// If the group is empty, returns the default value of type T.
		/// </summary>
		/// <returns>Result of the evaluator function applied to the first entity, or default(T) if the group is empty.</returns>
		public static T EvaluateFirst<T, TEntity>(
			this IGroup<TEntity> group,
			Func<TEntity, T> evaluator)
			where TEntity : class, IEntity
		{
			foreach (TEntity entity in group)
			{
				return evaluator(entity);
			}

			return default;
		}

		/// <summary>
		/// Evaluates the first entity in the group using the provided evaluator action.
		/// If the group is empty, does nothing.
		/// </summary>
		public static void EvaluateFirst<TEntity>(
			this IGroup<TEntity> group,
			Action<TEntity> evaluator)
			where TEntity : class, IEntity
		{
			foreach (TEntity entity in group)
			{
				evaluator(entity);
				return;
			}
		}

		public static T EvaluateFirst<T, TEntity>(
			this HashSet<TEntity> entities,
			Func<TEntity, T> evaluator)
			where TEntity : class, IEntity
		{
			foreach (TEntity entity in entities)
			{
				return evaluator(entity);
			}

			return default;
		}

		public static void EvaluateFirst<TEntity>(
			this HashSet<TEntity> entities,
			Action<TEntity> evaluator)
			where TEntity : class, IEntity
		{
			foreach (TEntity entity in entities)
			{
				evaluator(entity);
				return;
			}
		}
	}
}