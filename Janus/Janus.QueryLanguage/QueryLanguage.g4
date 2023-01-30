grammar QueryLanguage;
/* LEXER RULES */

/* keywords */
SELECT_KW   :'SELECT';
FROM_KW     :'FROM';
WHERE_KW    :'WHERE';
JOIN_KW     :'JOIN';
ON_KW       :'ON';

/* operators */
/* logical operators */
AND_OP  :'AND';
OR_OP   :'OR';
NOT_OP  :'NOT';

/* comparison operators */
GTE_OP  :'>=';
GT_OP   :'>';
LTE_OP  :'<=';
LT_OP   :'<';
EQ_OP   :'==';
NEQ_OP  :'!=';

/* special operators */
STAR_OP :'*';

/* special chars */
SEMICOLON   :';';
COMMA       :',';
LPAREN      :'(';
RPAREN      :')';
WHITESPACE	:[ \r\t\n]+ -> skip;

/* literals */
DATETIME	: [0-9][0-9]'-'[0-9][0-9]'-'[0-9][0-9][0-9][0-9]'T'[0-9][0-9]':'[0-9][0-9]':'[0-9][0-9];
LONGINT     :'-'?([1-9]+[0-9]*|[0])'L';
INTEGER     :'-'?([1-9]+[0-9]*|[0]);
DECIMAL     :'-'?([1-9]+[0-9]*|[0])('.'[0-9]+);
BOOLEAN     :'TRUE'|'FALSE'|'true'|'false';
STRING      :'"' ('\\""'|.*?) '"';
BINARY      :'0x'([A-F0-9]+|[a-f0-9]+);

/* structure identifiers */
ATTRIBUTE_ID     :[a-zA-Z][a-zA-Z0-9_-]*'.'[a-zA-Z][a-zA-Z0-9_-]*'.'[a-zA-Z][a-zA-Z0-9_-]*'.'[a-zA-Z][a-zA-Z0-9_-]*;
TABLEAU_ID       :[a-zA-Z][a-zA-Z0-9_-]*'.'[a-zA-Z][a-zA-Z0-9_-]*'.'[a-zA-Z][a-zA-Z0-9_-]*;
SCHEMA_ID        :[a-zA-Z][a-zA-Z0-9_-]*'.'[a-zA-Z][a-zA-Z0-9_-]*;
DATASOURCE_ID    :[a-zA-Z][a-zA-Z0-9_-]*;

/* Parser rules */
/* literals */
literal :STRING|DATETIME|INTEGER|DECIMAL|BOOLEAN|BINARY|LONGINT;

/* comparison expressions */
lvalue          :ATTRIBUTE_ID;
rvalue          :literal;
gte_expr        :lvalue GTE_OP rvalue;
gt_expr         :lvalue GT_OP rvalue;
lte_expr        :lvalue LTE_OP rvalue;
lt_expr         :lvalue LT_OP rvalue;
eq_expr         :lvalue EQ_OP rvalue;
neq_expr        :lvalue NEQ_OP rvalue;
comparison_expr 
    : gt_expr
    | gte_expr
    | lte_expr
    | lt_expr
    | eq_expr
    | neq_expr
    ;

/* selection expressions */
/* left recursion can be maintained within just one projection, that's why there are no and_expr, or_expr, not_expr */
selection_expr
    : BOOLEAN
    | comparison_expr
    | NOT_OP LPAREN selection_expr RPAREN
    | selection_expr AND_OP selection_expr
    | selection_expr OR_OP  selection_expr
    | LPAREN selection_expr RPAREN
    ;

projection_expr	:STAR_OP|(ATTRIBUTE_ID (COMMA ATTRIBUTE_ID)*);

/* clauses */
select_clause   :SELECT_KW projection_expr;
from_clause     :FROM_KW TABLEAU_ID (join_clause)*;
join_clause     :JOIN_KW TABLEAU_ID ON_KW ATTRIBUTE_ID EQ_OP ATTRIBUTE_ID;
where_clause    :WHERE_KW selection_expr;

/* query */
query   :select_clause from_clause where_clause? SEMICOLON;
