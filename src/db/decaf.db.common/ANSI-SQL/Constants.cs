using decaf.db.common.Builders;

namespace decaf.db.common.ANSISQL
{
	public class Constants : IConstants
	{
        public virtual string OpeningParen => "(";

        public virtual string ClosingParen => ")";

        public virtual string And => "and";

        public virtual string Or => "or";

        public virtual string Not => "not";

        public virtual string Like => "like";

        public virtual string OrderBy => "order by";

        public virtual string GroupBy => "group by";

        public virtual string ValueQuote => "'";

        public virtual string ColumnQuote => "\"";

        public virtual string Seperator => ",";

        public virtual string Ascending => "asc";

        public virtual string Descending => "desc";

        public virtual string Comment => "--";

        public virtual string Join => "join";

        public virtual string On => "on";

        public virtual string From => "from";

        public virtual string Where => "where";

        public virtual string Returning => "returning";

        public virtual string Values => "values";

        public virtual string Limit => "limit";
        
        public virtual string ParameterPrefix => "@";
        
        public virtual string EndStatement => ";";
        
        public virtual string Cascade => "cascade";
        
        public virtual string QuoteFormat => "{0}{1}{0}";
        public string True => "1";
        public string False => "0";
	}
}

