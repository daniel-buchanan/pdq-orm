﻿using System;
using System.Linq;
using pdq.common.Exceptions;
using pdq.common.Templates;
using pdq.common.Utilities;
using pdq.db.common;
using pdq.db.common.Builders;
using pdq.Exceptions;
using pdq.state;

namespace pdq.db.common.ANSISQL
{
	public abstract class InsertBuilderPipeline : db.common.Builders.InsertBuilderPipeline
	{
        private readonly IQuotedIdentifierBuilder quotedIdentifierBuilder;
        private readonly IValueParser valueParser;
        private readonly IBuilderPipeline<ISelectQueryContext> selectBuilder;
        private readonly IConstants constants;

        public InsertBuilderPipeline(
            PdqOptions options,
            IHashProvider hashProvider,
            db.common.Builders.IWhereBuilder whereBuilder,
            IQuotedIdentifierBuilder quotedIdentifierBuilder,
            IValueParser valueParser,
            IBuilderPipeline<ISelectQueryContext> selectBuilder,
            IConstants constants)
            : base(options, constants, hashProvider)
        {
            this.quotedIdentifierBuilder = quotedIdentifierBuilder;
            this.valueParser = valueParser;
            this.selectBuilder = selectBuilder;
            this.constants = constants;
        }

        private void AppendItems<T>(ISqlBuilder sqlBuilder, T[] items, Action<ISqlBuilder, T> processMethod, bool appendNewLine = false)
        {
            var lastItemIndex = items.Length - 1;
            for (var i = 0; i < items.Length; i++)
            {
                var delimiter = string.Empty;
                if (i < lastItemIndex) delimiter = constants.Seperator;
                processMethod(sqlBuilder, items[i]);
                sqlBuilder.Append(delimiter);

                if(appendNewLine) sqlBuilder.AppendLine();
            }
        }

        protected override void AddColumns(IPipelineStageInput<IInsertQueryContext> input)
        {
            input.Builder.IncreaseIndent();
            var columns = input.Context.Columns.ToArray();

            input.Builder.PrependIndent();
            input.Builder.Append(constants.OpeningParen);

            AppendItems(input.Builder, columns, (b, i) => this.quotedIdentifierBuilder.AddSelect(i, b));

            input.Builder.Append(constants.ClosingParen);
            input.Builder.DecreaseIndent();
            input.Builder.AppendLine();
        }

        protected override void AddOutput(IPipelineStageInput<IInsertQueryContext> input)
        {
            if (!input.Context.Outputs.Any())
                return;

            input.Builder.AppendLine(constants.Returning);
            input.Builder.IncreaseIndent();
            var outputs = input.Context.Outputs.ToArray();

            AppendItems(input.Builder, outputs, (b, o) =>
            {
                input.Builder.PrependIndent();
                this.quotedIdentifierBuilder.AddOutput(o, input.Builder);
            });

            input.Builder.DecreaseIndent();
            input.Builder.AppendLine();
        }

        protected override void AddTable(IPipelineStageInput<IInsertQueryContext> input)
        {
            input.Builder.IncreaseIndent();

            input.Builder.PrependIndent();
            this.quotedIdentifierBuilder.AddFromTable(input.Context.Target.Name, input.Builder);
            input.Builder.AppendLine();

            input.Builder.DecreaseIndent();
        }

        protected override void AddValues(IPipelineStageInput<IInsertQueryContext> input)
        {
            if (input.Context.Source is IInsertStaticValuesSource)
                AddValuesFromStatic(input.Context, input.Builder, input.Parameters);
            else if (input.Context.Source is IInsertQueryValuesSource queryValuesSource)
                AddValuesFromQuery(queryValuesSource, input);
            else
                throw new ShouldNeverOccurException($"Insert Values should be one of {nameof(IInsertStaticValuesSource)} or {nameof(IInsertQueryValuesSource)}");
        }

        private void AddValuesFromStatic(IInsertQueryContext context, ISqlBuilder sqlBuilder, IParameterManager parameterManager)
        {
            var source = context.Source as IInsertStaticValuesSource;
            var columns = context.Columns.ToArray();

            sqlBuilder.AppendLine(constants.Values);
            var values = source.Values?.ToArray();

            if (values == null) return;

            sqlBuilder.IncreaseIndent();

            for(var i = 0; i < values.Length; i++)
            {
                sqlBuilder.PrependIndent();
                sqlBuilder.Append(constants.OpeningParen);
                
                for(var j = 0; j < values[i].Length; j++)
                {
                    var itemNumber = j + 1;
                    var quoteCharacter = valueParser.ValueNeedsQuoting(columns[j]) ?
                        constants.ValueQuote :
                        string.Empty;

                    var parameter = parameterManager.Add(columns[j], values[i][j]);
                    sqlBuilder.Append("{0}{1}{0}", quoteCharacter, parameter.Name);

                    if (itemNumber < values[i].Length)
                        sqlBuilder.Append(constants.Seperator);
                }

                sqlBuilder.Append(constants.ClosingParen);

                var valueNumber = i + 1;
                if (valueNumber < values.Length)
                    sqlBuilder.Append(constants.Seperator);

                sqlBuilder.AppendLine();
            }

            sqlBuilder.DecreaseIndent();
        }

        private void AddValuesFromQuery(IInsertQueryValuesSource source, IPipelineStageInput<IInsertQueryContext> input)
            => selectBuilder.Execute(source.Query, input);
    }
}

