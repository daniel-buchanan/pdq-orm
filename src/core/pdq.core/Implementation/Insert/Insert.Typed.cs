﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using pdq.common;
using pdq.state;

namespace pdq.Implementation
{
	internal class Insert<T> :
        InsertBase,
		IInsertInto<T>,
		IInsertValues<T>
	{
        private Insert(IQueryInternal query, IInsertQueryContext context)
            : base(query, context) { }

        public static Insert<T> Create(IInsertQueryContext context, IQueryInternal query)
            => new Insert<T>(query, context);

        /// <inheritdoc/>
        public IInsertValues<T> Columns(Expression<Func<T, dynamic>> columns)
        {
            base.AddColumns(columns);
            return this;
        }

        /// <inheritdoc/>
        public IInsertValues<T> From(Action<ISelect> query)
        {
            base.FromQuery(query);
            return this;
        }

        /// <inheritdoc/>
        public IInsertValues<T> Value(Expression<Func<T>> value)
        {
            base.AddValues(value);
            return this;
        }

        /// <inheritdoc/>
        public IInsertValues<T> Values(Expression<Func<IEnumerable<T>>> values)
        {
            base.AddValues(values);
            return this;
        }
    }
}

