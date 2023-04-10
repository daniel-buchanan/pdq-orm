﻿using System.Data;
using pdq.common;
using pdq.common.Options;
using pdq.db.common;

namespace pdq.npgsql
{
	public class NpgsqlOptionsBuilder
		: OptionsBuilder<NpgsqlOptions>,
		INpgsqlOptionsBuilder
	{
        /// <inheritdoc/>
        public INpgsqlOptionsBuilder SetIsolationLevel(IsolationLevel level)
            => ConfigureProperty(nameof(NpgsqlOptions.TransactionIsolationLevel), level);

        /// <inheritdoc/>
        public INpgsqlOptionsBuilder UseQuotedIdentifiers()
			=> ConfigureProperty(nameof(NpgsqlOptions.QuotedIdentifiers), true);

        /// <inheritdoc/>
        public INpgsqlOptionsBuilder WithConnectionDetails(IConnectionDetails connectionDetails)
            => ConfigureProperty(nameof(NpgsqlOptions.ConnectionDetails), connectionDetails);

        private new INpgsqlOptionsBuilder ConfigureProperty<TValue>(string name, TValue value)
        {
            base.ConfigureProperty(name, value);
            return this;
        }
    }
}

